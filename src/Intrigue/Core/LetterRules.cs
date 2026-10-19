namespace CheatMenu.Intrigue.Core
{
    /// <summary>Haberciyle gönderilen mektup türleri (P2.2).</summary>
    public enum LetterKind
    {
        /// <summary>Uzaktaki bir lorda koz talebi: koz, mektup varınca harcanır.</summary>
        HookDemand = 1,
        /// <summary>Görüşme daveti (isteğe bağlı hediyeyle): varınca lordla konuşma penceresi açılabilir.</summary>
        Audience = 2,
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

        /// <summary>Görüşme davetine eklenebilecek hediyeler (dinar).</summary>
        public static readonly int[] GiftOptions = { 0, 1000, 3000, 6000 };
        /// <summary>Aynı lorda hediyeden ilişki bu kadar günde bir gelir (altınla ilişki satın alınmasın).</summary>
        public const int GiftCooldownDays = 30;
        /// <summary>Seninle ilişkisi bu kadar ya da daha kötüyse ulağı geri çevirir (hediye geri gelir).</summary>
        public const int AudienceRefuseRelation = -30;
        /// <summary>Ulak döndükten sonra görüşme bu kadar gün bekler.</summary>
        public const int AudienceWaitDays = 5;
        /// <summary>Hedef esirse ya da savaştaysa ulak bekler; gönderilişinden bu kadar gün sonra vazgeçer.</summary>
        public const int MaxLetterDelayDays = 10;

        /// <summary>Mesafeye göre yolculuk (1-8 gün).</summary>
        public static int TravelDays(float distance)
        {
            if (distance <= 0) return 1;
            int days = 1 + (int)System.Math.Ceiling(distance / MessengerSpeed);
            return SchemeMath.Clamp(days, 1, MaxTravelDays);
        }

        public static int MessengerCost(int travelDays) => 50 + 25 * travelDays;

        /// <summary>Hediyenin getirdiği ilişki: azalan getiri (1.000 → +2, 3.000 → +3, 6.000 → +5, en fazla +10).</summary>
        public static int GiftRelation(int gold) =>
            gold <= 0 ? 0 : SchemeMath.Clamp((int)System.Math.Round(System.Math.Sqrt(gold / 250.0)), 1, 10);

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
