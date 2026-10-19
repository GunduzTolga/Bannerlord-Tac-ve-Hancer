using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// "Kişiler" paneli: hakkında rapor, sır ya da koz bulunan herkes. Seçilen kişinin dosyası sağda:
    /// son rapor, sırlar (ifşa), koz (kullanımlar) ve doğrudan entrika başlatma. (Ansiklopedi bağlantısı gelene kadar
    /// lord sayfasındaki bilgilerin yeri burası.)
    /// </summary>
    public sealed class PeopleVM : TvhViewModel
    {
        private readonly Action _changed;
        private readonly Action<SchemeKind, Hero> _startScheme;
        private Hero _selected;
        private bool _hasNoPeople;

        public PeopleVM(Action changed, Action<SchemeKind, Hero> startScheme)
        {
            _changed = changed;
            _startScheme = startScheme;
            People = new MBBindingList<PersonItemVM>();
            Dossier = new DossierVM(changed, startScheme);
        }

        [DataSourceProperty] public MBBindingList<PersonItemVM> People { get; }
        [DataSourceProperty] public DossierVM Dossier { get; }
        [DataSourceProperty] public bool HasNoPeople { get => _hasNoPeople; set => Set(ref _hasNoPeople, value, nameof(HasNoPeople)); }
        [DataSourceProperty] public string EmptyText =>
            "Henüz kimse hakkında bir şey bilmiyorsun.\n\nCasusluk raporları, bulunan sırlar ve şantajla kazanılan kozlar burada kişi kişi toplanır.";

        public void Select(Hero hero)
        {
            _selected = hero;
            Refresh(SchemeEngine.State);
        }

        public void Refresh(IntrigueState state)
        {
            int today = SchemeFactors.Today;
            var lastSeen = new Dictionary<string, int>();
            void Note(string id, int day)
            {
                if (!lastSeen.TryGetValue(id, out int known) || day > known) lastSeen[id] = day;
            }
            foreach (IntelReport report in state.Reports) Note(report.TargetId, report.Day);
            foreach (Secret secret in state.Secrets) Note(secret.TargetId, secret.Day);
            foreach (Hook hook in state.Hooks.Where(h => h.Until > today)) Note(hook.TargetId, hook.Day);

            List<Hero> heroes = lastSeen.OrderByDescending(p => p.Value)
                .Select(p => Hero.Find(p.Key)).Where(h => h != null).ToList();
            if (_selected != null && !heroes.Contains(_selected)) _selected = null;
            if (_selected == null) _selected = heroes.FirstOrDefault();

            Clear(People);
            foreach (Hero hero in heroes) People.Add(new PersonItemVM(state, hero, hero == _selected, OnPerson));
            HasNoPeople = heroes.Count == 0;
            Dossier.Refresh(state, _selected);
        }

        private void OnPerson(PersonItemVM item)
        {
            _selected = item.Hero;
            foreach (PersonItemVM person in People) person.IsSelected = person == item;
            Dossier.Refresh(SchemeEngine.State, _selected);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(People);
            Dossier.OnFinalize();
        }
    }

    public sealed class PersonItemVM : TvhViewModel
    {
        private readonly Action<PersonItemVM> _onSelect;
        private bool _isSelected;

        public PersonItemVM(IntrigueState state, Hero hero, bool selected, Action<PersonItemVM> onSelect)
        {
            Hero = hero;
            _isSelected = selected;
            _onSelect = onSelect;
            Portrait = PortraitOf(hero);
            Name = hero.Name.ToString() + (hero.IsAlive ? "" : " (öldü)");
            Affiliation = SchemeFactors.Affiliation(hero);
            int today = SchemeFactors.Today;
            var tags = new List<string>();
            IntelReport report = state.LatestReport(hero.StringId);
            if (report != null) tags.Add("rapor " + (today - report.Day) + " gün önce" + (today - report.Day > SchemeMath.ReportValidDays ? " (eski)" : ""));
            int secrets = state.Secrets.Count(s => s.TargetId == hero.StringId && !s.Exposed);
            if (secrets > 0) tags.Add(secrets + " sır");
            Hook hook = state.ActiveHook(hero.StringId, today);
            if (hook != null) tags.Add("koz " + (hook.Until - today) + " gün");
            TagsText = string.Join(" · ", tags);
        }

        public Hero Hero { get; }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string Affiliation { get; }
        [DataSourceProperty] public string TagsText { get; }
        [DataSourceProperty] public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value, nameof(IsSelected)); }

        public void ExecuteSelect() => _onSelect(this);

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }

    /// <summary>Seçili kişinin dosyası.</summary>
    public sealed class DossierVM : TvhViewModel
    {
        private readonly Action _changed;
        private readonly Action<SchemeKind, Hero> _startScheme;
        private Hero _hero;
        private ImageIdentifierVM _portrait;
        private bool _hasPerson, _hasReport, _hasSecrets, _hasHook;
        private string _name = "", _affiliation = "", _relationText = "", _reportTitle = "", _reportText = "", _hookText = "", _courtText = "";

        public DossierVM(Action changed, Action<SchemeKind, Hero> startScheme)
        {
            _changed = changed;
            _startScheme = startScheme;
            Secrets = new MBBindingList<SecretItemVM>();
            HookActions = new MBBindingList<ActionItemVM>();
            StartActions = new MBBindingList<ActionItemVM>();
            _portrait = PortraitOf(null);
        }

        [DataSourceProperty] public bool HasPerson { get => _hasPerson; set => Set(ref _hasPerson, value, nameof(HasPerson)); }
        [DataSourceProperty] public ImageIdentifierVM Portrait { get => _portrait; set => Set(ref _portrait, value, nameof(Portrait)); }
        [DataSourceProperty] public string Name { get => _name; set => Set(ref _name, value, nameof(Name)); }
        [DataSourceProperty] public string Affiliation { get => _affiliation; set => Set(ref _affiliation, value, nameof(Affiliation)); }
        [DataSourceProperty] public string RelationText { get => _relationText; set => Set(ref _relationText, value, nameof(RelationText)); }
        [DataSourceProperty] public string CourtText { get => _courtText; set => Set(ref _courtText, value, nameof(CourtText)); }
        [DataSourceProperty] public bool HasReport { get => _hasReport; set => Set(ref _hasReport, value, nameof(HasReport)); }
        [DataSourceProperty] public string ReportTitle { get => _reportTitle; set => Set(ref _reportTitle, value, nameof(ReportTitle)); }
        [DataSourceProperty] public string ReportText { get => _reportText; set => Set(ref _reportText, value, nameof(ReportText)); }
        [DataSourceProperty] public bool HasSecrets { get => _hasSecrets; set => Set(ref _hasSecrets, value, nameof(HasSecrets)); }
        [DataSourceProperty] public MBBindingList<SecretItemVM> Secrets { get; }
        [DataSourceProperty] public bool HasHook { get => _hasHook; set => Set(ref _hasHook, value, nameof(HasHook)); }
        [DataSourceProperty] public string HookText { get => _hookText; set => Set(ref _hookText, value, nameof(HookText)); }
        [DataSourceProperty] public MBBindingList<ActionItemVM> HookActions { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> StartActions { get; }
        [DataSourceProperty] public string ReportHeader => "İSTİHBARAT RAPORU";
        [DataSourceProperty] public string SecretsHeader => "SIRLAR";
        [DataSourceProperty] public string HookHeader => "KOZ";
        [DataSourceProperty] public string StartHeader => "ENTRİKA BAŞLAT";
        [DataSourceProperty] public string DeleteReportText => "Raporu sil";

        public void Refresh(IntrigueState state, Hero hero)
        {
            int today = SchemeFactors.Today;
            if (hero != _hero || _portrait == null)
            {
                _portrait?.OnFinalize();
                Portrait = PortraitOf(hero);
            }
            _hero = hero;
            Clear(Secrets);
            HookActions.Clear();
            StartActions.Clear();
            HasPerson = hero != null;
            if (hero == null)
            {
                Name = Affiliation = RelationText = ReportTitle = ReportText = HookText = CourtText = "";
                HasReport = HasSecrets = HasHook = false;
                return;
            }
            Name = hero.Name.ToString();
            Affiliation = SchemeFactors.Affiliation(hero);
            RelationText = hero.IsAlive ? "Seninle ilişkisi: " + SchemeFactors.Signed(Hero.MainHero.GetRelation(hero)) : "Hayatta değil";
            CourtText = hero.IsAlive && hero.Clan?.Kingdom != null && hero.Clan.Kingdom.Leader != hero
                ? "Hükümdarı " + hero.Clan.Kingdom.Leader.Name + " ile: " + Intrigue.Court.MoodText(state, hero)
                : "";

            IntelReport report = state.LatestReport(hero.StringId);
            HasReport = report != null;
            if (report != null)
            {
                int age = today - report.Day;
                ReportTitle = "Gün " + report.Day + " (" + age + " gün önce" + (age > SchemeMath.ReportValidDays ? ", eski: bonus vermez" : ", geçerli") + ")";
                ReportText = SchemeOutcomes.ReportText(report);
            }

            bool blackmailRunning = state.FindScheme(hero.StringId)?.Kind == SchemeKind.Blackmail;
            foreach (Secret secret in state.Secrets.Where(s => s.TargetId == hero.StringId))
                Secrets.Add(new SecretItemVM(state, secret, blackmailRunning));
            HasSecrets = Secrets.Count > 0;

            Hook hook = state.ActiveHook(hero.StringId, today);
            HasHook = hook != null && hero.IsAlive;
            if (HasHook)
            {
                Letter pending = Messengers.PendingTo(state, hero);
                HookText = (hook.Until - today) + " gün daha geçerli. Bırakma dışındaki her talep kozu harcar ve ilişkiyi -"
                    + SchemeMath.HookUseRelationPenalty + " düşürür. "
                    + (Messengers.IsFaceToFace(hero) ? "Yüz yüzesiniz: talep hemen olur." : "Uzakta: talep haberciyle gider, cevap varınca gelir.")
                    + (pending != null ? "\nOna giden bir haberci yolda (" + Math.Max(0, pending.ArriveDay - today) + " gün)." : "");
                foreach (Intrigue.HookActions.Option option in Intrigue.HookActions.Options(hero))
                {
                    Intrigue.HookActions.Use use = option.Use;
                    string blocked = option.Blocked
                        ?? (use != Intrigue.HookActions.Use.Release && pending != null ? "Ona giden bir haberci zaten yolda" : null);
                    HookActions.Add(new ActionItemVM(option.Label + Messengers.HookRouteText(hero, use), blocked, () =>
                    {
                        Messengers.UseHook(SchemeEngine.State, hero, use);
                        _changed();
                    }));
                }
            }

            if (hero.IsAlive)
                foreach (SchemeKind kind in new[] { SchemeKind.Spy, SchemeKind.Blackmail, SchemeKind.Kidnap, SchemeKind.Murder })
                {
                    string blocked = SchemeEngine.CannotStartReason(state) ?? SchemeFactors.BlockedReason(state, hero, kind);
                    SchemeKind chosen = kind;
                    StartActions.Add(new ActionItemVM(SchemeRules.Name(kind) + "...", blocked, () => _startScheme(chosen, hero), kind == SchemeKind.Murder));
                }
            if (hero.IsAlive && hero.IsLord && hero.Clan != Clan.PlayerClan)
            {
                StartActions.Add(LordActions.Audience(state, hero, _changed));
                StartActions.Add(new ActionItemVM("Haritada izle (konuşmaya git)", null, () =>
                {
                    string missing = Intrigue.Court.Track(hero);
                    if (missing != null) throw new InvalidOperationException(missing);
                    MenuUI.Success(hero.Name + " haritada izleniyor");
                }));
                bool rumoring = state.Mission == SpymasterMission.SpreadRumors && state.RumorTargetId == hero.StringId;
                if (!rumoring)
                    StartActions.Add(new ActionItemVM("Hakkında söylenti yay (casus başı görevi)",
                        SchemeFactors.Spymaster(state) == null ? "Önce bir casus başı ata" : SpymasterMissions.RumorBlocked(state, hero), () =>
                        {
                            SpymasterMissions.SetMission(SchemeEngine.State, SpymasterMission.SpreadRumors, hero);
                            _changed();
                        }));
            }
        }

        public void ExecuteDeleteReport()
        {
            if (_hero == null) return;
            IntrigueState state = SchemeEngine.State;
            string id = _hero.StringId;
            MenuUI.Confirm("Raporu sil", _hero.Name + " hakkındaki rapor silinsin mi? (Sırlar ve koz kalır.)", () =>
            {
                state.Reports.RemoveAll(r => r.TargetId == id);
                MenuUI.Success("Rapor silindi");
            }, null);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _portrait?.OnFinalize();
            Clear(Secrets);
        }
    }

    public sealed class SecretItemVM : TvhViewModel
    {
        private readonly Secret _secret;

        public SecretItemVM(IntrigueState state, Secret secret, bool blackmailRunning)
        {
            _secret = secret;
            Name = SecretRules.Name(secret.Kind);
            Description = SecretRules.Description(secret.Kind);
            StateText = secret.Exposed ? "İfşa edildi: artık herkes biliyor" : "Yalnızca sen biliyorsun (gün " + secret.Day + ")";
            ExposeHint = secret.Exposed ? "Zaten ifşa edildi" : blackmailRunning ? "Şantaj sürerken ifşa edilemez" : "";
            CanExpose = ExposeHint.Length == 0;
        }

        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string Description { get; }
        [DataSourceProperty] public string StateText { get; }
        [DataSourceProperty] public bool CanExpose { get; }
        [DataSourceProperty] public string ExposeHint { get; }
        [DataSourceProperty] public string ExposeText => "İfşa et";
        [DataSourceProperty] public bool IsExposed => _secret.Exposed;

        public void ExecuteExpose()
        {
            if (!CanExpose) return;
            IntrigueState state = SchemeEngine.State;
            Secret secret = _secret;
            MenuUI.Confirm("Sır ifşası: " + Hero.Find(secret.TargetId)?.Name,
                SecretRules.Description(secret.Kind) + "\n\nİfşa edilirse hükümdarıyla (gizli ilişkide eşiyle) arası açılır. %"
                + SecretRules.SourceTracedChance + " ihtimalle kaynağın sen olduğun anlaşılır (seninle ilişkisi -"
                + SecretRules.SourceTracedPenalty + "). İfşa edilsin mi?",
                () => SchemeOutcomes.ExposeSecret(state, secret), null);
        }
    }

    /// <summary>Esir satırı (P2.1): kim, nerede, sorguya etkisi olan özellikleri, sorgula düğmesi.</summary>
    public sealed class PrisonerItemVM : TvhViewModel
    {
        private readonly Interrogation.Prisoner _prisoner;
        private readonly Action _changed;

        public PrisonerItemVM(IntrigueState state, Interrogation.Prisoner prisoner, Action changed)
        {
            _prisoner = prisoner;
            _changed = changed;
            Hero hero = prisoner.Hero;
            Portrait = PortraitOf(hero);
            Name = hero.Name.ToString();
            Affiliation = SchemeFactors.Affiliation(hero);
            WhereText = prisoner.WhereText;
            TraitsText = "İlişki " + SchemeFactors.Signed(Hero.MainHero.GetRelation(hero))
                + "  ·  Hesapçı " + SchemeFactors.Signed(hero.GetTraitLevel(DefaultTraits.Calculating))
                + "  ·  Cesaret " + SchemeFactors.Signed(hero.GetTraitLevel(DefaultTraits.Valor))
                + "  ·  Onur " + SchemeFactors.Signed(hero.GetTraitLevel(DefaultTraits.Honor));
            ChanceText = "Konuştur %" + Interrogation.Chance(state, hero, InterrogationMethod.Talk)
                + "  ·  Baskı %" + Interrogation.Chance(state, hero, InterrogationMethod.Pressure)
                + "  ·  Anlaşma %" + Interrogation.Chance(state, hero, InterrogationMethod.Deal);
            Hint = Interrogation.BlockedReason(state, prisoner) ?? "";
            IntelText = state.HasValidReport(hero.StringId, SchemeFactors.Today) ? "Güncel raporun var" : "";
        }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string Affiliation { get; }
        [DataSourceProperty] public string WhereText { get; }
        [DataSourceProperty] public string TraitsText { get; }
        [DataSourceProperty] public string ChanceText { get; }
        [DataSourceProperty] public string IntelText { get; }
        [DataSourceProperty] public string Hint { get; }
        [DataSourceProperty] public bool CanInterrogate => Hint.Length == 0;
        [DataSourceProperty] public string InterrogateText => "Sorgula...";

        public void ExecuteInterrogate()
        {
            if (CanInterrogate) EventCards.ShowInterrogation(_prisoner, _changed);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }

    /// <summary>Gölge ağı satırı.</summary>
    public sealed class NetworkItemVM : TvhViewModel
    {
        public const float BarWidth = 240f;
        private readonly string _settlementId;

        public NetworkItemVM(IntrigueState state, ShadowNetwork network)
        {
            _settlementId = network.SettlementId;
            Settlement town = Settlement.Find(network.SettlementId);
            TownName = town?.Name.ToString() ?? network.SettlementId;
            FactionText = town?.MapFaction?.Name.ToString() ?? "";
            LevelText = "Seviye " + network.Level + "/" + SchemeMath.MaxNetworkLevel;
            LevelBarWidth = BarWidth * network.Level / SchemeMath.MaxNetworkLevel;
            UpkeepText = "Haftalık bakım " + SchemeFactors.Money(SchemeMath.NetworkUpkeep(network.Level));
            BonusText = "Bu şehirde başarı +" + SchemeMath.NetworkSuccess(network.Level, true) + ", yakalanma -"
                + SchemeMath.NetworkDiscovery(network.Level, true) + "; aynı krallıkta yarısı";
            DescriptionText = town != null ? ShadowNetworks.Describe(state, town) : "";
            SinceText = "Gün " + network.Since + "'den beri";
        }

        [DataSourceProperty] public string TownName { get; }
        [DataSourceProperty] public string FactionText { get; }
        [DataSourceProperty] public string LevelText { get; }
        [DataSourceProperty] public float LevelBarWidth { get; }
        [DataSourceProperty] public string UpkeepText { get; }
        [DataSourceProperty] public string BonusText { get; }
        [DataSourceProperty] public string DescriptionText { get; }
        [DataSourceProperty] public string SinceText { get; }
        [DataSourceProperty] public string DisbandText => "Dağıt";

        public void ExecuteDisband()
        {
            Settlement town = Settlement.Find(_settlementId);
            if (town == null) return;
            IntrigueState state = SchemeEngine.State;
            MenuUI.Confirm("Ağı dağıt: " + town.Name, "Harcanan para geri gelmez. Dağıtılsın mı?", () => ShadowNetworks.Disband(state, town), null);
        }
    }

    /// <summary>Sezilen ve sana karşı kurulan bir entrika.</summary>
    public sealed class PlotItemVM : TvhViewModel
    {
        public PlotItemVM(IntrigueState state, AiPlot plot, Action changed)
        {
            Hero plotter = Hero.Find(plot.PlotterId);
            Hero target = Hero.Find(plot.TargetId);
            Portrait = PortraitOf(plotter);
            Title = (plotter?.Name.ToString() ?? "?") + (plot.Kind == SchemeKind.Kidnap ? " kaçırma" : " suikast") + " planlıyor";
            Detail = "Hedef: " + (target == Hero.MainHero ? "sen" : target?.Name.ToString() ?? "?")
                + (plotter != null ? "  ·  " + SchemeFactors.Affiliation(plotter) : "")
                + "  ·  Sezildi: gün " + plot.StartDay + (plot.Protected ? "  ·  Koruma tutuldu" : "");
            Actions = new MBBindingList<ActionItemVM>();
            foreach (IntrigueEvents.Option option in IntriguePopups.PlotOptions(state, plot).Where(o => !o.Label.StartsWith("Şimdilik")))
            {
                Action apply = option.Apply;
                Actions.Add(new ActionItemVM(option.Label, option.DisabledHint, () =>
                {
                    plot.Asked = true;
                    apply();
                    changed();
                }, option.Label.StartsWith("Karşı")));
            }
        }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string Title { get; }
        [DataSourceProperty] public string Detail { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> Actions { get; }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }
}
