namespace CheatMenu.Intrigue.Core
{
    /// <summary>Ağ ifşasından doğan diplomatik krizde oyuncunun cevabı (P2.5).</summary>
    public enum CrisisAnswer
    {
        /// <summary>Tazminat öde: suç silinir, gerilim yatışır.</summary>
        Pay = 1,
        /// <summary>İnkâr et: zar; tutarsa hafif, tutmazsa ağır.</summary>
        Deny = 2,
        /// <summary>Reddet: altın yok, ama hükümdar kızar ve gerilim artar.</summary>
        Defy = 3,
    }

    /// <summary>
    /// Ağ ifşasından diplomatik kriz (P2.5, tasarım: docs/superpowers/specs/2026-10-19-p2-diplomatik-kriz.md). Saf: oyunsuz test edilir.
    /// Gerilim krallık başına 0-100 tutulur; eşiği aşınca o krallığın meclisi oyunun kendi savaş kararını oylar.
    /// </summary>
    public static class CrisisRules
    {
        public const int MaxTension = 100;
        /// <summary>İfşa edilen ağın her seviyesi için gerilim.</summary>
        public const int TensionPerLevel = 10;
        public const int TensionDecayPerWeek = 5;
        public const int DenyFailTension = 15;
        public const int DefyTension = 20;
        /// <summary>Bu gerilimde hükümdar savaş meclisini toplayabilir.</summary>
        public const int WarThreshold = 60;
        /// <summary>Meclis toplandıktan sonra gerilim buraya iner (her hafta yeniden önerilmesin).</summary>
        public const int TensionAfterWarProposal = 40;
        /// <summary>Oyunun suç puanı: ağ seviyesi başına.</summary>
        public const int CrimePerLevel = 15;

        public const int PayRulerPenalty = 5;
        public const int DenyRulerPenalty = 5;
        public const int DenyFailRulerPenalty = 20;
        public const int DefyRulerPenalty = 15;
        /// <summary>Kendi krallığında ağ çökerse hükümdarın kızgınlığı (seviye başına).</summary>
        public const int OwnKingdomRulerPenaltyPerLevel = 5;
        /// <summary>Senin yüzünden savaş meclisi toplanınca kendi hükümdarının kızgınlığı.</summary>
        public const int OwnRulerWarAnger = 20;

        /// <summary>Savaş önerecek krallık en fazla bu kadar savaşta olmalı.</summary>
        public const int MaxWarsForProposal = 1;
        /// <summary>Savaş önerecek krallık seninkinin en az bu oranı kadar güçlü olmalı.</summary>
        public const float MinStrengthRatio = 0.5f;

        public static int ExposureTension(int networkLevel) => TensionPerLevel * SchemeMath.Clamp(networkLevel, 1, SchemeMath.MaxNetworkLevel);

        public static int Crime(int networkLevel) => CrimePerLevel * SchemeMath.Clamp(networkLevel, 1, SchemeMath.MaxNetworkLevel);

        /// <summary>Tazminat: 2.500 / 4.000 / 5.500 dinar.</summary>
        public static int Reparations(int networkLevel) => 1000 + 1500 * SchemeMath.Clamp(networkLevel, 1, SchemeMath.MaxNetworkLevel);

        /// <summary>İnkârın tutma olasılığı: casus başının Roguery'si ve senin Charm'ın; büyük ağı inkâr etmek zor.</summary>
        public static int DenyChance(int spymasterRoguery, int charm, int networkLevel, int difficulty)
        {
            int bonus = difficulty == SchemeMath.EasyDifficulty ? 10 : difficulty == SchemeMath.HardDifficulty ? -10 : 0;
            return SchemeMath.Clamp(30 + spymasterRoguery / 5 + charm / 10 - 10 * networkLevel + bonus, 5, 80);
        }

        public static int AddTension(int tension, int delta) => SchemeMath.Clamp(tension + delta, 0, MaxTension);

        /// <summary>Cevaptan sonra gerilim (ifşanın kendi gerilimi zaten eklenmiş).</summary>
        public static int TensionAfter(int tension, int networkLevel, CrisisAnswer answer, bool denySucceeded)
        {
            int exposure = ExposureTension(networkLevel);
            switch (answer)
            {
                case CrisisAnswer.Pay: return AddTension(tension, -exposure);
                case CrisisAnswer.Deny: return denySucceeded ? AddTension(tension, -exposure / 2) : AddTension(tension, DenyFailTension);
                default: return AddTension(tension, DefyTension);
            }
        }

        public static int RulerPenalty(CrisisAnswer answer, bool denySucceeded) =>
            answer == CrisisAnswer.Pay ? PayRulerPenalty
            : answer == CrisisAnswer.Deny ? (denySucceeded ? DenyRulerPenalty : DenyFailRulerPenalty)
            : DefyRulerPenalty;

        public static int Decay(int tension) => SchemeMath.Clamp(tension - TensionDecayPerWeek, 0, MaxTension);

        /// <summary>Eşiği aşmış bir krallığın o hafta savaş meclisini toplama olasılığı (%10-40).</summary>
        public static int WeeklyWarChance(int tension) => tension < WarThreshold ? 0 : SchemeMath.Clamp(tension - 50, 10, 40);

        /// <summary>Meclis toplanabilir mi: eşik, aynı anda fazla savaş yok ve çok zayıf değil.</summary>
        public static bool MayProposeWar(int tension, float theirStrength, float ourStrength, int theirWars) =>
            tension >= WarThreshold && theirWars <= MaxWarsForProposal && theirStrength >= ourStrength * MinStrengthRatio;

        public static string TensionName(int tension) =>
            tension >= WarThreshold ? "savaş eşiğinde" : tension >= 30 ? "gergin" : tension > 0 ? "soğuk" : "yok";
    }
}
