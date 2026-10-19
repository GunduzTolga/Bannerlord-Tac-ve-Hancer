using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace CheatMenu.UI
{
    /// <summary>
    /// Resimli olay kartları (yol haritası P1): entrika olayları, kararlar ve sonuç bildirimleri düz popup yerine
    /// solda resim, sağda metin ve seçeneklerle gösterilir. Resim Images klasöründe yoksa kişinin portresi görünür.
    /// Kart gösterilemezse (ayar kapalı, arayüz hatası) aynı seçenekler oyunun seçim penceresiyle sorulur.
    /// </summary>
    public static class EventCards
    {
        private const string Movie = "TvhEventCard";
        private const int Order = 410;

        public sealed class CardOption
        {
            public string Label;
            /// <summary>null: seçilebilir; değilse neden seçilemediği (gri görünür).</summary>
            public string DisabledHint;
            public Action Apply;
        }

        private static readonly MovieLayer Layer = new MovieLayer();
        private static string _title;
        private static Action _onClosed;
        private static CardOption _dismiss;
        private static bool _choosing;
        private static bool _failedOnce;

        public static bool IsOpen => Layer.IsOpen;

        /// <summary>Kartı gösterir; olmazsa seçim penceresine düşer. Seçilen seçenek bir sonraki karede uygulanır.</summary>
        /// <param name="dismiss">Esc ile seçilecek seçenek (bildirimlerde "Tamam"); null ise seçim zorunlu.</param>
        /// <param name="onClosed">Kart ya da pencere kapanınca (seçimden önce) çağrılır.</param>
        public static void Show(string caption, string title, string text, string image, Hero hero, List<CardOption> options,
            CardOption dismiss, Action onClosed)
        {
            if (TryShow(caption, title, text, image, hero, options, dismiss, onClosed)) return;
            ShowInquiry(title, text, options, dismiss, onClosed);
        }

        public static bool TryShow(string caption, string title, string text, string image, Hero hero, List<CardOption> options,
            CardOption dismiss, Action onClosed)
        {
            if (IsOpen || !SettingsStore.Current.EventCards || IntrigueScreen.CannotOpenReason() != null) return false;
            if (!TvhUi.ImageWidgetAvailable())
            {
                if (!_failedOnce) Debug.Print("[CheatMenu] TvhImageWidget is not registered; event cards fall back to inquiries");
                _failedOnce = true;
                return false;
            }
            TvhUi.PrepareBrushes();
            EventCardVM vm = null;
            try
            {
                vm = new EventCardVM(caption, title, text, image, hero, options, Choose);
                if (!Layer.Open(Movie, vm, Order, true)) return false;
            }
            catch (Exception ex)
            {
                vm?.OnFinalize();
                Layer.Close();
                Debug.Print("[CheatMenu] event card failed: " + ex);
                if (!_failedOnce) MenuUI.Error("Olay kartı gösterilemedi (" + ex.Message + "); düz pencere kullanılıyor.");
                _failedOnce = true;
                return false;
            }
            _title = title;
            _onClosed = onClosed;
            _dismiss = dismiss;
            _choosing = false;
            return true;
        }

        /// <summary>Suikast ya da kaçırmada "harekete geç": rüşvet seçimi kartı (Entrika ekranından).</summary>
        public static void ShowExecuteChoice(Scheme scheme, Action afterwards)
        {
            Hero target = Hero.Find(scheme.TargetId);
            if (target == null) return;
            string blocked = SchemeEngine.CannotExecuteReason(scheme);
            List<CardOption> options = SchemeEngine.BribeChoices(scheme)
                .Select(choice => new CardOption
                {
                    Label = choice.Label,
                    DisabledHint = blocked ?? (choice.Affordable ? null : "Yeterli altının yok"),
                    Apply = () =>
                    {
                        SchemeEngine.Execute(scheme, choice.Level);
                        afterwards?.Invoke();
                    },
                })
                .ToList();
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            bool kidnap = scheme.Kind == SchemeKind.Kidnap;
            Show(SchemeRules.Name(scheme.Kind).ToUpper(Tr.Culture) + " · KARAR", "Harekete geç: " + target.Name,
                "Aşama " + scheme.Phase + "/" + SchemeRules.MaxPhases(scheme.Kind) + " tamamlandı. "
                + (scheme.Phase < SchemeRules.MaxPhases(scheme.Kind) ? "Beklersen başarı artar, ama her aşamada yakalanma riski var.\n\n" : "\n\n")
                + "Rüşvet başarıyı artırır ama geri gelmez. Zar şimdi atılacak.",
                kidnap ? CardImages.Kidnap : CardImages.Murder, target, options, cancel, null);
        }

        /// <summary>Her kare (SubModule): harita ekranı değiştiyse kartı kapatır, Esc ile kapatılabilir kartları kapatır.</summary>
        public static void Tick()
        {
            if (!IsOpen) return;
            try
            {
                if (!Layer.IsAlive)
                {
                    Action closed = _onClosed;
                    Close();
                    closed?.Invoke();
                    return;
                }
                if (_dismiss != null && !_choosing && !InformationManager.IsAnyInquiryActive() && Input.IsKeyPressed(InputKey.Escape))
                    Choose(_dismiss);
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] event card tick failed: " + ex);
                Action closed = _onClosed;
                Close();
                closed?.Invoke();
            }
        }

        /// <summary>Kampanya kapanırken: açık kart sessizce kapanır (kararlar kayıtta bekler, sonra yine sorulur).</summary>
        public static void CloseForGameEnd()
        {
            _onClosed = null;
            _dismiss = null;
            _choosing = false;
            Layer.Close();
        }

        private static void Choose(CardOption option)
        {
            if (_choosing) return;
            _choosing = true;
            // Tıklama olayı sürerken katmanı kaldırmamak için bir sonraki karede.
            MenuUI.Defer(() =>
            {
                string title = _title;
                Action closed = _onClosed;
                Close();
                closed?.Invoke();
                MenuUI.SafeRun(title ?? "Olay", option.Apply);
            });
        }

        private static void Close()
        {
            _onClosed = null;
            _dismiss = null;
            _choosing = false;
            if (!Layer.IsOpen) return;
            Layer.Close();
            IntrigueScreen.Refocus();
        }

        private static void ShowInquiry(string title, string text, List<CardOption> options, CardOption dismiss, Action onClosed)
        {
            List<InquiryElement> elements = options
                .Select(o => new InquiryElement(o, o.Label, null, o.DisabledHint == null, o.DisabledHint ?? string.Empty))
                .ToList();
            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(title, text, elements, dismiss != null, 1, 1, "Seç", dismiss != null ? "Kapat" : string.Empty,
                    selected =>
                    {
                        onClosed?.Invoke();
                        if (selected != null && selected.Count > 0 && selected[0].Identifier is CardOption option)
                            MenuUI.SafeRun(title, option.Apply);
                    },
                    _ =>
                    {
                        onClosed?.Invoke();
                        if (dismiss != null) MenuUI.SafeRun(title, dismiss.Apply);
                    }),
                true, false);
        }
    }
}
