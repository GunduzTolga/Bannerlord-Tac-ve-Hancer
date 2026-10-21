using System;
using System.Collections.Generic;
using CheatMenu.Bundle;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// Saray > Ticaret paneli (tasarım: docs/superpowers/specs/2026-10-21-ticaret-sekmesi.md): AutoTrader'ın ayarları, onun ayrı penceresi
    /// yerine (Alt+A buraya açılır: BundleShim). Solda fiyat, yük ve elde tutulacaklar; sağda mal türlerine göre al/sat ve özel ayarlar;
    /// üstte hazır ayar setleri. Her değişiklik hemen AutoTrader'ın kendi dosyasına yazılır, ekran bir sonraki karede yeniden kurulur.
    /// </summary>
    public sealed class TradeVM : TvhViewModel
    {
        private static readonly List<int> Toggle = new List<int> { 0, 1 };
        private readonly Action _changed;
        private bool _isAvailable;

        public TradeVM(Action changed)
        {
            _changed = changed;
            Settings = new MBBindingList<StepRowVM>();
            Goods = new MBBindingList<StepRowVM>();
        }

        [DataSourceProperty] public string Title => "AutoTrader: kendiliğinden ticaret";
        [DataSourceProperty] public string InfoText =>
            "Kasaba ve köy menüsündeki \"Otomatik ticaret yap\" seçeneği (kervanlarla konuşurken \"Mallarına bakmak istiyorum\") bu ayarlarla alıp satar. "
            + "Kilitlediğin eşyalara dokunmaz. Değişiklikler hemen kaydedilir.";
        [DataSourceProperty] public string PresetsTitle => "Hazır ayarlar:";
        [DataSourceProperty] public string Preset0Text => TradeSettingsRules.PresetNames[0];
        [DataSourceProperty] public string Preset1Text => TradeSettingsRules.PresetNames[1];
        [DataSourceProperty] public string Preset2Text => TradeSettingsRules.PresetNames[2];
        [DataSourceProperty] public string Preset3Text => TradeSettingsRules.PresetNames[3];
        [DataSourceProperty] public string EmptyText => "AutoTrader paketli değil ya da ayarlarına ulaşılamadı.";
        [DataSourceProperty] public MBBindingList<StepRowVM> Settings { get; }
        [DataSourceProperty] public MBBindingList<StepRowVM> Goods { get; }
        [DataSourceProperty] public bool IsAvailable { get => _isAvailable; set { if (Set(ref _isAvailable, value, nameof(IsAvailable))) OnPropertyChanged(nameof(IsUnavailable)); } }
        [DataSourceProperty] public bool IsUnavailable => !_isAvailable;

        public void Refresh()
        {
            Clear(Settings);
            Clear(Goods);
            IsAvailable = AutoTraderCompat.IsAvailable;
            if (!IsAvailable) return;

            bool simple = Get("SimpleTradingAI") == 1, weighted = Get("UseWeightedValue") == 1;
            Settings.Add(StepRowVM.Header("FİYAT"));
            Settings.Add(Row("Fiyat yöntemi", Toggle, "SimpleTradingAI", v => v == 1 ? "yeşil/kırmızı" : "ortalama fiyat"));
            if (!simple)
            {
                Settings.Add(Row("Ortalamayı mesafeye göre ağırlıklandır", Toggle, "UseWeightedValue", OnOff));
                if (!weighted) Settings.Add(Row("Ortalama fiyat arama yarıçapı", TradeSettingsRules.Radius(), "SearchRadiusValue", TradeSettingsRules.RadiusText));
            }
            Settings.Add(Row("Alış eşiği (ortalama fiyatın)", TradeSettingsRules.Percent(200), "BuyThresholdValue", Percent));
            Settings.Add(Row("Satış eşiği (ortalama fiyatın)", TradeSettingsRules.Percent(200), "SellThresholdValue", Percent));

            Settings.Add(StepRowVM.Header("YÜK VE KESE"));
            Settings.Add(Row("Kullanılacak envanter", TradeSettingsRules.Percent(150), "UseInventorySpaceValue", Percent, v =>
            {
                if (Get("MaxCapacityValue") > v) AutoTraderCompat.Set("MaxCapacityValue", v);
            }));
            Settings.Add(Row("Tek bir mal en çok (envanterin)", TradeSettingsRules.Percent(Get("UseInventorySpaceValue")), "MaxCapacityValue", Percent));
            if (AutoTraderCompat.FleetAvailable)
                Settings.Add(Row("Envanter yerine filo kapasitesi", Toggle, "UseMaxFleetCapacityValue", OnOff));
            Settings.Add(Row("Kesede bırakılacak asker maaşı", TradeSettingsRules.WageDays(), "KeepWagesValue", v => v == 0 ? "yok" : v + " günlük"));

            Settings.Add(StepRowVM.Header("ELDE TUTULACAKLAR"));
            Settings.Add(Row("Tahıl en az", TradeSettingsRules.GrainMin(), "KeepGrainsMinValue", v => v.ToString(), v =>
            {
                if (Get("KeepGrainsMaxValue") < v) AutoTraderCompat.Set("KeepGrainsMaxValue", v);
            }));
            Settings.Add(Row("Tahıl en çok", TradeSettingsRules.GrainMax(Get("KeepGrainsMinValue")), "KeepGrainsMaxValue", TradeSettingsRules.GrainMaxText));
            Settings.Add(Row("Öteki yiyecekler en az (her biri)", TradeSettingsRules.ConsumableMin(), "KeepConsumablesMinValue", v => v.ToString(), v =>
            {
                if (Get("KeepConsumablesMaxValue") < v) AutoTraderCompat.Set("KeepConsumablesMaxValue", v);
            }));
            Settings.Add(Row("Öteki yiyecekler en çok (her biri)", TradeSettingsRules.ConsumableMax(Get("KeepConsumablesMinValue")), "KeepConsumablesMaxValue",
                v => v.ToString()));

            Goods.Add(StepRowVM.Header("AL / SAT"));
            Goods.Add(BuySell("Atlar (yalnızca yük atı alır)", "Horses"));
            Goods.Add(BuySell("Silahlar", "Weapons"));
            Goods.Add(BuySell("Zırhlar", "Armor"));
            Goods.Add(BuySell("Ticaret malları", "Goods"));
            Goods.Add(BuySell("Yiyecek ve içecek", "Consumables"));
            Goods.Add(BuySell("Hayvanlar (sürü yavaşlatır)", "Livestock"));
            Goods.Add(Row("Satılacak silah ve zırh kademesi", TradeSettingsRules.Tier(), "WeaponsArmorTierValue", v => v == 0 ? "hiçbiri" : v + " ve altı"));

            Goods.Add(StepRowVM.Header("ÖZEL"));
            Goods.Add(Row("Bütün sığırları sat", Toggle, "JunkCattleValue", OnOff));
            Goods.Add(Row("Kömür ve külçeleri sat", Toggle, "SellSmithingValue", OnOff));
            Goods.Add(Row("Eritilebilecek silahları tut", Toggle, "KeepSmeltingValue", OnOff));
            Goods.Add(Row("Yiyeceği \"en az\" değerine tamamla", Toggle, "ResupplyValue", OnOff));
            Goods.Add(Row("Keresteyi \"en az\" değerine tamamla", Toggle, "ResupplyHardwoodValue", OnOff));
            Goods.Add(Row("Bu sekmeyi açan kısayol", Toggle, "UseAltATValue", v => v == 1 ? "Alt+A+T" : "Alt+A"));
        }

        private static int Get(string name) => AutoTraderCompat.Get(name);

        private static string OnOff(int value) => value == 1 ? "açık" : "kapalı";

        private static string Percent(int value) => "%" + value;

        /// <summary>Bir AutoTrader ayarı: değişince (after: bağlı ayarları düzelt) dosyaya yazılır ve ekran yenilenir.</summary>
        private StepRowVM Row(string label, List<int> steps, string name, Func<int, string> format, Action<int> after = null) =>
            new StepRowVM(label, TradeSettingsRules.WithCurrent(steps, Get(name)), () => Get(name), v => Run("AutoTrader ayarı", () =>
            {
                AutoTraderCompat.Set(name, v);
                after?.Invoke(v);
            }), format, Saved);

        /// <summary>Bir mal türünde al/sat tek satırda (Buy{kind}Value, Sell{kind}Value).</summary>
        private StepRowVM BuySell(string label, string kind) =>
            new StepRowVM(label, new List<int> { 0, 1, 2, 3 }, () => TradeSettingsRules.BuySell(Get("Buy" + kind + "Value") == 1, Get("Sell" + kind + "Value") == 1),
                v => Run("AutoTrader ayarı", () =>
                {
                    AutoTraderCompat.Set("Buy" + kind + "Value", v & 1);
                    AutoTraderCompat.Set("Sell" + kind + "Value", (v & 2) >> 1);
                }), TradeSettingsRules.BuySellText, Saved);

        private void Saved() => Run("AutoTrader ayarı", () =>
        {
            AutoTraderCompat.Save();
            _changed();
        });

        public void ExecutePreset0() => ApplyPreset(0);
        public void ExecutePreset1() => ApplyPreset(1);
        public void ExecutePreset2() => ApplyPreset(2);
        public void ExecutePreset3() => ApplyPreset(3);

        private void ApplyPreset(int index)
        {
            if (!IsAvailable) return;
            string name = TradeSettingsRules.PresetNames[index];
            MenuUI.Confirm("Hazır ayar: " + name, name + " ayarları uygulansın mı? Değiştirdiği ayarların şimdiki değerleri gider.", () =>
            {
                foreach (KeyValuePair<string, int> setting in TradeSettingsRules.Preset(index)) AutoTraderCompat.Set(setting.Key, setting.Value);
                AutoTraderCompat.Save();
                MenuUI.Success("AutoTrader: " + name + " ayarları uygulandı");
                _changed();
            }, null);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Settings);
            Clear(Goods);
        }
    }
}
