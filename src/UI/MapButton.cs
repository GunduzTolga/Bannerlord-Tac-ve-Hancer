using System;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.UI
{
    /// <summary>
    /// Haritadaki "Entrika" düğmesi: kendi küçük katmanımız (oyunun harita çubuğu prefab'ına dokunmadan; oyun yaması
    /// çubuğu değiştirse bile bozulmaz). Yeri ayarlardan seçilir ya da gizlenir. Rozet: harekete geçmeye hazır
    /// suikast/kaçırma ve sezilen entrika sayısı. "Harita çubuğunda" seçiliyken düğmeyi MapBarButton oyunun çubuğuna koyar;
    /// oraya eklenemezse (ya da ayar kayıt yüklenmeden değiştiyse) bu katman üst ortada yedek olarak görünür.
    /// </summary>
    public static class MapButton
    {
        private const string Movie = "TvhMapButton";
        // Haritanın oyun içi menülerinin altında kalsın: kaçış menüsü gibi pencereler düğmeyi örter.
        private const int Order = 160;
        private const int RefreshMs = 500;

        private static readonly MovieLayer Layer = new MovieLayer();
        private static int _lastRefresh;
        private static bool _disabled;

        public static void Tick()
        {
            if (_disabled) return;
            try
            {
                int position = SettingsStore.Current.MapButtonPosition;
                if (position == ModSettings.MapBarButtonPosition)
                    position = MapBarButton.Injected ? 0 : ModSettings.MapBarFallbackPosition;
                bool wanted = position != 0 && !IntrigueScreen.IsOpen && IntrigueScreen.CannotOpenReason() == null
                    && Game.Current?.GameStateManager?.ActiveState is MapState;
                if (!wanted)
                {
                    if (Layer.IsOpen) Layer.Close();
                    return;
                }
                if (!Layer.IsAlive)
                {
                    Layer.Close();
                    TvhUi.PrepareBrushes();
                    Layer.Open(Movie, new MapButtonVM(), Order, false);
                    _lastRefresh = Environment.TickCount - RefreshMs;
                }
                if (Environment.TickCount - _lastRefresh < RefreshMs) return;
                _lastRefresh = Environment.TickCount;
                if (Layer.DataSource is MapButtonVM vm) vm.Refresh(position, PendingCount());
            }
            catch (Exception ex)
            {
                // Bir daha denenmez (her karede hata basmasın); entrika ekranı kısayol ve Insert menüsüyle açılmaya devam eder.
                _disabled = true;
                Layer.Close();
                Debug.Print("[CheatMenu] map button failed: " + ex);
                MenuUI.Error("Haritadaki Entrika düğmesi gösterilemedi (" + ex.Message + "). Ekranı kısayol ya da Insert menüsüyle açabilirsin.");
            }
        }

        /// <summary>Kampanya kapanırken: eski harita ekranına bağlı katman bırakılsın.</summary>
        public static void Close()
        {
            Layer.Close();
            MapBarButton.Reset();
        }

        /// <summary>Rozet: harekete geçmeye hazır suikast/kaçırma ve sezilen (korunulmamış) entrika sayısı.</summary>
        public static int PendingCount()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return 0;
            int ready = state.Schemes.Count(s => SchemeRules.IsStrike(s.Kind) && SchemeEngine.CannotExecuteReason(s) == null);
            return ready + state.AiPlots.Count(p => p.Detected && !p.Protected);
        }
    }
}
