using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Eşinin gizli hayatı ve boşanma (tasarım: docs/superpowers/specs/2026-10-19-p2-esin-gizli-hayati.md). Saf: oyunsuz test edilir.
    /// Eşin kendi başına sevgili edinebilir; casus başın araştırırsa ortaya çıkar; yüzleşir, görmezden gelir ya da boşarsın.
    /// </summary>
    public static class FidelityRules
    {
        /// <summary>Seninle ilişkisi bu kadar ya da iyiyse eşin seni aldatmaz.</summary>
        public const int FaithfulRelation = 50;
        /// <summary>Bu kadar hafta ayrı kalırsanız ihtimal artar.</summary>
        public const int LonelyWeeks = 8;
        public const int MaxWeeklyChance = 8;
        public const int MeetingDays = 14;

        public const int InvestigationDays = 14;
        public const int InvestigationCost = 1000;
        /// <summary>Eşin araştırıldığını fark ederse: seninle ilişki.</summary>
        public const int NoticeChance = 10;
        public const int NoticeRelation = 10;
        /// <summary>Casus başı karşı casusluk görevindeyken haftalık ortaya çıkarma olasılığı.</summary>
        public const int CounterIntelWeeklyChance = 10;

        public const int ConfrontEndRelation = 10;
        public const int ConfrontRefuseRelation = 20;

        /// <summary>Boşanma: eski eşinle ilişki, ailesinin lideriyle (haklı / haksız), haksızsa onurlu lordlarla.</summary>
        public const int DivorceRelation = 40;
        public const int DivorceFamilyJust = 5;
        public const int DivorceFamilyUnjust = 20;
        public const int DivorceHonorablePenalty = 5;

        /// <summary>
        /// Eşinin bu hafta bir sevgili edinme olasılığı (%0-8): seninle ilişkisi 50 ve üstüyse 0; ilişki düştükçe artar, Onur azaltır,
        /// uzun süre ayrı kalmak artırır.
        /// </summary>
        public static int WeeklyAffairChance(int relationWithYou, int honor, int weeksApart)
        {
            if (relationWithYou >= FaithfulRelation) return 0;
            int value = 1 + Math.Max(0, 20 - relationWithYou) / 10 - 2 * honor + (weeksApart >= LonelyWeeks ? 2 : 0);
            return SchemeMath.Clamp(value, 0, MaxWeeklyChance);
        }

        /// <summary>Araştırma ilişki varsa bulur mu: casus başının Roguery'si, %40-95.</summary>
        public static int InvestigationFindChance(int spymasterRoguery) => SchemeMath.Clamp(40 + spymasterRoguery / 4, 40, 95);

        /// <summary>Yüzleşince eşinin ilişkiyi bitirme olasılığı: sana ilişkisi ve Onuru, %10-90.</summary>
        public static int ConfrontEndChance(int relationWithYou, int honor) => SchemeMath.Clamp(50 + relationWithYou / 2 + 10 * honor, 10, 90);

        public static int DivorceFamilyPenalty(bool justified) => justified ? DivorceFamilyJust : DivorceFamilyUnjust;
    }
}
