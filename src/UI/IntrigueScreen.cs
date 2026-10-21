using System;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

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
        // Sekmeler ve panel oyunun Klan ekranı fırçalarını kullanır; bu ekran Klan ekranı dışında açıldığı için onların
        // sprite'larını kendisi yükler (yalnızca oyun yüklememişse; kapanınca boşaltır).
        private static SpriteCategory _clanSprites;

        public static bool IsOpen => Layer.IsOpen;

        public static void Toggle()
        {
            if (IsOpen) Close();
            else if (ClanIntrigueTab.Registered) ClanIntrigueTab.Toggle();
            else Open();
        }

        /// <summary>
        /// Entrika'yı açar: Klan ekranındaki sekmede (UIExtenderEx kaydı tuttuysa) ya da bu ayrı ekranda (yedek).
        /// </summary>
        /// <param name="tab">-1: en son açık sekme.</param>
        public static void Open(int tab = -1)
        {
            if (ClanIntrigueTab.Registered)
            {
                ClanIntrigueTab.Open(tab >= 0 ? vm => vm.SelectTab(tab) : (Action<IntrigueScreenVM>)null);
                return;
            }
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
                _clanSprites = TvhUi.AcquireSprites(TvhUi.ClanSpriteCategory);
                vm = new IntrigueScreenVM(() => MenuUI.Defer(Close), tab >= 0 ? tab : _lastTab);
                if (!Layer.Open(Movie, vm, Order, true)) throw new InvalidOperationException("harita ekranı bulunamadı");
            }
            catch (Exception ex)
            {
                vm?.OnFinalize();
                Layer.Close();
                TvhUi.ReleaseSprites(_clanSprites);
                _clanSprites = null;
                MenuUI.Error("Entrika ekranı açılamadı (" + ex.Message + "). Liste menüsü açılıyor.");
                Debug.Print("[CheatMenu] intrigue screen failed: " + ex);
                MenuUI.Defer(() => MenuUI.SafeRun("Entrika", Categories.ShowIntrigueMenu));
                return;
            }
            _wasBusy = false;
            Escape.Reset();
            TimePause.Acquire(PauseOwner);
        }

        /// <summary>
        /// Ansiklopedideki lord sayfasından: ansiklopedi kapanır, ekran o kişiye odaklı açılır
        /// (kind null: Kişiler sekmesinde dosyası; değilse Yeni entrika sekmesinde o tür ve hedef seçili).
        /// </summary>
        public static void OpenOn(Hero hero, SchemeKind? kind)
        {
            MapEncyclopediaView encyclopedia = MapScreen.Instance?.EncyclopediaScreenManager;
            if (encyclopedia != null && encyclopedia.IsEncyclopediaOpen) encyclopedia.CloseEncyclopedia();
            // Bir sonraki karede: ansiklopedi katmanı tamamen kapansın, klavye odağı yeni ekrana geçsin.
            MenuUI.Defer(() =>
            {
                if (ClanIntrigueTab.Registered)
                {
                    if (hero != null) ClanIntrigueTab.Open(vm => vm.FocusOn(hero, kind));
                    else ClanIntrigueTab.Open();
                    return;
                }
                Open(kind == null ? IntrigueScreenVM.TabPeople : IntrigueScreenVM.TabNew);
                if (hero != null && Layer.DataSource is IntrigueScreenVM vm) vm.FocusOn(hero, kind);
            });
        }

        /// <summary>
        /// Entrika ekranını bir odakla açar (sekmeyse Klan ekranında, değilse ayrı ekranda); başka yerden (ansiklopedi, Krallık ekranı,
        /// Savaş Raporu) gelindiyse önce onlar kapanır. Her özellik tek yerde: pencereler buraya yönlenir.
        /// </summary>
        public static void OpenAt(Action<IntrigueScreenVM> focus)
        {
            MapEncyclopediaView encyclopedia = MapScreen.Instance?.EncyclopediaScreenManager;
            if (encyclopedia != null && encyclopedia.IsEncyclopediaOpen) encyclopedia.CloseEncyclopedia();
            WarReportScreen.Close();
            // Krallık ekranından geliniyorsa önce o kapanır (Entrika yalnızca haritada ya da Klan ekranında açılır); açılış bir sonraki karede.
            MenuUI.Defer(() =>
            {
                GameStateManager states = Game.Current?.GameStateManager;
                if (states?.ActiveState is KingdomState) states.PopState();
                MenuUI.Defer(() =>
                {
                    if (ClanIntrigueTab.Registered)
                    {
                        ClanIntrigueTab.Open(focus);
                        return;
                    }
                    if (!IsOpen) Open();
                    if (IsOpen && Layer.DataSource is IntrigueScreenVM vm) focus(vm);
                });
            });
        }

        public static void OpenTab(int tab) => OpenAt(vm => vm.SelectTab(tab));

        /// <summary>Savaş ve barış > Savaşlar, bu savaşın Barış Masası (terms: masaya konacak şartlar; null: boş).</summary>
        public static void OpenWar(Kingdom enemy, PeaceTreaties.Terms terms = null) => OpenAt(vm => vm.FocusWar(enemy, terms));

        /// <summary>Savaş ve barış > İsyanlar, bu krallığın grupları (null: kendi krallığın).</summary>
        public static void OpenRebellions(Kingdom kingdom) => OpenAt(vm => vm.FocusRebellions(kingdom));

        public static void Close()
        {
            if (!IsOpen) return;
            if (Layer.DataSource is IntrigueScreenVM vm) _lastTab = vm.CurrentTab;
            Layer.Close();
            TvhUi.ReleaseSprites(_clanSprites);
            _clanSprites = null;
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

        /// <summary>null: Entrika açılabilir (sekmeyse haritada ya da Klan ekranında; ayrı ekransa yalnızca haritada).</summary>
        public static string CannotOpenReason()
        {
            if (ClanIntrigueTab.Registered && !IsOpen) return ClanIntrigueTab.CannotOpenReason();
            if (Campaign.Current == null || IntrigueBehavior.Current == null) return "Entrika yalnızca kampanyada";
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState)) return "Entrika ekranı yalnızca haritadayken açılır";
            if (MapScreen.Instance == null) return "Harita ekranı hazır değil";
            if (Campaign.Current.ConversationManager?.IsConversationInProgress == true) return "Konuşma sırasında açılmaz";
            return null;
        }
    }
}
