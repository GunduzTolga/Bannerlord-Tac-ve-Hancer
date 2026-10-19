using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Naval;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>Parti boyutu bonusu: yalnızca oyuncu partisinin asker ve esir sınırı. Diğer üyeler delege edilir.</summary>
    public sealed class CheatPartySizeLimitModel : PartySizeLimitModel
    {
        private static readonly TextObject BonusText = new TextObject(ModInfo.DisplayName + ": parti boyutu bonusu", null);

        public override int MinimumNumberOfVillagersAtVillagerParty => BaseModel.MinimumNumberOfVillagersAtVillagerParty;

        public override ExplainedNumber GetPartyMemberSizeLimit(PartyBase party, bool includeDescriptions = false) =>
            WithBonus(party, BaseModel.GetPartyMemberSizeLimit(party, includeDescriptions));

        public override ExplainedNumber GetPartyPrisonerSizeLimit(PartyBase party, bool includeDescriptions = false) =>
            WithBonus(party, BaseModel.GetPartyPrisonerSizeLimit(party, includeDescriptions));

        public override ExplainedNumber CalculateGarrisonPartySizeLimit(Settlement settlement, bool includeDescriptions = false) =>
            BaseModel.CalculateGarrisonPartySizeLimit(settlement, includeDescriptions);

        public override int GetClanTierPartySizeEffectForHero(Hero hero) => BaseModel.GetClanTierPartySizeEffectForHero(hero);

        public override int GetNextClanTierPartySizeEffectChangeForHero(Hero hero) =>
            BaseModel.GetNextClanTierPartySizeEffectChangeForHero(hero);

        public override int GetAssumedPartySizeForLordParty(Hero leaderHero, IFaction partyMapFaction, Clan actualClan) =>
            BaseModel.GetAssumedPartySizeForLordParty(leaderHero, partyMapFaction, actualClan);

        public override int GetIdealVillagerPartySize(Village village) => BaseModel.GetIdealVillagerPartySize(village);

        public override TroopRoster FindAppropriateInitialRosterForMobileParty(MobileParty party, PartyTemplateObject partyTemplate) =>
            BaseModel.FindAppropriateInitialRosterForMobileParty(party, partyTemplate);

        public override List<Ship> FindAppropriateInitialShipsForMobileParty(MobileParty party, PartyTemplateObject partyTemplate) =>
            BaseModel.FindAppropriateInitialShipsForMobileParty(party, partyTemplate);

        private static ExplainedNumber WithBonus(PartyBase party, ExplainedNumber result)
        {
            int bonus = SettingsStore.Current.PartySizeBonus;
            if (bonus > 0 && party == PartyBase.MainParty) result.Add(bonus, BonusText, null);
            return result;
        }
    }
}
