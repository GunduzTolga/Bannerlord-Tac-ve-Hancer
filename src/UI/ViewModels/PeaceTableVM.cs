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
    /// Barış Masası (UI.PeaceTableScreen): solda istediklerin (haraç, tazminat, asker, şehir ve kaleler), sağda verdiklerin (altın, haraç),
    /// altta özet, kabul olasılığı ve Teklif et. Şartlar Intrigue.PeaceTreaties.Terms'te; satırlar yalnızca kendi değerini değiştirir
    /// (liste tıklama sırasında değişmez).
    /// </summary>
    public sealed class PeaceTableVM : TvhViewModel
    {
        private readonly Kingdom _enemy;
        private readonly Action _close;
        private readonly PeaceTreaties.Terms _terms;
        private string _leverageText, _summaryText, _chanceText, _weightText, _blockedText;
        private bool _canPropose;

        public PeaceTableVM(Kingdom enemy, PeaceTreaties.Terms initial, Action close)
        {
            _enemy = enemy;
            _close = close;
            _terms = initial ?? new PeaceTreaties.Terms();
            Demands = new MBBindingList<PeaceTermVM>();
            Offers = new MBBindingList<PeaceTermVM>();
            Build();
            Refresh();
        }

        [DataSourceProperty] public string Title => "Barış Masası";
        [DataSourceProperty] public string Subtitle =>
            _enemy.Name + "  ·  hükümdarı " + _enemy.Leader?.Name + "  ·  kesesi " + SchemeFactors.Money(PeaceTreaties.Treasury(_enemy));
        [DataSourceProperty] public string DemandsTitle => "İstediklerin";
        [DataSourceProperty] public string OffersTitle => "Verdiklerin";
        [DataSourceProperty] public MBBindingList<PeaceTermVM> Demands { get; }
        [DataSourceProperty] public MBBindingList<PeaceTermVM> Offers { get; }
        [DataSourceProperty] public string LeverageText { get => _leverageText; set => Set(ref _leverageText, value, nameof(LeverageText)); }
        [DataSourceProperty] public string SummaryText { get => _summaryText; set => Set(ref _summaryText, value, nameof(SummaryText)); }
        [DataSourceProperty] public string ChanceText { get => _chanceText; set => Set(ref _chanceText, value, nameof(ChanceText)); }
        [DataSourceProperty] public string WeightText { get => _weightText; set => Set(ref _weightText, value, nameof(WeightText)); }
        [DataSourceProperty] public string BlockedText { get => _blockedText; set => Set(ref _blockedText, value, nameof(BlockedText)); }
        [DataSourceProperty] public bool CanPropose { get => _canPropose; set => Set(ref _canPropose, value, nameof(CanPropose)); }
        [DataSourceProperty] public string ProposeText => "Teklif et";
        [DataSourceProperty] public string ResetText => "Şartları sıfırla";
        [DataSourceProperty] public string CloseText => "Kapat";

        private void Build()
        {
            int treasury = PeaceTreaties.Treasury(_enemy);
            List<int> tribute = Enumerable.Range(0, PeaceTreatyRules.TributeLevels.Count).ToList();
            Demands.Add(new PeaceTermVM("Haraç (onlar öder)", tribute, () => _terms.Tribute, v => _terms.Tribute = v, PeaceTreaties.TributeText, Changed));
            Demands.Add(new PeaceTermVM("Savaş tazminatı", WithCurrent(PeaceTreatyRules.GoldSteps(treasury), _terms.Gold), () => _terms.Gold, v => _terms.Gold = v,
                v => v == 0 ? "yok" : SchemeFactors.Money(v), Changed));
            Demands.Add(new PeaceTermVM("Asker (garnizonlarından)", PeaceTreatyRules.TroopSteps(PeaceTreaties.TroopsAvailable(_enemy)), () => _terms.Troops,
                v => _terms.Troops = v, v => v == 0 ? "yok" : v + " asker", Changed));
            foreach (Town town in PeaceTreaties.FiefsOnTable(_enemy))
            {
                string id = town.Settlement.StringId;
                Demands.Add(new PeaceTermVM((town.IsTown ? "Şehir: " : "Kale: ") + town.Name, new List<int> { 0, 1 }, () => _terms.Fiefs.Contains(id) ? 1 : 0,
                    v =>
                    {
                        if (v == 1) _terms.Fiefs.Add(id);
                        else _terms.Fiefs.Remove(id);
                    }, v => v == 1 ? "İSTENİYOR" : "istenmiyor", Changed));
            }
            Offers.Add(new PeaceTermVM("Altın ver", WithCurrent(PeaceTreatyRules.GiftSteps(Hero.MainHero.Gold), _terms.Gift), () => _terms.Gift, v => _terms.Gift = v,
                v => v == 0 ? "yok" : SchemeFactors.Money(v), Changed));
            Offers.Add(new PeaceTermVM("Haraç öde", tribute, () => _terms.Pay, v => _terms.Pay = v, PeaceTreaties.TributeText, Changed));
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
            foreach (PeaceTermVM term in Demands.Concat(Offers)) term.Update();
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
                if (outcome.Accepted)
                {
                    MenuUI.Defer(WarReportScreen.Close);
                    _close();
                }
                else Refresh();
            });
        }

        public void ExecuteReset()
        {
            _terms.Tribute = _terms.Gold = _terms.Troops = _terms.Gift = _terms.Pay = 0;
            _terms.Fiefs.Clear();
            Refresh();
        }

        public void ExecuteClose() => _close();

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Demands);
            Clear(Offers);
        }
    }

    /// <summary>Masadaki bir şart: ad, değer, azalt/arttır düğmeleri, ağırlığı. Değerler basamaklıdır (aç/kapa iki basamak).</summary>
    public sealed class PeaceTermVM : TvhViewModel
    {
        private readonly List<int> _steps;
        private readonly Func<int> _get;
        private readonly Action<int> _set;
        private readonly Func<int, string> _format;
        private readonly Action _changed;
        private string _valueText;
        private bool _canLess, _canMore, _isOn;

        public PeaceTermVM(string label, List<int> steps, Func<int> get, Action<int> set, Func<int, string> format, Action changed)
        {
            Label = label;
            _steps = steps.Count > 0 ? steps : new List<int> { 0 };
            _get = get;
            _set = set;
            _format = format;
            _changed = changed;
            Update();
        }

        [DataSourceProperty] public string Label { get; }
        [DataSourceProperty] public string ValueText { get => _valueText; set => Set(ref _valueText, value, nameof(ValueText)); }
        [DataSourceProperty] public bool CanLess { get => _canLess; set => Set(ref _canLess, value, nameof(CanLess)); }
        [DataSourceProperty] public bool CanMore { get => _canMore; set => Set(ref _canMore, value, nameof(CanMore)); }
        /// <summary>Şart masada (değeri sıfırdan büyük): değer altın renkte.</summary>
        [DataSourceProperty] public bool IsOn { get => _isOn; set => Set(ref _isOn, value, nameof(IsOn)); }
        [DataSourceProperty] public bool IsOff => !IsOn;

        private int Index => Math.Max(0, _steps.IndexOf(_get()));

        public void Update()
        {
            int value = _get();
            ValueText = _format(value);
            CanLess = Index > 0;
            CanMore = Index < _steps.Count - 1;
            IsOn = value > 0;
            OnPropertyChanged(nameof(IsOff));
        }

        public void ExecuteLess() => Step(-1);

        public void ExecuteMore() => Step(1);

        private void Step(int delta)
        {
            int next = SchemeMath.Clamp(Index + delta, 0, _steps.Count - 1);
            _set(_steps[next]);
            _changed();
        }
    }
}
