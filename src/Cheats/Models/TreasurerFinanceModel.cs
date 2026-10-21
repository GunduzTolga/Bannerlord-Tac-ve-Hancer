using System.Linq;
using CheatMenu.Intrigue;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// Divan, hazinedar: klanının günlük gelirine hazinedarın satırı eklenir (atölye ve kervan gelirine Trade'e göre yüzde; oyunun
    /// gelir açıklamasında görünür). Satır hem gelire hem günlük altın değişimine eklenir: oyun altını ikincisinden verir.
    /// Diğer her şey oyunun kendi modeline delege edilir.
    /// </summary>
    public sealed class TreasurerFinanceModel : ClanFinanceModel
    {
        public override ExplainedNumber CalculateClanGoldChange(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
        {
            ExplainedNumber result = BaseModel.CalculateClanGoldChange(clan, includeDescriptions, applyWithdrawals, includeDetails);
            AddTreasurer(clan, ref result);
            return result;
        }

        public override ExplainedNumber CalculateClanIncome(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false)
        {
            ExplainedNumber result = BaseModel.CalculateClanIncome(clan, includeDescriptions, applyWithdrawals, includeDetails);
            AddTreasurer(clan, ref result);
            return result;
        }

        private void AddTreasurer(Clan clan, ref ExplainedNumber result)
        {
            if (clan == null || clan != Clan.PlayerClan) return;
            int income = Hero.MainHero.OwnedWorkshops.Where(w => w?.Settlement?.Town != null && w.WorkshopType != null)
                    .Sum(w => BaseModel.CalculateOwnerIncomeFromWorkshop(w))
                + clan.Heroes.Where(h => h.IsAlive).SelectMany(h => h.OwnedCaravans).Select(c => c?.MobileParty)
                    .Where(p => p != null && p.IsActive).Distinct().Sum(p => BaseModel.CalculateOwnerIncomeFromCaravan(p));
            int bonus = Treasury.IncomeBonus(income, out string name);
            if (bonus > 0) result.Add(bonus, new TextObject("Hazinedar " + name));
        }

        public override ExplainedNumber CalculateClanExpenses(Clan clan, bool includeDescriptions = false, bool applyWithdrawals = false, bool includeDetails = false) =>
            BaseModel.CalculateClanExpenses(clan, includeDescriptions, applyWithdrawals, includeDetails);

        public override ExplainedNumber CalculateTownIncomeFromTariffs(Clan clan, Town town, bool applyWithdrawals = false) =>
            BaseModel.CalculateTownIncomeFromTariffs(clan, town, applyWithdrawals);

        public override int CalculateTownIncomeFromProjects(Town town) => BaseModel.CalculateTownIncomeFromProjects(town);
        public override int CalculateNotableDailyGoldChange(Hero hero, bool applyWithdrawals) => BaseModel.CalculateNotableDailyGoldChange(hero, applyWithdrawals);
        public override int CalculateVillageIncome(Clan clan, Village village, bool applyWithdrawals = false) => BaseModel.CalculateVillageIncome(clan, village, applyWithdrawals);
        public override int CalculateOwnerIncomeFromCaravan(MobileParty caravan) => BaseModel.CalculateOwnerIncomeFromCaravan(caravan);
        public override int CalculateOwnerIncomeFromWorkshop(Workshop workshop) => BaseModel.CalculateOwnerIncomeFromWorkshop(workshop);
        public override float RevenueSmoothenFraction() => BaseModel.RevenueSmoothenFraction();
        public override int PartyGoldLowerThreshold => BaseModel.PartyGoldLowerThreshold;
    }
}
