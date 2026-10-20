using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheatMenu
{
    public enum PatchKind { Prefix, Postfix, Transpiler, Finalizer }

    public enum CompatRisk { Low, Medium, High }

    /// <summary>Bir oyun metoduna bir modun yaptığı tek Harmony yaması.</summary>
    public sealed class PatchEntry
    {
        public string Target;
        public string Mod;
        public PatchKind Kind;
        /// <summary>bool döndüren ön yama: false döndürürse asıl metot (ve başka modların değiştirdiği hâli) hiç çalışmaz.</summary>
        public bool CanSkipOriginal;
    }

    /// <summary>Oyunun model listesindeki bir model, eklenme sırasıyla.</summary>
    public sealed class ModelEntry
    {
        /// <summary>Modelin türü (ör. PregnancyModel): aynı türden yalnızca en son eklenen çalışır, öncekileri ancak o çağırırsa.</summary>
        public string Kind;
        public string Mod;
        public bool IsGame;
        /// <summary>Önceki modeli (BaseModel) çağırıyor: öncekilerin etkisi korunur.</summary>
        public bool CallsBase;
    }

    public sealed class CompatFinding
    {
        public CompatRisk Risk;
        public string Title;
        public List<string> Lines = new List<string>();
    }

    /// <summary>
    /// Paketlenen modların birbiriyle uyumu (oyun içinde CompatReport toplar, burada değerlendirilir). İki çakışma türü:
    /// - Aynı oyun metodunu birden çok mod yamalıyor. Ardışık son yamalar (postfix) genelde sorunsuz; iki modun kod değiştirmesi
    ///   (transpiler) ya da birinin asıl metodu atlaması (false döndüren prefix) öbürünün işini boşa çıkarabilir.
    /// - Aynı oyun modelini birden çok mod ekliyor. Yalnızca sonuncusu çalışır; öncekiler ancak sonraki onları (BaseModel) çağırırsa.
    /// Saf: oyunsuz test edilir.
    /// </summary>
    public static class CompatRules
    {
        /// <summary>Altyapı modları: başkalarıyla aynı yeri yamalamaları tasarım gereği (arayüz eklentisi, ayar ekranı).</summary>
        public static readonly string[] InfrastructurePrefixes =
        {
            "0Harmony", "Bannerlord.Harmony", "Bannerlord.ButterLib", "Bannerlord.UIExtenderEx", "Bannerlord.BLSE", "MCM",
        };

        // "MCM": MCMv5, MCMv5.UI... ; ötekiler: tam ad ya da "ad." ile başlayan.
        public static bool IsInfrastructure(string mod) =>
            mod != null && InfrastructurePrefixes.Any(p => p == "MCM"
                ? mod.StartsWith(p, StringComparison.OrdinalIgnoreCase)
                : mod.Equals(p, StringComparison.OrdinalIgnoreCase) || mod.StartsWith(p + ".", StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// İncelenmiş ön yamalar: asıl metodu yalnızca geçersiz girdide atlıyor (koruma), başka modun işini boşa çıkarmıyor.
        /// Mod adı tam ya da "ad." ile başlayan (ör. Bannerlord.Diplomacy.1.4.8).
        /// </summary>
        public static readonly string[][] ReviewedGuards =
        {
            // Diplomacy 1.5.4: iki kişiden biri null ise çıkışları null yapıp atlar; ikisi de varsa dokunmaz (20 Ekim'de IL'den incelendi).
            new[] { "TaleWorlds.CampaignSystem.GameComponents.DefaultDiplomacyModel.GetHeroesForEffectiveRelation", "Bannerlord.Diplomacy" },
        };

        public static bool IsReviewedGuard(PatchEntry p) =>
            p != null && p.Kind == PatchKind.Prefix && p.Mod != null && ReviewedGuards.Any(g => g[0] == p.Target
                && (p.Mod.Equals(g[1], StringComparison.OrdinalIgnoreCase) || p.Mod.StartsWith(g[1] + ".", StringComparison.OrdinalIgnoreCase)));

        public static List<CompatFinding> PatchFindings(IEnumerable<PatchEntry> patches)
        {
            var findings = new List<CompatFinding>();
            foreach (IGrouping<string, PatchEntry> target in patches.Where(p => !IsInfrastructure(p.Mod)).GroupBy(p => p.Target))
            {
                List<string> mods = target.Select(p => p.Mod).Distinct().ToList();
                if (mods.Count < 2) continue;
                List<string> transpilers = target.Where(p => p.Kind == PatchKind.Transpiler).Select(p => p.Mod).Distinct().ToList();
                List<string> skippers = target.Where(p => p.Kind == PatchKind.Prefix && p.CanSkipOriginal && !IsReviewedGuard(p)).Select(p => p.Mod).Distinct().ToList();
                CompatRisk risk;
                string why;
                if (transpilers.Count >= 2)
                {
                    risk = CompatRisk.High;
                    why = "iki mod metodun kodunu değiştiriyor (" + string.Join(", ", transpilers) + "); biri öbürünü bozabilir";
                }
                else if (skippers.Count >= 2)
                {
                    risk = CompatRisk.High;
                    why = "iki mod da asıl metodu atlayıp kendi işini yapabiliyor (" + string.Join(", ", skippers) + "); hangisi geçerli belirsiz";
                }
                else if (skippers.Count == 1 && target.Any(p => p.Mod != skippers[0] && !IsReviewedGuard(p) && (p.Kind == PatchKind.Transpiler || p.Kind == PatchKind.Prefix)))
                {
                    risk = transpilers.Any(t => t != skippers[0]) ? CompatRisk.High : CompatRisk.Medium;
                    why = skippers[0] + " asıl metodu atlayabiliyor; o zaman öbür modun bu metoda yaptığı değişiklik çalışmaz";
                }
                else
                {
                    risk = CompatRisk.Low;
                    why = "yamalar art arda çalışır";
                }
                var finding = new CompatFinding { Risk = risk, Title = target.Key + ": " + why };
                foreach (PatchEntry p in target.OrderBy(p => p.Mod).ThenBy(p => p.Kind))
                    finding.Lines.Add(p.Mod + " — " + KindText(p));
                findings.Add(finding);
            }
            return Sorted(findings);
        }

        /// <param name="models">Oyunun model listesi, eklenme sırasıyla.</param>
        public static List<CompatFinding> ModelFindings(IEnumerable<ModelEntry> models)
        {
            var findings = new List<CompatFinding>();
            foreach (IGrouping<string, ModelEntry> kind in models.GroupBy(m => m.Kind))
            {
                List<ModelEntry> chain = kind.ToList();
                List<string> mods = chain.Where(m => !m.IsGame).Select(m => m.Mod).Distinct().ToList();
                if (mods.Count < 2) continue;
                var lost = new List<string>();
                var alive = new List<ModelEntry>();
                foreach (ModelEntry m in chain)
                {
                    if (!m.CallsBase)
                    {
                        lost.AddRange(alive.Where(a => !a.IsGame && a.Mod != m.Mod).Select(a => a.Mod + " (" + m.Mod + " çağırmıyor)"));
                        alive.Clear();
                    }
                    alive.Add(m);
                }
                var finding = new CompatFinding
                {
                    Risk = lost.Count > 0 ? CompatRisk.High : CompatRisk.Low,
                    Title = kind.Key + ": " + (lost.Count > 0
                        ? "çalışmayan: " + string.Join(", ", lost.Distinct())
                        : "hepsi zincirli, hepsinin etkisi geçerli"),
                };
                foreach (ModelEntry m in chain.Where(m => !m.IsGame))
                    finding.Lines.Add(m.Mod + (m.CallsBase ? " — öncekini çağırıyor" : " — öncekini çağırmıyor (kendinden öncekilerin yerine geçer)"));
                findings.Add(finding);
            }
            return Sorted(findings);
        }

        public static int Count(IEnumerable<CompatFinding> findings, CompatRisk risk) => findings.Count(f => f.Risk == risk);

        public static string Format(IList<CompatFinding> patchFindings, IList<CompatFinding> modelFindings, IEnumerable<string> mods, string when)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Taç ve Hançer — mod uyum raporu (" + when + ")");
            sb.AppendLine("Yüklü kod modülleri: " + string.Join(", ", mods));
            sb.AppendLine();
            sb.AppendLine("Ciddi: " + (Count(patchFindings, CompatRisk.High) + Count(modelFindings, CompatRisk.High))
                + "   Dikkat: " + (Count(patchFindings, CompatRisk.Medium) + Count(modelFindings, CompatRisk.Medium))
                + "   Sorunsuz ortak nokta: " + (Count(patchFindings, CompatRisk.Low) + Count(modelFindings, CompatRisk.Low)));
            Section(sb, "Aynı oyun modelini ekleyen modlar", modelFindings);
            Section(sb, "Aynı oyun metodunu yamalayan modlar", patchFindings);
            return sb.ToString();
        }

        private static void Section(StringBuilder sb, string title, IList<CompatFinding> findings)
        {
            sb.AppendLine();
            sb.AppendLine("== " + title + " (" + findings.Count + ")");
            if (findings.Count == 0) sb.AppendLine("   yok");
            foreach (CompatFinding f in findings)
            {
                sb.AppendLine("[" + RiskText(f.Risk) + "] " + f.Title);
                foreach (string line in f.Lines) sb.AppendLine("     " + line);
            }
        }

        public static string RiskText(CompatRisk risk) =>
            risk == CompatRisk.High ? "CİDDİ" : risk == CompatRisk.Medium ? "DİKKAT" : "sorunsuz";

        private static string KindText(PatchEntry p)
        {
            switch (p.Kind)
            {
                case PatchKind.Prefix: return IsReviewedGuard(p) ? "önce çalışır, yalnızca geçersiz girdide atlar (incelendi)" : p.CanSkipOriginal ? "önce çalışır, asıl metodu atlayabilir" : "önce çalışır";
                case PatchKind.Postfix: return "sonra çalışır";
                case PatchKind.Transpiler: return "kodunu değiştirir";
                default: return "hata yakalar";
            }
        }

        private static List<CompatFinding> Sorted(List<CompatFinding> findings) =>
            findings.OrderByDescending(f => f.Risk).ThenBy(f => f.Title, StringComparer.Ordinal).ToList();
    }
}
