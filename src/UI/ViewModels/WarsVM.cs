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
    /// Savaş ve barış > Savaşlar paneli (Entrika ekranı): solda savaştığın krallıklar (sancak, yorgunluk, güç, şartsız barış olasılığı,
    /// Savaş Raporu düğmesi), sağda seçilinin Barış Masası. Seçim bir sonraki karede uygulanır (tıklanan liste o sırada değişmesin).
    /// </summary>
    public sealed class WarsVM : TvhViewModel
    {
        private readonly Action _changed;
        private Kingdom _selected;
        private PeaceTreaties.Terms _pendingTerms;
        private PeaceTableVM _table;
        private bool _hasTable, _hasNoWars;
        private string _emptyText = "";

        public WarsVM(Action changed)
        {
            _changed = changed;
            Items = new MBBindingList<WarItemVM>();
        }

        [DataSourceProperty] public string WarsTitle => "Savaşların";
        [DataSourceProperty] public MBBindingList<WarItemVM> Items { get; }
        [DataSourceProperty] public PeaceTableVM Table { get => _table; set => Set(ref _table, value, nameof(Table)); }
        [DataSourceProperty] public bool HasTable { get => _hasTable; set => Set(ref _hasTable, value, nameof(HasTable)); }
        [DataSourceProperty] public bool HasNoWars { get => _hasNoWars; set => Set(ref _hasNoWars, value, nameof(HasNoWars)); }
        [DataSourceProperty] public string EmptyText { get => _emptyText; set => Set(ref _emptyText, value, nameof(EmptyText)); }

        /// <summary>Bu savaş seçili açılsın (terms: masaya konacak şartlar, ör. elçinin teklifi).</summary>
        public void Select(Kingdom enemy, PeaceTreaties.Terms terms)
        {
            _selected = enemy;
            _pendingTerms = terms;
        }

        public void Refresh()
        {
            IntrigueState state = SchemeEngine.State;
            Kingdom mine = Clan.PlayerClan.Kingdom;
            List<Kingdom> wars = mine == null ? new List<Kingdom>() : Marshal.Enemies();
            if (_selected == null || !wars.Contains(_selected))
            {
                _selected = wars.FirstOrDefault();
                _pendingTerms = null;
            }
            Clear(Items);
            foreach (Kingdom enemy in wars) Items.Add(new WarItemVM(state, enemy, enemy == _selected, Choose));
            HasNoWars = wars.Count == 0;
            EmptyText = mine == null
                ? "Bir krallıkta değilsin: barış antlaşmasını krallıklar yapar."
                : "Kimseyle savaşta değilsin.\n\nSavaş açılınca burada her savaşın durumu ve Barış Masası görünür.";

            if (_selected == null)
            {
                Table?.OnFinalize();
                Table = null;
                HasTable = false;
                return;
            }
            if (Table == null || Table.Enemy != _selected || _pendingTerms != null)
            {
                Table?.OnFinalize();
                Table = new PeaceTableVM(_selected, _pendingTerms, _changed);
                _pendingTerms = null;
            }
            else Table.Refresh();
            HasTable = true;
        }

        private void Choose(Kingdom enemy)
        {
            _selected = enemy;
            _changed();
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Items);
            Table?.OnFinalize();
        }
    }

    /// <summary>Bir savaş: sancak, durum, şartsız barış olasılığı; seç ve rapor.</summary>
    public sealed class WarItemVM : TvhViewModel
    {
        private readonly Kingdom _enemy;
        private readonly Action<Kingdom> _choose;

        public WarItemVM(IntrigueState state, Kingdom enemy, bool selected, Action<Kingdom> choose)
        {
            _enemy = enemy;
            _choose = choose;
            IsSelected = selected;
            Banner = new BannerImageIdentifierVM(enemy.Banner, true);
            Name = enemy.Name.ToString();
            int days = (int)Clan.PlayerClan.Kingdom.GetStanceWith(enemy).WarStartDate.ElapsedDaysUntilNow;
            PeaceTreaties.Leverage leverage = PeaceTreaties.LeverageOf(state, enemy);
            StatsText = days + " gündür  ·  güç oranı " + leverage.Ratio.ToString("0.0", Tr.Culture)
                + (leverage.TheirExhaustion >= 0 ? "  ·  yorgunluk %" + (int)Math.Round(leverage.TheirExhaustion) + " / sen %" + (int)Math.Round(Math.Max(0, leverage.OurExhaustion)) : "");
            string blocked = PeaceTreaties.Blocked(state, enemy);
            ChanceText = blocked ?? "Şartsız barış: %" + PeaceTreatyRules.Chance(leverage.Willingness, 0);
            IsBlocked = blocked != null;
        }

        [DataSourceProperty] public ImageIdentifierVM Banner { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string StatsText { get; }
        [DataSourceProperty] public string ChanceText { get; }
        [DataSourceProperty] public bool IsBlocked { get; }
        [DataSourceProperty] public bool IsOpen => !IsBlocked;
        [DataSourceProperty] public bool IsSelected { get; }
        [DataSourceProperty] public string ReportText => "Savaş Raporu";

        public void ExecuteSelect()
        {
            if (!IsSelected) _choose(_enemy);
        }

        public void ExecuteReport() => MenuUI.Defer(() =>
        {
            if (!WarReportScreen.Open(_enemy, TaleWorlds.ScreenSystem.ScreenManager.TopScreen)) MenuUI.Error("Savaş Raporu açılamadı");
        });

        public override void OnFinalize()
        {
            base.OnFinalize();
            Banner?.OnFinalize();
        }
    }
}
