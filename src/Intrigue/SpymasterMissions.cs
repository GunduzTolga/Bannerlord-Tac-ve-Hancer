using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Casus başı görevleri (P2.3, tasarım: docs/superpowers/specs/2026-10-19-p2-casus-basi-gorevleri.md). Haftalık işler;
    /// casus başı yoksa ya da meşgulse (esir, vali, parti lideri) görev durur. Karşı casusluk AiIntrigue'deki sezmeye bonus verir.
    /// </summary>
    public static class SpymasterMissions
    {
        public static Hero RumorTarget(IntrigueState state) => state.RumorTargetId != null ? Hero.Find(state.RumorTargetId) : null;

        /// <summary>Casus başı şu an görevini yapabiliyor mu.</summary>
        public static bool Working(IntrigueState state, out Hero spymaster)
        {
            spymaster = SchemeFactors.Spymaster(state);
            return spymaster != null && SchemeFactors.SpymasterBusyReason(spymaster) == null;
        }

        // ---- Görev seçimi ----

        public static void SetMission(IntrigueState state, SpymasterMission mission, Hero rumorTarget = null)
        {
            if (mission != SpymasterMission.None && SchemeFactors.Spymaster(state) == null)
                throw new InvalidOperationException("Önce bir casus başı ata");
            if (mission == SpymasterMission.WatchDissent && Clan.PlayerClan.Kingdom == null)
                throw new InvalidOperationException("İzlenecek bir saray yok: önce bir krallıkta ol");
            if (mission == SpymasterMission.SpreadRumors)
            {
                string blocked = RumorBlocked(state, rumorTarget);
                if (blocked != null) throw new InvalidOperationException(blocked);
            }
            if (mission == SpymasterMission.TrainAgents)
            {
                int max = AgentRules.MaxAgents(SchemeFactors.Roguery(SchemeFactors.Spymaster(state)));
                if (state.Agents >= max) throw new InvalidOperationException("Ajan kadron dolu (" + state.Agents + "/" + max + "): Roguery arttıkça büyür");
            }
            state.Mission = mission;
            state.RumorTargetId = mission == SpymasterMission.SpreadRumors ? rumorTarget.StringId : null;
            string what = MissionRules.Name(mission) + (mission == SpymasterMission.SpreadRumors ? ": " + rumorTarget.Name : "");
            state.AddLog(SchemeFactors.Today, "Casus başı görevi: " + what);
            MenuUI.Success("Casus başının görevi: " + what);
        }

        /// <summary>null: bu lord hakkında söylenti yayılabilir; değilse neden.</summary>
        public static string RumorBlocked(IntrigueState state, Hero target)
        {
            if (target == null || !target.IsAlive) return "Hedef yok";
            if (!target.IsLord || target.Clan == null || target.Clan == Clan.PlayerClan) return "Yalnızca başka klanların lordları";
            Kingdom kingdom = target.Clan.Kingdom;
            if (kingdom == null) return "Bir krallıkta değil: arasını açacak bir hükümdarı yok";
            if (kingdom.Leader == target) return "Hükümdarın kendisi hakkında değil, lordları hakkında söylenti yayılır";
            if (kingdom.Leader == Hero.MainHero) return "Kendi sarayında kendi lordlarını sana düşürmenin anlamı yok";
            if (Court.KnowledgeOf(state, target) == Court.Knowledge.Unknown)
                return "Sarayında kulağın yok: önce casusluk, sorgu ya da o krallıkta gölge ağı";
            return null;
        }

        /// <summary>Söylenti hedefi seçilebilecek lordlar (hükümdarlarıyla ilişkisi en iyi olandan başlayarak).</summary>
        public static List<Hero> RumorCandidates(IntrigueState state) =>
            Court.Kingdoms().SelectMany(Court.ClanLeaders).Where(h => RumorBlocked(state, h) == null)
                .OrderByDescending(Court.RelationWithRuler).ToList();

        public static string StatusText(IntrigueState state)
        {
            string text = MissionRules.Name(state.Mission);
            if (state.Mission == SpymasterMission.SpreadRumors) text += ": " + (RumorTarget(state)?.Name.ToString() ?? "?");
            if (state.Mission != SpymasterMission.None && !Working(state, out _)) text += " (casus başı meşgul: duruyor)";
            if (state.Agents > 0) text += " · ajan " + state.Agents;
            return text;
        }

        public static string Description(SpymasterMission mission, int roguery, int difficulty)
        {
            switch (mission)
            {
                case SpymasterMission.CounterIntel:
                    return "Sana karşı kurulan entrikaları sezme şansı haftalık +%" + MissionRules.CounterIntelBonus
                        + " (şu an %" + MissionRules.Detection(true, roguery, mission) + ").";
                case SpymasterMission.WatchDissent:
                    return "Her hafta krallığındaki en hoşnutsuz " + MissionRules.DissentReportsPerWeek(roguery)
                        + " klan lideri hakkında rapor: kim kralına küs, kim isyana yakın.";
                case SpymasterMission.SpreadRumors:
                    return "Seçtiğin lordun hükümdarıyla ilişkisi haftada -" + MissionRules.RumorRelationLoss(roguery) + "; ücret haftada "
                        + SchemeFactors.Money(MissionRules.RumorWeeklyCost) + ". Haftalık yakalanma %" + MissionRules.RumorDiscovery(roguery, difficulty)
                        + ": o zaman lordla -" + MissionRules.RumorExposedTargetPenalty + ", hükümdarıyla -" + MissionRules.RumorExposedRulerPenalty
                        + " ve görev biter.";
                case SpymasterMission.TrainAgents:
                    return "Her " + AgentRules.WeeksPerAgent + " haftada bir ajan; eğitim haftada " + SchemeFactors.Money(AgentRules.TrainingWeeklyCost)
                        + ". Kadro en fazla " + AgentRules.MaxAgents(roguery) + " (Roguery'ye göre). Her ajan bütün entrikalara +%"
                        + AgentRules.SuccessPerAgent + " başarı, -%" + AgentRules.DiscoveryPerAgent + " yakalanma (gölge ağı gibi, ama her yerde); bakım ajan başına haftada "
                        + SchemeFactors.Money(AgentRules.UpkeepPerAgent) + ". Bir entrikan ortaya çıkarsa bir ajan yakalanır.";
                default:
                    return "Görev yok: casus başı yalnızca entrikalarla uğraşır.";
            }
        }

        // ---- Haftalık ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            AgentUpkeep(state);
            if (state.Mission == SpymasterMission.None) return;
            if (!Working(state, out Hero spymaster)) return;
            int roguery = SchemeFactors.Roguery(spymaster);
            switch (state.Mission)
            {
                case SpymasterMission.TrainAgents:
                    TrainAgents(state, roguery);
                    break;
                case SpymasterMission.WatchDissent:
                    WatchDissent(state, roguery);
                    break;
                case SpymasterMission.SpreadRumors:
                    SpreadRumors(state, roguery);
                    break;
            }
            spymaster.AddSkillXp(DefaultSkills.Roguery, MissionRules.MissionXpPerWeek);
        }

        private static void WatchDissent(IntrigueState state, int roguery)
        {
            Kingdom kingdom = Clan.PlayerClan.Kingdom;
            if (kingdom == null) return;
            int today = SchemeFactors.Today;
            List<Hero> found = Court.ClanLeaders(kingdom)
                .Where(h => LetterRules.IsDiscontent(Court.RelationWithRuler(h)) && !state.HasValidReport(h.StringId, today))
                .OrderBy(Court.RelationWithRuler)
                .Take(MissionRules.DissentReportsPerWeek(roguery))
                .ToList();
            if (found.Count == 0) return;
            foreach (Hero lord in found)
            {
                state.Reports.RemoveAll(r => r.TargetId == lord.StringId);
                state.Reports.Add(SchemeOutcomes.CreateReport(lord, today));
            }
            string lines = string.Join("\n", found.Select(h => "- " + h.Name + " (" + h.Clan.Name + "): hükümdarıyla "
                + LetterRules.MoodName(Court.RelationWithRuler(h)) + " (" + SchemeFactors.Signed(Court.RelationWithRuler(h)) + ")"));
            SchemeOutcomes.Notify(state, "Muhalefet raporu",
                "Casus başın sarayda kulak kabarttı. Hükümdarına küs olanlar:\n\n" + lines
                + "\n\nHer biri için güncel rapor Kişiler sekmesinde.", found[0], CardImages.Spy);
            state.AddLog(today, "Muhalefet raporu: " + string.Join(", ", found.Select(h => h.Name.ToString())));
        }

        private static void SpreadRumors(IntrigueState state, int roguery)
        {
            Hero target = RumorTarget(state);
            string blocked = RumorBlocked(state, target);
            int today = SchemeFactors.Today;
            if (blocked != null)
            {
                Stop(state, "Söylenti görevi bitti: " + (target?.Name.ToString() ?? "hedef") + " için artık mümkün değil (" + blocked.ToLower(Tr.Culture) + ").", target);
                return;
            }
            if (!SchemeFactors.CanAfford(MissionRules.RumorWeeklyCost))
            {
                Stop(state, "Söylenti görevi bitti: muhbirlere ödeyecek altının kalmadı.", target);
                return;
            }
            SchemeFactors.Pay(MissionRules.RumorWeeklyCost);
            Hero ruler = target.Clan.Kingdom.Leader;
            if (!CheatState.IntrigueNeverDiscovered && state.Roll100() <= MissionRules.RumorDiscovery(roguery, SettingsStore.Current.IntrigueDifficulty))
            {
                SchemeOutcomes.Relate(target, -MissionRules.RumorExposedTargetPenalty);
                SchemeOutcomes.Relate(ruler, -MissionRules.RumorExposedRulerPenalty);
                state.Mission = SpymasterMission.None;
                state.RumorTargetId = null;
                SchemeOutcomes.Notify(state, "Söylentilerin kaynağı ortaya çıktı",
                    Tr.Genitive(target.Name.ToString()) + " hakkındaki fısıltıların arkasında senin adamların olduğu anlaşıldı.\n\nSonuç: "
                    + target.Name + " ile ilişki -" + MissionRules.RumorExposedTargetPenalty + ", " + ruler.Name + " ile -"
                    + MissionRules.RumorExposedRulerPenalty + ". Casus başın görevden döndü.", target, CardImages.Exposed);
                state.AddLog(today, "Söylenti ortaya çıktı: " + target.Name);
                return;
            }
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(target, ruler, -MissionRules.RumorRelationLoss(roguery), false);
        }

        // ---- Ajanlar ----

        /// <summary>Ajanların haftalık bakımı (görev ne olursa olsun); ödenemezse bir ajan dağılır.</summary>
        private static void AgentUpkeep(IntrigueState state)
        {
            if (state.Agents <= 0) return;
            int cost = AgentRules.Upkeep(state.Agents);
            if (SchemeFactors.CanAfford(cost))
            {
                SchemeFactors.Pay(cost);
                return;
            }
            state.Agents--;
            string text = "Ajanlarından birine bu hafta ödeme yapılamadı ve ortadan kayboldu. Kalan ajan: " + state.Agents + ".";
            SchemeOutcomes.Notify(state, "Bir ajan gitti", text, SchemeFactors.Spymaster(state));
            state.AddLog(SchemeFactors.Today, "Ajan dağıldı (ödeme yok); kalan " + state.Agents);
        }

        private static void TrainAgents(IntrigueState state, int roguery)
        {
            int max = AgentRules.MaxAgents(roguery);
            if (state.Agents >= max)
            {
                Stop(state, "Ajan yetiştirme bitti: kadro dolu (" + state.Agents + "/" + max + "). Casus başının Roguery'si arttıkça yeniden seçebilirsin.", null);
                return;
            }
            if (!SchemeFactors.CanAfford(AgentRules.TrainingWeeklyCost))
            {
                Stop(state, "Ajan yetiştirme bitti: eğitim için altının kalmadı.", null);
                return;
            }
            SchemeFactors.Pay(AgentRules.TrainingWeeklyCost);
            int agents = state.Agents, progress = state.AgentTraining;
            bool ready = AgentRules.TrainWeek(ref agents, ref progress);
            state.Agents = agents;
            state.AgentTraining = progress;
            if (!ready) return;
            state.AddLog(SchemeFactors.Today, "Yeni ajan hazır (" + state.Agents + "/" + max + ")");
            MenuUI.Success("Casus başın yeni bir ajan yetiştirdi: " + state.Agents + "/" + max);
        }

        /// <summary>Bir entrika ortaya çıktığında: bir ajan yakalanır. Sonuç metnine eklenecek cümle (ajan yoksa boş).</summary>
        public static string LoseAgent(IntrigueState state)
        {
            if (state.Agents <= 0) return "";
            state.Agents--;
            state.AddLog(SchemeFactors.Today, "Bir ajan yakalandı; kalan " + state.Agents);
            return " Ajanlarından biri yakalandı (kalan " + state.Agents + ").";
        }

        private static void Stop(IntrigueState state, string text, Hero target)
        {
            state.Mission = SpymasterMission.None;
            state.RumorTargetId = null;
            SchemeOutcomes.Notify(state, "Casus başı görevi bitti", text, target != null && target.IsAlive ? target : null);
            state.AddLog(SchemeFactors.Today, text);
        }
    }
}
