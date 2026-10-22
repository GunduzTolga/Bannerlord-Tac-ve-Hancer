using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Bundle;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Savaş ve barış > İsyanlar paneli (Entrika ekranı; Diplomacy'nin grup penceresi de buraya yönlenir). Gruplar ve düğmelerin kuralı
    /// Diplomacy'nin (onun ViewModel'leri arkada çalışır); görünüş ve hoşnutsuzlar bölümü (Saray'ın lord kartları) bizim.
    /// Varsayılan kendi krallığın; Saray'dan ya da ansiklopediden başka bir krallığınki açılabilir.
    /// </summary>
    public sealed class RebellionsVM : TvhViewModel
    {
        private Kingdom _kingdom;
        private readonly Action _close;
        private object _diplomacyWindow;
        private string _subtitle, _summary, _emptyText, _lordsEmptyText, _createHint;
        private bool _hasNoFactions, _hasNoLords, _canCreateFaction, _finalized, _isForeign;

        /// <param name="close">Entrika ekranını kapatır (Diplomacy ansiklopediye giderken).</param>
        public RebellionsVM(Action close)
        {
            _close = close;
            Factions = new MBBindingList<RebelFactionCardVM>();
            Lords = new MBBindingList<CourtLordItemVM>();
        }

        /// <summary>Gösterilecek krallık (null: kendi krallığın). Sekme açılınca kurulur.</summary>
        public void Show(Kingdom kingdom) => _kingdom = kingdom;

        [DataSourceProperty] public bool IsForeign { get => _isForeign; set => Set(ref _isForeign, value, nameof(IsForeign)); }
        [DataSourceProperty] public string OwnKingdomText => "Kendi krallığın";

        public void ExecuteOwnKingdom()
        {
            _kingdom = null;
            RefreshLater();
        }
        [DataSourceProperty] public string Subtitle { get => _subtitle; set => Set(ref _subtitle, value, nameof(Subtitle)); }
        [DataSourceProperty] public string Summary { get => _summary; set => Set(ref _summary, value, nameof(Summary)); }
        [DataSourceProperty] public string FactionsHeader => "İSYANCI GRUPLAR";
        [DataSourceProperty] public string LordsHeader => "HOŞNUTSUZLAR (bildiğin kadarıyla)";
        [DataSourceProperty] public MBBindingList<RebelFactionCardVM> Factions { get; }
        [DataSourceProperty] public MBBindingList<CourtLordItemVM> Lords { get; }
        [DataSourceProperty] public bool HasNoFactions { get => _hasNoFactions; set => Set(ref _hasNoFactions, value, nameof(HasNoFactions)); }
        [DataSourceProperty] public string EmptyText { get => _emptyText; set => Set(ref _emptyText, value, nameof(EmptyText)); }
        [DataSourceProperty] public bool HasNoLords { get => _hasNoLords; set => Set(ref _hasNoLords, value, nameof(HasNoLords)); }
        [DataSourceProperty] public string LordsEmptyText { get => _lordsEmptyText; set => Set(ref _lordsEmptyText, value, nameof(LordsEmptyText)); }
        [DataSourceProperty] public string CreateFactionText => "Grup kur...";
        [DataSourceProperty] public bool CanCreateFaction { get => _canCreateFaction; set => Set(ref _canCreateFaction, value, nameof(CanCreateFaction)); }
        [DataSourceProperty] public string CreateFactionHint { get => _createHint; set => Set(ref _createHint, value, nameof(CreateFactionHint)); }
        public void Refresh()
        {
            IntrigueState state = SchemeEngine.State;
            if (_kingdom != null && (_kingdom.IsEliminated || _kingdom == Clan.PlayerClan.Kingdom)) _kingdom = null;
            Kingdom kingdom = _kingdom ?? Clan.PlayerClan.Kingdom;
            IsForeign = _kingdom != null && Clan.PlayerClan.Kingdom != null;
            if (kingdom == null || !DiplomacyCompat.IsAvailable)
            {
                Clear(Factions);
                Clear(Lords);
                HasNoFactions = true;
                HasNoLords = false;
                CanCreateFaction = false;
                CreateFactionHint = "";
                Subtitle = kingdom == null ? "Bir krallıkta değilsin" : kingdom.Name.ToString();
                Summary = "";
                EmptyText = kingdom == null
                    ? "İsyan grupları bir krallığın içinde kurulur. Bir krallığa katılınca burada görürsün; başka bir krallığınkine Saray sekmesinden "
                      + "(krallığı seç, \"İsyanlar...\") bakabilirsin."
                    : "Diplomacy paketli değil: isyan grupları onun kuralıyla çalışır.";
                LordsEmptyText = "";
                return;
            }
            RefreshFor(state, kingdom);
        }

        private void RefreshFor(IntrigueState state, Kingdom kingdom)
        {
            Hero ruler = kingdom.Leader;
            Subtitle = kingdom.Name + (ruler != null ? "  ·  Hükümdar: " + ruler.Name : "")
                + (Clan.PlayerClan.Kingdom == kingdom ? "  ·  Sen: hükümdarla " + (ruler == Hero.MainHero ? "hükümdar sensin" : SchemeFactors.Signed(Court.RelationWithRuler(Hero.MainHero))) : "");

            Clear(Factions);
            List<object> factions = DiplomacyCompat.RawRebelFactions(kingdom);
            foreach (object faction in factions) Factions.Add(new RebelFactionCardVM(kingdom, faction, _close, RefreshLater));
            HasNoFactions = Factions.Count == 0;
            RebelFactionCardVM strongest = Factions.OrderByDescending(f => f.Ratio).FirstOrDefault();
            Summary = Factions.Count == 0 ? "Örgütlü bir isyan grubu yok."
                : Factions.Count + " grup  ·  en güçlüsü " + strongest.Leader + ": %" + (int)Math.Round(strongest.Ratio * 100) + " (gereken %"
                    + (int)Math.Round(strongest.Required * 100) + ")" + (Factions.Any(f => f.IsAtWar) ? "  ·  iç savaş sürüyor" : "");
            EmptyText = "Bu krallıkta örgütlü bir isyan grubu yok. Gruplar, hükümdarına küs (ilişkisi eksi) klanlar arasında kendiliğinden kurulur: "
                + "bir klan grubu kurar, ötekiler katılır; grubun gücü sadık klanlarınkini yeterince geçince önderi isyanı başlatır (ayrılık ya da "
                + "tahttan indirme). Aşağıda hoşnutsuzları görüyorsun; casus başının söylentileri onları daha da küstürür.";

            Clear(Lords);
            List<Hero> discontent = Court.ClanLeaders(kingdom)
                .Where(h => Court.KnowledgeOf(state, h) != Court.Knowledge.Unknown && LetterRules.IsDiscontent(Court.RelationWithRuler(h)))
                .OrderBy(Court.RelationWithRuler).Take(12).ToList();
            foreach (Hero lord in discontent) Lords.Add(new CourtLordItemVM(state, lord, RefreshLater, (kind, target) =>
                MenuUI.Info("Casusluğu Entrika ekranından başlat (Klan > Entrika > Yeni entrika): " + target.Name)));
            HasNoLords = Lords.Count == 0;
            LordsEmptyText = Court.ClanLeaders(kingdom).Any(h => Court.KnowledgeOf(state, h) != Court.Knowledge.Unknown)
                ? "Bildiğin kadarıyla hükümdarına küs bir klan lideri yok."
                : "Bu sarayda kulağın yok: o krallıkta casusluk, sorgu ya da gölge ağı gerekir.";

            (_diplomacyWindow as ViewModel)?.OnFinalize();
            _diplomacyWindow = DiplomacyCompat.CreateFactionsWindow(kingdom, _close);
            CanCreateFaction = DiplomacyCompat.Read<bool>(_diplomacyWindow, "ShouldShowCreateFaction");
            string hint = DiplomacyCompat.Read<HintViewModel>(_diplomacyWindow, "CreateFactionHint")?.HintText?.ToString() ?? "";
            CreateFactionHint = CanCreateFaction ? "Bedel: " + DiplomacyCompat.Read<int>(_diplomacyWindow, "CreateFactionInfluenceCost") + " nüfuz"
                : hint.Replace("\n", " ").Trim();
        }

        /// <summary>Düğme olayı bitince (bir sonraki karede) yenilenir: tıklama sürerken bağlı liste değişmesin.</summary>
        private void RefreshLater() => MenuUI.Defer(() =>
        {
            if (!_finalized) Refresh();
        });

        public void ExecuteCreateFaction()
        {
            if (CanCreateFaction) Run("Grup kur", () => DiplomacyCompat.Invoke(_diplomacyWindow, "OnCreateFaction"));
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            _finalized = true;
            Clear(Factions);
            Clear(Lords);
            (_diplomacyWindow as ViewModel)?.OnFinalize();
        }
    }

    /// <summary>Bir isyancı grup: önderi, isteği, durumu, güç dengesi, katılanlar ve Diplomacy'nin düğmeleri.</summary>
    public sealed class RebelFactionCardVM : TvhViewModel
    {
        /// <summary>TvhRebelFactionCard.xml'deki güç çubuğunun genişliği.</summary>
        public const float BarWidth = 300f;

        private readonly object _item;

        public RebelFactionCardVM(Kingdom kingdom, object faction, Action close, Action changed)
        {
            _item = DiplomacyCompat.CreateFactionItem(faction, close, changed);
            Clan sponsor = DiplomacyCompat.Read<Clan>(faction, "SponsorClan");
            List<Clan> clans = (DiplomacyCompat.Read<System.Collections.IEnumerable>(faction, "Clans")?.OfType<Clan>() ?? Enumerable.Empty<Clan>()).ToList();
            bool abdication = DiplomacyCompat.Read<object>(faction, "RebelDemandType")?.ToString() == "Abdication";
            IsAtWar = DiplomacyCompat.Read<bool>(faction, "AtWar");
            Required = DiplomacyCompat.Read<float>(faction, "RequiredStrengthRatio");
            int strength = DiplomacyCompat.Read<int>(_item, "FactionStrength");
            int loyal = DiplomacyCompat.Read<int>(_item, "LoyalistStrength");
            Ratio = strength + loyal > 0 ? (float)strength / (strength + loyal) : 0f;

            Hero leader = sponsor?.Leader;
            Leader = sponsor?.Name.ToString() ?? "?";
            Portrait = PortraitOf(leader);
            Name = Leader + " önderliğinde: " + (abdication ? "tahttan indirme" : "ayrılık");
            DemandText = abdication
                ? "İstek: " + (kingdom.Leader?.Name.ToString() ?? "hükümdar") + " tahttan insin; yerine grubun önderi geçsin."
                : "İstek: krallıktan ayrılıp kendi krallıklarını kurmak.";
            StatusText = IsAtWar ? "İç savaş sürüyor."
                : Ratio >= Required && Required > 0 ? "İsyana hazır: önderi her gün isyanı başlatabilir."
                : "Destek topluyor.";
            IsNotAtWar = !IsAtWar;
            bool mine = clans.Contains(Clan.PlayerClan);
            ParticipantsText = "Katılanlar (" + clans.Count + "): " + string.Join(", ", clans.Select(c => c.Name + (c.Leader != null && c.Leader != kingdom.Leader
                ? " (" + SchemeFactors.Signed(Court.RelationWithRuler(c.Leader)) + ")" : ""))) + (mine ? "  ·  sen de içindesin" : "");
            StrengthText = "Grup " + strength + "  ·  sadıklar " + loyal;
            RatioText = "Güç %" + (int)Math.Round(Ratio * 100) + "  ·  gereken %" + (int)Math.Round(Required * 100);
            FillWidth = BarWidth * Math.Max(0f, Math.Min(1f, Ratio));
            MarkerOffset = BarWidth * Math.Max(0f, Math.Min(1f, Required));
            // Casus başının notu: grubun en gönülsüz üyesi (hükümdarla arası en iyi olan): grubu bölmek isteyen ona gider.
            Hero weakest = clans.Where(c => c != sponsor && c != Clan.PlayerClan && c.Leader != null && c.Leader.IsAlive)
                .Select(c => c.Leader).OrderByDescending(Court.RelationWithRuler).FirstOrDefault();
            NoteText = weakest != null ? "En gönülsüz üye: " + weakest.Name + " (" + weakest.Clan.Name + ", hükümdarla "
                + SchemeFactors.Signed(Court.RelationWithRuler(weakest)) + "): grubu bölmek isteyen ona gider." : "";

            Actions = new MBBindingList<ActionItemVM>();
            if (_item == null) return;
            // Savaş başladıysa katılma ve ayrılma yok (Diplomacy'nin kendi penceresi de gizler; savaşan gruptan çıkmak iç savaşı bozar).
            bool member = DiplomacyCompat.Read<bool>(_item, "ShouldShowLeave");
            if (member && !IsAtWar)
                Actions.Add(new ActionItemVM("Gruptan ayrıl", null, () => DiplomacyCompat.Invoke(_item, "OnLeave"), true));
            else if (!member && !IsAtWar)
                Actions.Add(new ActionItemVM("Gruba katıl", DiplomacyCompat.JoinBlocked(faction), () =>
                {
                    DiplomacyCompat.Invoke(_item, "OnJoin");
                    changed();
                }));
            // Diplomacy'nin kendi komutları da (ayrıl, isyan) "changed" ile bizi yeniler; o da bir sonraki karede (RebellionsVM.RefreshLater).
            if (DiplomacyCompat.Read<bool>(_item, "ShouldShowStartRebellion") && !IsAtWar)
                Actions.Add(new ActionItemVM("İsyanı başlat",
                    DiplomacyCompat.Read<bool>(_item, "ShouldAllowStartRebellion") ? null : "Grubun gücü sadıklarınkini geçmeli",
                    () => MenuUI.Confirm("İsyanı başlat", Leader + " önderliğindeki grup " + kingdom.Name + " krallığına karşı ayaklanacak: iç savaş. Emin misin?",
                        () => DiplomacyCompat.Invoke(_item, "OnStartRebellion"), null), true));
            if (DiplomacyCompat.Read<bool>(_item, "ShouldShowRecruitSupport"))
                Actions.Add(new ActionItemVM("Destek topla: kime gidilir...", null, () => DiplomacyCompat.Invoke(_item, "OnRecruitSupport")));
        }

        public float Ratio { get; }
        public float Required { get; }
        public string Leader { get; }
        public bool IsAtWar { get; }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string DemandText { get; }
        [DataSourceProperty] public string StatusText { get; }
        [DataSourceProperty] public bool IsNotAtWar { get; }
        [DataSourceProperty] public bool IsWar => IsAtWar;
        [DataSourceProperty] public string ParticipantsText { get; }
        [DataSourceProperty] public string StrengthText { get; }
        [DataSourceProperty] public string RatioText { get; }
        [DataSourceProperty] public float FillWidth { get; }
        [DataSourceProperty] public float MarkerOffset { get; }
        [DataSourceProperty] public string NoteText { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> Actions { get; }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
            (_item as ViewModel)?.OnFinalize();
        }
    }
}
