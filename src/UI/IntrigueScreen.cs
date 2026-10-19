using System;
using CheatMenu.Intrigue;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace CheatMenu.UI
{
    /// <summary>
    /// Entrika ekranı (yol haritası P1): harita üstünde tam ekran Gauntlet katmanı. Açıkken oyun durur.
    /// Açılış: kısayol tuşu (ayarlardan), haritadaki "Entrika" düğmesi, Insert menüsü. Esc ya da Kapat ile kapanır.
    /// Ekrandaki eylemler popup açabilir (onay, seçim); popup kapanınca ekran kendini yeniler.
    /// </summary>
    public static class IntrigueScreen
    {
        private const string Movie = "TvhIntrigueScreen";
        private const int Order = 400;

        private static readonly MovieLayer Layer = new MovieLayer();
        private static readonly EscapeKey Escape = new EscapeKey();
        private static readonly object PauseOwner = new object();
        private static bool _wasBusy;
        private static int _lastTab;

        public static bool IsOpen => Layer.IsOpen;

        public static void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }

        /// <param name="tab">-1: en son açık sekme.</param>
        public static void Open(int tab = -1)
        {
            if (IsOpen) return;
            string reason = CannotOpenReason();
            if (reason != null)
            {
                MenuUI.Error(reason);
                return;
            }
            TvhUi.PrepareBrushes();
            IntrigueScreenVM vm = null;
            try
            {
                vm = new IntrigueScreenVM(() => MenuUI.Defer(Close), tab >= 0 ? tab : _lastTab);
                if (!Layer.Open(Movie, vm, Order, true)) throw new InvalidOperationException("harita ekranı bulunamadı");
            }
            catch (Exception ex)
            {
                vm?.OnFinalize();
                Layer.Close();
                MenuUI.Error("Entrika ekranı açılamadı (" + ex.Message + "). Liste menüsü açılıyor.");
                Debug.Print("[CheatMenu] intrigue screen failed: " + ex);
                MenuUI.Defer(() => MenuUI.SafeRun("Entrika", Categories.ShowIntrigueMenu));
                return;
            }
            _wasBusy = false;
            Escape.Reset();
            TimePause.Acquire(PauseOwner);
        }

        public static void Close()
        {
            if (!IsOpen) return;
            if (Layer.DataSource is IntrigueScreenVM vm) _lastTab = vm.CurrentTab;
            Layer.Close();
            TimePause.Release(PauseOwner);
        }

        /// <summary>Ekranın üstünde açılan olay kartı kapanınca: klavye yeniden ekrana.</summary>
        public static void Refocus()
        {
            if (IsOpen) Layer.Refocus();
        }

        /// <summary>Her kare (SubModule): kapanma koşulları, Esc, popup sonrası yenileme.</summary>
        public static void Tick()
        {
            if (!IsOpen) return;
            try
            {
                if (!Layer.IsAlive || CannotOpenReason() != null)
                {
                    Close();
                    return;
                }
                bool busy = InformationManager.IsAnyInquiryActive() || EventCards.IsOpen || MenuUI.HasPending;
                if (Escape.Released(!busy))
                {
                    // Bir sonraki karede: bırakılan tuş bu karede alttaki haritaya ulaşmasın.
                    MenuUI.Defer(Close);
                    return;
                }
                if (!busy && Layer.DataSource is IntrigueScreenVM vm && (_wasBusy || vm.IsDirty)) vm.Refresh();
                _wasBusy = busy;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] intrigue screen tick failed: " + ex);
                MenuUI.Error("Entrika ekranı hata verdi ve kapatıldı: " + ex.Message);
                Close();
            }
        }

        /// <summary>null: ekran açılabilir.</summary>
        public static string CannotOpenReason()
        {
            if (Campaign.Current == null || IntrigueBehavior.Current == null) return "Entrika yalnızca kampanyada";
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState)) return "Entrika ekranı yalnızca haritadayken açılır";
            if (MapScreen.Instance == null) return "Harita ekranı hazır değil";
            if (Campaign.Current.ConversationManager?.IsConversationInProgress == true) return "Konuşma sırasında açılmaz";
            return null;
        }
    }
}
