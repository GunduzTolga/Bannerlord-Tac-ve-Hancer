using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Varis ve meşrulaştırma (P2.8, tasarım: docs/superpowers/specs/2026-10-19-p2-varis.md). Meşrulaştırma: başka bir klanda resmî
    /// babasıyla büyüyen gerçek çocuğunu oyunun evlat edinme işlemiyle (AdoptHeroAction) klanına alırsın; babalığın herkesçe bilinir.
    /// Çekilme: oyunun emeklilik işlemiyle (ApplyHeirSelectionAction.ApplyByRetirement) klanı seçtiğin varise devredip onunla devam edersin.
    /// </summary>
    public static class Dynasty
    {
        private static int Today => SchemeFactors.Today;
        private static bool InKingdom => Clan.PlayerClan.Kingdom != null;

        // ---- Meşrulaştırma ----

        /// <summary>Gerçek babası (ya da annesi) olduğun, başka bir klanda büyüyen çocuklar (gayrimeşru çocuk sırlarından).</summary>
        public static List<Hero> Bastards(IntrigueState state) =>
            state.Secrets.Where(s => s.Kind == SecretKind.Bastard && s.OtherId == Hero.MainHero.StringId && s.ChildId != null)
                .Select(s => Hero.Find(s.ChildId))
                .Where(c => c != null && c.IsAlive && c.Clan != Clan.PlayerClan)
                .Distinct()
                .ToList();

        private static Secret SecretOf(IntrigueState state, Hero child) =>
            state.Secrets.FirstOrDefault(s => s.Kind == SecretKind.Bastard && s.ChildId == child.StringId && s.OtherId == Hero.MainHero.StringId);

        public static string CostText() =>
            InKingdom ? DynastyRules.LegitimizeInfluence + " etki (hükümdarın onayı)" : SchemeFactors.Money(DynastyRules.LegitimizeGold) + " (kâtipler ve tanıklar)";

        public static string LegitimizeBlocked(IntrigueState state, Hero child)
        {
            if (child == null || !child.IsAlive) return "Hayatta değil";
            if (child.Clan == Clan.PlayerClan) return "Zaten klanında";
            if (SecretOf(state, child) == null) return "Gerçek ebeveyni olduğun bilinmiyor";
            if (child.IsPrisoner) return "Esir";
            if (Hero.MainHero.IsPrisoner) return "Esirken olmaz";
            if (InKingdom && Clan.PlayerClan.Influence < DynastyRules.LegitimizeInfluence) return "Yeterli etkin yok (" + DynastyRules.LegitimizeInfluence + ")";
            return !InKingdom && !SchemeFactors.CanAfford(DynastyRules.LegitimizeGold) ? "Yeterli altının yok" : null;
        }

        public static void Legitimize(IntrigueState state, Hero child)
        {
            string blocked = LegitimizeBlocked(state, child);
            if (blocked != null) throw new InvalidOperationException(blocked);
            int influence = DynastyRules.LegitimizeInfluenceCost(InKingdom);
            int gold = DynastyRules.LegitimizeGoldCost(InKingdom);
            if (influence > 0) ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -influence);
            if (gold > 0) SchemeFactors.Pay(gold);

            Secret secret = SecretOf(state, child);
            Hero other = Hero.Find(secret.TargetId);
            var parts = new List<string> { influence > 0 ? "etki -" + influence : SchemeFactors.Money(gold) + " harcandı" };
            if (!secret.Exposed)
            {
                parts.AddRange(RomanceSecrets.ExposeEffects(secret));
                foreach (Secret linked in RomanceSecrets.Linked(state, secret)) linked.Exposed = true;
            }
            if (other != null && other.IsAlive && other != Hero.MainHero)
            {
                SchemeOutcomes.Relate(other, -DynastyRules.MotherEmbarrassment);
                parts.Add(other.Name + " herkesin önünde utandı: seninle -" + DynastyRules.MotherEmbarrassment);
            }
            Clan from = child.Clan;
            AdoptHeroAction.Apply(child);
            if (child.Clan != Clan.PlayerClan) child.Clan = Clan.PlayerClan;
            SchemeOutcomes.Notify(state, "Meşrulaştırma: " + child.Name,
                child.Name + " artık senin klanında ve senin çocuğun olarak anılıyor" + (from != null ? " (" + from.Name + " klanından geldi)" : "") + ". "
                + "Aile ağacında resmî ebeveyni değişmez; ama babalığın artık bir sır değil.\n\nSonuç: " + string.Join(", ", parts) + ".",
                child, CardImages.Legitimize);
            state.AddLog(Today, "Meşrulaştırma: " + child.Name);
        }

        // ---- Çekilme ----

        /// <summary>Klanındaki yetişkin aile üyeleri; oyunun kendi varis sıralamasına göre.</summary>
        public static List<Hero> HeirCandidates()
        {
            Dictionary<Hero, int> ranking;
            try
            {
                ranking = Clan.PlayerClan.GetHeirApparents();
            }
            catch (Exception)
            {
                ranking = new Dictionary<Hero, int>();
            }
            return Clan.PlayerClan.Heroes
                .Where(h => h.IsAlive && h != Hero.MainHero && !h.IsChild && !h.IsPrisoner && h.Clan == Clan.PlayerClan)
                .OrderByDescending(h => ranking.TryGetValue(h, out int score) ? score : int.MinValue)
                .ThenByDescending(h => h.Age)
                .ToList();
        }

        public static string AbdicateBlocked()
        {
            if (Hero.MainHero.IsPrisoner) return "Esirken olmaz";
            MobileParty main = MobileParty.MainParty;
            if (main != null && (main.MapEvent != null || main.SiegeEvent != null)) return "Savaş ya da kuşatma sırasında olmaz";
            if (main?.Army != null) return "Bir ordudayken olmaz";
            return HeirCandidates().Count == 0 ? "Klanında yetişkin bir varis yok" : null;
        }

        /// <summary>Oyunun emeklilik işlemi: karakterin çekilir, oyuna varisinle devam edersin.</summary>
        public static void Abdicate(Hero heir)
        {
            string blocked = AbdicateBlocked();
            if (blocked != null) throw new InvalidOperationException(blocked);
            if (!HeirCandidates().Contains(heir)) throw new InvalidOperationException(heir.Name + " varis olamaz");
            UI.IntrigueScreen.Close();
            ApplyHeirSelectionAction.ApplyByRetirement(heir);
        }
    }
}
