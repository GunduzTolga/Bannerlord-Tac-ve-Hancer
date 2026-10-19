namespace CheatMenu.Intrigue.Core
{
    /// <summary>Haberciyle gönderilen mektup türleri (P2.2).</summary>
    public enum LetterKind
    {
        /// <summary>Uzaktaki bir lorda koz talebi: koz, mektup varınca harcanır.</summary>
        HookDemand = 1,
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
        /// <summary>
        /// Oyunun kendi ikna konuşmalarında (krallığa davet, evlilik, görevler) bilginin getirdiği başarı: hakkında güncel raporun
        /// varsa her denemede +10 puan, bildiğin ve ifşa edilmemiş bir sırrı varsa +5 daha.
        /// </summary>
        public const int PersuasionReportBonus = 10;
        public const int PersuasionSecretBonus = 5;

        /// <summary>Mesafeye göre yolculuk (1-8 gün).</summary>
        public static int TravelDays(float distance)
        {
            if (distance <= 0) return 1;
            int days = 1 + (int)System.Math.Ceiling(distance / MessengerSpeed);
            return SchemeMath.Clamp(days, 1, MaxTravelDays);
        }

        public static int MessengerCost(int travelDays) => 50 + 25 * travelDays;

        /// <summary>İkna denemesine eklenen başarı puanı (0-15).</summary>
        public static int PersuasionBonus(bool hasReport, bool knowsSecret) =>
            (hasReport ? PersuasionReportBonus : 0) + (knowsSecret ? PersuasionSecretBonus : 0);

        // ---- Saray istihbaratı ----

        /// <summary>Hükümdarla ilişkinin sözlü karşılığı (söylenti düzeyindeki bilgi bu kadarını verir).</summary>
        public static string MoodName(int relationWithRuler) =>
            relationWithRuler >= 30 ? "sadık"
            : relationWithRuler >= 10 ? "hoşnut"
            : relationWithRuler > -10 ? "kayıtsız"
            : relationWithRuler > -30 ? "hoşnutsuz"
            : "düşmanca";

        /// <summary>Kendi tarafına çekmek için umut veren: hükümdarına en az "hoşnutsuz".</summary>
        public static bool IsDiscontent(int relationWithRuler) => relationWithRuler <= -10;
    }
}
