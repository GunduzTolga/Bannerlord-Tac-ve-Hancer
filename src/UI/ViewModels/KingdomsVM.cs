using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Bundle;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Savaş ve barış > Krallıklar paneli (tasarım: docs/superpowers/specs/2026-10-21-kralliklar-sekmesi.md): solda bütün krallıklar ve
    /// seninle durumları; sağda seçilinin hükümdarı, gücü, bağları ve ne düşündüğü, altında yapılabilecekler (savaş ilanı, saldırmazlık,
    /// ittifak, ticaret, savaşa çağırma, ulak; savaştaysan Barış Masası ve Savaş Raporu). Kurallar KingdomTies'ta.
    /// </summary>
    public sealed class KingdomsVM : TvhViewModel
    {
        private readonly Action _changed;
        private readonly Action<Kingdom> _openWar;
        private Kingdom _selected, _portraitKingdom;
        private ImageIdentifierVM _rulerPortrait, _banner;
        private string _subtitle = "", _summary = "", _emptyText = "", _selectedName = "", _rulerLine = "", _statsLine = "", _stanceLine = "";
        private bool _hasKingdoms, _hasSelection, _isHostile, _isFriendly, _isNeutral;

        public KingdomsVM(Action changed, Action<Kingdom> openWar)
        {
            _changed = changed;
            _openWar = openWar;
            Items = new MBBindingList<KingdomItemVM>();
            Lines = new MBBindingList<WarLineVM>();
            Actions = new MBBindingList<ActionItemVM>();
            _rulerPortrait = PortraitOf(null);
            _banner = PortraitOf(null);
        }

        [DataSourceProperty] public string ListTitle => "KRALLIKLAR";
        [DataSourceProperty] public string LinesTitle => "DURUM";
        [DataSourceProperty] public string ActionsTitle => "NE YAPABİLİRSİN";
        [DataSourceProperty] public MBBindingList<KingdomItemVM> Items { get; }
        [DataSourceProperty] public MBBindingList<WarLineVM> Lines { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> Actions { get; }
        [DataSourceProperty] public bool HasKingdoms { get => _hasKingdoms; set { if (Set(ref _hasKingdoms, value, nameof(HasKingdoms))) OnPropertyChanged(nameof(HasNoKingdoms)); } }
        [DataSourceProperty] public bool HasNoKingdoms => !_hasKingdoms;
        [DataSourceProperty] public string Subtitle { get => _subtitle; set => Set(ref _subtitle, value, nameof(Subtitle)); }
        [DataSourceProperty] public string Summary { get => _summary; set => Set(ref _summary, value, nameof(Summary)); }
        [DataSourceProperty] public string EmptyText { get => _emptyText; set => Set(ref _emptyText, value, nameof(EmptyText)); }
        [DataSourceProperty] public bool HasSelection { get => _hasSelection; set => Set(ref _hasSelection, value, nameof(HasSelection)); }
        [DataSourceProperty] public ImageIdentifierVM RulerPortrait { get => _rulerPortrait; set => Set(ref _rulerPortrait, value, nameof(RulerPortrait)); }
        [DataSourceProperty] public ImageIdentifierVM Banner { get => _banner; set => Set(ref _banner, value, nameof(Banner)); }
        [DataSourceProperty] public string SelectedName { get => _selectedName; set => Set(ref _selectedName, value, nameof(SelectedName)); }
        [DataSourceProperty] public string RulerLine { get => _rulerLine; set => Set(ref _rulerLine, value, nameof(RulerLine)); }
        [DataSourceProperty] public string StatsLine { get => _statsLine; set => Set(ref _statsLine, value, nameof(StatsLine)); }
        [DataSourceProperty] public string StanceLine { get => _stanceLine; set => Set(ref _stanceLine, value, nameof(StanceLine)); }
        /// <summary>Seninle durumu: savaş kırmızı, ittifak, saldırmazlık ve ticaret altın, barış soluk.</summary>
        [DataSourceProperty] public bool IsHostile { get => _isHostile; set => Set(ref _isHostile, value, nameof(IsHostile)); }
        [DataSourceProperty] public bool IsFriendly { get => _isFriendly; set => Set(ref _isFriendly, value, nameof(IsFriendly)); }
        [DataSourceProperty] public bool IsNeutral { get => _isNeutral; set => Set(ref _isNeutral, value, nameof(IsNeutral)); }

        /// <summary>Bu krallık seçili açılsın.</summary>
        public void Focus(Kingdom kingdom)
        {
            if (kingdom != null) _selected = kingdom;
        }

        public void Refresh()
        {
            Kingdom mine = KingdomTies.Mine;
            List<Kingdom> kingdoms = Kingdom.All.Where(k => !k.IsEliminated && k != mine && k.Leader != null)
                .OrderBy(KingdomTies.Order).ThenByDescending(k => k.CurrentTotalStrength).ToList();
            HasKingdoms = kingdoms.Count > 0;
            EmptyText = "Calradia'da senden başka krallık kalmadı.";
            Subtitle = mine == null ? "Bir krallığın yok: bağları izleyebilir, hükümdarlara ulak gönderebilirsin."
                : mine.Name + "  ·  " + (KingdomTies.IsRuler ? "hükümdarsın: divansız karar verirsin" : "hükümdar " + mine.Leader?.Name + ": anlaşmaları o yapar");
            Summary = mine == null ? "" : kingdoms.Count(KingdomTies.AtWar) + " savaş  ·  " + kingdoms.Count(KingdomTies.Allied) + " müttefik  ·  "
                + (int)Clan.PlayerClan.Influence + " nüfuzun  ·  kesende " + SchemeFactors.Money(Hero.MainHero.Gold);

            if (_selected == null || !kingdoms.Contains(_selected)) _selected = kingdoms.FirstOrDefault();
            Clear(Items);
            foreach (Kingdom kingdom in kingdoms) Items.Add(new KingdomItemVM(kingdom, kingdom == _selected, Choose));
            RefreshSelection();
        }

        private void Choose(Kingdom kingdom)
        {
            _selected = kingdom;
            _changed();
        }

        private void RefreshSelection()
        {
            Clear(Lines);
            Clear(Actions);
            HasSelection = _selected != null;
            if (_selected == null) return;
            Kingdom k = _selected;
            Hero ruler = k.Leader;
            if (_portraitKingdom != k || _rulerPortrait == null)
            {
                _portraitKingdom = k;
                _rulerPortrait?.OnFinalize();
                RulerPortrait = PortraitOf(ruler);
                _banner?.OnFinalize();
                Banner = new BannerImageIdentifierVM(k.Banner, true);
            }
            SelectedName = k.Name.ToString();
            RulerLine = "Hükümdar " + ruler.Name + "  ·  seninle " + SchemeFactors.Signed(ruler.GetRelation(Hero.MainHero)) + "  ·  " + k.Culture?.Name;
            float expansion = DiplomacyCompat.Expansionism(k);
            string expansionWord = expansion < 0 ? "" : KingdomsRules.Expansion(expansion, DiplomacyCompat.CriticalExpansionism);
            StatsLine = "Güç " + ((int)k.CurrentTotalStrength).ToString("N0") + "  ·  " + k.Clans.Count(c => !c.IsEliminated) + " klan  ·  "
                + k.Fiefs.Count(t => t.IsTown) + " şehir, " + k.Fiefs.Count(t => t.IsCastle) + " kale, " + k.Settlements.Count(s => s.IsVillage) + " köy"
                + (expansionWord.Length > 0 ? "  ·  yayılmacılık " + expansionWord : "");

            bool war = KingdomTies.AtWar(k), ally = KingdomTies.Allied(k), trade = KingdomTies.HasTrade(k);
            float pact = KingdomTies.PactDays(k);
            StanceLine = KingdomTies.StanceText(k);
            IsHostile = war;
            IsFriendly = !war && (ally || pact >= 0 || trade);
            IsNeutral = !IsHostile && !IsFriendly;

            AddLines(k, war, ally, pact, trade);
            AddActions(k, war, ally, pact, trade);
        }

        private void Line(string text) => Lines.Add(new WarLineVM(text));

        private void AddLines(Kingdom k, bool war, bool ally, float pact, bool trade)
        {
            Kingdom mine = KingdomTies.Mine;
            if (war && mine != null)
            {
                int days = (int)mine.GetStanceWith(k).WarStartDate.ElapsedDaysUntilNow;
                float ours = DiplomacyCompat.WarExhaustion(mine, k), theirs = DiplomacyCompat.WarExhaustion(k, mine);
                Line(days + " gündür savaştasınız" + (theirs >= 0 ? "  ·  yorgunluk: onlar %" + (int)Math.Round(theirs) + ", siz %" + (int)Math.Round(Math.Max(0, ours)) : "") + ".");
            }
            else if (war)
                Line("Seninle savaşta.");
            if (ally) Line("Müttefikin: " + KingdomsRules.DaysLeft(KingdomTies.AllianceDays(k)) + ".");
            List<Kingdom> called = ally ? KingdomTies.CalledAgainst(k) : new List<Kingdom>();
            if (called.Count > 0) Line("Senin çağrınla savaşıyor: " + Names(called) + ".");
            if (pact >= 0) Line("Saldırmazlık anlaşması: " + KingdomsRules.DaysLeft(pact) + ". Bu sürede birbirinize savaş açamazsınız.");
            if (trade) Line("Ticaret anlaşması: " + KingdomsRules.DaysLeft(KingdomTies.TradeDays(k)) + ". Kervanlar iki krallıkta da kazanç getirir.");

            List<Kingdom> enemies = Kingdom.All.Where(o => o != k && !o.IsEliminated && FactionManager.IsAtWarAgainstFaction(k, o)).ToList();
            Line(enemies.Count == 0 ? "Kimseyle savaşmıyor." : "Savaştığı: " + Names(enemies) + ".");
            List<Kingdom> allies = k.AlliedKingdoms.Where(o => !o.IsEliminated).ToList();
            if (allies.Count > 0) Line("Müttefikleri: " + Names(allies) + ".");

            if (mine == null) return;
            if (!KingdomTies.IsRuler) Line("Anlaşmaları hükümdarın " + mine.Leader?.Name + " yapar; sen oyunun Krallık ekranından divana sunabilirsin.");
            if (war) return;
            Hero diplomat = Diplomat.Working(SchemeEngine.State);
            Diplomat.ThreatLine threat = diplomat == null ? null : Diplomat.Threats(SchemeEngine.State, diplomat).FirstOrDefault(t => t.Kingdom == k);
            Line(threat != null ? "Diplomatın " + diplomat.Name + ": " + DiplomatRules.ThreatText(threat.Seen) + "."
                : "Tutumunu öğrenmek için Divan'a bir diplomat ata.");
            if (!ally) Line("İttifak: " + (KingdomTies.AllianceRefusal(k) ?? "sıcak bakıyor") + ".");
            if (!trade) Line("Ticaret: " + (KingdomTies.TradeRefusal(k) ?? "sıcak bakıyor") + ".");
        }

        private static string Names(IEnumerable<Kingdom> kingdoms) => string.Join(", ", kingdoms.Select(o => o.Name.ToString()));

        private void Act(string label, string blocked, string title, string text, Action apply, string done, bool danger = false) =>
            Actions.Add(new ActionItemVM(label, blocked, () => MenuUI.Confirm(title, text, () =>
            {
                apply();
                MenuUI.Success(done);
                _changed();
            }, null), danger));

        private void AddActions(Kingdom k, bool war, bool ally, float pact, bool trade)
        {
            string name = k.Name.ToString();
            if (war)
            {
                Actions.Add(new ActionItemVM("Barış Masası'na otur", KingdomTies.Mine == null ? "Bir krallığın yok" : null, () => _openWar(k)));
                Actions.Add(new ActionItemVM("Savaş Raporu", KingdomTies.Mine == null ? "Bir krallığın yok" : null, () => MenuUI.Defer(() =>
                {
                    if (!WarReportScreen.Open(k, TaleWorlds.ScreenSystem.ScreenManager.TopScreen)) MenuUI.Error("Savaş Raporu açılamadı");
                })));
            }

            Hero ruler = k.Leader;
            var messenger = new ActionItemVM("Ulak gönder: " + ruler.Name + "...", ruler == Hero.MainHero ? "Kendine ulak gönderemezsin" : null,
                () => MenuUI.Defer(() => EventCards.ShowAudienceSend(ruler, _changed)));
            // Anlaşmaları hükümdar yapar (Diplomacy'nin divansız düğmeleri gibi); ötekiler bilgi ve ulakla yetinir.
            if (!KingdomTies.IsRuler)
            {
                Actions.Add(messenger);
                return;
            }

            if (!war)
            {
                if (!ally)
                {
                    var (influence, gold) = KingdomTies.AllianceCost(k);
                    string cost = KingdomTies.CostText(influence, gold);
                    Act("İttifak öner  ·  " + cost, KingdomTies.AllianceBlocked(k), name + " ile ittifak kurulsun mu?",
                        "Bedeli: " + cost + ". Müttefikler birbirini savaşa çağırabilir; ittifak oyunun süresi dolunca ya da biri bozunca biter.\n\n"
                        + name + " ittifaka sıcak bakıyor; divanı toplamadan karar veriyorsun.",
                        () => KingdomTies.MakeAlliance(k), name + " ile ittifak kuruldu");
                }
                if (!trade)
                {
                    int cost = KingdomTies.TradeCost();
                    Act("Ticaret anlaşması öner  ·  " + KingdomTies.CostText(cost, 0), KingdomTies.TradeBlocked(k), name + " ile ticaret anlaşması yapılsın mı?",
                        "Bedeli: " + KingdomTies.CostText(cost, 0) + ". Kervanlar iki krallıkta da kazanç getirir; anlaşma birkaç yıl sürer.",
                        () => KingdomTies.MakeTrade(k), name + " ile ticaret anlaşması imzalandı");
                }
                if (pact < 0 && !ally)
                {
                    DiplomacyCompat.Cost cost = KingdomTies.PactCost(k);
                    string text = cost == null ? "bedeli okunamadı" : KingdomTies.CostText(cost.Influence, cost.Gold);
                    Act("Saldırmazlık öner  ·  " + text, KingdomTies.PactBlocked(k), name + " ile saldırmazlık anlaşması yapılsın mı?",
                        "Bedeli: " + text + ". Anlaşma süresince birbirinize savaş açamazsınız (Diplomacy'nin kuralı).",
                        () => KingdomTies.ProposePact(k), name + " ile saldırmazlık anlaşması yapıldı");
                }
            }

            if (ally)
            {
                foreach (Kingdom enemy in KingdomTies.CallTargets(k))
                {
                    Kingdom against = enemy;
                    int influence = KingdomTies.CallInfluence(), gold = KingdomTies.CallGold(k, against);
                    string cost = KingdomTies.CostText(influence, gold);
                    Act("Savaşa çağır: " + against.Name + "  ·  " + cost, KingdomTies.CallBlocked(k, against),
                        name + " " + Tr.Dative(against.Name.ToString()) + " karşı savaşa çağrılsın mı?",
                        "Bedeli: " + cost + " (savaş bedelini oyunun kuralıyla sen ödersin). " + name + " " + against.Name + " ile savaşa girer.",
                        () => KingdomTies.CallToWar(k, against), name + " savaşa katıldı");
                }
                Act("İttifakı boz", KingdomTies.BreakAllianceBlocked(k), name + " ile ittifak bozulsun mu?",
                    "İttifak hemen biter; senin çağrınla savaşa girdiyse savaştan çekilebilir. Hükümdarı bunu unutmaz.",
                    () => KingdomTies.BreakAlliance(k), name + " ile ittifak bozuldu", true);
            }

            if (!war && !ally)
            {
                DiplomacyCompat.Cost cost = KingdomTies.WarCost();
                string text = cost == null ? "bedeli okunamadı" : KingdomTies.CostText(cost.Influence, cost.Gold);
                Act("Savaş ilan et (divansız)  ·  " + text, KingdomTies.WarBlocked(k), name + " krallığına savaş ilan edilsin mi?",
                    "Bedeli: " + text + ". Divanı toplamadan savaş açıyorsun: krallığın hemen " + name + " ile savaşa girer.",
                    () => KingdomTies.DeclareWar(k), name + " krallığına savaş ilan edildi", true);
            }
            Actions.Add(messenger);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Items);
            Clear(Lines);
            Clear(Actions);
            _rulerPortrait?.OnFinalize();
            _banner?.OnFinalize();
        }
    }

    /// <summary>Krallıklar listesinde bir krallık: sancak, ad, hükümdar ve ilişkin, seninle durumu (renkli); tıklanınca seçilir.</summary>
    public sealed class KingdomItemVM : TvhViewModel
    {
        private readonly Kingdom _kingdom;
        private readonly Action<Kingdom> _choose;

        public KingdomItemVM(Kingdom kingdom, bool selected, Action<Kingdom> choose)
        {
            _kingdom = kingdom;
            _choose = choose;
            IsSelected = selected;
            Banner = new BannerImageIdentifierVM(kingdom.Banner, true);
            Name = kingdom.Name.ToString();
            Info = kingdom.Leader.Name + "  ·  seninle " + SchemeFactors.Signed(kingdom.Leader.GetRelation(Hero.MainHero)) + "  ·  "
                + kingdom.Fiefs.Count + " tımar  ·  güç " + ((int)kingdom.CurrentTotalStrength).ToString("N0");
            StanceText = KingdomTies.StanceText(kingdom);
            bool war = KingdomTies.AtWar(kingdom);
            IsHostile = war;
            IsFriendly = !war && (KingdomTies.Allied(kingdom) || KingdomTies.PactDays(kingdom) >= 0 || KingdomTies.HasTrade(kingdom));
            IsNeutral = !IsHostile && !IsFriendly;
        }

        [DataSourceProperty] public ImageIdentifierVM Banner { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string Info { get; }
        [DataSourceProperty] public string StanceText { get; }
        [DataSourceProperty] public bool IsHostile { get; }
        [DataSourceProperty] public bool IsFriendly { get; }
        [DataSourceProperty] public bool IsNeutral { get; }
        [DataSourceProperty] public bool IsSelected { get; }

        public void ExecuteSelect()
        {
            if (!IsSelected) _choose(_kingdom);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Banner?.OnFinalize();
        }
    }
}
