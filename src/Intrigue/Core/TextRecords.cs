using System.Collections.Generic;
using System.Text;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>Tek kayıt: bir tür adı ve sıralı "alan=değer" çiftleri.</summary>
    public sealed class TextRecord
    {
        public TextRecord(string kind)
        {
            Kind = kind;
        }

        public string Kind { get; }
        public List<KeyValuePair<string, string>> Fields { get; } = new List<KeyValuePair<string, string>>();

        public TextRecord Add(string key, string value)
        {
            Fields.Add(new KeyValuePair<string, string>(key, value ?? string.Empty));
            return this;
        }

        /// <summary>Alan yoksa null.</summary>
        public string Get(string key)
        {
            foreach (KeyValuePair<string, string> field in Fields)
                if (field.Key == key) return field.Value;
            return null;
        }
    }

    /// <summary>
    /// Kayıt dosyasına yazılan düz metin biçimi: her satır bir kayıt, "tür\talan=değer\talan=değer".
    /// Değerlerde \ , sekme ve satır sonu kaçışlanır. Bilinmeyen türler/alanlar okuyanın işi; bozuk satırlar atlanır.
    /// Saf: oyunsuz test edilir.
    /// </summary>
    public static class TextRecords
    {
        public static string Write(IEnumerable<TextRecord> records)
        {
            var text = new StringBuilder();
            foreach (TextRecord record in records)
            {
                text.Append(Escape(record.Kind));
                foreach (KeyValuePair<string, string> field in record.Fields)
                    text.Append('\t').Append(Escape(field.Key)).Append('=').Append(Escape(field.Value));
                text.Append('\n');
            }
            return text.ToString();
        }

        public static List<TextRecord> Parse(string text)
        {
            var records = new List<TextRecord>();
            if (string.IsNullOrEmpty(text)) return records;

            foreach (string line in text.Split('\n'))
            {
                string[] parts = line.TrimEnd('\r').Split('\t');
                if (parts[0].Length == 0) continue;

                var record = new TextRecord(Unescape(parts[0]));
                for (int i = 1; i < parts.Length; i++)
                {
                    int separator = parts[i].IndexOf('=');
                    if (separator <= 0) continue;
                    record.Add(Unescape(parts[i].Substring(0, separator)), Unescape(parts[i].Substring(separator + 1)));
                }
                records.Add(record);
            }
            return records;
        }

        // "=" kaçışlanmaz: anahtarlar kodda sabit ve "=" içermez; değer ilk "="den sonrası olarak okunur.
        private static string Escape(string value) =>
            value.Replace("\\", "\\\\").Replace("\t", "\\t").Replace("\n", "\\n").Replace("\r", "\\r");

        private static string Unescape(string value)
        {
            if (value.IndexOf('\\') < 0) return value;
            var text = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                if (c != '\\' || i == value.Length - 1)
                {
                    text.Append(c);
                    continue;
                }
                char next = value[++i];
                text.Append(next == 't' ? '\t' : next == 'n' ? '\n' : next == 'r' ? '\r' : next);
            }
            return text.ToString();
        }
    }
}
