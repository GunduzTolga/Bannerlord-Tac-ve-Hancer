namespace CheatMenu
{
    /// <summary>
    /// Adlandırma kutusuna yazılan adı temizler ve doğrular. Oyun API'sine bağımlı değildir;
    /// tests/CheatMenu.Tests altında oyunsuz test edilir.
    /// </summary>
    public static class NameRules
    {
        public const int MaxLength = 50;

        private static readonly char[] Forbidden = { '{', '}' };

        /// <param name="allowEmpty">Yerleşim ve partilerde boş ad "orijinal ada dön" demektir.</param>
        public static bool TryNormalize(string input, bool allowEmpty, out string name, out string error)
        {
            name = (input ?? string.Empty).Trim();
            error = null;

            if (name.Length == 0)
            {
                if (allowEmpty) return true;
                error = "Ad boş olamaz";
                return false;
            }
            if (name.Length > MaxLength)
            {
                error = "Ad en fazla " + MaxLength + " karakter olabilir";
                return false;
            }
            // TextObject süslü parantezleri değişken sanar; ad bozulur.
            if (name.IndexOfAny(Forbidden) >= 0)
            {
                error = "Ad { veya } içeremez";
                return false;
            }
            return true;
        }
    }
}
