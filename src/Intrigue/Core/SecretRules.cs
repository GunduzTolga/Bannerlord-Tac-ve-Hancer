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
        /// <summary>Bir çocuğun gerçek babası resmî babası değil (P2.4). Gizli ilişkinin iki tarafında da kayıtlı.</summary>
        Bastard = 10,
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

        // ---- Gizli ilişki ve gayrimeşru çocuk (P2.4, tasarım: docs/superpowers/specs/2026-10-19-p2-gizli-iliski.md) ----

        /// <summary>Romantik sırların iki tarafı da en az bu yaşta olmalı (kodda zorunlu, testli).</summary>
        public const int MinAdultAge = 18;
        /// <summary>Ebeveynin çocuğun doğumundaki en düşük yaşı: gebeliğin başında da 18+ olsun diye bir yıl pay.</summary>
        public const int MinParentAgeAtBirth = MinAdultAge + 1;
        /// <summary>Gizli ilişkinin inandırıcı olması için en büyük yaş farkı.</summary>
        public const int MaxLoverAgeGap = 20;
        /// <summary>Gizli ilişkisi bilinen birinde gayrimeşru çocuk sırrının bulunma olasılığı (uygun çocuk varsa).</summary>
        public const int BastardChance = 35;
        /// <summary>İfşada eşin klan liderinin (kayın) aldatanla ilişki cezası.</summary>
        public const int InLawPenalty = 15;
        /// <summary>İfşada aldatılan eşin, eşinin sevgilisiyle ilişki cezası.</summary>
        public const int LoverResentment = 15;
        /// <summary>Gayrimeşru çocuk ifşasında gerçek babanın kendi eşiyle ilişki cezası.</summary>
        public const int BastardSpousePenalty = 20;
        /// <summary>Aldatılan eşe gizlice söyleyince onun sana minneti.</summary>
        public const int WhisperGratitude = 10;

        public static bool IsAdult(int age) => age >= MinAdultAge;

        /// <summary>İkisi de yetişkin ve yaşları inandırıcı bir aralıkta.</summary>
        public static bool CanBeLovers(int ageA, int ageB) =>
            IsAdult(ageA) && IsAdult(ageB) && System.Math.Abs(ageA - ageB) <= MaxLoverAgeGap;

        /// <summary>Bu yaştaki biri bu yaştaki çocuğun ebeveyni olabilir mi (gebelik başında 18+).</summary>
        public static bool CouldBeParent(int parentAge, int childAge) => childAge >= 0 && parentAge - childAge >= MinParentAgeAtBirth;

        /// <summary>Gizli ilişki ya da gayrimeşru çocuk: kişiye ve ilişkiye bağlı sırlar.</summary>
        public static bool IsRomantic(SecretKind kind) => kind == SecretKind.Affair || kind == SecretKind.Bastard;

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
                case SecretKind.Bastard: return "Gayrimeşru çocuk";
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
                case SecretKind.Bastard: return "Bir çocuğun gerçek babası, resmî babası değil.";
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
                case SecretKind.Bastard: return 40;
                default: return 0;
            }
        }
    }
}
