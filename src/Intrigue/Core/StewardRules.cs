using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Divan: kâhya (P2.15, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-kahya.md). Bütün şehir ve kalelerine tek bakış, uzaktan
    /// inşaat ve günlük proje emri, isyan ve kıtlık uyarısı. Steward'ı yüksek kâhya klanının yerleşimlerinde sadakati her gün biraz artırır;
    /// 30'un altındaysa yolsuzluk yapar, sadakat düşer. Saf: formüller; oyunsuz test edilir.
    /// </summary>
    public static class StewardRules
    {
        /// <summary>Bunun altında yolsuz kâhya: sadakate eksi.</summary>
        public const int CorruptBelow = 30;
        public const float CorruptLoyalty = -0.3f;
        public const float MaxLoyalty = 1f;
        /// <summary>Uzaktan emir: kâhyanın ulağı.</summary>
        public const int RemoteOrderFee = 100;
        /// <summary>İsyan eşiğine bu kadar gün kala uyarır.</summary>
        public const int RebellionWarnDays = 10;
        /// <summary>Erzak bu kadar günden az kalınca uyarır.</summary>
        public const int StarvationWarnDays = 5;
        /// <summary>Aynı yerleşim için aynı uyarı bu kadar gün tekrarlanmaz.</summary>
        public const int WarnCooldownDays = 14;
        public const int MaxBuildingLevel = 3;
        public const int WeeklyXp = 40;

        public static bool IsCorrupt(int steward) => steward < CorruptBelow;

        /// <summary>Günlük sadakat etkisi: (Steward - 30) / 200, en fazla +1; 30'un altında -0,3.</summary>
        public static float LoyaltyBonus(int steward) =>
            IsCorrupt(steward) ? CorruptLoyalty : Math.Min(MaxLoyalty, (steward - CorruptBelow) / 200f);

        /// <summary>Değer eşiğin altına kaç günde iner; düşmüyorsa int.MaxValue, zaten altındaysa 0.</summary>
        public static int DaysUntil(float value, float dailyChange, float threshold)
        {
            if (value <= threshold) return 0;
            if (dailyChange >= 0) return int.MaxValue;
            return (int)Math.Ceiling((value - threshold) / -dailyChange);
        }

        public static string Signed(float value, IFormatProvider culture) =>
            (value > 0 ? "+" : "") + value.ToString("0.0", culture);
    }
}
