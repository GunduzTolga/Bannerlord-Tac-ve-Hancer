namespace CheatMenu.Intrigue.Core
{
    /// <summary>Esirin gizli infazı: ölüm neye benzetilecek.</summary>
    public enum PrisonDeathMethod
    {
        /// <summary>Zindan hastalığı: yavaş bir zehir, doğal ölüm gibi görünür. En pahalısı, en az dikkat çekeni.</summary>
        Illness = 1,
        /// <summary>Kaçarken öldürüldü: gardiyanlar "firar" der. En ucuzu, en çok soru doğuranı.</summary>
        Escape = 2,
        /// <summary>Kaza: merdivenden düştü, başını çarptı.</summary>
        Accident = 3,
    }

    /// <summary>
    /// Esiri açık idam yerine gizlice öldürtmek (tasarım: docs/superpowers/specs/2026-10-21-gizli-infaz.md). Açık idamın lordlarla ilişki
    /// kaybı olmaz; ama gardiyanlar konuşabilir: önce şüphe (ölenin klanı seni görür), şüphenin bir kısmı ifşaya döner (suikast izi gibi
    /// ağır sonuçlar). İşi senin ya da casus başının (hangisi yüksekse) Roguery'si ayarlar. Saf: oyunsuz test edilir.
    /// </summary>
    public static class PrisonDeathRules
    {
        public static readonly PrisonDeathMethod[] Methods = { PrisonDeathMethod.Illness, PrisonDeathMethod.Accident, PrisonDeathMethod.Escape };

        /// <summary>Gardiyanlara, hekime ve sessizliğe giden para.</summary>
        public static int Cost(PrisonDeathMethod method) =>
            method == PrisonDeathMethod.Illness ? 1500 : method == PrisonDeathMethod.Accident ? 1000 : 500;

        public static int BaseSuspicion(PrisonDeathMethod method) =>
            method == PrisonDeathMethod.Illness ? 20 : method == PrisonDeathMethod.Accident ? 28 : 36;

        /// <summary>Yeni yakalanan birinin ölümü daha şüphelidir: bu kadar günden azsa.</summary>
        public const int FreshCaptiveDays = 7;

        /// <summary>
        /// Şüphe olasılığı: yönteme göre taban, Roguery/8 (en fazla -25) düşer; esirin klan kademesi ×2, hükümdarsa +15, yeni yakalandıysa +10,
        /// partindeyse (askerlerin görür) +5, entrika zorluğu. %5-90.
        /// </summary>
        public static int Suspicion(PrisonDeathMethod method, int roguery, int clanTier, bool isRuler, int daysHeld, bool inParty, int difficulty) =>
            SchemeMath.Clamp(BaseSuspicion(method)
                - System.Math.Min(System.Math.Max(0, roguery) / 8, 25)
                + System.Math.Max(0, clanTier) * 2
                + (isRuler ? 15 : 0)
                + (daysHeld < FreshCaptiveDays ? 10 : 0)
                + (inParty ? 5 : 0)
                + Difficulty(difficulty), 5, 90);

        /// <summary>Şüphe doğarsa ifşaya dönme olasılığı: 45 - Roguery/8, %10-45.</summary>
        public static int ProofChance(int roguery) => SchemeMath.Clamp(45 - System.Math.Max(0, roguery) / 8, 10, 45);

        /// <summary>Toplam ifşa olasılığı (kartta gösterilir).</summary>
        public static int ExposeChance(int suspicion, int proof) => suspicion * proof / 100;

        /// <summary>Şüphe: ölenin klanındaki yetişkinlerle ve krallığının hükümdarıyla ilişki cezası.</summary>
        public const int SuspicionClanPenalty = 25;
        public const int SuspicionRulerPenalty = 10;

        /// <summary>Zorluk: kolay -5, normal 0, zor +5 (entrika zorluğu).</summary>
        private static int Difficulty(int difficulty) =>
            difficulty == SchemeMath.EasyDifficulty ? -5 : difficulty == SchemeMath.HardDifficulty ? 5 : 0;

        public static string Name(PrisonDeathMethod method) =>
            method == PrisonDeathMethod.Illness ? "Zindan hastalığı" : method == PrisonDeathMethod.Accident ? "Kaza" : "Kaçarken öldürüldü";
    }
}
