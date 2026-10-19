using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Yapay zekânın oyuncu klanına kurduğu entrikalar ve karşı casusluk (tasarım 8.4).
    /// Haftalık: yeni entrika olasılığı, casus başının sezme zarı. Günlük: süresi dolan entrikalar sonuçlanır.
    /// Formüller oyuncununkiyle aynı (SchemeMath); entrikacının casus başı klanındaki en yüksek Roguery'li kahraman.
    /// </summary>
    public static class AiIntrigue
    {
        private const int KidnapShare = 30;
        private const int IdentifyAfterSuccess = 40;
        private const int IdentifyAfterFailure = 50;

        // ---- Haftalık ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            DetectPlots(state);

            int setting = SettingsStore.Current.AiIntrigue;
            if (state.AiPlots.Count >= SchemeMath.AiMaxPlots(setting)) return;
            List<Hero> plotters = Plotters(state);
            if (plotters.Count == 0 || state.Roll100() > SchemeMath.AiPlotWeeklyChance(plotters.Count, setting)) return;

            Hero plotter = PickWeighted(state, plotters, h => Weight(state, h));
            Hero target = PickTarget(state);
            if (plotter == null || target == null) return;

            int today = SchemeFactors.Today;
            bool canKidnap = plotter.PartyBelongedTo != null && target != Hero.MainHero;
            bool canMurder = SchemeFactors.CanBeMurdered(target);
            if (!canKidnap && !canMurder) return;
            bool kidnap = canKidnap && (!canMurder || state.Roll100() <= KidnapShare);
            state.AiPlots.Add(new AiPlot
            {
                Id = state.NextPlotId++,
                PlotterId = plotter.StringId,
                TargetId = target.StringId,
                Kind = kidnap ? SchemeKind.Kidnap : SchemeKind.Murder,
                StartDay = today,
                ResolveDay = today + SchemeMath.AiPlotMinDays + SchemeRandom.Pick(state.Seed, ref state.Rolls, SchemeMath.AiPlotExtraDays + 1),
                Seed = MBRandom.RandomInt(),
            });
        }

        private static void DetectPlots(IntrigueState state)
        {
            Hero spymaster = SchemeFactors.Spymaster(state);
            bool working = spymaster != null && SchemeFactors.SpymasterBusyReason(spymaster) == null;
            int chance = MissionRules.Detection(working, SchemeFactors.Roguery(spymaster), state.Mission);
            foreach (AiPlot plot in state.AiPlots.Where(p => !p.Detected))
            {
                if (plot.Roll100() > chance) continue;
                plot.Detected = true;
                Hero plotter = Hero.Find(plot.PlotterId);
                if (plotter != null && plotter.IsAlive) Expose(state, plotter, false);
            }
        }

        /// <summary>Entrikacıyı öğrendin: hakkında rapor ve "suikast planı" sırrı. Karşı hamleler (suikast, şantaj) böylece açılır.</summary>
        private static void Expose(IntrigueState state, Hero plotter, bool afterTheFact)
        {
            int today = SchemeFactors.Today;
            state.Reports.RemoveAll(r => r.TargetId == plotter.StringId);
            state.Reports.Add(SchemeOutcomes.CreateReport(plotter, today));
            if (!state.Secrets.Any(s => s.TargetId == plotter.StringId && s.Kind == SecretKind.MurderPlot && !s.Exposed))
                state.Secrets.Add(new Secret { TargetId = plotter.StringId, Kind = SecretKind.MurderPlot, Day = today });
            state.AddLog(today, (afterTheFact ? "Saldırının arkasındaki isim: " : "Sana karşı entrika sezildi: ") + plotter.Name);
        }

        /// <summary>Oyuncuya nefret duyan klan liderleri ve kan davalılar.</summary>
        private static List<Hero> Plotters(IntrigueState state)
        {
            int today = SchemeFactors.Today;
            var busy = new HashSet<string>(state.AiPlots.Select(p => p.PlotterId));
            return Hero.AllAliveHeroes
                .Where(h => h.IsLord && !h.IsChild && !h.IsPrisoner && h.Clan != null && h.Clan != Clan.PlayerClan && !busy.Contains(h.StringId)
                    && state.ActiveHook(h.StringId, today) == null
                    && ((h.IsClanLeader && Hero.MainHero.GetRelation(h) <= SchemeMath.HatredThreshold) || state.HasFeud(h.StringId, today)))
                .ToList();
        }

        private static int Weight(IntrigueState state, Hero h) =>
            Math.Max(1, 2 + (state.HasFeud(h.StringId, SchemeFactors.Today) ? 4 : 0)
                + (h.GetTraitLevel(DefaultTraits.Calculating) >= 1 ? 2 : 0) - (h.GetTraitLevel(DefaultTraits.Honor) >= 1 ? 1 : 0));

        /// <summary>Oyuncu klanının yetişkin üyeleri; oyuncunun kendisi yalnızca ayar açıksa.</summary>
        private static Hero PickTarget(IntrigueState state)
        {
            var targeted = new HashSet<string>(state.AiPlots.Select(p => p.TargetId));
            List<Hero> targets = SchemeFactors.SpymasterCandidates()
                .Where(h => !targeted.Contains(h.StringId) && !h.IsPrisoner)
                .ToList();
            if (SettingsStore.Current.PlayerCanBeTargeted && !targeted.Contains(Hero.MainHero.StringId) && !Hero.MainHero.IsPrisoner)
                targets.Add(Hero.MainHero);
            return PickWeighted(state, targets, _ => 1);
        }

        private static Hero PickWeighted(IntrigueState state, List<Hero> heroes, Func<Hero, int> weight)
        {
            int total = heroes.Sum(weight);
            if (total <= 0) return null;
            int pick = SchemeRandom.Pick(state.Seed, ref state.Rolls, total);
            foreach (Hero hero in heroes)
            {
                pick -= weight(hero);
                if (pick < 0) return hero;
            }
            return null;
        }

        // ---- Günlük ----

        public static void Daily()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            int today = SchemeFactors.Today;
            foreach (AiPlot plot in state.AiPlots.ToList())
            {
                Hero plotter = Hero.Find(plot.PlotterId);
                Hero target = Hero.Find(plot.TargetId);
                string gone = GoneReason(plotter, target);
                if (gone != null)
                {
                    state.AiPlots.Remove(plot);
                    if (plot.Detected) SchemeOutcomes.Notify(state, "Sana karşı entrika dağıldı", gone + ".", null);
                    continue;
                }
                if (today < plot.ResolveDay) continue;
                if (SchemeFactors.InBattle(target) || (plot.Kind == SchemeKind.Kidnap && target.IsPrisoner))
                {
                    plot.ResolveDay = today + 3;
                    continue;
                }
                state.AiPlots.Remove(plot);
                Resolve(state, plot, plotter, target);
            }
        }

        private static string GoneReason(Hero plotter, Hero target)
        {
            if (plotter == null || !plotter.IsAlive) return "Entrikacı artık yaşamıyor";
            if (plotter.Clan == Clan.PlayerClan) return plotter.Name + " artık senin klanında";
            if (target == null || !target.IsAlive) return "Hedef artık yaşamıyor";
            if (target.Clan != Clan.PlayerClan) return target.Name + " artık senin klanında değil";
            if (target == Hero.MainHero && !SettingsStore.Current.PlayerCanBeTargeted) return "Ayarlar gereği sana dokunulmuyor";
            return null;
        }

        private static void Resolve(IntrigueState state, AiPlot plot, Hero plotter, Hero target)
        {
            int chance = Math.Max(5, SchemeMath.Success(plot.Kind, Inputs(state, plotter, target)) - (plot.Protected ? SchemeMath.ProtectionBonus : 0));
            bool kidnap = plot.Kind == SchemeKind.Kidnap;
            // Partisi dağılmış entrikacı kaçıramaz; cinayet kapalı kampanyada öldüremez. İkisi de başarısızlık sayılır, asla öbürüne dönmez.
            bool possible = kidnap ? plotter.PartyBelongedTo != null : SchemeFactors.CanBeMurdered(target);
            bool success = possible && plot.Roll100() <= chance;
            bool known = plot.Detected || plot.Roll100() <= (success ? IdentifyAfterSuccess : IdentifyAfterFailure);
            if (known && !plot.Detected) Expose(state, plotter, true);
            string who = known ? "\n\nArkasında " + plotter.Name + " (" + SchemeFactors.Affiliation(plotter) + ") var. Hakkında rapor ve \"suikast planı\" sırrı artık elinde."
                : "\n\nKimin yaptığı bilinmiyor.";
            string targetName = target.Name.ToString();
            int today = SchemeFactors.Today;

            if (!success)
            {
                SchemeOutcomes.Notify(state, (kidnap ? "Kaçırma" : "Suikast") + " girişimi püskürtüldü",
                    Tr.Dative(targetName) + " yönelik bir " + (kidnap ? "kaçırma" : "suikast") + " girişimi son anda boşa çıktı" + (plot.Protected ? " (tuttuğun korumalar sayesinde)" : "") + "." + who,
                    target, CardImages.Caught);
                state.AddLog(today, "Püskürtülen " + (kidnap ? "kaçırma" : "suikast") + ": " + targetName);
                return;
            }

            if (kidnap)
            {
                TakePrisonerAction.Apply(plotter.PartyBelongedTo.Party, target);
                SchemeOutcomes.Notify(state, targetName + " kaçırıldı!",
                    targetName + " kaçırıldı ve şimdi " + Tr.Genitive(plotter.Name.ToString()) + " partisinde esir. Fidye ya da kurtarma senin elinde.", target,
                    CardImages.Kidnap);
                state.AddLog(today, targetName + " kaçırıldı (" + plotter.Name + ")");
                return;
            }

            KillCharacterAction.ApplyByMurder(target, null, true);
            SchemeOutcomes.Notify(state, targetName + " öldürüldü", targetName + " bir suikaste kurban gitti." + who, null, CardImages.MurderDagger);
            state.AddLog(today, targetName + " suikaste kurban gitti" + (known ? " (" + plotter.Name + ")" : ""));
        }

        /// <summary>Yapay zekânın gözünden girdiler: tüm aşamaları beklemiş, hedefi tanıyor; oyuncunun zorluk ayarı tersine işler.</summary>
        private static SchemeInputs Inputs(IntrigueState state, Hero plotter, Hero target)
        {
            Hero agent = plotter.Clan.Heroes.Where(h => h.IsAlive && !h.IsChild).OrderByDescending(SchemeFactors.Roguery).FirstOrDefault() ?? plotter;
            return new SchemeInputs
            {
                Roguery = SchemeFactors.Roguery(agent),
                Calculating = agent.GetTraitLevel(DefaultTraits.Calculating),
                CompletedPhases = 5,
                HasReport = true,
                Difficulty = SchemeMath.HardDifficulty - SettingsStore.Current.IntrigueDifficulty,
                TargetRoguery = target.GetSkillValue(DefaultSkills.Roguery),
                TargetCalculating = target.GetTraitLevel(DefaultTraits.Calculating),
                TargetClanTier = Clan.PlayerClan.Tier,
                TargetIsClanLeader = target.IsClanLeader,
                TargetIsRuler = target.IsKingdomLeader,
                TargetInOwnSettlement = target.CurrentSettlement != null && target.CurrentSettlement.OwnerClan == Clan.PlayerClan,
                TargetInArmy = target.PartyBelongedTo?.Army != null,
            };
        }

        // ---- Oyuncunun karşılığı ----

        public static void ExposePlot(IntrigueState state, AiPlot plot)
        {
            Hero plotter = Hero.Find(plot.PlotterId);
            state.AiPlots.Remove(plot);
            if (plotter == null || !plotter.IsAlive) return;
            Hero ruler = plotter.Clan?.Kingdom?.Leader;
            if (ruler != null && ruler != plotter) ChangeRelationAction.ApplyRelationChangeBetweenHeroes(plotter, ruler, -20, false);
            SchemeOutcomes.Relate(plotter, -10);
            foreach (Secret secret in state.Secrets.Where(s => s.TargetId == plotter.StringId && s.Kind == SecretKind.MurderPlot)) secret.Exposed = true;
            SchemeOutcomes.Notify(state, "Entrika ifşa edildi: " + plotter.Name,
                plotter.Name + " planından vazgeçmek zorunda kaldı; herkes ne yapmaya çalıştığını konuşuyor.\n\nSonuç: "
                + (ruler != null && ruler != plotter ? "hükümdarı " + ruler.Name + " ile ilişkisi -20, " : "") + "seninle ilişkisi -10.", plotter,
                CardImages.Exposed);
            state.AddLog(SchemeFactors.Today, "Entrika ifşa edildi: " + plotter.Name);
        }

        public static void Protect(IntrigueState state, AiPlot plot)
        {
            if (!state.AiPlots.Contains(plot)) throw new InvalidOperationException("Bu entrika artık sürmüyor");
            if (plot.Protected) throw new InvalidOperationException("Zaten koruma tuttun");
            SchemeFactors.Pay(SchemeMath.ProtectionCost);
            plot.Protected = true;
            Hero target = Hero.Find(plot.TargetId);
            state.AddLog(SchemeFactors.Today, "Koruma tutuldu: " + target?.Name);
            Menu.MenuUI.Success(target?.Name + " için koruma tutuldu (başarı şansları -" + SchemeMath.ProtectionBonus + ")");
        }

        /// <summary>Entrikacıya bonuslu suikast başlatır (sezme ona rapor verdiği için hedef listesine girer).</summary>
        public static void CounterMurder(IntrigueState state, AiPlot plot)
        {
            Hero plotter = Hero.Find(plot.PlotterId) ?? throw new InvalidOperationException("Entrikacı bulunamadı");
            SchemeEngine.Start(SchemeKind.Murder, plotter);
            Scheme scheme = state.FindScheme(plotter.StringId);
            if (scheme != null) scheme.EventSuccess += 10;
        }

        public static string CounterMurderBlocked(IntrigueState state, AiPlot plot)
        {
            Hero plotter = Hero.Find(plot.PlotterId);
            return SchemeEngine.CannotStartReason(state) ?? SchemeFactors.BlockedReason(state, plotter, SchemeKind.Murder);
        }

        public static string PlotLine(AiPlot plot)
        {
            Hero plotter = Hero.Find(plot.PlotterId);
            Hero target = Hero.Find(plot.TargetId);
            return plotter?.Name + " -> " + target?.Name + " (" + (plot.Kind == SchemeKind.Kidnap ? "kaçırma" : "suikast") + ")"
                + (plot.Protected ? " [korumalı]" : "");
        }
    }
}
