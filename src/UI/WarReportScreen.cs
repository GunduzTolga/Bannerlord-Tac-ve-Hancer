using System;
using System.Collections.Generic;
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
    /// Savaş Raporu (tasarım: docs/superpowers/specs/2026-10-21-savas-raporu.md): Diplomacy'nin raporu (DetailWarViewInterface) açılmak
    /// istendiğinde onun yerine açılır (BundleShim). Krallık ekranının ya da haritanın üstünde; Esc ve Kapat kapatır. Popup kapanınca yenilenir.
    /// </summary>
    public static class WarReportScreen
    {
        private const string Movie = "TvhWarReport";
        // Ayrı Entrika ekranının (400) üstünde, olay kartlarının (410) altında.
        private const int Order = 405;
        // Klan: düğmeler ve sekmeler; ansiklopedi ve krallık: Diplomacy'nin üst kısmı (sancak maskesi, başlık şeridi, ilişki kutuları).
        private static readonly string[] SpriteCategories = { TvhUi.ClanSpriteCategory, "ui_encyclopedia", "ui_kingdom" };

        private static readonly MovieLayer Layer = new MovieLayer();
        private static readonly EscapeKey Escape = new EscapeKey();
        private static readonly object PauseOwner = new object();
        private static readonly List<SpriteCategory> Loaded = new List<SpriteCategory>();
        private static bool _wasBusy;

        public static bool IsOpen => Layer.IsOpen;

        /// <summary>Açıldıysa true; açılamazsa (Diplomacy'nin kendi raporu açılsın diye) false.</summary>
        /// <param name="host">null: harita.</param>
        public static bool Open(Kingdom enemy, ScreenBase host = null)
        {
            if (enemy == null || Clan.PlayerClan?.Kingdom == null || enemy == Clan.PlayerClan.Kingdom || SchemeEngine.State == null) return false;
            Close();
            TvhUi.PrepareBrushes();
            WarReportVM vm = null;
            try
            {
                foreach (string category in SpriteCategories)
                {
                    SpriteCategory loaded = TvhUi.AcquireSprites(category);
                    if (loaded != null) Loaded.Add(loaded);
                }
                vm = new WarReportVM(enemy, () => MenuUI.Defer(Close));
                if (!Layer.Open(Movie, vm, Order, true, host)) throw new InvalidOperationException("ekran bulunamadı");
            }
            catch (Exception ex)
            {
                vm?.OnFinalize();
                Layer.Close();
                ReleaseSprites();
                Debug.Print("[CheatMenu] war report window failed: " + ex);
                MenuUI.Error("Savaş Raporu açılamadı (" + ex.Message + "); Diplomacy'nin raporu açılıyor.");
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
            ReleaseSprites();
            TimePause.Release(PauseOwner);
        }

        private static void ReleaseSprites()
        {
            foreach (SpriteCategory category in Loaded) TvhUi.ReleaseSprites(category);
            Loaded.Clear();
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
                if (!busy && _wasBusy && Layer.DataSource is WarReportVM vm) vm.Refresh();
                if (!busy && _wasBusy) Layer.Refocus();
                _wasBusy = busy;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] war report window tick failed: " + ex);
                MenuUI.Error("Savaş Raporu hata verdi ve kapatıldı: " + ex.Message);
                Close();
            }
        }
    }
}
