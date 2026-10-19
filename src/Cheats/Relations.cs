using System;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Cheats
{
    /// <summary>Oyuncunun bir kahramanla ilişkisi. Tam değer Hero.SetPersonalRelation ile (oyun sınırlara kırpar).</summary>
    public static class Relations
    {
        public static int Min => Campaign.Current.Models.DiplomacyModel.MinRelationLimit;
        public static int Max => Campaign.Current.Models.DiplomacyModel.MaxRelationLimit;

        public static int Get(Hero hero) => Hero.MainHero.GetRelation(hero);

        public static void Set(Hero hero, int value)
        {
            Hero.MainHero.SetPersonalRelation(hero, Math.Max(Min, Math.Min(Max, value)));
            MenuUI.Success(hero.Name + " ile ilişki: " + Get(hero));
        }

        public static void Add(Hero hero, int delta) => Set(hero, Get(hero) + delta);
    }
}
