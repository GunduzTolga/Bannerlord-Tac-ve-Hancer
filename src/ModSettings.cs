using System;
using System.Globalization;

namespace CheatMenu
{
    /// <summary>
    /// Kalıcı mod ayarları ("Anahtar=Değer" satırları). Saf: dosya işi SettingsStore'da.
    /// Oyunsuz test edilir (tests/CheatMenu.Tests). İzinli listede olmayan değerler varsayılana döner.
    /// </summary>
    public sealed class ModSettings
    {
        public static readonly int[] AllowedSpeeds = { 4, 8, 16 };
        public static readonly int[] AllowedXpMultipliers = { 1, 2, 3, 5, 10 };
        public static readonly int[] AllowedPartySizeBonuses = { 0, 50, 100, 200, 500 };
        /// <summary>Entrika zorluğu: 0 Kolay, 1 Normal, 2 Zor (Intrigue.Core.SchemeMath ile aynı sayılar).</summary>
        public static readonly int[] AllowedIntrigueDifficulties = { 0, 1, 2 };
        public const int DefaultSpeed = 4;
        public const int MaxPartyLimitBonus = 100;

        /// <summary>
        /// Haritadaki "Entrika" düğmesinin yeri: 0 gizli, 1 sol üst, 2 üst orta, 3 sağ üst, 4 oyunun harita çubuğunda
        /// (sol alttaki Klan / Krallık düğmelerinin yanında). Yeni seçenekler sona eklenir: kayıtlı sıra numaraları kaymasın.
        /// </summary>
        public static readonly string[] MapButtonPositionNames = { "Gizli", "Sol üst", "Üst orta", "Sağ üst", "Harita çubuğunda" };
        public const int MapBarButtonPosition = 4;
        public const int DefaultMapButtonPosition = MapBarButtonPosition;
        /// <summary>Harita çubuğuna eklenemezse (oyun yaması) düğmenin yedek yeri: üst orta.</summary>
        public const int MapBarFallbackPosition = 2;

        /// <summary>
        /// Ayar dosyasının biçim sürümü. 2: varsayılan düğme yeri harita çubuğu oldu; sürüm satırı olmayan (eski) dosyada
        /// kayıtlı "Üst orta" (eski varsayılan) bir kez harita çubuğuna taşınır.
        /// </summary>
        public const int CurrentVersion = 2;

        /// <summary>Parse sırasında eski dosyadan taşıma yapıldı (MCM'in eski değeri geri yazmaması için).</summary>
        public bool Migrated { get; private set; }
        /// <summary>Entrika ekranı kısayolu (sıra numarasıyla saklanır; 0 = yok). Oyunun kendi harita tuşlarıyla çakışmayanlar.</summary>
        public static readonly string[] ScreenHotkeyNames = { "Yok", "Home", "End", "PageUp", "PageDown", "U", "Y", "O", "F9", "F10" };
        public const int DefaultScreenHotkey = 1;

        public int FastForwardSpeed { get; private set; } = DefaultSpeed;
        public int SkillXpMultiplier { get; private set; } = 1;
        public int TroopXpMultiplier { get; private set; } = 1;
        public int PartySizeBonus { get; private set; }
        public bool NoLearningLimit { get; set; }
        public bool NoCompanionLimit { get; set; }
        public int PartyLimitBonus { get; private set; }
        public bool NoPartyLimit { get; set; }
        public int IntrigueDifficulty { get; private set; } = 1;
        /// <summary>Yapay zekâ entrikaları: 0 Kapalı, 1 Az, 2 Normal (Intrigue.Core.SchemeMath.AiOff/AiFew/AiNormal).</summary>
        public int AiIntrigue { get; private set; } = 1;
        /// <summary>Yapay zekâ oyuncunun kendisine de suikast/kaçırma kurabilir mi (varsayılan kapalı).</summary>
        public bool PlayerCanBeTargeted { get; set; }
        public int MapButtonPosition { get; private set; } = DefaultMapButtonPosition;
        public int ScreenHotkey { get; private set; } = DefaultScreenHotkey;
        /// <summary>Entrika olayları resimli kartla mı (true) yoksa oyunun düz seçim penceresiyle mi sorulsun.</summary>
        public bool EventCards { get; set; } = true;
        /// <summary>Gizli ilişki ve gayrimeşru çocuk sırları (P2.4, sahnesiz). Kapalıyken bu sırlar bulunmaz.</summary>
        public bool RomanceSecrets { get; set; } = true;
        /// <summary>Gece evi (genelev) işletmesi: yetişkin teması, metin olarak. Varsayılan kapalı.</summary>
        public bool NightHouses { get; set; }
        /// <summary>Eşin kendi başına sevgili edinebilir (araştırınca ortaya çıkar). Varsayılan açık; gizli ilişki ayarıyla birlikte çalışır.</summary>
        public bool SpouseInfidelity { get; set; } = true;
        /// <summary>Kendin takip et (P2.11): kartlar yerine oyunun şehirde gizli takip görevini aç. Deneysel, varsayılan kapalı.</summary>
        public bool TailInScene { get; set; }
        /// <summary>Herkesi biç: öldürücü kılıç darbesi arkadakine geçer (CutThroughRules: 0 kapalı, 1 yalnızca ben, 2 herkes).</summary>
        public int CutThrough { get; private set; } = CutThroughRules.DefaultMode;

        public void SetCutThrough(int value) =>
            CutThrough = value >= 0 && value < CutThroughRules.ModeNames.Length ? value : CutThroughRules.DefaultMode;

        public void SetFastForwardSpeed(int speed) => FastForwardSpeed = Allowed(AllowedSpeeds, speed, DefaultSpeed);
        public void SetSkillXpMultiplier(int value) => SkillXpMultiplier = Allowed(AllowedXpMultipliers, value, 1);
        public void SetTroopXpMultiplier(int value) => TroopXpMultiplier = Allowed(AllowedXpMultipliers, value, 1);
        public void SetPartySizeBonus(int value) => PartySizeBonus = Allowed(AllowedPartySizeBonuses, value, 0);
        public void SetIntrigueDifficulty(int value) => IntrigueDifficulty = Allowed(AllowedIntrigueDifficulties, value, 1);
        public void SetAiIntrigue(int value) => AiIntrigue = Allowed(AllowedIntrigueDifficulties, value, 1);
        public void SetPartyLimitBonus(int value) => PartyLimitBonus = value >= 0 && value <= MaxPartyLimitBonus ? value : 0;
        public void SetMapButtonPosition(int value) =>
            MapButtonPosition = value >= 0 && value < MapButtonPositionNames.Length ? value : DefaultMapButtonPosition;
        public void SetScreenHotkey(int value) => ScreenHotkey = value >= 0 && value < ScreenHotkeyNames.Length ? value : DefaultScreenHotkey;

        /// <summary>Bilinmeyen anahtarlar, yorumlar (#) ve bozuk satırlar yok sayılır.</summary>
        public static ModSettings Parse(string text)
        {
            var settings = new ModSettings();
            if (string.IsNullOrEmpty(text)) return settings;

            int version = 1;
            foreach (string rawLine in text.Split('\n'))
            {
                string line = rawLine.Trim();
                int separator = line.IndexOf('=');
                if (line.StartsWith("#") || separator <= 0) continue;

                string key = line.Substring(0, separator).Trim();
                string value = line.Substring(separator + 1).Trim();
                bool isNumber = int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int number);
                switch (key)
                {
                    case "FastForwardSpeed": if (isNumber) settings.SetFastForwardSpeed(number); break;
                    case "SkillXpMultiplier": if (isNumber) settings.SetSkillXpMultiplier(number); break;
                    case "TroopXpMultiplier": if (isNumber) settings.SetTroopXpMultiplier(number); break;
                    case "PartySizeBonus": if (isNumber) settings.SetPartySizeBonus(number); break;
                    case "NoLearningLimit": settings.NoLearningLimit = IsTrue(value); break;
                    case "NoCompanionLimit": settings.NoCompanionLimit = IsTrue(value); break;
                    case "PartyLimitBonus": if (isNumber) settings.SetPartyLimitBonus(number); break;
                    case "NoPartyLimit": settings.NoPartyLimit = IsTrue(value); break;
                    case "IntrigueDifficulty": if (isNumber) settings.SetIntrigueDifficulty(number); break;
                    case "AiIntrigue": if (isNumber) settings.SetAiIntrigue(number); break;
                    case "PlayerCanBeTargeted": settings.PlayerCanBeTargeted = IsTrue(value); break;
                    case "MapButtonPosition": if (isNumber) settings.SetMapButtonPosition(number); break;
                    case "ScreenHotkey": if (isNumber) settings.SetScreenHotkey(number); break;
                    case "EventCards": settings.EventCards = IsTrue(value); break;
                    case "RomanceSecrets": settings.RomanceSecrets = IsTrue(value); break;
                    case "NightHouses": settings.NightHouses = IsTrue(value); break;
                    case "SpouseInfidelity": settings.SpouseInfidelity = IsTrue(value); break;
                    case "TailInScene": settings.TailInScene = IsTrue(value); break;
                    case "CutThrough": if (isNumber) settings.SetCutThrough(number); break;
                    case "SettingsVersion": if (isNumber) version = number; break;
                }
            }
            if (version < 2 && settings.MapButtonPosition == MapBarFallbackPosition)
            {
                settings.MapButtonPosition = MapBarButtonPosition;
                settings.Migrated = true;
            }
            return settings;
        }

        public string Format() =>
            "# CheatMenu (Tac ve Hancer) ayarlari\n"
            + "SettingsVersion=" + Invariant(CurrentVersion) + "\n"
            + "FastForwardSpeed=" + Invariant(FastForwardSpeed) + "\n"
            + "SkillXpMultiplier=" + Invariant(SkillXpMultiplier) + "\n"
            + "TroopXpMultiplier=" + Invariant(TroopXpMultiplier) + "\n"
            + "PartySizeBonus=" + Invariant(PartySizeBonus) + "\n"
            + "NoLearningLimit=" + (NoLearningLimit ? "1" : "0") + "\n"
            + "NoCompanionLimit=" + (NoCompanionLimit ? "1" : "0") + "\n"
            + "PartyLimitBonus=" + Invariant(PartyLimitBonus) + "\n"
            + "NoPartyLimit=" + (NoPartyLimit ? "1" : "0") + "\n"
            + "IntrigueDifficulty=" + Invariant(IntrigueDifficulty) + "\n"
            + "AiIntrigue=" + Invariant(AiIntrigue) + "\n"
            + "PlayerCanBeTargeted=" + (PlayerCanBeTargeted ? "1" : "0") + "\n"
            + "MapButtonPosition=" + Invariant(MapButtonPosition) + "\n"
            + "ScreenHotkey=" + Invariant(ScreenHotkey) + "\n"
            + "EventCards=" + (EventCards ? "1" : "0") + "\n"
            + "RomanceSecrets=" + (RomanceSecrets ? "1" : "0") + "\n"
            + "NightHouses=" + (NightHouses ? "1" : "0") + "\n"
            + "SpouseInfidelity=" + (SpouseInfidelity ? "1" : "0") + "\n"
            + "TailInScene=" + (TailInScene ? "1" : "0") + "\n"
            + "CutThrough=" + Invariant(CutThrough) + "\n";

        private static int Allowed(int[] allowed, int value, int fallback) =>
            Array.IndexOf(allowed, value) >= 0 ? value : fallback;

        private static bool IsTrue(string value) => value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);

        private static string Invariant(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
