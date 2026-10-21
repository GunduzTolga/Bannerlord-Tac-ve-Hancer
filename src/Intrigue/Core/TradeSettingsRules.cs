using System;
using System.Collections.Generic;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Saray > Ticaret sekmesinin hesapları (AutoTrader'ın ayarları; tasarım: docs/superpowers/specs/2026-10-21-ticaret-sekmesi.md): ayar
    /// basamakları (AutoTrader'ın kaydırıcı aralıklarıyla), gösterim ve hazır ayar setleri. Saf: oyunsuz test edilir.
    /// </summary>
    public static class TradeSettingsRules
    {
        /// <summary>AutoTrader'da tahılın "hepsi" sayıldığı değer (MaxKeepGrainsValue) ve arama yarıçapının sınırsız sayıldığı eşik.</summary>
        public const int AllGrain = 500, UnlimitedRadius = 999;

        /// <summary>0'dan max'a 5'er (alış/satış eşiği 0-200, envanter 0-150, aynı mal 0-envanter).</summary>
        public static List<int> Percent(int max) => Enumerable.Range(0, Math.Max(0, max) / 5 + 1).Select(i => i * 5).ToList();

        /// <summary>Ortalama fiyat arama yarıçapı (80-1000; 1000 sınırsız).</summary>
        public static List<int> Radius() => new List<int> { 80, 100, 150, 200, 250, 300, 400, 500, 600, 800, 1000 };

        public static List<int> GrainMin() => new List<int> { 0, 5, 10, 20, 30, 50, 75, 100, 150, 200, 300, 500, 750, 999 };

        /// <summary>Tahılın en çoğu: en azından aşağı inmez; 500 ve üstü "hepsi".</summary>
        public static List<int> GrainMax(int min) => AtLeast(new List<int> { 10, 20, 30, 50, 75, 100, 150, 200, 300, 400, AllGrain }, min);

        public static List<int> ConsumableMin() => new List<int> { 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20, 30, 50, 100, 200, 499 };

        public static List<int> ConsumableMax(int min) =>
            AtLeast(new List<int> { 2, 4, 6, 8, 10, 15, 20, 25, 30, 40, 50, 75, 100, 150, 200, 300, 500 }, min);

        public static List<int> WageDays() => new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 10, 12, 15, 20, 25, 30, 40, 50 };

        public static List<int> Tier() => Enumerable.Range(0, 7).ToList();

        private static List<int> AtLeast(List<int> steps, int min)
        {
            List<int> list = steps.Where(s => s >= min).ToList();
            if (!list.Contains(min)) list.Insert(0, min);
            return list;
        }

        /// <summary>Basamaklara şu anki değeri de katar (ayar dosyasında listede olmayan bir değer varsa kaybolmasın).</summary>
        public static List<int> WithCurrent(List<int> steps, int current)
        {
            if (steps.Contains(current)) return steps;
            var list = new List<int>(steps) { current };
            list.Sort();
            return list;
        }

        public static string RadiusText(int value) => value > UnlimitedRadius ? "sınırsız" : value.ToString();

        public static string GrainMaxText(int value) => value >= AllGrain ? "hepsi" : value.ToString();

        /// <summary>Bir mal türünde al/sat: 0 dokunma, 1 yalnızca al, 2 yalnızca sat, 3 al ve sat.</summary>
        public static int BuySell(bool buy, bool sell) => (buy ? 1 : 0) + (sell ? 2 : 0);

        public static string BuySellText(int value) =>
            value == 3 ? "al ve sat" : value == 1 ? "yalnızca al" : value == 2 ? "yalnızca sat" : "dokunma";

        public static readonly string[] PresetNames = { "Varsayılan", "Ganimet satıcısı", "Uzak ticaret", "Yakın ticaret" };

        /// <summary>
        /// AutoTrader'ın hazır ayar setleri (kendi penceresindeki düğmelerle aynı değerler; ad: AutoTraderConfig özelliği, aç/kapa 1/0).
        /// Varsayılan her şeyi, ötekiler yalnızca kendi alanlarını değiştirir.
        /// </summary>
        public static Dictionary<string, int> Preset(int index)
        {
            switch (index)
            {
                case 0:
                    return new Dictionary<string, int>
                    {
                        ["SimpleTradingAI"] = 1, ["BuyThresholdValue"] = 85, ["SellThresholdValue"] = 95, ["UseWeightedValue"] = 0,
                        ["MaxCapacityValue"] = 15, ["KeepGrainsMinValue"] = 10, ["KeepGrainsMaxValue"] = 100, ["KeepConsumablesMinValue"] = 4,
                        ["KeepConsumablesMaxValue"] = 20, ["UseInventorySpaceValue"] = 90, ["UseMaxFleetCapacityValue"] = 0, ["KeepWagesValue"] = 5,
                        ["SearchRadiusValue"] = 300, ["WeaponsArmorTierValue"] = 2, ["JunkCattleValue"] = 0, ["SellSmithingValue"] = 0,
                        ["KeepSmeltingValue"] = 0, ["ResupplyHardwoodValue"] = 0, ["ResupplyValue"] = 1,
                        ["BuyHorsesValue"] = 1, ["SellHorsesValue"] = 0, ["BuyWeaponsValue"] = 0, ["SellWeaponsValue"] = 1,
                        ["BuyArmorValue"] = 0, ["SellArmorValue"] = 1, ["BuyGoodsValue"] = 1, ["SellGoodsValue"] = 1,
                        ["BuyConsumablesValue"] = 1, ["SellConsumablesValue"] = 1, ["BuyLivestockValue"] = 0, ["SellLivestockValue"] = 1,
                    };
                case 1:
                    return new Dictionary<string, int>
                    {
                        ["WeaponsArmorTierValue"] = 3, ["SellSmithingValue"] = 0, ["KeepSmeltingValue"] = 0,
                        ["BuyHorsesValue"] = 0, ["SellHorsesValue"] = 1, ["BuyWeaponsValue"] = 0, ["SellWeaponsValue"] = 1,
                        ["BuyArmorValue"] = 0, ["SellArmorValue"] = 1, ["BuyGoodsValue"] = 0, ["SellGoodsValue"] = 0,
                        ["BuyConsumablesValue"] = 0, ["SellConsumablesValue"] = 0, ["BuyLivestockValue"] = 0, ["SellLivestockValue"] = 0,
                        ["JunkCattleValue"] = 1,
                    };
                case 2:
                case 3:
                    bool global = index == 2;
                    return new Dictionary<string, int>
                    {
                        ["SimpleTradingAI"] = 0, ["SearchRadiusValue"] = global ? 1000 : 300, ["UseWeightedValue"] = global ? 0 : 1,
                        ["BuyHorsesValue"] = 1, ["BuyGoodsValue"] = 1, ["SellGoodsValue"] = 1, ["BuyConsumablesValue"] = 1, ["SellConsumablesValue"] = 1,
                    };
                default:
                    return new Dictionary<string, int>();
            }
        }
    }
}
