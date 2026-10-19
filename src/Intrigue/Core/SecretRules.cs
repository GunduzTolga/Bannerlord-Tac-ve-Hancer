namespace CheatMenu.Intrigue.Core
{
    /// <summary>Sır türleri (tasarım 6.2). Hangi lordda hangi sırrın olabileceğine oyun katmanı (SecretFinder) karar verir.</summary>
    public enum SecretKind
    {
        Embezzlement = 1,
        TreasonPlot = 2,
        Bribery = 3,
        Cruelty = 4,
        Cowardice = 5,
        Debt = 6,
        Smuggling = 7,
        Affair = 8,
        /// <summary>Oyuncunun klanına karşı suikast planladığı ortaya çıktı (karşı casusluktan).</summary>
        MurderPlot = 9,
    }

    public static class SecretRules
    {
        /// <summary>İfşa edilince kaynağın oyuncu olduğunun anlaşılma olasılığı ve o zaman hedefle ilişki cezası.</summary>
        public const int SourceTracedChance = 30;
        public const int SourceTracedPenalty = 20;

        /// <summary>"Gizli ilişki" sırrının evli bir lordda var olma olasılığı.</summary>
        public const int AffairChance = 15;

        /// <summary>Bu altının altındaki lordlar "ağır borç" sırrına aday.</summary>
        public const int DebtGoldThreshold = 1000;

        public const int SmugglingRoguery = 100;

        public static string Name(SecretKind kind)
        {
            switch (kind)
            {
                case SecretKind.Embezzlement: return "Zimmet";
                case SecretKind.TreasonPlot: return "İhanet planı";
                case SecretKind.Bribery: return "Rüşvet alma";
                case SecretKind.Cruelty: return "Esirlere zulüm";
                case SecretKind.Cowardice: return "Korkaklık";
                case SecretKind.Debt: return "Ağır borç";
                case SecretKind.Smuggling: return "Kaçakçılık bağlantısı";
                case SecretKind.Affair: return "Gizli ilişki";
                case SecretKind.MurderPlot: return "Suikast planı";
                default: return "Bilinmeyen sır";
            }
        }

        public static string Description(SecretKind kind)
        {
            switch (kind)
            {
                case SecretKind.Embezzlement: return "Hükümdarın vergisinden kendi kesesine para aktarıyor.";
                case SecretKind.TreasonPlot: return "Hükümdarına karşı gizlice destek topluyor.";
                case SecretKind.Bribery: return "Kararlarını rüşvetle satıyor.";
                case SecretKind.Cruelty: return "Esirlerine anlatılmayacak şeyler yapıyor.";
                case SecretKind.Cowardice: return "Bir savaşta adamlarını bırakıp kaçmış.";
                case SecretKind.Debt: return "Boğazına kadar borçta; alacaklıları kapısında.";
                case SecretKind.Smuggling: return "Kaçakçı çeteleriyle iş tutuyor.";
                case SecretKind.Affair: return "Eşinden gizli bir ilişkisi var.";
                case SecretKind.MurderPlot: return "Senin ailene karşı bir suikast planladı.";
                default: return "";
            }
        }

        /// <summary>İfşa edilince hükümdarıyla (gizli ilişkide eşiyle) ilişki cezası, pozitif sayı.</summary>
        public static int ExposurePenalty(SecretKind kind)
        {
            switch (kind)
            {
                case SecretKind.Embezzlement: return 20;
                case SecretKind.TreasonPlot: return 30;
                case SecretKind.Bribery: return 15;
                case SecretKind.Cruelty: return 10;
                case SecretKind.Cowardice: return 15;
                case SecretKind.Debt: return 10;
                case SecretKind.Smuggling: return 15;
                case SecretKind.Affair: return 30;
                case SecretKind.MurderPlot: return 40;
                default: return 0;
            }
        }
    }
}
