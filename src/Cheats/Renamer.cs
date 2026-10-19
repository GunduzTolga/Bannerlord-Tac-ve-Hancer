using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Adlandırma. Kahraman/klan/krallık için oyunun kendi ekranlarının metin şablonları kullanılır;
    /// yerleşim ve parti için kaydedilen "özel ad" alanı (PartyBase.CustomName).
    /// </summary>
    public static class Renamer
    {
        // ClanLordItemVM.OnNamingHeroOver ile aynı şablonlar.
        public static void RenameHero(Hero hero, string name)
        {
            int gender = hero.IsFemale ? 1 : 0;
            TextObject firstName = GameTexts.FindText("str_generic_character_firstname", null)
                .SetTextVariable("CHARACTER_FIRSTNAME", new TextObject(name, null))
                .SetTextVariable("CHARACTER_GENDER", gender);
            TextObject fullName = GameTexts.FindText("str_generic_character_name", null)
                .SetTextVariable("CHARACTER_NAME", new TextObject(name, null))
                .SetTextVariable("CHARACTER_GENDER", gender);
            hero.SetName(fullName, firstName);

            Army army = hero.PartyBelongedTo?.Army;
            if (army != null && army.LeaderParty?.Owner == hero) army.UpdateName();
            MenuUI.Success("Kahramanın yeni adı: " + name);
        }

        // ClanManagementVM.OnChangeClanNameDone ile aynı.
        public static void RenameClan(Clan clan, string name)
        {
            TextObject text = GameTexts.FindText("str_generic_clan_name", null)
                .SetTextVariable("CLAN_NAME", new TextObject(name, null));
            clan.ChangeClanName(text, text);
            MenuUI.Success("Klanın yeni adı: " + name);
        }

        // KingdomManagementVM.OnChangeKingdomNameDone ile aynı.
        public static void RenameKingdom(Kingdom kingdom, string name)
        {
            KingdomNameTexts(name, out TextObject fullName, out TextObject shortName);
            kingdom.ChangeKingdomName(fullName, shortName);
            MenuUI.Success("Krallığın yeni adı: " + kingdom.Name);
        }

        /// <summary>Oyunun krallık adı şablonları; krallık kurarken de kullanılır.</summary>
        internal static void KingdomNameTexts(string name, out TextObject fullName, out TextObject shortName)
        {
            TextObject input = new TextObject(name, null);
            fullName = GameTexts.FindText("str_generic_kingdom_name", null).SetTextVariable("KINGDOM_NAME", input);
            shortName = GameTexts.FindText("str_generic_kingdom_short_name", null).SetTextVariable("KINGDOM_SHORT_NAME", input);
        }

        /// <summary>Boş ad = orijinal ada dön. Yerleşimin asıl adı save'e yazılmadığı için ona dokunulmaz.</summary>
        public static void RenameSettlement(Settlement settlement, string name)
        {
            settlement.Party.SetCustomName(ToCustomName(name));
            MenuUI.Success(name.Length == 0 ? "Orijinal ada dönüldü: " + settlement.Name : "Yerleşimin yeni adı: " + name);
        }

        /// <summary>Boş ad = orijinal ada dön. Oyun bazı olaylarda (dağılma vb.) parti adını kendisi sıfırlayabilir.</summary>
        public static void RenameParty(MobileParty party, string name)
        {
            party.Party.SetCustomName(ToCustomName(name));
            MenuUI.Success(name.Length == 0 ? "Orijinal ada dönüldü: " + party.Name : "Partinin yeni adı: " + name);
        }

        // Sıfırlamada oyunun kendi yöntemi (DisbandPartyAction.CancelDisband) TextObject.GetEmpty().
        private static TextObject ToCustomName(string name) =>
            name.Length == 0 ? TextObject.GetEmpty() : new TextObject(name, null);
    }
}
