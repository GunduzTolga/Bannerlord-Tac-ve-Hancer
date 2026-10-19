using CheatMenu.Intrigue;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// Aile editörü (P2.7): "çocuk istemiyorlar" işaretli çiftte oyunun kendi gebelik ihtimali 0. Divan, hekim (P2.13): az önce doğuran
    /// klanındaki bir annenin doğumda ölüm olasılığı hekimin Medicine'ine göre. Diğer üyeler olduğu gibi delege edilir.
    /// </summary>
    public sealed class FamilyPregnancyModel : PregnancyModel
    {
        public override float PregnancyDurationInDays => BaseModel.PregnancyDurationInDays;
        public override float MaternalMortalityProbabilityInLabor => Physician.LaborMortality(BaseModel.MaternalMortalityProbabilityInLabor);
        public override float StillbirthProbability => BaseModel.StillbirthProbability;
        public override float DeliveringFemaleOffspringProbability => BaseModel.DeliveringFemaleOffspringProbability;
        public override float DeliveringTwinsProbability => BaseModel.DeliveringTwinsProbability;

        public override float GetDailyChanceOfPregnancyForHero(Hero hero) =>
            FamilyEditor.WantsNoChildren(hero) ? 0f : BaseModel.GetDailyChanceOfPregnancyForHero(hero);
    }
}
