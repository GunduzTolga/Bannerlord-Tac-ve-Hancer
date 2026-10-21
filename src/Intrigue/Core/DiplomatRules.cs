using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Divan: diplomat (tasarım: docs/superpowers/specs/2026-10-21-divan-diplomat.md). Charm'ına göre: Barış Masası'nda pazarlık gücü,
    /// elçi olasılığı, çağrılan ve ziyafete davet edilen lordların gelme olasılığı, hediyenin ilişkisi; savaş tehdidi raporunun isabeti
    /// ve erken uyarı; "ilişkileri geliştir" görevinin haftalık kazancı. Saf: oyunsuz test edilir.
    /// </summary>
    public static class DiplomatRules
    {
        /// <summary>Barış Masası'nda pazarlık gücüne: Charm/20, en fazla 15.</summary>
        public static int PeaceBonus(int charm) => SchemeMath.Clamp(Math.Max(0, charm) / 20, 0, 15);

        /// <summary>Elçinin haftalık gelme olasılığına (gelebiliyorsa): Charm/30, en fazla 10.</summary>
        public static int EnvoyBonus(int charm) => SchemeMath.Clamp(Math.Max(0, charm) / 30, 0, 10);

        /// <summary>Yanına çağrılan ve ziyafete davet edilen lordun gelme olasılığına: Charm/25, en fazla 12.</summary>
        public static int InviteBonus(int charm) => SchemeMath.Clamp(Math.Max(0, charm) / 25, 0, 12);

        /// <summary>Ulakla giden hediyenin ilişkisine: Charm 100'de +1, 200'de +2.</summary>
        public static int GiftBonus(int charm) => SchemeMath.Clamp(Math.Max(0, charm) / 100, 0, 2);

        /// <summary>"İlişkileri geliştir": haftada 1 + Charm/75, en fazla 4; ilişki MissionCap'e varınca görev biter.</summary>
        public static int RelationPerWeek(int charm) => SchemeMath.Clamp(1 + Math.Max(0, charm) / 75, 1, 4);
        public const int MissionCap = 60;

        /// <summary>Savaş tehdidini yanlış okuma olasılığı: %40 - Charm/6, %5-40.</summary>
        public static int ErrorPercent(int charm) => SchemeMath.Clamp(40 - Math.Max(0, charm) / 6, 5, 40);

        /// <summary>Savaş hazırlığını haftalık fark edip uyarma olasılığı: %30 + Charm/4, en fazla 95.</summary>
        public static int WarningChance(int charm) => SchemeMath.Clamp(30 + Math.Max(0, charm) / 4, 30, 95);
        /// <summary>Aynı krallık için uyarı bu kadar gün tekrarlanmaz.</summary>
        public const int WarningCooldownDays = 14;

        /// <summary>Savaş tehdidi: 0 yok, 1 konuşuluyor, 2 hazırlanıyor (oyunun savaş ilanı puanından).</summary>
        public static int Threat(float warScore) => warScore > 0 ? 2 : warScore > -WarTalkMargin ? 1 : 0;
        /// <summary>Puan sıfırın bu kadar altındaysa sarayda savaş "konuşuluyor" sayılır.</summary>
        public const float WarTalkMargin = 25000f;

        /// <summary>Diplomatın gözüyle tehdit: roll (1-100) hata payının içindeyse bir basamak yanılır (yukarı ya da aşağı).</summary>
        public static int SeenThreat(int threat, int charm, int roll)
        {
            int error = ErrorPercent(charm);
            if (roll > error) return threat;
            int shifted = roll % 2 == 0 ? threat + 1 : threat - 1;
            return SchemeMath.Clamp(shifted, 0, 2);
        }

        public static string ThreatText(int threat) =>
            threat >= 2 ? "sana savaş açmaya hazırlanıyor" : threat == 1 ? "sarayında savaş konuşuluyor" : "barış yanlısı";
    }
}
