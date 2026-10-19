namespace CheatMenu.Intrigue.Core
{
    /// <summary>Kur yapan kim ve amaç ne (P2.6).</summary>
    public enum CourtshipMode
    {
        /// <summary>Sen kur yaparsın: başarırsa sevgilin olur.</summary>
        Lover = 1,
        /// <summary>Eşin, kendi isteğiyle, bilgi için kur yapar: rapor ve sır. Aralarında ne olacağı ikisinin kararı.</summary>
        Intel = 2,
    }

    /// <summary>
    /// Kur, sevgili ve gebelik (P2.6, tasarım: docs/superpowers/specs/2026-10-19-p2-sevgili.md). Saf: oyunsuz test edilir.
    /// İki taraf da yetişkin olmak zorunda (SecretRules.IsAdult); yalnızca metin, sahne yok.
    /// </summary>
    public static class RomanceRules
    {
        public const int StageCount = 3;
        public const int StageDays = 7;
        /// <summary>Hediyeler ve mektuplar: başlarken.</summary>
        public const int GiftCost = 500;
        /// <summary>Reddeden hedefe ya da teklifi geri çeviren eşe bu kadar gün yeniden sorulmaz.</summary>
        public const int RefuseCooldownDays = 60;
        public const int RefuseRelation = 5;
        public const int SpouseRefuseRelation = 10;
        public const int LoverRelation = 15;
        public const int MeetingCooldownDays = 14;
        public const int MeetingRelation = 5;
        public const int EndRelation = 10;
        /// <summary>Bilgi için kur: sonunda hedefle eşinin ilişkisi ve hedefin sana ilişkisi.</summary>
        public const int IntelSpouseRelation = 10;
        public const int IntelPlayerRelation = 5;
        public const int IntelSecretChance = 50;
        /// <summary>Gebelik olasılığı (yakınlık başına): tedbirsiz / tedbirli.</summary>
        public const int PregnancyChance = 15;
        public const int CarefulPregnancyChance = 4;
        /// <summary>Annenin en yüksek yaşı (en düşüğü SecretRules.MinAdultAge).</summary>
        public const int MaxMotherAge = 45;

        public static string StageName(int stage) =>
            stage <= 0 ? "Hediye ve iltifat" : stage == 1 ? "Şiir ve mektuplar" : "Gizli buluşma";

        private static int DifficultyBonus(int difficulty) =>
            difficulty == SchemeMath.EasyDifficulty ? 10 : difficulty == SchemeMath.HardDifficulty ? -10 : 0;

        /// <summary>
        /// Kurun tutma olasılığı: kur yapanın Charm'ı, hedefin ona ilişkisi, oyunun çekicilik modeli (0-100), hedefin Onuru;
        /// hedef evliyse zor. %5-85.
        /// </summary>
        public static int SuccessChance(int charm, int relation, int attraction, int targetHonor, bool targetMarried, int difficulty) =>
            SchemeMath.Clamp(15 + charm / 5 + relation / 2 + attraction / 4 - 8 * targetHonor - (targetMarried ? 10 : 0)
                + DifficultyBonus(difficulty), 5, 85);

        /// <summary>Her aşama sonunda yakalanma: hedef evliyse yüksek, kur yapanın Roguery'si düşürür. %2-20.</summary>
        public static int DiscoveryChance(int roguery, bool targetMarried, int difficulty) =>
            SchemeMath.Clamp(6 + (targetMarried ? 6 : 0) - roguery / 50 - DifficultyBonus(difficulty) / 3, 2, 20);

        /// <summary>Eşin bilgi için kur yapmayı kabul etme olasılığı: sana ilişkisi, Onuru (reddeder), Hesapçı (kabul eder). %5-90.</summary>
        public static int SpouseAgreeChance(int relationWithYou, int honor, int calculating, int relationWithTarget) =>
            SchemeMath.Clamp(30 + relationWithYou / 2 - 15 * honor + 10 * calculating + relationWithTarget / 4, 5, 90);

        /// <summary>Gizli buluşmada yakalanma: taraflardan evli olanların sayısı (0-2), senin Roguery'n. %2-16.</summary>
        public static int MeetingDiscovery(int roguery, int marriedPartners) =>
            SchemeMath.Clamp(4 + 4 * marriedPartners - roguery / 50, 2, 16);

        public static int PregnancyChanceFor(bool careful) => careful ? CarefulPregnancyChance : PregnancyChance;

        /// <summary>Anne olabilir mi: 18-45 yaş (gebeliğin başında 18+).</summary>
        public static bool CanConceive(int motherAge) => motherAge >= SecretRules.MinAdultAge && motherAge <= MaxMotherAge;

        /// <summary>Kurun kalan günü.</summary>
        public static int DaysLeft(int stage, int daysIntoStage) =>
            stage >= StageCount ? 0 : (StageCount - stage) * StageDays - daysIntoStage;
    }
}
