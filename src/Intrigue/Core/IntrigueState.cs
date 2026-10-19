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
            Hooks.RemoveAll(h => h.Until <= today);
        }

        // ---- Metin biçimi ----

        public string Write()
        {
            var records = new List<TextRecord>
            {
                new TextRecord("version").Add("v", I(CurrentVersion)),
                new TextRecord("meta").Add("spymaster", SpymasterId).Add("next", I(NextSchemeId)).Add("seed", I(Seed)).Add("rolls", I(Rolls))
                    .Add("nextplot", I(NextPlotId)),
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
                    .Add("target", s.TargetId).Add("kind", I((int)s.Kind)).Add("day", I(s.Day)).Add("exposed", B(s.Exposed)));

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
