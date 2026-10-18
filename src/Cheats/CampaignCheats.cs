using System;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Kampanyadaki tek seferlik hileler. Her metot sonucu ekrana yazar;
    /// istisnaları çağıran MenuUI.SafeRun yakalar.
    /// </summary>
    public static class CampaignCheats
    {
        private static Hero Me => Hero.MainHero ?? throw new InvalidOperationException("Ana karakter bulunamadı");
        private static MobileParty MyParty => MobileParty.MainParty ?? throw new InvalidOperationException("Ana parti bulunamadı");

        // ---- Para & Ekonomi ----

        public static void AddGold(int amount)
        {
            Me.ChangeHeroGold(amount);
            MenuUI.Success("Altın " + Signed(amount) + " (toplam " + Me.Gold.ToString("N0") + ")");
        }

        public static void AddInfluence(int amount)
        {
            Clan clan = Clan.PlayerClan ?? throw new InvalidOperationException("Oyuncu klanı bulunamadı");
            ChangeClanInfluenceAction.Apply(clan, amount);
            MenuUI.Success("Etki " + Signed(amount) + " (toplam " + clan.Influence.ToString("N0") + ")");
        }

        public static void AddGrain(int amount)
        {
            MyParty.ItemRoster.AddToCounts(DefaultItems.Grain, amount);
            MenuUI.Success(amount + " tahıl eklendi");
        }

        // ---- Karakter ----

        public static void AddFocusPoints(int amount)
        {
            var developer = Me.HeroDeveloper;
            developer.UnspentFocusPoints = Math.Max(0, developer.UnspentFocusPoints + amount);
            MenuUI.Success("Odak puanı " + Signed(amount) + " (harcanmamış " + developer.UnspentFocusPoints + ")");
        }

        public static void AddAttributePoints(int amount)
        {
            var developer = Me.HeroDeveloper;
            developer.UnspentAttributePoints = Math.Max(0, developer.UnspentAttributePoints + amount);
            MenuUI.Success("Attribute puanı " + Signed(amount) + " (harcanmamış " + developer.UnspentAttributePoints + ")");
        }

        public static void AddAllSkills(int levels)
        {
            foreach (SkillObject skill in Skills.All)
                Me.HeroDeveloper.ChangeSkillLevel(skill, levels, false);
            MenuUI.Success("Tüm yetenekler " + Signed(levels));
        }

        public static void HealHero()
        {
            Me.HitPoints = Me.MaxHitPoints;
            MenuUI.Success("Can dolduruldu");
        }

        // ---- Ordu / Parti ----

        public static void AddBasicTroops(int count) => AddTroops(Me.Culture?.BasicTroop, count);

        public static void AddEliteTroops(int count) => AddTroops(Me.Culture?.EliteBasicTroop, count);

        /// <summary>Her asker türüne, tamamının bir üst seviyeye terfi edebileceği kadar XP verir.</summary>
        public static void GiveUpgradeXp()
        {
            TroopRoster roster = MyParty.MemberRoster;
            int readyTypes = 0;
            for (int i = 0; i < roster.Count; i++)
            {
                TroopRosterElement element = roster.GetElementCopyAtIndex(i);
                CharacterObject troop = element.Character;
                if (troop.IsHero || troop.UpgradeTargets == null || troop.UpgradeTargets.Length == 0) continue;

                int needed = troop.GetUpgradeXpCost(PartyBase.MainParty, 0) * element.Number - roster.GetElementXp(i);
                if (needed <= 0) continue;
                roster.AddXpToTroop(troop, needed);
                readyTypes++;
            }
            MenuUI.Success(readyTypes + " asker türü terfiye hazır (parti ekranından terfi ettir)");
        }

        public static void HealWounded()
        {
            TroopRoster roster = MyParty.MemberRoster;
            int healed = 0;
            for (int i = 0; i < roster.Count; i++)
            {
                CharacterObject troop = roster.GetCharacterAtIndex(i);
                if (troop.IsHero)
                {
                    Hero hero = troop.HeroObject;
                    if (hero != null && hero.HitPoints < hero.MaxHitPoints)
                    {
                        hero.HitPoints = hero.MaxHitPoints;
                        healed++;
                    }
                    continue;
                }

                int wounded = roster.GetElementWoundedNumber(i);
                if (wounded <= 0) continue;
                roster.AddToCountsAtIndex(i, 0, -wounded, 0, false);
                healed += wounded;
            }
            MenuUI.Success(healed + " yaralı iyileştirildi");
        }

        public static void AddMorale(int amount)
        {
            MyParty.RecentEventsMorale += amount;
            MenuUI.Success("Moral " + Signed(amount) + " (şu an " + MyParty.Morale.ToString("0") + ")");
        }

        private static void AddTroops(CharacterObject troop, int count)
        {
            if (troop == null) throw new InvalidOperationException("Kültürünün bu tür askeri yok");
            if (count <= 0) throw new InvalidOperationException("Asker sayısı pozitif olmalı");
            MyParty.MemberRoster.AddToCounts(troop, count);
            MenuUI.Success(count + " x " + troop.Name + " eklendi");
        }

        private static string Signed(int value) => value.ToString("+#,0;-#,0;0");
    }
}
