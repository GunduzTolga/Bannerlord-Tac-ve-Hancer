using System;
using CheatMenu.Intrigue;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;
using TaleWorlds.TwoDimension;

namespace CheatMenu.UI
{
    /// <summary>
    /// Lordlar penceresi (lordlarla iletişim, ziyafet, yanına çağır): Insert > Entrika ve Entrika > Saray'dan. Haritanın ya da o an açık
    /// ekranın üstünde; Esc ve Kapat kapatır. Popup kapanınca yenilenir.
    /// </summary>
    public static class LordsScreen
    {
        private const string Movie = "TvhLords";
        // Ayrı Entrika ekranının (400) üstünde, olay kartlarının (410) altında.
        private const int Order = 405;

        private static readonly MovieLayer Layer = new MovieLayer();
        private static readonly EscapeKey Escape = new EscapeKey();
        private static readonly object PauseOwner = new object();
        private static bool _wasBusy;
        private static SpriteCategory _clanSprites;

        public static bool IsOpen => Layer.IsOpen;

        /// <summary>Açıldıysa true.</summary>
        /// <param name="host">null: harita.</param>
        public static bool Open(ScreenBase host = null)
        {
            if (SchemeEngine.State == null) return false;
            Close();
            TvhUi.PrepareBrushes();
            LordsVM vm = null;
            try
            {
                // Kartlar ve düğmeler Klan ekranı fırçalarından kopyalanır: o ekran kapalıyken sprite'larını biz yükleriz.
                _clanSprites = TvhUi.AcquireSprites(TvhUi.ClanSpriteCategory);
                vm = new LordsVM(() => MenuUI.Defer(Close));
                if (!Layer.Open(Movie, vm, Order, true, host)) throw new InvalidOperationException("ekran bulunamadı");
            }
            catch (Exception ex)
            {
                vm?.OnFinalize();
                Layer.Close();
                TvhUi.ReleaseSprites(_clanSprites);
                _clanSprites = null;
                Debug.Print("[CheatMenu] lords window failed: " + ex);
                MenuUI.Error("Lordlar penceresi açılamadı (" + ex.Message + ").");
                return false;
            }
            _wasBusy = false;
            Escape.Reset();
            TimePause.Acquire(PauseOwner);
            return true;
        }

        public static void Close()
        {
            if (!IsOpen) return;
            Layer.Close();
            TvhUi.ReleaseSprites(_clanSprites);
            _clanSprites = null;
            TimePause.Release(PauseOwner);
        }

        /// <summary>Her kare (SubModule): ekranı kapandıysa kapanır, Esc, popup sonrası yenileme.</summary>
        public static void Tick()
        {
            if (!IsOpen) return;
            try
            {
                if (!Layer.IsAlive)
                {
                    Close();
                    return;
                }
                bool busy = InformationManager.IsAnyInquiryActive() || EventCards.IsOpen || MenuUI.HasPending;
                if (Escape.Released(!busy))
                {
                    MenuUI.Defer(Close);
                    return;
                }
                if (!busy && _wasBusy && Layer.DataSource is LordsVM vm) vm.Refresh();
                if (!busy && _wasBusy) Layer.Refocus();
                _wasBusy = busy;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] lords window tick failed: " + ex);
                MenuUI.Error("Lordlar penceresi hata verdi ve kapatıldı: " + ex.Message);
                Close();
            }
        }
    }
}
