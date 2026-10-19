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
    /// Divan sekmesi: üstte koltuklar (serdar P2.14, hazinedar P2.12, hekim P2.13), altında raporları. Gelirler oyunun kendi hesabı (kesin); fiyatlar,
    /// tahminler ve ömür riskleri koltuk sahibinin gözüyle (becerisine göre hata payı). Casus başı sol sütunda kalır.
    /// </summary>
    public sealed class CouncilVM : TvhViewModel
    {
        private readonly Action _changed;
        private readonly CouncilSeatVM _marshal, _treasurer, _physician;

        public CouncilVM(Action changed)
        {
            _changed = changed;
            _marshal = new CouncilSeatVM("SERDAR",
                "Tactics'i ve Scouting'i yüksek birini ata: savaş meclisi (düşmanın gücü, orduları, zayıf kaleleri, savunman) ve şehrine ya da "
                + "köyüne yürüyen ordulara erken uyarı. Sahadayken de çalışır. Tactics düşükse sayıları yanlış tahmin eder.",
                AssignMarshal, () => Dismiss("Serdar", Marshal.Dismiss), null);
            _treasurer = new CouncilSeatVM("HAZİNEDAR",
                "Trade'i yüksek birini ata: gelirler, kervanların sefer defteri, atölye önerileri, ticaret fırsatları. Trade düşükse tahminleri "
                + "yanlış olabilir; onuru düşükse kasadan çalabilir.",
                AssignTreasurer, () => Dismiss("Hazinedar", Treasury.Dismiss), () => EventCards.ShowReckoning(_changed));
            _physician = new CouncilSeatVM("SARAY HEKİMİ",
                "Medicine'i yüksek birini ata: klanında doğumda ve yaşlılıktan ölümü azaltır, suikastte kurbanı kurtarabilir, eşinin gizli "
                + "gebeliğini fark eder. Medicine 30'un altındaysa şarlatandır: tedavisi zarar verir.",
                AssignPhysician, () => Dismiss("Hekim", Physician.Dismiss), null);
            Seats = new MBBindingList<CouncilSeatVM> { _marshal, _treasurer, _physician };
            Rows = new MBBindingList<CouncilRowVM>();
        }

        [DataSourceProperty] public MBBindingList<CouncilSeatVM> Seats { get; }
        [DataSourceProperty] public MBBindingList<CouncilRowVM> Rows { get; }

        public void Refresh(IntrigueState state)
        {
            Clear(Rows);
            RefreshMarshalSeat(state);
            RefreshTreasurerSeat(state);
            RefreshPhysicianSeat(state);
            if (Marshal.Current(state) == null && Treasury.Treasurer(state) == null && Physician.Current(state) == null)
                Rows.Add(CouncilRowVM.Note("Divanda koltuklar boş. Yukarıdan klanından birini ata; raporları burada görünür."));
            if (Marshal.Working(state) is Hero marshal) AddWar(state, marshal);
            if (Physician.Working(state) != null) AddHealth(state);
            if (Treasury.Blocked(state) == null)
            {
                Treasury.Board board = Treasury.CurrentBoard(state);
                AddIncome();
                AddWorkshops(board);
                AddCaravans(state);
                AddRoutes(state);
                AddOpportunities(board);
            }
        }

        private void RefreshMarshalSeat(IntrigueState state)
        {
            Hero marshal = Marshal.Current(state);
            if (marshal == null)
            {
                _marshal.Show(null, "", "", "", false);
                return;
            }
            _marshal.Show(marshal, "Tactics " + Marshal.Tactics(marshal) + "  ·  Scouting " + Marshal.Scouting(marshal), Marshal.DetailText(marshal),
                Marshal.Blocked(state) ?? (marshal.PartyBelongedTo?.LeaderHero == marshal ? "Görevde (sahada)" : "Görevde"), false);
        }

        private void RefreshTreasurerSeat(IntrigueState state)
        {
            Hero treasurer = Treasury.Treasurer(state);
            if (treasurer == null)
            {
                _treasurer.Show(null, "", "", "", false);
                return;
            }
            int error = Treasury.Error(treasurer);
            string blocked = Treasury.Blocked(state);
            _treasurer.Show(treasurer,
                "Trade " + Treasury.Trade(treasurer) + "  ·  Hesapçı " + SchemeFactors.Signed(treasurer.GetTraitLevel(DefaultTraits.Calculating))
                + "  ·  Onur " + SchemeFactors.Signed(treasurer.GetTraitLevel(DefaultTraits.Honor)),
                "Tahmin payı %" + error + " (" + TreasuryRules.Confidence(error) + ")",
                state.EmbezzleFound ? "Zimmet ortaya çıktı: " + SchemeFactors.Money(state.Embezzled) + ". Hesabını sor."
                    : blocked ?? (treasurer.GetTraitLevel(DefaultTraits.Honor) < 0 ? "Görevde. Onuru düşük: kasadan çalabilir." : "Görevde"),
                state.EmbezzleFound, "Hesap sor...");
        }

        private void RefreshPhysicianSeat(IntrigueState state)
        {
            Hero physician = Physician.Current(state);
            if (physician == null)
            {
                _physician.Show(null, "", "", "", false);
                return;
            }
            int medicine = Physician.Medicine(physician);
            _physician.Show(physician,
                "Medicine " + medicine + (PhysicianRules.IsQuack(medicine) ? " (şarlatan)" : "") + "  ·  tahmin payı %" + PhysicianRules.ErrorPercent(medicine),
                Physician.ProtectionText(physician),
                Physician.Blocked(state) ?? "Görevde", false);
        }

        // ---- Serdar ----

        private void AddWar(IntrigueState state, Hero marshal)
        {
            List<Marshal.WarLine> wars = Marshal.Wars(state, marshal);
            Rows.Add(CouncilRowVM.Header("SAVAŞ MECLİSİ (serdarın gözüyle)"));
            if (wars.Count == 0) Rows.Add(CouncilRowVM.Note("Kimseyle savaşta değilsin."));
            foreach (Marshal.WarLine war in wars)
                Rows.Add(new CouncilRowVM(war.Kingdom.Name + " ile savaş · güç: onlar ~" + war.Theirs + ", biz " + war.Ours + " · " + war.Armies + " ordu"
                    + "\nSerdar: " + MarshalRules.Advice(war.Ratio), "oran " + war.Ratio.ToString("0.0", Tr.Culture),
                    MarshalRules.IsDangerous(war.Ratio), null, null, null));

            List<Marshal.FortLine> own = Marshal.OwnDefense(state, marshal);
            if (own.Count > 0)
            {
                Rows.Add(CouncilRowVM.Header("SAVUNMAN"));
                foreach (Marshal.FortLine fort in own)
                {
                    bool low = fort.FoodDays < MarshalRules.LowFoodDays;
                    bool danger = fort.Settlement.IsUnderSiege || fort.Threats.Count > 0 || low;
                    string second = fort.Settlement.IsUnderSiege ? "Kuşatma altında!"
                        : fort.Threats.Count > 0 ? "Tehdit: " + string.Join("; ", fort.Threats)
                        : low ? "Erzak azalıyor: kuşatmaya dayanamaz" : "Sakin";
                    Settlement shown = fort.Settlement;
                    Rows.Add(new CouncilRowVM(FortText(fort, true) + "\n" + second, "savunma " + fort.Defense, danger,
                        "Haritada göster", null, () => Track(shown)));
                }
            }

            if (wars.Count == 0) return;
            List<Marshal.ArmyLine> armies = Marshal.Armies(state, marshal);
            Rows.Add(CouncilRowVM.Header("DÜŞMAN ORDULARI (görüş " + (int)MarshalRules.KnownRange(Marshal.Scouting(marshal)) + ")"));
            if (armies.Count == 0) Rows.Add(CouncilRowVM.Note("Serdarın görüş menzilinde düşman ordusu ya da büyük bir lord partisi yok."));
            foreach (Marshal.ArmyLine army in armies.Take(10))
            {
                bool ours = army.Target != null && army.Target.OwnerClan == Clan.PlayerClan;
                string text = army.Name + " (" + army.Party.MapFaction?.Name + ") · ~" + army.Men + " kişi" + (army.Near != null ? " · " + army.Near.Name + " yakınında" : "")
                    + "\n" + (army.Target != null ? Marshal.Goal(army) + " gidiyor" + (ours ? " (senin!)" : "") + ", " + MarshalRules.TravelText(army.HoursToTarget)
                        : "Hedefi belli değil");
                MobileParty tracked = army.Party;
                Rows.Add(new CouncilRowVM(text, "sana " + MarshalRules.TravelText(MarshalRules.TravelHours(army.Distance, army.Party.Speed)), ours,
                    "Haritada izle", null, () =>
                    {
                        Campaign.Current.VisualTrackerManager.RegisterObject(tracked);
                        MenuUI.Success(tracked.Name + " haritada izleniyor");
                    }));
            }

            List<Marshal.FortLine> targets = Marshal.WeakTargets(state, marshal, 5);
            Rows.Add(CouncilRowVM.Header("ZAYIF HEDEFLER (kuşatma için)"));
            if (targets.Count == 0) Rows.Add(CouncilRowVM.Note("Serdarın görüş menzilinde düşman şehri ya da kalesi yok."));
            foreach (Marshal.FortLine fort in targets)
            {
                Settlement shown = fort.Settlement;
                Rows.Add(new CouncilRowVM(FortText(fort, false) + "\n" + fort.Settlement.MapFaction?.Name
                        + (fort.Settlement.IsUnderSiege ? " · zaten kuşatma altında" : "") + " · sana " + MarshalRules.TravelText(MarshalRules.TravelHours(fort.Distance, MobileParty.MainParty.Speed)),
                    "savunma ~" + fort.Defense, false, "Haritada göster", null, () => Track(shown)));
            }
        }

        private static string FortText(Marshal.FortLine fort, bool exact)
        {
            string approx = exact ? "" : "~";
            return fort.Settlement.Name + " (" + (fort.Settlement.IsCastle ? "kale" : "şehir") + ") · garnizon " + approx + fort.Garrison + ", milis "
                + approx + fort.Militia + " · sur " + fort.Wall + " · erzak " + (fort.FoodDays == int.MaxValue ? "yeterli" : approx + fort.FoodDays + " gün");
        }

        private static void Track(Settlement settlement)
        {
            Campaign.Current.VisualTrackerManager.RegisterObject(settlement);
            MenuUI.Success(settlement.Name + " haritada işaretlendi");
        }

        // ---- Hekim ----

        private void AddHealth(IntrigueState state)
        {
            Rows.Add(CouncilRowVM.Header("SAĞLIK (hekimin gözüyle)"));
            foreach (Physician.HealthLine line in Physician.Report(state))
                Rows.Add(new CouncilRowVM(line.Text, line.Risk > 0 ? "yıllık ~%" + line.Risk : "", line.Risk >= PhysicianRules.WarnYearlyRisk,
                    null, null, null));
        }

        // ---- Hazinedar ----

        private void AddIncome()
        {
            int workshops = Treasury.Workshops().Sum(Treasury.WorkshopIncome), caravans = Treasury.Caravans().Sum(Treasury.CaravanIncome);
            Rows.Add(CouncilRowVM.Header("HAZİNE"));
            Rows.Add(new CouncilRowVM("Günlük gelir (oyunun hesabı): atölyeler " + Treasury.Signed(workshops) + ", kervanlar " + Treasury.Signed(caravans),
                "haftalık " + Treasury.Signed((workshops + caravans) * 7), workshops + caravans < 0, null, null, null));
        }

        private void AddWorkshops(Treasury.Board board)
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
                List<CaravanLeg> recent = legs.Skip(Math.Max(0, legs.Count - 3)).ToList();
                bool losing = legs.Count >= 3 && recent.Sum(l => l.Profit) < 0;
                string text = (caravan.LeaderHero?.Name.ToString() ?? caravan.Name.ToString()) + " kervanı · " + where + "\n"
                    + (legs.Count == 0 ? "defterde henüz sefer yok"
                        : "son seferler: " + string.Join(", ", recent.Select(l => Treasury.TownName(l.FromId) + " > " + Treasury.TownName(l.ToId) + " " + Treasury.Signed(l.Profit))))
                    + (losing ? "  ·  zarar ediyor: dağıtmayı düşün" : "");
                MobileParty tracked = caravan;
                Rows.Add(new CouncilRowVM(text, "günde " + Treasury.Signed(Treasury.CaravanIncome(caravan)), losing, "Haritada izle", null, () =>
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

        // ---- Atama ----

        private void Dismiss(string what, Action<IntrigueState> dismiss)
        {
            Run(what, () => dismiss(SchemeEngine.State));
            _changed();
        }

        private void AssignTreasurer()
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

        private void AssignMarshal()
        {
            IntrigueState state = SchemeEngine.State;
            Picker.Pick("Serdar seç", Marshal.Candidates(state).OrderByDescending(h => Marshal.Tactics(h) + Marshal.Scouting(h)).ToList(), Marshal.Label,
                hero => Run("Serdar", () =>
                {
                    Marshal.Assign(SchemeEngine.State, hero);
                    _changed();
                }),
                null, hero => Categories.HeroPortrait(hero), sort: false);
        }

        private void AssignPhysician()
        {
            IntrigueState state = SchemeEngine.State;
            Picker.Pick("Saray hekimi seç", Physician.Candidates(state).OrderByDescending(Physician.Medicine).ToList(), Physician.Label,
                hero => Run("Hekim", () =>
                {
                    Physician.Assign(SchemeEngine.State, hero);
                    _changed();
                }),
                null, hero => Categories.HeroPortrait(hero), sort: false);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _marshal.OnFinalize();
            _treasurer.OnFinalize();
            _physician.OnFinalize();
            Clear(Rows);
        }
    }

    /// <summary>Divanda bir koltuk: boşsa açıklama ve "Ata"; doluysa portre, beceri, durum ve düğmeler.</summary>
    public sealed class CouncilSeatVM : TvhViewModel
    {
        private readonly Action _assign, _dismiss, _special;
        private ImageIdentifierVM _portrait;
        private bool _hasPerson, _hasSpecial;
        private string _name = "", _skillText = "", _detailText = "", _statusText = "", _specialText = "";

        public CouncilSeatVM(string header, string emptyText, Action assign, Action dismiss, Action special)
        {
            Header = header;
            EmptyText = emptyText;
            _assign = assign;
            _dismiss = dismiss;
            _special = special;
            _portrait = PortraitOf(null);
        }

        [DataSourceProperty] public string Header { get; }
        [DataSourceProperty] public string EmptyText { get; }
        [DataSourceProperty] public ImageIdentifierVM Portrait { get => _portrait; set => Set(ref _portrait, value, nameof(Portrait)); }
        [DataSourceProperty] public bool HasPerson { get => _hasPerson; set { if (Set(ref _hasPerson, value, nameof(HasPerson))) OnPropertyChangedWithValue(!value, nameof(IsEmpty)); } }
        [DataSourceProperty] public bool IsEmpty => !_hasPerson;
        [DataSourceProperty] public string Name { get => _name; set => Set(ref _name, value, nameof(Name)); }
        [DataSourceProperty] public string SkillText { get => _skillText; set => Set(ref _skillText, value, nameof(SkillText)); }
        [DataSourceProperty] public string DetailText { get => _detailText; set => Set(ref _detailText, value, nameof(DetailText)); }
        [DataSourceProperty] public string StatusText { get => _statusText; set => Set(ref _statusText, value, nameof(StatusText)); }
        [DataSourceProperty] public bool HasSpecial { get => _hasSpecial; set => Set(ref _hasSpecial, value, nameof(HasSpecial)); }
        [DataSourceProperty] public string SpecialText { get => _specialText; set => Set(ref _specialText, value, nameof(SpecialText)); }
        [DataSourceProperty] public string AssignText => "Ata";
        [DataSourceProperty] public string ChangeText => "Değiştir";
        [DataSourceProperty] public string DismissText => "Görevden al";

        public void Show(Hero hero, string skill, string detail, string status, bool special, string specialText = "")
        {
            if (Name != (hero?.Name.ToString() ?? "") || Portrait == null)
            {
                _portrait?.OnFinalize();
                Portrait = PortraitOf(hero);
            }
            HasPerson = hero != null;
            Name = hero?.Name.ToString() ?? "";
            SkillText = skill;
            DetailText = detail;
            StatusText = status;
            HasSpecial = special && _special != null;
            SpecialText = specialText;
        }

        public void ExecuteAssign() => _assign();

        public void ExecuteDismiss() => _dismiss();

        public void ExecuteSpecial() => _special?.Invoke();

        public override void OnFinalize()
        {
            base.OnFinalize();
            _portrait?.OnFinalize();
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
