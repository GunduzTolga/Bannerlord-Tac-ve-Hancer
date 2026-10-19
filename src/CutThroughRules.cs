using System;

namespace CheatMenu
{
    /// <summary>
    /// "Herkesi biç" kuralının oyun türlerinden bağımsız kararları (testlerde de denetlenir). Oyunda bir kılıç darbesi düşmanı
    /// öldürürse durmaz, hızının bir kısmıyla arkadakine geçer. Dostta ve kalkanda durur; tekme/kalkan vuruşu, yumruk ve
    /// dürtme (mızrak ucu) sayılmaz. Uygulama: Cheats.Models.CutThroughDamageModel (oyunun AgentApplyDamageModel'i zincirlenir).
    /// </summary>
    public static class CutThroughRules
    {
        public const int Off = 0, PlayerOnly = 1, Everyone = 2;
        public static readonly string[] ModeNames = { "Kapalı", "Yalnızca ben", "Herkes" };
        public const int DefaultMode = PlayerOnly;

        /// <summary>Her kesişte kılıcın hızından korunan pay (her yeni kesişte yine bu kadarı kalır: 0,7 → 0,49 → ...).</summary>
        public const float RetainedPerCut = 0.7f;

        /// <param name="isPlayer">Vuran oyuncunun kendisi mi.</param>
        /// <param name="isEnemy">Vurulan, vuranın düşmanı mı (dost askerde kılıç durur).</param>
        /// <param name="isFatal">Bu darbe vurulanı öldürdü mü.</param>
        /// <param name="isSwing">Savuruş mu (dürtme değil).</param>
        /// <param name="hasWeapon">Elinde silah var mı (yumruk değil).</param>
        /// <param name="isAlternative">Tekme ya da kalkanla vuruş mu.</param>
        /// <param name="hitShield">Darbe kalkana mı geldi.</param>
        public static bool ShouldCut(int mode, bool isPlayer, bool isEnemy, bool isFatal, bool isSwing, bool hasWeapon, bool isAlternative, bool hitShield)
        {
            if (mode == Off || (mode == PlayerOnly && !isPlayer)) return false;
            return isEnemy && isFatal && isSwing && hasWeapon && !isAlternative && !hitShield;
        }

        /// <summary>Kesişten sonra kalan hız: oyunun hesabı ile korunan pay arasında büyük olanı.</summary>
        public static float RemainingMomentum(float original, float gameRemaining) => Math.Max(gameRemaining, original * RetainedPerCut);
    }
}
