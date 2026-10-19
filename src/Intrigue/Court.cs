using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Saray istihbaratı (P2.2): lordların kendi hükümdarlarıyla ilişkisini ne kadar bildiğin. Bilgi bedava değil:
    /// güncel rapor tam sayıyı, o krallıkta gölge ağı ya da aynı sarayda olmak söylentiyi (sözlü düzey) verir.
    /// </summary>
    public static class Court
    {
        public enum Knowledge
        {
            Unknown = 0,
            Rumor = 1,
            Exact = 2,
        }

        public static IEnumerable<Kingdom> Kingdoms() => Kingdom.All.Where(k => !k.IsEliminated && k.Leader != null);

        /// <summary>Hükümdar dışındaki klan liderleri: taraf değiştirme kararını onlar verir.</summary>
        public static List<Hero> ClanLeaders(Kingdom kingdom) =>
            kingdom.Clans.Where(c => !c.IsEliminated && c.Leader != null && c.Leader.IsAlive && c.Leader != kingdom.Leader && c != Clan.PlayerClan)
                .Select(c => c.Leader).ToList();

        public static Knowledge KnowledgeOf(IntrigueState state, Hero lord)
        {
            if (state.HasValidReport(lord.StringId, SchemeFactors.Today)) return Knowledge.Exact;
            Kingdom kingdom = lord.Clan?.Kingdom;
            if (kingdom == null) return Knowledge.Unknown;
            return kingdom == Clan.PlayerClan.Kingdom || HasNetworkIn(state, kingdom) ? Knowledge.Rumor : Knowledge.Unknown;
        }

        public static bool HasNetworkIn(IntrigueState state, Kingdom kingdom) =>
            state.Networks.Any(n => Settlement.Find(n.SettlementId)?.MapFaction == kingdom);

        /// <summary>Lordun hükümdarıyla ilişkisi (oyunun gerçek değeri); hükümdarı yoksa ya da kendisiyse 0.</summary>
        public static int RelationWithRuler(Hero lord)
        {
            Hero ruler = lord.Clan?.Kingdom?.Leader;
            return ruler == null || ruler == lord ? 0 : lord.GetRelation(ruler);
        }

        /// <summary>Bildiğin kadarıyla: "hoşnutsuz (-24)", "hoşnutsuz (söylenti)" ya da "bilinmiyor".</summary>
        public static string MoodText(IntrigueState state, Hero lord)
        {
            int relation = RelationWithRuler(lord);
            switch (KnowledgeOf(state, lord))
            {
                case Knowledge.Exact: return LetterRules.MoodName(relation) + " (" + SchemeFactors.Signed(relation) + ")";
                case Knowledge.Rumor: return LetterRules.MoodName(relation) + " (söylenti)";
                default: return "bilinmiyor";
            }
        }

        /// <summary>Saray için bilgi kaynağının açıklaması.</summary>
        public static string SourceText(IntrigueState state, Kingdom kingdom)
        {
            if (kingdom == Clan.PlayerClan.Kingdom) return "Kendi sarayın: lordların havasını söylentilerden biliyorsun; tam sayılar için rapor gerekir.";
            if (HasNetworkIn(state, kingdom)) return "Bu krallıkta gölge ağın var: muhbirlerin lordların havasını söylüyor; tam sayılar için rapor gerekir.";
            return "Bu sarayda kulağın yok: yalnızca hakkında güncel raporun olan lordları biliyorsun. "
                + "Bir şehrinde gölge ağı kurarsan herkesin havasını öğrenirsin.";
        }
    }
}
