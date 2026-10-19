namespace CheatMenu
{
    /// <summary>
    /// Aç/kapa hilelerin durumu. Bilerek statik ve save'e yazılmıyor:
    /// mod kaldırılsa bile kayıt dosyaları bu moda bağımlı hâle gelmez.
    /// </summary>
    public static class CheatState
    {
        public static bool GodModeMe;
        public static bool GodModeArmy;
        public static bool OneHitMe;
        public static bool OneHitArmy;
        public static bool InfiniteAmmoMe;
        public static bool InfiniteAmmoArmy;
        public static bool FastParty;
        public static bool RevealMap;
        public static bool ArmyIncludesAllies;
        public static bool IntrigueNeverDiscovered;
        public static bool IntrigueAlwaysSucceed;

        public static void ResetAll()
        {
            RevealMap = false;
            ArmyIncludesAllies = false;
            GodModeMe = GodModeArmy = false;
            OneHitMe = OneHitArmy = false;
            InfiniteAmmoMe = InfiniteAmmoArmy = false;
            FastParty = false;
            IntrigueNeverDiscovered = IntrigueAlwaysSucceed = false;
        }
    }
}
