using System.Globalization;

namespace CheatMenu
{
    /// <summary>
    /// "Özel miktar" kutusuna yazılan metni sayıya çevirir. Oyun API'sine bağımlı değildir;
    /// tests/CheatMenu.Tests altında oyunsuz test edilir.
    /// </summary>
    public static class AmountParser
    {
        public static bool TryParse(string input, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            if (!int.TryParse(input.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out int parsed)) return false;
            if (parsed <= 0) return false;
            value = parsed;
            return true;
        }

        /// <summary>Eksi değerlere izin veren, [min, max] aralığına sınırlı okuma (ilişki, kişilik, yaş vb.).</summary>
        public static bool TryParseInRange(string input, int min, int max, out int value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input)) return false;
            if (!int.TryParse(input.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out int parsed)) return false;
            if (parsed < min || parsed > max) return false;
            value = parsed;
            return true;
        }
    }
}
