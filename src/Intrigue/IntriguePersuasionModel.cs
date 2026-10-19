using System;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Conversation.Persuasion;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Oyunun kendi ikna konuşmaları (lordu krallığa davet etmek, evlilik, görevler) entrikayı tanır: konuştuğun kişi hakkında güncel
    /// raporun varsa her ikna denemesinde başarı +10 puan, bildiğin ve ifşa edilmemiş bir sırrı varsa +5 daha (LetterRules).
    /// Puan "başarısız"dan alınır; kritik başarı ve kritik başarısızlık oyunun hesabında kalır. Diğer üyeler önceki modele delege.
    /// </summary>
    public sealed class IntriguePersuasionModel : PersuasionModel
    {
        public override int GetSkillXpFromPersuasion(PersuasionDifficulty difficulty, int argumentDifficultyBonusCoefficient) =>
            BaseModel.GetSkillXpFromPersuasion(difficulty, argumentDifficultyBonusCoefficient);

        public override void GetChances(PersuasionOptionArgs optionArgs, out float successChance, out float critSuccessChance,
            out float critFailChance, out float failChance, float difficultyMultiplier)
        {
            BaseModel.GetChances(optionArgs, out successChance, out critSuccessChance, out critFailChance, out failChance, difficultyMultiplier);
            float bonus = Bonus() / 100f;
            if (bonus <= 0f) return;
            float moved = Math.Min(bonus, Math.Max(0f, failChance));
            successChance += moved;
            failChance -= moved;
        }

        public override void GetEffectChances(PersuasionOptionArgs option, out float moveToNextStageChance, out float blockRandomOptionChance,
            float difficultyMultiplier) =>
            BaseModel.GetEffectChances(option, out moveToNextStageChance, out blockRandomOptionChance, difficultyMultiplier);

        public override PersuasionArgumentStrength GetArgumentStrengthBasedOnTargetTraits(CharacterObject character, Tuple<TraitObject, int>[] traitCorrelation) =>
            BaseModel.GetArgumentStrengthBasedOnTargetTraits(character, traitCorrelation);

        public override float GetDifficulty(PersuasionDifficulty difficulty) => BaseModel.GetDifficulty(difficulty);

        public override float CalculateInitialPersuasionProgress(CharacterObject character, float goalValue, float successValue) =>
            BaseModel.CalculateInitialPersuasionProgress(character, goalValue, successValue);

        public override float CalculatePersuasionGoalValue(CharacterObject oneToOneConversationCharacter, float successValue) =>
            BaseModel.CalculatePersuasionGoalValue(oneToOneConversationCharacter, successValue);

        /// <summary>Konuştuğun kişi hakkındaki bilgine göre eklenecek puan (0-15).</summary>
        public static int Bonus() => Bonus(Hero.OneToOneConversationHero);

        public static int Bonus(Hero hero)
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || hero == null) return 0;
            bool report = state.HasValidReport(hero.StringId, SchemeFactors.Today);
            bool secret = state.Secrets.Any(s => s.TargetId == hero.StringId && !s.Exposed);
            return LetterRules.PersuasionBonus(report, secret);
        }
    }
}
