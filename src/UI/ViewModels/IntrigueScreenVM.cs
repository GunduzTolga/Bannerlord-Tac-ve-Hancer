using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Entrika ekranının kökü (yol haritası P1): sol sütunda casus başı, sağda sekmeler. Her eylemden sonra ekran
    /// bir sonraki karede yeniden kurulur (MarkDirty); oyun mantığı SchemeEngine ve diğer sınıflarda, burada yalnızca sunum var.
    /// </summary>
    public sealed class IntrigueScreenVM : TvhViewModel
    {
        public const int TabSchemes = 0, TabNew = 1, TabPeople = 2, TabPrisoners = 3, TabCourt = 4, TabCouncil = 5, TabNetworks = 6, TabPlots = 7,
            TabLog = 8, TabLords = 9, TabWars = 10, TabRebellions = 11, TabRealm = 12, TabKingdoms = 13, TabTrade = 14;

        /// <summary>
        /// Bölümler (üst sekmeler) ve alt sekmeleri (tasarım: docs/superpowers/specs/2026-10-21-arayuz-duzeni.md). Her özellik tek bir yerde:
        /// entrika işleri, insanlar, saray ve divan, savaş ve barış. Casus başı sütunu yalnızca Entrika bölümünde.
        /// </summary>
        private static readonly string[] GroupNames = { "Entrika", "Kişiler", "Saray", "Savaş ve barış" };
        private static readonly int[][] GroupTabs =
        {
            new[] { TabSchemes, TabNew, TabNetworks, TabPlots },
            new[] { TabPeople, TabLords, TabPrisoners },
            new[] { TabCourt, TabRealm, TabCouncil, TabTrade, TabLog },
            new[] { TabKingdoms, TabWars, TabRebellions },
        };
        private static readonly string[] TabNames =
            { "Süren", "Yeni entrika", "Kişiler", "Esirler", "Saray", "Divan", "Ağlar", "Sana karşı", "Kronik", "Lordlar", "Savaşlar", "İsyanlar", "Krallık", "Krallıklar", "Ticaret" };

        public static int GroupOf(int tab)
        {
            for (int g = 0; g < GroupTabs.Length; g++)
                if (Array.IndexOf(GroupTabs[g], tab) >= 0) return g;
            return 0;
        }

        private readonly Action _close;
        private int _tab;
        private bool _dirty;
        private string _subtitle = "", _status = "";
        private bool _isSchemesTab, _isNewTab, _isPeopleTab, _isPrisonersTab, _isCourtTab, _isCouncilTab, _isNetworksTab, _isPlotsTab, _isLogTab;
        private bool _isLordsTab, _isWarsTab, _isRebellionsTab, _isRealmTab, _isKingdomsTab, _isTradeTab, _isIntrigueGroup;
        private float _contentMargin;
        /// <summary>Her bölümde en son açık alt sekme (bölüme dönünce oraya).</summary>
        private readonly int[] _lastInGroup = new int[4];
        private bool _hasRomances;
        private bool _hasNothingRunning, _hasLetters, _hasNoPrisoners, _hasNoNetworks, _hasNoPlots, _hasNoLog;

        public IntrigueScreenVM(Action close, int tab)
        {
            _close = close;
            Spymaster = new SpymasterVM(MarkDirty);
            Groups = new MBBindingList<TabVM>();
            for (int g = 0; g < GroupNames.Length; g++)
            {
                Groups.Add(new TabVM(g, GroupNames[g], t => SelectTab(_lastInGroup[t.Index])));
                _lastInGroup[g] = GroupTabs[g][0];
            }
            Tabs = new MBBindingList<TabVM>();
            for (int i = 0; i < TabNames.Length; i++) Tabs.Add(new TabVM(i, TabNames[i], t => SelectTab(t.Index)));
            Lords = new LordsVM();
            Wars = new WarsVM(MarkDirty);
            Rebellions = new RebellionsVM(close);
            Realm = new RealmVM(MarkDirty);
            Kingdoms = new KingdomsVM(MarkDirty, enemy => FocusWar(enemy, null));
            Trade = new TradeVM(MarkDirty);
            Schemes = new MBBindingList<SchemeItemVM>();
            Letters = new MBBindingList<LetterItemVM>();
            Romances = new MBBindingList<RomanceRowVM>();
            Court = new CourtVM(MarkDirty, StartSchemeOn, FocusRebellions);
            Council = new CouncilVM(MarkDirty, enemy => FocusWar(enemy, null));
            NewScheme = new NewSchemeVM(() =>
            {
                _tab = TabSchemes;
                MarkDirty();
            });
            People = new PeopleVM(MarkDirty, StartSchemeOn);
            Prisoners = new MBBindingList<PrisonerItemVM>();
            Networks = new MBBindingList<NetworkItemVM>();
            Plots = new MBBindingList<PlotItemVM>();
            Log = new MBBindingList<LogItemVM>();
            _tab = tab;
            Refresh();
        }

        // ---- Başlık ----

        [DataSourceProperty] public string TitleText => "ENTRİKA";
        [DataSourceProperty] public string SubtitleText { get => _subtitle; set => Set(ref _subtitle, value, nameof(SubtitleText)); }
        [DataSourceProperty] public string StatusText { get => _status; set => Set(ref _status, value, nameof(StatusText)); }
        [DataSourceProperty] public string CloseText => "Kapat";
        [DataSourceProperty] public string ClassicMenuText => "Liste menüsü";

        [DataSourceProperty] public SpymasterVM Spymaster { get; }
        [DataSourceProperty] public MBBindingList<TabVM> Groups { get; }
        [DataSourceProperty] public MBBindingList<TabVM> Tabs { get; }
        /// <summary>Entrika bölümü: solda casus başı sütunu, içerik onun sağında.</summary>
        [DataSourceProperty] public bool IsIntrigueGroup { get => _isIntrigueGroup; set => Set(ref _isIntrigueGroup, value, nameof(IsIntrigueGroup)); }
        [DataSourceProperty] public float ContentMargin { get => _contentMargin; set => Set(ref _contentMargin, value, nameof(ContentMargin)); }
        [DataSourceProperty] public bool IsLordsTab { get => _isLordsTab; set => Set(ref _isLordsTab, value, nameof(IsLordsTab)); }
        [DataSourceProperty] public bool IsWarsTab { get => _isWarsTab; set => Set(ref _isWarsTab, value, nameof(IsWarsTab)); }
        [DataSourceProperty] public bool IsRebellionsTab { get => _isRebellionsTab; set => Set(ref _isRebellionsTab, value, nameof(IsRebellionsTab)); }
        [DataSourceProperty] public bool IsRealmTab { get => _isRealmTab; set => Set(ref _isRealmTab, value, nameof(IsRealmTab)); }
        [DataSourceProperty] public bool IsKingdomsTab { get => _isKingdomsTab; set => Set(ref _isKingdomsTab, value, nameof(IsKingdomsTab)); }
        [DataSourceProperty] public bool IsTradeTab { get => _isTradeTab; set => Set(ref _isTradeTab, value, nameof(IsTradeTab)); }
        [DataSourceProperty] public LordsVM Lords { get; }
        [DataSourceProperty] public WarsVM Wars { get; }
        [DataSourceProperty] public RebellionsVM Rebellions { get; }
        [DataSourceProperty] public RealmVM Realm { get; }
        [DataSourceProperty] public KingdomsVM Kingdoms { get; }
        [DataSourceProperty] public TradeVM Trade { get; }

        [DataSourceProperty] public bool IsSchemesTab { get => _isSchemesTab; set => Set(ref _isSchemesTab, value, nameof(IsSchemesTab)); }
        [DataSourceProperty] public bool IsNewTab { get => _isNewTab; set => Set(ref _isNewTab, value, nameof(IsNewTab)); }
        [DataSourceProperty] public bool IsPeopleTab { get => _isPeopleTab; set => Set(ref _isPeopleTab, value, nameof(IsPeopleTab)); }
        [DataSourceProperty] public bool IsPrisonersTab { get => _isPrisonersTab; set => Set(ref _isPrisonersTab, value, nameof(IsPrisonersTab)); }
        [DataSourceProperty] public bool IsCourtTab { get => _isCourtTab; set => Set(ref _isCourtTab, value, nameof(IsCourtTab)); }
        [DataSourceProperty] public bool IsCouncilTab { get => _isCouncilTab; set => Set(ref _isCouncilTab, value, nameof(IsCouncilTab)); }
        [DataSourceProperty] public bool IsNetworksTab { get => _isNetworksTab; set => Set(ref _isNetworksTab, value, nameof(IsNetworksTab)); }
        [DataSourceProperty] public bool IsPlotsTab { get => _isPlotsTab; set => Set(ref _isPlotsTab, value, nameof(IsPlotsTab)); }
        [DataSourceProperty] public bool IsLogTab { get => _isLogTab; set => Set(ref _isLogTab, value, nameof(IsLogTab)); }

        // ---- Süren entrikalar ----

        [DataSourceProperty] public MBBindingList<SchemeItemVM> Schemes { get; }
        [DataSourceProperty] public MBBindingList<LetterItemVM> Letters { get; }
        [DataSourceProperty] public bool HasLetters { get => _hasLetters; set => Set(ref _hasLetters, value, nameof(HasLetters)); }
        [DataSourceProperty] public bool HasNothingRunning { get => _hasNothingRunning; set => Set(ref _hasNothingRunning, value, nameof(HasNothingRunning)); }
        [DataSourceProperty] public string LettersHeader => "YOLDAKİ HABERCİLER";
        [DataSourceProperty] public MBBindingList<RomanceRowVM> Romances { get; }
        [DataSourceProperty] public bool HasRomances { get => _hasRomances; set => Set(ref _hasRomances, value, nameof(HasRomances)); }
        [DataSourceProperty] public string RomancesHeader => "KUR, SEVGİLİLER, GEBELİKLER";
        [DataSourceProperty] public string SchemesEmptyText =>
            "Süren bir entrika ya da yolda bir haberci yok.\n\n\"Yeni entrika\" sekmesinden casusluk, şantaj, kaçırma ya da suikast başlatabilirsin.";
        [DataSourceProperty] public CourtVM Court { get; }
        [DataSourceProperty] public CouncilVM Council { get; }

        // ---- Diğer sekmeler ----

        [DataSourceProperty] public NewSchemeVM NewScheme { get; }
        [DataSourceProperty] public PeopleVM People { get; }
        [DataSourceProperty] public MBBindingList<PrisonerItemVM> Prisoners { get; }
        [DataSourceProperty] public bool HasNoPrisoners { get => _hasNoPrisoners; set => Set(ref _hasNoPrisoners, value, nameof(HasNoPrisoners)); }
        [DataSourceProperty] public string PrisonersInfoText =>
            "Partindeki ve klanının zindanlarındaki esir kahramanlar. Sorgu rapor, belki bir sır ve belki efendisi hakkında bilgi verir. "
            + "Zindandaki bir esiri sorgulamak için o yerleşimde olmalısın. Her esir " + InterrogationRules.CooldownDays + " günde bir sorgulanır.";
        [DataSourceProperty] public MBBindingList<NetworkItemVM> Networks { get; }
        [DataSourceProperty] public bool HasNoNetworks { get => _hasNoNetworks; set => Set(ref _hasNoNetworks, value, nameof(HasNoNetworks)); }
        [DataSourceProperty] public string NetworksInfoText =>
            "Gölge ağı, bir şehirdeki çete lideri üzerinden kurulan muhbir ağıdır: o şehirdeki hedeflere başarı ekler, yakalanmayı azaltır. "
            + "Kurmak ve güçlendirmek için şehre git: kasaba menüsünde \"Gölgelerde iş çevir\". Buradan bakılır ve uzaktan dağıtılır.";
        [DataSourceProperty] public MBBindingList<PlotItemVM> Plots { get; }
        [DataSourceProperty] public bool HasNoPlots { get => _hasNoPlots; set => Set(ref _hasNoPlots, value, nameof(HasNoPlots)); }
        [DataSourceProperty] public string PlotsEmptyText =>
            "Sezdiğin bir entrika yok.\n\nYalnızca casus başının sezdikleri görünür; Roguery'si yüksekse sana karşı kurulanları daha kolay sezer. "
            + "Yapay zekâ entrikaları ayarlardan kapatılabilir ya da azaltılabilir.";
        [DataSourceProperty] public MBBindingList<LogItemVM> Log { get; }
        [DataSourceProperty] public bool HasNoLog { get => _hasNoLog; set => Set(ref _hasNoLog, value, nameof(HasNoLog)); }
        [DataSourceProperty] public string LogInfoText => "En yeni " + IntrigueState.MaxLogEntries + " olay tutulur; en yenisi en üstte.";
        [DataSourceProperty] public string ExportText => "Dosyaya aktar";

        public bool IsDirty => _dirty;

        public int CurrentTab => _tab;

        public void MarkDirty() => _dirty = true;

        public void SelectTab(int tab)
        {
            _tab = tab >= 0 && tab < TabNames.Length ? tab : TabSchemes;
            Refresh();
        }

        /// <summary>Savaş ve barış > Savaşlar: o savaş seçili, masada verilen şartlar (elçinin teklifi; null: boş masa).</summary>
        public void FocusWar(Kingdom enemy, PeaceTreaties.Terms terms)
        {
            Wars.Select(enemy, terms);
            SelectTab(TabWars);
        }

        /// <summary>Savaş ve barış > İsyanlar: bu krallığın grupları (null: kendi krallığın).</summary>
        public void FocusRebellions(Kingdom kingdom)
        {
            Rebellions.Show(kingdom);
            SelectTab(TabRebellions);
        }

        /// <summary>Savaş ve barış > Krallıklar: bu krallık seçili (null: son seçili ya da ilk).</summary>
        public void FocusKingdoms(Kingdom kingdom)
        {
            Kingdoms.Focus(kingdom);
            SelectTab(TabKingdoms);
        }

        /// <summary>Saray > Krallık: bu klan seçili (null: son seçili ya da ilk klan).</summary>
        public void FocusRealm(Clan clan)
        {
            Realm.Focus(clan);
            SelectTab(TabRealm);
        }

        /// <summary>Kişi dosyasından: "Yeni entrika" sekmesini tür ve hedef seçili açar.</summary>
        private void StartSchemeOn(SchemeKind kind, Hero hero)
        {
            _tab = TabNew;
            NewScheme.Preselect(kind, hero);
            Refresh(false);
        }

        /// <summary>Ansiklopediden: kişinin dosyası (kind null) ya da onu hedef alan yeni entrika.</summary>
        public void FocusOn(Hero hero, SchemeKind? kind)
        {
            if (kind != null)
            {
                StartSchemeOn(kind.Value, hero);
                return;
            }
            People.Select(hero);
            SelectTab(TabPeople);
        }

        public void Refresh(bool rebuildNew = true)
        {
            _dirty = false;
            IntrigueState state = SchemeEngine.State;
            int today = SchemeFactors.Today;

            SubtitleText = "Zorluk: " + SchemeMath.DifficultyName(SettingsStore.Current.IntrigueDifficulty)
                + "   ·   Kesende " + SchemeFactors.Money(Hero.MainHero.Gold) + "   ·   " + CampaignTime.Now;
            Spymaster.Refresh(state);

            int detected = state.AiPlots.Count(p => p.Detected);
            int people = state.Reports.Select(r => r.TargetId).Concat(state.Secrets.Select(s => s.TargetId))
                .Concat(state.Hooks.Where(h => h.Until > today).Select(h => h.TargetId)).Distinct().Count();
            List<Interrogation.Prisoner> prisoners = Interrogation.Prisoners();
            Tabs[TabPeople].Name = "Kişiler (" + people + ")";
            Tabs[TabPrisoners].Name = "Esirler (" + prisoners.Count + ")";
            Tabs[TabSchemes].Name = "Süren (" + (state.Schemes.Count + state.Letters.Count + Courtships.Rows(state).Count()) + ")";
            Tabs[TabNetworks].Name = "Ağlar (" + state.Networks.Count + ")";
            Tabs[TabPlots].Name = "Sana karşı (" + detected + ")";
            int group = GroupOf(_tab);
            _lastInGroup[group] = _tab;
            foreach (TabVM g in Groups) g.IsSelected = g.Index == group;
            foreach (TabVM tab in Tabs)
            {
                tab.IsSelected = tab.Index == _tab;
                tab.IsVisible = Array.IndexOf(GroupTabs[group], tab.Index) >= 0;
            }
            IsIntrigueGroup = group == 0;
            ContentMargin = IsIntrigueGroup ? 356 : 0;
            IsLordsTab = _tab == TabLords;
            IsWarsTab = _tab == TabWars;
            IsRebellionsTab = _tab == TabRebellions;
            IsRealmTab = _tab == TabRealm;
            IsKingdomsTab = _tab == TabKingdoms;
            IsTradeTab = _tab == TabTrade;
            IsSchemesTab = _tab == TabSchemes;
            IsNewTab = _tab == TabNew;
            IsPeopleTab = _tab == TabPeople;
            IsPrisonersTab = _tab == TabPrisoners;
            IsCourtTab = _tab == TabCourt;
            IsCouncilTab = _tab == TabCouncil;
            IsNetworksTab = _tab == TabNetworks;
            IsPlotsTab = _tab == TabPlots;
            IsLogTab = _tab == TabLog;

            // Yalnızca görünen sekme kurulur: portreler pahalı.
            Clear(Schemes);
            Letters.Clear();
            Romances.Clear();
            Clear(Prisoners);
            Clear(Networks);
            Clear(Plots);
            Log.Clear();
            switch (_tab)
            {
                case TabSchemes:
                    foreach (Scheme scheme in state.Schemes) Schemes.Add(new SchemeItemVM(state, scheme, MarkDirty));
                    foreach (Letter letter in state.Letters.OrderBy(l => l.ArriveDay)) Letters.Add(new LetterItemVM(letter));
                    HasLetters = Letters.Count > 0;
                    foreach ((string text, string days) in Courtships.Rows(state)) Romances.Add(new RomanceRowVM(text, days));
                    HasRomances = Romances.Count > 0;
                    HasNothingRunning = Schemes.Count == 0 && Letters.Count == 0 && Romances.Count == 0;
                    break;
                case TabCourt:
                    Court.Refresh(state);
                    break;
                case TabCouncil:
                    Council.Refresh(state);
                    break;
                case TabNew:
                    NewScheme.Refresh(state, rebuildNew);
                    break;
                case TabPeople:
                    People.Refresh(state);
                    break;
                case TabPrisoners:
                    foreach (Interrogation.Prisoner prisoner in prisoners) Prisoners.Add(new PrisonerItemVM(state, prisoner, MarkDirty));
                    HasNoPrisoners = Prisoners.Count == 0;
                    break;
                case TabNetworks:
                    foreach (ShadowNetwork network in state.Networks) Networks.Add(new NetworkItemVM(state, network));
                    HasNoNetworks = Networks.Count == 0;
                    break;
                case TabPlots:
                    foreach (AiPlot plot in state.AiPlots.Where(p => p.Detected)) Plots.Add(new PlotItemVM(state, plot, MarkDirty));
                    HasNoPlots = Plots.Count == 0;
                    break;
                case TabLog:
                    foreach (LogEntry entry in Enumerable.Reverse(state.Log)) Log.Add(new LogItemVM("Gün " + entry.Day, entry.Text));
                    HasNoLog = Log.Count == 0;
                    break;
                case TabLords:
                    Lords.Refresh();
                    break;
                case TabWars:
                    Wars.Refresh();
                    break;
                case TabRebellions:
                    Rebellions.Refresh();
                    break;
                case TabRealm:
                    Realm.Refresh();
                    break;
                case TabKingdoms:
                    Kingdoms.Refresh();
                    break;
                case TabTrade:
                    Trade.Refresh();
                    break;
            }
        }

        public void ExecuteClose() => _close();

        /// <summary>Eski popup menüsü (test ve yedek için).</summary>
        public void ExecuteClassicMenu()
        {
            _close();
            MenuUI.Defer(() => MenuUI.SafeRun("Entrika", Categories.ShowIntrigueMenu));
        }

        public void ExecuteExportLog()
        {
            IntrigueState state = SchemeEngine.State;
            try
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "Mount and Blade II Bannerlord", "Configs", "TacVeHancer");
                Directory.CreateDirectory(folder);
                string name = "Kronik_" + Clan.PlayerClan.Name + "_gun" + SchemeFactors.Today + ".txt";
                foreach (char bad in Path.GetInvalidFileNameChars()) name = name.Replace(bad, '_');
                string file = Path.Combine(folder, name);
                var text = new StringBuilder();
                text.AppendLine(ModInfo.DisplayName + " - entrika kroniği (" + Hero.MainHero.Name + ", " + CampaignTime.Now + ")");
                text.AppendLine();
                foreach (LogEntry entry in state.Log) text.AppendLine("Gün " + entry.Day + ": " + entry.Text);
                File.WriteAllText(file, text.ToString(), Encoding.UTF8);
                StatusText = "Kronik kaydedildi: " + file;
                MenuUI.Success("Kronik kaydedildi: " + file);
            }
            catch (Exception ex)
            {
                StatusText = "Kronik kaydedilemedi: " + ex.Message;
                MenuUI.Error(StatusText);
            }
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Spymaster.OnFinalize();
            Clear(Schemes);
            Clear(Prisoners);
            Clear(Networks);
            Clear(Plots);
            NewScheme.OnFinalize();
            People.OnFinalize();
            Court.OnFinalize();
            Council.OnFinalize();
            Lords.OnFinalize();
            Wars.OnFinalize();
            Rebellions.OnFinalize();
            Realm.OnFinalize();
            Kingdoms.OnFinalize();
            Trade.OnFinalize();
        }
    }
}
