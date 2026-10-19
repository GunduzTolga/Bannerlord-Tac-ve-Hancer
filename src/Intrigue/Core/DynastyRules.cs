namespace CheatMenu.Intrigue.Core
{
    /// <summary>Varis ve meşrulaştırma (P2.8, tasarım: docs/superpowers/specs/2026-10-19-p2-varis.md). Saf: oyunsuz test edilir.</summary>
    public static class DynastyRules
    {
        /// <summary>Meşrulaştırmanın bedeli: krallıktaysan etki (hükümdar onaylar), değilsen altın (kâtipler, tanıklar, kilise).</summary>
        public const int LegitimizeInfluence = 50;
        public const int LegitimizeGold = 2000;
        /// <summary>Babalığını açıklayınca çocuğun annesi seninle: kendisinin skandalı olur.</summary>
        public const int MotherEmbarrassment = 20;

        public static int LegitimizeInfluenceCost(bool inKingdom) => inKingdom ? LegitimizeInfluence : 0;
        public static int LegitimizeGoldCost(bool inKingdom) => inKingdom ? 0 : LegitimizeGold;
    }
}
