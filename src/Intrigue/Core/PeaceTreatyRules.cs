using System;
using System.Collections.Generic;
using System.Linq;

namespace CheatMenu.Intrigue.Core
{
    /// <summary>
    /// Barış Masası (tasarım: docs/superpowers/specs/2026-10-21-baris-masasi.md): pazarlık gücü, şartların ağırlığı, kabul olasılığı,
    /// elçinin getirdiği teklif. Saf: oyunsuz test edilir.
    /// Pazarlık gücü (puan): onların savaş yorgunluğu - seninki, güç oranı (en fazla ±40), başka cepheleri (her biri +10, en fazla 3),
    /// oyunun barış puanı (+15 / -15), elçi gönderdilerse +15, diplomatın Charm'ı (en fazla +15). Her şartın bir ağırlığı var; kabul = %50 + güç - ağırlık, %3-95.
    /// </summary>
    public static class PeaceTreatyRules
    {
        /// <summary>Reddedilen tekliften sonra aynı krallığa bu kadar gün yeniden teklif götürülmez.</summary>
        public const int RefuseCooldownDays = 7;
        /// <summary>Bir krallığın elçisi en fazla bu kadar günde bir gelir.</summary>
        public const int EnvoyCooldownDays = 21;
        /// <summary>Elçi gönderdilerse bu kadar gün masada eli zayıf: pazarlık gücüne +AskedBonus.</summary>
        public const int AskedDays = 5;
        public const int AskedBonus = 15;
        /// <summary>Altın ağırlıkları en az bu hazineye göre hesaplanır (yoksul hükümdarın küçük ödemesi abartılmasın).</summary>
        public const int MinTreasury = 20000;
        /// <summary>Elçi, kabul olasılığı en az bu olan teklifi getirir.</summary>
        public const int EnvoyOfferChance = 80;
        public const int RefuseRelation = -3;

        /// <summary>Haraç seçenekleri: günlük tutar ve gün (0: haraç yok).</summary>
        public static readonly IReadOnlyList<(int Daily, int Days)> TributeLevels = new[] { (0, 0), (100, 60), (250, 60), (500, 90), (1000, 120) };

        /// <summary>Asker seçenekleri (garnizonlarından, senin partine).</summary>
        public static readonly IReadOnlyList<int> TroopLevels = new[] { 0, 20, 50, 100, 150 };

        /// <param name="diplomatBonus">Diplomatın katkısı (DiplomatRules.PeaceBonus).</param>
        public static int Willingness(float theirExhaustion, float ourExhaustion, double strengthRatio, int otherWars, bool gameLeansToPeace, bool theyAsked,
            int diplomatBonus = 0)
        {
            double ratio = double.IsNaN(strengthRatio) || double.IsInfinity(strengthRatio) ? 1 : strengthRatio;
            double score = (theirExhaustion - ourExhaustion)
                + SchemeMath.Clamp((int)Math.Round((ratio - 1) * 40), -40, 40)
                + 10 * SchemeMath.Clamp(otherWars, 0, 3)
                + (gameLeansToPeace ? 15 : -15)
                + (theyAsked ? AskedBonus : 0)
                + Math.Max(0, diplomatBonus);
            return (int)Math.Round(score);
        }

        public static int Chance(int willingness, int weight) => SchemeMath.Clamp(50 + willingness - weight, 3, 95);

        private static int Share(double scale, double amount, int treasury) =>
            amount <= 0 ? 0 : (int)Math.Round(scale * amount / Math.Max(treasury, MinTreasury));

        /// <summary>Toplu altın: hükümdarın kesesine oranla (kesenin tamamı 100).</summary>
        public static int GoldWeight(int gold, int treasury) => Share(100, gold, treasury);

        /// <summary>Haraç zamana yayılır: toplamın keseye oranının %60'ı.</summary>
        public static int TributeWeight(int daily, int days, int treasury) => days <= 0 ? 0 : Share(60, (double)daily * days, treasury);

        /// <summary>Garnizonlarından asker: her 4 asker 1.</summary>
        public static int TroopWeight(int troops) => Math.Max(0, troops) / 4;

        /// <summary>Kale 30, şehir 55; hükümdarın kendi klanınınsa +15; son kaleleri/şehirleri giderse +50.</summary>
        public static int FiefWeight(bool town, bool rulersOwn, bool leavesThemLandless) =>
            (town ? 55 : 30) + (rulersOwn ? 15 : 0) + (leavesThemLandless ? 50 : 0);

        /// <summary>Senin verdiğin altın ağırlığı düşürür (kendi keselerine oranla, %80'i).</summary>
        public static int GiftRelief(int gold, int theirTreasury) => Share(80, gold, theirTreasury);

        /// <summary>Senin ödeyeceğin haraç ağırlığı düşürür (toplamın onların kesesine oranla %50'si).</summary>
        public static int TributeRelief(int daily, int days, int theirTreasury) => days <= 0 ? 0 : Share(50, (double)daily * days, theirTreasury);

        /// <summary>Toplu altın seçenekleri: 0, kesenin %5/15/30/50/75'i (500'e yuvarlanmış, 500'den azsa yok).</summary>
        public static List<int> GoldSteps(int treasury)
        {
            var steps = new List<int> { 0 };
            foreach (double share in new[] { 0.05, 0.15, 0.30, 0.50, 0.75 })
            {
                int gold = (int)(Math.Max(0, treasury) * share / 500) * 500;
                if (gold >= 500 && !steps.Contains(gold)) steps.Add(gold);
            }
            return steps;
        }

        /// <summary>Senin verebileceğin altın: 0, keseden 1.000 / 5.000 / 10.000 / 25.000 / 50.000 (yetenleri).</summary>
        public static List<int> GiftSteps(int yourGold) =>
            new[] { 0, 1000, 5000, 10000, 25000, 50000 }.Where(g => g <= Math.Max(0, yourGold)).ToList();

        /// <summary>Asker seçenekleri: alabileceğin (yer ve garnizonlarındaki asker) kadarı; aradaki son basamak tam sınır.</summary>
        public static List<int> TroopSteps(int available)
        {
            var steps = TroopLevels.Where(t => t <= Math.Max(0, available)).ToList();
            if (available > 0 && available < TroopLevels[TroopLevels.Count - 1] && !steps.Contains(available)) steps.Add(available);
            return steps;
        }

        /// <summary>Elçinin bu hafta gelme olasılığı: pazarlık gücün 30'dan azsa hiç; sonra %10-45.</summary>
        public static int EnvoyChance(int willingness) => willingness < 30 ? 0 : SchemeMath.Clamp(10 + (willingness - 30) / 2, 10, 45);

        /// <summary>
        /// Elçinin getirdiği toplu altın: elçi gönderdikleri için güçlenen pazarlığında kabul olasılığı EnvoyOfferChance'i geçen en büyük
        /// basamak (yoksa 0: yalnızca barış ister).
        /// </summary>
        public static int EnvoyGold(int willingness, int treasury)
        {
            int best = 0;
            foreach (int gold in GoldSteps(treasury))
                if (Chance(willingness, GoldWeight(gold, treasury)) >= EnvoyOfferChance) best = Math.Max(best, gold);
            return best;
        }
    }
}
