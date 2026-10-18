using System;
using CheatMenu.Menu;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Kahraman düzenleme. Oyunun konsol hilelerinin içinde kullandığı eylemler doğrudan çağrılır
    /// (konsol hileleri değil; onlar kayda "hile kullanıldı" işareti koyar). Hepsi oyunun kendi kayıt alanlarına yazar.
    /// </summary>
    public static class HeroEditor
    {
        public const int MaxSkill = 330;

        public static int MinAge => Campaign.Current.Models.AgeModel.HeroComesOfAge;
        public static int MaxAge => Campaign.Current.Models.AgeModel.MaxAge;
        public static int MaxAttribute => Campaign.Current.Models.CharacterDevelopmentModel.MaxAttribute;
        public static int MaxFocus => Campaign.Current.Models.CharacterDevelopmentModel.MaxFocusPerSkill;

        public static void SetAge(Hero hero, int age)
        {
            hero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge(age));
            MenuUI.Success(hero.Name + " artık " + (int)hero.Age + " yaşında");
        }

        // campaign.set_skill_player ile aynı iki adım.
        public static void SetSkill(Hero hero, SkillObject skill, int value)
        {
            hero.HeroDeveloper.SetInitialSkillLevel(skill, value);
            hero.HeroDeveloper.InitializeSkillXp(skill);
            MenuUI.Success(hero.Name + ": " + skill.Name + " = " + hero.GetSkillValue(skill));
        }

        public static void SetAllSkills(Hero hero, int value)
        {
            foreach (SkillObject skill in Skills.All)
            {
                hero.HeroDeveloper.SetInitialSkillLevel(skill, value);
                hero.HeroDeveloper.InitializeSkillXp(skill);
            }
            MenuUI.Success(hero.Name + ": tüm yetenekler " + value);
        }

        public static void SetAttribute(Hero hero, CharacterAttribute attribute, int value)
        {
            int delta = value - hero.GetAttributeValue(attribute);
            if (delta > 0) hero.HeroDeveloper.AddAttribute(attribute, delta, false);
            else if (delta < 0) hero.HeroDeveloper.RemoveAttribute(attribute, -delta);
            MenuUI.Success(hero.Name + ": " + attribute.Name + " = " + hero.GetAttributeValue(attribute));
        }

        public static void SetFocus(Hero hero, SkillObject skill, int value)
        {
            int delta = value - hero.HeroDeveloper.GetFocus(skill);
            if (delta > 0) hero.HeroDeveloper.AddFocus(skill, delta, false);
            else if (delta < 0) hero.HeroDeveloper.RemoveFocus(skill, -delta);
            MenuUI.Success(hero.Name + ": " + skill.Name + " odağı = " + hero.HeroDeveloper.GetFocus(skill));
        }

        public static void AddPerk(Hero hero, PerkObject perk)
        {
            hero.HeroDeveloper.AddPerk(perk);
            MenuUI.Success(hero.Name + " perk kazandı: " + perk.Name);
        }

        // campaign.set_player_trait ile aynı: ana kahramanda trait XP'si de güncellenir, yoksa seviye geri kayabilir.
        public static void SetTrait(Hero hero, TraitObject trait, int value)
        {
            int previous = hero.GetTraitLevel(trait);
            hero.SetTraitLevel(trait, value);
            if (hero == Hero.MainHero)
            {
                TraitLevelingHelper.UpdateTraitXPAccordingToTraitLevels();
                CampaignEventDispatcher.Instance.OnPlayerTraitChanged(trait, previous);
            }
            MenuUI.Success(hero.Name + ": " + trait.Name + " = " + hero.GetTraitLevel(trait));
        }

        /// <summary>Oyunun berber ekranı; yalnızca ana kahraman (ekran Hero.MainHero'ya bağlı).</summary>
        public static void OpenBarber()
        {
            if (Hero.MainHero == null) throw new InvalidOperationException("Ana karakter bulunamadı");
            if (Mission.Current != null) throw new InvalidOperationException("Sahne içindeyken berber ekranı açılamaz");
            GameStateManager states = Game.Current.GameStateManager;
            states.PushState(states.CreateState<BarberState>(Hero.MainHero.CharacterObject, CharacterHelper.GetFaceGeneratorFilter()), 0);
        }
    }
}
