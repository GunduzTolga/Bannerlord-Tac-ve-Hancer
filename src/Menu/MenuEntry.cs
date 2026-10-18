using System;

namespace CheatMenu.Menu
{
    /// <summary>
    /// Menüdeki tek satır. Run, menüyü yeniden açan "reopen" geri çağrısını alır; böylece kendi
    /// popup'ını açan eylemler (özel miktar) işleri bitince menüye kendileri dönebilir.
    /// </summary>
    public sealed class MenuEntry
    {
        private MenuEntry(string label, Action<Action> run)
        {
            Label = label;
            Run = run;
        }

        public string Label { get; }
        public Action<Action> Run { get; }

        /// <summary>Tek tıkla çalışan eylem; ardından aynı menü yeniden açılır.</summary>
        public static MenuEntry Simple(string label, Action action) =>
            new MenuEntry(label, reopen =>
            {
                MenuUI.SafeRun(label, action);
                reopen();
            });

        /// <summary>Sayı soran eylem; girilen miktarla apply çalışır, ardından menü yeniden açılır.</summary>
        public static MenuEntry Custom(string label, Action<int> apply) =>
            new MenuEntry(label, reopen => MenuUI.AskNumber(label, apply, reopen));

        /// <summary>Ad soran eylem; kutu mevcut adla dolu açılır, onaylanınca apply çalışır.</summary>
        public static MenuEntry Text(string label, Func<string> currentName, bool allowEmpty, Action<string> apply) =>
            new MenuEntry(label, reopen => MenuUI.AskName(label, currentName(), allowEmpty, apply, reopen));

        /// <summary>Aralıklı sayı soran eylem; kutu mevcut değerle dolu açılır.</summary>
        public static MenuEntry Range(string label, int min, int max, Func<int> current, Action<int> apply) =>
            new MenuEntry(label, reopen => MenuUI.AskNumberInRange(label, min, max, current(), apply, reopen));

        /// <summary>Aç/kapa anahtarı; etiket menü açıldığı andaki durumu gösterir.</summary>
        public static MenuEntry Toggle(string name, Func<bool> get, Action<bool> set)
        {
            string label = name + ": " + (get() ? "[AÇIK]" : "[KAPALI]");
            return new MenuEntry(label, reopen =>
            {
                bool now = !get();
                set(now);
                MenuUI.Success(name + (now ? " açıldı" : " kapatıldı"));
                reopen();
            });
        }

        /// <summary>Alt menüye geçer.</summary>
        public static MenuEntry Submenu(string label, Action show) =>
            new MenuEntry(label, _ => MenuUI.Defer(show));
    }
}
