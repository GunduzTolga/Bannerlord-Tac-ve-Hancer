using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Divan: serdar (P2.14, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-serdar.md). Savaş meclisi: düşmanın gücü, orduları,
    /// zayıf kaleleri, kendi savunman ve sana yürüyen ordulara erken uyarı. Tactics tahminlerin isabetini, Scouting ne kadar uzağı
    /// gördüğünü ve uyarının olasılığını belirler. Saf: formüller; oyunsuz test edilir.
    /// </summary>
    public static class MarshalRules
    {
        /// <summary>Uyarılan ordu-hedef çifti bu kadar gün yeniden sorulmaz.</summary>
        public const int WarnMemoryDays = 10;
        public const int WeeklyXp = 40;
        public const int WarningXp = 60;
        /// <summary>Erzağı bu kadar günden az kalan kale ya da şehir "kıtlık" sayılır.</summary>
        public const int LowFoodDays = 10;

        /// <summary>Asker sayısı tahminlerinin hata payı (%): 45 - Tactics/6, %5–60.</summary>
        public static int ErrorPercent(int tactics) => SchemeMath.Clamp(45 - Math.Max(0, tactics) / 6, 5, 60);

        /// <summary>Düşman ordularını gördüğü harita uzaklığı: 200 + Scouting × 2, en fazla 900.</summary>
        public static float KnownRange(int scouting) => SchemeMath.Clamp(200 + Math.Max(0, scouting) * 2, 200, 900);

        /// <summary>Sana yürüyen bir orduyu zamanında haber alma olasılığı: 30 + Scouting/4, en fazla %95.</summary>
        public static int WarningChance(int scouting) => SchemeMath.Clamp(30 + Math.Max(0, scouting) / 4, 30, 95);

        /// <summary>Kuşatmada karşına çıkacak savunma: garnizon + milisin %60'ı, sur seviyesi başına +%25.</summary>
        public static int DefenseScore(int garrison, int militia, int wallLevel) =>
            (int)Math.Round((Math.Max(0, garrison) + Math.Max(0, militia) * 0.6) * (1 + 0.25 * Math.Max(0, wallLevel)), MidpointRounding.AwayFromZero);

        /// <summary>Erzak kaç gün yeter; azalmıyorsa int.MaxValue.</summary>
        public static int FoodDays(float stocks, float change) =>
            change >= 0 ? int.MaxValue : (int)Math.Floor(Math.Max(0, stocks) / -change);

        /// <summary>Yürüyüş süresi (saat): harita uzaklığı / saatlik hız.</summary>
        public static int TravelHours(float distance, float speedPerHour) =>
            (int)Math.Ceiling(Math.Max(0, distance) / Math.Max(0.5f, speedPerHour));

        public static string TravelText(int hours) => hours < 24 ? "~" + Math.Max(1, hours) + " saat" : "~" + (int)Math.Round(hours / 24.0, MidpointRounding.AwayFromZero) + " gün";

        /// <summary>Tahmini sayı: 100'ün altı 5'e, 1000'in altı 10'a, üstü 50'ye yuvarlanır.</summary>
        public static int Round(double value)
        {
            int step = value < 100 ? 5 : value < 1000 ? 10 : 50;
            return (int)(Math.Round(Math.Max(0, value) / step, MidpointRounding.AwayFromZero) * step);
        }

        /// <summary>Onların gücü / bizim gücümüz oranına göre serdarın tavsiyesi.</summary>
        public static string Advice(double ratio)
        {
            if (ratio >= 1.5) return "Bizden çok güçlüler: barışı düşün";
            if (ratio >= 1.15) return "Bizden güçlüler: savunmada kal, kalelerini besle";
            if (ratio >= 0.85) return "Güçler denk: fırsat kolla";
            if (ratio >= 0.6) return "Bizden zayıflar: saldırı zamanı";
            return "Çok zayıflar: kalelerine yürü";
        }

        /// <summary>Oranın tehlikeli sayıldığı eşik (kırmızı gösterilir).</summary>
        public static bool IsDangerous(double ratio) => ratio >= 1.15;
    }
}
