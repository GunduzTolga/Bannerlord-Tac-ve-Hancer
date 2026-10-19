using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// "Yoldaş sınırını kaldır": oyuncu klanının yoldaş sınırı (Clan.CompanionLimit) pratikte sınırsız olur.
    /// Klan kademesi, parti sayısı sınırı ve diğer 13 üye olduğu gibi delege edilir.
    /// </summary>
    public sealed class CheatClanTierModel : ClanTierModel
    {
        // int.MaxValue değil: oyunda sınıra ekleme yapan bir hesap taşmasın; klan ekranında da okunur kalsın.
        private const int NoLimitCompanions = 999;

        public override int MinClanTier => BaseModel.MinClanTier;
        public override int MaxClanTier => BaseModel.MaxClanTier;
        public override int MercenaryEligibleTier => BaseModel.MercenaryEligibleTier;
        public override int VassalEligibleTier => BaseModel.VassalEligibleTier;
        public override int BannerEligibleTier => BaseModel.BannerEligibleTier;
        public override int RebelClanStartingTier => BaseModel.RebelClanStartingTier;
        public override int CompanionToLordClanStartingTier => BaseModel.CompanionToLordClanStartingTier;

        public override int CalculateInitialRenown(Clan clan) => BaseModel.CalculateInitialRenown(clan);
        public override int CalculateInitialInfluence(Clan clan) => BaseModel.CalculateInitialInfluence(clan);
        public override int CalculateTier(Clan clan) => BaseModel.CalculateTier(clan);

        public override (ExplainedNumber, bool) HasUpcomingTier(Clan clan, out TextObject extraExplanation,
            bool includeDescriptions = false) =>
            BaseModel.HasUpcomingTier(clan, out extraExplanation, includeDescriptions);

        public override int GetRequiredRenownForTier(int tier) => BaseModel.GetRequiredRenownForTier(tier);

        public override int GetPartyLimitForTier(Clan clan, int clanTierToCheck) =>
            BaseModel.GetPartyLimitForTier(clan, clanTierToCheck);

        public override int GetCompanionLimit(Clan clan)
        {
            int limit = BaseModel.GetCompanionLimit(clan);
            return SettingsStore.Current.NoCompanionLimit && clan == Clan.PlayerClan ? Math.Max(limit, NoLimitCompanions) : limit;
        }
    }
}
