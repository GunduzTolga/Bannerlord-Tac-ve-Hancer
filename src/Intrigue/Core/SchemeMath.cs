using System;

namespace CheatMenu.Intrigue.Core
{
    public enum SchemeKind
    {
        Spy = 1,
        Murder = 2,
        Blackmail = 3,
        Kidnap = 4,
        /// <summary>E3: kuşattığın yerleşimin erzakı, garnizonu ya da surları. Hedef bir yerleşim.</summary>
        Sabotage = 5,
        /// <summary>E3: şehrin sadakatini düşürür, oyunun kendi isyanına iter. Hedef bir şehir.</summary>
        Incite = 6,
    }

    /// <summary>Türlere göre sabit kurallar (tasarım bölüm 4).</summary>
    public static class SchemeRules
    {
        public static int MinPhases(SchemeKind kind) =>
            kind == SchemeKind.Sabotage ? 1 : kind == SchemeKind.Incite || IsStrike(kind) ? 2 : 1;

        public static int MaxPhases(SchemeKind kind)
        {
            switch (kind)
            {
                case SchemeKind.Murder: return 5;
                case SchemeKind.Kidnap:
                case SchemeKind.Incite: return 4;
                case SchemeKind.Sabotage: return 3;
                default: return 2;
            }
        }

        /// <summary>Oyuncunun seçtiği anda zarı atılan ("harekete geç") türler; diğerleri son aşamada kendiliğinden sonuçlanır.</summary>
        public static bool IsStrike(SchemeKind kind) =>
            kind == SchemeKind.Murder || kind == SchemeKind.Kidnap || TargetsSettlement(kind);

        /// <summary>Hedefi bir kişi değil yerleşim olan türler (E3): Scheme.TargetId yerleşimin kimliğidir.</summary>
        public static bool TargetsSettlement(SchemeKind kind) => kind == SchemeKind.Sabotage || kind == SchemeKind.Incite;

        public static string Name(SchemeKind kind)
        {
            switch (kind)
            {
                case SchemeKind.Murder: return "Suikast";
                case SchemeKind.Blackmail: return "Şantaj";
                case SchemeKind.Kidnap: return "Kaçırma";
                case SchemeKind.Sabotage: return "Sabotaj";
                case SchemeKind.Incite: return "Kışkırtma";
                default: return "Casusluk";
            }
        }
    }

    /// <summary>Bir hesap için gereken her şey, düz sayı olarak. Oyun katmanı (SchemeFactors) doldurur.</summary>
    public sealed class SchemeInputs
    {
        // Casus başı
        public int Roguery;
        public int Calculating;

        // Plan
        public int CompletedPhases;
        public bool HasReport;
        public int EventSuccess;
        public int EventDiscovery;
        public int BribeLevel;
        public int Difficulty = SchemeMath.NormalDifficulty;

        // Hedef
        public int TargetRoguery;
        public int TargetCalculating;
        public int TargetClanTier;
        public bool TargetIsClanLeader;
        public bool TargetIsRuler;
        public bool TargetInOwnSettlement;
        public bool TargetInArmy;
        public bool TargetIsPlayersCaptive;
        public bool TargetAlerted;
        public bool HasMet = true;

        // E2
        public int NetworkSuccess;
        public int NetworkDiscovery;
        public bool HasHook;
        public int SecretSeverity;
    }

    /// <summary>
    /// Entrika formülleri (tasarım bölüm 5.3, 5.4, 6.1). Saf: oyunsuz test edilir; denge ayarı yalnızca burada yapılır.
    /// Ara değerler ondalıklı hesaplanır, sonuç en yakın tam sayıya (yarım yukarı) yuvarlanıp sınırlara kırpılır.
    /// </summary>
    public static class SchemeMath
    {
        public const int EasyDifficulty = 0;
        public const int NormalDifficulty = 1;
        public const int HardDifficulty = 2;

        public const int ReportValidDays = 60;
        public const int CooldownDays = 120;
        public const int AlertDays = 180;
        public const int MaxBribeLevel = 3;

        private static readonly int[] BribeSuccess = { 0, 5, 10, 15 };
        private static readonly double[] BribeCostFactor = { 0, 0.5, 1, 2 };

        public static int MurderSuccess(SchemeInputs i) =>
            Clamp(Round(StrikeValue(SchemeKind.Murder, i)), 5, i.TargetIsRuler ? 70 : 85);

        /// <summary>Kaçırma: suikastla aynı etkenler, 5 puan daha zor, daha düşük tavan.</summary>
        public static int KidnapSuccess(SchemeInputs i) =>
            Clamp(Round(StrikeValue(SchemeKind.Kidnap, i) - 5), 5, i.TargetIsRuler ? 60 : 80);

        private static double StrikeValue(SchemeKind kind, SchemeInputs i) =>
            10
                + Math.Min(i.Roguery / 10.0, 30)
                + i.Calculating * 3
                + (i.HasReport ? 10 : 0)
                + Opportunity(kind, i.CompletedPhases)
                + Clamp(i.EventSuccess, -15, 15)
                + BribeSuccess[Clamp(i.BribeLevel, 0, MaxBribeLevel)]
                - Math.Min(i.TargetRoguery / 20.0, 15)
                - Math.Max(i.TargetCalculating, 0) * 3
                - i.TargetClanTier * 2
                - (i.TargetIsClanLeader ? 5 : 0)
                - (i.TargetIsRuler ? 15 : 0)
                - (i.TargetInOwnSettlement ? 10 : 0)
                - (i.TargetInArmy ? 10 : 0)
                + (i.TargetIsPlayersCaptive ? 20 : 0)
                - (i.TargetAlerted ? 15 : 0)
                + Math.Min(i.NetworkSuccess, 15)
                + (i.HasHook ? 10 : 0)
                + DifficultySuccess(i.Difficulty);

        /// <summary>Her aşama sonunda entrikanın ortaya çıkma olasılığı.</summary>
        public static int MurderDiscovery(SchemeInputs i) => Clamp(Round(StrikeDiscoveryValue(i)), 2, 40);

        /// <summary>Kaçırma suikasttan daha gürültülü: +3.</summary>
        public static int KidnapDiscovery(SchemeInputs i) => Clamp(Round(StrikeDiscoveryValue(i) + 3), 2, 40);

        private static double StrikeDiscoveryValue(SchemeInputs i) =>
            12
                - Math.Min(i.Roguery / 25.0, 12)
                + Math.Min(i.TargetRoguery / 30.0, 10)
                + Math.Max(i.TargetCalculating, 0) * 2
                + i.EventDiscovery
                - Math.Min(i.NetworkDiscovery, 6)
                + DifficultyDiscovery(i.Difficulty);

        /// <summary>Şantaj: elindeki sır ne kadar ağırsa (ifşa cezası) o kadar etkili; 2. aşama +15.</summary>
        public static int BlackmailSuccess(SchemeInputs i)
        {
            double value = 40
                + i.Roguery / 5.0
                - i.TargetRoguery / 10.0
                - i.TargetClanTier * 2
                - (i.TargetIsRuler ? 10 : 0)
                + i.SecretSeverity / 2.0
                + Opportunity(SchemeKind.Blackmail, i.CompletedPhases)
                + Clamp(i.EventSuccess, -15, 15)
                + i.NetworkSuccess / 2.0
                + DifficultySuccess(i.Difficulty);
            return Clamp(Round(value), 15, 90);
        }

        public static int BlackmailDiscovery(SchemeInputs i)
        {
            double value = 8
                - i.Roguery / 40.0
                + i.TargetRoguery / 40.0
                + i.EventDiscovery
                - i.NetworkDiscovery / 2.0
                + DifficultyDiscovery(i.Difficulty);
            return Clamp(Round(value), 2, 30);
        }

        public static int SpySuccess(SchemeInputs i)
        {
            double value = 50
                + i.Roguery / 5.0
                - i.TargetRoguery / 10.0
                - i.TargetClanTier * 2
                - (i.TargetIsRuler ? 10 : 0)
                - (i.HasMet ? 0 : 10)
                + Clamp(i.EventSuccess, -15, 15)
                + i.NetworkSuccess / 2.0
                + DifficultySuccess(i.Difficulty);
            return Clamp(Round(value), 20, 95);
        }

        public static int SpyDiscovery(SchemeInputs i)
        {
            double value = 6
                - i.Roguery / 50.0
                + i.TargetRoguery / 40.0
                + i.EventDiscovery
                - i.NetworkDiscovery / 2.0
                + DifficultyDiscovery(i.Difficulty);
            return Clamp(Round(value), 1, 25);
        }

        public static int Success(SchemeKind kind, SchemeInputs i)
        {
            switch (kind)
            {
                case SchemeKind.Murder: return MurderSuccess(i);
                case SchemeKind.Kidnap: return KidnapSuccess(i);
                case SchemeKind.Blackmail: return BlackmailSuccess(i);
                default: return SpySuccess(i);
            }
        }

        public static int Discovery(SchemeKind kind, SchemeInputs i)
        {
            switch (kind)
            {
                case SchemeKind.Murder: return MurderDiscovery(i);
                case SchemeKind.Kidnap: return KidnapDiscovery(i);
                case SchemeKind.Blackmail: return BlackmailDiscovery(i);
                default: return SpyDiscovery(i);
            }
        }

        /// <summary>
        /// Zorunlu aşamalardan sonra beklenen her aşamanın başarıya katkısı: suikastta +6 (en fazla 3 aşama),
        /// kaçırmada +6 (en fazla 2), şantajda 2. aşama +15, casuslukta yok (2. aşama sır şansını artırır).
        /// </summary>
        public static int Opportunity(SchemeKind kind, int completedPhases)
        {
            int extra = completedPhases - SchemeRules.MinPhases(kind);
            switch (kind)
            {
                case SchemeKind.Murder: return 6 * Clamp(extra, 0, 3);
                case SchemeKind.Kidnap: return 6 * Clamp(extra, 0, 2);
                case SchemeKind.Blackmail: return 15 * Clamp(extra, 0, 1);
                default: return 0;
            }
        }

        public static int StartCost(SchemeKind kind, int targetClanTier, bool isClanLeader, bool isRuler)
        {
            if (kind == SchemeKind.Spy) return 500 + 250 * targetClanTier;
            if (kind == SchemeKind.Blackmail) return 300 + 200 * targetClanTier;
            int cost = 1000 + 750 * targetClanTier + (isClanLeader ? 1000 : 0);
            if (isRuler) cost *= 2;
            return kind == SchemeKind.Kidnap ? Round(cost * 0.8) : cost;
        }

        public static int BribeCost(int startCost, int level) =>
            Round(startCost * BribeCostFactor[Clamp(level, 0, MaxBribeLevel)]);

        public static int BribeBonus(int level) => BribeSuccess[Clamp(level, 0, MaxBribeLevel)];

        /// <summary>Bir aşamanın gün sayısı: casus başı ustalaştıkça kısalır.</summary>
        public static int PhaseDays(SchemeKind kind, int roguery) =>
            SchemeRules.IsStrike(kind)
                ? Math.Max(4, (int)Math.Ceiling(7 - roguery / 60.0))
                : Math.Max(3, (int)Math.Ceiling(5 - roguery / 100.0));

        /// <summary>Casusluk başarılıysa bir sır bulma olasılığı.</summary>
        public static int SecretChance(int roguery, int completedPhases) =>
            Clamp(Math.Min(80, Round(30 + roguery / 6.0)) + (completedPhases >= 2 ? 20 : 0), 0, 95);

        /// <summary>Başarılı suikastta iz bırakma olasılığı.</summary>
        public static int TraceChance(int discovery, bool targetIsPlayersCaptive, bool accident)
        {
            double value = discovery * 2.0 * (targetIsPlayersCaptive ? 2 : 1) * (accident ? 0.5 : 1);
            return Clamp(Round(value), 1, 90);
        }

        /// <summary>Başarısız suikastta suikastçının yakalanma olasılığı.</summary>
        public static int CaughtOnFailureChance(int roguery) => Math.Max(20, Round(50 - roguery / 10.0));

        public static int MaxActiveSchemes(int roguery) => 1 + (roguery >= 125 ? 1 : 0) + (roguery >= 225 ? 1 : 0);

        /// <summary>İkna zarı (hükümdara inkâr, casus başını ikna): Charm'a bağlı.</summary>
        public static int PersuadeChance(int charm) => Clamp(Round(30 + charm / 5.0), 0, 80);

        public static int BloodPriceGold(int victimClanTier) => 5000 * Math.Max(1, victimClanTier);
        public const int BloodPriceInfluence = 100;

        // ---- Gölge ağı (E2) ----

        public const int MaxNetworkLevel = 3;

        /// <summary>Ağın olduğu şehirdeki hedefe tam bonus, aynı krallıktaki diğer hedeflere yarısı.</summary>
        public static int NetworkSuccess(int level, bool sameTown) => sameTown ? 5 * level : 5 * level / 2;
        public static int NetworkDiscovery(int level, bool sameTown) => sameTown ? 2 * level : level;
        public static int NetworkCost(int level) => level <= 1 ? 3000 : level == 2 ? 6000 : 12000;
        public static int NetworkUpkeep(int level) => 150 * level;
        public static int NetworkRelationRequired(int level) => (level - 1) * 10;
        public static int MaxNetworks(int roguery) => 1 + roguery / 75;
        /// <summary>Haftalık: yapay zekânın karşı casusluğu ağı bulup dağıtır.</summary>
        public static int NetworkExposeChance(int level) => 2 * level;

        // ---- Kozlar (E2) ----

        public const int HookDays = 84;
        public const int HookGoldPercent = 20;
        public const int HookInfluence = 50;
        public const int DefectionInfluenceCost = 200;
        public const int HookUseRelationPenalty = 10;

        // ---- Yapay zekâ entrikaları (E2) ----

        public const int AiOff = 0;
        public const int AiFew = 1;
        public const int AiNormal = 2;
        public const int HatredThreshold = -40;
        public const int FeudDays = 168;
        public const int ProtectionBonus = 25;
        public const int ProtectionCost = 2000;
        public const int AiPlotMinDays = 14;
        public const int AiPlotExtraDays = 21;

        /// <summary>Haftalık yeni yapay zekâ entrikası olasılığı: entrikacı adayı başına Az %3, Normal %6, en fazla %25.</summary>
        public static int AiPlotWeeklyChance(int candidates, int setting) =>
            setting == AiOff ? 0 : Math.Min(25, candidates * (setting == AiNormal ? 6 : 3));

        public static int AiMaxPlots(int setting) => setting == AiNormal ? 2 : setting == AiFew ? 1 : 0;

        /// <summary>Haftalık: casus başın sana karşı süren bir entrikayı sezme olasılığı.</summary>
        public static int DetectionChance(bool hasSpymaster, int roguery) =>
            hasSpymaster ? Math.Min(90, Round(20 + roguery / 5.0)) : 5;

        public static string AiSettingName(int setting) =>
            setting == AiOff ? "Kapalı" : setting == AiNormal ? "Normal" : "Az";

        public static string DifficultyName(int difficulty) =>
            difficulty == EasyDifficulty ? "Kolay" : difficulty == HardDifficulty ? "Zor" : "Normal";

        private static int DifficultySuccess(int difficulty) =>
            difficulty == EasyDifficulty ? 10 : difficulty == HardDifficulty ? -10 : 0;

        private static int DifficultyDiscovery(int difficulty) =>
            difficulty == EasyDifficulty ? -3 : difficulty == HardDifficulty ? 3 : 0;

        public static int Clamp(int value, int min, int max) => value < min ? min : value > max ? max : value;

        private static int Round(double value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Tohumlu zar (SplitMix64). Sonuç yalnızca tohuma ve sayaca bağlı; ikisi de kayda yazılır,
    /// bu yüzden kaydet-yükle aynı zarı verir (tasarım 11.4).
    /// </summary>
    public static class SchemeRandom
    {
        /// <summary>1..100 arası zar; sayaç bir artar.</summary>
        public static int Roll100(int seed, ref int counter) => (int)(Next(seed, counter++) % 100) + 1;

        /// <summary>0..count-1 arası seçim; sayaç bir artar.</summary>
        public static int Pick(int seed, ref int counter, int count) => count <= 0 ? 0 : (int)(Next(seed, counter++) % (ulong)count);

        private static ulong Next(int seed, int counter)
        {
            ulong z = (uint)seed * 0x9E3779B97F4A7C15UL + (uint)counter * 0xD1B54A32D192ED03UL + 0x632BE59BD9B4E019UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
    }
}
