namespace CheatMenu.Intrigue.Core
{
    /// <summary>Genelevde çalışanların kazançtan aldığı pay (gönüllü ve ücretli; düşük pay verilirse ayrılırlar).</summary>
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
    /// Genelev işletmesi: formüller (tasarım: docs/superpowers/specs/2026-10-19-p2-genelev.md). Saf: oyunsuz test edilir.
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

        // ---- Müşteri olarak: herhangi bir şehrin genelevinde bir gece (yalnızca metin, perde iner) ----

        public const int VisitPlainCost = 150;
        public const int VisitFineCost = 400;
        /// <summary>Kendi genelevinde yarı fiyat.</summary>
        public static int VisitCost(bool fine, bool ownHouse) => (fine ? VisitFineCost : VisitPlainCost) * (ownHouse ? 50 : 100) / 100;
        /// <summary>Gece boyunca şehirdeki bir lord hakkında konuşulanların kulağına gelmesi: iyi odada %25, sade odada %10.</summary>
        public static int VisitIntelChance(bool fine) => fine ? 25 : 10;
        /// <summary>Hastalık kapma: sade odada %12, iyi odada %3.</summary>
        public static int VisitIllnessChance(bool fine) => fine ? 3 : 12;
        /// <summary>Eşin duyar: aynı şehirde ya da aynı partideyse %40, değilse %15; aranız bu kadar bozulur.</summary>
        public const int SpouseHearsChance = 15;
        public const int SpouseHearsNearChance = 40;
        public const int SpouseHearsPenalty = 10;

        // ---- Askerlere bir gece (moral) ----

        /// <summary>Asker başına 3 dinar, en az 100.</summary>
        public static int TroopNightCost(int troops) => System.Math.Max(100, 3 * System.Math.Max(0, troops));
        public const int TroopNightMorale = 12;
        public const int TroopNightCooldownDays = 7;

        // ---- Hastalık ----

        /// <summary>Tedavisiz geçme süresi.</summary>
        public const int IllnessDays = 30;
        /// <summary>Hastayken can bu yüzdenin üstüne çıkmaz.</summary>
        public const int IllnessHealthCapPercent = 70;
        public static int HealthCap(int maxHitPoints) => System.Math.Max(1, maxHitPoints * IllnessHealthCapPercent / 100);
        /// <summary>Saray hekimi tedavi eder: Medicine 0'da 15 gün, her 20 puan bir gün kısaltır, en az 3 gün.</summary>
        public static int PhysicianCureDays(int medicine) => SchemeMath.Clamp(15 - System.Math.Max(0, medicine) / 20, 3, 15);
        /// <summary>Şehrin hekimi: bu ücretle, bu kadar günde.</summary>
        public const int DoctorCost = 500;
        public const int DoctorCureDays = 3;

        /// <summary>Hastalığın geçeceği gün: kendiliğinden, saray hekimiyle ya da şehrin hekimiyle (hangisi önceyse).</summary>
        public static int IllnessEnd(int since, int until, int physicianMedicine)
        {
            if (until <= 0) return 0;
            return physicianMedicine < 0 ? until : System.Math.Min(until, since + PhysicianCureDays(physicianMedicine));
        }
    }
}
