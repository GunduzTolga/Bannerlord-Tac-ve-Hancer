namespace CheatMenu.Intrigue.Core
{
    /// <summary>Haberciyle gönderilen mektup türleri (P2.2).</summary>
    public enum LetterKind
    {
        /// <summary>Uzaktaki bir lorda koz talebi: koz, mektup varınca harcanır.</summary>
        HookDemand = 1,
        /// <summary>Hükümdarından hoşnutsuz bir klan liderine taraf değiştirme daveti.</summary>
        Defection = 2,
    }

    /// <summary>
    /// Haberci ve saray istihbaratının formülleri (P2.2, tasarım: docs/superpowers/specs/2026-10-19-p2-haberci-saray.md).
    /// Saf: oyunsuz test edilir.
    /// </summary>
    public static class LetterRules
    {
        /// <summary>Atlı haberci günde bu kadar harita birimi yol alır (bir parti yaklaşık 100-120).</summary>
        public const float MessengerSpeed = 150f;
        public const int MaxTravelDays = 8;
        /// <summary>Bu kadar yakındaysa yüz yüze sayılır: koz hemen kullanılır, haberci gerekmez.</summary>
        public const float FaceToFaceDistance = 10f;
        /// <summary>Davet reddedilince aynı lorda bu kadar gün yeni davet gönderilemez.</summary>
        public const int DefectionCooldownDays = 60;
        public const int DefectionGiftPerTier = 500;
        public const int DefectionRelationBonus = 10;
        public const int RefusalRelationPenalty = 5;
        public const int ForwardedRulerPenalty = 15;

        /// <summary>Mesafeye göre yolculuk (1-8 gün).</summary>
        public static int TravelDays(float distance)
        {
            if (distance <= 0) return 1;
            int days = 1 + (int)System.Math.Ceiling(distance / MessengerSpeed);
            return SchemeMath.Clamp(days, 1, MaxTravelDays);
        }

        public static int MessengerCost(int travelDays) => 50 + 25 * travelDays;

        /// <summary>Davet mektubuna eklenen hediye (geri gelmez).</summary>
        public static int DefectionGift(int clanTier) => DefectionGiftPerTier * System.Math.Max(1, clanTier);

        /// <param name="relationWithRuler">Lordun kendi hükümdarıyla ilişkisi (eksi = hoşnutsuz).</param>
        /// <param name="strengthRatio">Senin krallığının gücü / onun krallığının gücü.</param>
        /// <param name="hasReport">Güncel raporun var: teklifi ona göre yazarsın.</param>
        public static int DefectionChance(int relationWithRuler, int relationWithYou, int honor, float strengthRatio, bool hasReport,
            int difficulty)
        {
            int value = 10 - relationWithRuler / 2 + relationWithYou / 3 - 12 * honor;
            if (strengthRatio >= 1.25f) value += 10;
            else if (strengthRatio <= 0.8f) value -= 10;
            if (hasReport) value += 10;
            value += difficulty == SchemeMath.EasyDifficulty ? 10 : difficulty == SchemeMath.HardDifficulty ? -10 : 0;
            return SchemeMath.Clamp(value, 0, 80);
        }

        /// <summary>Reddeden lordun mektubu hükümdarına götürme olasılığı: onurlu lordlar ihaneti bildirir.</summary>
        public static int ForwardChance(int honor) => SchemeMath.Clamp(25 + 15 * honor, 5, 60);

        // ---- Saray istihbaratı ----

        /// <summary>Hükümdarla ilişkinin sözlü karşılığı (söylenti düzeyindeki bilgi bu kadarını verir).</summary>
        public static string MoodName(int relationWithRuler) =>
            relationWithRuler >= 30 ? "sadık"
            : relationWithRuler >= 10 ? "hoşnut"
            : relationWithRuler > -10 ? "kayıtsız"
            : relationWithRuler > -30 ? "hoşnutsuz"
            : "düşmanca";

        /// <summary>Davet için umut veren: hükümdarına en az "hoşnutsuz".</summary>
        public static bool IsDiscontent(int relationWithRuler) => relationWithRuler <= -10;
    }
}
