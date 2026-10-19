using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Divan: saray hekimi (P2.13, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-hekim.md). Hekimin Medicine'i ne kadar yüksekse
    /// klanında doğumda ve yaşlılıktan ölüm o kadar azalır, suikastte kurtarma şansı artar, eşinin gizli gebeliğini fark eder.
    /// Medicine 30'un altındaki hekim şarlatandır: tedavisi riski artırır. Saf: formüller; oyunsuz test edilir.
    /// </summary>
    public static class PhysicianRules
    {
        /// <summary>Bunun altında şarlatan: koruma eksiye döner.</summary>
        public const int QuackBelow = 30;
        /// <summary>Oyunun bir yılı (gün).</summary>
        public const int DaysPerYear = 84;
        /// <summary>Yıllık ölüm riski bunu geçince hekim uyarır (%).</summary>
        public const int WarnYearlyRisk = 10;
        public const int WarnCooldownDays = 84;
        public const int SaveXp = 500;
        public const int WeeklyXp = 40;

        public static bool IsQuack(int medicine) => medicine < QuackBelow;

        /// <summary>Yaşlılıktan ölüm riskindeki azalma (%): (Medicine - 30) / 5, -%10 ile %50 arası (eksi: risk artar).</summary>
        public static int LongevityPercent(int medicine) => SchemeMath.Clamp((medicine - QuackBelow) / 5, -10, 50);

        /// <summary>Doğumda ölüm riskindeki azalma (%): (Medicine - 30) / 3, -%10 ile %80 arası.</summary>
        public static int LaborPercent(int medicine) => SchemeMath.Clamp((medicine - QuackBelow) / 3, -10, 80);

        /// <summary>Başarılı bir suikastten kurtarma olasılığı: (Medicine - 30) / 6, en fazla %40.</summary>
        public static int PoisonSaveChance(int medicine) => SchemeMath.Clamp((medicine - QuackBelow) / 6, 0, 40);

        /// <summary>Eşinin gizli gebeliğini haftalık fark etme olasılığı: 5 + Medicine/6, en fazla %50.</summary>
        public static int HiddenPregnancyChance(int medicine) => SchemeMath.Clamp(5 + Math.Max(0, medicine) / 6, 5, 50);

        /// <summary>Hekimin ömür tahminlerindeki hata payı (%): 45 - Medicine/6, %5–60.</summary>
        public static int ErrorPercent(int medicine) => SchemeMath.Clamp(45 - Math.Max(0, medicine) / 6, 5, 60);

        /// <summary>Olasılığa yüzde azalma uygular (eksi yüzde artırır); 0–1 arasında kalır.</summary>
        public static float Apply(float probability, int percent) => Math.Min(1f, Math.Max(0f, probability * (100 - percent) / 100f));

        /// <summary>Oyunun günlük ölüm olasılığından yıllık risk.</summary>
        public static double YearlyRisk(double daily) => 1 - Math.Pow(1 - Math.Min(1, Math.Max(0, daily)), DaysPerYear);
    }
}
