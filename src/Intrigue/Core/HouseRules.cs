namespace CheatMenu.Intrigue.Core
{
    /// <summary>Gece evinde çalışanların kazançtan aldığı pay (gönüllü ve ücretli; düşük pay verilirse ayrılırlar).</summary>
    public enum HouseShare
    {
        /// <summary>%25: senin kazancın yüksek, çalışanlar ayrılmaya başlar.</summary>
        Low = 1,
        /// <summary>%40.</summary>
        Standard = 2,
        /// <summary>%60: kimse ayrılmaz, müşterilerden duyulanlar sana daha çok gelir.</summary>
        Fair = 3,
    }

    /// <summary>
    /// Gece evi (genelev) işletmesi: formüller (tasarım: docs/superpowers/specs/2026-10-19-p2-gece-evi.md). Saf: oyunsuz test edilir.
    /// Çalışanlar isimsiz, gönüllü ve ücretli yetişkinlerdir: yalnızca bir sayı. Hiçbir kahraman (esir, eş, aile, yoldaş) çalıştırılamaz.
    /// </summary>
    public static class HouseRules
    {
        public const int OpenCost = 4000;
        public const int HireCost = 300;
        public const int MinStaff = 2;
        /// <summary>İlk açılışta çalışan sayısı.</summary>
        public const int StartStaff = MinStaff;
        /// <summary>Haftalık "duyuldu" olasılığı: kendi şehrinde skandal, başka şehirde baskın.</summary>
        public const int ExposeChance = 3;
        /// <summary>Kendi şehrinde duyulursa onurlu klan liderleriyle ilişki cezası.</summary>
        public const int ScandalPenalty = 2;
        /// <summary>Başka bir şehirde baskın yenince şehrin sahibiyle ilişki cezası ve o krallıkta suç puanı.</summary>
        public const int RaidOwnerPenalty = 10;
        public const int RaidCrime = 10;
        /// <summary>Duyulanlardan bir sır çıkma olasılığı.</summary>
        public const int SecretChance = 25;

        /// <summary>En fazla çalışan: gölge ağının seviyesine göre 4 / 6 / 8.</summary>
        public static int MaxStaff(int networkLevel) => 2 + 2 * SchemeMath.Clamp(networkLevel, 1, SchemeMath.MaxNetworkLevel);

        public static int SharePercent(HouseShare share) => share == HouseShare.Low ? 25 : share == HouseShare.Fair ? 60 : 40;

        public static string ShareName(HouseShare share) =>
            share == HouseShare.Low ? "düşük (%25)" : share == HouseShare.Fair ? "adil (%60)" : "standart (%40)";

        /// <summary>Haftalık toplam kazanç: çalışan başına 40 + müreffeh/100 dinar.</summary>
        public static int WeeklyGross(int staff, float prosperity) =>
            staff <= 0 ? 0 : staff * (40 + (int)(System.Math.Max(0f, prosperity) / 100f));

        /// <summary>Çalışanların payı düşüldükten sonra sana kalan.</summary>
        public static int WeeklyNet(int staff, float prosperity, HouseShare share) =>
            WeeklyGross(staff, prosperity) * (100 - SharePercent(share)) / 100;

        /// <summary>Haftalık bir çalışanın ayrılma olasılığı: düşük payda %15, standartta %5, adilde 0.</summary>
        public static int LeaveChance(HouseShare share) => share == HouseShare.Low ? 15 : share == HouseShare.Standard ? 5 : 0;

        /// <summary>Haftalık "müşteriler konuştu" olasılığı: çalışan başına %5, adil payda +10, en fazla %60.</summary>
        public static int IntelChance(int staff, HouseShare share) =>
            staff <= 0 ? 0 : SchemeMath.Clamp(5 * staff + (share == HouseShare.Fair ? 10 : 0), 0, 60);
    }
}
