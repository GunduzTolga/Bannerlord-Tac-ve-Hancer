using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>Yerleşim entrikası hesabı için gereken her şey, düz sayı olarak. Oyun katmanı (SettlementSchemes) doldurur.</summary>
    public sealed class SettlementSchemeInputs
    {
        // Casus başı
        public int Roguery;
        public int Calculating;

        // Plan
        public int CompletedPhases;
        public int EventSuccess;
        public int EventDiscovery;
        public int BribeLevel;
        public int Difficulty = SchemeMath.NormalDifficulty;

        // Yerleşim
        /// <summary>O yerleşimdeki gölge ağının seviyesi (0: yok).</summary>
        public int NetworkLevel;
        public bool IsCastle;
        public int Security;
        public int Loyalty;
        public int GovernorRoguery;
        public bool Alerted;
    }

    /// <summary>
    /// Sabotaj ve kışkırtma (E3, tasarım: docs/superpowers/specs/2026-10-20-e3-sabotaj-kiskirtma.md). Saf: oyunsuz test edilir;
    /// denge ayarı yalnızca burada yapılır. Yuvarlama SchemeMath'teki gibi: yarım yukarı, sonra sınırlara kırpılır.
    /// </summary>
    public static class SettlementSchemeRules
    {
        public const int SabotageTownCost = 1500;
        public const int SabotageCastleCost = 1000;
        public const int InciteCost = 2500;

        // Sabotaj etkileri
        /// <summary>Ambar yanınca erzakın bu yüzdesi kalır.</summary>
        public const int FoodKeptPercent = 30;
        /// <summary>Kuyular bozulunca garnizondaki sağlıklı askerlerin bu yüzdesi ölür.</summary>
        public const int GarrisonLossPercent = 20;
        /// <summary>Temeller oyulunca her sur bölümünün sağlamlığı bu yüzdeye iner.</summary>
        public const int WallKeptPercent = 50;
        /// <summary>Sağlamlığı bunun altına inen bölüm çöker: gedik açılır.</summary>
        public const int WallBreachBelowPercent = 10;

        // Kışkırtma etkileri
        public const int LoyaltyPerPhase = 3;
        public const int LoyaltyOnSuccess = 20;
        public const int SecurityOnSuccess = 10;
        public const int LoyaltyOnFailure = 5;

        // Ortaya çıkma ve iz bırakma bedelleri (ilişki, suç puanı, casus gerilimi)
        public const int SabotageOwnerPenalty = 10;
        public const int SabotageDiscoveredCrime = 15;
        public const int SabotageDiscoveredTension = 10;
        public const int InciteOwnerPenalty = 15;
        public const int InciteRulerPenalty = 10;
        public const int InciteTracedCrime = 30;
        public const int InciteTracedTension = 20;
        public const int InciteCaughtCrime = 20;
        public const int InciteCaughtTension = 15;

        public static int StartCost(SchemeKind kind, bool isCastle) =>
            kind == SchemeKind.Incite ? InciteCost : isCastle ? SabotageCastleCost : SabotageTownCost;

        /// <summary>Kışkırtma ağ ister (elebaşları yeraltından gelir); sabotajda ağ yalnızca bonus.</summary>
        public static bool NeedsNetwork(SchemeKind kind) => kind == SchemeKind.Incite;

        /// <summary>Oyunda kaleler isyan etmez: kışkırtma yalnızca şehirde.</summary>
        public static bool AllowsCastle(SchemeKind kind) => kind == SchemeKind.Sabotage;

        public static int SabotageSuccess(SettlementSchemeInputs i)
        {
            double value = 20
                + Math.Min(i.Roguery / 8.0, 30)
                + i.Calculating * 3
                + 8 * SchemeMath.Clamp(i.CompletedPhases - SchemeRules.MinPhases(SchemeKind.Sabotage), 0, 2)
                + SchemeMath.Clamp(i.EventSuccess, -15, 15)
                + SchemeMath.BribeBonus(i.BribeLevel)
                + 5 * NetworkLevel(i)
                - Security(i) / 5.0
                - Math.Min(i.GovernorRoguery / 20.0, 10)
                - (i.IsCastle ? 5 : 0)
                - (i.Alerted ? 15 : 0)
                + DifficultySuccess(i.Difficulty);
            return SchemeMath.Clamp(Round(value), 5, 85);
        }

        public static int SabotageDiscovery(SettlementSchemeInputs i)
        {
            double value = 10
                - Math.Min(i.Roguery / 25.0, 8)
                + Security(i) / 20.0
                + Math.Min(i.GovernorRoguery / 30.0, 6)
                + i.EventDiscovery
                - 2 * NetworkLevel(i)
                + DifficultyDiscovery(i.Difficulty);
            return SchemeMath.Clamp(Round(value), 2, 35);
        }

        public static int InciteSuccess(SettlementSchemeInputs i)
        {
            double value = 25
                + Math.Min(i.Roguery / 8.0, 25)
                + i.Calculating * 2
                + 6 * SchemeMath.Clamp(i.CompletedPhases - SchemeRules.MinPhases(SchemeKind.Incite), 0, 2)
                + SchemeMath.Clamp(i.EventSuccess, -15, 15)
                + SchemeMath.BribeBonus(i.BribeLevel)
                + 5 * NetworkLevel(i)
                + (50 - SchemeMath.Clamp(i.Loyalty, 0, 100)) / 2.0
                - Security(i) / 6.0
                - Math.Min(i.GovernorRoguery / 20.0, 10)
                - (i.Alerted ? 15 : 0)
                + DifficultySuccess(i.Difficulty);
            return SchemeMath.Clamp(Round(value), 5, 85);
        }

        public static int InciteDiscovery(SettlementSchemeInputs i)
        {
            double value = 12
                - Math.Min(i.Roguery / 25.0, 8)
                + Security(i) / 15.0
                + Math.Min(i.GovernorRoguery / 30.0, 6)
                + i.EventDiscovery
                - 2 * NetworkLevel(i)
                + DifficultyDiscovery(i.Difficulty);
            return SchemeMath.Clamp(Round(value), 3, 40);
        }

        public static int Success(SchemeKind kind, SettlementSchemeInputs i) =>
            kind == SchemeKind.Incite ? InciteSuccess(i) : SabotageSuccess(i);

        public static int Discovery(SchemeKind kind, SettlementSchemeInputs i) =>
            kind == SchemeKind.Incite ? InciteDiscovery(i) : SabotageDiscovery(i);

        /// <summary>Başarılı vuruşta iz bırakma olasılığı (suikasttaki gibi: yakalanma × 2).</summary>
        public static int TraceChance(int discovery) => SchemeMath.TraceChance(discovery, false, false);

        // ---- Etkiler ----

        public static float FoodAfterSabotage(float food) => Math.Max(0f, food * FoodKeptPercent / 100f);

        /// <summary>Bir asker yığınından ölecek sayı (en az bir kişilik yığından en az biri).</summary>
        public static int GarrisonLoss(int healthy) =>
            healthy <= 0 ? 0 : Math.Max(1, (int)Math.Round(healthy * GarrisonLossPercent / 100.0, MidpointRounding.AwayFromZero));

        public static float WallAfterSabotage(float ratio)
        {
            float after = Math.Max(0f, Math.Min(1f, ratio)) * WallKeptPercent / 100f;
            return after * 100f < WallBreachBelowPercent ? 0f : after;
        }

        public static float LoyaltyAfter(float loyalty, int delta) => Math.Max(0f, Math.Min(100f, loyalty + delta));

        public static SchemeMethod DefaultSabotageMethod => SchemeMethod.SabotageFood;

        public static bool IsSabotageMethod(SchemeMethod method) =>
            method == SchemeMethod.SabotageFood || method == SchemeMethod.SabotageGarrison || method == SchemeMethod.SabotageWalls;

        public static string MethodName(SchemeMethod method)
        {
            switch (method)
            {
                case SchemeMethod.SabotageGarrison: return "garnizon";
                case SchemeMethod.SabotageWalls: return "surlar";
                case SchemeMethod.SabotageFood: return "erzak ambarı";
                default: return "-";
            }
        }

        public static string MethodEffect(SchemeMethod method)
        {
            switch (method)
            {
                case SchemeMethod.SabotageGarrison: return "kuyular bozulur, garnizondaki sağlıklı askerlerin %" + GarrisonLossPercent + "'si ölür";
                case SchemeMethod.SabotageWalls: return "temeller oyulur, her sur bölümü yarıya iner, çok hasarlısı çöker";
                default: return "ambar yanar, erzakın %" + (100 - FoodKeptPercent) + "'i gider";
            }
        }

        private static int NetworkLevel(SettlementSchemeInputs i) => SchemeMath.Clamp(i.NetworkLevel, 0, SchemeMath.MaxNetworkLevel);

        private static int Security(SettlementSchemeInputs i) => SchemeMath.Clamp(i.Security, 0, 100);

        private static int DifficultySuccess(int difficulty) =>
            difficulty == SchemeMath.EasyDifficulty ? 10 : difficulty == SchemeMath.HardDifficulty ? -10 : 0;

        private static int DifficultyDiscovery(int difficulty) =>
            difficulty == SchemeMath.EasyDifficulty ? -3 : difficulty == SchemeMath.HardDifficulty ? 3 : 0;

        private static int Round(double value) => (int)Math.Round(value, MidpointRounding.AwayFromZero);
    }
}
