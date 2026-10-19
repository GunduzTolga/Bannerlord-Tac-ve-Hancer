using CheatMenu.Intrigue;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// Divan, hekim (P2.13): oyunun yaşlılıktan ölüm olasılığı, klanındakiler için hekimin Medicine'ine göre azalır (şarlatansa artar).
    /// Diğer herkes için oyunun kendi değeri.
    /// </summary>
    public sealed class PhysicianDeathModel : HeroDeathProbabilityCalculationModel
    {
        public override float CalculateHeroDeathProbability(Hero hero) =>
            Physician.DeathProbability(hero, BaseModel.CalculateHeroDeathProbability(hero));
    }
}
