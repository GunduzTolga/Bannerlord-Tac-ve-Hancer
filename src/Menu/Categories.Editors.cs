using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
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

        // ---- Aile editörü (P2.7) ----

        private static void ShowFamilyPick()
        {
            var heroes = new List<Hero> { Hero.MainHero };
            heroes.AddRange(Clan.PlayerClan.Heroes.Where(h => h.IsAlive && h != Hero.MainHero)
                .OrderBy(h => h.Age).ThenBy(h => h.Name.ToString(), StringComparer.CurrentCultureIgnoreCase));
            heroes.AddRange(Lists.LordsAndCompanions(true).Where(h => h.Clan != Clan.PlayerClan)
                .OrderBy(h => h.Name.ToString(), StringComparer.CurrentCultureIgnoreCase));
            Picker.Pick("Aile editörü", heroes,
                h => (h == Hero.MainHero ? "[Ben] " : h.Clan == Clan.PlayerClan ? "[Klanım] " : "") + h.Name + ", " + (int)h.Age + " yaş"
                    + (h.Spouse != null && h.Spouse.IsAlive ? ", eşi " + h.Spouse.Name : "") + (h.Clan != null ? " (" + h.Clan.Name + ")" : ""),
                ShowFamily, ShowEditors, sort: false);
        }

        private static void ShowFamily(Hero hero)
        {
            var entries = new List<MenuEntry>();
            Hero spouse = hero.Spouse != null && hero.Spouse.IsAlive ? hero.Spouse : null;
            void Add(string label, string blocked, Action action) =>
                entries.Add(blocked == null ? MenuEntry.Simple(label, action) : MenuEntry.Simple(label + " - " + blocked, () => MenuUI.Error(blocked)));

            if (spouse == null)
            {
                string blocked = FamilyEditor.MarryBlocked(hero);
                entries.Add(blocked == null ? MenuEntry.Submenu("Evlendir...", () => PickSpouseFor(hero))
                    : MenuEntry.Simple("Evlendir - " + blocked, () => MenuUI.Error(blocked)));
            }
            else
            {
                string blocked = FamilyEditor.DivorceBlocked(hero);
                bool player = hero == Hero.MainHero || spouse == Hero.MainHero;
                entries.Add(blocked != null ? MenuEntry.Simple("Boşa (" + spouse.Name + ") - " + blocked, () => MenuUI.Error(blocked))
                    : MenuEntry.Submenu("Boşa (" + spouse.Name + ")...", () =>
                    {
                        if (player) UI.ViewModels.DossierVM.ConfirmDivorce(() => MenuUI.Defer(() => ShowFamily(hero)));
                        else MenuUI.Confirm("Boşa", hero.Name + " ile " + spouse.Name + " boşansın mı? Evlilikle klana giren taraf ailesine döner.",
                            () => FamilyEditor.Divorce(hero), () => ShowFamily(hero));
                    }));
            }
            Hero mother = hero.IsFemale ? hero : spouse;
            if (mother != null)
                Add("Gebelik başlat (" + mother.Name + ", oyunun kendi gebeliği)", FamilyEditor.PregnancyBlocked(mother), () => FamilyEditor.StartPregnancy(mother));
            entries.Add(MenuEntry.Toggle("Çocuk istemiyorlar" + (spouse != null ? " (çift)" : ""), () => FamilyEditor.WantsNoChildren(hero),
                v => FamilyEditor.SetNoChildren(hero, v)));
            if (hero.IsChild)
            {
                entries.Add(MenuEntry.Simple("Bir yaş büyüt (" + (int)hero.Age + " yaş)", () => FamilyEditor.Grow(hero, 1)));
                entries.Add(MenuEntry.Simple("Yetişkinlik yaşına getir (" + FamilyEditor.ComesOfAge + ")", () => FamilyEditor.Grow(hero, FamilyEditor.ComesOfAge)));
            }
            entries.Add(MenuEntry.Toggle("Yaşlanmayı dondur", () => FamilyEditor.IsFrozen(hero), v => FamilyEditor.SetFrozen(hero, v)));
            if (hero == Hero.MainHero)
            {
                IntrigueState state = SchemeEngine.State;
                List<Hero> bastards = Dynasty.Bastards(state);
                if (bastards.Count > 0)
                    entries.Add(MenuEntry.Submenu("Gayrimeşru çocuğunu meşrulaştır (" + bastards.Count + ")...", () => PickBastard(hero)));
                string abdicate = Dynasty.AbdicateBlocked();
                entries.Add(abdicate == null ? MenuEntry.Submenu("Klanı devret: çekil ve varisinle devam et...", () => PickHeir(hero))
                    : MenuEntry.Simple("Klanı devret - " + abdicate, () => MenuUI.Error(abdicate)));
            }

            List<Hero> children = hero.Children.Where(c => c.IsAlive).ToList();
            MenuUI.ShowList("Aile: " + hero.Name, entries, () => ShowFamily(hero), "Geri", ShowFamilyPick,
                hero.Name + ", " + (int)hero.Age + " yaş. " + (spouse != null ? "Eşi: " + spouse.Name + ". " : "Evli değil. ")
                + (children.Count > 0 ? "Çocukları: " + string.Join(", ", children.Select(c => c.Name + " (" + (int)c.Age + ")")) + ". " : "")
                + "Evlilik ve gebelik oyunun kendi kurallarıyla olur: yaş, akrabalık, Doğum ve Ölüm ayarı.");
        }

        private static void PickBastard(Hero hero)
        {
            IntrigueState state = SchemeEngine.State;
            Picker.Pick("Meşrulaştır (" + Dynasty.CostText() + ")", Dynasty.Bastards(state),
                c => c.Name + ", " + (int)c.Age + " yaş (" + c.Clan?.Name + ")" + (Dynasty.LegitimizeBlocked(state, c) is string why ? "  [" + why + "]" : ""),
                c => MenuUI.Confirm("Meşrulaştır: " + c.Name,
                    c.Name + " klanına katılır ve senin çocuğun olarak anılır. Bedeli " + Dynasty.CostText() + ". Babalığın herkesçe bilinir: "
                    + "annesi ve onun ailesi skandala sürüklenir (gayrimeşru çocuk ifşasının sonuçları). Aile ağacında resmî ebeveyni değişmez. Onaylıyor musun?",
                    () => Dynasty.Legitimize(SchemeEngine.State, c), () => ShowFamily(hero)),
                () => ShowFamily(hero), sort: false);
        }

        private static void PickHeir(Hero hero)
        {
            Picker.Pick("Varisini seç (oyunun varis sıralamasıyla)", Dynasty.HeirCandidates(),
                h => h.Name + ", " + (int)h.Age + " yaş" + (h == Hero.MainHero.Spouse ? " (eşin)" : h.Father == Hero.MainHero || h.Mother == Hero.MainHero ? " (çocuğun)" : ""),
                h => MenuUI.Confirm("Klanı devret: " + h.Name,
                    "Karakterin emekliye ayrılır ve oyundan çekilir; oyuna " + h.Name + " olarak devam edersin. Bu oyunun kendi emeklilik işlemidir ve "
                    + "geri alınamaz. Önce kayıt al. Devredilsin mi?",
                    () => Dynasty.Abdicate(h), () => ShowFamily(hero)),
                () => ShowFamily(hero), sort: false);
        }

        private static void PickSpouseFor(Hero hero)
        {
            Picker.Pick(hero.Name + " kiminle evlensin?", FamilyEditor.MarriageCandidates(hero),
                h => h.Name + ", " + (int)h.Age + " yaş" + (h.Clan != null ? " (" + h.Clan.Name + ")" : ""),
                h =>
                {
                    MenuUI.SafeRun("Evlendir", () => FamilyEditor.Marry(hero, h));
                    MenuUI.Defer(() => ShowFamily(hero));
                },
                () => ShowFamily(hero), sort: false);
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
