using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Lordlar penceresi (UI.LordsScreen): krallığının lordları, tanıdıkların ve ziyafet davetlileri. Her kartta ulak ve hediye,
    /// yanına çağır, ziyafete davet, haritada izle ve kur. Üstte ziyafet (kendi şehrinde ya da kalende).
    /// </summary>
    public sealed class LordsVM : TvhViewModel
    {
        public const int TabKingdom = 0, TabKnown = 1, TabGuests = 2;
        /// <summary>Bir listede en fazla bu kadar kart (portreler bellek ister).</summary>
        private const int MaxCards = 40;

        /// <summary>Ziyafet davet listesi: pencere kapanıp açılsa da oturum boyunca durur (kayda yazılmaz).</summary>
        private static readonly List<string> Guests = new List<string>();

        private readonly Action _close;
        private int _tab;
        private bool _finalized, _canFeast, _hasNoLords;
        private string _subtitle, _feastText, _feastHint, _emptyText;

        public LordsVM(Action close)
        {
            _close = close;
            Tabs = new MBBindingList<TabVM>
            {
                new TabVM(TabKingdom, "Krallığın", SelectTab),
                new TabVM(TabKnown, "Tanıdıkların", SelectTab),
                new TabVM(TabGuests, "Davetliler", SelectTab),
            };
            Lords = new MBBindingList<LordCardVM>();
            _tab = Clan.PlayerClan.Kingdom != null ? TabKingdom : TabKnown;
            Refresh();
        }

        [DataSourceProperty] public string Title => "Lordlar";
        [DataSourceProperty] public string Subtitle { get => _subtitle; set => Set(ref _subtitle, value, nameof(Subtitle)); }
        [DataSourceProperty] public MBBindingList<TabVM> Tabs { get; }
        [DataSourceProperty] public MBBindingList<LordCardVM> Lords { get; }
        [DataSourceProperty] public bool HasNoLords { get => _hasNoLords; set => Set(ref _hasNoLords, value, nameof(HasNoLords)); }
        [DataSourceProperty] public string EmptyText { get => _emptyText; set => Set(ref _emptyText, value, nameof(EmptyText)); }
        [DataSourceProperty] public string FeastText { get => _feastText; set => Set(ref _feastText, value, nameof(FeastText)); }
        [DataSourceProperty] public bool CanFeast { get => _canFeast; set => Set(ref _canFeast, value, nameof(CanFeast)); }
        [DataSourceProperty] public string FeastHint { get => _feastHint; set => Set(ref _feastHint, value, nameof(FeastHint)); }
        [DataSourceProperty] public string CloseText => "Kapat";

        private void SelectTab(TabVM tab)
        {
            _tab = tab.Index;
            Refresh();
        }

        private List<Hero> GuestHeroes() =>
            Guests.Select(Hero.Find).Where(h => h != null && h.IsAlive).ToList();

        private static bool IsListed(Hero h) => h != null && h.IsAlive && h.IsLord && !h.IsChild && h != Hero.MainHero && h.Clan != Clan.PlayerClan;

        private List<Hero> Listed()
        {
            switch (_tab)
            {
                case TabKingdom:
                    Kingdom kingdom = Clan.PlayerClan.Kingdom;
                    if (kingdom == null) return new List<Hero>();
                    return Court.ClanLeaders(kingdom).Concat(new[] { kingdom.Leader }).Where(IsListed).Distinct()
                        .OrderByDescending(h => h == kingdom.Leader).ThenByDescending(h => h.GetRelation(Hero.MainHero)).ToList();
                case TabGuests:
                    return GuestHeroes();
                default:
                    return Hero.AllAliveHeroes.Where(h => IsListed(h) && h.HasMet)
                        .OrderByDescending(h => h.GetRelation(Hero.MainHero)).ToList();
            }
        }

        public void Refresh()
        {
            IntrigueState state = SchemeEngine.State;
            foreach (TabVM tab in Tabs) tab.IsSelected = tab.Index == _tab;
            Guests.RemoveAll(id => Hero.Find(id) is Hero h ? !h.IsAlive : true);
            Tabs[TabGuests].Name = "Davetliler (" + Guests.Count + "/" + LordContactRules.MaxGuests + ")";

            Clear(Lords);
            List<Hero> listed = Listed();
            foreach (Hero lord in listed.Take(MaxCards)) Lords.Add(new LordCardVM(state, lord, Guests, RefreshLater));
            HasNoLords = Lords.Count == 0;
            EmptyText = _tab == TabKingdom ? (Clan.PlayerClan.Kingdom == null ? "Bir krallıkta değilsin: Tanıdıkların sekmesine bak." : "Krallığında başka lord yok.")
                : _tab == TabGuests ? "Davet listen boş. Lord kartlarındaki \"Ziyafete davet et\" ile ekle (en fazla " + LordContactRules.MaxGuests + ")."
                : "Henüz tanıştığın bir lord yok.";

            var hall = LordContacts.FeastHall();
            Subtitle = (hall != null ? "Bulunduğun yer: " + hall.Name + " (senin)" : "Ziyafet için kendi şehrinde ya da kalende olmalısın")
                + (listed.Count > MaxCards ? "  ·  ilk " + MaxCards + " lord (ilişkiye göre)" : "");
            List<Hero> guests = GuestHeroes();
            FeastText = "Ziyafet ver (" + guests.Count + " davetli, " + SchemeFactors.Money(LordContactRules.FeastCost(guests.Count)) + ")";
            string blocked = LordContacts.FeastBlocked(state, guests);
            CanFeast = blocked == null;
            FeastHint = blocked ?? "Gelme olasılıkları: " + string.Join(", ", guests.Select(g => g.Name + " %" + LordContacts.AcceptChance(g)));
        }

        /// <summary>Düğme olayı bitince (bir sonraki karede) yenilenir: tıklama sürerken bağlı liste değişmesin.</summary>
        private void RefreshLater() => MenuUI.Defer(() =>
        {
            if (!_finalized) Refresh();
        });

        public void ExecuteFeast()
        {
            if (!CanFeast) return;
            Run("Ziyafet", () =>
            {
                string result = LordContacts.Feast(SchemeEngine.State, GuestHeroes());
                Guests.Clear();
                var ok = new EventCards.CardOption { Label = "Tamam", Apply = () => { } };
                MenuUI.Defer(() => EventCards.Show("ZİYAFET", "Ziyafet", result, CardImages.ForEvent(9), null,
                    new List<EventCards.CardOption> { ok }, ok, null));
                RefreshLater();
            });
        }

        public void ExecuteClose() => _close();

        public override void OnFinalize()
        {
            base.OnFinalize();
            _finalized = true;
            Clear(Lords);
        }
    }

    /// <summary>Bir lord: nerede, seninle ve hükümdarıyla arası, son görüşme; ulak, çağır, davet, izle, kur.</summary>
    public sealed class LordCardVM : TvhViewModel
    {
        public LordCardVM(IntrigueState state, Hero lord, List<string> guests, Action changed)
        {
            int today = SchemeFactors.Today;
            Portrait = PortraitOf(lord);
            Name = lord.Name.ToString();
            ClanText = (lord.Clan?.Name.ToString() ?? "klansız") + (lord.Clan != null ? "  ·  kademe " + lord.Clan.Tier : "")
                + (lord.Clan?.Kingdom != null ? "  ·  " + lord.Clan.Kingdom.Name : "")
                + (lord.IsKingdomLeader ? "  ·  hükümdar" : lord.Clan?.Leader == lord ? "  ·  klan lideri" : "");
            WhereText = Where(lord);
            int relation = lord.GetRelation(Hero.MainHero);
            RelationText = "Seninle " + SchemeFactors.Signed(relation)
                + (lord.Clan?.Kingdom != null && !lord.IsKingdomLeader ? "  ·  hükümdarıyla: " + Court.MoodText(state, lord) : "");
            LastText = lord.HasMet ? "Son görüşme: " + Math.Max(0, (int)(CampaignTime.Now - lord.LastMeetingTimeWithPlayer).ToDays) + " gün önce" : "Tanışmadınız";
            bool invited = guests.Contains(lord.StringId);
            var tags = new List<string>();
            if (invited) tags.Add("ziyafete davetli");
            if (LordContacts.SummonOf(state, lord) != null) tags.Add("sana doğru yolda");
            if (state.HasValidReport(lord.StringId, today)) tags.Add("raporun var");
            if (state.ActiveHook(lord.StringId, today) != null) tags.Add("üzerinde kozun var");
            if (Messengers.PendingTo(state, lord) != null) tags.Add("haberci yolda");
            TagsText = string.Join("  ·  ", tags);

            Actions = new MBBindingList<ActionItemVM>
            {
                LordActions.Audience(state, lord, changed),
                new ActionItemVM("Yanına çağır (" + LordContacts.SummonCostText(lord) + ", gelme %" + LordContacts.SummonChance(lord) + ")",
                    LordContacts.SummonBlocked(state, lord), () =>
                    {
                        MenuUI.Info(LordContacts.Summon(SchemeEngine.State, lord));
                        changed();
                    }),
                invited
                    ? new ActionItemVM("Davetten çıkar", null, () =>
                    {
                        guests.Remove(lord.StringId);
                        changed();
                    }, true)
                    : new ActionItemVM("Ziyafete davet et (gelme %" + LordContacts.AcceptChance(lord) + ")",
                        guests.Count >= LordContactRules.MaxGuests ? "Davet listesi dolu (" + LordContactRules.MaxGuests + ")" : null, () =>
                        {
                            guests.Add(lord.StringId);
                            changed();
                        }),
                new ActionItemVM("Haritada izle", null, () =>
                {
                    string missing = Court.Track(lord);
                    if (missing != null) throw new InvalidOperationException(missing);
                    MenuUI.Success(lord.Name + " haritada izleniyor");
                }),
            };
            foreach (ActionItemVM action in LordActions.Romance(state, lord, changed)) Actions.Add(action);
        }

        /// <summary>"Pravend'de", "yolda: Sargot'a gidiyor", "esir"; yanında yaklaşık yol.</summary>
        private static string Where(Hero lord)
        {
            if (lord.IsPrisoner) return "Esir";
            string place = lord.CurrentSettlement != null ? Tr.Locative(lord.CurrentSettlement.Name.ToString())
                : lord.PartyBelongedTo?.TargetSettlement != null ? "Yolda: " + Tr.Dative(lord.PartyBelongedTo.TargetSettlement.Name.ToString()) + " gidiyor"
                : lord.PartyBelongedTo != null ? "Yolda" : "Bilinmiyor";
            float? distance = Messengers.DistanceTo(lord);
            return place + (distance != null ? "  ·  ~" + Messengers.TravelDays(lord) + " gün" : "");
        }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string ClanText { get; }
        [DataSourceProperty] public string WhereText { get; }
        [DataSourceProperty] public string RelationText { get; }
        [DataSourceProperty] public string LastText { get; }
        [DataSourceProperty] public string TagsText { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> Actions { get; }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }
}
