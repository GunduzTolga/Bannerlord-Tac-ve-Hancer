using CheatMenu.Intrigue;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// Divan, kâhya (P2.15): klanının şehir ve kalelerinde günlük sadakate kâhyanın satırı eklenir (oyunun sadakat açıklamasında görünür).
    /// Diğer her şey oyunun kendi modeline delege edilir.
    /// </summary>
    public sealed class StewardLoyaltyModel : SettlementLoyaltyModel
    {
        public override ExplainedNumber CalculateLoyaltyChange(Town town, bool includeDescriptions = false)
        {
            ExplainedNumber result = BaseModel.CalculateLoyaltyChange(town, includeDescriptions);
            float bonus = Steward.LoyaltyEffect(town, out string name);
            if (bonus != 0f) result.Add(bonus, new TextObject("Kâhya " + name));
            return result;
        }

        public override void CalculateGoldGainDueToHighLoyalty(Town town, ref ExplainedNumber explainedNumber) =>
            BaseModel.CalculateGoldGainDueToHighLoyalty(town, ref explainedNumber);

        public override void CalculateGoldCutDueToLowLoyalty(Town town, ref ExplainedNumber explainedNumber) =>
            BaseModel.CalculateGoldCutDueToLowLoyalty(town, ref explainedNumber);

        public override int SettlementLoyaltyChangeDueToSecurityThreshold => BaseModel.SettlementLoyaltyChangeDueToSecurityThreshold;
        public override int MaximumLoyaltyInSettlement => BaseModel.MaximumLoyaltyInSettlement;
        public override int LoyaltyDriftMedium => BaseModel.LoyaltyDriftMedium;
        public override float HighLoyaltyProsperityEffect => BaseModel.HighLoyaltyProsperityEffect;
        public override int LowLoyaltyProsperityEffect => BaseModel.LowLoyaltyProsperityEffect;
        public override int MilitiaBoostPercentage => BaseModel.MilitiaBoostPercentage;
        public override float HighSecurityLoyaltyEffect => BaseModel.HighSecurityLoyaltyEffect;
        public override float LowSecurityLoyaltyEffect => BaseModel.LowSecurityLoyaltyEffect;
        public override float GovernorSameCultureLoyaltyEffect => BaseModel.GovernorSameCultureLoyaltyEffect;
        public override float GovernorDifferentCultureLoyaltyEffect => BaseModel.GovernorDifferentCultureLoyaltyEffect;
        public override float SettlementOwnerDifferentCultureLoyaltyEffect => BaseModel.SettlementOwnerDifferentCultureLoyaltyEffect;
        public override int ThresholdForTaxBoost => BaseModel.ThresholdForTaxBoost;
        public override int RebellionStartLoyaltyThreshold => BaseModel.RebellionStartLoyaltyThreshold;
        public override int ThresholdForTaxCorruption => BaseModel.ThresholdForTaxCorruption;
        public override int ThresholdForHigherTaxCorruption => BaseModel.ThresholdForHigherTaxCorruption;
        public override int ThresholdForProsperityBoost => BaseModel.ThresholdForProsperityBoost;
        public override int ThresholdForProsperityPenalty => BaseModel.ThresholdForProsperityPenalty;
        public override int AdditionalStarvationPenaltyStartDay => BaseModel.AdditionalStarvationPenaltyStartDay;
        public override int AdditionalStarvationLoyaltyEffect => BaseModel.AdditionalStarvationLoyaltyEffect;
        public override int RebelliousStateStartLoyaltyThreshold => BaseModel.RebelliousStateStartLoyaltyThreshold;
        public override int LoyaltyBoostAfterRebellionStartValue => BaseModel.LoyaltyBoostAfterRebellionStartValue;
        public override float ThresholdForNotableRelationBonus => BaseModel.ThresholdForNotableRelationBonus;
        public override int DailyNotableRelationBonus => BaseModel.DailyNotableRelationBonus;
    }
}
