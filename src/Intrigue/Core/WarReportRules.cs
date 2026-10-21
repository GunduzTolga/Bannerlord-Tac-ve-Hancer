using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Savaş Raporu penceresinin hesapları (tasarım: docs/superpowers/specs/2026-10-21-savas-raporu.md): karşılaştırma çubukları,
    /// krallık renginin okunur hâli, kısa metinler. Saf: oyunsuz test edilir.
    /// </summary>
    public static class WarReportRules
    {
        /// <summary>Karşılaştırma çubuğunun genişliği (TvhWarRow'daki boşlukla aynı).</summary>
        public const float BarWidth = 270f;

        /// <summary>Çubuğun iki parçası, değerler oranında. İkisi de 0 ise eşit; eksi değer 0 sayılır.</summary>
        public static (float Ours, float Theirs) Bar(double ours, double theirs, float width = BarWidth)
        {
            double a = Math.Max(0, ours), b = Math.Max(0, theirs);
            if (a + b <= 0) return (width / 2f, width / 2f);
            float left = (float)Math.Round(width * a / (a + b), 1);
            return (left, width - left);
        }

        /// <summary>Rengin parlaklığı (0-1), ARGB.</summary>
        public static double Luminance(uint argb)
        {
            double r = (argb >> 16) & 0xFF, g = (argb >> 8) & 0xFF, b = argb & 0xFF;
            return (0.2126 * r + 0.7152 * g + 0.0722 * b) / 255.0;
        }

        /// <summary>
        /// Koyu zeminde okunur krallık rengi (ARGB, tam opak): birinci renk çok koyuysa ikincisi daha açıksa o, değilse birinci beyazla
        /// %45 karıştırılır.
        /// </summary>
        public static uint Readable(uint primary, uint secondary)
        {
            const double dark = 0.22;
            uint pick = primary;
            if (Luminance(primary) < dark && Luminance(secondary) > Luminance(primary)) pick = secondary;
            if (Luminance(pick) < dark)
            {
                uint Mix(uint c) => (uint)Math.Round(c + (255 - c) * 0.45);
                pick = (Mix((pick >> 16) & 0xFF) << 16) | (Mix((pick >> 8) & 0xFF) << 8) | Mix(pick & 0xFF);
            }
            return 0xFF000000 | (pick & 0x00FFFFFF);
        }

        /// <summary>"bugün", "dün", "5 gün önce".</summary>
        public static string Ago(int days) => days <= 0 ? "bugün" : days == 1 ? "dün" : days + " gün önce";

        /// <summary>Oyunun günlük metnindeki bağlantı ve biçim etiketleri ("&lt;a ...&gt;", "&lt;b&gt;") atılır.</summary>
        public static string StripTags(string text) =>
            string.IsNullOrEmpty(text) ? "" : Regex.Replace(Regex.Replace(text, "<[^>]*>", ""), @"\s+", " ").Trim();

        /// <summary>"A, B" ya da "A, B, C +2"; boşsa "yok".</summary>
        public static string Names(IList<string> names, int max)
        {
            if (names == null || names.Count == 0) return "yok";
            string shown = string.Join(", ", names.Take(Math.Max(1, max)));
            return names.Count > max ? shown + " +" + (names.Count - max) : shown;
        }

        /// <summary>Oyunun barış puanına göre bir krallığın tavrı.</summary>
        public static string PeaceMood(float score) => score > 0 ? "barışa yatkın" : "barışa yanaşmıyor";
    }
}
