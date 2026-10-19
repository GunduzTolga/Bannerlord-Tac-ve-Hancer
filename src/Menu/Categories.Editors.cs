using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace CheatMenu.Menu
{
    /// <summary>Kahraman editörü, ilişkiler, yerleşim editörü, eşya ekleme.</summary>
    public static partial class Categories
    {
        // ---- Kahraman editörü ----

        private static void ShowHeroEditorPick()
        {
            var heroes = new List<Hero> { Hero.MainHero };
            heroes.AddRange(Lists.LordsAndCompanions(true).OrderBy(h => h.Name.ToString(), StringComparer.CurrentCultureIgnoreCase));
            Picker.Pick("Kahraman editörü", heroes,
                h => h == Hero.MainHero ? "[Ben] " + h.Name : Lists.HeroLabel(h),
                ShowHero, ShowEditors, sort: false);
        }

        private static void ShowHero(Hero hero)
        {
            var entries = new List<MenuEntry>
            {
                MenuEntry.Range("Yaş: " + (int)hero.Age, HeroEditor.MinAge, HeroEditor.MaxAge,
                    () => (int)hero.Age, age => HeroEditor.SetAge(hero, age)),
                MenuEntry.Submenu("Bir yeteneği ayarla...", () => PickSkill(hero, "Yetenek",
                    skill => hero.GetSkillValue(skill), HeroEditor.MaxSkill, (skill, v) => HeroEditor.SetSkill(hero, skill, v))),
                MenuEntry.Range("Tüm yetenekleri ayarla...", 0, HeroEditor.MaxSkill, () => 100,
                    v => HeroEditor.SetAllSkills(hero, v)),
                MenuEntry.Submenu("Bir niteliği ayarla...", () => PickAttribute(hero)),
                MenuEntry.Submenu("Bir yeteneğin odağını ayarla...", () => PickSkill(hero, "Odak",
                    skill => hero.HeroDeveloper.GetFocus(skill), HeroEditor.MaxFocus, (skill, v) => HeroEditor.SetFocus(hero, skill, v))),
                MenuEntry.Submenu("Perk ekle...", () => PickPerk(hero)),
                MenuEntry.Submenu("Kişilik özelliğini ayarla...", () => PickTrait(hero)),
            };
            if (hero == Hero.MainHero)
                entries.Add(MenuEntry.Submenu("Görünümümü düzenle (berber ekranı)", HeroEditor.OpenBarber));

            MenuUI.ShowList("Kahraman: " + hero.Name, entries, () => ShowHero(hero), "Geri", ShowHeroEditorPick);
        }

        private static void PickSkill(Hero hero, string what, Func<SkillObject, int> current, int max,
            Action<SkillObject, int> apply)
        {
            Picker.Pick(hero.Name + " - " + what, Skills.All, skill => skill.Name + ": " + current(skill),
                skill => MenuUI.AskNumberInRange(what + ": " + skill.Name, 0, max, current(skill),
                    v => apply(skill, v), () => ShowHero(hero)),
                () => ShowHero(hero), sort: false);
        }

        private static void PickAttribute(Hero hero)
        {
            Picker.Pick(hero.Name + " - Nitelik", Attributes.All, a => a.Name + ": " + hero.GetAttributeValue(a),
                a => MenuUI.AskNumberInRange("Nitelik: " + a.Name, 0, HeroEditor.MaxAttribute, hero.GetAttributeValue(a),
                    v => HeroEditor.SetAttribute(hero, a, v), () => ShowHero(hero)),
                () => ShowHero(hero), sort: false);
        }

        private static void PickPerk(Hero hero)
        {
            Picker.Pick(hero.Name + " - Perk ekle", PerkObject.All.Where(p => !hero.GetPerkValue(p)),
                p => p.Name + " (" + p.Skill?.Name + " " + (int)p.RequiredSkillValue + ")",
                p =>
                {
                    MenuUI.SafeRun("Perk ekle", () => HeroEditor.AddPerk(hero, p));
                    MenuUI.Defer(() => PickPerk(hero));
                },
                () => ShowHero(hero));
        }

        private static void PickTrait(Hero hero)
        {
            Picker.Pick(hero.Name + " - Kişilik", DefaultTraits.Personality, t => t.Name + ": " + hero.GetTraitLevel(t),
                t => MenuUI.AskNumberInRange("Kişilik: " + t.Name, t.MinValue, t.MaxValue, hero.GetTraitLevel(t),
                    v => HeroEditor.SetTrait(hero, t, v), () => ShowHero(hero)),
                () => ShowHero(hero), sort: false);
        }

        // ---- İlişkiler ----

        private static void ShowRelationPick()
        {
            Picker.Pick("İlişkiler", Lists.LordsAndCompanions(false).Concat(Lists.Notables()),
                h => Lists.HeroLabel(h) + " [" + Relations.Get(h) + "]", ShowRelation, ShowEditors);
        }

        private static void ShowRelation(Hero hero)
        {
            MenuUI.ShowList(hero.Name + " ile ilişki: " + Relations.Get(hero), new List<MenuEntry>
            {
                MenuEntry.Simple("+10", () => Relations.Add(hero, 10)),
                MenuEntry.Simple("+50", () => Relations.Add(hero, 50)),
                MenuEntry.Simple("-10", () => Relations.Add(hero, -10)),
                MenuEntry.Simple("Tam dost (" + Relations.Max + ")", () => Relations.Set(hero, Relations.Max)),
                MenuEntry.Simple("Tam düşman (" + Relations.Min + ")", () => Relations.Set(hero, Relations.Min)),
                MenuEntry.Range("Özel değer...", Relations.Min, Relations.Max, () => Relations.Get(hero),
                    v => Relations.Set(hero, v)),
            }, () => ShowRelation(hero), "Geri", ShowRelationPick,
                "Not: oyun bazı ilişkileri kişinin klan liderine yönlendirir.");
        }

        // ---- Yerleşim editörü ----

        private static void ShowSettlementPick() =>
            Picker.Pick("Yerleşim editörü", Lists.Settlements(), Lists.SettlementLabel, ShowSettlement, ShowEditors);

        private static void ShowSettlement(Settlement s)
        {
            var entries = new List<MenuEntry>();
            if (!s.IsVillage)
                entries.Add(MenuEntry.Submenu("Sahibini değiştir... (şu an: " + s.OwnerClan?.Name + ")", () => PickNewOwner(s)));
            if (s.Town != null)
            {
                Town town = s.Town;
                entries.Add(MenuEntry.Simple("Refah +1000 (şu an " + town.Prosperity.ToString("N0") + ")",
                    () => SettlementEditor.AddProsperity(s, 1000)));
                entries.Add(MenuEntry.Custom("Özel miktar refah...", v => SettlementEditor.AddProsperity(s, v)));
                entries.Add(MenuEntry.Simple("Sadakati tam yap (şu an " + town.Loyalty.ToString("0") + ")",
                    () => SettlementEditor.MaxLoyalty(s)));
                entries.Add(MenuEntry.Simple("Güvenliği tam yap (şu an " + town.Security.ToString("0") + ")",
                    () => SettlementEditor.MaxSecurity(s)));
                entries.Add(MenuEntry.Simple("Yiyecek deposunu doldur (şu an " + town.FoodStocks.ToString("0") + ")",
                    () => SettlementEditor.FillFood(s)));
                entries.Add(MenuEntry.Simple("Tüm inşaat projelerini bitir (" + town.BuildingsInProgress.Count + " proje)",
                    () => SettlementEditor.FinishAllProjects(s)));
            }
            if (s.Village != null)
            {
                entries.Add(MenuEntry.Simple("Hane +100 (şu an " + s.Village.Hearth.ToString("0") + ")",
                    () => SettlementEditor.AddHearth(s, 100)));
                entries.Add(MenuEntry.Custom("Özel miktar hane...", v => SettlementEditor.AddHearth(s, v)));
            }
            entries.Add(MenuEntry.Simple("Milis +50 (şu an " + s.Militia.ToString("0") + ")",
                () => SettlementEditor.AddMilitia(s, 50)));

            MenuUI.ShowList(s.Name.ToString(), entries, () => ShowSettlement(s), "Geri", ShowSettlementPick);
        }

        private static void PickNewOwner(Settlement s)
        {
            var clans = new List<Clan> { Clan.PlayerClan };
            clans.AddRange(Lists.Clans()
                .Where(c => c != Clan.PlayerClan && c.Leader != null)
                .OrderBy(c => c.Name.ToString(), StringComparer.CurrentCultureIgnoreCase));
            Picker.Pick(s.Name + " - yeni sahip", clans,
                c => c == Clan.PlayerClan ? "[Senin klanın] " + c.Name : Lists.ClanLabel(c),
                c =>
                {
                    MenuUI.SafeRun("Sahiplik", () => SettlementEditor.ChangeOwner(s, c));
                    MenuUI.Defer(() => ShowSettlement(s));
                },
                () => ShowSettlement(s), sort: false);
        }

        // ---- Eşya ekle ----

        private static void ShowItemCategories()
        {
            List<MenuEntry> entries = ItemSpawner.Categories
                .Select(category => MenuEntry.Submenu(category.Label + "...", () => PickItem(category)))
                .ToList();
            ShowCategory("Eşya ekle", entries, ShowItemCategories, back: ShowEditors);
        }

        private static void PickItem(ItemSpawner.Category category)
        {
            Picker.Pick(category.Label, ItemSpawner.ItemsOfType(category.Type), ItemSpawner.Label,
                item => MenuUI.AskNumberInRange(item.Name + " - adet", 1, 9999, 1,
                    count => ItemSpawner.Add(item, count), () => PickItem(category)),
                ShowItemCategories, ItemSpawner.Image);
        }
    }
}
