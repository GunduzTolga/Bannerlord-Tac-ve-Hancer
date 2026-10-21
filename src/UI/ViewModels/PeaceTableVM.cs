using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Barış Masası (Savaş ve barış > Savaşlar'da seçili savaş): solda istediklerin, sağda verdiklerin, özet, kabul olasılığı ve Teklif et.
    /// Şartlar Intrigue.PeaceTreaties.Terms'te; satırlar yalnızca kendi değerini değiştirir (liste tıklama sırasında değişmez).
    /// </summary>
    public sealed class PeaceTableVM : TvhViewModel
    {
        private readonly Kingdom _enemy;
        private readonly Action _changed;
        private readonly PeaceTreaties.Terms _terms;
        private string _leverageText, _summaryText, _chanceText, _weightText, _blockedText, _noteText = "";
        private bool _canPropose;

        /// <param name="changed">Barış imzalandı ya da teklif reddedildi: Entrika ekranı yenilensin.</param>
        public PeaceTableVM(Kingdom enemy, PeaceTreaties.Terms initial, Action changed)
        {
            _enemy = enemy;
            _changed = changed;
            _terms = initial ?? new PeaceTreaties.Terms();
            Demands = new MBBindingList<StepRowVM>();
            Offers = new MBBindingList<StepRowVM>();
            Build();
            Refresh();
        }

        public Kingdom Enemy => _enemy;

        [DataSourceProperty] public string Title => "Barış Masası: " + _enemy.Name;
        [DataSourceProperty] public string Subtitle =>
            "Hükümdarı " + _enemy.Leader?.Name + "  ·  kesesi " + SchemeFactors.Money(PeaceTreaties.Treasury(_enemy));
        [DataSourceProperty] public string DemandsTitle => "İstediklerin";
        [DataSourceProperty] public string OffersTitle => "Verdiklerin";
        [DataSourceProperty] public MBBindingList<StepRowVM> Demands { get; }
        [DataSourceProperty] public MBBindingList<StepRowVM> Offers { get; }
        [DataSourceProperty] public string LeverageText { get => _leverageText; set => Set(ref _leverageText, value, nameof(LeverageText)); }
        [DataSourceProperty] public string SummaryText { get => _summaryText; set => Set(ref _summaryText, value, nameof(SummaryText)); }
        [DataSourceProperty] public string ChanceText { get => _chanceText; set => Set(ref _chanceText, value, nameof(ChanceText)); }
        [DataSourceProperty] public string WeightText { get => _weightText; set => Set(ref _weightText, value, nameof(WeightText)); }
        [DataSourceProperty] public string BlockedText { get => _blockedText; set => Set(ref _blockedText, value, nameof(BlockedText)); }
        [DataSourceProperty] public bool CanPropose { get => _canPropose; set => Set(ref _canPropose, value, nameof(CanPropose)); }
        [DataSourceProperty] public string NoteText { get => _noteText; set => Set(ref _noteText, value, nameof(NoteText)); }
        [DataSourceProperty] public string ProposeText => "Teklif et";
        [DataSourceProperty] public string ResetText => "Şartları sıfırla";

        private static readonly List<int> Toggle = new List<int> { 0, 1 };

        /// <summary>Şartlar bölümlere ayrılır: para, asker ve esirler, toprak, anlaşmalar; sağda verdiklerin.</summary>
        private void Build()
        {
            IntrigueState state = SchemeEngine.State;
            int treasury = PeaceTreaties.Treasury(_enemy);
            List<int> tribute = Enumerable.Range(0, PeaceTreatyRules.TributeLevels.Count).ToList();
            string Money(int v) => v == 0 ? "yok" : SchemeFactors.Money(v);
            string YesNo(int v) => v == 1 ? "İSTENİYOR" : "istenmiyor";

            Demands.Add(StepRowVM.Header("PARA"));
            Demands.Add(new StepRowVM("Haraç (onlar öder)", tribute, () => _terms.Tribute, v => _terms.Tribute = v, PeaceTreaties.TributeText, Changed));
            Demands.Add(new StepRowVM("Savaş tazminatı", WithCurrent(PeaceTreatyRules.GoldSteps(treasury), _terms.Gold), () => _terms.Gold, v => _terms.Gold = v,
                Money, Changed));

            Demands.Add(StepRowVM.Header("ASKER VE ESİRLER"));
            Demands.Add(new StepRowVM("Asker (garnizonlarından)", PeaceTreatyRules.TroopSteps(PeaceTreaties.TroopsAvailable(_enemy)), () => _terms.Troops,
                v => _terms.Troops = v, v => v == 0 ? "yok" : v + " asker", Changed));
            int ours = PeaceTreaties.OurLordsHeld(_enemy).Count;
            Demands.Add(new StepRowVM("Esir lordlarımızı bıraksınlar (" + ours + ")", ours > 0 ? Toggle : new List<int> { 0 },
                () => _terms.ReleaseOurs ? 1 : 0, v => _terms.ReleaseOurs = v == 1, YesNo, Changed));

            Demands.Add(StepRowVM.Header("TOPRAK (sınırına en yakınlar)"));
            foreach (Town town in PeaceTreaties.FiefsOnTable(_enemy))
            {
                string id = town.Settlement.StringId;
                Demands.Add(new StepRowVM((town.IsTown ? "Şehir: " : "Kale: ") + town.Name, Toggle, () => _terms.Fiefs.Contains(id) ? 1 : 0,
                    v =>
                    {
                        if (v == 1) _terms.Fiefs.Add(id);
                        else _terms.Fiefs.Remove(id);
                    }, YesNo, Changed));
            }

            Demands.Add(StepRowVM.Header("ANLAŞMALAR"));
            Hero hostage = PeaceTreaties.HostageCandidate(state, _enemy);
            Demands.Add(new StepRowVM(hostage != null ? "Rehine: " + hostage.Name : "Rehine (verecek kimse yok)", hostage != null ? Toggle : new List<int> { 0 },
                () => _terms.Hostage ? 1 : 0, v => _terms.Hostage = v == 1, YesNo, Changed));
            if (Bundle.DiplomacyCompat.IsAvailable)
                Demands.Add(new StepRowVM("Saldırmazlık anlaşması", PeaceTreatyRules.PactLevels.ToList(), () => _terms.PactDays, v => _terms.PactDays = v,
                    v => v == 0 ? "yok" : v + " gün", Changed));
            Demands.Add(new StepRowVM("Ticaret anlaşması", Toggle, () => _terms.Trade ? 1 : 0, v => _terms.Trade = v == 1, YesNo, Changed));
            Demands.Add(new StepRowVM("İttifak", Toggle, () => _terms.Alliance ? 1 : 0, v => _terms.Alliance = v == 1, YesNo, Changed));

            Offers.Add(new StepRowVM("Altın ver", WithCurrent(PeaceTreatyRules.GiftSteps(Hero.MainHero.Gold), _terms.Gift), () => _terms.Gift, v => _terms.Gift = v,
                Money, Changed));
            Offers.Add(new StepRowVM("Haraç öde", tribute, () => _terms.Pay, v => _terms.Pay = v, PeaceTreaties.TributeText, Changed));
            int theirs = PeaceTreaties.TheirLordsHeld(_enemy).Count;
            Offers.Add(new StepRowVM("Esirlerini bırak (" + theirs + ")", theirs > 0 ? Toggle : new List<int> { 0 },
                () => _terms.ReleaseTheirs ? 1 : 0, v => _terms.ReleaseTheirs = v == 1, v => v == 1 ? "BIRAKILIYOR" : "hayır", Changed));
        }

        /// <summary>Elçinin getirdiği tutar basamaklarda yoksa araya eklenir.</summary>
        private static List<int> WithCurrent(List<int> steps, int current)
        {
            if (current > 0 && !steps.Contains(current)) steps.Add(current);
            steps.Sort();
            return steps;
        }

        private void Changed() => Refresh();

        public void Refresh()
        {
            IntrigueState state = SchemeEngine.State;
            foreach (StepRowVM term in Demands.Concat(Offers)) term.Update();
            NoteText = _terms.Trade || _terms.Alliance
                ? "Ticaret ve ittifak barıştan sonra oyunun kendi kuralıyla kurulur; kurulamazsa sonuçta nedeni yazar." : "";
            string blocked = PeaceTreaties.Blocked(state, _enemy);
            PeaceTreaties.Leverage leverage = Clan.PlayerClan.Kingdom != null && _enemy.Leader != null ? SafeLeverage(state) : null;
            LeverageText = leverage == null ? "" : LeverageLine(leverage);
            SummaryText = PeaceTreaties.Describe(_enemy, _terms);
            int weight = PeaceTreaties.Weight(_enemy, _terms);
            int chance = leverage == null ? 0 : PeaceTreatyRules.Chance(leverage.Willingness, weight);
            ChanceText = leverage == null ? "" : "Kabul olasılığı: %" + chance;
            WeightText = "Şartların ağırlığı " + weight + (leverage != null ? "  ·  pazarlık gücün " + leverage.Willingness : "");
            blocked = blocked ?? PeaceTreaties.TermsBlocked(_enemy, _terms);
            BlockedText = blocked ?? "";
            CanPropose = blocked == null;
        }

        private PeaceTreaties.Leverage SafeLeverage(IntrigueState state)
        {
            try
            {
                return PeaceTreaties.LeverageOf(state, _enemy);
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] peace leverage failed: " + ex);
                return null;
            }
        }

        private string LeverageLine(PeaceTreaties.Leverage l)
        {
            var parts = new List<string>();
            if (l.TheirExhaustion >= 0) parts.Add("onların yorgunluğu %" + (int)Math.Round(l.TheirExhaustion) + ", seninki %" + (int)Math.Round(Math.Max(0, l.OurExhaustion)));
            parts.Add("güç oranı " + l.Ratio.ToString("0.0", Tr.Culture));
            if (l.OtherWars > 0) parts.Add(l.OtherWars + " başka cephede savaşıyorlar");
            parts.Add(l.GameLeans ? "barışa yatkınlar" : "barışa yanaşmıyorlar");
            if (l.Asked) parts.Add("elçi gönderdiler (+" + PeaceTreatyRules.AskedBonus + ")");
            if (l.Diplomat > 0) parts.Add("diplomatın (+" + l.Diplomat + ")");
            return "Pazarlık gücün " + l.Willingness + ": " + string.Join(", ", parts) + ".";
        }

        public void ExecutePropose()
        {
            if (!CanPropose) return;
            Run("Barış teklifi", () =>
            {
                PeaceTreaties.Outcome outcome = PeaceTreaties.Propose(SchemeEngine.State, _enemy, _terms);
                var ok = new EventCards.CardOption { Label = "Tamam", Apply = () => { } };
                MenuUI.Defer(() => EventCards.Show(outcome.Accepted ? "BARIŞ" : "TEKLİF REDDEDİLDİ", outcome.Accepted ? "Barış imzalandı" : _enemy.Name + " reddetti",
                    outcome.Text, outcome.Accepted ? CardImages.Treaty : CardImages.Envoy, _enemy.Leader, new List<EventCards.CardOption> { ok }, ok, null));
                if (outcome.Accepted) MenuUI.Defer(WarReportScreen.Close);
                _changed();
            });
        }

        public void ExecuteReset()
        {
            _terms.Reset();
            Refresh();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Demands);
            Clear(Offers);
        }
    }
}
