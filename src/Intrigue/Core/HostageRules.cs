namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Rehine anlaşmaları (P2.10, tasarım: docs/superpowers/specs/2026-10-19-p2-rehine.md): barışın teminatı olarak aileden birini yabancı bir
    /// sarayda "misafir" (esir değil) bırakmak. Saf: oyunsuz test edilir.
    /// </summary>
    public static class HostageRules
    {
        /// <summary>Anlaşma bir oyun yılı sürer (oyunda yıl 84 gün).</summary>
        public const int DurationDays = 84;
        public const int GiveRulerRelation = 15;
        public const int ExchangeRulerRelation = 10;
        /// <summary>Anlaşma casus gerilimini (P2.5) bu kadar düşürür.</summary>
        public const int TensionRelief = 30;
        /// <summary>Misafir her hafta ev sahibiyle yakınlaşır (en fazla 30).</summary>
        public const int WeeklyFosterRelation = 2;
        public const int MaxFosterRelation = 30;
        public const int ReturnRulerRelation = 5;
        public const int EarlyEndPenalty = 15;
        /// <summary>Barışı sen bozarsan: hükümdarla ve krallığındaki onurlu lordlarla.</summary>
        public const int BreachRulerPenalty = 30;
        public const int BreachHonorablePenalty = 10;

        /// <summary>
        /// Değişimi kabul olasılığı: hükümdarın sana ilişkisi, casus gerilimi, güç dengesi (zayıf taraf teminata daha çok razı olur). %5-90.
        /// </summary>
        public static int ExchangeAcceptChance(int rulerRelation, int tension, float theirStrength, float ourStrength)
        {
            int balance = ourStrength > theirStrength * 1.5f ? 20 : theirStrength > ourStrength * 1.5f ? -20 : 0;
            return SchemeMath.Clamp(40 + rulerRelation / 2 - tension / 2 + balance, 5, 90);
        }

        /// <summary>Misafirin bu hafta kazanacağı yakınlık (birikmiş toplam en fazla 30).</summary>
        public static int FosterGain(int fosteredSoFar) =>
            fosteredSoFar >= MaxFosterRelation ? 0 : System.Math.Min(WeeklyFosterRelation, MaxFosterRelation - fosteredSoFar);
    }
}
