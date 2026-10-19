using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats.Models
{
    /// <summary>
    /// "Öğrenme sınırını kaldır": oyuncu klanının kahramanlarında öğrenme sınırına +1000.
    /// DefaultCharacterDevelopmentModel.CalculateLearningRate sınırı zincirin tepesinden (Campaign.Current.Models)
    /// sorduğu için "sınır aşıldı" cezası da kendiliğinden kalkar. Diğer 19 üye olduğu gibi delege edilir.
    /// </summary>
    public sealed class CheatCharacterDevelopmentModel : CharacterDevelopmentModel
    {
        private const int NoLimitBonus = 1000;
        private static readonly TextObject NoLimitText = new TextObject(ModInfo.DisplayName + ": öğrenme sınırı kaldırıldı", null);

        public override int MaxAttribute => BaseModel.MaxAttribute;
        public override int MaxFocusPerSkill => BaseModel.MaxFocusPerSkill;
        public override int MaxSkillRequiredForEpicPerkBonus => BaseModel.MaxSkillRequiredForEpicPerkBonus;
        public override int MinSkillRequiredForEpicPerkBonus => BaseModel.MinSkillRequiredForEpicPerkBonus;
        public override int FocusPointsPerLevel => BaseModel.FocusPointsPerLevel;
        public override int FocusPointsAtStart => BaseModel.FocusPointsAtStart;
        public override int AttributePointsAtStart => BaseModel.AttributePointsAtStart;
        public override int LevelsPerAttributePoint => BaseModel.LevelsPerAttributePoint;

        public override int SkillsRequiredForLevel(int level) => BaseModel.SkillsRequiredForLevel(level);
        public override int GetMaxSkillPoint() => BaseModel.GetMaxSkillPoint();
        public override int GetXpRequiredForSkillLevel(int skillLevel) => BaseModel.GetXpRequiredForSkillLevel(skillLevel);

        public override int GetSkillLevelChange(Hero hero, SkillObject skill, float skillXp) =>
            BaseModel.GetSkillLevelChange(hero, skill, skillXp);

        public override int GetXpAmountForSkillLevelChange(Hero hero, SkillObject skill, int skillLevelChange) =>
            BaseModel.GetXpAmountForSkillLevelChange(hero, skill, skillLevelChange);

        public override void GetTraitLevelForTraitXp(Hero hero, TraitObject trait, int newValue, out int traitLevel, out int traitXp) =>
            BaseModel.GetTraitLevelForTraitXp(hero, trait, newValue, out traitLevel, out traitXp);

        public override int GetTraitXpRequiredForTraitLevel(TraitObject trait, int traitLevel) =>
            BaseModel.GetTraitXpRequiredForTraitLevel(trait, traitLevel);

        public override ExplainedNumber CalculateLearningLimit(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes,
            int focusValue, SkillObject skill, bool includeDescriptions = false)
        {
            ExplainedNumber result = BaseModel.CalculateLearningLimit(characterAttributes, focusValue, skill, includeDescriptions);
            if (SettingsStore.Current.NoLearningLimit && BelongsToPlayerClan(characterAttributes))
                result.Add(NoLimitBonus, NoLimitText, null);
            return result;
        }

        public override ExplainedNumber CalculateLearningRate(IReadOnlyPropertyOwner<CharacterAttribute> characterAttributes,
            int focusValue, int skillValue, SkillObject skill, bool includeDescriptions = false) =>
            BaseModel.CalculateLearningRate(characterAttributes, focusValue, skillValue, skill, includeDescriptions);

        public override SkillObject GetNextSkillToAddFocus(Hero hero) => BaseModel.GetNextSkillToAddFocus(hero);
        public override CharacterAttribute GetNextAttributeToUpgrade(Hero hero) => BaseModel.GetNextAttributeToUpgrade(hero);
        public override PerkObject GetNextPerkToChoose(Hero hero, PerkObject perk) => BaseModel.GetNextPerkToChoose(hero, perk);

        // Öznitelik nesnesi kahramana özel; hangi kahramana ait olduğunu referansla buluyoruz (oyuncu klanı küçük bir liste).
        private static bool BelongsToPlayerClan(IReadOnlyPropertyOwner<CharacterAttribute> attributes)
        {
            Clan clan = Clan.PlayerClan;
            if (clan == null || attributes == null) return false;
            foreach (Hero hero in clan.Heroes)
                if (ReferenceEquals(hero.CharacterAttributes, attributes)) return true;
            return false;
        }
    }
}
