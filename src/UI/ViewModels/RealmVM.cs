using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Bundle;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Saray > Krallık paneli (tasarım: docs/superpowers/specs/2026-10-21-krallik-sekmesi.md): krallığının klanları solda; seçili klana
    /// altın bağışı ve (hükümdarsan) fief verme sağda; altında barıştaki krallıklara iade edilebilecek yerler. İşlemler Diplomacy'nin
    /// (bağış, fief verme ve iade pencereleri buraya yönlenir: BundleShim); geri alınamayanlar onay ister.
    /// </summary>
    public sealed class RealmVM : TvhViewModel
    {
        private readonly Action _changed;
        private Clan _selected, _portraitClan;
        private int _gift;
        private ImageIdentifierVM _leaderPortrait;
        private string _subtitle = "", _summary = "", _emptyText = "", _selectedName = "", _selectedInfo = "", _donatePreview = "", _donateHint = "",
            _grantNote = "";
        private bool _hasKingdom, _hasSelection, _canDonate, _hasReturns, _isRuler;

        public RealmVM(Action changed)
        {
            _changed = changed;
            Clans = new MBBindingList<ClanItemVM>();
            Grants = new MBBindingList<ActionItemVM>();
            Returns = new MBBindingList<ActionItemVM>();
            _leaderPortrait = PortraitOf(null);
            Donation = new StepRowVM("Miktar", new List<int> { 0 }, () => _gift, v => _gift = v, v => v == 0 ? "yok" : SchemeFactors.Money(v), UpdateDonation);
        }

        [DataSourceProperty] public string ClansTitle => "KLANLAR";
        [DataSourceProperty] public string DonateTitle => "ALTIN BAĞIŞLA";
        [DataSourceProperty] public string GrantTitle => "TOPRAK VER";
        [DataSourceProperty] public string ReturnsTitle => "İADE EDİLEBİLİR TOPRAKLAR";
        [DataSourceProperty] public string DonateText => "Bağışla";
        [DataSourceProperty] public string DonateNote => "Altın klanın komutanlarına paylaşılır; lideri sana minnet duyar. Cömertlik ününü artırır.";
        [DataSourceProperty] public MBBindingList<ClanItemVM> Clans { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> Grants { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> Returns { get; }
        [DataSourceProperty] public StepRowVM Donation { get; }
        [DataSourceProperty] public bool HasKingdom { get => _hasKingdom; set { if (Set(ref _hasKingdom, value, nameof(HasKingdom))) OnPropertyChanged(nameof(HasNoKingdom)); } }
        [DataSourceProperty] public bool HasNoKingdom => !_hasKingdom;
        [DataSourceProperty] public string Subtitle { get => _subtitle; set => Set(ref _subtitle, value, nameof(Subtitle)); }
        [DataSourceProperty] public string Summary { get => _summary; set => Set(ref _summary, value, nameof(Summary)); }
        [DataSourceProperty] public string EmptyText { get => _emptyText; set => Set(ref _emptyText, value, nameof(EmptyText)); }
        [DataSourceProperty] public bool HasSelection { get => _hasSelection; set => Set(ref _hasSelection, value, nameof(HasSelection)); }
        [DataSourceProperty] public ImageIdentifierVM LeaderPortrait { get => _leaderPortrait; set => Set(ref _leaderPortrait, value, nameof(LeaderPortrait)); }
        [DataSourceProperty] public string SelectedName { get => _selectedName; set => Set(ref _selectedName, value, nameof(SelectedName)); }
        [DataSourceProperty] public string SelectedInfo { get => _selectedInfo; set => Set(ref _selectedInfo, value, nameof(SelectedInfo)); }
        [DataSourceProperty] public string DonatePreview { get => _donatePreview; set => Set(ref _donatePreview, value, nameof(DonatePreview)); }
        [DataSourceProperty] public string DonateHint { get => _donateHint; set => Set(ref _donateHint, value, nameof(DonateHint)); }
        [DataSourceProperty] public bool CanDonate { get => _canDonate; set => Set(ref _canDonate, value, nameof(CanDonate)); }
        [DataSourceProperty] public string GrantNote { get => _grantNote; set => Set(ref _grantNote, value, nameof(GrantNote)); }
        /// <summary>İade bölümü yalnızca hükümdara görünür (Diplomacy'nin kuralı).</summary>
        [DataSourceProperty] public bool IsRuler { get => _isRuler; set => Set(ref _isRuler, value, nameof(IsRuler)); }
        [DataSourceProperty] public bool HasReturns { get => _hasReturns; set { if (Set(ref _hasReturns, value, nameof(HasReturns))) OnPropertyChanged(nameof(HasNoReturns)); } }
        [DataSourceProperty] public bool HasNoReturns => !_hasReturns;
        [DataSourceProperty] public string NoReturnsText => "Şu an iade edilebilecek yer yok: barışta olduğun bir krallıktan aldığın şehir ve kaleler burada görünür.";

        /// <summary>Bu klan seçili açılsın (Diplomacy'nin bağış ve fief verme düğmelerinden).</summary>
        public void Focus(Clan clan)
        {
            if (clan == null || clan == _selected) return;
            _selected = clan;
            _gift = 0;
        }

        public void Refresh()
        {
            Kingdom kingdom = Clan.PlayerClan.Kingdom;
            HasKingdom = kingdom != null;
            Clear(Clans);
            Clear(Grants);
            Clear(Returns);
            if (kingdom == null)
            {
                EmptyText = "Bir krallıkta değilsin: klanlara bağış, toprak verme ve iade bir krallığın içinde yapılır.";
                Subtitle = Summary = GrantNote = "";
                HasSelection = HasReturns = IsRuler = false;
                return;
            }
            IsRuler = kingdom.Leader == Hero.MainHero;
            Subtitle = kingdom.Name + "  ·  hükümdar " + kingdom.Leader?.Name + (IsRuler ? " (sensin)" : "");
            List<Clan> clans = kingdom.Clans.Where(c => c != Clan.PlayerClan && !c.IsEliminated && c.Leader != null)
                .OrderByDescending(c => c == kingdom.RulingClan).ThenBy(c => c.IsUnderMercenaryService).ThenByDescending(c => c.Tier)
                .ThenBy(c => c.Name.ToString()).ToList();
            Summary = clans.Count + " klan  ·  kesende " + SchemeFactors.Money(Hero.MainHero.Gold) + "  ·  " + Clan.PlayerClan.Fiefs.Count + " yerin";

            if (_selected == null || !clans.Contains(_selected))
            {
                _selected = clans.FirstOrDefault();
                _gift = 0;
            }
            foreach (Clan clan in clans) Clans.Add(new ClanItemVM(clan, clan == _selected, Choose));
            RefreshSelection();
            RefreshReturns(kingdom);
        }

        private void Choose(Clan clan)
        {
            _selected = clan;
            _gift = 0;
            _changed();
        }

        private void RefreshSelection()
        {
            HasSelection = _selected != null;
            if (_selected == null)
            {
                GrantNote = "";
                return;
            }
            Hero leader = _selected.Leader;
            if (_portraitClan != _selected)
            {
                _portraitClan = _selected;
                _leaderPortrait?.OnFinalize();
                LeaderPortrait = PortraitOf(leader);
            }
            SelectedName = _selected.Name + " klanı";
            SelectedInfo = "Lideri " + leader.Name + "  ·  seninle " + SchemeFactors.Signed(leader.GetRelation(Hero.MainHero)) + "  ·  kademe " + _selected.Tier
                + "  ·  " + _selected.Fiefs.Count + " yer" + (_selected.IsUnderMercenaryService ? "  ·  paralı asker" : "")
                + (_selected == _selected.Kingdom?.RulingClan ? "  ·  hükümdar klanı" : "");

            List<int> steps = PeaceTreatyRules.GiftSteps(Hero.MainHero.Gold);
            if (!steps.Contains(_gift)) _gift = 0;
            Donation.SetSteps(steps);
            UpdateDonation();
            RefreshGrants(leader);
        }

        private void RefreshGrants(Hero leader)
        {
            List<Town> fiefs = Clan.PlayerClan.Fiefs.Where(t => !t.IsOwnerUnassigned).OrderBy(t => t.IsCastle).ThenBy(t => t.Name.ToString()).ToList();
            string blocked = !IsRuler ? "Toprak (şehir ya da kale) vermek hükümdarın işi; sen bağış yapabilirsin."
                : fiefs.Count == 0 ? "Verebileceğin bir şehrin ya da kalen yok."
                : _selected.IsUnderMercenaryService || _selected.IsMinorFaction ? "Paralı klanlara toprak verilmez."
                : !DiplomacyCompat.IsAvailable ? "Diplomacy paketli değil."
                : DiplomacyCompat.GrantFiefBlocked(_selected);
            GrantNote = blocked ?? "Senden az yeri olan klanlar kıskanabilir: onlarla ilişkin biraz düşer.";
            if (blocked != null) return;
            Clan target = _selected;
            string clanName = target.Name.ToString();
            foreach (Town town in fiefs)
            {
                Settlement settlement = town.Settlement;
                int preview = DiplomacyCompat.GrantFiefPreview(settlement, leader);
                string place = town.Name + (town.IsTown ? " (şehir)" : " (kale)");
                Grants.Add(new ActionItemVM(place + (preview >= 0 ? "  ·  ilişki +" + preview : ""),
                    settlement.IsUnderSiege ? "Kuşatma altında" : null,
                    () => MenuUI.Confirm(town.Name + " " + Tr.Dative(clanName) + " verilsin mi?",
                        place + " artık " + clanName + " klanının olacak; geri alınamaz.\n\n" + leader.Name + " ile ilişki +" + Math.Max(0, preview)
                        + ". Senden az yeri olan klanlar kıskanabilir (onlarla ilişkin biraz düşer).",
                        () =>
                        {
                            DiplomacyCompat.GrantFief(settlement, target);
                            MenuUI.Success(town.Name + " " + Tr.Dative(clanName) + " verildi");
                            _changed();
                        }, null)));
            }
        }

        private void UpdateDonation()
        {
            if (_selected == null) return;
            Donation.Update();
            var model = Campaign.Current.Models.DiplomacyModel;
            int baseRelation = RealmRules.DonationRelation(_gift, model.DenarsToInfluence(), model.GetRelationValueOfSupportingClan(), model.GetInfluenceCostOfSupportingClan());
            int relation = baseRelation <= 0 ? 0 : (int)Math.Floor(model.GetRelationIncreaseFactor(Hero.MainHero, _selected.Leader, baseRelation));
            DonatePreview = _gift <= 0 ? "" : _selected.Leader.Name + " ile ilişki +" + relation;
            string blocked = !DiplomacyCompat.IsAvailable ? "Diplomacy paketli değil" : _gift <= 0 ? "Miktar seç" : _gift > Hero.MainHero.Gold ? "Kesen yetmez" : null;
            DonateHint = blocked ?? "";
            CanDonate = blocked == null;
        }

        public void ExecuteDonate()
        {
            if (!CanDonate || _selected == null) return;
            Clan clan = _selected;
            int amount = _gift;
            MenuUI.Confirm(clan.Name + " klanına " + SchemeFactors.Money(amount) + " bağışlansın mı?",
                "Altın klanın komutanlarına paylaşılır; geri alınamaz. " + DonatePreview + ".", () =>
                {
                    if (!DiplomacyCompat.DonateGold(clan, amount)) throw new InvalidOperationException("Diplomacy'nin bağış işlemine ulaşılamadı");
                    MenuUI.Success(clan.Name + " klanına " + SchemeFactors.Money(amount) + " bağışladın");
                    _gift = 0;
                    _changed();
                }, null);
        }

        private void RefreshReturns(Kingdom mine)
        {
            if (!IsRuler || !DiplomacyCompat.IsAvailable)
            {
                HasReturns = false;
                return;
            }
            foreach (Kingdom kingdom in Kingdom.All.Where(k => k != mine && !k.IsEliminated && !FactionManager.IsAtWarAgainstFaction(k, mine))
                .OrderBy(k => k.Name.ToString()))
                foreach (Settlement settlement in DiplomacyCompat.ReturnableFiefs(kingdom))
                {
                    int preview = DiplomacyCompat.ReturnFiefPreview(settlement, kingdom);
                    Kingdom to = kingdom;
                    string toName = to.Name.ToString();
                    Returns.Add(new ActionItemVM(settlement.Name + ", " + Tr.Dative(toName) + (preview >= 0 ? "  ·  ilişki +" + preview : ""), null,
                        () => MenuUI.Confirm(settlement.Name + " " + Tr.Dative(toName) + " iade edilsin mi?",
                            settlement.Name + " eski sahibine döner; geri alınamaz.\n\nEski sahibiyle ilişki +" + Math.Max(0, preview)
                            + "; " + toName + " krallığının öteki klanları da memnun kalır. Krallığının yayılmacılık ünü azalır.",
                            () =>
                            {
                                DiplomacyCompat.ReturnFief(settlement, to);
                                MenuUI.Success(settlement.Name + " " + Tr.Dative(toName) + " iade edildi");
                                _changed();
                            }, null)));
                }
            HasReturns = Returns.Count > 0;
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Clans);
            Clear(Grants);
            Clear(Returns);
            _leaderPortrait?.OnFinalize();
            Donation.OnFinalize();
        }
    }

    /// <summary>Krallığının bir klanı: sancak, ad ve kademe, lideri ve ilişkin, yerleri ve gücü; tıklanınca seçilir.</summary>
    public sealed class ClanItemVM : TvhViewModel
    {
        private readonly Clan _clan;
        private readonly Action<Clan> _choose;

        public ClanItemVM(Clan clan, bool selected, Action<Clan> choose)
        {
            _clan = clan;
            _choose = choose;
            IsSelected = selected;
            Banner = new BannerImageIdentifierVM(clan.Banner, true);
            Name = clan.Name + "  ·  kademe " + clan.Tier;
            Info = clan.Leader.Name + "  ·  seninle " + SchemeFactors.Signed(clan.Leader.GetRelation(Hero.MainHero)) + "  ·  " + clan.Fiefs.Count + " yer"
                + "  ·  güç " + ((int)clan.CurrentTotalStrength).ToString("N0");
            Tag = clan == clan.Kingdom?.RulingClan ? "hükümdar klanı" : clan.IsUnderMercenaryService ? "paralı asker" : "";
        }

        [DataSourceProperty] public ImageIdentifierVM Banner { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string Info { get; }
        [DataSourceProperty] public string Tag { get; }
        [DataSourceProperty] public bool IsSelected { get; }

        public void ExecuteSelect()
        {
            if (!IsSelected) _choose(_clan);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Banner?.OnFinalize();
        }
    }
}
