using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Hızı önce zincirdeki önceki model (vanilla / NavalDLC) hesaplar; FastParty açıksa
    /// yalnızca oyuncunun partisi için sonuca +%100 çarpan eklenir.
    /// </summary>
    public sealed class CheatPartySpeedModel : PartySpeedModel
    {
        private static readonly TextObject CheatText = new TextObject(ModInfo.DisplayName + ": parti hızı x2", null);

        public override float BaseSpeed => BaseModel.BaseSpeed;

        public override float MinimumSpeed => BaseModel.MinimumSpeed;

        public override ExplainedNumber CalculateBaseSpeed(MobileParty party, bool includeDescriptions = false,
            int additionalTroopOnFootCount = 0, int additionalTroopOnHorseCount = 0) =>
            BaseModel.CalculateBaseSpeed(party, includeDescriptions, additionalTroopOnFootCount, additionalTroopOnHorseCount);

        public override ExplainedNumber CalculateFinalSpeed(MobileParty mobileParty, ExplainedNumber finalSpeed)
        {
            ExplainedNumber result = BaseModel.CalculateFinalSpeed(mobileParty, finalSpeed);
            if (CheatState.FastParty && mobileParty == MobileParty.MainParty)
                result.AddFactor(1f, CheatText);
            return result;
        }
    }
}
