namespace CheatMenu.Intrigue.Core
{
    /// <summary>Düellonun sebebi: sonucun etkisini belirler.</summary>
    public enum DuelReason
    {
        /// <summary>Aranızda husumet var (ilişki -10 ya da kötü).</summary>
        Honor = 1,
        /// <summary>Eşinin sevgilisi (ilişki ortaya çıktıysa).</summary>
        Affair = 2,
        /// <summary>Kan davası (yakınını öldürdüğünü ya da kaçırdığını biliyor).</summary>
        Feud = 3,
    }

    /// <summary>
    /// Düello (tasarım: docs/superpowers/specs/2026-10-19-duello.md): oyunun kendi arena düellosu görevi. Saf: oyunsuz test edilir.
    /// </summary>
    public static class DuelRules
    {
        /// <summary>Husumet sayılan ilişki: bundan iyiyse "onur" düellosu için sebep yok.</summary>
        public const int GrievanceRelation = -10;
        public const int CooldownDays = 30;
        public const int WinRenown = 10;
        public const int LoseRenown = 5;
        /// <summary>Reddeden korkak görünür: meydan okuyan ün kazanır.</summary>
        public const int RefuseRenown = 5;
        public const int RefuseRelation = 5;
        /// <summary>Aldatma düellosunu kazanırsan sevgili eşinden uzaklaşır.</summary>
        public const int AffairLoverSpousePenalty = 20;
        /// <summary>Sevgili düelloyu reddederse eşinin gözünden düşer.</summary>
        public const int AffairRefuseSpousePenalty = 10;
        /// <summary>Arenadaki iki tarafın canı (kimse ölmez; düşen kaybeder).</summary>
        public const float AgentHealth = 100f;

        /// <summary>
        /// Kabul olasılığı: Cesaret ve Onur kabul ettirir; senden nefret eden kabul eder; sen çok daha iyi dövüşüyorsan korkar;
        /// kan davasında intikam, aldatmada namus kabul ettirir. %5-95.
        /// </summary>
        public static int AcceptChance(int valor, int honor, int relationWithYou, int yourMelee, int theirMelee, DuelReason reason)
        {
            int value = 40 + 15 * valor + 10 * honor - relationWithYou / 4 - (yourMelee - theirMelee) / 4
                + (reason == DuelReason.Feud ? 20 : reason == DuelReason.Affair ? 10 : 0);
            return SchemeMath.Clamp(value, 5, 95);
        }

        public static string ReasonName(DuelReason reason) =>
            reason == DuelReason.Affair ? "aldatma" : reason == DuelReason.Feud ? "kan davası" : "onur";
    }
}
