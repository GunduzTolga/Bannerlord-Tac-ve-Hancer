using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Menu
{
    /// <summary>Klan & krallık araçları ve Bilgi araçları.</summary>
    public static partial class Categories
    {
        private const string IrreversibleWarning =
            "Bu işlem geri alınamaz. Önce oyunu kaydetmeni öneririm. Devam edilsin mi?";

        // ---- Klan & krallık ----

        private static void ShowPolitics()
        {
            Clan me = Clan.PlayerClan;
            ShowCategory("Klan & krallık", new List<MenuEntry>
            {
                MenuEntry.Submenu("Bir krallığa vasal olarak katıl...", () => PickKingdom("Vasal olarak katıl", KingdomTools.JoinAsVassal)),
                MenuEntry.Submenu("Bir krallığa paralı asker olarak katıl...", () => PickKingdom("Paralı asker ol", KingdomTools.JoinAsMercenary)),
                MenuEntry.Simple("Krallığımdan ayrıl" + (me.Kingdom != null ? " (" + me.Kingdom.Name + ")" : ""), KingdomTools.LeaveKingdom),
                MenuEntry.Submenu("Kendi krallığını kur...", AskCreateKingdom),
                MenuEntry.Submenu("Krallığımın hükümdarı ol", () => ConfirmThen("Hükümdar ol", KingdomTools.BecomeRuler, ShowPolitics)),
                MenuEntry.Range("Klan kademesi: " + me.Tier, KingdomTools.MinTier, KingdomTools.MaxTier, () => me.Tier, KingdomTools.SetTier),
                MenuEntry.Simple("+500 şöhret", () => KingdomTools.AddRenown(500)),
                MenuEntry.Submenu("Savaş ilan ettir...", () => PickTwoFactions("Savaş", KingdomTools.PeacefulWith, KingdomTools.DeclareWar)),
                MenuEntry.Submenu("Barış yaptır...", () => PickTwoFactions("Barış", KingdomTools.AtWarWith, KingdomTools.MakePeace)),
                MenuEntry.Submenu("Bir klanı bir krallığa sok...", PickClanToMove),
                MenuEntry.Submenu("Bir klanın liderini değiştir...", PickClanForLeader),
                MenuEntry.Submenu("Bir krallığın hükümdarını değiştir...", PickKingdomForRuler),
                MenuEntry.Submenu("Bir krallığı yok et...", PickKingdomToDestroy),
            }, ShowPolitics);
        }

        private static void PickKingdom(string title, Action<Kingdom> apply) =>
            Picker.Pick(title, KingdomTools.LiveKingdoms(), k => k.Name.ToString(),
                k =>
                {
                    MenuUI.SafeRun(title, () => apply(k));
                    MenuUI.Defer(ShowPolitics);
                },
                ShowPolitics);

        private static void AskCreateKingdom() =>
            MenuUI.Confirm("Krallık kur", IrreversibleWarning,
                () => MenuUI.AskName("Yeni krallığın adı", Clan.PlayerClan.Name.ToString(), false,
                    KingdomTools.CreateKingdom, ShowPolitics),
                ShowPolitics);

        private static void PickTwoFactions(string verb, Func<IFaction, IEnumerable<IFaction>> partners,
            Action<IFaction, IFaction> apply) =>
            Picker.Pick(verb + ": birinci taraf", KingdomTools.Factions(), FactionLabel,
                a => Picker.Pick(verb + ": " + a.Name + " ile kim?", partners(a), FactionLabel,
                    b =>
                    {
                        MenuUI.SafeRun(verb, () => apply(a, b));
                        MenuUI.Defer(ShowPolitics);
                    },
                    ShowPolitics),
                ShowPolitics);

        private static string FactionLabel(IFaction faction) =>
            faction == Clan.PlayerClan ? "[Benim klanım] " + faction.Name : faction.Name.ToString();

        private static void PickClanToMove() =>
            Picker.Pick("Hangi klan?", KingdomTools.MovableClans(), Lists.ClanLabel,
                clan => Picker.Pick(clan.Name + " hangi krallığa?", KingdomTools.LiveKingdoms().Where(k => k != clan.Kingdom),
                    k => k.Name.ToString(),
                    k =>
                    {
                        MenuUI.SafeRun("Klanı taşı", () => KingdomTools.MoveClanToKingdom(clan, k));
                        MenuUI.Defer(ShowPolitics);
                    },
                    ShowPolitics),
                ShowPolitics);

        private static void PickClanForLeader() =>
            Picker.Pick("Hangi klanın lideri?", Lists.Clans().Where(c => c != Clan.PlayerClan), Lists.ClanLabel,
                clan => Picker.Pick(clan.Name + " - yeni lider", KingdomTools.LeaderCandidates(clan), Lists.HeroLabel,
                    hero => ConfirmThen("Klan lideri değiştir", () => KingdomTools.ChangeClanLeader(clan, hero), ShowPolitics),
                    ShowPolitics),
                ShowPolitics);

        private static void PickKingdomForRuler() =>
            Picker.Pick("Hangi krallık?", KingdomTools.LiveKingdoms(), k => k.Name + " (hükümdar: " + k.RulingClan?.Name + ")",
                k => Picker.Pick(k.Name + " - yeni hükümdar klan", KingdomTools.RulerCandidates(k), Lists.ClanLabel,
                    clan => ConfirmThen("Hükümdar değiştir", () => KingdomTools.ChangeRuler(k, clan), ShowPolitics),
                    ShowPolitics),
                ShowPolitics);

        private static void PickKingdomToDestroy() =>
            Picker.Pick("Yok edilecek krallık", KingdomTools.LiveKingdoms().Where(k => k != Clan.PlayerClan.Kingdom),
                k => k.Name.ToString(),
                k => ConfirmThen(k.Name + " yok edilsin mi?", () => KingdomTools.DestroyKingdom(k), ShowPolitics),
                ShowPolitics);

        private static void ConfirmThen(string title, Action action, Action back) =>
            MenuUI.Confirm(title, IrreversibleWarning,
                () =>
                {
                    MenuUI.SafeRun(title, action);
                    MenuUI.Defer(back);
                },
                back);

        // ---- Bilgi ----

        private static void ShowInfo()
        {
            ShowCategory("Bilgi", new List<MenuEntry>
            {
                MenuEntry.Submenu("Bir lordu bul...", () => Picker.Pick("Bir lordu bul", Lists.LordsAndCompanions(false),
                    Lists.HeroLabel, MapInfo.FindHero, ShowInfo)),
                MenuEntry.Toggle("Harita sisini kaldır (herkesi göster)", () => CheatState.RevealMap, MapInfo.SetReveal),
            }, ShowInfo, "Listedeki konumlar gerçek konumdur (oyunun 'son görülme' bilgisi değil).");
        }
    }
}
