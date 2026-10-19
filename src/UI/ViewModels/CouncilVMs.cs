using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Divan sekmesi (P2.12): hazinedarın koltuğu ve raporu. Gelirler oyunun kendi hesabı (kesin); fiyatlar, tahminler ve öneriler
    /// hazinedarın gözüyle (Trade'e göre hata payı). Sefer defteri kervanların şehirden çıkışlarından tutulur.
    /// </summary>
    public sealed class CouncilVM : TvhViewModel
    {
        private readonly Action _changed;
        private ImageIdentifierVM _portrait;
        private bool _hasTreasurer, _isEmbezzleFound;
        private string _name = "", _skillText = "", _accuracyText = "", _statusText = "", _summaryText = "";

        public CouncilVM(Action changed)
        {
            _changed = changed;
            _portrait = PortraitOf(null);
            Rows = new MBBindingList<CouncilRowVM>();
        }

        [DataSourceProperty] public string SeatHeader => "HAZİNEDAR";
        [DataSourceProperty] public ImageIdentifierVM Portrait { get => _portrait; set => Set(ref _portrait, value, nameof(Portrait)); }
        [DataSourceProperty] public bool HasTreasurer { get => _hasTreasurer; set { if (Set(ref _hasTreasurer, value, nameof(HasTreasurer))) OnPropertyChangedWithValue(!value, nameof(HasNoTreasurer)); } }
        [DataSourceProperty] public bool HasNoTreasurer => !_hasTreasurer;
        [DataSourceProperty] public bool IsEmbezzleFound { get => _isEmbezzleFound; set => Set(ref _isEmbezzleFound, value, nameof(IsEmbezzleFound)); }
        [DataSourceProperty] public string Name { get => _name; set => Set(ref _name, value, nameof(Name)); }
        [DataSourceProperty] public string SkillText { get => _skillText; set => Set(ref _skillText, value, nameof(SkillText)); }
        [DataSourceProperty] public string AccuracyText { get => _accuracyText; set => Set(ref _accuracyText, value, nameof(AccuracyText)); }
        [DataSourceProperty] public string StatusText { get => _statusText; set => Set(ref _statusText, value, nameof(StatusText)); }
        [DataSourceProperty] public string SummaryText { get => _summaryText; set => Set(ref _summaryText, value, nameof(SummaryText)); }
        [DataSourceProperty] public string AssignText => "Hazinedar ata";
        [DataSourceProperty] public string ChangeText => "Değiştir";
        [DataSourceProperty] public string DismissText => "Görevden al";
        [DataSourceProperty] public string ReckonText => "Hesap sor...";
        [DataSourceProperty] public string EmptyText =>
            "Divanda hazinedar koltuğu boş.\n\nKlanından Trade'i yüksek birini ata: atölyelerinin ve kervanlarının gelirini, kervanların hangi "
            + "rotada kazanıp kaybettiğini, atölyelerin ne üretmesi gerektiğini, ticaret fırsatlarını ve kervan kurmaya uygun şehirleri "
            + "sana uzaktan söyler. Trade'i düşükse tahminleri yanlış olabilir; onuru düşükse kasadan çalabilir.";
        [DataSourceProperty] public MBBindingList<CouncilRowVM> Rows { get; }

        public void Refresh(IntrigueState state)
        {
            Clear(Rows);
            Hero treasurer = Treasury.Treasurer(state);
            HasTreasurer = treasurer != null;
            IsEmbezzleFound = treasurer != null && state.EmbezzleFound;
            if (treasurer == null)
            {
                if (Name.Length > 0 || Portrait == null)
                {
                    _portrait?.OnFinalize();
                    Portrait = PortraitOf(null);
                }
                Name = SkillText = AccuracyText = StatusText = SummaryText = "";
                return;
            }
            if (Name != treasurer.Name.ToString() || Portrait == null)
            {
                _portrait?.OnFinalize();
                Portrait = PortraitOf(treasurer);
            }
            Name = treasurer.Name.ToString();
            int error = Treasury.Error(treasurer);
            SkillText = "Trade " + Treasury.Trade(treasurer) + "  ·  Hesapçı " + SchemeFactors.Signed(treasurer.GetTraitLevel(DefaultTraits.Calculating))
                + "  ·  Onur " + SchemeFactors.Signed(treasurer.GetTraitLevel(DefaultTraits.Honor));
            AccuracyText = "Tahmin payı %" + error + " (" + TreasuryRules.Confidence(error) + ")";
            string blocked = Treasury.Blocked(state);
            StatusText = state.EmbezzleFound ? "Zimmet ortaya çıktı: " + SchemeFactors.Money(state.Embezzled) + ". Hesabını sor."
                : blocked ?? (treasurer.GetTraitLevel(DefaultTraits.Honor) < 0 ? "Görevde. Onuru düşük: kasadan çalabilir." : "Görevde");
            int workshops = Treasury.Workshops().Sum(Treasury.WorkshopIncome), caravans = Treasury.Caravans().Sum(Treasury.CaravanIncome);
            SummaryText = "Günlük gelir (oyunun hesabı): atölyeler " + Treasury.Signed(workshops) + ", kervanlar " + Treasury.Signed(caravans)
                + "  ·  haftalık " + Treasury.Signed((workshops + caravans) * 7);
            if (blocked != null) return;

            Treasury.Board board = Treasury.CurrentBoard(state);
            AddWorkshops(state, board);
            AddCaravans(state);
            AddRoutes(state);
            AddOpportunities(board);
        }

        private void AddWorkshops(IntrigueState state, Treasury.Board board)
        {
            List<Workshop> workshops = Treasury.Workshops();
            Rows.Add(CouncilRowVM.Header("ATÖLYELER (" + workshops.Count + ")"));
            if (workshops.Count == 0) Rows.Add(CouncilRowVM.Note("Atölyen yok. Bir şehirde atölye satın almak için oradaki dükkân sahibiyle konuş."));
            foreach (Workshop workshop in workshops)
            {
                Treasury.WorkshopAdvice advice = Treasury.Advise(board, workshop);
                string text = workshop.Settlement.Name + " · " + workshop.WorkshopType.Name + "  ·  sermaye " + SchemeFactors.Money(workshop.Capital)
                    + "\nHazinedar: " + (advice.Worth
                        ? advice.Options[0].type.Name + " daha kârlı (tahmini günde " + Treasury.Signed((int)advice.Options[0].estimate)
                          + ", şimdiki " + Treasury.Signed((int)advice.Current) + ")"
                        : "böyle iyi (tahmini günde " + Treasury.Signed((int)advice.Current) + ")");
                Workshop chosen = workshop;
                Rows.Add(new CouncilRowVM(text, "günde " + Treasury.Signed(advice.Income), advice.Income < 0,
                    advice.Worth ? "Üretimi değiştir..." : "Seçenekler...", null, () => EventCards.ShowWorkshopChange(chosen, _changed)));
            }
        }

        private void AddCaravans(IntrigueState state)
        {
            List<MobileParty> caravans = Treasury.Caravans();
            Rows.Add(CouncilRowVM.Header("KERVANLAR (" + caravans.Count + ")"));
            if (caravans.Count == 0) Rows.Add(CouncilRowVM.Note("Kervanın yok. Kervan kurmak için bir şehirde tüccar eşrafla konuş (aşağıda önerilen şehirlere bak)."));
            foreach (MobileParty caravan in caravans)
            {
                List<CaravanLeg> legs = Treasury.LegsOf(state, caravan);
                string where = caravan.CurrentSettlement != null ? Tr.Locative(caravan.CurrentSettlement.Name.ToString())
                    : caravan.TargetSettlement != null ? Tr.Dative(caravan.TargetSettlement.Name.ToString()) + " gidiyor" : "yolda";
                string recent = legs.Count == 0 ? "defterde henüz sefer yok"
                    : "son seferler: " + string.Join(", ", legs.Skip(Math.Max(0, legs.Count - 3))
                        .Select(l => Treasury.TownName(l.FromId) + ">" + Treasury.TownName(l.ToId) + " " + Treasury.Signed(l.Profit)));
                int lastThree = legs.Skip(Math.Max(0, legs.Count - 3)).Sum(l => l.Profit);
                string text = (caravan.LeaderHero?.Name.ToString() ?? caravan.Name.ToString()) + " kervanı · " + where
                    + "\n" + recent + (legs.Count >= 3 && lastThree < 0 ? "  ·  zarar ediyor: dağıtmayı düşün" : "");
                MobileParty tracked = caravan;
                Rows.Add(new CouncilRowVM(text, "günde " + Treasury.Signed(Treasury.CaravanIncome(caravan)), legs.Count >= 3 && lastThree < 0,
                    "Haritada izle", null, () =>
                    {
                        Campaign.Current.VisualTrackerManager.RegisterObject(tracked);
                        MenuUI.Success("Kervan haritada izleniyor");
                    }));
            }
        }

        private void AddRoutes(IntrigueState state)
        {
            List<RouteStat> routes = TreasuryRules.Routes(state.CaravanLegs);
            Rows.Add(CouncilRowVM.Header("SEFER DEFTERİ (" + state.CaravanLegs.Count + " sefer)"));
            if (routes.Count == 0)
            {
                Rows.Add(CouncilRowVM.Note("Kervanların şehirden şehre her seferi buraya yazılır: kazanç = kesedeki altın ve yükün değerindeki fark, artı sana gönderilen pay."));
                return;
            }
            IEnumerable<RouteStat> shown = routes.Count <= 8 ? routes : routes.Take(5).Concat(routes.Skip(routes.Count - 3));
            foreach (RouteStat route in shown)
                Rows.Add(new CouncilRowVM(Treasury.TownName(route.FromId) + " > " + Treasury.TownName(route.ToId) + "  ·  " + route.Trips + " sefer",
                    "ort. " + Treasury.Signed(route.Average), route.Average < 0, null, null, null));
        }

        private void AddOpportunities(Treasury.Board board)
        {
            Rows.Add(CouncilRowVM.Header("TİCARET FIRSATLARI (hazinedarın gözüyle, " + board.Towns.Count + " şehir)"));
            List<Treasury.Opportunity> opportunities = Treasury.Opportunities(board, 6);
            if (opportunities.Count == 0) Rows.Add(CouncilRowVM.Note("Bildiği şehirler arasında kayda değer bir fiyat farkı görmüyor."));
            foreach (Treasury.Opportunity o in opportunities)
                Rows.Add(new CouncilRowVM(Treasury.OpportunityText(board, o), "+%" + o.Spread, false, null, null, null));

            Rows.Add(CouncilRowVM.Header("KERVAN KURMAYA UYGUN ŞEHİRLER"));
            List<(Settlement town, int spread, List<string> goods)> homes = Treasury.CaravanHomes(board, 3);
            if (homes.Count == 0) Rows.Add(CouncilRowVM.Note("Şu an öne çıkan bir şehir yok."));
            foreach ((Settlement town, int spread, List<string> goods) in homes)
                Rows.Add(new CouncilRowVM(town.Name + ": " + string.Join(", ", goods) + " alıp yakın şehirlerde satmak", "ort. +%" + spread, false,
                    "Haritada göster", null, () =>
                    {
                        Campaign.Current.VisualTrackerManager.RegisterObject(town);
                        MenuUI.Success(town.Name + " haritada işaretlendi");
                    }));
        }

        public void ExecuteAssign()
        {
            IntrigueState state = SchemeEngine.State;
            Picker.Pick("Hazinedar seç", Treasury.Candidates(state).OrderByDescending(Treasury.Trade).ToList(), Treasury.Label,
                hero => Run("Hazinedar", () =>
                {
                    Treasury.Assign(SchemeEngine.State, hero);
                    _changed();
                }),
                null, hero => Categories.HeroPortrait(hero), sort: false);
        }

        public void ExecuteDismiss()
        {
            Run("Hazinedar", () => Treasury.Dismiss(SchemeEngine.State));
            _changed();
        }

        public void ExecuteReckon() => EventCards.ShowReckoning(_changed);

        public override void OnFinalize()
        {
            base.OnFinalize();
            _portrait?.OnFinalize();
            Clear(Rows);
        }
    }

    /// <summary>Divan listesinde bir satır: bölüm başlığı, açıklama ya da (sağda değer ve isteğe bağlı düğmeyle) kayıt.</summary>
    public sealed class CouncilRowVM : TvhViewModel
    {
        private readonly Action _action;

        public CouncilRowVM(string text, string value, bool isBad, string buttonText, string blockedReason, Action action)
        {
            Text = text;
            ValueText = value ?? "";
            IsBad = isBad;
            ButtonText = buttonText ?? "";
            Hint = blockedReason ?? "";
            IsButtonEnabled = blockedReason == null;
            _action = action;
        }

        public static CouncilRowVM Header(string text) => new CouncilRowVM(text, null, false, null, null, null) { IsHeader = true };

        public static CouncilRowVM Note(string text) => new CouncilRowVM(text, null, false, null, null, null) { IsNote = true };

        [DataSourceProperty] public string Text { get; }
        [DataSourceProperty] public string ValueText { get; }
        [DataSourceProperty] public string ButtonText { get; }
        [DataSourceProperty] public string Hint { get; }
        [DataSourceProperty] public bool IsHeader { get; private set; }
        [DataSourceProperty] public bool IsNote { get; private set; }
        [DataSourceProperty] public bool IsRow => !IsHeader && !IsNote;
        [DataSourceProperty] public bool IsBad { get; }
        [DataSourceProperty] public bool IsGood => !IsBad;
        [DataSourceProperty] public bool HasButton => ButtonText.Length > 0;
        [DataSourceProperty] public bool IsButtonEnabled { get; }

        public void ExecuteButton()
        {
            if (_action != null && IsButtonEnabled) Run(ButtonText, _action);
        }
    }
}
