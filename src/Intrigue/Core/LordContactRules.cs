namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Lordlarla iletişim (tasarım: docs/superpowers/specs/2026-10-21-lordlar.md): ziyafet vermek ve bir lordu yanına çağırmak.
    /// Saf: oyunsuz test edilir.
    /// </summary>
    public static class LordContactRules
    {
        // ---- Ziyafet: kendi şehrinde ya da kalende, davet listesindeki lordlar ----

        public const int MaxGuests = 6;
        public const int FeastCooldownDays = 30;
        public const int InfluencePerGuest = 2;
        /// <summary>Gelenlerden birinin ağzından laf kaçarsa sırrı da çıkma olasılığı.</summary>
        public const int FeastSecretChance = 20;

        /// <summary>Sofra, şarap, müzik: 400 + davetli başına 250.</summary>
        public static int FeastCost(int guests) => 400 + 250 * SchemeMath.Clamp(guests, 0, MaxGuests);

        /// <summary>
        /// Davete gelme olasılığı: 50 + seninle ilişkisi/2, yolu her gün -8, aynı krallıktansa +10; savaştaysanız gelmez. %5-95.
        /// </summary>
        public static int FeastAcceptChance(int relation, int travelDays, bool sameKingdom, bool atWar) =>
            atWar ? 0 : SchemeMath.Clamp(50 + relation / 2 - 8 * System.Math.Max(0, travelDays) + (sameKingdom ? 10 : 0), 5, 95);

        /// <summary>Gelen her lordla ilişki: 5 + Charm/50, en fazla 10.</summary>
        public static int FeastRelationGain(int charm) => SchemeMath.Clamp(5 + System.Math.Max(0, charm) / 50, 5, 10);

        /// <summary>Sofrada konuşulanlardan birinin raporu: 20 + Roguery/10, en fazla %50.</summary>
        public static int FeastGossipChance(int roguery) => SchemeMath.Clamp(20 + System.Math.Max(0, roguery) / 10, 20, 50);

        // ---- Yanına çağır: lordun partisi sana gelir (oyunun eşlik emri), konuşma oyunun kendi diyaloğu ----

        /// <summary>Bu kadar günde gelemezse vazgeçer.</summary>
        public const int SummonDays = 3;
        /// <summary>Aynı krallıktan bir lordu çağırmak nüfuz ister; başka krallıktansa altın (hediye ve yol parası).</summary>
        public const int SummonInfluence = 20;
        public const int SummonGold = 800;
        /// <summary>Bu kadar yakına gelince "geldi" sayılır (harita birimi).</summary>
        public const float ArriveDistance = 3f;

        /// <summary>
        /// Gelme olasılığı: aynı krallıktansa 40, değilse 20; + seninle ilişkisi; hükümdarıysan +30. %5-95.
        /// </summary>
        public static int SummonAcceptChance(int relation, bool sameKingdom, bool youAreTheirRuler) =>
            SchemeMath.Clamp((sameKingdom ? 40 : 20) + relation + (youAreTheirRuler ? 30 : 0), 5, 95);
    }
}
