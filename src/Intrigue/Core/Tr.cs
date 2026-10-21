using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Özel isimlere kesme işaretiyle Türkçe hâl eki: ünlü uyumu (son ünlüye göre), ünlüyle bitenlerde kaynaştırma
    /// (n/y), bulunma ekinde sertleşme (de/te). "Caladog'un", "Rhagaea'ya", "Sargot'ta". Saf: test edilir.
    /// </summary>
    public static class Tr
    {
        private const string Vowels = "aıoueiöüâîû";
        private const string HardConsonants = "fstkçşhp";
        private static readonly CultureInfo Turkish = new CultureInfo("tr-TR");

        /// <summary>Büyük/küçük harf çevirisi için (i/İ, ı/I doğru olsun).</summary>
        public static CultureInfo Culture => Turkish;

        /// <summary>İlgi hâli: Derthert'in, Caladog'un, Rhagaea'nın.</summary>
        public static string Genitive(string name) => Suffix(name, (v, vowelEnd) => (vowelEnd ? "n" : "") + FourWay(v) + "n");

        /// <summary>Yönelme hâli: Derthert'e, Caladog'a, Rhagaea'ya.</summary>
        public static string Dative(string name) => Suffix(name, (v, vowelEnd) => (vowelEnd ? "y" : "") + TwoWay(v));

        /// <summary>Belirtme hâli: Derthert'i, Caladog'u, Rhagaea'yı.</summary>
        public static string Accusative(string name) => Suffix(name, (v, vowelEnd) => (vowelEnd ? "y" : "") + FourWay(v));

        /// <summary>Bulunma hâli: Pravend'de, Sargot'ta, Rhagaea'da.</summary>
        public static string Locative(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            char last = char.ToLower(name[name.Length - 1], Turkish);
            return Suffix(name, (v, vowelEnd) => (HardConsonants.IndexOf(last) >= 0 ? "t" : "d") + TwoWay(v));
        }

        private static readonly Regex TimarWord = new Regex(@"\b(tımar|Tımar|TIMAR)(\w*)", RegexOptions.CultureInvariant);

        /// <summary>
        /// Oyunun Türkçesindeki "tımar"ı "toprak" yapar (kullanıcı kararı, CLAUDE.md > Terimler), ekleriyle ve büyük/küçük harfine göre:
        /// Tımarlar → Topraklar, tımarı → toprağı, tımarsız → topraksız, TIMAR → TOPRAK. "tımar" geçmeyen metin olduğu gibi döner.
        /// </summary>
        public static string TimarToToprak(string text)
        {
            if (string.IsNullOrEmpty(text) || (text.IndexOf("ımar", StringComparison.Ordinal) < 0 && text.IndexOf("IMAR", StringComparison.Ordinal) < 0))
                return text;
            return TimarWord.Replace(text, m =>
            {
                string word = m.Groups[1].Value, rest = m.Groups[2].Value;
                bool upper = word == "TIMAR";
                string stem = upper ? "TOPRA" : word[0] == 'T' ? "Topra" : "topra";
                bool softens = rest.Length > 0 && (Vowels + "AIOUEİÖÜÂÎÛ").IndexOf(rest[0]) >= 0;
                return stem + (softens ? (upper ? "Ğ" : "ğ") : (upper ? "K" : "k")) + rest;
            });
        }

        private static string Suffix(string name, System.Func<char, bool, string> suffix)
        {
            if (string.IsNullOrEmpty(name)) return name;
            string lower = name.ToLower(Turkish);
            char lastVowel = 'e';
            for (int i = lower.Length - 1; i >= 0; i--)
            {
                if (Vowels.IndexOf(lower[i]) < 0) continue;
                lastVowel = lower[i];
                break;
            }
            bool vowelEnd = Vowels.IndexOf(lower[lower.Length - 1]) >= 0;
            return name + "'" + suffix(lastVowel, vowelEnd);
        }

        private static bool IsBack(char vowel) => "aıouâû".IndexOf(vowel) >= 0;

        private static string TwoWay(char vowel) => IsBack(vowel) ? "a" : "e";

        private static string FourWay(char vowel)
        {
            switch (vowel)
            {
                case 'a': case 'ı': case 'â': return "ı";
                case 'o': case 'u': case 'û': return "u";
                case 'ö': case 'ü': return "ü";
                default: return "i";
            }
        }
    }
}
