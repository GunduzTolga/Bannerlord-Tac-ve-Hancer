using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Esiri hizmetine davet (tasarım: docs/superpowers/specs/2026-10-19-p2-esir-sorgulama.md, "Hizmetine davet"). Saf: oyunsuz test edilir.
    /// Esir kabul ederse klanına katılır; etmezse esir kalır (fidye ve serbest bırakma oyunun kendi sistemi).
    /// </summary>
    public static class RecruitRules
    {
        /// <summary>Reddeden esire bu kadar gün yeniden teklif edilmez.</summary>
        public const int CooldownDays = 30;
        /// <summary>Kabul edince seninle ilişki.</summary>
        public const int JoinRelation = 10;
        /// <summary>Kabul edince eski klan lideri: onunla ve seninle ilişkisi.</summary>
        public const int FormerLeaderPenalty = 20;
        public const int FormerLeaderPlayerPenalty = 10;

        /// <summary>
        /// Kabul olasılığı: seninle ilişki, kendi klan lideriyle arası (hoşnutsuzsa kolay), Onur (sadık kalır), Hesapçı (fırsatı tartar),
        /// senin Charm'ın ve iki klanın kademesi. %5-75.
        /// </summary>
        public static int Chance(int relationWithYou, int relationWithOwnLeader, int honor, int calculating, int charm,
            int yourClanTier, int theirClanTier, int difficulty)
        {
            double loyalty = relationWithOwnLeader < 0 ? Math.Min(20.0, -relationWithOwnLeader / 2.0) : -Math.Min(15.0, relationWithOwnLeader / 4.0);
            double value = 10 + relationWithYou / 2.0 + loyalty - 10 * honor + 5 * calculating + charm / 10.0
                + 5 * (yourClanTier - theirClanTier)
                + (difficulty == SchemeMath.EasyDifficulty ? 10 : difficulty == SchemeMath.HardDifficulty ? -10 : 0);
            return SchemeMath.Clamp((int)Math.Round(value), 5, 75);
        }
    }
}
