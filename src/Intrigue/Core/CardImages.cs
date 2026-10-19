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
                         Crisis, WarCouncil, NightHouse,
                     })
                yield return name;
            foreach (string name in Events.Values) yield return name;
        }
    }
}
