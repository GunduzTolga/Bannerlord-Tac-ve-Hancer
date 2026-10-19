using System;
using System.Collections.Generic;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>Takipte bir seçeneğin dayandığı beceri (oyunun kendi becerileri).</summary>
    public enum TailSkill
    {
        None = 0,
        Roguery = 1,
        Charm = 2,
        Athletics = 3,
    }

    public sealed class TailOption
    {
        public string Label;
        public TailSkill Skill;
        /// <summary>Temel şüphe artışı; beceri, gece ve gölge ağı düşürür.</summary>
        public int Suspicion;
        public int Clues;
        public int Gold;
        /// <summary>"Yeterince gördün": takip burada, gördüklerinle biter.</summary>
        public bool Stop;
    }

    public sealed class TailScene
    {
        public int Id;
        public string Title;
        /// <summary>{hedef} hedefin adıyla değişir.</summary>
        public string Text;
        /// <summary>Son sahne (buluşma, mektup, defter): çekilişte en sona konur.</summary>
        public bool Climax;
        public TailOption[] Options;
    }

    /// <summary>Fark edilince: kaç, rol yap, muhafızı sustur.</summary>
    public enum TailEscape
    {
        Run = 1,
        Bluff = 2,
        Bribe = 3,
    }

    /// <summary>Sahnede takibin (deneysel 3B görev) sonucu.</summary>
    public enum TailSceneResult
    {
        /// <summary>Hedef sahnede hiç görünmedi: deneme sayılmaz.</summary>
        NotFound = 0,
        /// <summary>İzini kaybettin: sonuç yok, ceza yok.</summary>
        Lost = 1,
        Success = 2,
        /// <summary>Çok yakına sokuldun ve fark edildin.</summary>
        Spotted = 3,
    }

    /// <summary>
    /// Casusluğu kendin yap (P2.11, tasarım: docs/superpowers/specs/2026-10-19-p2-kendin-takip.md): hedefle aynı şehirdeysen onu bizzat
    /// takip edersin. Üç sahne (iki sokak, bir son sahne), her birinde üç seçenek; şüphe 100'e varırsa fark edilirsin. Saf: veri ve
    /// formüller; oyunsuz test edilir.
    /// </summary>
    public static class TailRules
    {
        public const int MaxSuspicion = 100;
        /// <summary>Hedef tetikteyse (yakın zamanda biri peşine düştü) takip bu şüpheyle başlar.</summary>
        public const int AlertedStart = 20;
        public const int StreetScenes = 2;
        /// <summary>Aynı hedefi tekrar takip etmek için beklenecek gün.</summary>
        public const int CooldownDays = 7;
        /// <summary>Fark edilen takip hedefi bu kadar gün tetikte tutar.</summary>
        public const int AlertDays = 60;
        /// <summary>Bu kadar ipucu toplanırsa aynı hedefe karşı süren casusluk entrikası bir aşama ilerler.</summary>
        public const int PhaseClues = 30;
        public const int XpPerScene = 80;
        public const int CaughtRelation = 15;
        public const int CaughtCrime = 20;
        public const int BribeRelation = 5;
        /// <summary>Sahnede takip (deneysel): fark edilirsen hedefle ilişki.</summary>
        public const int SceneSpottedRelation = 5;

        private static TailOption O(string label, TailSkill skill, int suspicion, int clues, int gold = 0) =>
            new TailOption { Label = label, Skill = skill, Suspicion = suspicion, Clues = clues, Gold = gold };

        private static TailOption Stop(string label) => new TailOption { Label = label, Stop = true };

        public static readonly TailScene[] All =
        {
            new TailScene
            {
                Id = 1, Title = "Çarşı",
                Text = "{hedef} iki muhafızıyla çarşıya indi; tezgâhların arasında ağır ağır yürüyor, arada bir tüccarlarla konuşuyor. "
                    + "Kalabalık seni saklıyor, ama onu da.",
                Options = new[]
                {
                    O("Uzaktan izle", TailSkill.Roguery, 10, 10),
                    O("Kalabalığa karışıp yaklaş", TailSkill.Roguery, 25, 25),
                    O("Bir sokak çocuğuna takip ettir", TailSkill.None, 5, 15, 30),
                },
            },
            new TailScene
            {
                Id = 2, Title = "Meyhane",
                Text = "{hedef} köşedeki bir meyhaneye girdi. Pencereden bakınca sırtını duvara vermiş, biriyle alçak sesle konuştuğunu görüyorsun.",
                Options = new[]
                {
                    O("Kapıda bekle", TailSkill.None, 3, 5),
                    O("İçeri gir, yan masaya otur", TailSkill.Roguery, 30, 30),
                    O("Meyhaneciye kese ver, kulak versin", TailSkill.None, 10, 20, 100),
                },
            },
            new TailScene
            {
                Id = 3, Title = "Tapınak",
                Text = "{hedef} tapınağa girdi ve bir rahiple uzun uzun konuştu. Rahip arada bir başını sallıyor; yüzü ciddi.",
                Options = new[]
                {
                    O("Arka sırada diz çök, dinle", TailSkill.Roguery, 20, 25),
                    O("Çıkışta rahiple sohbet et", TailSkill.Charm, 15, 20),
                    O("Bağış yap, rahibin dilini çöz", TailSkill.None, 5, 20, 150),
                },
            },
            new TailScene
            {
                Id = 4, Title = "Konak kapısı",
                Text = "{hedef} bir konağa girdi. Kapıda muhafızlar var; içeriden müzik ve kahkaha sesleri geliyor.",
                Options = new[]
                {
                    O("Karşı damdan pencereyi gözle", TailSkill.Athletics, 20, 20),
                    O("Hizmetkârlarla laflaş", TailSkill.Charm, 15, 20),
                    O("Uzakta bekle", TailSkill.None, 0, 5),
                },
            },
            new TailScene
            {
                Id = 5, Title = "Ara sokak",
                Text = "{hedef} adamlarını geride bırakıp dar bir ara sokağa saptı. Ya seni fark etti ya da kimsenin görmemesi gereken bir işi var.",
                Options = new[]
                {
                    O("Peşinden dal", TailSkill.Roguery, 35, 35),
                    O("Sokağın öbür ucuna dolaş", TailSkill.Athletics, 15, 20),
                    Stop("Yeterince gördün, bırak gitsin"),
                },
            },
            new TailScene
            {
                Id = 6, Title = "Gizli buluşma", Climax = true,
                Text = "{hedef} bir hanın arka odasına geçti. Az sonra yüzünü pelerinle saklayan biri geldi; kapı kapandı.",
                Options = new[]
                {
                    O("Kapıya kulak ver", TailSkill.Roguery, 30, 40),
                    O("Hancıya rüşvet ver, yan odayı tut", TailSkill.None, 15, 30, 200),
                    Stop("Yeterince gördün, çekil"),
                },
            },
            new TailScene
            {
                Id = 7, Title = "Mühürlü mektup", Climax = true,
                Text = "{hedef} bir ulağa mühürlü bir mektup verdi ve kulağına bir şey fısıldadı. Ulak kalabalığa karışıp şehir kapısına yürüyor.",
                Options = new[]
                {
                    O("Ulağı meyhanede sarhoş et", TailSkill.Charm, 20, 40, 50),
                    O("Ulağın çantasını kes", TailSkill.Roguery, 35, 45),
                    Stop("Bırak gitsin"),
                },
            },
            new TailScene
            {
                Id = 8, Title = "Hesap defterleri", Climax = true,
                Text = "{hedef} konaktaki çalışma odasında kâhyasıyla defterlere gömüldü. Kâhya her akşam defterleri kilitli bir sandığa koyuyor.",
                Options = new[]
                {
                    O("Gece odaya sız", TailSkill.Roguery, 40, 50),
                    O("Kâhyaya rüşvet ver", TailSkill.None, 15, 35, 250),
                    Stop("Bu kadarı yeter"),
                },
            },
        };

        /// <summary>İki farklı sokak sahnesi, sonra bir son sahne.</summary>
        public static List<TailScene> Draw(Func<int, int> pick)
        {
            List<TailScene> street = All.Where(s => !s.Climax).ToList();
            List<TailScene> climax = All.Where(s => s.Climax).ToList();
            var drawn = new List<TailScene>();
            while (drawn.Count < StreetScenes && street.Count > 0)
            {
                TailScene next = street[pick(street.Count)];
                street.Remove(next);
                drawn.Add(next);
            }
            if (climax.Count > 0) drawn.Add(climax[pick(climax.Count)]);
            return drawn;
        }

        /// <summary>
        /// Bir adımın şüphe artışı: beceri her 5 puanda %1 düşürür (en fazla %60), hedefin Roguery'si her 30 puanda +1, zar ±%25,
        /// gece %25 ve gölge ağı seviye başına %10 düşürür. Temel 0 ise 0; değilse en az 1.
        /// </summary>
        /// <param name="roll100">1..100; 50 ortalamadır.</param>
        public static int SuspicionGain(int baseValue, int skill, int targetRoguery, bool night, int networkLevel, int roll100)
        {
            if (baseValue <= 0) return 0;
            double value = baseValue * (100 - Math.Min(60, Math.Max(0, skill) / 5)) / 100.0;
            value += Math.Max(0, targetRoguery) / 30;
            value += baseValue * (SchemeMath.Clamp(roll100, 1, 100) - 50) / 200.0;
            if (night) value *= 0.75;
            value *= 1 - 0.1 * SchemeMath.Clamp(networkLevel, 0, 3);
            return Math.Max(1, (int)Math.Round(value, MidpointRounding.AwayFromZero));
        }

        /// <summary>Seçenek düğmesinde yazan aralık: en iyi ve en kötü zar.</summary>
        public static void SuspicionRange(int baseValue, int skill, int targetRoguery, bool night, int networkLevel, out int min, out int max)
        {
            min = SuspicionGain(baseValue, skill, targetRoguery, night, networkLevel, 1);
            max = SuspicionGain(baseValue, skill, targetRoguery, night, networkLevel, 100);
        }

        /// <summary>Takip sonunda sır bulma olasılığı: toplanan ipucunun üçte ikisi, en fazla %75.</summary>
        public static int SecretChance(int clues) => SchemeMath.Clamp(clues * 2 / 3, 0, 75);

        /// <summary>Fark edilince kurtulma olasılığı. Muhafızı susturmak her zaman tutar (bedeli BribeCost).</summary>
        public static int EscapeChance(TailEscape escape, int roguery, int athletics, int charm, bool night, bool alerted)
        {
            switch (escape)
            {
                case TailEscape.Run:
                    return SchemeMath.Clamp(30 + roguery / 5 + athletics / 10 + (night ? 15 : 0), 10, 90);
                case TailEscape.Bluff:
                    return SchemeMath.Clamp(25 + charm / 4 - (alerted ? 15 : 0), 5, 85);
                default:
                    return 100;
            }
        }

        public static int BribeCost(int clanTier) => 300 + 200 * Math.Max(0, clanTier);

        /// <summary>Sahnede takip (deneysel): hedefi 3-20 metre arasından toplam 60 saniye izlersen başarı; 3 metreden yakında 8 saniye kalırsan fark edilirsin.</summary>
        public const float SceneFollowMax = 20f;
        public const float SceneTooClose = 3f;
        public const float SceneFollowSeconds = 60f;
        public const float SceneSpottedSeconds = 8f;

        public static TailSceneResult SceneResult(bool targetSeen, float followSeconds, float tooCloseSeconds)
        {
            if (!targetSeen) return TailSceneResult.NotFound;
            if (tooCloseSeconds >= SceneSpottedSeconds) return TailSceneResult.Spotted;
            return followSeconds >= SceneFollowSeconds ? TailSceneResult.Success : TailSceneResult.Lost;
        }

        /// <summary>Sahnede başarılı takibin ipucu: 30 + her 10 saniye için 5, en fazla 70.</summary>
        public static int SceneClues(float followSeconds) => SchemeMath.Clamp(30 + (int)(followSeconds / 10) * 5, 30, 70);

        public static string SkillName(TailSkill skill)
        {
            switch (skill)
            {
                case TailSkill.Roguery: return "Roguery";
                case TailSkill.Charm: return "Charm";
                case TailSkill.Athletics: return "Athletics";
                default: return "";
            }
        }
    }
}
