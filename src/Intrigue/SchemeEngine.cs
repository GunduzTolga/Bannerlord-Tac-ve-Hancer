using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Entrikaların yaşam döngüsü (tasarım 5.2): başlat, günlük ilerleme, aşama, harekete geç, iptal.
    /// Oyuncu kararları (olay, casusluk seçimi, son aşama) durumda bayrak olarak bekler; IntriguePopups onları sorar.
    /// </summary>
    public static class SchemeEngine
    {
        private const int PhaseXp = 200;
        public const int EventChance = 50;

        public static IntrigueState State =>
            IntrigueBehavior.Current?.State ?? throw new InvalidOperationException("Entrika sistemi bu kampanyada yüklenmedi");

        // ---- Casus başı ----

        public static void AssignSpymaster(Hero hero)
        {
            IntrigueState state = State;
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            if (busy != null) throw new InvalidOperationException(hero.Name + " şu an " + busy + "; casus başı olamaz");
            state.SpymasterId = hero.StringId;
            state.AddLog(SchemeFactors.Today, hero.Name + " casus başı oldu");
            MenuUI.Success(hero.Name + " artık casus başın");
        }

        public static void DismissSpymaster()
        {
            IntrigueState state = State;
            if (state.Schemes.Count > 0) throw new InvalidOperationException("Önce süren entrikaları bitir ya da iptal et");
            state.SpymasterId = null;
            MenuUI.Success("Casus başı görevden alındı");
        }

        // ---- Başlatma ----

        /// <summary>null: yeni entrika başlatılabilir; değilse neden.</summary>
        public static string CannotStartReason(IntrigueState state)
        {
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (spymaster == null) return "Önce bir casus başı ata";
            string busy = SchemeFactors.SpymasterBusyReason(spymaster);
            if (busy != null) return "Casus başın şu an " + busy;
            int max = SchemeMath.MaxActiveSchemes(SchemeFactors.Roguery(spymaster));
            if (state.Schemes.Count >= max)
                return "Aynı anda en fazla " + max + " entrika yürütebilirsin (casus başının Roguery'si 125 ve 225'te artar)";
            return null;
        }

        public static void Start(SchemeKind kind, Hero target)
        {
            IntrigueState state = State;
            string reason = CannotStartReason(state) ?? SchemeFactors.BlockedReason(state, target, kind);
            if (reason != null) throw new InvalidOperationException(reason);

            Hero spymaster = SchemeFactors.Spymaster(state);
            int cost = SchemeFactors.StartCost(kind, target);
            SchemeFactors.Pay(cost);

            int today = SchemeFactors.Today;
            state.Schemes.Add(new Scheme
            {
                Id = state.NextSchemeId++,
                Kind = kind,
                TargetId = target.StringId,
                StartDay = today,
                PhaseDays = SchemeMath.PhaseDays(kind, SchemeFactors.Roguery(spymaster)),
                StartCost = cost,
                Seed = MBRandom.RandomInt(),
            });
            state.AddLog(today, SchemeRules.Name(kind) + " başladı: " + target.Name);
            MenuUI.Success(SchemeRules.Name(kind) + " başladı: " + target.Name + " (" + SchemeFactors.Money(cost) + ")");
        }

        // ---- Günlük ilerleme ----

        public static void DailyTick()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            state.RemoveExpired(SchemeFactors.Today);

            Hero spymaster = CheckSpymaster(state);
            foreach (Scheme scheme in state.Schemes.ToList())
            {
                Hero target = Hero.Find(scheme.TargetId);
                string gone = TargetGoneReason(target);
                if (gone != null)
                {
                    Close(state, scheme, SchemeRules.Name(scheme.Kind) + " sona erdi: " + gone, target);
                    continue;
                }
                if (spymaster == null || SchemeFactors.SpymasterBusyReason(spymaster) != null || IsWaiting(scheme)) continue;
                if (++scheme.DaysIntoPhase >= scheme.PhaseDays) CompletePhase(state, scheme, spymaster, target, false);
            }
        }

        /// <summary>Oyuncudan karar bekliyor mu (o zaman sayaç durur).</summary>
        public static bool IsWaiting(Scheme scheme) =>
            scheme.PendingEvent > 0 || scheme.AwaitingChoice
            || (SchemeRules.IsStrike(scheme.Kind) && scheme.Phase >= SchemeRules.MaxPhases(scheme.Kind));

        private static Hero CheckSpymaster(IntrigueState state)
        {
            if (state.SpymasterId == null) return null;
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (spymaster != null) return spymaster;

            Hero lost = Hero.Find(state.SpymasterId);
            state.SpymasterId = null;
            if (state.Schemes.Count == 0) return null;
            state.Schemes.Clear();
            SchemeOutcomes.Notify(state, "Casus başın kayıp",
                (lost != null ? lost.Name + (lost.IsAlive ? " artık klanında değil" : " öldü") : "Casus başın ortadan kayboldu")
                + ". Süren tüm entrikalar dağıldı; harcanan para geri gelmez.", lost);
            state.AddLog(SchemeFactors.Today, "Casus başı kaybedildi; entrikalar dağıldı");
            return null;
        }

        private static string TargetGoneReason(Hero target)
        {
            if (target == null || !target.IsAlive) return "hedef öldü";
            if (target.Clan == Clan.PlayerClan) return "hedef artık senin klanında";
            return null;
        }

        /// <param name="cheat">Hile: keşif zarı ve olay atlanır.</param>
        /// <returns>Entrika sürüyorsa true.</returns>
        private static bool CompletePhase(IntrigueState state, Scheme scheme, Hero spymaster, Hero target, bool cheat)
        {
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
            if (!cheat && !CheatState.IntrigueNeverDiscovered && scheme.Roll100() <= SchemeMath.Discovery(scheme.Kind, inputs))
            {
                SchemeOutcomes.Discovered(state, scheme, target);
                return false;
            }

            scheme.Phase++;
            scheme.DaysIntoPhase = 0;
            scheme.PhaseDays = SchemeMath.PhaseDays(scheme.Kind, inputs.Roguery);
            spymaster.AddSkillXp(DefaultSkills.Roguery, PhaseXp);

            int min = SchemeRules.MinPhases(scheme.Kind), max = SchemeRules.MaxPhases(scheme.Kind);
            if (!SchemeRules.IsStrike(scheme.Kind))
            {
                if (scheme.Phase >= max)
                {
                    Resolve(state, scheme, spymaster, target);
                    return false;
                }
                scheme.AwaitingChoice = true;
            }
            else
            {
                MenuUI.Info(SchemeRules.Name(scheme.Kind) + " (" + target.Name + "): aşama " + scheme.Phase + "/" + max + " tamamlandı"
                    + (scheme.Phase == min ? " - artık harekete geçebilirsin (Entrika menüsü)" : ""));
                if (scheme.Phase >= max) scheme.AskedToExecute = false;
            }

            if (!cheat && scheme.Phase < max && scheme.Roll100() <= EventChance)
                scheme.PendingEvent = IntrigueEvents.Pick(scheme, target, spymaster);
            return true;
        }

        // ---- Casusluk ve şantaj: son aşamada kendiliğinden sonuçlanır ----

        /// <param name="keepGoing">true: bir aşama daha (casuslukta sır şansı +20, şantajda başarı +15); false: şimdi sonuçlandır.</param>
        public static void ChooseContinuation(Scheme scheme, bool keepGoing)
        {
            IntrigueState state = State;
            scheme.AwaitingChoice = false;
            if (keepGoing) return;
            Hero target = Hero.Find(scheme.TargetId);
            if (TargetGoneReason(target) != null) return;
            Resolve(state, scheme, SchemeFactors.Spymaster(state), target);
        }

        private static void Resolve(IntrigueState state, Scheme scheme, Hero spymaster, Hero target)
        {
            state.Schemes.Remove(scheme);
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
            bool success = scheme.Roll100() <= (CheatState.IntrigueAlwaysSucceed ? 100 : SchemeMath.Success(scheme.Kind, inputs));
            if (scheme.Kind == SchemeKind.Blackmail)
            {
                if (success) SchemeOutcomes.BlackmailSucceeded(state, scheme, spymaster, target);
                else SchemeOutcomes.BlackmailFailed(state, target);
            }
            else if (success) SchemeOutcomes.SpySucceeded(state, scheme, spymaster, target);
            else SchemeOutcomes.SpyFailed(state, target);
        }

        // ---- Suikast ve kaçırma: harekete geç ----

        public sealed class BribeChoice
        {
            public int Level;
            public int Cost;
            public int Chance;
            public bool Affordable;

            public string Label =>
                (Level == 0 ? "Rüşvetsiz" : (Level == 1 ? "Küçük" : Level == 2 ? "Orta" : "Büyük") + " rüşvetle (" + SchemeFactors.Money(Cost) + ")")
                + " - başarı %" + Chance;
        }

        public static List<BribeChoice> BribeChoices(Scheme scheme)
        {
            IntrigueState state = State;
            Hero target = Hero.Find(scheme.TargetId);
            Hero spymaster = SchemeFactors.Spymaster(state);
            var choices = new List<BribeChoice>();
            if (target == null) return choices;
            for (int level = 0; level <= SchemeMath.MaxBribeLevel; level++)
            {
                int cost = SchemeMath.BribeCost(scheme.StartCost, level);
                choices.Add(new BribeChoice
                {
                    Level = level,
                    Cost = cost,
                    Chance = SchemeMath.Success(scheme.Kind, SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme, level)),
                    Affordable = SchemeFactors.CanAfford(cost),
                });
            }
            return choices;
        }

        /// <summary>null: harekete geçilebilir; değilse neden.</summary>
        public static string CannotExecuteReason(Scheme scheme, bool ignoreMinPhases = false)
        {
            if (!SchemeRules.IsStrike(scheme.Kind)) return "Yalnızca suikast ve kaçırmada harekete geçilir";
            if (!ignoreMinPhases && scheme.Phase < SchemeRules.MinPhases(scheme.Kind))
                return "En az " + SchemeRules.MinPhases(scheme.Kind) + " aşama tamamlanmalı";
            Hero spymaster = SchemeFactors.Spymaster(State);
            if (spymaster == null) return "Casus başın yok";
            string busy = SchemeFactors.SpymasterBusyReason(spymaster);
            if (busy != null) return "Casus başın şu an " + busy;
            Hero target = Hero.Find(scheme.TargetId);
            if (TargetGoneReason(target) != null) return TargetGoneReason(target);
            if (SchemeFactors.InBattle(target)) return target.Name + " şu an bir savaşın ya da kuşatmanın içinde; bitince tekrar dene";
            if (scheme.Kind == SchemeKind.Kidnap && target.IsPrisoner) return target.Name + " zaten birinin esiri";
            return null;
        }

        /// <param name="extraSuccess">Olaydan gelen anlık bonus (muhafız değişimi).</param>
        public static void Execute(Scheme scheme, int bribeLevel, int extraSuccess = 0, bool ignoreMinPhases = false)
        {
            IntrigueState state = State;
            string reason = CannotExecuteReason(scheme, ignoreMinPhases);
            if (reason != null) throw new InvalidOperationException(reason);

            Hero target = Hero.Find(scheme.TargetId);
            Hero spymaster = SchemeFactors.Spymaster(state);
            SchemeFactors.Pay(SchemeMath.BribeCost(scheme.StartCost, bribeLevel));

            scheme.EventSuccess += extraSuccess;
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme, bribeLevel);
            int chance = CheatState.IntrigueAlwaysSucceed ? 100 : SchemeMath.Success(scheme.Kind, inputs);
            int discovery = SchemeMath.Discovery(scheme.Kind, inputs);
            state.Schemes.Remove(scheme);

            bool success = scheme.Roll100() <= chance;
            if (scheme.Kind == SchemeKind.Kidnap)
            {
                if (success) SchemeOutcomes.KidnapSucceeded(state, scheme, spymaster, target, discovery);
                else SchemeOutcomes.StrikeFailed(state, scheme, spymaster, target, inputs.Roguery);
            }
            else if (success) SchemeOutcomes.MurderSucceeded(state, scheme, spymaster, target, discovery, inputs.TargetIsPlayersCaptive);
            else SchemeOutcomes.StrikeFailed(state, scheme, spymaster, target, inputs.Roguery);
        }

        // ---- İptal ve kapanış ----

        public static void Cancel(Scheme scheme)
        {
            IntrigueState state = State;
            Hero target = Hero.Find(scheme.TargetId);
            state.Schemes.Remove(scheme);
            state.AddLog(SchemeFactors.Today, SchemeRules.Name(scheme.Kind) + " iptal edildi: " + target?.Name);
            MenuUI.Success(SchemeRules.Name(scheme.Kind) + " iptal edildi (harcanan para geri gelmez)");
        }

        /// <summary>Entrikayı sonuçsuz kapatır ve oyuncuya bildirir.</summary>
        public static void Close(IntrigueState state, Scheme scheme, string text, Hero target)
        {
            state.Schemes.Remove(scheme);
            SchemeOutcomes.Notify(state, SchemeRules.Name(scheme.Kind) + " sona erdi", text, target != null && target.IsAlive ? target : null);
            state.AddLog(SchemeFactors.Today, text);
        }

        // ---- Hile ----

        /// <summary>Süren entrikaların aşamalarını keşif zarı ve olay olmadan sona kadar ilerletir.</summary>
        public static void CompleteAllPhases()
        {
            IntrigueState state = State;
            Hero spymaster = SchemeFactors.Spymaster(state) ?? throw new InvalidOperationException("Önce bir casus başı ata");
            if (state.Schemes.Count == 0) throw new InvalidOperationException("Süren bir entrika yok");

            foreach (Scheme scheme in state.Schemes.ToList())
            {
                Hero target = Hero.Find(scheme.TargetId);
                if (TargetGoneReason(target) != null) continue;
                while (true)
                {
                    scheme.AwaitingChoice = false;
                    if (IsWaiting(scheme) || !CompletePhase(state, scheme, spymaster, target, true)) break;
                }
            }
            MenuUI.Success("Entrikaların aşamaları tamamlandı");
        }
    }
}
