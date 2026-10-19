using System.Collections.Generic;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Entrika sisteminin kayda yazılan tüm durumu. Saf veri: oyun nesnesi tutmaz, kahramanlar StringId ile anılır.
    /// E0: yalnızca kayıt testi işareti. E1'de entrikalar, raporlar, sırlar eklenecek.
    /// </summary>
    public sealed class IntrigueState
    {
        public const int CurrentVersion = 1;

        /// <summary>E0 kayıt testi: oyuncunun bıraktığı işaret; kaydet-yükle sonrası durmalı.</summary>
        public string SaveTestMarker { get; set; }

        public string Write()
        {
            var records = new List<TextRecord> { new TextRecord("version").Add("v", CurrentVersion.ToString()) };
            if (!string.IsNullOrEmpty(SaveTestMarker)) records.Add(new TextRecord("savetest").Add("marker", SaveTestMarker));
            return TextRecords.Write(records);
        }

        /// <summary>Asla istisna atmaz: boş, bozuk ya da yeni sürümden metin boş/kısmi duruma döner.</summary>
        public static IntrigueState Parse(string text)
        {
            var state = new IntrigueState();
            List<TextRecord> records = TextRecords.Parse(text);
            state.SaveTestMarker = records.FirstOrDefault(r => r.Kind == "savetest")?.Get("marker");
            return state;
        }
    }
}
