using System;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Krallık sekmesinin hesapları (tasarım: docs/superpowers/specs/2026-10-21-krallik-sekmesi.md). Saf: oyunsuz test edilir.
    /// </summary>
    public static class RealmRules
    {
        /// <summary>
        /// Bağışın ilişki karşılığı (Diplomacy'nin bağış penceresinin formülü): altın nüfuza çevrilir (DenarsToInfluence), nüfuz da oyunun
        /// "klanı destekle" oranıyla ilişkiye (destek ilişkisi / destek nüfuz bedeli). Kendi klanına 0.
        /// </summary>
        public static int DonationRelation(int amount, float denarsToInfluence, int relationPerSupport, int influencePerSupport)
        {
            if (amount <= 0 || influencePerSupport <= 0) return 0;
            return (int)Math.Round(amount * denarsToInfluence * relationPerSupport / (float)influencePerSupport);
        }
    }
}
