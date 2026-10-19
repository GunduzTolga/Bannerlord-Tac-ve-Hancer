using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Localization;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Klan ve krallık araçları. Oyunun eylem sınıfları doğrudan çağrılır; konsol hilelerinin ön koşulları
    /// (ör. hükümdarken başka krallığa katılamama) burada da uygulanır.
    /// </summary>
    public static class KingdomTools
    {
        private static Clan Me => Clan.PlayerClan ?? throw new InvalidOperationException("Oyuncu klanı bulunamadı");

        // ---- Listeler ----

        public static IEnumerable<Kingdom> LiveKingdoms() => Kingdom.All.Where(k => !k.IsEliminated);

        /// <summary>Savaş/barış tarafları: yaşayan krallıklar + (bağımsızsa) oyuncunun klanı.</summary>
        public static IEnumerable<IFaction> Factions()
        {
            foreach (Kingdom kingdom in LiveKingdoms()) yield return kingdom;
            if (Me.Kingdom == null) yield return Me;
        }

        public static IEnumerable<IFaction> PeacefulWith(IFaction a) =>
            Factions().Where(b => b != a && !FactionManager.IsAtWarAgainstFaction(a, b));

        public static IEnumerable<IFaction> AtWarWith(IFaction a) =>
            Factions().Where(b => b != a && FactionManager.IsAtWarAgainstFaction(a, b)
                && !FactionManager.IsAtConstantWarAgainstFaction(a, b));

        public static IEnumerable<Clan> MovableClans() =>
            Clan.All.Where(c => !c.IsEliminated && !c.IsBanditFaction && c != Clan.PlayerClan && c.Leader != null
                && (c.Kingdom == null || c.Kingdom.RulingClan != c));

        public static IEnumerable<Hero> LeaderCandidates(Clan clan) =>
            clan.Heroes.Where(h => h.IsAlive && !h.IsChild && h != clan.Leader);

        public static IEnumerable<Clan> RulerCandidates(Kingdom kingdom) =>
            kingdom.Clans.Where(c => c != kingdom.RulingClan && !c.IsEliminated && !c.IsUnderMercenaryService);

        // ---- Benim klanım ----

        public static void JoinAsVassal(Kingdom kingdom)
        {
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Krallık yönetirken başka krallığa katılamazsın");
            if (Me.Kingdom == kingdom && !Me.IsUnderMercenaryService)
                throw new InvalidOperationException("Zaten " + kingdom.Name + " vasalısın");
            ChangeKingdomAction.ApplyByJoinToKingdom(Me, kingdom, default, true);
            MenuUI.Success(kingdom.Name + " krallığına vasal olarak katıldın");
        }

        public static void JoinAsMercenary(Kingdom kingdom)
        {
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Krallık yönetirken paralı asker olamazsın");
            if (Me.Kingdom != null) throw new InvalidOperationException("Paralı asker olmak için önce krallığından ayrıl");
            ChangeKingdomAction.ApplyByJoinFactionAsMercenary(Me, kingdom, default, 50, true);
            MenuUI.Success(kingdom.Name + " için paralı asker oldun");
        }

        public static void LeaveKingdom()
        {
            Kingdom kingdom = Me.Kingdom ?? throw new InvalidOperationException("Bir krallıkta değilsin");
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Yönettiğin krallıktan ayrılamazsın");
            if (Me.IsUnderMercenaryService) ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(Me, true);
            else ChangeKingdomAction.ApplyByLeaveKingdom(Me, true);
            MenuUI.Success(kingdom.Name + " krallığından ayrıldın");
        }

        public static void CreateKingdom(string name)
        {
            if (Me.Kingdom != null) throw new InvalidOperationException("Krallık kurmak için önce krallığından ayrıl");
            Renamer.KingdomNameTexts(name, out TextObject fullName, out TextObject shortName);
            Campaign.Current.KingdomManager.CreateKingdom(fullName, shortName, Me.Culture, Me, null, null, null, null);
            MenuUI.Success("Krallığın kuruldu: " + (Me.Kingdom != null ? Me.Kingdom.Name : fullName));
        }

        public static void BecomeRuler()
        {
            Kingdom kingdom = Me.Kingdom ?? throw new InvalidOperationException("Bir krallıkta değilsin");
            if (Hero.MainHero.IsKingdomLeader) throw new InvalidOperationException("Zaten krallığın hükümdarısın");
            if (Me.IsUnderMercenaryService) throw new InvalidOperationException("Paralı askerken hükümdar olamazsın");
            ChangeRulingClanAction.Apply(kingdom, Me);
            MenuUI.Success(kingdom.Name + " krallığının hükümdarı oldun");
        }

        public static int MinTier => Campaign.Current.Models.ClanTierModel.MinClanTier;
        public static int MaxTier => Campaign.Current.Models.ClanTierModel.MaxClanTier;

        /// <summary>Tier'ın setter'ı private; kademe AddRenown içinde şöhrete göre yeniden hesaplanır.</summary>
        public static void SetTier(int tier)
        {
            Me.Renown = Campaign.Current.Models.ClanTierModel.GetRequiredRenownForTier(tier);
            Me.AddRenown(0f, true);
            MenuUI.Success("Klan kademesi: " + Me.Tier + " (şöhret " + Me.Renown.ToString("N0") + ")");
        }

        public static void AddRenown(int amount)
        {
            Me.AddRenown(amount, true);
            MenuUI.Success("Şöhret +" + amount + " (kademe " + Me.Tier + ")");
        }

        // ---- Herkes ----

        public static void DeclareWar(IFaction a, IFaction b)
        {
            if (FactionManager.IsAtWarAgainstFaction(a, b)) throw new InvalidOperationException(a.Name + " ile " + b.Name + " zaten savaşta");
            DeclareWarAction.ApplyByDefault(a, b);
            bool pactEnded = Bundle.DiplomacyCompat.EndNonAggressionPact(a as Kingdom, b as Kingdom);
            MenuUI.Success(a.Name + " ile " + b.Name + " artık savaşta" + (pactEnded ? " (aradaki saldırmazlık anlaşması bozuldu)" : ""));
        }

        public static void MakePeace(IFaction a, IFaction b)
        {
            if (!FactionManager.IsAtWarAgainstFaction(a, b)) throw new InvalidOperationException(a.Name + " ile " + b.Name + " savaşta değil");
            if (FactionManager.IsAtConstantWarAgainstFaction(a, b)) throw new InvalidOperationException("Bu iki taraf arasında barış mümkün değil");
            MakePeaceAction.Apply(a, b);
            MenuUI.Success(a.Name + " ile " + b.Name + " barış yaptı");
        }

        public static void MoveClanToKingdom(Clan clan, Kingdom kingdom)
        {
            if (clan == Clan.PlayerClan) throw new InvalidOperationException("Kendi klanın için 'Benim klanım' seçeneklerini kullan");
            if (clan.Kingdom != null && clan.Kingdom.RulingClan == clan)
                throw new InvalidOperationException(clan.Name + " kendi krallığının hükümdar klanı");
            if (clan.Kingdom == kingdom) throw new InvalidOperationException(clan.Name + " zaten " + kingdom.Name + " içinde");
            ChangeKingdomAction.ApplyByJoinToKingdom(clan, kingdom, default, true);
            MenuUI.Success(clan.Name + " artık " + kingdom.Name + " krallığında");
        }

        public static void ChangeClanLeader(Clan clan, Hero newLeader)
        {
            if (clan == Clan.PlayerClan)
                throw new InvalidOperationException("Kendi klanının liderini değiştirmek oyuncu karakterini değiştirir; desteklenmiyor");
            ChangeClanLeaderAction.ApplyWithSelectedNewLeader(clan, newLeader);
            MenuUI.Success(clan.Name + " klanının yeni lideri: " + newLeader.Name);
        }

        public static void ChangeRuler(Kingdom kingdom, Clan clan)
        {
            if (kingdom.RulingClan == clan) throw new InvalidOperationException(clan.Name + " zaten hükümdar klan");
            if (clan.IsUnderMercenaryService) throw new InvalidOperationException("Paralı asker klan hükümdar olamaz");
            ChangeRulingClanAction.Apply(kingdom, clan);
            MenuUI.Success(kingdom.Name + " krallığının yeni hükümdar klanı: " + clan.Name);
        }

        public static void DestroyKingdom(Kingdom kingdom)
        {
            if (kingdom == Me.Kingdom) throw new InvalidOperationException("Kendi krallığını yok edemezsin");
            DestroyKingdomAction.Apply(kingdom);
            MenuUI.Success(kingdom.Name + " yok edildi");
        }
    }
}
