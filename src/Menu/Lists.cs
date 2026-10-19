using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Menu
{
    /// <summary>Seçici listelerinin kaynakları ve satır etiketleri.</summary>
    public static class Lists
    {
        public static IEnumerable<Settlement> Settlements() =>
            Settlement.All.Where(s => s.IsTown || s.IsCastle || s.IsVillage);

        public static IEnumerable<Hero> LordsAndCompanions(bool includeChildren) =>
            Hero.AllAliveHeroes.Where(h => h != Hero.MainHero
                && (h.IsLord || h.IsWanderer || h.IsPlayerCompanion)
                && (includeChildren || !h.IsChild));

        public static IEnumerable<Hero> Notables() => Hero.AllAliveHeroes.Where(h => h.IsNotable);

        public static IEnumerable<Clan> Clans() => Clan.All.Where(c => !c.IsEliminated && !c.IsBanditFaction);

        public static IEnumerable<Kingdom> Kingdoms() => Kingdom.All.Where(k => !k.IsEliminated);

        public static IEnumerable<MobileParty> Parties() =>
            MobileParty.All.Where(p => p.IsActive && (p.IsMainParty || p.IsLordParty || p.IsCaravan));

        public static string SettlementLabel(Settlement s) =>
            s.Name + " (" + Kind(s) + (s.MapFaction != null ? ", " + s.MapFaction.Name : "") + ")";

        public static string HeroLabel(Hero h) =>
            h.Name + (h.Clan != null ? " (" + h.Clan.Name + ")" : "") + " - " + Where(h);

        public static string ClanLabel(Clan c) =>
            c.Name + " (" + (c.Kingdom != null ? c.Kingdom.Name.ToString() : "bağımsız") + ")";

        public static string PartyLabel(MobileParty p) =>
            p.Name + (p.LeaderHero != null && !p.IsMainParty ? " (" + p.LeaderHero.Name + ")" : "");

        private static string Kind(Settlement s) => s.IsTown ? "Şehir" : s.IsCastle ? "Kale" : "Köy";

        private static string Where(Hero h)
        {
            if (h.IsPrisoner) return "esir";
            MobileParty party = h.PartyBelongedTo;
            if (party?.CurrentSettlement != null) return party.CurrentSettlement.Name.ToString();
            if (party != null) return party.IsCurrentlyAtSea ? "denizde" : "seferde";
            if (h.CurrentSettlement != null) return h.CurrentSettlement.Name.ToString();
            return "bilinmiyor";
        }
    }
}
