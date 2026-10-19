using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ImageIdentifiers;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Partiye eklenebilecek askerler: kültür askerleri, paralı askerler, haydutlar, kervan muhafızları.
    /// Kahramanlar, şablonlar ve ansiklopedide gizli özel karakterler listelenmez.
    /// </summary>
    public static class TroopSpawner
    {
        private static readonly HashSet<Occupation> TroopOccupations = new HashSet<Occupation>
        {
            Occupation.Soldier, Occupation.Mercenary, Occupation.Bandit, Occupation.Gangster, Occupation.CaravanGuard,
        };

        public static IEnumerable<CharacterObject> All() =>
            CharacterObject.All.Where(troop => !troop.IsHero && !troop.IsTemplate && !troop.IsObsolete
                && !troop.HiddenInEncyclopedia && TroopOccupations.Contains(troop.Occupation));

        public static IEnumerable<CharacterObject> OfTier(int tier) => All().Where(troop => troop.Tier == tier);

        public static string TierLabel(int tier) => "Kademe " + tier + " (T" + tier + ")";

        public static string Label(CharacterObject troop) =>
            "[" + (troop.Culture != null ? troop.Culture.Name.ToString() : "kültürsüz") + "] " + troop.Name + " - " + Kind(troop);

        public static string LabelWithTier(CharacterObject troop) => "T" + troop.Tier + " " + Label(troop);

        /// <summary>Parti ekranındaki asker resmi (kültür renkleriyle).</summary>
        public static ImageIdentifier Image(CharacterObject troop)
        {
            try
            {
                return new CharacterImageIdentifier(CampaignUIHelper.GetCharacterCode(troop, false));
            }
            catch (Exception ex)
            {
                // Tek bir bozuk asker tüm listeyi açılmaz hale getirmesin; o satır resimsiz kalır.
                TaleWorlds.Library.Debug.Print("[CheatMenu] troop image failed: " + troop.StringId + " " + ex);
                return null;
            }
        }

        private static string Kind(CharacterObject troop) =>
            troop.IsMounted ? (troop.IsRanged ? "Atlı okçu" : "Süvari") : (troop.IsRanged ? "Okçu" : "Piyade");
    }
}
