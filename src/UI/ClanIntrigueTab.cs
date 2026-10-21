using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.ViewModels;
using CheatMenu.Intrigue;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using SandBox.View.Map.Navigation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection.ClanManagement;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace CheatMenu.UI
{
    /// <summary>
    /// Entrika, Klan ekranının bir sekmesidir: Birlikler ile Tımarlar arasında (Diplomacy modunun Krallık ekranına sekme
    /// eklemesi gibi, UIExtenderEx ile). Oyunun arka planı, üst şeridi, sancak, lider portresi ve "Bitti" düğmesi aynen kalır;
    /// alt yarıda bizim panelimiz (TvhIntriguePanel) görünür. Harita çubuğundaki düğme, kısayol, Insert menüsü ve ansiklopedi
    /// Klan ekranını bu sekmede açar. UIExtenderEx kaydı tutmazsa (Registered false) eski ayrı ekran (IntrigueScreen) kullanılır.
    /// </summary>
    public static class ClanIntrigueTab
    {
        // Klan ekranı açılması istenip açılmadıysa (izin yok) bekleyen odak bu kadar karede düşer.
        private const int PendingFrames = 120;
        // Haritaya dönüp bir eylem çalıştırma (düello, takip sahnesi) en fazla bu kadar kare bekler.
        private const int MapWaitFrames = 300;

        internal static ClanIntrigueMixin Current;
        internal static int LastTab = -1;

        private static bool _hasPending;
        private static Action<IntrigueScreenVM> _pending;
        private static int _pendingAge;
        private static Action _onMap;
        private static int _onMapAge;
        private static bool _wasBusy;
        private static int _unboundFrames;
        private static bool _unboundReported;

        /// <summary>UIExtenderEx kaydı tuttu (kampanya başında, UiExtensions.Register).</summary>
        public static bool Registered { get; internal set; }

        /// <summary>Klan ekranı açık ve Entrika sekmesi seçili.</summary>
        public static bool IsShowing => Current != null && Current.IsShowing;

        /// <summary>Olay kartı gibi pencereler nereye açılsın: sekme görünüyorsa Klan ekranının üstüne, değilse haritaya (null).</summary>
        public static ScreenBase CardHost => IsShowing ? ScreenManager.TopScreen : null;

        /// <summary>null: açılabilir. Haritada ya da zaten Klan ekranındayken.</summary>
        public static string CannotOpenReason()
        {
            if (Campaign.Current == null || IntrigueBehavior.Current == null) return "Entrika yalnızca kampanyada";
            GameState active = Game.Current?.GameStateManager?.ActiveState;
            if (!(active is MapState) && !(active is ClanState)) return "Entrika yalnızca haritadayken açılır";
            if (Campaign.Current.ConversationManager?.IsConversationInProgress == true) return "Konuşma sırasında açılmaz";
            return null;
        }

        /// <summary>Klan ekranını Entrika sekmesinde açar; zaten açıksa sekmeye geçer. focus: sekme açılınca (ör. bir kişiye odaklan).</summary>
        public static void Open(Action<IntrigueScreenVM> focus = null)
        {
            string reason = CannotOpenReason();
            if (reason != null)
            {
                MenuUI.Error(reason);
                return;
            }
            _hasPending = true;
            _pending = focus;
            _pendingAge = 0;
            if (Current != null) return;
            try
            {
                INavigationElement clan = MapBarButton.Find("clan");
                if (clan != null)
                {
                    // Oyunun Klan düğmesiyle aynı yol: izin denetimi ve hazırlık oyunun kendisinde.
                    if (!clan.Permission.IsAuthorized)
                    {
                        _hasPending = false;
                        MenuUI.Error("Klan ekranı şu an açılamıyor" + (clan.Permission.ReasonString != null ? ": " + clan.Permission.ReasonString : ""));
                        return;
                    }
                    clan.OpenView();
                }
                else
                {
                    GameStateManager states = Game.Current.GameStateManager;
                    states.PushState(states.CreateState<ClanState>());
                }
            }
            catch (Exception ex)
            {
                _hasPending = false;
                Debug.Print("[CheatMenu] clan screen could not be opened: " + ex);
                MenuUI.Error("Klan ekranı açılamadı (" + ex.Message + ")");
            }
        }

        /// <summary>Kısayol: sekme açıksa Klan ekranını kapatır, değilse açar.</summary>
        public static void Toggle()
        {
            if (IsShowing) Current.CloseScreen();
            else Open();
        }

        /// <summary>
        /// Sahne açan eylemler (düello, şehirde takip) Klan ekranının üstünde başlamasın: ekran açıksa kapanır, eylem harita
        /// geri gelince çalışır. Haritadaysa hemen.
        /// </summary>
        public static void WhenOnMap(Action action)
        {
            if (action == null) return;
            if (Current == null && Game.Current?.GameStateManager?.ActiveState is MapState)
            {
                action();
                return;
            }
            Current?.CloseScreen();
            _onMap = action;
            _onMapAge = 0;
        }

        /// <summary>Her kare (SubModule): bekleyen sekme seçimi, haritaya dönüş, açılan pencereden sonra yenileme.</summary>
        public static void Tick()
        {
            try
            {
                CheckBound();
                if (_onMap != null)
                {
                    if (Current == null && Game.Current?.GameStateManager?.ActiveState is MapState)
                    {
                        Action action = _onMap;
                        _onMap = null;
                        MenuUI.SafeRun("Entrika", action);
                    }
                    else if (++_onMapAge > MapWaitFrames) _onMap = null;
                }

                if (_hasPending)
                {
                    if (Current == null)
                    {
                        if (++_pendingAge > PendingFrames) _hasPending = false;
                        return;
                    }
                    // Bir kare sonra: oyun ekranı kurarken kendi sekmesini seçebilir (SelectHero gibi); bizimki onun üstüne gelsin.
                    if (_pendingAge++ < 1) return;
                    _hasPending = false;
                    Action<IntrigueScreenVM> focus = _pending;
                    _pending = null;
                    Current.Select();
                    if (focus != null && Current.Intrigue != null) focus(Current.Intrigue);
                    return;
                }

                if (!IsShowing) return;
                IntrigueScreenVM vm = Current.Intrigue;
                bool busy = InformationManager.IsAnyInquiryActive() || EventCards.IsOpen || MenuUI.HasPending;
                if (!busy && (_wasBusy || vm.IsDirty)) vm.Refresh();
                _wasBusy = busy;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] clan intrigue tab tick failed: " + ex);
            }
        }

        /// <summary>
        /// Klan ekranı açık ama sekmemizin ViewModel eki oluşmadıysa (düğme boş ve tıklanmaz) bir kez nedenini yazar ve bu oturumda
        /// ayrı Entrika ekranına (IntrigueScreen) geçer.
        /// </summary>
        private static void CheckBound()
        {
            if (!Registered || !(Game.Current?.GameStateManager?.ActiveState is ClanState))
            {
                _unboundFrames = 0;
                return;
            }
            if (Current != null || _unboundReported || ++_unboundFrames < 60) return;
            _unboundReported = true;
            object vm = (ScreenManager.TopScreen as SandBox.GauntletUI.GauntletClanScreen)?._dataSource;
            string type = vm?.GetType().FullName ?? "(ekran bulunamadı: " + ScreenManager.TopScreen?.GetType().FullName + ")";
            Debug.Print("[CheatMenu] clan intrigue mixin missing; clan view model: " + type + " from " + vm?.GetType().Assembly.GetName().Name);
            // Sekme boş kalacak: bu oturumda Home, harita çubuğu, ansiklopedi ve Insert ayrı Entrika ekranını açsın.
            Registered = false;
            MenuUI.Error("Entrika sekmesi Klan ekranına bağlanamadı (ekran: " + type
                + (vm != null ? ", " + vm.GetType().Assembly.GetName().Name : "") + "). Bu satırın ekran görüntüsünü at. Bu oturumda Home ve "
                + "harita düğmesi ayrı Entrika ekranını açar.");
        }

        /// <summary>Kampanya kapanırken.</summary>
        public static void Reset()
        {
            Current = null;
            _hasPending = false;
            _pending = null;
            _onMap = null;
            _wasBusy = false;
        }
    }

    /// <summary>
    /// Klan ekranının ViewModel'ine eklenen alanlar. Oyunun bir sekmesi seçilince (SetSelectedCategory) UIExtenderEx OnRefresh'i
    /// çağırır; bizim sekme o an kapanır. Bizim sekme seçilince oyunun dört sekmesi kapatılır (SetSelectedCategory çağrılmadan).
    /// </summary>
    // handleDerived: başka bir mod Klan ekranını kendi türettiği ViewModel'le açabilir (GauntletClanScreen.CreateDataSource);
    // UIExtenderEx ekleri yalnızca tam türe bağlar, o zaman sekme boş ve tıklanmaz kalırdı.
    [ViewModelMixin(nameof(ClanManagementVM.SetSelectedCategory), true)]
    public sealed class ClanIntrigueMixin : BaseViewModelMixin<ClanManagementVM>
    {
        private IntrigueScreenVM _intrigue;
        private bool _isSelected, _canOpen;

        public ClanIntrigueMixin(ClanManagementVM vm) : base(vm)
        {
            TvhUi.PrepareBrushes();
            try
            {
                if (IntrigueBehavior.Current != null)
                {
                    // Panelin veri kaynağı baştan dolu olsun: sekme gizliyken de bağlanacak boş bir şey kalmasın.
                    _intrigue = new IntrigueScreenVM(() => MenuUI.Defer(CloseScreen), ClanIntrigueTab.LastTab >= 0 ? ClanIntrigueTab.LastTab : 0);
                    _canOpen = true;
                }
            }
            catch (Exception ex)
            {
                _intrigue = null;
                Debug.Print("[CheatMenu] intrigue tab could not be prepared: " + ex);
            }
            ClanIntrigueTab.Current = this;
        }

        internal IntrigueScreenVM Intrigue => _intrigue;
        internal bool IsShowing => _isSelected && _intrigue != null;

        [DataSourceProperty] public string TvhIntrigueTabText => "Entrika";
        [DataSourceProperty] public bool TvhCanOpenIntrigue { get => _canOpen; set => SetField(ref _canOpen, value, nameof(TvhCanOpenIntrigue)); }
        [DataSourceProperty] public bool TvhIsIntrigueSelected { get => _isSelected; set => SetField(ref _isSelected, value, nameof(TvhIsIntrigueSelected)); }
        [DataSourceProperty] public IntrigueScreenVM TvhIntrigue => _intrigue;

        [DataSourceMethod]
        public void ExecuteTvhSelectIntrigue() => Select();

        /// <summary>Oyunun bir sekmesi seçildi.</summary>
        public override void OnRefresh()
        {
            if (_isSelected) TvhIsIntrigueSelected = false;
        }

        internal void Select()
        {
            if (!_canOpen || _intrigue == null) return;
            try
            {
                ClanManagementVM vm = ViewModel;
                vm.ClanMembers.IsSelected = false;
                vm.ClanParties.IsSelected = false;
                vm.ClanFiefs.IsSelected = false;
                vm.ClanIncome.IsSelected = false;
                vm.IsMembersSelected = false;
                vm.IsPartiesSelected = false;
                vm.IsFiefsSelected = false;
                vm.IsIncomeSelected = false;
                _intrigue.Refresh();
                TvhIsIntrigueSelected = true;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] intrigue tab could not be selected: " + ex);
                MenuUI.Error("Entrika sekmesi açılamadı: " + ex.Message);
            }
        }

        /// <summary>Klan ekranını oyunun "Bitti" düğmesi gibi kapatır.</summary>
        internal void CloseScreen()
        {
            try
            {
                ViewModel?.ExecuteClose();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] clan screen close failed: " + ex);
            }
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            if (_intrigue != null)
            {
                ClanIntrigueTab.LastTab = _intrigue.CurrentTab;
                _intrigue.OnFinalize();
                _intrigue = null;
            }
            if (ClanIntrigueTab.Current == this) ClanIntrigueTab.Current = null;
        }
    }

    // ---- Klan ekranı prefab yamaları (ClanScreen.xml) ----

    /// <summary>Sekme düğmesi: Birlikler'in hemen sağına (oyunun sekmesiyle aynı fırça, boyut ve yazı).</summary>
    [PrefabExtension("ClanScreen", "descendant::ButtonWidget[@CommandParameter.Click='1']")]
    public sealed class ClanIntrigueTabButtonPrefab : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionText]
        public string Content => @"
<ButtonWidget DoNotPassEventsToChildren='true' WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='!Header.Tab.Center.Width.Scaled' SuggestedHeight='!Header.Tab.Center.Height.Scaled' PositionYOffset='6' Brush='Header.Tab.Center' Command.Click='ExecuteTvhSelectIntrigue' IsSelected='@TvhIsIntrigueSelected' IsEnabled='@TvhCanOpenIntrigue' UpdateChildrenStates='true'>
  <Children>
    <TextWidget WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' Brush='Clan.TabControl.Text' Text='@TvhIntrigueTabText' />
  </Children>
</ButtonWidget>";
    }

    /// <summary>Panel: alt yarıda, oyunun dört panelinin yanında; yalnızca bizim sekme seçiliyken görünür.</summary>
    [PrefabExtension("ClanScreen", "descendant::ClanIncome")]
    public sealed class ClanIntriguePanelPrefab : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Append;

        [PrefabExtensionText]
        public string Content => @"
<Widget WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' MarginLeft='50' MarginRight='50' MarginTop='14' MarginBottom='6' IsVisible='@TvhIsIntrigueSelected' DoNotAcceptEvents='true'>
  <Children>
    <TvhIntriguePanel DataSource='{TvhIntrigue}' />
  </Children>
</Widget>";
    }

    /// <summary>Sağdaki "Mali Durum" kutusu Entrika sekmesinde gizlenir (panelin sağ tarafını örtmesin).</summary>
    [PrefabExtension("ClanScreen", "descendant::Widget[@Id='FinancePanelWidget']")]
    public sealed class ClanIntrigueFinancePrefab : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new List<Attribute> { new Attribute("IsHidden", "@TvhIsIntrigueSelected") };
    }

    /// <summary>Beş sekme üst şeride sığsın: ortadaki sekmeler biraz daralır (oyunda 0.90; Diplomacy de böyle yapar).</summary>
    [PrefabExtension("ClanScreen", "descendant::Constant[@Name='Header.Tab.Center.Width.Scaled']")]
    public sealed class ClanIntrigueTabWidthPrefab : PrefabExtensionSetAttributePatch
    {
        public override List<Attribute> Attributes => new List<Attribute> { new Attribute("MultiplyResult", "0.72") };
    }
}
