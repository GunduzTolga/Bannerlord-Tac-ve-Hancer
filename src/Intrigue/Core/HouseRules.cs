using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>Bir çalışanın öne çıkan yanı: kartta adının yanında yazar, evin kazancına ya da haberlerine etki eder.</summary>
    public enum WorkerTrait
    {
        None = 0,
        /// <summary>Kulağı delik: müşterilerden daha çok şey duyar.</summary>
        Listener = 1,
        /// <summary>Gözde: daha çok kazandırır.</summary>
        Favorite = 2,
        /// <summary>Sadık: düşük payda bile kolay kolay ayrılmaz.</summary>
        Loyal = 3,
    }

    /// <summary>
    /// Genelev işletmesi: formüller (tasarım: docs/superpowers/specs/2026-10-19-p2-genelev.md). Saf: oyunsuz test edilir.
    /// Çalışanlar adlı, gönüllü ve ücretli yetişkinlerdir ama oyunun kahramanı değildir; hiçbir kahraman (esir, eş, aile, yoldaş, lord)
    /// müşteriye çıkarılamaz. Bir yoldaş yalnızca evi işletebilir (yönetici): kendisi kabul eder, kazancın bir payı kendi kesesine gider.
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

        /// <summary>
        /// Haftalık toplam kazanç: çalışan başına 40 + müreffeh/100 dinar; gözdeler bunun yarısı kadar fazla. Yönetici varsa
        /// (managerCharm 0 ya da üstü) üstüne ManagerIncomeBonus.
        /// </summary>
        public static int WeeklyGross(int staff, float prosperity, int favorites = 0, int managerCharm = -1)
        {
            if (staff <= 0) return 0;
            int each = 40 + (int)(System.Math.Max(0f, prosperity) / 100f);
            int gross = staff * each + SchemeMath.Clamp(favorites, 0, staff) * each / 2;
            return gross * (100 + ManagerIncomeBonus(managerCharm)) / 100;
        }

        /// <summary>Çalışanların payı ve (varsa) yöneticinin payı düşüldükten sonra sana kalan.</summary>
        public static int WeeklyNet(int staff, float prosperity, HouseShare share, int favorites = 0, int managerCharm = -1) =>
            WeeklyGross(staff, prosperity, favorites, managerCharm) * (100 - SharePercent(share) - (managerCharm >= 0 ? ManagerCutPercent : 0)) / 100;

        /// <summary>Haftalık bir çalışanın ayrılma olasılığı: düşük payda %15, standartta %5, adilde 0.</summary>
        public static int LeaveChance(HouseShare share) => share == HouseShare.Low ? 15 : share == HouseShare.Standard ? 5 : 0;

        /// <summary>Sadık olan üçte biri kadar ihtimalle ayrılır.</summary>
        public static int LeaveChance(HouseShare share, WorkerTrait trait) => trait == WorkerTrait.Loyal ? LeaveChance(share) / 3 : LeaveChance(share);

        /// <summary>
        /// Haftalık "müşteriler konuştu" olasılığı: çalışan başına %5 (kulağı delik olan %10), adil payda +10, yöneticinin
        /// Roguery'si (managerRoguery 0 ya da üstü) /20, en fazla +10; toplam en fazla %60.
        /// </summary>
        public static int IntelChance(int staff, HouseShare share, int listeners = 0, int managerRoguery = -1) =>
            staff <= 0 ? 0 : SchemeMath.Clamp(5 * staff + 5 * SchemeMath.Clamp(listeners, 0, staff) + (share == HouseShare.Fair ? 10 : 0)
                + (managerRoguery >= 0 ? System.Math.Min(managerRoguery / 20, 10) : 0), 0, 60);

        /// <summary>Haftalık duyulma olasılığı: yöneticinin Roguery'si her 75 puanda bir azaltır, en az %1; yönetici yoksa (-1) ExposeChance.</summary>
        public static int ExposeChanceWith(int managerRoguery) =>
            managerRoguery < 0 ? ExposeChance : SchemeMath.Clamp(ExposeChance - managerRoguery / 75, 1, ExposeChance);

        // ---- Evin hesabı: çalışanların özellikleri ve yönetici birlikte ----

        public static int Favorites(House house) => house.Workers.Count(w => w.Trait == WorkerTrait.Favorite);

        public static int Listeners(House house) => house.Workers.Count(w => w.Trait == WorkerTrait.Listener);

        /// <param name="managerCharm">Yöneticinin Charm'ı; yönetici yoksa -1.</param>
        public static int Gross(House house, float prosperity, int managerCharm) =>
            WeeklyGross(house.Staff, prosperity, Favorites(house), managerCharm);

        public static int Net(House house, float prosperity, int managerCharm) =>
            WeeklyNet(house.Staff, prosperity, house.Share, Favorites(house), managerCharm);

        /// <param name="managerRoguery">Yöneticinin Roguery'si; yönetici yoksa -1.</param>
        public static int Intel(House house, int managerRoguery) => IntelChance(house.Staff, house.Share, Listeners(house), managerRoguery);

        // ---- Çalışanlar: ad, yaş, özellik ----

        /// <summary>Çalışanlar en az bu yaştadır (yetişkin).</summary>
        public const int MinAge = 19;
        public const int MaxHireAge = 34;
        /// <summary>Oyunda bir yıl (CampaignTime.DaysInYear).</summary>
        public const int DaysPerYear = 84;
        /// <summary>Çalışan alırken kapıya gelen aday sayısı.</summary>
        public const int Candidates = 3;

        /// <summary>Bugünkü yaşı: işe girdiği yaş ve o günden beri geçen yıllar.</summary>
        public static int AgeOn(HouseWorker worker, int today) =>
            System.Math.Max(MinAge, worker.Age) + System.Math.Max(0, today - worker.Since) / DaysPerYear;

        public static string TraitName(WorkerTrait trait) =>
            trait == WorkerTrait.Listener ? "kulağı delik" : trait == WorkerTrait.Favorite ? "gözde" : trait == WorkerTrait.Loyal ? "sadık" : "";

        public static string TraitHint(WorkerTrait trait) =>
            trait == WorkerTrait.Listener ? "müşterilerden daha çok şey duyar"
            : trait == WorkerTrait.Favorite ? "daha çok kazandırır"
            : trait == WorkerTrait.Loyal ? "kolay kolay ayrılmaz" : "";

        /// <summary>Kartta: "Lysa, 23 · kulağı delik".</summary>
        public static string WorkerLine(HouseWorker worker, int today)
        {
            string trait = TraitName(worker.Trait);
            return (worker.Name ?? "?") + ", " + AgeOn(worker, today) + (trait.Length > 0 ? " · " + trait : "");
        }

        /// <summary>Şehrin kültüründe ad listesi yoksa kullanılan adlar.</summary>
        public static readonly string[] FallbackFemaleNames = { "Ira", "Lysa", "Mira", "Sera", "Talia", "Elys", "Nadia", "Vesna", "Ilona", "Rhea" };
        public static readonly string[] FallbackMaleNames = { "Adran", "Belos", "Corin", "Davor", "Erent", "Fenn" };

        /// <summary>
        /// Kapıya gelen bir aday: dörtte üçü kadın; ad şehrin kültüründen, evdekilerle aynı olmasın diye birkaç kez denenir; yaş
        /// MinAge-MaxHireAge; özellik %40 yok, kalanı üçe eşit.
        /// </summary>
        /// <param name="pick">0 ile count-1 arasında bir sayı (tohumlu).</param>
        public static HouseWorker NewWorker(IList<string> femaleNames, IList<string> maleNames, Func<int, int> pick, int today,
            ICollection<string> taken)
        {
            bool female = pick(4) != 0;
            IList<string> names = female ? femaleNames : maleNames;
            if (names == null || names.Count == 0) names = female ? FallbackFemaleNames : FallbackMaleNames;
            string name = names[pick(names.Count)];
            for (int tries = 0; tries < names.Count && taken != null && taken.Contains(name); tries++)
                name = names[pick(names.Count)];
            int roll = pick(5);
            return new HouseWorker
            {
                Name = name,
                Female = female,
                Age = MinAge + pick(MaxHireAge - MinAge + 1),
                Trait = roll == 2 ? WorkerTrait.Listener : roll == 3 ? WorkerTrait.Favorite : roll == 4 ? WorkerTrait.Loyal : WorkerTrait.None,
                Since = today,
            };
        }

        /// <summary>Eski kayıttan gelen adsız çalışanlara ad, yaş ve özellik verir (işe giriş günü kalır). Ad verilenlerin sayısı.</summary>
        public static int NameUnnamed(House house, IList<string> femaleNames, IList<string> maleNames, Func<int, int> pick)
        {
            int named = 0;
            foreach (HouseWorker worker in house.Workers.Where(w => w.Name == null).ToList())
            {
                HouseWorker fresh = NewWorker(femaleNames, maleNames, pick, worker.Since, house.Workers.Select(w => w.Name).ToList());
                worker.Name = fresh.Name;
                worker.Female = fresh.Female;
                worker.Age = fresh.Age;
                worker.Trait = fresh.Trait;
                named++;
            }
            return named;
        }

        // ---- Yönetici: evi işleten yoldaş (müşteriye çıkmaz) ----

        /// <summary>Yöneticinin kazançtan aldığı pay: kendi kesesine gider.</summary>
        public const int ManagerCutPercent = 10;

        /// <summary>Yönetici kazancı artırır: Charm/4 yüzde, en fazla +%50; yönetici yoksa (-1) 0.</summary>
        public static int ManagerIncomeBonus(int managerCharm) => managerCharm < 0 ? 0 : System.Math.Min(managerCharm / 4, 50);

        public static int ManagerPay(int gross) => System.Math.Max(0, gross) * ManagerCutPercent / 100;

        /// <summary>
        /// Yoldaş yöneticiliği kabul eder mi? null: kabul; değilse nedeni. Kendi kararıdır: onurlu biri ya da sana güvenmeyen reddeder,
        /// zorlamanın yolu yok. Eşin ya da sevgilin bu işe hiç koşulmaz.
        /// </summary>
        public static string ManagerRefusal(int age, bool isPartner, int honor, int relation)
        {
            if (age < 18) return "yetişkin değil";
            if (isPartner) return "eşin ya da sevgilin: bu ev ona göre değil";
            if (honor >= 1) return "onurlu biri: böyle bir evi işletmez";
            if (relation < 0) return "sana güvenmiyor (ilişki " + relation + ")";
            return null;
        }

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
