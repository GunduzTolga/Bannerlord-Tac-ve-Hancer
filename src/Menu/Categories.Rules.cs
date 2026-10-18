using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;

namespace CheatMenu.Menu
{
    /// <summary>Kurallar: kalıcı oran ayarları (config dosyasında, save'de değil).</summary>
    public static partial class Categories
    {
        private static void ShowRules()
        {
            ShowCategory("Kurallar", new List<MenuEntry>
            {
                Choice("Yetenek XP çarpanı (senin klanın)", ModSettings.AllowedXpMultipliers, () => Rules.Settings.SkillXpMultiplier,
                    v => Rules.Change(s => s.SetSkillXpMultiplier(v), "Yetenek XP çarpanı: " + v + "x"), v => v + "x"),
                Choice("Asker XP çarpanı (senin partin)", ModSettings.AllowedXpMultipliers, () => Rules.Settings.TroopXpMultiplier,
                    v => Rules.Change(s => s.SetTroopXpMultiplier(v), "Asker XP çarpanı: " + v + "x"), v => v + "x"),
                Choice("Parti boyutu bonusu", ModSettings.AllowedPartySizeBonuses, () => Rules.Settings.PartySizeBonus,
                    v => Rules.Change(s => s.SetPartySizeBonus(v), "Parti boyutu bonusu: +" + v), v => "+" + v),
                MenuEntry.Toggle("Öğrenme sınırını kaldır (senin klanın)", () => Rules.Settings.NoLearningLimit,
                    v => Rules.Change(s => s.NoLearningLimit = v, null)),
            }, ShowRules, "Ayarlar kalıcıdır ve anında etkilidir; kayıt dosyasına bir şey yazılmaz.");
        }

        private static MenuEntry Choice(string name, int[] options, Func<int> current, Action<int> set, Func<int, string> format) =>
            MenuEntry.Submenu(name + ": " + format(current()), () => ShowChoice(name, options, current, set, format));

        private static void ShowChoice(string name, int[] options, Func<int> current, Action<int> set, Func<int, string> format)
        {
            List<MenuEntry> entries = options
                .Select(option => MenuEntry.Simple(format(option) + (option == current() ? "  [SEÇİLİ]" : ""), () => set(option)))
                .ToList();
            MenuUI.ShowList(name, entries, () => ShowChoice(name, options, current, set, format), "Geri", ShowRules);
        }
    }
}
