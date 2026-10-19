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
            TabLog = 8;

        private readonly Action _close;
        private int _tab;
        private bool _dirty;
        private string _subtitle = "", _status = "";
        private bool _isSchemesTab, _isNewTab, _isPeopleTab, _isPrisonersTab, _isCourtTab, _isCouncilTab, _isNetworksTab, _isPlotsTab, _isLogTab;
        private bool _hasRomances;
        private bool _hasNothingRunning, _hasLetters, _hasNoPrisoners, _hasNoNetworks, _hasNoPlots, _hasNoLog;

        public IntrigueScreenVM(Action close, int tab)
        {
            _close = close;
            Spymaster = new SpymasterVM(MarkDirty);
            Tabs = new MBBindingList<TabVM>();
            string[] names = { "Süren", "Yeni entrika", "Kişiler", "Esirler", "Saray", "Divan", "Ağlar", "Sana karşı", "Kronik" };
            for (int i = 0; i < names.Length; i++) Tabs.Add(new TabVM(i, names[i], t => SelectTab(t.Index)));
            Schemes = new MBBindingList<SchemeItemVM>();
            Letters = new MBBindingList<LetterItemVM>();
            Romances = new MBBindingList<RomanceRowVM>();
            Court = new CourtVM(MarkDirty, StartSchemeOn);
            Council = new CouncilVM(MarkDirty);
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
        [DataSourceProperty] public MBBindingList<TabVM> Tabs { get; }

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
            _tab = tab;
            Refresh();
        }

        /// <summary>Kişi dosyasından: "Yeni entrika" sekmesini tür ve hedef seçili açar.</summary>
        private void StartSchemeOn(SchemeKind kind, Hero hero)
        {
            _tab = TabNew;
            NewScheme.Preselect(kind, hero);
            Refresh(false);
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
            foreach (TabVM tab in Tabs) tab.IsSelected = tab.Index == _tab;
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
        }
    }
}
