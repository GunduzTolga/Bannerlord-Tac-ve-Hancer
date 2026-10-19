using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Casusluğu kendin yap (P2.11, tasarım: docs/superpowers/specs/2026-10-19-p2-kendin-takip.md). Hedefle aynı şehirdeysen onu bizzat
    /// takip edersin: iki sokak sahnesi ve bir son sahne, her biri resimli kartla. Şüphe 100'e varırsa bir muhafız seni fark eder: kaç,
    /// rol yap ya da sustur. Takip bitince rapor ve sır şansı; aynı hedefe süren casusluk entrikası bir aşama ilerler. Ayar açıksa
    /// (deneysel) kartlar yerine oyunun şehirde gizli takip görevi açılır.
    /// </summary>
    public static class Tailing
    {
        private static int Today => SchemeFactors.Today;

        /// <summary>Sahnede takip açılırken hedef; oyunun görev kancası (SubModule.OnMissionBehaviorInitialize) ölçümü buna ekler.</summary>
        private static string _pendingSceneTarget;
        private static int _pendingSince;
        /// <summary>Görev bu sürede başlamazsa bekleyen hedef unutulur (sonraki alakasız bir göreve eklenmesin).</summary>
        private const int PendingSceneMs = 15000;

        /// <summary>Kartlar arasında taşınan takip durumu. Kayda yazılmaz: takip tek oturuşta biter.</summary>
        private sealed class Run
        {
            public Hero Target;
            public Settlement Town;
            public List<TailScene> Scenes;
            public int Suspicion;
            public int Clues;
            public bool Night;
            public bool Alerted;
            public int NetworkLevel;
            public readonly List<string> Steps = new List<string>();
        }

        public static void RegisterMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption("town", "tvh_tail", "Birini gizlice takip et",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    Settlement town = Settlement.CurrentSettlement;
                    return town != null && town.IsTown && Candidates(town).Count > 0;
                },
                _ => MenuUI.SafeRun("Takip", () => ShowPick(Settlement.CurrentSettlement, null)),
                false, 7);
        }

        /// <summary>Şehirdeki, takip edilebilecek lordlar (sende değil, esir değil, yetişkin).</summary>
        public static List<Hero> Candidates(Settlement town) =>
            town.HeroesWithoutParty.Concat(town.Parties.Where(p => p.LeaderHero != null).Select(p => p.LeaderHero)).Distinct()
                .Where(h => IsTailable(h) && !h.IsPrisoner).ToList();

        private static bool IsTailable(Hero h) =>
            h != null && h.IsAlive && h.IsLord && !h.IsChild && h != Hero.MainHero && h.Clan != Clan.PlayerClan;

        /// <summary>Şehirdeki lordlardan birini seç ve takibi başlat.</summary>
        public static void ShowPick(Settlement town, Action onBack)
        {
            IntrigueState state = SchemeEngine.State;
            Picker.Pick("Kimi takip edeceksin?", Candidates(town),
                h => SchemeFactors.TargetLabel(h) + (Blocked(state, h) is string why ? "  [" + why + "]" : ""),
                h => MenuUI.SafeRun("Takip", () => Start(SchemeEngine.State, h)), onBack ?? (() => { }));
        }

        /// <summary>null: şimdi takip edilebilir; değilse neden.</summary>
        public static string Blocked(IntrigueState state, Hero target)
        {
            if (Hero.MainHero.IsPrisoner) return "Esirken olmaz";
            if (target == null || !target.IsAlive) return "Hedef hayatta değil";
            if (!IsTailable(target)) return "Yalnızca başka klanlardan yetişkin lordlar takip edilir";
            if (target.IsPrisoner) return target.Name + " esir";
            Settlement here = Settlement.CurrentSettlement;
            if (here == null || !here.IsTown || SchemeFactors.TargetSettlement(target) != here)
                return "Onunla aynı şehirde olmalısın (" + target.Name + ": " + Cheats.MapInfo.Where(target) + ")";
            if (here.SiegeEvent != null) return "Kuşatma altındaki şehirde olmaz";
            if (state.TailUntil.TryGetValue(target.StringId, out int until) && Today < until)
                return "Onu yakın zamanda takip ettin; " + (until - Today) + " gün sonra";
            return null;
        }

        public static void Start(IntrigueState state, Hero target)
        {
            string blocked = Blocked(state, target);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Settlement town = Settlement.CurrentSettlement;
            state.TailUntil[target.StringId] = Today + TailRules.CooldownDays;
            UI.IntrigueScreen.Close();
            if (SettingsStore.Current.TailInScene && TryOpenScene(state, target, town)) return;

            bool alerted = state.IsAlerted(target.StringId, Today);
            var run = new Run
            {
                Target = target,
                Town = town,
                Scenes = TailRules.Draw(count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count)),
                Suspicion = alerted ? TailRules.AlertedStart : 0,
                Night = CampaignTime.Now.IsNightTime,
                Alerted = alerted,
                NetworkLevel = state.Network(town.StringId)?.Level ?? 0,
            };
            ShowScene(state, run, 0);
        }

        // ---- Kartlar ----

        private static void ShowScene(IntrigueState state, Run run, int index)
        {
            if (index >= run.Scenes.Count)
            {
                Finish(state, run, TailRules.SecretChance(run.Clues), "Takibi sonuna kadar götürdün; kimse seni fark etmedi.");
                return;
            }
            TailScene scene = run.Scenes[index];
            string name = run.Target.Name.ToString();
            int targetRoguery = run.Target.GetSkillValue(DefaultSkills.Roguery);
            List<UI.EventCards.CardOption> options = scene.Options.Select(option => option.Stop
                ? new UI.EventCards.CardOption
                {
                    Label = option.Label + " (takibi bitir)",
                    Apply = () => MenuUI.Defer(() => Finish(state, run, TailRules.SecretChance(run.Clues), "Yeterince gördün ve sessizce çekildin.")),
                }
                : new UI.EventCards.CardOption
                {
                    Label = option.Label + " (" + OptionText(option, run, targetRoguery) + ")",
                    DisabledHint = option.Gold > 0 && !SchemeFactors.CanAfford(option.Gold) ? "Yeterli altının yok" : null,
                    Apply = () => Step(state, run, index, scene, option, targetRoguery),
                }).ToList();
            string status = "Şüphe: " + run.Suspicion + "/" + TailRules.MaxSuspicion + " · İpucu: " + run.Clues
                + (run.Night ? "\nGece: karanlık senden yana." : "")
                + (run.NetworkLevel > 0 ? "\nGölge ağının adamları gözcülük ediyor." : "")
                + (run.Alerted ? "\n" + name + " tetikte: yakın zamanda biri peşine düştü." : "");
            UI.EventCards.Show("TAKİP " + (index + 1) + "/" + run.Scenes.Count, scene.Title + ": " + name,
                scene.Text.Replace("{hedef}", name) + "\n\n" + status,
                run.Night ? CardImages.TailNight : CardImages.Tail, run.Target, options, null, null);
        }

        /// <summary>Düğmede yazan: "100 dinar, şüphe 8-13, ipucu +20, Roguery".</summary>
        private static string OptionText(TailOption option, Run run, int targetRoguery)
        {
            TailRules.SuspicionRange(option.Suspicion, SkillValue(option.Skill), targetRoguery, run.Night, run.NetworkLevel, out int min, out int max);
            var parts = new List<string>();
            if (option.Gold > 0) parts.Add(SchemeFactors.Money(option.Gold));
            parts.Add(max == 0 ? "şüphe yok" : "şüphe " + (min == max ? min.ToString() : min + "-" + max));
            parts.Add("ipucu +" + option.Clues);
            if (option.Skill != TailSkill.None) parts.Add(TailRules.SkillName(option.Skill));
            return string.Join(", ", parts);
        }

        private static void Step(IntrigueState state, Run run, int index, TailScene scene, TailOption option, int targetRoguery)
        {
            SchemeFactors.Pay(option.Gold);
            int gain = TailRules.SuspicionGain(option.Suspicion, SkillValue(option.Skill), targetRoguery, run.Night, run.NetworkLevel, state.Roll100());
            run.Suspicion += gain;
            run.Clues += option.Clues;
            Hero.MainHero.AddSkillXp(Skill(option.Skill) ?? DefaultSkills.Roguery, TailRules.XpPerScene);
            run.Steps.Add(scene.Title + ": " + option.Label.ToLower(Tr.Culture) + " (şüphe +" + gain + ")");
            if (run.Suspicion >= TailRules.MaxSuspicion) MenuUI.Defer(() => ShowSpotted(state, run));
            else MenuUI.Defer(() => ShowScene(state, run, index + 1));
        }

        private static void ShowSpotted(IntrigueState state, Run run)
        {
            Hero me = Hero.MainHero;
            int roguery = me.GetSkillValue(DefaultSkills.Roguery), athletics = me.GetSkillValue(DefaultSkills.Athletics);
            int charm = me.GetSkillValue(DefaultSkills.Charm);
            int flee = TailRules.EscapeChance(TailEscape.Run, roguery, athletics, charm, run.Night, run.Alerted);
            int bluff = TailRules.EscapeChance(TailEscape.Bluff, roguery, athletics, charm, run.Night, run.Alerted);
            int cost = TailRules.BribeCost(SchemeFactors.TargetTier(run.Target));
            string name = run.Target.Name.ToString();
            bool foreign = Foreign(run.Town);
            var options = new List<UI.EventCards.CardOption>
            {
                new UI.EventCards.CardOption
                {
                    Label = "Kalabalığa dalıp kaç (%" + flee + ", Roguery ve Athletics)",
                    Apply = () => Escape(state, run, TailEscape.Run, flee),
                },
                new UI.EventCards.CardOption
                {
                    Label = "Yolunu kaybetmiş bir yolcu gibi yol sor (%" + bluff + ", Charm)",
                    Apply = () => Escape(state, run, TailEscape.Bluff, bluff),
                },
                new UI.EventCards.CardOption
                {
                    Label = "Muhafızın avucuna kese bırak (" + SchemeFactors.Money(cost) + ")",
                    DisabledHint = SchemeFactors.CanAfford(cost) ? null : "Yeterli altının yok",
                    Apply = () =>
                    {
                        SchemeFactors.Pay(cost);
                        Escape(state, run, TailEscape.Bribe, 100);
                    },
                },
            };
            UI.EventCards.Show("TAKİP · FARK EDİLDİN", "Bir muhafız seni gözüne kestirdi",
                Tr.Genitive(name) + " muhafızlarından biri birkaç kez dönüp sana baktı; şimdi kalabalığı yararak sana doğru geliyor. " + name
                + " henüz bir şey fark etmedi.\n\nYakalanırsan: " + name + " ile ilişki -" + TailRules.CaughtRelation
                + (foreign ? ", bu krallıkta suç puanı +" + TailRules.CaughtCrime : "") + ", " + name + " tetikte; gördüklerin de elinden gider.",
                CardImages.Caught, run.Target, options, null, null);
        }

        private static void Escape(IntrigueState state, Run run, TailEscape escape, int chance)
        {
            if (escape != TailEscape.Bribe && state.Roll100() > chance)
            {
                Caught(state, run, escape);
                return;
            }
            string name = run.Target.Name.ToString();
            string how;
            switch (escape)
            {
                case TailEscape.Run:
                    Alert(state, run.Target);
                    Hero.MainHero.AddSkillXp(DefaultSkills.Athletics, TailRules.XpPerScene);
                    how = "Kalabalığa dalıp ara sokaklarda kayboldun. Muhafız yüzünü göremedi, ama " + name
                        + " artık peşinde biri olduğunu biliyor (" + TailRules.AlertDays + " gün tetikte).";
                    break;
                case TailEscape.Bluff:
                    Hero.MainHero.AddSkillXp(DefaultSkills.Charm, TailRules.XpPerScene);
                    how = "Yolunu kaybetmiş yolcu rolünü iyi oynadın; muhafız sana hanın yolunu tarif edip uzaklaştı.";
                    break;
                default:
                    how = "Muhafız keseyi tarttı, başını salladı ve hiçbir şey görmemiş gibi yürüyüp gitti.";
                    break;
            }
            Finish(state, run, TailRules.SecretChance(run.Clues / 2), how + " Takibi burada bırakmak zorunda kaldın.");
        }

        private static void Caught(IntrigueState state, Run run, TailEscape tried)
        {
            Hero target = run.Target;
            string name = target.Name.ToString();
            var parts = new List<string>();
            SchemeOutcomes.Relate(target, -TailRules.CaughtRelation);
            parts.Add(name + " ile ilişki -" + TailRules.CaughtRelation);
            Alert(state, target);
            parts.Add(name + " " + TailRules.AlertDays + " gün tetikte");
            if (Foreign(run.Town))
            {
                ChangeCrimeRatingAction.Apply(run.Town.MapFaction, TailRules.CaughtCrime, true);
                parts.Add(run.Town.MapFaction.Name + " suç puanı +" + TailRules.CaughtCrime);
            }
            string opening = tried == TailEscape.Run ? "Kaçmaya çalıştın ama muhafız seni bir çıkmaz sokakta yakaladı." : "Muhafız hikâyeni yutmadı.";
            SchemeOutcomes.Notify(state, "Takipte yakalandın: " + name,
                opening + " Seni " + Tr.Dative(name) + " götürdüler; gözlerinin içine bakıp neden peşinde olduğunu sordu. Gördüklerin de elinden gitti."
                + "\n\nSonuç: " + string.Join(", ", parts) + ".",
                target, CardImages.Caught);
            state.AddLog(Today, "Takipte yakalandın: " + name);
        }

        /// <summary>Takibin sonu: ipucu varsa rapor, sır zarı ve süren casusluk entrikasına bir aşama.</summary>
        private static void Finish(IntrigueState state, Run run, int secretChance, string opener) =>
            Finish(state, run.Target, run.Clues, secretChance, opener, run.Steps);

        private static void Finish(IntrigueState state, Hero target, int clues, int secretChance, string opener, List<string> steps)
        {
            string name = target.Name.ToString();
            var lines = new List<string> { opener };
            if (steps.Count > 0) lines.Add("İzlediğin yol:\n" + string.Join("\n", steps.Select(s => "· " + s)));
            string image = CardImages.Spy;
            if (clues <= 0) lines.Add("İşe yarar bir şey görmedin.");
            else
            {
                IntelReport report = SchemeOutcomes.CreateReport(target, Today);
                state.Reports.RemoveAll(r => r.TargetId == target.StringId);
                state.Reports.Add(report);
                lines.Add(SchemeOutcomes.ReportText(report));

                Secret found = null;
                if (target.IsLord && state.Roll100() <= secretChance)
                    found = SchemeOutcomes.FindSecret(state, target, state.Roll100, count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count));
                if (found != null)
                {
                    RomanceSecrets.Record(state, found);
                    image = SchemeOutcomes.SecretImage(found);
                    lines.Add(SchemeOutcomes.SecretFoundLine(found) + "\n(Entrika ekranı > Kişiler'den ifşa edebilir ya da şantaja çevirebilirsin.)");
                }
                else lines.Add("Bir sır yakalayamadın (sır şansı %" + secretChance + ").");

                Scheme scheme = clues >= TailRules.PhaseClues
                    ? state.Schemes.FirstOrDefault(s => s.Kind == SchemeKind.Spy && s.TargetId == target.StringId)
                    : null;
                string advanced = scheme != null ? SchemeEngine.AdvanceByHand(state, scheme) : null;
                if (advanced != null) lines.Add(advanced);
            }
            SchemeOutcomes.Notify(state, "Takip: " + name, string.Join("\n\n", lines), target, image);
            state.AddLog(Today, "Kendin takip ettin: " + name + " (ipucu " + clues + ")");
        }

        private static void Alert(IntrigueState state, Hero target)
        {
            int until = Today + TailRules.AlertDays;
            if (!state.AlertUntil.TryGetValue(target.StringId, out int current) || current < until) state.AlertUntil[target.StringId] = until;
        }

        private static bool Foreign(Settlement town) => town.MapFaction != null && town.MapFaction != Clan.PlayerClan.MapFaction;

        private static SkillObject Skill(TailSkill skill)
        {
            switch (skill)
            {
                case TailSkill.Roguery: return DefaultSkills.Roguery;
                case TailSkill.Charm: return DefaultSkills.Charm;
                case TailSkill.Athletics: return DefaultSkills.Athletics;
                default: return null;
            }
        }

        private static int SkillValue(TailSkill skill) => Skill(skill) is SkillObject s ? Hero.MainHero.GetSkillValue(s) : 0;

        // ---- Sahnede takip (deneysel, ayar: TailInScene) ----

        /// <summary>
        /// Oyunun şehir merkezinde gizli takip görevini hedefle açar. Görevin sonucu bize bildirilmiyor; ölçümü TailMissionLogic yapar
        /// (hedefe uzaklık ve süre). Görev açılamazsa false: kartlarla devam edilir.
        /// </summary>
        private static bool TryOpenScene(IntrigueState state, Hero target, Settlement town)
        {
            try
            {
                Location center = town.LocationComplex?.GetLocationWithId("center")
                    ?? throw new InvalidOperationException("şehir merkezi bulunamadı");
                string scene = town.LocationComplex.GetScene("center", town.Town.GetWallLevel());
                string levels = Campaign.Current.Models.LocationModel.GetCivilianSceneLevel(town);
                _pendingSceneTarget = target.StringId;
                _pendingSince = Environment.TickCount;
                if (SandBoxMissions.OpenTownCenterShadowATargetMission(scene, levels, center, target.CharacterObject, null) == null)
                    throw new InvalidOperationException("görev açılmadı");
                state.AddLog(Today, "Sahnede takip: " + target.Name + ", " + town.Name);
                return true;
            }
            catch (Exception ex)
            {
                _pendingSceneTarget = null;
                Debug.Print("[CheatMenu] shadow mission failed: " + ex);
                MenuUI.Error("Sahnede takip açılamadı (" + ex.Message + "); kartlarla devam ediliyor.");
                return false;
            }
        }

        /// <summary>SubModule.OnMissionBehaviorInitialize: sahnede takip açılıyorsa ölçümü göreve ekler.</summary>
        public static void OnMissionStarting(Mission mission)
        {
            string targetId = _pendingSceneTarget;
            if (targetId == null) return;
            _pendingSceneTarget = null;
            if (Environment.TickCount - _pendingSince > PendingSceneMs) return;
            Hero target = Hero.Find(targetId);
            if (target != null) mission.AddMissionBehavior(new TailMissionLogic(target));
        }

        internal static void SceneEnded(string targetId, TailSceneResult result, float followSeconds)
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            Hero target = Hero.Find(targetId);
            if (state == null || target == null || !target.IsAlive) return;
            string name = target.Name.ToString();
            switch (result)
            {
                case TailSceneResult.NotFound:
                    state.TailUntil.Remove(targetId);
                    SchemeOutcomes.Notify(state, "Sahnede takip: " + name,
                        name + " sahnede görünmedi; oyunun takip görevi onu sahneye getirmedi. Takip sayılmadı.\n\n"
                        + "Kartlarla takip için ayarı kapat: Kurallar > Takibi sahnede dene (deneysel).", target, CardImages.Tail);
                    break;
                case TailSceneResult.Lost:
                    SchemeOutcomes.Notify(state, "Sahnede takip: " + name,
                        "İzini kaybettin (yakından izlediğin süre: " + (int)followSeconds + " saniye; en az " + (int)TailRules.SceneFollowSeconds
                        + " gerekir). Kimse seni fark etmedi.", target, CardImages.Tail);
                    break;
                case TailSceneResult.Spotted:
                    SchemeOutcomes.Relate(target, -TailRules.SceneSpottedRelation);
                    Alert(state, target);
                    SchemeOutcomes.Notify(state, "Sahnede takip: " + name,
                        "Fazla yaklaştın; " + name + " döndü ve seni gördü.\n\nSonuç: ilişki -" + TailRules.SceneSpottedRelation + ", "
                        + TailRules.AlertDays + " gün tetikte.", target, CardImages.Caught);
                    state.AddLog(Today, "Sahnede takipte fark edildin: " + name);
                    break;
                default:
                    int clues = TailRules.SceneClues(followSeconds);
                    Finish(state, target, clues, TailRules.SecretChance(clues),
                        name + " seni fark etmeden " + (int)followSeconds + " saniye izledin.", new List<string>());
                    break;
            }
        }
    }

    /// <summary>Sahnede takip (deneysel): oyuncunun hedefe uzaklığını ölçer; görev kapanınca sonucu kampanyaya bildirir.</summary>
    internal sealed class TailMissionLogic : MissionLogic
    {
        private readonly string _targetId;
        private readonly CharacterObject _character;
        private Agent _agent;
        private bool _seen;
        private bool _reported;
        private float _follow;
        private float _tooClose;

        public TailMissionLogic(Hero target)
        {
            _targetId = target.StringId;
            _character = target.CharacterObject;
        }

        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);
            Agent me = Mission?.MainAgent;
            if (me == null || !me.IsActive()) return;
            if (_agent == null || !_agent.IsActive())
            {
                _agent = Mission.Agents.FirstOrDefault(a => a != me && a.IsHuman && a.IsActive() && a.Character == _character);
                if (_agent == null) return;
            }
            _seen = true;
            float distance = me.Position.Distance(_agent.Position);
            if (distance <= TailRules.SceneTooClose) _tooClose += dt;
            else if (distance <= TailRules.SceneFollowMax) _follow += dt;
        }

        public override void OnRemoveBehavior()
        {
            base.OnRemoveBehavior();
            if (_reported) return;
            _reported = true;
            TailSceneResult result = TailRules.SceneResult(_seen, _follow, _tooClose);
            string targetId = _targetId;
            float follow = _follow;
            MenuUI.Defer(() => Tailing.SceneEnded(targetId, result, follow));
        }
    }
}
