using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Menu
{
    public static class MenuUI
    {
        private static readonly Color GreenColor = Color.FromUint(0xFF7CD67C);
        private static readonly Color RedColor = Color.FromUint(0xFFFF6B6B);
        private static readonly Color YellowColor = Color.FromUint(0xFFFFD166);

        // Bir popup'ın callback'i içinden yeni popup açmak, kapanmakta olan popup'la çakışabilir.
        // Bu yüzden "sonraki popup" işleri kuyruğa alınır ve bir sonraki karede
        // (SubModule.OnApplicationTick → RunDeferred) çalıştırılır.
        private static readonly Queue<Action> Deferred = new Queue<Action>();

        /// <summary>Tek seçimli liste gösterir. Seçilen satır çalışır; reshow menüyü yeniden açar.</summary>
        public static void ShowList(string title, List<MenuEntry> entries, Action reshow, string backText, Action onBack,
            string description = "")
        {
            List<InquiryElement> elements = entries.Select(e => new InquiryElement(e, e.Label, null)).ToList();
            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(
                    title, description, elements,
                    true, 1, 1,
                    "Seç", backText,
                    selected => OnPicked(selected, reshow),
                    _ => Defer(onBack)),
                true, false);
        }

        /// <summary>Sayı sorar; geçerliyse apply çalışır. Her iki durumda da sonunda onDone çağrılır.</summary>
        public static void AskNumber(string title, Action<int> apply, Action onDone)
        {
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    title, "Bir miktar yaz:", true, true, "Uygula", "İptal",
                    text =>
                    {
                        if (AmountParser.TryParse(text, out int value)) SafeRun(title, () => apply(value));
                        else Error("Geçersiz miktar: " + text);
                        Defer(onDone);
                    },
                    () => Defer(onDone),
                    false,
                    text => AmountParser.TryParse(text, out _)
                        ? new Tuple<bool, string>(true, string.Empty)
                        : new Tuple<bool, string>(false, "Geçerli bir sayı yaz")),
                true, false);
        }

        /// <summary>[min, max] aralığında sayı sorar (eksi olabilir); kutu mevcut değerle dolu açılır.</summary>
        public static void AskNumberInRange(string title, int min, int max, int current, Action<int> apply, Action onDone)
        {
            string hint = min + " ile " + max + " arasında bir sayı yaz:";
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    title, hint, true, true, "Uygula", "İptal",
                    text =>
                    {
                        if (AmountParser.TryParseInRange(text, min, max, out int value)) SafeRun(title, () => apply(value));
                        else Error("Geçersiz değer: " + text);
                        Defer(onDone);
                    },
                    () => Defer(onDone),
                    false,
                    text => AmountParser.TryParseInRange(text, min, max, out _)
                        ? new Tuple<bool, string>(true, string.Empty)
                        : new Tuple<bool, string>(false, hint),
                    "",
                    current.ToString(CultureInfo.InvariantCulture)),
                true, false);
        }

        /// <summary>Ad sorar (kutu mevcut adla dolu). Geçerliyse apply çalışır; her durumda sonunda onDone çağrılır.</summary>
        public static void AskName(string title, string currentName, bool allowEmpty, Action<string> apply, Action onDone)
        {
            string hint = allowEmpty ? "Yeni adı yaz (boş bırakırsan orijinal ada döner):" : "Yeni adı yaz:";
            InformationManager.ShowTextInquiry(
                new TextInquiryData(
                    title, hint, true, true, "Uygula", "İptal",
                    text =>
                    {
                        if (NameRules.TryNormalize(text, allowEmpty, out string name, out string error)) SafeRun(title, () => apply(name));
                        else Error(error);
                        Defer(onDone);
                    },
                    () => Defer(onDone),
                    false,
                    text => NameRules.TryNormalize(text, allowEmpty, out _, out string error)
                        ? new Tuple<bool, string>(true, string.Empty)
                        : new Tuple<bool, string>(false, error),
                    "",
                    currentName ?? string.Empty),
                true, false);
        }

        /// <summary>Evet/Vazgeç onayı (geri alınamaz işlemler için). Seçim bir sonraki karede işlenir.</summary>
        public static void Confirm(string title, string text, Action onYes, Action onNo)
        {
            InformationManager.ShowInquiry(
                new InquiryData(title, text, true, true, "Evet", "Vazgeç",
                    () => Defer(() => SafeRun(title, onYes)),
                    () => Defer(onNo)),
                true, false);
        }

        /// <summary>
        /// Popup callback'lerinde yakalanmayan istisna oyunu çökertir; her eylem buradan geçer.
        /// </summary>
        public static void SafeRun(string what, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Error("'" + what + "' uygulanamadı: " + ex.Message);
                TaleWorlds.Library.Debug.Print("[CheatMenu] " + what + " failed: " + ex);
            }
        }

        public static void Defer(Action action)
        {
            if (action != null) Deferred.Enqueue(action);
        }

        /// <summary>Kuyruktaki işleri çalıştırır. Bu karede eklenenler bir sonraki kareye kalır.</summary>
        public static void RunDeferred()
        {
            int count = Deferred.Count;
            for (int i = 0; i < count; i++) SafeRun("Menü", Deferred.Dequeue());
        }

        public static void Success(string message) => Show(message, GreenColor);
        public static void Error(string message) => Show(message, RedColor);
        public static void Info(string message) => Show(message, YellowColor);

        private static void Show(string message, Color color) =>
            InformationManager.DisplayMessage(new InformationMessage("[CheatMenu] " + message, color));

        private static void OnPicked(List<InquiryElement> selected, Action reshow)
        {
            if (selected == null || selected.Count == 0 || !(selected[0].Identifier is MenuEntry entry)) return;
            SafeRun(entry.Label, () => entry.Run(() => Defer(reshow)));
        }
    }
}
