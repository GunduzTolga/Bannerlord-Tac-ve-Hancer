using System;
using System.Collections.Generic;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>Kervan defterinde bir sefer: bir şehirden çıkıştan bir sonraki şehirden çıkışa kadar.</summary>
    public sealed class CaravanLeg
    {
        public string PartyId;
        public string FromId;
        public string ToId;
        /// <summary>Seferin net kazancı: kesedeki altın + yükün değeri farkı, artı bu arada sahibine gönderilen pay.</summary>
        public int Profit;
        public int Days;
        /// <summary>Seferin bittiği gün.</summary>
        public int Day;
    }

    /// <summary>Kervanın son şehirden çıkışı: sonraki seferin başlangıcı.</summary>
    public sealed class CaravanBook
    {
        public string PartyId;
        public string TownId;
        public int Worth;
        public int Day;
        /// <summary>O çıkıştan beri sahibine (sana) gönderilen pay.</summary>
        public int SentHome;
    }

    /// <summary>Bir rotanın (şehirden şehre) defterdeki toplamı.</summary>
    public sealed class RouteStat
    {
        public string FromId;
        public string ToId;
        public int Trips;
        public int Total;
        public int Average => Trips == 0 ? 0 : Total / Trips;
    }

    /// <summary>
    /// Divan: hazinedar (P2.12, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-hazinedar.md). Hazinedarın Trade'i ne kadar
    /// yüksekse fiyat tahminleri o kadar isabetli; düşükse yanlış öneri verebilir. Onuru düşük hazinedar zimmete para geçirebilir.
    /// Saf: formüller; oyunsuz test edilir.
    /// </summary>
    public static class TreasuryRules
    {
        /// <summary>Defterde tutulan en fazla sefer (en eskisi düşer).</summary>
        public const int MaxLegs = 80;
        /// <summary>Atölyenin üretimini uzaktan değiştirmek: ustabaşına giden ulağın ücreti (oyunun değiştirme bedeline ek).</summary>
        public const int RemoteOrderFee = 200;
        /// <summary>Ticaret fırsatı sayılması için en az fiyat farkı (%).</summary>
        public const int MinSpreadPercent = 20;
        /// <summary>Üretimi değiştirmeye değmesi için günde en az bu kadar fazla kazanç (ya da şimdikinin %20'si).</summary>
        public const int SwitchMinGain = 10;
        /// <summary>Kervanla gidilebilecek şehirler arası en fazla harita uzaklığı (fırsatlar ve kervan önerisi için).</summary>
        public const float TradeRange = 250f;
        public const int EmbezzleRefundRelation = 10;
        public const int EmbezzleKeepRelation = 5;
        public const int EmbezzleForgiveRelation = 5;

        /// <summary>Tahminlerin hata payı (%): Trade her 6 puanda 1, Hesapçı her seviyede 5 düşürür; %5–60.</summary>
        public static int ErrorPercent(int trade, int calculating) => SchemeMath.Clamp(45 - Math.Max(0, trade) / 6 - 5 * calculating, 5, 60);

        public static string Confidence(int error) =>
            error <= 10 ? "kesin" : error <= 20 ? "güvenilir" : error <= 35 ? "tahmini" : "kulaktan dolma";

        /// <summary>Hazinedarın gördüğü değer: gerçeği ± hata payı (zar 1 en düşük, 100 en yüksek).</summary>
        public static double Noisy(double value, int error, int roll100) =>
            value * (1 + error / 100.0 * (SchemeMath.Clamp(roll100, 1, 100) - 50.5) / 49.5);

        /// <summary>Hazinedarın fiyatlarını bildiği şehir sayısı (sana en yakınlar): 8 + Trade/6, en fazla 60.</summary>
        public static int KnownTowns(int trade) => SchemeMath.Clamp(8 + Math.Max(0, trade) / 6, 8, 60);

        public static int SpreadPercent(double buy, double sell) => buy <= 0 ? 0 : (int)Math.Round((sell - buy) * 100 / buy, MidpointRounding.AwayFromZero);

        /// <summary>Bir atölye türünün günlük tahmini kârı: her üretim için (çıktı - girdi) × hız.</summary>
        public static double WorkshopMargin(IEnumerable<(double inputs, double outputs, double speed)> productions) =>
            productions.Sum(p => (p.outputs - p.inputs) * p.speed);

        public static bool WorthSwitching(double current, double best) =>
            best - current >= Math.Max(SwitchMinGain, Math.Abs(current) * 0.2);

        public static int LegProfit(int worthBefore, int worthAfter, int sentHome) => worthAfter - worthBefore + sentHome;

        public static void AddLeg(List<CaravanLeg> legs, CaravanLeg leg)
        {
            legs.Add(leg);
            if (legs.Count > MaxLegs) legs.RemoveRange(0, legs.Count - MaxLegs);
        }

        /// <summary>Seferleri rotalara toplar; en kârlıdan en zararlıya.</summary>
        public static List<RouteStat> Routes(IEnumerable<CaravanLeg> legs) =>
            legs.Where(l => l.FromId != l.ToId)
                .GroupBy(l => l.FromId + ">" + l.ToId)
                .Select(g => new RouteStat { FromId = g.First().FromId, ToId = g.First().ToId, Trips = g.Count(), Total = g.Sum(l => l.Profit) })
                .OrderByDescending(r => r.Average).ThenBy(r => r.FromId, StringComparer.Ordinal).ThenBy(r => r.ToId, StringComparer.Ordinal)
                .ToList();

        /// <summary>Haftalık zimmet olasılığı: Onur -1'de %25, -2'de %50.</summary>
        public static int EmbezzleChance(int honor) => honor <= -2 ? 50 : honor == -1 ? 25 : 0;

        /// <summary>Haftalık gelirin %5–14'ü (zara göre).</summary>
        public static int EmbezzleAmount(int weeklyIncome, int roll100) =>
            weeklyIncome <= 0 ? 0 : weeklyIncome * (5 + (SchemeMath.Clamp(roll100, 1, 100) - 1) / 10) / 100;

        /// <summary>Casus başının zimmeti haftalık fark etme olasılığı: 15 + Roguery/10 (+20 karşı casuslukta), en fazla %75.</summary>
        public static int EmbezzleDetectChance(int spymasterRoguery, bool counterIntel) =>
            SchemeMath.Clamp(15 + Math.Max(0, spymasterRoguery) / 10 + (counterIntel ? 20 : 0), 0, 75);

        /// <summary>FNV-1a: sürümden sürüme değişmeyen metin özeti (zar tohumu için).</summary>
        public static int Hash(string text)
        {
            unchecked
            {
                uint hash = 2166136261;
                foreach (char c in text ?? "")
                {
                    hash ^= c;
                    hash *= 16777619;
                }
                return (int)hash;
            }
        }

        /// <summary>Haftaya ve anahtara bağlı sabit zar: aynı hafta aynı fiyat aynı hatayla görünür, ekran her açılışta değişmez.</summary>
        public static int WeeklyRoll(int seed, int week, string key)
        {
            int counter = 0;
            return SchemeRandom.Roll100(seed ^ (week * 7919) ^ Hash(key), ref counter);
        }
    }
}
