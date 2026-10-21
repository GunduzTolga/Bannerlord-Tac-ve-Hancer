using System;
using System.Collections.Generic;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Savaş ve barış > Krallıklar sekmesinin hesapları (tasarım: docs/superpowers/specs/2026-10-21-kralliklar-sekmesi.md). Saf: oyunsuz test edilir.
    /// </summary>
    public static class KingdomsRules
    {
        /// <summary>Listede sıra: savaştıkların, müttefiklerin, saldırmazlık, ticaret, ötekiler.</summary>
        public static int Order(bool war, bool ally, bool pact, bool trade) => war ? 0 : ally ? 1 : pact ? 2 : trade ? 3 : 4;

        /// <summary>Seninle durumu tek satırda: "Savaşta" ya da "Müttefik · Ticaret" gibi; hiçbiri yoksa "Barış".</summary>
        public static string StanceText(bool war, bool ally, bool pact, bool trade)
        {
            if (war) return "Savaşta";
            var parts = new List<string>();
            if (ally) parts.Add("Müttefik");
            if (pact) parts.Add("Saldırmazlık");
            if (trade) parts.Add("Ticaret");
            return parts.Count == 0 ? "Barış" : string.Join(" · ", parts);
        }

        /// <summary>Anlaşmanın kalan süresi.</summary>
        public static string DaysLeft(float days) => days < 1f ? "bugün bitiyor" : (int)Math.Ceiling(days) + " gün kaldı";

        /// <summary>Diplomacy'nin yayılmacılığı, kritik eşiğe göre (kritikte komşular ona karşı birleşir).</summary>
        public static string Expansion(float value, float critical)
        {
            if (critical <= 0f) return "";
            if (value >= critical) return "kritik";
            if (value >= critical * 0.66f) return "yüksek";
            return value >= critical * 0.33f ? "orta" : "düşük";
        }

        /// <summary>
        /// Oyunun ve Diplomacy'nin çok satırlı gerekçesini tek satıra indirir ("X şunlardan ötürü düşünmüyor:" başlığı atılır, nedenler
        /// "; " ile). Boşsa boş.
        /// </summary>
        public static string Compact(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            List<string> lines = text.Replace("\r", "").Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
            if (lines.Count > 1 && lines[0].EndsWith(":")) lines.RemoveAt(0);
            return string.Join("; ", lines);
        }
    }
}
