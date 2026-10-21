using System;
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
    /// <summary>Sol sütundaki casus başı kartı.</summary>
    public sealed class SpymasterVM : TvhViewModel
    {
        private readonly Action _changed;
        private ImageIdentifierVM _portrait;
        private bool _hasSpymaster;
        private string _name = "";
        private string _skillText = "";
        private string _statusText = "";
        private string _limitsText = "";
        private string _intelText = "";
        private string _missionText = "";
        private bool _canDismiss;

        public SpymasterVM(Action changed)
        {
            _changed = changed;
            _portrait = PortraitOf(null);
        }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get => _portrait; set => Set(ref _portrait, value, nameof(Portrait)); }
        [DataSourceProperty] public bool HasSpymaster { get => _hasSpymaster; set { if (Set(ref _hasSpymaster, value, nameof(HasSpymaster))) OnPropertyChangedWithValue(!value, nameof(HasNoSpymaster)); } }
        [DataSourceProperty] public bool HasNoSpymaster => !_hasSpymaster;
        [DataSourceProperty] public string Name { get => _name; set => Set(ref _name, value, nameof(Name)); }
        [DataSourceProperty] public string SkillText { get => _skillText; set => Set(ref _skillText, value, nameof(SkillText)); }
        [DataSourceProperty] public string StatusText { get => _statusText; set => Set(ref _statusText, value, nameof(StatusText)); }
        [DataSourceProperty] public string LimitsText { get => _limitsText; set => Set(ref _limitsText, value, nameof(LimitsText)); }
        [DataSourceProperty] public string IntelText { get => _intelText; set => Set(ref _intelText, value, nameof(IntelText)); }
        [DataSourceProperty] public bool CanDismiss { get => _canDismiss; set => Set(ref _canDismiss, value, nameof(CanDismiss)); }
        [DataSourceProperty] public string MissionText { get => _missionText; set => Set(ref _missionText, value, nameof(MissionText)); }
        [DataSourceProperty] public string MissionButtonText => "Görev...";
        [DataSourceProperty] public string AssignText => "Casus başı ata";
        [DataSourceProperty] public string ChangeText => "Değiştir";
        [DataSourceProperty] public string DismissText => "Görevden al";
        [DataSourceProperty] public string EmptyText =>
            "Henüz bir casus başın yok.\n\nKlanından Roguery'si yüksek birini ata: entrikaları o yürütür, sınırları onun becerisi belirler.";

        public void Refresh(IntrigueState state)
        {
            Hero spymaster = SchemeFactors.Spymaster(state);
            HasSpymaster = spymaster != null;
            int today = SchemeFactors.Today;
            IntelText = "Raporlar " + state.Reports.Count + "  ·  Sırlar " + state.Secrets.Count(s => !s.Exposed)
                + "  ·  Kozlar " + state.Hooks.Count(h => h.Until > today);
            if (spymaster == null)
            {
                if (Name.Length > 0 || Portrait == null)
                {
                    _portrait?.OnFinalize();
                    Portrait = PortraitOf(null);
                }
                Name = "";
                SkillText = StatusText = LimitsText = "";
                CanDismiss = false;
                return;
            }
            if (Name != spymaster.Name.ToString() || Portrait == null)
            {
                _portrait?.OnFinalize();
                Portrait = PortraitOf(spymaster);
            }
            Name = spymaster.Name.ToString();
            int roguery = SchemeFactors.Roguery(spymaster);
            SkillText = "Roguery " + roguery + "  ·  Hesapçı " + SchemeFactors.Signed(spymaster.GetTraitLevel(DefaultTraits.Calculating));
            string busy = SchemeFactors.SpymasterBusyReason(spymaster);
            StatusText = busy == null ? "Görevde" : "Durdu: şu an " + busy + " (entrikalar bekliyor)";
            LimitsText = "Entrika " + state.Schemes.Count + "/" + SchemeMath.MaxActiveSchemes(roguery)
                + "  ·  Gölge ağı " + state.Networks.Count + "/" + SchemeMath.MaxNetworks(roguery);
            CanDismiss = state.Schemes.Count == 0;
            MissionText = "Görev: " + SpymasterMissions.StatusText(state);
        }

        public void ExecuteMission() => EventCards.ShowMissionChoice(_changed);

        public void ExecuteAssign()
        {
            Picker.Pick("Casus başı seç", Council.Candidates(SchemeEngine.State, "casus başın").OrderByDescending(SchemeFactors.Roguery).ToList(),
                SchemeFactors.SpymasterLabel,
                hero => Run("Casus başı", () => SchemeEngine.AssignSpymaster(hero)),
                null, hero => Categories.HeroPortrait(hero), sort: false);
        }

        public void ExecuteDismiss()
        {
            Run("Casus başı", SchemeEngine.DismissSpymaster);
            _changed();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _portrait?.OnFinalize();
        }
    }

    /// <summary>Süren bir entrikanın kartı (tasarım P1: portre, aşama çubuğu, kalan gün, başarı/yakalanma, eylemler).</summary>
    public sealed class SchemeItemVM : TvhViewModel
    {
        /// <summary>Prefab'daki aşama çubuğunun genişliği (TvhSchemeCard.xml ile aynı olmalı).</summary>
        public const float BarWidth = 360f;

        private readonly Scheme _scheme;
        private readonly Action _changed;

        public SchemeItemVM(IntrigueState state, Scheme scheme, Action changed)
        {
            _scheme = scheme;
            _changed = changed;
            bool onSettlement = SchemeRules.TargetsSettlement(scheme.Kind);
            Hero target = onSettlement ? null : Hero.Find(scheme.TargetId);
            Settlement place = onSettlement ? SettlementSchemes.Find(scheme) : null;
            Hero spymaster = SchemeFactors.Spymaster(state);
            int min = SchemeRules.MinPhases(scheme.Kind), max = SchemeRules.MaxPhases(scheme.Kind);

            Portrait = PortraitOf(SchemeEngine.PortraitHero(scheme));
            TargetName = SchemeEngine.TargetName(scheme);
            KindText = SchemeRules.Name(scheme.Kind).ToUpper(Tr.Culture);
            Affiliation = target != null ? SchemeFactors.Affiliation(target) : place != null ? SettlementSchemes.Affiliation(place) : "";
            PhaseText = "Aşama " + scheme.Phase + "/" + max + (SchemeRules.IsStrike(scheme.Kind) ? "  (en az " + min + ")" : "");
            PhaseBarWidth = BarWidth * Math.Min(scheme.Phase, max) / max;
            MinMarkerOffset = BarWidth * min / max - 1f;
            ShowMinMarker = SchemeRules.IsStrike(scheme.Kind);

            if (target == null && place?.Town == null) TimeText = "Hedef bulunamadı";
            else if (spymaster == null) TimeText = "Durdu: casus başın yok";
            else if (SchemeFactors.SpymasterBusyReason(spymaster) != null) TimeText = "Durdu: casus başın " + SchemeFactors.SpymasterBusyReason(spymaster);
            else if (scheme.PendingEvent > 0 || scheme.AwaitingChoice) TimeText = "Kararını bekliyor";
            else if (SchemeEngine.IsWaiting(scheme))
                TimeText = scheme.Kind == SchemeKind.Sabotage ? "Plan hazır: " + place.Name + " kuşatılınca vurabilirsin" : "Plan hazır: ne zaman vuracağın sana kalmış";
            else TimeText = "Sonraki aşamaya " + Math.Max(1, scheme.PhaseDays - scheme.DaysIntoPhase) + " gün";

            if (place?.Town != null)
            {
                SettlementSchemeInputs inputs = SettlementSchemes.Inputs(state, spymaster, place, scheme.Kind, scheme);
                int success = SettlementSchemeRules.Success(scheme.Kind, inputs);
                if (scheme.Phase < max)
                {
                    SettlementSchemeInputs atMax = SettlementSchemes.Inputs(state, spymaster, place, scheme.Kind, scheme);
                    atMax.CompletedPhases = max;
                    SuccessText = "Başarı %" + success + "  (" + max + ". aşamada %" + SettlementSchemeRules.Success(scheme.Kind, atMax) + ")";
                }
                else SuccessText = "Başarı %" + success;
                DiscoveryText = "Her aşamada yakalanma %" + SettlementSchemeRules.Discovery(scheme.Kind, inputs);
                string tags = scheme.Kind == SchemeKind.Sabotage ? "Hedef: " + SettlementSchemeRules.MethodName(scheme.Method) + "   " : "";
                TagsText = (tags + SettlementSchemes.Tags(state, place)).Trim();
            }
            else if (target != null)
            {
                SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
                int success = SchemeMath.Success(scheme.Kind, inputs);
                if (SchemeRules.IsStrike(scheme.Kind) && scheme.Phase < max)
                {
                    SchemeInputs atMax = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
                    atMax.CompletedPhases = max;
                    SuccessText = "Başarı %" + success + "  (" + max + ". aşamada %" + SchemeMath.Success(scheme.Kind, atMax) + ")";
                }
                else SuccessText = "Başarı %" + success;
                DiscoveryText = "Her aşamada yakalanma %" + SchemeMath.Discovery(scheme.Kind, inputs);
                string tags = "";
                if (scheme.Method != SchemeMethod.None) tags += "Yöntem: " + MethodName(scheme.Method) + "   ";
                if (scheme.FeastRisk) tags += "Ziyafette vurulacak (ifşa cezası 1,5 kat)   ";
                if (inputs.NetworkSuccess > 0) tags += "Gölge ağı: +" + inputs.NetworkSuccess + " başarı   ";
                if (inputs.AgentSuccess > 0) tags += "Ajanlar: +" + inputs.AgentSuccess + " başarı   ";
                if (inputs.HasReport && SchemeRules.IsStrike(scheme.Kind)) tags += "Güncel rapor: +10   ";
                if (inputs.TargetAlerted) tags += "Hedef tetikte: -15";
                TagsText = tags.Trim();
            }

            ShowExecute = SchemeRules.IsStrike(scheme.Kind);
            ExecuteBlocked = SchemeEngine.CannotExecuteReason(scheme);
        }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string TargetName { get; }
        [DataSourceProperty] public string KindText { get; }
        [DataSourceProperty] public string Affiliation { get; }
        [DataSourceProperty] public string PhaseText { get; }
        [DataSourceProperty] public float PhaseBarWidth { get; }
        [DataSourceProperty] public float MinMarkerOffset { get; }
        [DataSourceProperty] public bool ShowMinMarker { get; }
        [DataSourceProperty] public string TimeText { get; }
        [DataSourceProperty] public string SuccessText { get; } = "";
        [DataSourceProperty] public string DiscoveryText { get; } = "";
        [DataSourceProperty] public string TagsText { get; } = "";
        [DataSourceProperty] public bool HasTags => TagsText.Length > 0;
        [DataSourceProperty] public bool ShowExecute { get; }
        [DataSourceProperty] public bool CanExecute => ExecuteBlocked == null;
        [DataSourceProperty] public string ExecuteHint => ExecuteBlocked ?? "";
        [DataSourceProperty] public string ExecuteText => "Harekete geç";
        [DataSourceProperty] public string CancelText => "İptal et";

        private string ExecuteBlocked { get; }

        public void ExecuteAct()
        {
            if (ExecuteBlocked == null) EventCards.ShowExecuteChoice(_scheme, _changed);
        }

        public void ExecuteCancel()
        {
            MenuUI.Confirm("Entrikayı iptal et", "Harcanan para geri gelmez. İptal edilsin mi?", () => SchemeEngine.Cancel(_scheme), null);
        }

        public static string MethodName(SchemeMethod method) =>
            method == SchemeMethod.Poison ? "zehir" : method == SchemeMethod.Dagger ? "hançer" : method == SchemeMethod.Accident ? "kaza süsü" : "-";

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }
}
