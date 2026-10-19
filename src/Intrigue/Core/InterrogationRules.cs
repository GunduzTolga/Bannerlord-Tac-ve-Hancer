namespace CheatMenu.Intrigue.Core
{
    /// <summary>Esir sorgulama yöntemleri (yol haritası P2.1).</summary>
    public enum InterrogationMethod
    {
        /// <summary>Konuşturmak: Charm'a ve ilişkiye bağlı, bedeli yok.</summary>
        Talk = 1,
        /// <summary>Korkutup baskı yapmak: Roguery'ye bağlı, sır bulma şansı yüksek; Merhamet ve ilişki bedeli var.</summary>
        Pressure = 2,
        /// <summary>Bilgi karşılığı serbest bırakma sözü: en güvenilir yol, ama esir (ve fidyesi) gider.</summary>
        Deal = 3,
    }

    /// <summary>
    /// Esir sorgulamanın formülleri. Sahne yok, yalnızca metin: baskı "tehdit ve korkutma"dır, işkence değil.
    /// Saf: oyunsuz test edilir. Zorluk ayarı başarıya entrikalardaki gibi +10 / -10 ekler.
    /// </summary>
    public static class InterrogationRules
    {
        /// <summary>Aynı esir bu kadar gün içinde yeniden sorgulanamaz.</summary>
        public const int CooldownDays = 30;
        /// <summary>Baskı: esirle ilişki ve Merhamet bedeli.</summary>
        public const int PressureRelationPenalty = 15;
        public const int PressureMercy = -30;
        /// <summary>Baskı haberi esirin klanına %30 ihtimalle ulaşır; klan lideriyle ilişki -5.</summary>
        public const int ClanHearsChance = 30;
        public const int ClanHearsPenalty = 5;
        /// <summary>Anlaşma: serbest bırakılan esir minnet duyar.</summary>
        public const int DealRelationBonus = 10;
        public const int SuccessXp = 200;

        public static int Chance(InterrogationMethod method, int charm, int roguery, int relation, int targetCalculating,
            int targetValor, int targetHonor, int difficulty)
        {
            int bonus = difficulty == SchemeMath.EasyDifficulty ? 10 : difficulty == SchemeMath.HardDifficulty ? -10 : 0;
            switch (method)
            {
                case InterrogationMethod.Talk:
                    return SchemeMath.Clamp(30 + charm / 5 + relation / 2 - 5 * targetCalculating + bonus, 10, 85);
                case InterrogationMethod.Pressure:
                    return SchemeMath.Clamp(45 + roguery / 5 - 5 * targetValor + bonus, 15, 90);
                default:
                    return SchemeMath.Clamp(70 + relation / 2 + 5 * targetHonor + bonus, 40, 95);
            }
        }

        /// <summary>Konuşursa bir sırrını da ağzından kaçırma olasılığı (casuslukla aynı tabana, yönteme göre ek).</summary>
        public static int SecretChance(InterrogationMethod method, int roguery) =>
            SchemeMath.Clamp(SchemeMath.SecretChance(roguery, 1) + (method == InterrogationMethod.Pressure ? 20 : 0), 0, 95);

        /// <summary>Efendisi (klan lideri ya da hükümdarı) hakkında da konuşma olasılığı: o kişi için rapor verir.</summary>
        public static int LiegeIntelChance(InterrogationMethod method) =>
            method == InterrogationMethod.Pressure ? 50 : method == InterrogationMethod.Deal ? 35 : 25;

        public static string Name(InterrogationMethod method) =>
            method == InterrogationMethod.Talk ? "Konuştur" : method == InterrogationMethod.Pressure ? "Baskı yap" : "Anlaşma teklif et";
    }
}
