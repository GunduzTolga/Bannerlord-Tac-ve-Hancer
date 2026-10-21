using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Savaş Raporu (UI.WarReportScreen). Üst kısım Diplomacy'nin kendi ViewModel'i (Report: sancaklar, savaş yorgunluğu, ilişki, tarih);
    /// altında iki sütun bölüm (Intrigue.WarReport): solda gidişat, kuvvetler, yorgunluk dökümü; sağda mareşal, barış, esirler, günlük.
    /// </summary>
    public sealed class WarReportVM : TvhViewModel
    {
        private readonly Kingdom _enemy;
        private readonly Action _close;
        private bool _built;

        public WarReportVM(Kingdom enemy, Action close)
        {
            _enemy = enemy;
            _close = close;
            Report = Bundle.DiplomacyCompat.CreateWarDetail(enemy, close) ?? throw new InvalidOperationException("Diplomacy'nin raporu kurulamadı");
            LeftSections = new MBBindingList<WarSectionVM>();
            RightSections = new MBBindingList<WarSectionVM>();
            Refresh();
        }

        /// <summary>Diplomacy.ViewModel.DetailWarVM (prefab'da yalnızca onun kendi alanlarına bağlanır).</summary>
        [DataSourceProperty] public ViewModel Report { get; }
        [DataSourceProperty] public MBBindingList<WarSectionVM> LeftSections { get; }
        [DataSourceProperty] public MBBindingList<WarSectionVM> RightSections { get; }
        [DataSourceProperty] public string CloseText => "Kapat";
        [DataSourceProperty] public string PeaceText => "Barış Masası...";

        public void Refresh()
        {
            Kingdom ours = Clan.PlayerClan.Kingdom;
            if (ours == null) return;
            // Diplomacy'nin raporu kurulurken kendini doldurur; sonraki yenilemelerde (popup kapanınca) yeniden okunur.
            if (_built) Report.RefreshValues();
            _built = true;
            Color ourColor = ColorOf(ours), theirColor = ColorOf(_enemy);
            Fill(LeftSections, WarReport.Left(ours, _enemy, Report), ours, ourColor, theirColor);
            Fill(RightSections, WarReport.Right(ours, _enemy, Report), ours, ourColor, theirColor);
        }

        private void Fill(MBBindingList<WarSectionVM> list, List<WarReport.Section> sections, Kingdom ours, Color ourColor, Color theirColor)
        {
            Clear(list);
            foreach (WarReport.Section section in sections)
                list.Add(new WarSectionVM(section, ours.Name.ToString(), _enemy.Name.ToString(), ourColor, theirColor));
        }

        private static Color ColorOf(Kingdom kingdom) => Color.FromUint(WarReportRules.Readable(kingdom.Color, kingdom.Color2));

        public void ExecuteClose() => _close();

        /// <summary>Barış Masası raporun üstünde açılır; barış imzalanırsa rapor da kapanır.</summary>
        public void ExecutePeace() => Menu.MenuUI.Defer(() =>
        {
            if (!PeaceTableScreen.Open(_enemy, null, TaleWorlds.ScreenSystem.ScreenManager.TopScreen)) Menu.MenuUI.Error("Barış Masası açılamadı");
        });

        public override void OnFinalize()
        {
            base.OnFinalize();
            Report.OnFinalize();
            Clear(LeftSections);
            Clear(RightSections);
        }
    }

    /// <summary>Raporun bir bölümü: başlık, iki tarafın adı, karşılaştırma satırları, düz satırlar.</summary>
    public sealed class WarSectionVM : TvhViewModel
    {
        public WarSectionVM(WarReport.Section section, string ours, string theirs, Color ourColor, Color theirColor)
        {
            Title = section.Title;
            HasSides = section.Sides && section.Rows.Count > 0;
            OursName = ours;
            TheirsName = theirs;
            Rows = new MBBindingList<WarRowVM>();
            foreach (WarReport.Row row in section.Rows) Rows.Add(new WarRowVM(row, ourColor, theirColor));
            Lines = new MBBindingList<WarLineVM>();
            foreach (string line in section.Lines) Lines.Add(new WarLineVM(line));
        }

        [DataSourceProperty] public string Title { get; }
        [DataSourceProperty] public bool HasSides { get; }
        [DataSourceProperty] public string OursName { get; }
        [DataSourceProperty] public string TheirsName { get; }
        [DataSourceProperty] public MBBindingList<WarRowVM> Rows { get; }
        [DataSourceProperty] public MBBindingList<WarLineVM> Lines { get; }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Rows);
            Clear(Lines);
        }
    }

    /// <summary>Karşılaştırma satırı: solda biz, ortada ne, sağda onlar; altında iki renkli çubuk; değerlerin üstünde ipucu.</summary>
    public sealed class WarRowVM : TvhViewModel
    {
        public WarRowVM(WarReport.Row row, Color ourColor, Color theirColor)
        {
            Label = row.Label ?? "";
            Ours = row.Ours ?? "";
            Theirs = row.Theirs ?? "";
            HasBar = row.Bar;
            (OurWidth, TheirWidth) = WarReportRules.Bar(row.OurValue, row.TheirValue);
            OurColor = ourColor;
            TheirColor = theirColor;
            OursHint = Hint(row.OursHint, row.OursTip, Label);
            TheirsHint = Hint(row.TheirsHint, row.TheirsTip, Label);
            HasOursHint = OursHint != null;
            HasTheirsHint = TheirsHint != null;
            OursHint = OursHint ?? new BasicTooltipViewModel();
            TheirsHint = TheirsHint ?? new BasicTooltipViewModel();
        }

        /// <summary>Diplomacy'nin hazır ipucu ya da satırlardan bir ipucu; ikisi de yoksa null.</summary>
        private static BasicTooltipViewModel Hint(object ready, List<(string Name, string Value)> lines, string title)
        {
            if (ready is BasicTooltipViewModel hint) return hint;
            if (lines == null || lines.Count == 0) return null;
            var properties = new List<TooltipProperty> { new TooltipProperty(title, "", 0, false, TooltipProperty.TooltipPropertyFlags.Title) };
            properties.AddRange(lines.Select(l => new TooltipProperty(l.Name, l.Value, 0)));
            return new BasicTooltipViewModel(() => properties);
        }

        [DataSourceProperty] public string Label { get; }
        [DataSourceProperty] public string Ours { get; }
        [DataSourceProperty] public string Theirs { get; }
        [DataSourceProperty] public bool HasBar { get; }
        [DataSourceProperty] public float OurWidth { get; }
        [DataSourceProperty] public float TheirWidth { get; }
        [DataSourceProperty] public Color OurColor { get; }
        [DataSourceProperty] public Color TheirColor { get; }
        [DataSourceProperty] public BasicTooltipViewModel OursHint { get; }
        [DataSourceProperty] public BasicTooltipViewModel TheirsHint { get; }
        [DataSourceProperty] public bool HasOursHint { get; }
        [DataSourceProperty] public bool HasTheirsHint { get; }
    }

    /// <summary>Bölümdeki düz satır (mareşalin uyarısı, barış durumu, esirler, günlük).</summary>
    public sealed class WarLineVM : TvhViewModel
    {
        public WarLineVM(string text) => Text = text;

        [DataSourceProperty] public string Text { get; }
    }
}
