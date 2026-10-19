using System;
using System.Collections.Generic;
using CheatMenu.Cheats;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Menu
{
    /// <summary>Harita (ışınlanma, ileri sarma hızı) ve Adlandır.</summary>
    public static partial class Categories
    {
        private static void ShowMap()
        {
            var entries = new List<MenuEntry>
            {
                MenuEntry.Submenu("Yerleşime ışınlan...", PickSettlementToTeleport),
                MenuEntry.Submenu("Bir lordun yanına ışınlan...", PickHeroToTeleport),
            };
            foreach (int speed in ModSettings.AllowedSpeeds)
                entries.Add(MenuEntry.Simple(SpeedLabel(speed), () => TimeSpeed.Set(speed)));

            ShowCategory("Harita", entries, ShowMap,
                "İpucu: Ctrl+Alt + Sol tık ile haritada tıkladığın yere ışınlanırsın. "
                + "İleri sarma hızı yalnızca >> düğmesini etkiler.", ShowCheats);
        }

        private static string SpeedLabel(int speed) =>
            "İleri sarma hızı " + speed + "x" + (speed == ModSettings.DefaultSpeed ? " (normal)" : "")
            + (TimeSpeed.Current == speed ? "  [SEÇİLİ]" : "");

        private static void PickSettlementToTeleport()
        {
            if (TeleportBlocked()) return;
            Picker.Pick("Yerleşime ışınlan", Lists.Settlements(), Lists.SettlementLabel,
                s => MenuUI.SafeRun("Işınlanma", () => Teleport.ToSettlement(s)), ShowMap);
        }

        private static void PickHeroToTeleport()
        {
            if (TeleportBlocked()) return;
            Picker.Pick("Bir lordun yanına ışınlan", Lists.LordsAndCompanions(false), Lists.HeroLabel,
                h => MenuUI.SafeRun("Işınlanma", () => Teleport.ToHero(h)), ShowMap);
        }

        private static bool TeleportBlocked()
        {
            string blocker = Teleport.GetBlocker();
            if (blocker == null) return false;
            MenuUI.Error(blocker);
            MenuUI.Defer(ShowMap);
            return true;
        }

        private static void ShowRename()
        {
            ShowCategory("Adlandır", new List<MenuEntry>
            {
                MenuEntry.Text("Kendimi adlandır", () => Hero.MainHero.FirstName.ToString(), false,
                    name => Renamer.RenameHero(Hero.MainHero, name)),
                MenuEntry.Text("Partimi adlandır", () => MobileParty.MainParty.Name.ToString(), true,
                    name => Renamer.RenameParty(MobileParty.MainParty, name)),
                MenuEntry.Text("Bulunduğum yerleşimi adlandır", CurrentSettlementName, true,
                    name => Renamer.RenameSettlement(MobileParty.MainParty.CurrentSettlement, name)),
                MenuEntry.Submenu("Bir lord veya yoldaş...", () => PickAndRename("Lord veya yoldaş",
                    Lists.LordsAndCompanions(true), Lists.HeroLabel, h => h.FirstName.ToString(), false, Renamer.RenameHero)),
                MenuEntry.Submenu("Bir eşraf...", () => PickAndRename("Eşraf",
                    Lists.Notables(), Lists.HeroLabel, h => h.FirstName.ToString(), false, Renamer.RenameHero)),
                MenuEntry.Submenu("Bir klan...", () => PickAndRename("Klan",
                    Lists.Clans(), Lists.ClanLabel, c => c.Name.ToString(), false, Renamer.RenameClan)),
                MenuEntry.Submenu("Bir krallık...", () => PickAndRename("Krallık",
                    Lists.Kingdoms(), k => k.Name.ToString(), k => k.InformalName.ToString(), false, Renamer.RenameKingdom)),
                MenuEntry.Submenu("Bir yerleşim...", () => PickAndRename("Yerleşim",
                    Lists.Settlements(), Lists.SettlementLabel, s => s.Name.ToString(), true, Renamer.RenameSettlement)),
                MenuEntry.Submenu("Bir parti...", () => PickAndRename("Parti",
                    Lists.Parties(), Lists.PartyLabel, p => p.Name.ToString(), true, Renamer.RenameParty)),
            }, ShowRename, "Yerleşim ve partilerde adı boş bırakırsan orijinal ada döner. "
                + "Oyun bazı olaylarda parti adını kendisi sıfırlayabilir.", ShowEditors);
        }

        private static string CurrentSettlementName()
        {
            Settlement settlement = MobileParty.MainParty?.CurrentSettlement
                ?? throw new InvalidOperationException("Şu an bir yerleşimde değilsin");
            return settlement.Name.ToString();
        }

        private static void PickAndRename<T>(string kind, IEnumerable<T> items, Func<T, string> label,
            Func<T, string> currentName, bool allowEmpty, Action<T, string> rename) where T : class
        {
            string title = kind + " adlandır";
            Picker.Pick(title, items, label,
                item => MenuUI.AskName(title, currentName(item), allowEmpty, name => rename(item, name), ShowRename),
                ShowRename);
        }
    }
}
