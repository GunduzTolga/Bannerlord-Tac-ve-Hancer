using System;
using System.Collections.Generic;
using CheatMenu.Intrigue.Core;
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

        public void ExecuteSelect() => Run(Name, () => _onSelect(this));
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

    /// <summary>
    /// Basamaklı satır (TvhStepRow): ad, azalt, değer, arttır; ya da düğmesiz bölüm başlığı. Barış Masası'nın şartları ve Krallık'taki
    /// bağış miktarı gibi. Değerler basamaklıdır (aç/kapa iki basamak); değişince yalnızca kendi alanları güncellenir.
    /// </summary>
    public sealed class StepRowVM : TvhViewModel
    {
        private readonly List<int> _steps;
        private readonly Func<int> _get;
        private readonly Action<int> _set;
        private readonly Func<int, string> _format;
        private readonly Action _changed;
        private string _valueText;
        private bool _canLess, _canMore, _isOn;

        /// <summary>Bölüm başlığı satırı (düğmesiz).</summary>
        public static StepRowVM Header(string label) => new StepRowVM(label);

        private StepRowVM(string label)
        {
            Label = label;
            IsHeader = true;
            _steps = new List<int> { 0 };
            _get = () => 0;
            _set = v => { };
            _format = v => "";
            _changed = () => { };
        }

        public StepRowVM(string label, List<int> steps, Func<int> get, Action<int> set, Func<int, string> format, Action changed)
        {
            Label = label;
            _steps = new List<int>(steps.Count > 0 ? steps : new List<int> { 0 });
            _get = get;
            _set = set;
            _format = format;
            _changed = changed;
            Update();
        }

        [DataSourceProperty] public string Label { get; }
        [DataSourceProperty] public bool IsHeader { get; }
        [DataSourceProperty] public bool IsTerm => !IsHeader;
        [DataSourceProperty] public float Height => IsHeader ? 34 : 46;
        [DataSourceProperty] public string ValueText { get => _valueText; set => Set(ref _valueText, value, nameof(ValueText)); }
        [DataSourceProperty] public bool CanLess { get => _canLess; set => Set(ref _canLess, value, nameof(CanLess)); }
        [DataSourceProperty] public bool CanMore { get => _canMore; set => Set(ref _canMore, value, nameof(CanMore)); }
        /// <summary>Şart masada (değeri sıfırdan büyük): değer altın renkte.</summary>
        [DataSourceProperty] public bool IsOn { get => _isOn; set => Set(ref _isOn, value, nameof(IsOn)); }
        [DataSourceProperty] public bool IsOff => !IsOn;

        private int Index => Math.Max(0, _steps.IndexOf(_get()));

        public void Update()
        {
            if (IsHeader) return;
            int value = _get();
            ValueText = _format(value);
            CanLess = Index > 0;
            CanMore = Index < _steps.Count - 1;
            IsOn = value > 0;
            OnPropertyChanged(nameof(IsOff));
        }

        /// <summary>Basamaklar değişti (kese değişince bağış miktarları gibi).</summary>
        public void SetSteps(List<int> steps)
        {
            _steps.Clear();
            _steps.AddRange(steps.Count > 0 ? steps : new List<int> { 0 });
            Update();
        }

        public void ExecuteLess() => Step(-1);

        public void ExecuteMore() => Step(1);

        private void Step(int delta) => Run(Label, () =>
        {
            int next = SchemeMath.Clamp(Index + delta, 0, _steps.Count - 1);
            _set(_steps[next]);
            _changed();
        });
    }
}
