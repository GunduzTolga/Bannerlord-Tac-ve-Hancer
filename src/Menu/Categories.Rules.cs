using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using CheatMenu.Intrigue.Core;

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
                MenuEntry.Toggle("Yoldaş sınırını kaldır", () => Rules.Settings.NoCompanionLimit,
                    v => Rules.Change(s => s.NoCompanionLimit = v, null)),
                MenuEntry.Range("Parti sayısı sınırına ekle: +" + Rules.Settings.PartyLimitBonus, 0, ModSettings.MaxPartyLimitBonus,
                    () => Rules.Settings.PartyLimitBonus,
                    v => Rules.Change(s => s.SetPartyLimitBonus(v), "Parti sayısı sınırına ekleme: +" + v)),
                MenuEntry.Toggle("Parti sayısı sınırını kaldır", () => Rules.Settings.NoPartyLimit,
                    v => Rules.Change(s => s.NoPartyLimit = v, null)),
                Choice("Entrika zorluğu", ModSettings.AllowedIntrigueDifficulties, () => Rules.Settings.IntrigueDifficulty,
                    v => Rules.Change(s => s.SetIntrigueDifficulty(v), "Entrika zorluğu: " + SchemeMath.DifficultyName(v)),
                    SchemeMath.DifficultyName),
            }, ShowRules, "Ayarlar kalıcıdır ve anında etkilidir; kayıt dosyasına bir şey yazılmaz. "
                + "Parti sayısı sınırı = klanının aynı anda kurabileceği parti sayısı (kaldırma açıkken ekleme yok sayılır).");
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
