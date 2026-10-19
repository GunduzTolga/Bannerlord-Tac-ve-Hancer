using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Kozun kullanımları (tasarım 8.1). Hem Entrika > Kozlar menüsü hem lord diyaloğu aynı listeyi kullanır.
    /// Bırakma dışındaki her talep kozu harcar ve hedefle ilişkiyi düşürür.
    /// </summary>
    public static class HookActions
    {
        public enum Use
        {
            Gold,
            Influence,
            ReleasePrisoners,
            DefectToMe,
            LeaveKingdom,
            Release,
        }

        public sealed class Option
        {
            public Use Use;
            public string Label;
            /// <summary>null: kullanılabilir; değilse neden.</summary>
            public string Blocked;
        }

        public static List<Option> Options(Hero target)
        {
            int gold = GoldDemand(target);
            List<Hero> prisoners = PrisonersToFree(target);
            Kingdom mine = Clan.PlayerClan.Kingdom;
            Kingdom theirs = target.Clan?.Kingdom;
            return new List<Option>
            {
                new Option
                {
                    Use = Use.Gold, Label = "Altın iste: " + SchemeFactors.Money(gold),
                    Blocked = gold < 500 ? "Kesesi zaten boş" : null,
                },
                new Option
                {
                    Use = Use.Influence, Label = "Etki iste: " + SchemeMath.HookInfluence,
                    Blocked = !target.IsClanLeader ? "Yalnızca klan liderinden"
                        : target.Clan.Influence < SchemeMath.HookInfluence ? "Klanının yeterli etkisi yok" : null,
                },
                new Option
                {
                    Use = Use.ReleasePrisoners, Label = "Esirlerimi serbest bıraksın (" + prisoners.Count + " kişi)",
                   
                    Blocked = prisoners.Count == 0 ? "Elinde senin klanından ya da krallığından esir yok" : null,
                },
                new Option
                {
                    Use = Use.DefectToMe, Label = "Krallığını bırakıp benimkine katılsın (" + SchemeMath.DefectionInfluenceCost + " etki)",
                   
                    Blocked = DefectBlocked(target, mine, theirs),
                },
                new Option
                {
                    Use = Use.LeaveKingdom, Label = "Krallığından ayrılsın",
                    Blocked = LeaveBlocked(target, theirs),
                },
                new Option
                {
                    Use = Use.Release, Label = "Kozu bırak (ilişki +5)",
                },
            };
        }

        public static void Apply(IntrigueState state, Hero target, Use use)
        {
            int today = SchemeFactors.Today;
            Hook hook = state.ActiveHook(target.StringId, today) ?? throw new InvalidOperationException("Bu kişi üzerinde geçerli bir kozun yok");
            Option option = Options(target).First(o => o.Use == use);
            if (option.Blocked != null) throw new InvalidOperationException(option.Blocked);

            state.Hooks.Remove(hook);
            string result;
            switch (use)
            {
                case Use.Gold:
                    int gold = GoldDemand(target);
                    GiveGoldAction.ApplyBetweenCharacters(target, Hero.MainHero, gold, true);
                    result = SchemeFactors.Money(gold) + " aldın";
                    break;
                case Use.Influence:
                    ChangeClanInfluenceAction.Apply(target.Clan, -SchemeMath.HookInfluence);
                    ChangeClanInfluenceAction.Apply(Clan.PlayerClan, SchemeMath.HookInfluence);
                    result = SchemeMath.HookInfluence + " etki aldın";
                    break;
                case Use.ReleasePrisoners:
                    List<Hero> prisoners = PrisonersToFree(target);
                    foreach (Hero prisoner in prisoners) EndCaptivityAction.ApplyByReleasedByChoice(prisoner, target);
                    result = prisoners.Count + " esir serbest bırakıldı";
                    break;
                case Use.DefectToMe:
                    Kingdom from = target.Clan.Kingdom;
                    ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -SchemeMath.DefectionInfluenceCost);
                    ChangeKingdomAction.ApplyByJoinToKingdomByDefection(target.Clan, from, Clan.PlayerClan.Kingdom, default, true);
                    result = target.Clan.Name + " klanı " + from.Name + " krallığını bırakıp " + Clan.PlayerClan.Kingdom.Name + " krallığına katıldı";
                    break;
                case Use.LeaveKingdom:
                    Kingdom left = target.Clan.Kingdom;
                    ChangeKingdomAction.ApplyByLeaveKingdom(target.Clan, true);
                    result = target.Clan.Name + " klanı " + left.Name + " krallığından ayrıldı";
                    break;
                default:
                    SchemeOutcomes.Relate(target, 5);
                    state.AddLog(today, "Koz bırakıldı: " + target.Name);
                    SchemeOutcomes.Notify(state, "Koz bırakıldı: " + target.Name, target.Name + " sana minnettar. İlişki +5.", target);
                    return;
            }

            SchemeOutcomes.Relate(target, -SchemeMath.HookUseRelationPenalty);
            state.AddLog(today, "Koz kullanıldı (" + target.Name + "): " + result);
            SchemeOutcomes.Notify(state, "Koz kullanıldı: " + target.Name,
                result + ".\n\n" + target.Name + " bunu unutmayacak: ilişki -" + SchemeMath.HookUseRelationPenalty + ".", target);
        }

        private static int GoldDemand(Hero target) =>
            Math.Min(target.Gold, Math.Max(500, target.Gold * SchemeMath.HookGoldPercent / 100));

        private static string DefectBlocked(Hero target, Kingdom mine, Kingdom theirs)
        {
            if (mine == null) return "Önce bir krallıkta olmalısın";
            if (!target.IsClanLeader) return "Yalnızca klan liderinden";
            if (theirs == null) return "Bir krallıkta değil";
            if (theirs == mine) return "Zaten senin krallığında";
            if (target.IsKingdomLeader) return "Hükümdar kendi krallığını bırakmaz";
            if (target.Clan.IsUnderMercenaryService) return "Paralı asker";
            if (Clan.PlayerClan.Influence < SchemeMath.DefectionInfluenceCost) return SchemeMath.DefectionInfluenceCost + " etkin yok";
            return null;
        }

        private static string LeaveBlocked(Hero target, Kingdom theirs)
        {
            if (!target.IsClanLeader) return "Yalnızca klan liderinden";
            if (theirs == null) return "Bir krallıkta değil";
            if (target.IsKingdomLeader) return "Hükümdar kendi krallığını bırakmaz";
            if (theirs == Clan.PlayerClan.Kingdom) return "Kendi krallığından koparmak yerine başka kozlar kullan";
            return null;
        }

        /// <summary>Hedefin partisinde ve klanının yerleşimlerinde tutulan, oyuncunun klanından ya da krallığından kahraman esirler.</summary>
        private static List<Hero> PrisonersToFree(Hero target)
        {
            var rosters = new List<TroopRoster>();
            if (target.PartyBelongedTo != null) rosters.Add(target.PartyBelongedTo.PrisonRoster);
            if (target.IsClanLeader)
                rosters.AddRange(target.Clan.Settlements.Where(s => s.Party != null).Select(s => s.Party.PrisonRoster));

            Kingdom mine = Clan.PlayerClan.Kingdom;
            var heroes = new List<Hero>();
            foreach (TroopRoster roster in rosters)
                for (int i = 0; i < roster.Count; i++)
                {
                    CharacterObject character = roster.GetCharacterAtIndex(i);
                    Hero hero = character.IsHero ? character.HeroObject : null;
                    if (hero != null && hero.IsAlive && (hero.Clan == Clan.PlayerClan || (mine != null && hero.Clan?.Kingdom == mine)))
                        heroes.Add(hero);
                }
            return heroes.Distinct().ToList();
        }
    }
}
