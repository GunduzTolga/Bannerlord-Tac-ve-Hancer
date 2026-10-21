using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Intrigue
{
    /// <summary>Divan koltukları (casus başı, mareşal, hazinedar, hekim, kâhya) ve genelevdeki iş (yönetici ya da çalışan yoldaş): biri aynı anda tek koltukta oturur.</summary>
    public static class Council
    {
        /// <summary>Kişinin oturduğu koltuk ("casus başın", "hazinedarın", "hekimin", "mareşalin", "kâhyan", "genelev yöneticin", "genelevde çalışıyor"); boşsa null.</summary>
        public static string SeatOf(IntrigueState state, Hero hero)
        {
            if (hero == null) return null;
            if (hero.StringId == state.SpymasterId) return "casus başın";
            if (hero.StringId == state.TreasurerId) return "hazinedarın";
            if (hero.StringId == state.PhysicianId) return "hekimin";
            if (hero.StringId == state.MarshalId) return "mareşalin";
            if (hero.StringId == state.StewardId) return "kâhyan";
            return NightHouses.RoleOf(state, hero);
        }

        /// <summary>Başka bir koltukta oturuyorsa atamayı reddeder.</summary>
        /// <param name="seat">Atanacak koltuğun SeatOf adı.</param>
        public static void EnsureFree(IntrigueState state, Hero hero, string seat)
        {
            string current = SeatOf(state, hero);
            if (current != null && current != seat)
                throw new InvalidOperationException(hero.Name + " zaten " + current + "; biri iki koltukta oturamaz");
        }

        /// <summary>Koltuk adayları: klanından yetişkinler (sen hariç), başka koltukta oturmayanlar ve şu anki koltuk sahibi.</summary>
        public static IEnumerable<Hero> Candidates(IntrigueState state, string seat) =>
            SchemeFactors.SpymasterCandidates().Where(h => SeatOf(state, h) == null || SeatOf(state, h) == seat);
    }
}
