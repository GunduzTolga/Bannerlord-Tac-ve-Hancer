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
    /// Barış Masası penceresi (Intrigue.PeaceTreaties): Savaş Raporu'ndaki düğmeden, elçi kartından ya da Insert > Entrika'dan. Savaş
    /// Raporu'nun da üstünde; Esc ve Kapat kapatır. Popup kapanınca yenilenir.
    /// </summary>
    public static class PeaceTableScreen
    {
        private const string Movie = "TvhPeaceTable";
        // Savaş Raporu'nun (405) üstünde, olay kartlarının (410) altında.
        private const int Order = 407;

        private static readonly MovieLayer Layer = new MovieLayer();
        private static readonly EscapeKey Escape = new EscapeKey();
        private static readonly object PauseOwner = new object();
        private static bool _wasBusy;
        private static SpriteCategory _clanSprites;

        public static bool IsOpen => Layer.IsOpen;

        /// <summary>Açıldıysa true.</summary>
        /// <param name="initial">Masaya konmuş şartlar (elçinin teklifi); null: boş masa.</param>
        /// <param name="host">null: harita.</param>
        public static bool Open(Kingdom enemy, PeaceTreaties.Terms initial = null, ScreenBase host = null)
        {
            if (enemy == null || Clan.PlayerClan?.Kingdom == null || SchemeEngine.State == null) return false;
            Close();
            TvhUi.PrepareBrushes();
            PeaceTableVM vm = null;
            try
            {
                _clanSprites = TvhUi.AcquireSprites(TvhUi.ClanSpriteCategory);
                vm = new PeaceTableVM(enemy, initial, () => MenuUI.Defer(Close));
                if (!Layer.Open(Movie, vm, Order, true, host)) throw new InvalidOperationException("ekran bulunamadı");
            }
            catch (Exception ex)
            {
                vm?.OnFinalize();
                Layer.Close();
                TvhUi.ReleaseSprites(_clanSprites);
                _clanSprites = null;
                Debug.Print("[CheatMenu] peace table failed: " + ex);
                MenuUI.Error("Barış Masası açılamadı (" + ex.Message + ").");
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
                if (!busy && _wasBusy && Layer.DataSource is PeaceTableVM vm) vm.Refresh();
                if (!busy && _wasBusy) Layer.Refocus();
                _wasBusy = busy;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] peace table tick failed: " + ex);
                MenuUI.Error("Barış Masası hata verdi ve kapatıldı: " + ex.Message);
                Close();
            }
        }
    }
}
