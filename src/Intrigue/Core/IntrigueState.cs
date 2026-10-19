using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    public enum SchemeMethod
    {
        None = 0,
        Poison = 1,
        Dagger = 2,
        Accident = 3,
    }

    /// <summary>Sürmekte olan bir entrika.</summary>
    public sealed class Scheme
    {
        public int Id;
        public SchemeKind Kind;
        public string TargetId;
        public int StartDay;
        public int Phase;
        public int DaysIntoPhase;
        public int PhaseDays;
        public int StartCost;

        // Tohumlu zar (tasarım 11.4)
        public int Seed;
        public int Rolls;

        // Olayların birikmiş etkisi
        public int EventSuccess;
        public int EventDiscovery;
        public List<int> UsedEvents = new List<int>();
        public SchemeMethod Method;
        public bool FeastRisk;

        // Oyuncudan karar bekleyen durumlar (popup'lar buradan üretilir; kayda yazıldığı için yüklemeden sonra da sorulur)
        public int PendingEvent;
        public bool AwaitingChoice;
        public bool AskedToExecute;

        public int Roll100() => SchemeRandom.Roll100(Seed, ref Rolls);
        public int Pick(int count) => SchemeRandom.Pick(Seed, ref Rolls, count);
    }

    /// <summary>Casusluk raporu: hedefin o günkü durumunun anlık görüntüsü.</summary>
    public sealed class IntelReport
    {
        public string TargetId;
        public string TargetName;
        public int Day;
        public string Location;
        public string Party;
        public string Gold;
        public string Relations;
        public string Traits;
    }

    public sealed class Secret
    {
        public string TargetId;
        public SecretKind Kind;
        public int Day;
        public bool Exposed;
        /// <summary>Gizli ilişkinin öbür tarafı (P2.4); eski kayıtlarda ya da diğer sırlarda null.</summary>
        public string OtherId;
        /// <summary>Gayrimeşru çocuk sırrında çocuk.</summary>
        public string ChildId;
        /// <summary>Herkese değil, yalnızca aldatılan eşe söylendi (Exposed da true: sır kullanıldı).</summary>
        public bool Whispered;
    }

    /// <summary>Haritaya dönünce gösterilecek bilgi kartı. HeroId varsa portresi, Image varsa resmi (Module/Images) görünür.</summary>
    public sealed class Notice
    {
        public string Title;
        public string Text;
        public string HeroId;
        public string Image;
    }

    /// <summary>"Hükümdar hesap soruyor" kararı (tasarım 5.5).</summary>
    public sealed class LiegeDemand
    {
        public string KingdomId;
        public string VictimName;
        public int Gold;
        public int Influence;
        /// <summary>Ölüm değil kaçırma yüzünden mi.</summary>
        public bool Kidnap;
    }

    public sealed class LogEntry
    {
        public int Day;
        public string Text;
    }

    /// <summary>Şantajla kazanılan koz (CK3 "hook"): süresi dolana ya da harcanana kadar geçerli.</summary>
    public sealed class Hook
    {
        public string TargetId;
        public int Day;
        public int Until;
    }

    /// <summary>Yapay zekânın oyuncu klanından birine kurduğu entrika (tasarım 8.4).</summary>
    public sealed class AiPlot
    {
        public int Id;
        public string PlotterId;
        public string TargetId;
        public SchemeKind Kind;
        public int StartDay;
        public int ResolveDay;
        public int Seed;
        public int Rolls;
        /// <summary>Casus başın sezdi mi; sezildiyse oyuncuya bir kez sorulur (Asked).</summary>
        public bool Detected;
        public bool Asked;
        public bool Protected;

        public int Roll100() => SchemeRandom.Roll100(Seed, ref Rolls);
    }

    /// <summary>Bir şehirde çete lideri üzerinden kurulan muhbir ağı (tasarım 8.3).</summary>
    public sealed class ShadowNetwork
    {
        public string SettlementId;
        public int Level;
        public int Since;
    }

    /// <summary>Yoldaki mektup (P2.2): varış gününde sonuçlanır.</summary>
    public sealed class Letter
    {
        public int Id;
        public LetterKind Kind;
        public string TargetId;
        /// <summary>Koz talebinde HookActions.Use değeri.</summary>
        public int Use;
        public int SentDay;
        public int ArriveDay;
    }

    /// <summary>Rehine anlaşması (P2.10): verdiğin ve/veya tuttuğun misafir. Biri null olabilir (tek taraflı teminat).</summary>
    public sealed class HostageDeal
    {
        public string KingdomId;
        /// <summary>Senin klanından, onların sarayında.</summary>
        public string GivenId;
        /// <summary>Onların klanından, senin yanında.</summary>
        public string HeldId;
        public int Since;
        public int Until;
        /// <summary>Misafirlerin şimdiye kadar kazandığı yakınlık.</summary>
        public int Fostered;
    }

    /// <summary>Süren bir kur (P2.6): sen ya da eşin, hedefe üç aşamada.</summary>
    public sealed class Courtship
    {
        public int Id;
        public string TargetId;
        /// <summary>Kur yapan: sen ya da eşin.</summary>
        public string AgentId;
        public CourtshipMode Mode;
        public int Stage;
        public int DaysIntoStage;
        public int StartDay;
        /// <summary>Tedbirli olsunlar: gebelik olasılığı düşük.</summary>
        public bool Careful;
    }

    /// <summary>Sevgilin (P2.6). Public: ilişki duyuldu.</summary>
    public sealed class Lover
    {
        public string HeroId;
        public int Since;
        public int NextMeetingDay;
        public bool Public;
    }

    /// <summary>Kurdan ya da sevgiliden gebelik (oyunun kendi gebeliği yalnızca eşten olur; bu ayrı izlenir).</summary>
    public sealed class Pregnancy
    {
        public string MotherId;
        /// <summary>Gerçek baba.</summary>
        public string FatherId;
        public int DueDay;
        /// <summary>Eşinin gizli ilişkisinden: gerçek baba sen öğrenene kadar gizli.</summary>
        public bool Hidden;
    }

    /// <summary>Eşinin gizli ilişkisi (yapay zekâ kendi başlatır). Discovered: öğrendin; Asked: kararı soruldu.</summary>
    public sealed class SpouseAffair
    {
        public string SpouseId;
        public string LoverId;
        public int Since;
        public int NextMeetingDay;
        public bool Discovered;
        public bool Asked;
        /// <summary>Nasıl ortaya çıktı (kartta yazılır).</summary>
        public string How;
        /// <summary>Bu ilişkiden doğan çocuklar (öğrenince ortaya çıkar).</summary>
        public List<string> ChildIds = new List<string>();
    }

    /// <summary>Gölge ağı olan bir şehirdeki genelev. Çalışanlar isimsiz ve gönüllü: yalnızca sayı (HouseRules).</summary>
    public sealed class House
    {
        public string TownId;
        public int Staff;
        public HouseShare Share = HouseShare.Standard;
        public int Since;
    }

    /// <summary>Yabancı bir krallıkta gölge ağı çökertildi; hükümdarı hesap soruyor (P2.5). Cevaplanana kadar kartla sorulur.</summary>
    public sealed class Crisis
    {
        public string KingdomId;
        public string TownId;
        public int Level;
        public int Day;
        /// <summary>Eklenen suç puanı (tazminat bunu siler).</summary>
        public int Crime;
        /// <summary>Şehrin sahibiyle ilişki cezası uygulandıysa onun kimliği (kartta yazılır).</summary>
        public string OwnerId;
    }

    /// <summary>Ulağı kabul eden ve görüşmeye hazır lord (P2.2): süresi dolana kadar konuşma penceresi açılabilir.</summary>
    public sealed class Audience
    {
        public string HeroId;
        public int Until;
        /// <summary>Hediyenin getirdiği ilişki (kartta gösterilir).</summary>
        public int Relation;
        /// <summary>"Ulak döndü" kartı gösterildi mi.</summary>
        public bool Asked;
    }

    /// <summary>
    /// Entrika sisteminin kayda yazılan tüm durumu. Saf veri: oyun nesnesi tutmaz, kahramanlar StringId ile anılır.
    /// Metne TextRecords ile yazılır; okurken bozuk ya da eksik kayıtlar atlanır, asla istisna atmaz.
    /// </summary>
    public sealed class IntrigueState
    {
        public const int CurrentVersion = 1;
        /// <summary>Kronik: kayıt dosyası şişmesin diye en yeni bu kadar olay tutulur (P1: 20'den yükseltildi).</summary>
        public const int MaxLogEntries = 300;

        public string SpymasterId;
        public int NextSchemeId = 1;
        public int Seed;
        public int Rolls;

        public List<Scheme> Schemes = new List<Scheme>();
        public List<IntelReport> Reports = new List<IntelReport>();
        public List<Secret> Secrets = new List<Secret>();
        public Dictionary<string, int> CooldownUntil = new Dictionary<string, int>();
        public Dictionary<string, int> AlertUntil = new Dictionary<string, int>();
        public List<Notice> Notices = new List<Notice>();
        public LiegeDemand LiegeDemand;
        public List<LogEntry> Log = new List<LogEntry>();

        // E2
        public List<Hook> Hooks = new List<Hook>();
        public List<AiPlot> AiPlots = new List<AiPlot>();
        public int NextPlotId = 1;
        public List<ShadowNetwork> Networks = new List<ShadowNetwork>();
        /// <summary>Kan davası: kahraman → bitiş günü. Bu süre boyunca nefret eşiğine bakılmadan entrika adayıdır.</summary>
        public Dictionary<string, int> FeudUntil = new Dictionary<string, int>();

        // P2.1
        /// <summary>Esir sorgusu: kahraman → yeniden sorgulanabileceği gün.</summary>
        public Dictionary<string, int> InterrogatedUntil = new Dictionary<string, int>();
        /// <summary>Hizmet teklifini reddeden esir → yeniden teklif edilebileceği gün.</summary>
        public Dictionary<string, int> RecruitUntil = new Dictionary<string, int>();

        // P2.2
        public List<Letter> Letters = new List<Letter>();
        public int NextLetterId = 1;
        public List<Audience> Audiences = new List<Audience>();
        /// <summary>Hediye: kahraman → hediyeden yeniden ilişki gelebileceği gün.</summary>
        public Dictionary<string, int> GiftUntil = new Dictionary<string, int>();
        /// <summary>Casus skandallarından krallıklarla gerilim, 0-100 (P2.5).</summary>
        public Dictionary<string, int> Tension = new Dictionary<string, int>();
        public List<Crisis> Crises = new List<Crisis>();
        public List<House> Houses = new List<House>();
        public List<Courtship> Courtships = new List<Courtship>();
        public List<Lover> Lovers = new List<Lover>();
        public List<Pregnancy> Pregnancies = new List<Pregnancy>();
        /// <summary>Kuru reddeden hedef ya da teklifi reddeden eş → yeniden sorulabileceği gün.</summary>
        public Dictionary<string, int> RomanceUntil = new Dictionary<string, int>();
        public int NextCourtshipId = 1;
        public SpouseAffair SpouseAffair;
        /// <summary>Casus başının eşini araştırması bu gün biter (0: araştırma yok).</summary>
        public int SpouseWatchUntil;
        /// <summary>Eşinle kaç haftadır ayrısınız (ayrı partide, ayrı yerde).</summary>
        public int SpouseApartWeeks;
        /// <summary>Aile editörü (P2.7): "çocuk istemiyorlar" (çiftten biri yeter) ve yaşlanması dondurulanlar.</summary>
        public HashSet<string> NoChildren = new HashSet<string>();
        public HashSet<string> AgeFrozen = new HashSet<string>();
        /// <summary>Düello → aynı kişiye yeniden meydan okunabileceği gün.</summary>
        public Dictionary<string, int> DuelUntil = new Dictionary<string, int>();
        /// <summary>Kendin takip et (P2.11) → aynı kişiyi yeniden takip edebileceğin gün.</summary>
        public Dictionary<string, int> TailUntil = new Dictionary<string, int>();

        // P2.12: divan, hazinedar
        public string TreasurerId;
        public List<CaravanBook> CaravanBooks = new List<CaravanBook>();
        public List<CaravanLeg> CaravanLegs = new List<CaravanLeg>();
        /// <summary>Hazinedarın şimdiye kadar zimmete geçirdiği (ortaya çıkana ya da hesabı sorulana kadar).</summary>
        public int Embezzled;
        /// <summary>Zimmet ortaya çıktı; hesabı sorulmayı bekliyor.</summary>
        public bool EmbezzleFound;

        // P2.13: divan, hekim
        public string PhysicianId;
        /// <summary>Hekimin "sağlığın kötüye gidiyor" uyarısı bu güne kadar tekrarlanmaz.</summary>
        public int PhysicianWarnUntil;
        /// <summary>Klanındaki annelerin (oyunun kendi gebeliği) gebe kaldığı gün: doğuma kalan süre için.</summary>
        public Dictionary<string, int> Conceived = new Dictionary<string, int>();

        // P2.14: divan, serdar
        public string MarshalId;
        /// <summary>Uyarısı sorulmuş "ordu|hedef" çiftleri → yeniden sorulabileceği gün.</summary>
        public Dictionary<string, int> MarshalWarned = new Dictionary<string, int>();

        // P2.15: divan, kâhya
        public string StewardId;
        /// <summary>Kâhyanın uyarıları: "yerleşim|tür" → yeniden uyarabileceği gün.</summary>
        public Dictionary<string, int> StewardWarned = new Dictionary<string, int>();
        /// <summary>Saray günü: yerleşim → yeniden divan kurulabileceği gün.</summary>
        public Dictionary<string, int> CourtDayUntil = new Dictionary<string, int>();
        public List<HostageDeal> Hostages = new List<HostageDeal>();

        // Genelev (müşteri olarak) ve hastalık
        /// <summary>Genelevde yeniden gece geçirilebileceği gün (günde bir kez).</summary>
        public int BrothelVisitUntil;
        /// <summary>Askerlere yeniden bir gece ısmarlanabileceği gün.</summary>
        public int TroopNightUntil;
        /// <summary>Hastalık: başladığı gün ve kendiliğinden geçeceği gün (0: hasta değil). Tedavi bitişi öne çeker.</summary>
        public int IllnessSince;
        public int IllnessUntil;

        public bool IsIll(int today) => IllnessUntil > today;

        public HostageDeal HostageDealWith(string kingdomId) => Hostages.FirstOrDefault(h => h.KingdomId == kingdomId);

        public Lover LoverOf(string heroId) => Lovers.FirstOrDefault(l => l.HeroId == heroId);

        public House HouseIn(string townId) => Houses.FirstOrDefault(h => h.TownId == townId);

        public int TensionWith(string kingdomId) => kingdomId != null && Tension.TryGetValue(kingdomId, out int t) ? t : 0;

        // P2.3
        public SpymasterMission Mission;
        /// <summary>Söylenti görevinin hedefi.</summary>
        public string RumorTargetId;

        /// <summary>E0 kayıt testi: oyuncunun bıraktığı işaret; kaydet-yükle sonrası durmalı.</summary>
        public string SaveTestMarker { get; set; }

        public int Roll100() => SchemeRandom.Roll100(Seed, ref Rolls);

        public Scheme FindScheme(string targetId) => Schemes.FirstOrDefault(s => s.TargetId == targetId);

        public bool IsOnCooldown(string targetId, int today) => CooldownUntil.TryGetValue(targetId, out int until) && today < until;
        public bool IsAlerted(string targetId, int today) => AlertUntil.TryGetValue(targetId, out int until) && today < until;
        public bool HasFeud(string heroId, int today) => FeudUntil.TryGetValue(heroId, out int until) && today < until;

        /// <summary>Hedef üzerindeki geçerli koz; yoksa null.</summary>
        public Hook ActiveHook(string targetId, int today) => Hooks.FirstOrDefault(h => h.TargetId == targetId && today < h.Until);

        public ShadowNetwork Network(string settlementId) => Networks.FirstOrDefault(n => n.SettlementId == settlementId);

        /// <summary>Hedef hakkındaki en yeni rapor; geçerlilik süresine bakmaz.</summary>
        public IntelReport LatestReport(string targetId) =>
            Reports.Where(r => r.TargetId == targetId).OrderByDescending(r => r.Day).FirstOrDefault();

        public bool HasValidReport(string targetId, int today)
        {
            IntelReport report = LatestReport(targetId);
            return report != null && today - report.Day <= SchemeMath.ReportValidDays;
        }

        public void AddLog(int day, string text)
        {
            Log.Add(new LogEntry { Day = day, Text = text });
            if (Log.Count > MaxLogEntries) Log.RemoveRange(0, Log.Count - MaxLogEntries);
        }

        /// <summary>Süresi dolmuş bekleme ve tetikte kayıtlarını siler (kayıt şişmesin).</summary>
        public void RemoveExpired(int today)
        {
            foreach (string id in CooldownUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) CooldownUntil.Remove(id);
            foreach (string id in AlertUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) AlertUntil.Remove(id);
            foreach (string id in FeudUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) FeudUntil.Remove(id);
            foreach (string id in InterrogatedUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) InterrogatedUntil.Remove(id);
            foreach (string id in RecruitUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) RecruitUntil.Remove(id);
            foreach (string id in RomanceUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) RomanceUntil.Remove(id);
            foreach (string id in DuelUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) DuelUntil.Remove(id);
            foreach (string id in TailUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) TailUntil.Remove(id);
            foreach (string id in MarshalWarned.Where(p => p.Value <= today).Select(p => p.Key).ToList()) MarshalWarned.Remove(id);
            foreach (string id in StewardWarned.Where(p => p.Value <= today).Select(p => p.Key).ToList()) StewardWarned.Remove(id);
            foreach (string id in CourtDayUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) CourtDayUntil.Remove(id);
            foreach (string id in GiftUntil.Where(p => p.Value <= today).Select(p => p.Key).ToList()) GiftUntil.Remove(id);
            Audiences.RemoveAll(a => a.Until <= today);
            Hooks.RemoveAll(h => h.Until <= today);
        }

        // ---- Metin biçimi ----

        public string Write()
        {
            var records = new List<TextRecord>
            {
                new TextRecord("version").Add("v", I(CurrentVersion)),
                new TextRecord("meta").Add("spymaster", SpymasterId).Add("next", I(NextSchemeId)).Add("seed", I(Seed)).Add("rolls", I(Rolls))
                    .Add("nextplot", I(NextPlotId)).Add("nextletter", I(NextLetterId)).Add("nextcourt", I(NextCourtshipId))
                    .Add("mission", I((int)Mission)).Add("rumor", RumorTargetId)
                    .Add("treasurer", TreasurerId).Add("embezzled", I(Embezzled)).Add("embezzlefound", B(EmbezzleFound))
                    .Add("physician", PhysicianId).Add("physicianwarn", I(PhysicianWarnUntil)).Add("marshal", MarshalId).Add("steward", StewardId)
                    .Add("brothelvisit", I(BrothelVisitUntil)).Add("troopnight", I(TroopNightUntil))
                    .Add("illsince", I(IllnessSince)).Add("illuntil", I(IllnessUntil)),
            };
            if (!string.IsNullOrEmpty(SaveTestMarker)) records.Add(new TextRecord("savetest").Add("marker", SaveTestMarker));

            foreach (Scheme s in Schemes)
                records.Add(new TextRecord("scheme")
                    .Add("id", I(s.Id)).Add("kind", I((int)s.Kind)).Add("target", s.TargetId).Add("start", I(s.StartDay))
                    .Add("phase", I(s.Phase)).Add("days", I(s.DaysIntoPhase)).Add("phasedays", I(s.PhaseDays)).Add("cost", I(s.StartCost))
                    .Add("seed", I(s.Seed)).Add("rolls", I(s.Rolls))
                    .Add("evs", I(s.EventSuccess)).Add("evd", I(s.EventDiscovery))
                    .Add("used", string.Join(",", s.UsedEvents.Select(I))).Add("method", I((int)s.Method))
                    .Add("feast", B(s.FeastRisk))
                    .Add("pending", I(s.PendingEvent)).Add("spychoice", B(s.AwaitingChoice)).Add("asked", B(s.AskedToExecute)));

            foreach (IntelReport r in Reports)
                records.Add(new TextRecord("report")
                    .Add("target", r.TargetId).Add("name", r.TargetName).Add("day", I(r.Day)).Add("location", r.Location)
                    .Add("party", r.Party).Add("gold", r.Gold).Add("relations", r.Relations).Add("traits", r.Traits));

            foreach (Secret s in Secrets)
                records.Add(new TextRecord("secret")
                    .Add("target", s.TargetId).Add("kind", I((int)s.Kind)).Add("day", I(s.Day)).Add("exposed", B(s.Exposed))
                    .Add("other", s.OtherId).Add("child", s.ChildId).Add("whisper", B(s.Whispered)));

            foreach (KeyValuePair<string, int> p in CooldownUntil) records.Add(new TextRecord("cooldown").Add("target", p.Key).Add("until", I(p.Value)));
            foreach (KeyValuePair<string, int> p in AlertUntil) records.Add(new TextRecord("alert").Add("target", p.Key).Add("until", I(p.Value)));
            foreach (Notice n in Notices)
                records.Add(new TextRecord("notice").Add("title", n.Title).Add("text", n.Text).Add("hero", n.HeroId).Add("img", n.Image));

            if (LiegeDemand != null)
                records.Add(new TextRecord("liege")
                    .Add("kingdom", LiegeDemand.KingdomId).Add("victim", LiegeDemand.VictimName)
                    .Add("gold", I(LiegeDemand.Gold)).Add("influence", I(LiegeDemand.Influence)).Add("kidnap", B(LiegeDemand.Kidnap)));

            foreach (LogEntry e in Log) records.Add(new TextRecord("log").Add("day", I(e.Day)).Add("text", e.Text));

            foreach (Hook h in Hooks) records.Add(new TextRecord("hook").Add("target", h.TargetId).Add("day", I(h.Day)).Add("until", I(h.Until)));
            foreach (AiPlot p in AiPlots)
                records.Add(new TextRecord("aiplot")
                    .Add("id", I(p.Id)).Add("plotter", p.PlotterId).Add("target", p.TargetId).Add("kind", I((int)p.Kind))
                    .Add("start", I(p.StartDay)).Add("resolve", I(p.ResolveDay)).Add("seed", I(p.Seed)).Add("rolls", I(p.Rolls))
                    .Add("detected", B(p.Detected)).Add("asked", B(p.Asked)).Add("protected", B(p.Protected)));
            foreach (ShadowNetwork n in Networks)
                records.Add(new TextRecord("network").Add("settlement", n.SettlementId).Add("level", I(n.Level)).Add("since", I(n.Since)));
            foreach (KeyValuePair<string, int> p in FeudUntil) records.Add(new TextRecord("feud").Add("hero", p.Key).Add("until", I(p.Value)));
            foreach (KeyValuePair<string, int> p in InterrogatedUntil)
                records.Add(new TextRecord("interrogated").Add("hero", p.Key).Add("until", I(p.Value)));
            foreach (Audience a in Audiences)
                records.Add(new TextRecord("audience").Add("hero", a.HeroId).Add("until", I(a.Until)).Add("relation", I(a.Relation))
                    .Add("asked", B(a.Asked)));
            foreach (KeyValuePair<string, int> p in GiftUntil) records.Add(new TextRecord("gift").Add("hero", p.Key).Add("until", I(p.Value)));
            foreach (KeyValuePair<string, int> p in RecruitUntil) records.Add(new TextRecord("recruit").Add("hero", p.Key).Add("until", I(p.Value)));
            foreach (KeyValuePair<string, int> p in Tension.Where(p => p.Value > 0))
                records.Add(new TextRecord("tension").Add("kingdom", p.Key).Add("value", I(p.Value)));
            foreach (Courtship c in Courtships)
                records.Add(new TextRecord("court").Add("id", I(c.Id)).Add("target", c.TargetId).Add("agent", c.AgentId).Add("mode", I((int)c.Mode))
                    .Add("stage", I(c.Stage)).Add("days", I(c.DaysIntoStage)).Add("start", I(c.StartDay)).Add("careful", B(c.Careful)));
            foreach (Lover l in Lovers)
                records.Add(new TextRecord("lover").Add("hero", l.HeroId).Add("since", I(l.Since)).Add("next", I(l.NextMeetingDay)).Add("public", B(l.Public)));
            foreach (Pregnancy p in Pregnancies)
                records.Add(new TextRecord("pregnancy").Add("mother", p.MotherId).Add("father", p.FatherId).Add("due", I(p.DueDay)).Add("hidden", B(p.Hidden)));
            if (SpouseAffair != null)
                records.Add(new TextRecord("spouseaffair").Add("spouse", SpouseAffair.SpouseId).Add("lover", SpouseAffair.LoverId)
                    .Add("since", I(SpouseAffair.Since)).Add("next", I(SpouseAffair.NextMeetingDay)).Add("discovered", B(SpouseAffair.Discovered))
                    .Add("asked", B(SpouseAffair.Asked)).Add("how", SpouseAffair.How).Add("children", string.Join(";", SpouseAffair.ChildIds)));
            foreach (string id in NoChildren) records.Add(new TextRecord("nochild").Add("hero", id));
            foreach (KeyValuePair<string, int> p in DuelUntil) records.Add(new TextRecord("duelcd").Add("hero", p.Key).Add("until", I(p.Value)));
            foreach (KeyValuePair<string, int> p in TailUntil) records.Add(new TextRecord("tailcd").Add("hero", p.Key).Add("until", I(p.Value)));
            foreach (KeyValuePair<string, int> p in Conceived) records.Add(new TextRecord("conceived").Add("hero", p.Key).Add("day", I(p.Value)));
            foreach (KeyValuePair<string, int> p in MarshalWarned) records.Add(new TextRecord("marshalwarn").Add("key", p.Key).Add("until", I(p.Value)));
            foreach (KeyValuePair<string, int> p in StewardWarned) records.Add(new TextRecord("stewardwarn").Add("key", p.Key).Add("until", I(p.Value)));
            foreach (CaravanBook b in CaravanBooks)
                records.Add(new TextRecord("caravanbook").Add("party", b.PartyId).Add("town", b.TownId).Add("worth", I(b.Worth))
                    .Add("day", I(b.Day)).Add("sent", I(b.SentHome)));
            foreach (CaravanLeg l in CaravanLegs)
                records.Add(new TextRecord("caravanleg").Add("party", l.PartyId).Add("from", l.FromId).Add("to", l.ToId)
                    .Add("profit", I(l.Profit)).Add("days", I(l.Days)).Add("day", I(l.Day)));
            foreach (HostageDeal h in Hostages)
                records.Add(new TextRecord("hostage").Add("kingdom", h.KingdomId).Add("given", h.GivenId).Add("held", h.HeldId)
                    .Add("since", I(h.Since)).Add("until", I(h.Until)).Add("fostered", I(h.Fostered)));
            foreach (KeyValuePair<string, int> p in CourtDayUntil) records.Add(new TextRecord("courtday").Add("place", p.Key).Add("until", I(p.Value)));
            foreach (string id in AgeFrozen) records.Add(new TextRecord("frozen").Add("hero", id));
            if (SpouseWatchUntil > 0 || SpouseApartWeeks > 0)
                records.Add(new TextRecord("spouse").Add("watch", I(SpouseWatchUntil)).Add("apart", I(SpouseApartWeeks)));
            foreach (KeyValuePair<string, int> p in RomanceUntil) records.Add(new TextRecord("romancecd").Add("hero", p.Key).Add("until", I(p.Value)));
            foreach (House h in Houses)
                records.Add(new TextRecord("house").Add("town", h.TownId).Add("staff", I(h.Staff)).Add("share", I((int)h.Share)).Add("since", I(h.Since)));
            foreach (Crisis c in Crises)
                records.Add(new TextRecord("crisis").Add("kingdom", c.KingdomId).Add("town", c.TownId).Add("level", I(c.Level))
                    .Add("day", I(c.Day)).Add("crime", I(c.Crime)).Add("owner", c.OwnerId));
            foreach (Letter l in Letters)
                records.Add(new TextRecord("letter").Add("id", I(l.Id)).Add("kind", I((int)l.Kind)).Add("target", l.TargetId)
                    .Add("use", I(l.Use)).Add("sent", I(l.SentDay)).Add("arrive", I(l.ArriveDay)));
            return TextRecords.Write(records);
        }

        /// <summary>Asla istisna atmaz: boş, bozuk ya da yeni sürümden metin boş/kısmi duruma döner.</summary>
        public static IntrigueState Parse(string text)
        {
            var state = new IntrigueState();
            foreach (TextRecord r in TextRecords.Parse(text))
            {
                switch (r.Kind)
                {
                    case "meta":
                        state.SpymasterId = Empty(r.Get("spymaster"));
                        state.NextSchemeId = Math.Max(1, Int(r, "next"));
                        state.Seed = Int(r, "seed");
                        state.Rolls = Int(r, "rolls");
                        state.NextPlotId = Math.Max(1, Int(r, "nextplot"));
                        state.NextLetterId = Math.Max(1, Int(r, "nextletter"));
                        state.NextCourtshipId = Math.Max(1, Int(r, "nextcourt"));
                        int mission = Int(r, "mission");
                        state.Mission = Enum.IsDefined(typeof(SpymasterMission), mission) ? (SpymasterMission)mission : SpymasterMission.None;
                        state.RumorTargetId = Empty(r.Get("rumor"));
                        state.TreasurerId = Empty(r.Get("treasurer"));
                        state.Embezzled = Math.Max(0, Int(r, "embezzled"));
                        state.EmbezzleFound = Bool(r, "embezzlefound");
                        state.PhysicianId = Empty(r.Get("physician"));
                        state.PhysicianWarnUntil = Int(r, "physicianwarn");
                        state.MarshalId = Empty(r.Get("marshal"));
                        state.StewardId = Empty(r.Get("steward"));
                        state.BrothelVisitUntil = Int(r, "brothelvisit");
                        state.TroopNightUntil = Int(r, "troopnight");
                        state.IllnessSince = Int(r, "illsince");
                        state.IllnessUntil = Math.Max(0, Int(r, "illuntil"));
                        break;
                    case "savetest":
                        state.SaveTestMarker = r.Get("marker");
                        break;
                    case "scheme":
                        Scheme scheme = ParseScheme(r);
                        if (scheme != null) state.Schemes.Add(scheme);
                        break;
                    case "report":
                        if (Empty(r.Get("target")) == null) break;
                        state.Reports.Add(new IntelReport
                        {
                            TargetId = r.Get("target"), TargetName = r.Get("name") ?? "", Day = Int(r, "day"),
                            Location = r.Get("location") ?? "", Party = r.Get("party") ?? "", Gold = r.Get("gold") ?? "",
                            Relations = r.Get("relations") ?? "", Traits = r.Get("traits") ?? "",
                        });
                        break;
                    case "secret":
                        int kind = Int(r, "kind");
                        if (Empty(r.Get("target")) == null || !Enum.IsDefined(typeof(SecretKind), kind)) break;
                        state.Secrets.Add(new Secret
                        {
                            TargetId = r.Get("target"), Kind = (SecretKind)kind, Day = Int(r, "day"), Exposed = Bool(r, "exposed"),
                            OtherId = Empty(r.Get("other")), ChildId = Empty(r.Get("child")), Whispered = Bool(r, "whisper"),
                        });
                        break;
                    case "cooldown":
                        if (Empty(r.Get("target")) != null) state.CooldownUntil[r.Get("target")] = Int(r, "until");
                        break;
                    case "alert":
                        if (Empty(r.Get("target")) != null) state.AlertUntil[r.Get("target")] = Int(r, "until");
                        break;
                    case "notice":
                        state.Notices.Add(new Notice
                        {
                            Title = r.Get("title") ?? "", Text = r.Get("text") ?? "", HeroId = Empty(r.Get("hero")), Image = Empty(r.Get("img")),
                        });
                        break;
                    case "liege":
                        if (Empty(r.Get("kingdom")) == null) break;
                        state.LiegeDemand = new LiegeDemand
                        {
                            KingdomId = r.Get("kingdom"), VictimName = r.Get("victim") ?? "",
                            Gold = Int(r, "gold"), Influence = Int(r, "influence"), Kidnap = Bool(r, "kidnap"),
                        };
                        break;
                    case "log":
                        state.Log.Add(new LogEntry { Day = Int(r, "day"), Text = r.Get("text") ?? "" });
                        break;
                    case "hook":
                        if (Empty(r.Get("target")) != null)
                            state.Hooks.Add(new Hook { TargetId = r.Get("target"), Day = Int(r, "day"), Until = Int(r, "until") });
                        break;
                    case "aiplot":
                        int plotKind = Int(r, "kind");
                        if (Empty(r.Get("plotter")) == null || Empty(r.Get("target")) == null || !Enum.IsDefined(typeof(SchemeKind), plotKind)) break;
                        state.AiPlots.Add(new AiPlot
                        {
                            Id = Int(r, "id"), PlotterId = r.Get("plotter"), TargetId = r.Get("target"), Kind = (SchemeKind)plotKind,
                            StartDay = Int(r, "start"), ResolveDay = Int(r, "resolve"), Seed = Int(r, "seed"), Rolls = Int(r, "rolls"),
                            Detected = Bool(r, "detected"), Asked = Bool(r, "asked"), Protected = Bool(r, "protected"),
                        });
                        break;
                    case "network":
                        if (Empty(r.Get("settlement")) == null) break;
                        state.Networks.Add(new ShadowNetwork
                        {
                            SettlementId = r.Get("settlement"),
                            Level = SchemeMath.Clamp(Int(r, "level"), 1, SchemeMath.MaxNetworkLevel),
                            Since = Int(r, "since"),
                        });
                        break;
                    case "feud":
                        if (Empty(r.Get("hero")) != null) state.FeudUntil[r.Get("hero")] = Int(r, "until");
                        break;
                    case "interrogated":
                        if (Empty(r.Get("hero")) != null) state.InterrogatedUntil[r.Get("hero")] = Int(r, "until");
                        break;
                    case "audience":
                        if (Empty(r.Get("hero")) == null) break;
                        state.Audiences.Add(new Audience
                        {
                            HeroId = r.Get("hero"), Until = Int(r, "until"), Relation = Int(r, "relation"), Asked = Bool(r, "asked"),
                        });
                        break;
                    case "gift":
                        if (Empty(r.Get("hero")) != null) state.GiftUntil[r.Get("hero")] = Int(r, "until");
                        break;
                    case "recruit":
                        if (Empty(r.Get("hero")) != null) state.RecruitUntil[r.Get("hero")] = Int(r, "until");
                        break;
                    case "tension":
                        if (Empty(r.Get("kingdom")) != null)
                            state.Tension[r.Get("kingdom")] = SchemeMath.Clamp(Int(r, "value"), 0, CrisisRules.MaxTension);
                        break;
                    case "court":
                        int courtMode = Int(r, "mode");
                        if (Empty(r.Get("target")) == null || Empty(r.Get("agent")) == null || !Enum.IsDefined(typeof(CourtshipMode), courtMode)) break;
                        state.Courtships.Add(new Courtship
                        {
                            Id = Int(r, "id"), TargetId = r.Get("target"), AgentId = r.Get("agent"), Mode = (CourtshipMode)courtMode,
                            Stage = SchemeMath.Clamp(Int(r, "stage"), 0, RomanceRules.StageCount), DaysIntoStage = Math.Max(0, Int(r, "days")),
                            StartDay = Int(r, "start"), Careful = Bool(r, "careful"),
                        });
                        break;
                    case "lover":
                        if (Empty(r.Get("hero")) == null || state.LoverOf(r.Get("hero")) != null) break;
                        state.Lovers.Add(new Lover { HeroId = r.Get("hero"), Since = Int(r, "since"), NextMeetingDay = Int(r, "next"), Public = Bool(r, "public") });
                        break;
                    case "pregnancy":
                        if (Empty(r.Get("mother")) == null || Empty(r.Get("father")) == null) break;
                        state.Pregnancies.Add(new Pregnancy { MotherId = r.Get("mother"), FatherId = r.Get("father"), DueDay = Int(r, "due"), Hidden = Bool(r, "hidden") });
                        break;
                    case "spouseaffair":
                        if (Empty(r.Get("spouse")) == null || Empty(r.Get("lover")) == null) break;
                        state.SpouseAffair = new SpouseAffair
                        {
                            SpouseId = r.Get("spouse"), LoverId = r.Get("lover"), Since = Int(r, "since"), NextMeetingDay = Int(r, "next"),
                            Discovered = Bool(r, "discovered"), Asked = Bool(r, "asked"), How = Empty(r.Get("how")),
                            ChildIds = (r.Get("children") ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                        };
                        break;
                    case "hostage":
                        if (Empty(r.Get("kingdom")) == null || (Empty(r.Get("given")) == null && Empty(r.Get("held")) == null)) break;
                        state.Hostages.Add(new HostageDeal
                        {
                            KingdomId = r.Get("kingdom"), GivenId = Empty(r.Get("given")), HeldId = Empty(r.Get("held")),
                            Since = Int(r, "since"), Until = Int(r, "until"), Fostered = Int(r, "fostered"),
                        });
                        break;
                    case "courtday":
                        if (Empty(r.Get("place")) != null) state.CourtDayUntil[r.Get("place")] = Int(r, "until");
                        break;
                    case "duelcd":
                        if (Empty(r.Get("hero")) != null) state.DuelUntil[r.Get("hero")] = Int(r, "until");
                        break;
                    case "tailcd":
                        if (Empty(r.Get("hero")) != null) state.TailUntil[r.Get("hero")] = Int(r, "until");
                        break;
                    case "stewardwarn":
                        if (Empty(r.Get("key")) != null) state.StewardWarned[r.Get("key")] = Int(r, "until");
                        break;
                    case "marshalwarn":
                        if (Empty(r.Get("key")) != null) state.MarshalWarned[r.Get("key")] = Int(r, "until");
                        break;
                    case "conceived":
                        if (Empty(r.Get("hero")) != null) state.Conceived[r.Get("hero")] = Int(r, "day");
                        break;
                    case "caravanbook":
                        if (Empty(r.Get("party")) == null || Empty(r.Get("town")) == null) break;
                        state.CaravanBooks.Add(new CaravanBook
                        {
                            PartyId = r.Get("party"), TownId = r.Get("town"), Worth = Int(r, "worth"), Day = Int(r, "day"), SentHome = Int(r, "sent"),
                        });
                        break;
                    case "caravanleg":
                        if (Empty(r.Get("from")) == null || Empty(r.Get("to")) == null) break;
                        state.CaravanLegs.Add(new CaravanLeg
                        {
                            PartyId = Empty(r.Get("party")), FromId = r.Get("from"), ToId = r.Get("to"), Profit = Int(r, "profit"),
                            Days = Int(r, "days"), Day = Int(r, "day"),
                        });
                        break;
                    case "nochild":
                        if (Empty(r.Get("hero")) != null) state.NoChildren.Add(r.Get("hero"));
                        break;
                    case "frozen":
                        if (Empty(r.Get("hero")) != null) state.AgeFrozen.Add(r.Get("hero"));
                        break;
                    case "spouse":
                        state.SpouseWatchUntil = Math.Max(0, Int(r, "watch"));
                        state.SpouseApartWeeks = Math.Max(0, Int(r, "apart"));
                        break;
                    case "romancecd":
                        if (Empty(r.Get("hero")) != null) state.RomanceUntil[r.Get("hero")] = Int(r, "until");
                        break;
                    case "house":
                        int share = Int(r, "share");
                        if (Empty(r.Get("town")) == null || state.HouseIn(r.Get("town")) != null) break;
                        state.Houses.Add(new House
                        {
                            TownId = r.Get("town"), Staff = SchemeMath.Clamp(Int(r, "staff"), 0, HouseRules.MaxStaff(SchemeMath.MaxNetworkLevel)),
                            Share = Enum.IsDefined(typeof(HouseShare), share) ? (HouseShare)share : HouseShare.Standard, Since = Int(r, "since"),
                        });
                        break;
                    case "crisis":
                        if (Empty(r.Get("kingdom")) == null) break;
                        state.Crises.Add(new Crisis
                        {
                            KingdomId = r.Get("kingdom"), TownId = Empty(r.Get("town")), Level = SchemeMath.Clamp(Int(r, "level"), 1, SchemeMath.MaxNetworkLevel),
                            Day = Int(r, "day"), Crime = Int(r, "crime"), OwnerId = Empty(r.Get("owner")),
                        });
                        break;
                    case "letter":
                        int letterKind = Int(r, "kind");
                        if (Empty(r.Get("target")) == null || !Enum.IsDefined(typeof(LetterKind), letterKind)) break;
                        state.Letters.Add(new Letter
                        {
                            Id = Int(r, "id"), Kind = (LetterKind)letterKind, TargetId = r.Get("target"), Use = Int(r, "use"),
                            SentDay = Int(r, "sent"), ArriveDay = Int(r, "arrive"),
                        });
                        break;
                }
            }
            if (state.Log.Count > MaxLogEntries) state.Log.RemoveRange(0, state.Log.Count - MaxLogEntries);
            return state;
        }

        private static Scheme ParseScheme(TextRecord r)
        {
            int kind = Int(r, "kind");
            if (Empty(r.Get("target")) == null || !Enum.IsDefined(typeof(SchemeKind), kind)) return null;
            int method = Int(r, "method");
            return new Scheme
            {
                Id = Int(r, "id"),
                Kind = (SchemeKind)kind,
                TargetId = r.Get("target"),
                StartDay = Int(r, "start"),
                Phase = Math.Max(0, Int(r, "phase")),
                DaysIntoPhase = Math.Max(0, Int(r, "days")),
                PhaseDays = Math.Max(1, Int(r, "phasedays")),
                StartCost = Math.Max(0, Int(r, "cost")),
                Seed = Int(r, "seed"),
                Rolls = Int(r, "rolls"),
                EventSuccess = Int(r, "evs"),
                EventDiscovery = Int(r, "evd"),
                UsedEvents = (r.Get("used") ?? "").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out int id) ? id : 0)
                    .Where(id => id > 0).ToList(),
                Method = Enum.IsDefined(typeof(SchemeMethod), method) ? (SchemeMethod)method : SchemeMethod.None,
                FeastRisk = Bool(r, "feast"),
                PendingEvent = Math.Max(0, Int(r, "pending")),
                AwaitingChoice = Bool(r, "spychoice"),
                AskedToExecute = Bool(r, "asked"),
            };
        }

        private static string I(int value) => value.ToString(CultureInfo.InvariantCulture);
        private static string B(bool value) => value ? "1" : "0";
        private static string Empty(string value) => string.IsNullOrEmpty(value) ? null : value;
        private static bool Bool(TextRecord r, string key) => r.Get(key) == "1";

        private static int Int(TextRecord r, string key) =>
            int.TryParse(r.Get(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value) ? value : 0;
    }
}
