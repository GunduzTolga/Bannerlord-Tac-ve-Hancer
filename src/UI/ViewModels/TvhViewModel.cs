using System;
using System.Collections.Generic;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>Ekran ViewModel'lerinin ortak tabanı: değişince arayüze haber veren alan yazıcısı ve küçük yardımcılar.</summary>
    public abstract class TvhViewModel : ViewModel
    {
        protected bool Set<T>(ref T field, T value, string propertyName)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            switch (value)
            {
                case bool b: OnPropertyChangedWithValue(b, propertyName); break;
                case int i: OnPropertyChangedWithValue(i, propertyName); break;
                case float f: OnPropertyChangedWithValue(f, propertyName); break;
                default: OnPropertyChangedWithValue<object>(value, propertyName); break;
            }
            return true;
        }

        /// <summary>
        /// Kahramanın portresi (oyunun kendi portre çizicisi). Kahraman yoksa boş portre: arayüzde bağlı alan hiç null olmasın.
        /// </summary>
        public static ImageIdentifierVM PortraitOf(Hero hero) =>
            new CharacterImageIdentifierVM(hero?.CharacterObject == null
                ? CharacterCode.CreateEmpty()
                : CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));

        /// <summary>Listedeki öğeleri (portre dokuları dahil) bırakır ve listeyi boşaltır.</summary>
        public static void Clear<T>(MBBindingList<T> list) where T : ViewModel
        {
            foreach (T item in list) item.OnFinalize();
            list.Clear();
        }

        /// <summary>Arayüz komutları buradan geçer: istisna oyunu çökertmesin, kırmızı mesaj olarak görünsün.</summary>
        protected static void Run(string what, Action action) => MenuUI.SafeRun(what, action);
    }

    /// <summary>Sekme ya da filtre düğmesi.</summary>
    public sealed class TabVM : TvhViewModel
    {
        private readonly Action<TabVM> _onSelect;
        private string _name;
        private bool _isSelected, _isVisible = true;

        public TabVM(int index, string name, Action<TabVM> onSelect)
        {
            Index = index;
            _name = name;
            _onSelect = onSelect;
        }

        public int Index { get; }

        [DataSourceProperty]
        public string Name { get => _name; set => Set(ref _name, value, nameof(Name)); }

        [DataSourceProperty]
        public bool IsSelected { get => _isSelected; set => Set(ref _isSelected, value, nameof(IsSelected)); }

        /// <summary>Alt sekme şeridinde yalnızca seçili bölümün sekmeleri görünür.</summary>
        [DataSourceProperty]
        public bool IsVisible { get => _isVisible; set => Set(ref _isVisible, value, nameof(IsVisible)); }

        public void ExecuteSelect() => _onSelect(this);
    }

    /// <summary>Düğme: etiket, seçilemiyorsa nedeni (ipucu) ve eylem.</summary>
    public sealed class ActionItemVM : TvhViewModel
    {
        private readonly Action _action;

        public ActionItemVM(string label, string blockedReason, Action action, bool isDanger = false)
        {
            Label = label;
            Hint = blockedReason ?? "";
            IsEnabled = blockedReason == null;
            IsDanger = isDanger;
            _action = action;
        }

        [DataSourceProperty] public string Label { get; }
        [DataSourceProperty] public string Hint { get; }
        [DataSourceProperty] public bool HasHint => Hint.Length > 0;
        [DataSourceProperty] public bool IsEnabled { get; }
        [DataSourceProperty] public bool IsDanger { get; }
        [DataSourceProperty] public bool IsNormal => !IsDanger;

        public void ExecuteAction()
        {
            if (IsEnabled) Run(Label, _action);
        }
    }

    /// <summary>Kronik satırı.</summary>
    public sealed class LogItemVM : TvhViewModel
    {
        public LogItemVM(string day, string text)
        {
            DayText = day;
            Text = text;
        }

        [DataSourceProperty] public string DayText { get; }
        [DataSourceProperty] public string Text { get; }
    }
}
