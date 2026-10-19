namespace CheatMenu.Intrigue.Core
{
    /// <summary>Casus başının sürekli görevi (P2.3). Entrikalar sürerken de arka planda işler; aynı anda tek görev.</summary>
    public enum SpymasterMission
    {
        None = 0,
        /// <summary>Sana karşı kurulan entrikaları sezme şansı artar.</summary>
        CounterIntel = 1,
        /// <summary>Krallığındaki hoşnutsuz klan liderleri hakkında haftalık rapor.</summary>
        WatchDissent = 2,
        /// <summary>Seçilen lordun hükümdarıyla ilişkisini haftadan haftaya düşürür; yakalanma riski var.</summary>
        SpreadRumors = 3,
    }

    /// <summary>
    /// Casus başı görevlerinin formülleri (P2.3, tasarım: docs/superpowers/specs/2026-10-19-p2-casus-basi-gorevleri.md). Saf: test edilir.
    /// </summary>
    public static class MissionRules
    {
        /// <summary>Karşı casusluk: haftalık sezme şansına eklenir (en fazla %95).</summary>
        public const int CounterIntelBonus = 25;
        /// <summary>Söylenti: haftalık ücret (muhbirler, meyhane sohbetleri, rüşvet).</summary>
        public const int RumorWeeklyCost = 100;
        /// <summary>Söylentinin kaynağı ortaya çıkarsa: hedefle ilişki ve hükümdarıyla ilişki cezası.</summary>
        public const int RumorExposedTargetPenalty = 15;
        public const int RumorExposedRulerPenalty = 5;
        public const int MissionXpPerWeek = 50;

        public static int Detection(bool working, int roguery, SpymasterMission mission)
        {
            int chance = SchemeMath.DetectionChance(working, roguery);
            return working && mission == SpymasterMission.CounterIntel ? System.Math.Min(95, chance + CounterIntelBonus) : chance;
        }

        /// <summary>Muhalefeti izle: haftada kaç hoşnutsuz hakkında rapor (1-3).</summary>
        public static int DissentReportsPerWeek(int roguery) => SchemeMath.Clamp(1 + roguery / 150, 1, 3);

        /// <summary>Söylenti: hedefin hükümdarıyla ilişkisi haftada bu kadar düşer (1-4).</summary>
        public static int RumorRelationLoss(int roguery) => SchemeMath.Clamp(1 + roguery / 100, 1, 4);

        /// <summary>Söylenti: haftalık yakalanma olasılığı (2-12), zorluk ayarı entrikalardaki gibi ±3.</summary>
        public static int RumorDiscovery(int roguery, int difficulty)
        {
            int value = 8 - roguery / 50 + (difficulty == SchemeMath.EasyDifficulty ? -3 : difficulty == SchemeMath.HardDifficulty ? 3 : 0);
            return SchemeMath.Clamp(value, 2, 12);
        }

        public static string Name(SpymasterMission mission)
        {
            switch (mission)
            {
                case SpymasterMission.CounterIntel: return "Karşı casusluk";
                case SpymasterMission.WatchDissent: return "Muhalefeti izle";
                case SpymasterMission.SpreadRumors: return "Söylenti yay";
                default: return "Boşta";
            }
        }
    }
}
