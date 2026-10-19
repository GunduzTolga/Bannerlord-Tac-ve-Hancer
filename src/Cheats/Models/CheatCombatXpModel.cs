using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// Asker XP çarpanı: oyuncu partisindeki kahraman olmayan askerlerin vuruşlardan kazandığı XP.
    /// Kahramanların XP'si zaten GenericXpModel'den geçtiği için burada çarpılmaz (çift sayılmasın).
    /// </summary>
    public sealed class CheatCombatXpModel : CombatXpModel
    {
        private static readonly TextObject BonusText = new TextObject(ModInfo.DisplayName + ": asker XP çarpanı", null);

        public override float CaptainRadius => BaseModel.CaptainRadius;

        public override SkillObject GetSkillForWeapon(WeaponComponentData weapon, bool isSiegeEngineHit) =>
            BaseModel.GetSkillForWeapon(weapon, isSiegeEngineHit);

        public override ExplainedNumber GetXpFromHit(CharacterObject attackerTroop, CharacterObject captain,
            CharacterObject attackedTroop, PartyBase attackerParty, int damage, bool isFatal, MissionTypeEnum missionType)
        {
            ExplainedNumber result = BaseModel.GetXpFromHit(attackerTroop, captain, attackedTroop, attackerParty, damage, isFatal, missionType);
            int multiplier = SettingsStore.Current.TroopXpMultiplier;
            if (multiplier > 1 && attackerParty == PartyBase.MainParty && attackerTroop != null && !attackerTroop.IsHero)
                result.AddFactor(multiplier - 1, BonusText);
            return result;
        }

        public override float GetXpMultiplierFromShotDifficulty(float shotDifficulty) =>
            BaseModel.GetXpMultiplierFromShotDifficulty(shotDifficulty);
    }
}
