using System.Collections.Generic;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Olay kartı resimlerinin adları: modülün Images klasöründe ad.png (ya da .jpg). Liste ve prompt'lar:
    /// docs/gorseller/olay-kartlari.md. Saf: adların dosya adına uygunluğu test edilir.
    /// </summary>
    public static class CardImages
    {
        public const string Spy = "tur_casusluk";
        public const string SecretFound = "sonuc_sir_bulundu";
        public const string Blackmail = "tur_santaj";
        public const string Murder = "tur_suikast";
        public const string MurderPoison = "sonuc_suikast_zehir";
        public const string MurderDagger = "sonuc_suikast_hancer";
        public const string MurderAccident = "sonuc_suikast_kaza";
        public const string Kidnap = "tur_kacirma";
        public const string Caught = "sonuc_yakalandi";
        public const string Exposed = "sonuc_ifsa";
        public const string LiegeDemand = "sonuc_hukumdar_hesap";
        public const string PlotDetected = "sonuc_entrika_sezildi";
        public const string ShadowNetwork = "yer_golge_agi";
        public const string HookUsed = "sonuc_koz";
        public const string Interrogation = "sonuc_sorgu";
        public const string Messenger = "sonuc_haberci";
        /// <summary>Gizli ilişki ya da gayrimeşru çocuk sırrı bulundu (P2.4).</summary>
        public const string SecretAffair = "sonuc_gizli_bulusma";
        /// <summary>Gizli ilişki ya da gayrimeşru çocuk ifşa edildi: saray skandalı.</summary>
        public const string Scandal = "sonuc_skandal";
        /// <summary>Aldatılan eşe gizlice söylendi.</summary>
        public const string Whisper = "sonuc_fisilti";
        /// <summary>Yabancı krallıkta ağ çökertildi: elçi hesap soruyor (P2.5).</summary>
        public const string Crisis = "sonuc_diplomatik_kriz";
        /// <summary>Casus skandalları yüzünden savaş meclisi toplandı (P2.5).</summary>
        public const string WarCouncil = "sonuc_savas_meclisi";
        /// <summary>Genelev: müşterilerden duyulanlar, baskın, skandal.</summary>
        public const string NightHouse = "yer_genelev";
        /// <summary>Genelev: müşteri olarak bir gece (perde iner).</summary>
        public const string BrothelNight = "olay_genelev_gece";
        /// <summary>Genelev: askerlere bir gece ısmarlamak.</summary>
        public const string BrothelTroops = "olay_genelev_askerler";
        /// <summary>Genelevde duyulanlar ve dedikodu.</summary>
        public const string BrothelGossip = "sonuc_genelev_dedikodu";
        /// <summary>Genelev baskını.</summary>
        public const string BrothelRaid = "olay_genelev_baskin";
        /// <summary>Genelev: kapıya gelen adaylar (çalışan almak).</summary>
        public const string BrothelCandidates = "olay_genelev_adaylar";
        /// <summary>Genelev: evi işleten yoldaş (yönetici).</summary>
        public const string BrothelManager = "sonuc_genelev_yonetici";
        /// <summary>Genelev: evdeki yoldaşlar; kendi isteğiyle çalışan ya da ayrılan yoldaş.</summary>
        public const string BrothelCompanion = "sonuc_genelev_yoldas";
        /// <summary>Hastalık ve tedavisi.</summary>
        public const string Illness = "sonuc_hastalik";
        /// <summary>Esir hizmetine katıldı: bağlılık yemini.</summary>
        public const string Oath = "sonuc_hizmet_yemini";
        /// <summary>Kur: hediyeler, şiirler, mektuplar (P2.6).</summary>
        public const string Courtship = "sonuc_kur";
        /// <summary>Kurdan ya da sevgiliden doğan çocuk (P2.6).</summary>
        public const string Birth = "sonuc_dogum";
        /// <summary>Eşinin gizli ilişkisi ortaya çıktı.</summary>
        public const string Betrayal = "sonuc_aldatma";
        /// <summary>Boşanma.</summary>
        public const string Divorce = "sonuc_bosanma";
        /// <summary>Düello: meydan okuma ve sonuç.</summary>
        public const string Duel = "sonuc_duello";
        /// <summary>Gayrimeşru çocuk meşrulaştırıldı (P2.8).</summary>
        public const string Legitimize = "sonuc_mesrulastirma";
        /// <summary>Saray günü: divan ve dilekçeler (P2.9).</summary>
        public const string CourtDay = "yer_saray_gunu";
        /// <summary>Rehine anlaşması: misafir soylular (P2.10).</summary>
        public const string Hostage = "sonuc_rehine";
        /// <summary>Kendin takip et (P2.11): gündüz ve gece sahneleri.</summary>
        public const string Tail = "olay_takip";
        public const string TailNight = "olay_takip_gece";
        /// <summary>Divan: hazinedarın raporu ve uzaktan atölye emri (P2.12).</summary>
        public const string Treasury = "sonuc_hazine";
        /// <summary>Hazinedarın zimmeti ortaya çıktı.</summary>
        public const string Embezzle = "sonuc_zimmet";
        /// <summary>Divan: saray hekiminin uyarısı (P2.13).</summary>
        public const string Physician = "sonuc_hekim";
        /// <summary>Hekim suikast kurbanını kurtardı.</summary>
        public const string Antidote = "sonuc_panzehir";
        /// <summary>Divan: mareşalin savaş meclisi ve erken uyarısı (P2.14).</summary>
        public const string Marshal = "sonuc_serdar";
        /// <summary>Divan: kâhyanın uyarısı ve uzaktan emir (P2.15).</summary>
        public const string Steward = "sonuc_kahya";
        /// <summary>E3: sabotaj ve kışkırtma aşamaları.</summary>
        public const string Sabotage = "tur_sabotaj";
        public const string Incite = "tur_kiskirtma";
        /// <summary>Sabotaj tuttu: ambar yandı, kuyular bozuldu, sur çöktü.</summary>
        public const string SabotageFood = "sonuc_sabotaj_ambar";
        public const string SabotageGarrison = "sonuc_sabotaj_garnizon";
        public const string SabotageWalls = "sonuc_sabotaj_sur";
        /// <summary>Kışkırtma tuttu: halk meydanda, belki oyunun kendi isyanı.</summary>
        public const string Uprising = "sonuc_isyan";
        /// <summary>Barış Masası: kaybeden tarafın elçisi barış istiyor.</summary>
        public const string Envoy = "sonuc_baris_elcisi";
        /// <summary>Barış Masası: antlaşma imzalandı.</summary>
        public const string Treaty = "sonuc_baris_antlasmasi";
        /// <summary>Divan: diplomatın uyarısı ve başarısı.</summary>
        public const string Diplomat = "sonuc_diplomat";

        /// <summary>Aşamalar arası olayların resimleri (IntrigueEvents'teki olay numarasıyla).</summary>
        public static readonly IReadOnlyDictionary<int, string> Events = new Dictionary<int, string>
        {
            { 1, "olay_rusvetci_hizmetkar" },
            { 2, "olay_ajan_yakalandi" },
            { 3, "olay_zehir_hancer" },
            { 4, "olay_hedef_yola_cikti" },
            { 5, "olay_rakip_suikastci" },
            { 6, "olay_muhafiz_degisimi" },
            { 7, "olay_vicdan_azabi" },
            { 8, "olay_supheli_tuccar" },
            { 9, "olay_ziyafet" },
            { 10, "olay_cete_teklifi" },
            // E3: yerleşim entrikalarının olayları (SettlementSchemes)
            { 101, "olay_rusvetci_nobetci" },
            { 102, "olay_elebasi_yakalandi" },
            { 103, "olay_kacakci_tuneli" },
            { 104, "olay_ekmek_kuyrugu" },
        };

        /// <summary>
        /// Resmi henüz yoksa yerine gösterilecek, aynı konudan bir resim (ör. genelev kartları için genelevin dış cephesi). Kart önce
        /// kendi resmini, yoksa bunu, o da yoksa bunun yedeğini dener; hiçbiri yoksa kişinin portresi. Resim eklenince kendi resmi gelir.
        /// </summary>
        public static readonly IReadOnlyDictionary<string, string> Fallbacks = new Dictionary<string, string>
        {
            { BrothelNight, NightHouse },
            { BrothelTroops, NightHouse },
            { BrothelGossip, NightHouse },
            { BrothelRaid, NightHouse },
            { BrothelCandidates, NightHouse },
            { BrothelManager, NightHouse },
            { BrothelCompanion, NightHouse },
            { Illness, Physician },
            { Antidote, MurderPoison },
            { WarCouncil, Marshal },
            { Embezzle, Treasury },
            { TailNight, Tail },
            { SabotageFood, Sabotage },
            { SabotageGarrison, Sabotage },
            { SabotageWalls, Sabotage },
            { Uprising, Incite },
            { Envoy, Messenger },
            { Treaty, Envoy },
            { Diplomat, Messenger },
            { "olay_rusvetci_nobetci", "olay_rusvetci_hizmetkar" },
            { "olay_elebasi_yakalandi", "olay_ajan_yakalandi" },
            { "olay_kacakci_tuneli", Sabotage },
            { "olay_ekmek_kuyrugu", Incite },
        };

        /// <summary>Kartın denenecek resimleri: kendisi, sonra yedekleri (döngüye girmez).</summary>
        public static IEnumerable<string> WithFallbacks(string name)
        {
            var seen = new HashSet<string>();
            while (!string.IsNullOrEmpty(name) && seen.Add(name))
            {
                yield return name;
                name = Fallbacks.TryGetValue(name, out string next) ? next : null;
            }
        }

        /// <summary>Bir karta en fazla bu kadar resim sürümü: ad, ad_2 ... ad_5. Oyun var olanlardan rastgele birini gösterir.</summary>
        public const int MaxVariants = 5;

        public static string Variant(string name, int index) => index <= 1 ? name : name + "_" + index;

        public static IEnumerable<string> Variants(string name)
        {
            for (int i = 1; i <= MaxVariants; i++) yield return Variant(name, i);
        }

        /// <summary>Dosya adının ait olduğu kart resmi ("sonuc_kur_3" → "sonuc_kur"); kodda kullanılmayan bir adsa null.</summary>
        public static string BaseOf(string file)
        {
            var names = new HashSet<string>(All());
            if (names.Contains(file)) return file;
            int underscore = file.LastIndexOf('_');
            if (underscore <= 0 || !int.TryParse(file.Substring(underscore + 1), out int index) || index < 2 || index > MaxVariants
                || file.Substring(underscore + 1) != index.ToString(System.Globalization.CultureInfo.InvariantCulture))
                return null;
            string name = file.Substring(0, underscore);
            return names.Contains(name) ? name : null;
        }

        public static string ForEvent(int eventId) => Events.TryGetValue(eventId, out string name) ? name : null;

        public static string ForKind(SchemeKind kind)
        {
            switch (kind)
            {
                case SchemeKind.Spy: return Spy;
                case SchemeKind.Blackmail: return Blackmail;
                case SchemeKind.Kidnap: return Kidnap;
                case SchemeKind.Sabotage: return Sabotage;
                case SchemeKind.Incite: return Incite;
                default: return Murder;
            }
        }

        public static string ForSabotage(SchemeMethod method) =>
            method == SchemeMethod.SabotageGarrison ? SabotageGarrison : method == SchemeMethod.SabotageWalls ? SabotageWalls : SabotageFood;

        /// <summary>Gizli infazın sonucu: hastalık zehir, kaza kaza, firar hançer resmiyle.</summary>
        public static string ForPrisonDeath(PrisonDeathMethod method) =>
            method == PrisonDeathMethod.Illness ? MurderPoison : method == PrisonDeathMethod.Accident ? MurderAccident : MurderDagger;

        public static string ForMurderMethod(SchemeMethod method) =>
            method == SchemeMethod.Poison ? MurderPoison : method == SchemeMethod.Accident ? MurderAccident : MurderDagger;

        /// <summary>Kodda kullanılan bütün resim adları (belgedeki listeyle aynı olmalı).</summary>
        public static IEnumerable<string> All()
        {
            foreach (string name in new[]
                     {
                         Spy, SecretFound, Blackmail, Murder, MurderPoison, MurderDagger, MurderAccident, Kidnap, Caught, Exposed,
                         LiegeDemand, PlotDetected, ShadowNetwork, HookUsed, Interrogation, Messenger, SecretAffair, Scandal, Whisper,
                         Crisis, WarCouncil, NightHouse, Oath, Courtship, Birth, Betrayal, Divorce, Duel, Legitimize, CourtDay, Hostage,
                         Tail, TailNight, Treasury, Embezzle, Physician, Antidote, Marshal, Steward,
                         BrothelNight, BrothelTroops, BrothelGossip, BrothelRaid, BrothelCandidates, BrothelManager, BrothelCompanion, Illness,
                         Sabotage, Incite, SabotageFood, SabotageGarrison, SabotageWalls, Uprising, Envoy, Treaty, Diplomat,
                     })
                yield return name;
            foreach (string name in Events.Values) yield return name;
        }
    }
}
