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

        public void SetFastForwardSpeed(int speed) => FastForwardSpeed = Allowed(AllowedSpeeds, speed, DefaultSpeed);
        public void SetSkillXpMultiplier(int value) => SkillXpMultiplier = Allowed(AllowedXpMultipliers, value, 1);
        public void SetTroopXpMultiplier(int value) => TroopXpMultiplier = Allowed(AllowedXpMultipliers, value, 1);
        public void SetPartySizeBonus(int value) => PartySizeBonus = Allowed(AllowedPartySizeBonuses, value, 0);
        public void SetIntrigueDifficulty(int value) => IntrigueDifficulty = Allowed(AllowedIntrigueDifficulties, value, 1);
        public void SetAiIntrigue(int value) => AiIntrigue = Allowed(AllowedIntrigueDifficulties, value, 1);
        public void SetPartyLimitBonus(int value) => PartyLimitBonus = value >= 0 && value <= MaxPartyLimitBonus ? value : 0;

        /// <summary>Bilinmeyen anahtarlar, yorumlar (#) ve bozuk satırlar yok sayılır.</summary>
        public static ModSettings Parse(string text)
        {
            var settings = new ModSettings();
            if (string.IsNullOrEmpty(text)) return settings;

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
                }
            }
            return settings;
        }

        public string Format() =>
            "# CheatMenu (Tac ve Hancer) ayarlari\n"
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
            + "PlayerCanBeTargeted=" + (PlayerCanBeTargeted ? "1" : "0") + "\n";

        private static int Allowed(int[] allowed, int value, int fallback) =>
            Array.IndexOf(allowed, value) >= 0 ? value : fallback;

        private static bool IsTrue(string value) => value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase);

        private static string Invariant(int value) => value.ToString(CultureInfo.InvariantCulture);
    }
}
