using System;
using CheatMenu.Menu;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Buildings;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Yerleşim düzenleme. Kuşatma, yağma ve savaş sırasında oyunun kendi hilesi gibi reddeder.
    /// Sahiplik oyunun eylemiyle değişir (vali ve krallık olayları doğru tetiklenir).
    /// </summary>
    public static class SettlementEditor
    {
        public static string GetBlocker(Settlement settlement)
        {
            if (settlement.IsUnderSiege) return settlement.Name + " kuşatma altında";
            if (settlement.IsUnderRaid) return settlement.Name + " yağmalanıyor";
            if (settlement.Party.MapEvent != null) return settlement.Name + " şu an bir savaşın içinde";
            return null;
        }

        public static void ChangeOwner(Settlement settlement, Clan clan)
        {
            Guard(settlement);
            if (settlement.IsVillage) throw new InvalidOperationException("Köyler bağlı oldukları şehirle birlikte el değiştirir");
            if (clan == Clan.PlayerClan && clan.IsUnderMercenaryService)
                throw new InvalidOperationException("Paralı askerken yerleşim sahibi olamazsın");
            Hero newOwner = clan.Leader ?? throw new InvalidOperationException(clan.Name + " klanının lideri yok");
            ChangeOwnerOfSettlementAction.ApplyByDefault(newOwner, settlement);
            MenuUI.Success(settlement.Name + " artık " + clan.Name + " klanının");
        }

        public static void AddProsperity(Settlement settlement, int amount)
        {
            Town town = RequireTown(settlement);
            town.Prosperity += amount;
            MenuUI.Success(settlement.Name + " refahı: " + town.Prosperity.ToString("N0"));
        }

        public static void MaxLoyalty(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            town.Loyalty = 100f;
            MenuUI.Success(settlement.Name + " sadakati: " + town.Loyalty.ToString("0"));
        }

        public static void MaxSecurity(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            town.Security = 100f;
            MenuUI.Success(settlement.Name + " güvenliği: " + town.Security.ToString("0"));
        }

        public static void FillFood(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            town.FoodStocks = town.FoodStocksUpperLimit();
            MenuUI.Success(settlement.Name + " yiyecek deposu: " + town.FoodStocks.ToString("0"));
        }

        /// <summary>
        /// CheckIfBuildingIsComplete hangi binayı verirsen ver kuyruğun BAŞINI siler; bu yüzden her turda Peek().
        /// </summary>
        public static void FinishAllProjects(Settlement settlement)
        {
            Town town = RequireTown(settlement);
            int finished = 0;
            while (town.BuildingsInProgress.Count > 0 && finished < 100)
            {
                Building building = town.BuildingsInProgress.Peek();
                building.BuildingProgress = building.GetConstructionCost();
                BuildingHelper.CheckIfBuildingIsComplete(building);
                finished++;
            }
            MenuUI.Success(settlement.Name + ": " + finished + " proje tamamlandı");
        }

        public static void AddHearth(Settlement settlement, int amount)
        {
            Guard(settlement);
            Village village = settlement.Village ?? throw new InvalidOperationException(settlement.Name + " bir köy değil");
            village.Hearth += amount;
            MenuUI.Success(settlement.Name + " hane: " + village.Hearth.ToString("0"));
        }

        public static void AddMilitia(Settlement settlement, int amount)
        {
            Guard(settlement);
            settlement.Militia += amount;
            MenuUI.Success(settlement.Name + " milis: " + settlement.Militia.ToString("0"));
        }

        private static Town RequireTown(Settlement settlement)
        {
            Guard(settlement);
            return settlement.Town ?? throw new InvalidOperationException(settlement.Name + " bir şehir ya da kale değil");
        }

        private static void Guard(Settlement settlement)
        {
            string blocker = GetBlocker(settlement);
            if (blocker != null) throw new InvalidOperationException(blocker);
        }
    }
}
