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
        /// <summary>Gece evi: müşterilerden duyulanlar, baskın, skandal.</summary>
        public const string NightHouse = "yer_gece_evi";
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
        };

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

        public static string ForKind(SchemeKind kind) =>
            kind == SchemeKind.Spy ? Spy : kind == SchemeKind.Blackmail ? Blackmail : kind == SchemeKind.Kidnap ? Kidnap : Murder;

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
                         Tail, TailNight, Treasury, Embezzle,
                     })
                yield return name;
            foreach (string name in Events.Values) yield return name;
        }
    }
}
