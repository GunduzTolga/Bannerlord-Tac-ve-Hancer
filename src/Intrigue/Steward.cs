using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Divan: kâhya (P2.15, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-kahya.md). Bütün şehir ve kalelerine tek bakış (sadakat,
    /// güvenlik, refah, erzak, inşaat, vergi), uzaktan inşaat ve günlük proje emri (oyunun kendi işlemleriyle, ulak ücretiyle), isyan ve
    /// kıtlık uyarısı. Steward'ı yüksek kâhya klanının yerleşimlerinde sadakati her gün artırır (oyunun sadakat açıklamasında görünür);
    /// 30'un altındaysa yolsuzluk yapar.
    /// </summary>
    public static class Steward
    {
        public const string Seat = "kâhyan";
        private static int Today => SchemeFactors.Today;

        // ---- Koltuk ----

        public static Hero Current(IntrigueState state)
        {
            if (state?.StewardId == null) return null;
            Hero hero = Hero.Find(state.StewardId);
            return hero != null && hero.IsAlive && hero.Clan == Clan.PlayerClan ? hero : null;
        }

        public static Hero Working(IntrigueState state)
        {
            Hero hero = Current(state);
            return hero != null && SchemeFactors.SpymasterBusyReason(hero) == null ? hero : null;
        }

        public static string Blocked(IntrigueState state)
        {
            Hero hero = Current(state);
            if (hero == null) return "Kâhyan yok";
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            return busy != null ? hero.Name + " şu an " + busy + "; sarayda değil" : null;
        }

        public static int Skill(Hero hero) => hero?.GetSkillValue(DefaultSkills.Steward) ?? 0;

        public static IEnumerable<Hero> Candidates(IntrigueState state) => Council.Candidates(state, Seat);

        public static string Label(Hero hero)
        {
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            int steward = Skill(hero);
            return hero.Name + " - Steward " + steward + (StewardRules.IsCorrupt(steward) ? " (yolsuz)" : "")
                + ", sadakat günde " + StewardRules.Signed(StewardRules.LoyaltyBonus(steward), Tr.Culture) + (busy != null ? "  [" + busy + "]" : "");
        }

        public static string DetailText(Hero hero)
        {
            int steward = Skill(hero);
            return StewardRules.IsCorrupt(steward)
                ? "Yolsuz: valisiz şehir ve kalelerinde sadakat günde " + StewardRules.Signed(StewardRules.LoyaltyBonus(steward), Tr.Culture)
                : "Valisiz şehir ve kalelerinde sadakat günde " + StewardRules.Signed(StewardRules.LoyaltyBonus(steward), Tr.Culture);
        }

        public static void Assign(IntrigueState state, Hero hero)
        {
            Council.EnsureFree(state, hero, Seat);
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            if (busy != null) throw new InvalidOperationException(hero.Name + " şu an " + busy + "; kâhya olamaz");
            state.StewardId = hero.StringId;
            state.AddLog(Today, hero.Name + " kâhya oldu");
            MenuUI.Success(hero.Name + " artık kâhyan" + (StewardRules.IsCorrupt(Skill(hero)) ? " (dikkat: Steward'ı düşük, yolsuzluk yapar)" : ""));
        }

        public static void Dismiss(IntrigueState state)
        {
            if (state.StewardId == null) throw new InvalidOperationException("Kâhyan yok");
            Hero old = Current(state);
            state.StewardId = null;
            state.AddLog(Today, (old?.Name.ToString() ?? "Kâhya") + " görevden alındı");
            MenuUI.Success("Kâhya görevden alındı");
        }

        // ---- Oyunun sadakat modeline bağlanan etki ----

        /// <summary>
        /// Klanının bir yerleşimiyse, valisi yoksa ve kâhya çalışıyorsa günlük sadakat etkisi; değilse 0. Valisi olan yerleşimi vali
        /// yönetir: kâhyanın etkisi onun üstüne eklenmez (yolsuz kâhyanın cezası da).
        /// </summary>
        public static float LoyaltyEffect(Town town, out string name)
        {
            name = "";
            if (town == null || Clan.PlayerClan == null || town.OwnerClan != Clan.PlayerClan || town.Governor != null) return 0f;
            Hero steward = Working(IntrigueBehavior.Current?.State);
            if (steward == null) return 0f;
            name = steward.Name.ToString();
            return StewardRules.LoyaltyBonus(Skill(steward));
        }

        // ---- Defter ----

        public sealed class FiefLine
        {
            public Town Town;
            public int FoodDays;
            public int RebellionDays;
            public int Income;
            public Building Construction;
            public int ConstructionDays;
            public Building Daily;
            public List<string> Problems = new List<string>();
            public string Advice;
            public bool Danger;
        }

        /// <summary>Sadakat bunun altına inince yerleşim huzursuzluğa (isyan haline) girer.</summary>
        public static int RebellionThreshold => Campaign.Current.Models.SettlementLoyaltyModel.RebelliousStateStartLoyaltyThreshold;

        public static List<FiefLine> Report()
        {
            var lines = new List<FiefLine>();
            foreach (Town town in Clan.PlayerClan.Fiefs.Where(t => t?.Settlement != null).OrderBy(t => t.IsCastle).ThenBy(t => t.Name.ToString()))
            {
                var line = new FiefLine
                {
                    Town = town,
                    FoodDays = MarshalRules.FoodDays(town.FoodStocks, town.FoodChange),
                    RebellionDays = StewardRules.DaysUntil(town.Loyalty, town.LoyaltyChange, RebellionThreshold),
                    Income = Income(town),
                    Construction = town.CurrentBuilding,
                    Daily = town.CurrentDefaultBuilding,
                };
                if (line.Construction != null) line.ConstructionDays = DaysToComplete(line.Construction, town);
                if (town.InRebelliousState) line.Problems.Add("isyan halinde!");
                else if (line.RebellionDays <= StewardRules.RebellionWarnDays)
                    line.Problems.Add(line.RebellionDays == 0 ? "sadakat huzursuzluk eşiğinde" : "~" + line.RebellionDays + " günde huzursuzluk eşiği");
                if (line.FoodDays <= StewardRules.StarvationWarnDays) line.Problems.Add(line.FoodDays == 0 ? "erzak bitti" : "erzak " + line.FoodDays + " gün");
                if (town.Security < 30) line.Problems.Add("güvenlik düşük");
                if (town.Settlement.IsUnderSiege) line.Problems.Add("kuşatma altında");
                line.Danger = line.Problems.Count > 0;
                line.Advice = AdviceFor(town, line);
                lines.Add(line);
            }
            return lines;
        }

        private static int DaysToComplete(Building building, Town town)
        {
            try
            {
                return BuildingHelper.GetDaysToComplete(building, town);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>Vergi ve gümrük + bağlı köylerin günlük geliri (oyunun hesabı).</summary>
        public static int Income(Town town)
        {
            try
            {
                var finance = Campaign.Current.Models.ClanFinanceModel;
                return (int)finance.CalculateTownIncomeFromTariffs(Clan.PlayerClan, town, false).ResultNumber
                    + town.Villages.Sum(v => finance.CalculateVillageIncome(Clan.PlayerClan, v, false));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static string AdviceFor(Town town, FiefLine line)
        {
            if (town.InRebelliousState || line.RebellionDays <= StewardRules.RebellionWarnDays)
                return "Sadakati yükselt: günlük proje " + (DailyByKeyword(town, "festival")?.Name.ToString() ?? "şenlik") + ", aynı kültürden vali, güçlü garnizon";
            if (line.FoodDays <= StewardRules.StarvationWarnDays) return "Erzak gönder ya da köyleri koru; garnizonu azaltmak da yemeği artırır";
            if (town.Security < 30) return "Güvenliği yükselt: günlük proje " + (DailyByKeyword(town, "militia")?.Name.ToString() ?? "milis eğitimi") + ", garnizon";
            if (town.LoyaltyChange < 0) return "Sadakat yavaşça düşüyor; şimdilik tehlike yok";
            if (line.Construction == null) return "İnşaat kuyruğu boş: bir yapı başlat";
            return "Her şey yolunda";
        }

        private static Building DailyByKeyword(Town town, string keyword) =>
            town.Buildings.FirstOrDefault(b => b.BuildingType.IsDailyProject && b.BuildingType.StringId.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

        public static string BuildingText(Building building, int days) =>
            building.Name + " " + (building.CurrentLevel + 1) + (days > 0 ? " (~" + days + " gün)" : "");

        // ---- Uzaktan emir ----

        /// <summary>İnşa edilebilecek yapılar (en üst seviyede olmayanlar), sıradakiler önce.</summary>
        public static List<Building> Constructions(Town town) =>
            town.Buildings.Where(b => !b.BuildingType.IsDailyProject && b.CurrentLevel < StewardRules.MaxBuildingLevel)
                .OrderBy(b => town.BuildingsInProgress.Contains(b) ? 0 : 1).ThenBy(b => b.Name.ToString()).ToList();

        public static List<Building> DailyProjects(Town town) => town.Buildings.Where(b => b.BuildingType.IsDailyProject).ToList();

        public static string OrderBlocked(IntrigueState state, Town town)
        {
            string blocked = Blocked(state);
            if (blocked != null) return blocked;
            if (town.OwnerClan != Clan.PlayerClan) return town.Name + " artık senin değil";
            return SchemeFactors.CanAfford(StewardRules.RemoteOrderFee) ? null : "Yeterli altının yok";
        }

        /// <summary>Kâhya ulak gönderir: seçilen yapı inşaat kuyruğunun başına geçer (oyunun kendi işlemi).</summary>
        public static void StartConstruction(IntrigueState state, Town town, Building building)
        {
            string blocked = OrderBlocked(state, town);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(StewardRules.RemoteOrderFee);
            List<Building> queue = town.BuildingsInProgress.Where(b => b != building).ToList();
            queue.Insert(0, building);
            BuildingHelper.ChangeCurrentBuildingQueue(queue, town);
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " inşaat uzaktan değişti: " + building.Name);
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " şimdi " + building.Name + " yapılıyor");
        }

        public static void SetDailyProject(IntrigueState state, Town town, Building building)
        {
            string blocked = OrderBlocked(state, town);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(StewardRules.RemoteOrderFee);
            BuildingHelper.ChangeDefaultBuilding(building, town);
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " günlük proje uzaktan değişti: " + building.Name);
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " günlük proje: " + building.Name);
        }

        // ---- Günlük: uyarılar; haftalık: bakım ----

        public static void Daily()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            Hero steward = Working(state);
            if (steward == null) return;
            foreach (FiefLine line in Report())
            {
                Town town = line.Town;
                if (!town.InRebelliousState && line.RebellionDays <= StewardRules.RebellionWarnDays)
                    Warn(state, steward, town, "isyan", "Kâhyan uyarıyor: " + town.Name,
                        town.Name + " halkı huzursuz: sadakat " + (int)town.Loyalty + " (günde " + StewardRules.Signed(town.LoyaltyChange, Tr.Culture) + "). "
                        + (line.RebellionDays == 0 ? "Huzursuzluk eşiğinde; isyan çıkabilir." : "~" + line.RebellionDays + " gün sonra huzursuzluk eşiğine ("
                            + RebellionThreshold + ") iner; ardından isyan çıkabilir.")
                        + "\n\nKâhyanın önerisi: " + line.Advice + ".\n\nEntrika ekranı > Divan > " + town.Name + " > Yönet.");
                if (line.FoodDays <= StewardRules.StarvationWarnDays && !town.Settlement.IsUnderSiege)
                    Warn(state, steward, town, "kitlik", "Kâhyan uyarıyor: " + town.Name,
                        Tr.Locative(town.Name.ToString()) + " erzak " + (line.FoodDays == 0 ? "bitti" : line.FoodDays + " gün sonra bitiyor")
                        + " (günde " + StewardRules.Signed(town.FoodChange, Tr.Culture) + "). Açlık sadakati ve refahı düşürür.\n\nKâhyanın önerisi: "
                        + "tahıl gönder, köyleri yağmadan koru ya da garnizonu azalt.");
            }
        }

        private static void Warn(IntrigueState state, Hero steward, Town town, string kind, string title, string text)
        {
            string key = town.Settlement.StringId + "|" + kind;
            if (state.StewardWarned.TryGetValue(key, out int until) && Today < until) return;
            state.StewardWarned[key] = Today + StewardRules.WarnCooldownDays;
            SchemeOutcomes.Notify(state, title, text, steward, CardImages.Steward);
            state.AddLog(Today, title);
        }

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.StewardId == null) return;
            if (Current(state) == null)
            {
                Hero lost = Hero.Find(state.StewardId);
                state.AddLog(Today, "Kâhya koltuğu boşaldı" + (lost != null ? ": " + lost.Name + (lost.IsAlive ? " artık klanında değil" : " öldü") : ""));
                state.StewardId = null;
                return;
            }
            Working(state)?.AddSkillXp(DefaultSkills.Steward, StewardRules.WeeklyXp);
        }

        // ---- Liste menüsü için düz metin ----

        public static string ReportText(IntrigueState state)
        {
            string blocked = Blocked(state);
            if (blocked != null) return blocked + ".";
            List<FiefLine> lines = Report();
            if (lines.Count == 0) return DetailText(Current(state)) + "\nŞehrin ya da kalen yok.";
            return DetailText(Current(state)) + "\n" + string.Join("\n", lines.Select(l => "· " + l.Town.Name + ": sadakat " + (int)l.Town.Loyalty
                + ", güvenlik " + (int)l.Town.Security + ", erzak " + (l.FoodDays == int.MaxValue ? "yeterli" : l.FoodDays + " gün")
                + (l.Problems.Count > 0 ? " - " + string.Join(", ", l.Problems) : "")));
        }
    }
}
