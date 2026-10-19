using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// "Yeni entrika" paneli: tür sekmeleri, filtreler, sayfalı hedef listesi (portre, tahmini başarı, maliyet),
    /// sağda seçili hedefin özeti ve Başlat. Sayılar SchemePreview'dan gelir (Insert menüsüyle aynı).
    /// Sabotaj ve kışkırtmada (E3) hedefler yerleşimdir: liste, filtreler ve özet ona göre değişir.
    /// </summary>
    public sealed class NewSchemeVM : TvhViewModel
    {
        private const int PageSize = 24;
        private static readonly SchemeKind[] KindOrder =
            { SchemeKind.Spy, SchemeKind.Blackmail, SchemeKind.Kidnap, SchemeKind.Murder, SchemeKind.Sabotage, SchemeKind.Incite };
        private static readonly string[] HeroFilterNames = { "Hepsi", "Düşmanlar", "Krallığım", "Raporlular", "Tanıdıklar" };
        private static readonly string[] PlaceFilterNames = { "Hepsi", "Düşmanlar", "Gölge ağı olanlar", "Kuşattığın", "Şehirler" };

        private readonly Action _started;
        private SchemeKind _kind = SchemeKind.Spy;
        private int _filter;
        private int _page;
        private List<Hero> _all = new List<Hero>();
        private List<Settlement> _places = new List<Settlement>();
        private Hero _selected;
        private Settlement _selectedPlace;

        private string _kindDescription = "";
        private string _blockedText = "";
        private string _pageText = "";
        private bool _canPrev, _canNext, _hasNoTargets, _hasSelection, _canStart;
        private string _emptyText = "";
        private ImageIdentifierVM _selectedPortrait;
        private string _selectedName = "", _selectedAffiliation = "", _summaryText = "", _startHint = "", _startText = "";

        /// <param name="started">Entrika başarıyla başlayınca (ekran "Süren entrikalar"a geçer).</param>
        public NewSchemeVM(Action started)
        {
            _started = started;
            Kinds = new MBBindingList<TabVM>();
            for (int i = 0; i < KindOrder.Length; i++) Kinds.Add(new TabVM(i, SchemeRules.Name(KindOrder[i]), OnKind));
            Filters = new MBBindingList<TabVM>();
            for (int i = 0; i < HeroFilterNames.Length; i++) Filters.Add(new TabVM(i, HeroFilterNames[i], OnFilter));
            Targets = new MBBindingList<TargetItemVM>();
            _selectedPortrait = PortraitOf(null);
        }

        private bool PlaceMode => SchemeRules.TargetsSettlement(_kind);
        private int Count => PlaceMode ? _places.Count : _all.Count;

        [DataSourceProperty] public MBBindingList<TabVM> Kinds { get; }
        [DataSourceProperty] public MBBindingList<TabVM> Filters { get; }
        [DataSourceProperty] public MBBindingList<TargetItemVM> Targets { get; }
        [DataSourceProperty] public string KindDescription { get => _kindDescription; set => Set(ref _kindDescription, value, nameof(KindDescription)); }
        [DataSourceProperty] public string BlockedText { get => _blockedText; set { if (Set(ref _blockedText, value, nameof(BlockedText))) OnPropertyChangedWithValue(value.Length > 0, nameof(IsBlocked)); } }
        [DataSourceProperty] public bool IsBlocked => _blockedText.Length > 0;
        [DataSourceProperty] public string PageText { get => _pageText; set => Set(ref _pageText, value, nameof(PageText)); }
        [DataSourceProperty] public bool CanPrev { get => _canPrev; set => Set(ref _canPrev, value, nameof(CanPrev)); }
        [DataSourceProperty] public bool CanNext { get => _canNext; set => Set(ref _canNext, value, nameof(CanNext)); }
        [DataSourceProperty] public bool HasNoTargets { get => _hasNoTargets; set => Set(ref _hasNoTargets, value, nameof(HasNoTargets)); }
        [DataSourceProperty] public string EmptyText { get => _emptyText; set => Set(ref _emptyText, value, nameof(EmptyText)); }
        [DataSourceProperty] public bool HasSelection { get => _hasSelection; set { if (Set(ref _hasSelection, value, nameof(HasSelection))) OnPropertyChangedWithValue(!value, nameof(HasNoSelection)); } }
        [DataSourceProperty] public bool HasNoSelection => !_hasSelection;
        [DataSourceProperty] public ImageIdentifierVM SelectedPortrait { get => _selectedPortrait; set => Set(ref _selectedPortrait, value, nameof(SelectedPortrait)); }
        [DataSourceProperty] public string SelectedName { get => _selectedName; set => Set(ref _selectedName, value, nameof(SelectedName)); }
        [DataSourceProperty] public string SelectedAffiliation { get => _selectedAffiliation; set => Set(ref _selectedAffiliation, value, nameof(SelectedAffiliation)); }
        [DataSourceProperty] public string SummaryText { get => _summaryText; set => Set(ref _summaryText, value, nameof(SummaryText)); }
        [DataSourceProperty] public bool CanStart { get => _canStart; set => Set(ref _canStart, value, nameof(CanStart)); }
        [DataSourceProperty] public string StartHint { get => _startHint; set => Set(ref _startHint, value, nameof(StartHint)); }
        [DataSourceProperty] public string StartText { get => _startText; set => Set(ref _startText, value, nameof(StartText)); }
        [DataSourceProperty] public string PickHint => "Soldan bir hedef seç: özeti ve tahmini başarısı burada görünür.";
        [DataSourceProperty] public string PrevText => "« Önceki";
        [DataSourceProperty] public string NextText => "Sonraki »";

        /// <summary>Kişi dosyasından ya da raporlardan doğrudan: türü ve hedefi seçili açar.</summary>
        public void Preselect(SchemeKind kind, Hero target)
        {
            SetKind(kind);
            _filter = 0;
            _page = 0;
            _selected = target;
            Refresh(SchemeEngine.State, true);
            int index = _all.IndexOf(target);
            if (index >= 0)
            {
                _page = index / PageSize;
                FillPage();
            }
        }

        public void Refresh(IntrigueState state, bool rebuild)
        {
            for (int i = 0; i < KindOrder.Length; i++) Kinds[i].IsSelected = KindOrder[i] == _kind;
            foreach (TabVM filter in Filters) filter.IsSelected = filter.Index == _filter;
            KindDescription = SchemePreview.KindDescription(_kind);
            BlockedText = SchemeEngine.CannotStartReason(state) ?? "";

            if (rebuild)
            {
                Hero spymaster = SchemeFactors.Spymaster(state);
                if (PlaceMode)
                {
                    var ranked = new List<KeyValuePair<Settlement, int>>();
                    foreach (Settlement place in SettlementSchemes.Targets(state, _kind).Where(PassesPlaceFilter))
                    {
                        SettlementSchemes.SuccessRange(state, spymaster, place, _kind, out int low, out _);
                        ranked.Add(new KeyValuePair<Settlement, int>(place, low));
                    }
                    _places = ranked.OrderByDescending(p => p.Value).ThenBy(p => p.Key.Name.ToString()).Select(p => p.Key).ToList();
                }
                else
                {
                    var ranked = new List<KeyValuePair<Hero, int>>();
                    foreach (Hero hero in SchemeFactors.Targets(state, _kind).Where(PassesFilter))
                    {
                        SchemePreview.SuccessRange(state, spymaster, hero, _kind, out int low, out _);
                        ranked.Add(new KeyValuePair<Hero, int>(hero, low));
                    }
                    _all = ranked.OrderByDescending(p => p.Value).ThenBy(p => p.Key.Name.ToString()).Select(p => p.Key).ToList();
                }
                int pages = Math.Max(1, (Count + PageSize - 1) / PageSize);
                _page = Math.Min(_page, pages - 1);
            }
            if (_selected != null && (PlaceMode || SchemeFactors.BlockedReason(state, _selected, _kind) != null)) _selected = null;
            if (_selectedPlace != null && (!PlaceMode || SettlementSchemes.BlockedReason(state, _selectedPlace, _kind) != null)) _selectedPlace = null;
            FillPage();
            RefreshSummary(state);
        }

        private void FillPage()
        {
            IntrigueState state = SchemeEngine.State;
            Hero spymaster = SchemeFactors.Spymaster(state);
            Clear(Targets);
            if (PlaceMode)
                foreach (Settlement place in _places.Skip(_page * PageSize).Take(PageSize))
                    Targets.Add(new TargetItemVM(state, spymaster, place, _kind, place == _selectedPlace, OnTarget));
            else
                foreach (Hero hero in _all.Skip(_page * PageSize).Take(PageSize))
                    Targets.Add(new TargetItemVM(state, spymaster, hero, _kind, hero == _selected, OnTarget));
            int pages = Math.Max(1, (Count + PageSize - 1) / PageSize);
            PageText = "Sayfa " + (_page + 1) + "/" + pages + "  ·  " + Count + " hedef";
            CanPrev = _page > 0;
            CanNext = _page < pages - 1;
            HasNoTargets = Count == 0;
            EmptyText = _filter == 0 ? SchemePreview.NoTargetsText(_kind) : "Bu filtreye uyan hedef yok.";
        }

        private void RefreshSummary(IntrigueState state)
        {
            _selectedPortrait?.OnFinalize();
            if (_selected == null && _selectedPlace == null)
            {
                HasSelection = false;
                SelectedPortrait = PortraitOf(null);
                SelectedName = SelectedAffiliation = SummaryText = StartHint = "";
                CanStart = false;
                return;
            }
            HasSelection = true;
            int cost;
            if (_selectedPlace != null)
            {
                SelectedPortrait = PortraitOf(SettlementSchemes.PortraitHero(_selectedPlace));
                SelectedName = _selectedPlace.Name.ToString();
                SelectedAffiliation = SettlementSchemes.Affiliation(_selectedPlace);
                SummaryText = string.Join("\n", SettlementSchemes.PreviewLines(state, _kind, _selectedPlace));
                cost = SettlementSchemes.StartCost(_kind, _selectedPlace);
            }
            else
            {
                SelectedPortrait = PortraitOf(_selected);
                SelectedName = _selected.Name.ToString();
                SelectedAffiliation = SchemeFactors.Affiliation(_selected);
                SummaryText = string.Join("\n", SchemePreview.Lines(state, _kind, _selected));
                cost = SchemeFactors.StartCost(_kind, _selected);
            }
            StartText = (_kind == SchemeKind.Sabotage ? "Sabotaj hazırla..." : SchemeRules.Name(_kind) + " başlat") + " (" + SchemeFactors.Money(cost) + ")";
            string blocked = SchemeEngine.CannotStartReason(state)
                ?? (SchemeFactors.CanAfford(cost) ? null : "Yeterli altının yok");
            CanStart = blocked == null;
            StartHint = blocked ?? "";
        }

        private bool PassesFilter(Hero hero)
        {
            IntrigueState state = SchemeEngine.State;
            IFaction mine = Hero.MainHero.MapFaction;
            switch (_filter)
            {
                case 1: return mine != null && hero.MapFaction != null && mine.IsAtWarWith(hero.MapFaction);
                case 2: return Clan.PlayerClan.Kingdom != null && hero.Clan?.Kingdom == Clan.PlayerClan.Kingdom;
                case 3: return state.HasValidReport(hero.StringId, SchemeFactors.Today);
                case 4: return hero.HasMet;
                default: return true;
            }
        }

        private bool PassesPlaceFilter(Settlement place)
        {
            IFaction mine = Hero.MainHero.MapFaction;
            switch (_filter)
            {
                case 1: return mine != null && place.MapFaction != null && mine.IsAtWarWith(place.MapFaction);
                case 2: return SettlementSchemes.NetworkLevel(SchemeEngine.State, place) > 0;
                case 3: return SettlementSchemes.IsBesiegedByPlayer(place);
                case 4: return place.IsTown;
                default: return true;
            }
        }

        /// <summary>Tür değişince hedef türü (kişi/yerleşim) değiştiyse filtre adları ve seçim sıfırlanır.</summary>
        private void SetKind(SchemeKind kind)
        {
            bool wasPlace = PlaceMode;
            _kind = kind;
            if (wasPlace == PlaceMode) return;
            string[] names = PlaceMode ? PlaceFilterNames : HeroFilterNames;
            for (int i = 0; i < Filters.Count && i < names.Length; i++) Filters[i].Name = names[i];
            _filter = 0;
            _selected = null;
            _selectedPlace = null;
        }

        private void OnKind(TabVM tab)
        {
            SetKind(KindOrder[tab.Index]);
            _page = 0;
            Refresh(SchemeEngine.State, true);
        }

        private void OnFilter(TabVM tab)
        {
            _filter = tab.Index;
            _page = 0;
            Refresh(SchemeEngine.State, true);
        }

        private void OnTarget(TargetItemVM item)
        {
            _selected = item.Hero;
            _selectedPlace = item.Settlement;
            foreach (TargetItemVM target in Targets) target.IsSelected = target == item;
            RefreshSummary(SchemeEngine.State);
        }

        public void ExecutePrevPage()
        {
            if (_page == 0) return;
            _page--;
            FillPage();
        }

        public void ExecuteNextPage()
        {
            if (!CanNext) return;
            _page++;
            FillPage();
        }

        public void ExecuteStart()
        {
            if (!CanStart) return;
            SchemeKind kind = _kind;
            if (_selectedPlace != null)
            {
                Settlement place = _selectedPlace;
                if (kind == SchemeKind.Sabotage)
                {
                    // Neyin vurulacağı kartta seçilir; kart başlatınca ekran "Süren entrikalar"a geçer.
                    EventCards.ShowSabotageStart(place, () =>
                    {
                        _selectedPlace = null;
                        _started();
                    });
                    return;
                }
                if (!TryStart(kind, () => SchemeEngine.Start(kind, place))) return;
                _selectedPlace = null;
                _started();
                return;
            }
            if (_selected == null) return;
            Hero target = _selected;
            if (!TryStart(kind, () => SchemeEngine.Start(kind, target))) return;
            _selected = null;
            _started();
        }

        /// <returns>Başladıysa true (hata mesajı Run içinde gösterilir).</returns>
        private static bool TryStart(SchemeKind kind, Action start)
        {
            bool started = false;
            Run(SchemeRules.Name(kind), () =>
            {
                start();
                started = true;
            });
            return started;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Targets);
            _selectedPortrait?.OnFinalize();
        }
    }

    /// <summary>Hedef listesindeki satır: bir kişi ya da (sabotaj, kışkırtma) bir yerleşim.</summary>
    public sealed class TargetItemVM : TvhViewModel
    {
        private readonly Action<TargetItemVM> _onSelect;
        private bool _isSelected;

        public TargetItemVM(IntrigueState state, Hero spymaster, Hero hero, SchemeKind kind, bool selected, Action<TargetItemVM> onSelect)
        {
            Hero = hero;
            _onSelect = onSelect;
            _isSelected = selected;
            Portrait = PortraitOf(hero);
            Name = hero.Name.ToString();
            Affiliation = SchemeFactors.Affiliation(hero);
            ChanceText = "Başarı " + SchemePreview.ChanceText(state, spymaster, hero, kind);
            CostText = SchemeFactors.Money(SchemeFactors.StartCost(kind, hero));
            int today = SchemeFactors.Today;
            var tags = new List<string>();
            if (state.HasValidReport(hero.StringId, today)) tags.Add("rapor");
            if (state.ActiveHook(hero.StringId, today) != null) tags.Add("koz");
            int secrets = state.Secrets.Count(s => s.TargetId == hero.StringId && !s.Exposed);
            if (secrets > 0) tags.Add(secrets + " sır");
            if (state.IsAlerted(hero.StringId, today)) tags.Add("tetikte");
            TagsText = string.Join(" · ", tags);
        }

        public TargetItemVM(IntrigueState state, Hero spymaster, Settlement place, SchemeKind kind, bool selected, Action<TargetItemVM> onSelect)
        {
            Settlement = place;
            _onSelect = onSelect;
            _isSelected = selected;
            Portrait = PortraitOf(SettlementSchemes.PortraitHero(place));
            Name = place.Name.ToString();
            Affiliation = SettlementSchemes.Affiliation(place);
            ChanceText = "Başarı " + SettlementSchemes.ChanceText(state, spymaster, place, kind);
            CostText = SchemeFactors.Money(SettlementSchemes.StartCost(kind, place));
            TagsText = SettlementSchemes.Tags(state, place);
        }

        public Hero Hero { get; }
        public Settlement Settlement { get; }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string Affiliation { get; }
        [DataSourceProperty] public string ChanceText { get; }
        [DataSourceProperty] public string CostText { get; }
        [DataSourceProperty] public string TagsText { get; }
        [DataSourceProperty] public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value, nameof(IsSelected)); }

        public void ExecuteSelect() => _onSelect(this);

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }
}
