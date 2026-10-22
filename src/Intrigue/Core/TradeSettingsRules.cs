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
        /// Bir ayarı AutoTrader'ın kendi penceresindeki gibi değiştirir: o pencerenin özellikleri birbirine bağlı (ör. "bütün sığırları sat"
        /// açılınca hayvan alımı kapanır, satışı açılır; fiyat yöntemi değişince eşikler o yöntemin varsayılanına döner). Değer zaten aynıysa
        /// hiçbir şey olmaz (onun setter'ları gibi). get/set: AutoTraderConfig'in özellikleri (aç/kapa 1/0).
        /// </summary>
        public static void Set(string name, int value, Func<string, int> get, Action<string, int> set)
        {
            if (get(name) == value) return;
            set(name, value);
            switch (name)
            {
                case "SimpleTradingAI":
                    Set("SellThresholdValue", value == 1 ? 125 : 95, get, set);
                    Set("BuyThresholdValue", value == 1 ? 80 : 70, get, set);
                    break;
                case "JunkCattleValue":
                    if (value == 1)
                    {
                        Set("BuyLivestockValue", 0, get, set);
                        Set("SellLivestockValue", 1, get, set);
                    }
                    break;
                case "BuyLivestockValue":
                    if (value == 1) Set("JunkCattleValue", 0, get, set);
                    break;
                case "ResupplyValue":
                    if (value == 1) Set("BuyConsumablesValue", 1, get, set);
                    break;
                case "BuyConsumablesValue":
                    if (value == 0) Set("ResupplyValue", 0, get, set);
                    break;
                case "ResupplyHardwoodValue":
                    if (value == 1) Set("BuyGoodsValue", 1, get, set);
                    break;
                case "BuyGoodsValue":
                    if (value == 0) Set("ResupplyHardwoodValue", 0, get, set);
                    break;
            }
        }

        private static KeyValuePair<string, int> P(string name, int value) => new KeyValuePair<string, int>(name, value);

        /// <summary>
        /// AutoTrader'ın hazır ayar setleri: kendi penceresindeki düğmelerle aynı değerler ve aynı sırayla (Set ile uygulanır, bağlı ayarlar da
        /// onunki gibi değişir; ad: AutoTraderConfig özelliği, aç/kapa 1/0). Varsayılan her şeyi, ötekiler yalnızca kendi alanlarını değiştirir.
        /// </summary>
        public static List<KeyValuePair<string, int>> Preset(int index)
        {
            switch (index)
            {
                case 0:
                    return new List<KeyValuePair<string, int>>
                    {
                        P("SimpleTradingAI", 1), P("BuyThresholdValue", 85), P("SellThresholdValue", 95), P("UseWeightedValue", 0),
                        P("MaxCapacityValue", 15), P("KeepGrainsMinValue", 10), P("KeepGrainsMaxValue", 100), P("KeepConsumablesMinValue", 4),
                        P("KeepConsumablesMaxValue", 20), P("UseInventorySpaceValue", 90), P("UseMaxFleetCapacityValue", 0), P("KeepWagesValue", 5),
                        P("SearchRadiusValue", 300), P("WeaponsArmorTierValue", 2), P("JunkCattleValue", 0), P("SellSmithingValue", 0),
                        P("KeepSmeltingValue", 0), P("ResupplyHardwoodValue", 0), P("ResupplyValue", 1),
                        P("BuyHorsesValue", 1), P("SellHorsesValue", 0), P("BuyWeaponsValue", 0), P("SellWeaponsValue", 1),
                        P("BuyArmorValue", 0), P("SellArmorValue", 1), P("BuyGoodsValue", 1), P("SellGoodsValue", 1),
                        P("BuyConsumablesValue", 1), P("SellConsumablesValue", 1), P("BuyLivestockValue", 0), P("SellLivestockValue", 1),
                    };
                case 1:
                    return new List<KeyValuePair<string, int>>
                    {
                        P("WeaponsArmorTierValue", 3), P("SellSmithingValue", 0), P("KeepSmeltingValue", 0),
                        P("BuyHorsesValue", 0), P("SellHorsesValue", 1), P("BuyWeaponsValue", 0), P("SellWeaponsValue", 1),
                        P("BuyArmorValue", 0), P("SellArmorValue", 1), P("BuyGoodsValue", 0), P("SellGoodsValue", 0),
                        P("BuyConsumablesValue", 0), P("SellConsumablesValue", 0), P("BuyLivestockValue", 0), P("SellLivestockValue", 0),
                        P("JunkCattleValue", 1),
                    };
                case 2:
                case 3:
                    bool global = index == 2;
                    return new List<KeyValuePair<string, int>>
                    {
                        P("SimpleTradingAI", 0), P("SearchRadiusValue", global ? 1000 : 300), P("UseWeightedValue", global ? 0 : 1),
                        P("BuyHorsesValue", 1), P("BuyGoodsValue", 1), P("SellGoodsValue", 1), P("BuyConsumablesValue", 1), P("SellConsumablesValue", 1),
                    };
                default:
                    return new List<KeyValuePair<string, int>>();
            }
        }
    }
}
