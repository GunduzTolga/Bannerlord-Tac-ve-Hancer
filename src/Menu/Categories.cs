using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using TaleWorlds.CampaignSystem;
// Oyunun konsol hileleri de TaleWorlds.CampaignSystem.CampaignCheats adında; onlar kayda "hile kullanıldı"
// işareti koyar. Burada her zaman bizim sınıfımız kullanılsın.
using CampaignCheats = CheatMenu.Cheats.CampaignCheats;

namespace CheatMenu.Menu
{
    /// <summary>
    /// Ana menü ve kategoriler. Her ShowX listeyi her seferinde yeniden kurar;
    /// böylece aç/kapa etiketleri ve güncel değerler doğru görünür.
    /// Bu dosya: kök, gruplar ve hileler. Categories.World.cs: Harita ve Adlandır. Categories.Editors.cs: editörler.
    /// Categories.Intrigue.cs: Entrika.
    /// </summary>
    public static partial class Categories
    {
        public static void ShowRoot()
        {
            MenuUI.ShowList(ModInfo.DisplayName.ToUpperInvariant(), new List<MenuEntry>
            {
                MenuEntry.Submenu("Entrika", ShowIntrigue),
                MenuEntry.Submenu("Hileler", ShowCheats),
                MenuEntry.Submenu("Editörler", ShowEditors),
                MenuEntry.Submenu("Klan & krallık", ShowPolitics),
                MenuEntry.Submenu("Bilgi", ShowInfo),
                MenuEntry.Submenu("Kurallar", ShowRules),
            }, ShowRoot, "Kapat", null);
        }

        private static void ShowCheats()
        {
            ShowCategory("Hileler", new List<MenuEntry>
            {
                MenuEntry.Submenu("Para & Ekonomi", ShowEconomy),
                MenuEntry.Submenu("Karakter", ShowCharacter),
                MenuEntry.Submenu("Ordu / Parti", ShowParty),
                MenuEntry.Submenu("Savaş içi", ShowCombat),
                MenuEntry.Submenu("Harita", ShowMap),
            }, ShowCheats);
        }

        private static void ShowEditors()
        {
            ShowCategory("Editörler", new List<MenuEntry>
            {
                MenuEntry.Submenu("Adlandır", ShowRename),
                MenuEntry.Submenu("Kahraman editörü", ShowHeroEditorPick),
                MenuEntry.Submenu("İlişkiler", ShowRelationPick),
                MenuEntry.Submenu("Yerleşim editörü", ShowSettlementPick),
                MenuEntry.Submenu("Eşya ekle", ShowItemCategories),
            }, ShowEditors);
        }

        private static void ShowEconomy()
        {
            ShowCategory("Para & Ekonomi", new List<MenuEntry>
            {
                MenuEntry.Simple("+1.000 altın", () => CampaignCheats.AddGold(1000)),
                MenuEntry.Simple("+10.000 altın", () => CampaignCheats.AddGold(10000)),
                MenuEntry.Simple("+100.000 altın", () => CampaignCheats.AddGold(100000)),
                MenuEntry.Custom("Özel miktar altın...", CampaignCheats.AddGold),
                MenuEntry.Simple("+100 etki", () => CampaignCheats.AddInfluence(100)),
                MenuEntry.Simple("+500 etki", () => CampaignCheats.AddInfluence(500)),
                MenuEntry.Custom("Özel miktar etki...", CampaignCheats.AddInfluence),
                MenuEntry.Simple("+100 tahıl", () => CampaignCheats.AddGrain(100)),
            }, ShowEconomy, back: ShowCheats);
        }

        private static void ShowCharacter()
        {
            ShowCategory("Karakter", new List<MenuEntry>
            {
                MenuEntry.Simple("+5 odak puanı", () => CampaignCheats.AddFocusPoints(5)),
                MenuEntry.Custom("Özel miktar odak puanı...", CampaignCheats.AddFocusPoints),
                MenuEntry.Simple("+2 attribute puanı", () => CampaignCheats.AddAttributePoints(2)),
                MenuEntry.Custom("Özel miktar attribute puanı...", CampaignCheats.AddAttributePoints),
                MenuEntry.Simple("Tüm yeteneklere +25", () => CampaignCheats.AddAllSkills(25)),
                MenuEntry.Simple("Canı doldur", CampaignCheats.HealHero),
            }, ShowCharacter, back: ShowCheats);
        }

        private static void ShowParty()
        {
            ShowCategory("Ordu / Parti", new List<MenuEntry>
            {
                MenuEntry.Submenu("Asker ekle (kademeye göre)...", ShowTroopTiers),
                MenuEntry.Simple("+10 temel kültür askeri", () => CampaignCheats.AddBasicTroops(10)),
                MenuEntry.Simple("+50 temel kültür askeri", () => CampaignCheats.AddBasicTroops(50)),
                MenuEntry.Custom("Özel sayıda temel asker...", CampaignCheats.AddBasicTroops),
                MenuEntry.Simple("+10 elit kültür askeri", () => CampaignCheats.AddEliteTroops(10)),
                MenuEntry.Simple("Tüm askerlere terfi XP'si", CampaignCheats.GiveUpgradeXp),
                MenuEntry.Simple("Yaralıları iyileştir", CampaignCheats.HealWounded),
                MenuEntry.Simple("+50 moral", () => CampaignCheats.AddMorale(50)),
                MenuEntry.Toggle("Parti hızı x2", () => CheatState.FastParty, v => CheatState.FastParty = v),
            }, ShowParty, back: ShowCheats);
        }

        private static void ShowTroopTiers()
        {
            List<CharacterObject> troops = TroopSpawner.All().ToList();
            var entries = new List<MenuEntry>
            {
                MenuEntry.Submenu("Tüm kademeler (" + troops.Count + " asker türü) - isimle ara...", () => PickTroop(null)),
            };
            entries.AddRange(troops
                .GroupBy(troop => troop.Tier)
                .OrderBy(group => group.Key)
                .Select(group => MenuEntry.Submenu(TroopSpawner.TierLabel(group.Key) + " - " + group.Count() + " asker türü...",
                    () => PickTroop(group.Key))));

            MenuUI.ShowList("Asker ekle", entries, ShowTroopTiers, "Geri", ShowParty,
                "Kademe yükseldikçe asker güçlenir. Kültür askerleri, paralı askerler ve haydutlar dahil. "
                + "Parti sınırını aşmak oyunda cezaya yol açar (sınırı artırmak için: Kurallar > Parti boyutu bonusu).");
        }

        private static void PickTroop(int? tier)
        {
            // "Tüm kademeler" resimsiz: yüzlerce 3D asker resmi aynı anda çizilmesin.
            bool all = !tier.HasValue;
            Picker.Pick(all ? "Tüm askerler" : TroopSpawner.TierLabel(tier.Value),
                all ? TroopSpawner.All() : TroopSpawner.OfTier(tier.Value),
                all ? TroopSpawner.LabelWithTier : TroopSpawner.Label,
                troop => MenuUI.AskNumberInRange(troop.Name + " - adet", 1, 9999, 10,
                    count => CampaignCheats.AddTroops(troop, count), () => PickTroop(tier)),
                ShowTroopTiers, all ? null : TroopSpawner.Image);
        }

        private static void ShowCombat()
        {
            ShowCategory("Savaş içi", new List<MenuEntry>
            {
                MenuEntry.Toggle("Ölümsüzlük (Ben)", () => CheatState.GodModeMe, v => CheatState.GodModeMe = v),
                MenuEntry.Toggle("Ölümsüzlük (Ordu)", () => CheatState.GodModeArmy, v => CheatState.GodModeArmy = v),
                MenuEntry.Toggle("Tek vuruş (Ben)", () => CheatState.OneHitMe, v => CheatState.OneHitMe = v),
                MenuEntry.Toggle("Tek vuruş (Ordu)", () => CheatState.OneHitArmy, v => CheatState.OneHitArmy = v),
                MenuEntry.Toggle("Sınırsız cephane (Ben)", () => CheatState.InfiniteAmmoMe, v => CheatState.InfiniteAmmoMe = v),
                MenuEntry.Toggle("Sınırsız cephane (Ordu)", () => CheatState.InfiniteAmmoArmy, v => CheatState.InfiniteAmmoArmy = v),
                MenuEntry.Toggle("(Ordu) hilelerine müttefikleri de kat", () => CheatState.ArmyIncludesAllies,
                    v => CheatState.ArmyIncludesAllies = v),
            }, ShowCombat, back: ShowCheats);
        }

        /// <param name="back">"Geri" ile dönülecek menü; verilmezse ana menü.</param>
        private static void ShowCategory(string title, List<MenuEntry> entries, Action reshow, string description = "",
            Action back = null) =>
            MenuUI.ShowList(title, entries, reshow, "Geri", back ?? ShowRoot, description);
    }
}
