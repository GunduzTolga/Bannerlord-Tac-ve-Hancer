using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;

namespace CheatMenu.Cheats.Models
{
    /// <summary>Yetenek XP çarpanı (oyuncu klanı). HeroDeveloper.AddSkillXp her XP kazancında bunu çağırır.</summary>
    public sealed class CheatGenericXpModel : GenericXpModel
    {
        public override float GetXpMultiplier(Hero hero)
        {
            float value = BaseModel.GetXpMultiplier(hero);
            int multiplier = SettingsStore.Current.SkillXpMultiplier;
            return multiplier > 1 && hero != null && hero.Clan == Clan.PlayerClan ? value * multiplier : value;
        }
    }
}
