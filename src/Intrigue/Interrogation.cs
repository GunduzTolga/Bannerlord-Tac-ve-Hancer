using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Esir sorgulama (P2.1, tasarım: docs/superpowers/specs/2026-10-19-p2-esir-sorgulama.md). Formüller Core.InterrogationRules'ta;
    /// burası oyundan girdileri toplar ve sonucu uygular: rapor, sır, efendinin raporu, bedeller.
    /// </summary>
    public static class Interrogation
    {
        public sealed class Prisoner
        {
            public Hero Hero;
            /// <summary>null: partinde.</summary>
            public Settlement Prison;

            public string WhereText => Prison == null ? "Partinde" : Prison.Name + " zindanında";
        }

        /// <summary>Partindeki ve klanının zindanlarındaki kahraman esirler.</summary>
        public static List<Prisoner> Prisoners()
        {
            var list = new List<Prisoner>();
            Add(list, PartyBase.MainParty?.PrisonRoster, null);
            foreach (Settlement settlement in Clan.PlayerClan.Settlements)
                if (settlement.Party != null) Add(list, settlement.Party.PrisonRoster, settlement);
            return list;
        }

        private static void Add(List<Prisoner> list, TroopRoster roster, Settlement prison)
        {
            if (roster == null) return;
            for (int i = 0; i < roster.Count; i++)
            {
                CharacterObject character = roster.GetCharacterAtIndex(i);
                Hero hero = character != null && character.IsHero ? character.HeroObject : null;
                if (hero == null || !hero.IsAlive || hero.IsChild || hero.Clan == Clan.PlayerClan) continue;
                if (list.Any(p => p.Hero == hero)) continue;
                list.Add(new Prisoner { Hero = hero, Prison = prison });
            }
        }

        /// <summary>null: şimdi sorgulanabilir; değilse neden.</summary>
        public static string BlockedReason(IntrigueState state, Prisoner prisoner)
        {
            int today = SchemeFactors.Today;
            if (state.InterrogatedUntil.TryGetValue(prisoner.Hero.StringId, out int until) && today < until)
                return "yakında sorgulandı; " + (until - today) + " gün sonra yeniden";
            if (Hero.MainHero.IsPrisoner) return "esirken sorgu yapamazsın";
            if (prisoner.Prison != null && MobileParty.MainParty?.CurrentSettlement != prisoner.Prison)
                return prisoner.Prison.Name + " uzakta: oraya git ya da esiri partine al";
            return null;
        }

        /// <summary>Baskıda kullanılan Roguery: seninki ya da (görevdeyse) casus başınınki, hangisi yüksekse.</summary>
        public static int Roguery(IntrigueState state)
        {
            int mine = Hero.MainHero.GetSkillValue(DefaultSkills.Roguery);
            Hero spymaster = SchemeFactors.Spymaster(state);
            return spymaster != null && !spymaster.IsPrisoner ? Math.Max(mine, SchemeFactors.Roguery(spymaster)) : mine;
        }

        public static int Chance(IntrigueState state, Hero prisoner, InterrogationMethod method) =>
            InterrogationRules.Chance(method, Hero.MainHero.GetSkillValue(DefaultSkills.Charm), Roguery(state),
                Hero.MainHero.GetRelation(prisoner), prisoner.GetTraitLevel(DefaultTraits.Calculating),
                prisoner.GetTraitLevel(DefaultTraits.Valor), prisoner.GetTraitLevel(DefaultTraits.Honor),
                SettingsStore.Current.IntrigueDifficulty);

        /// <summary>Kartta gösterilen seçenek metni: başarı, sır şansı ve bedel.</summary>
        public static string OptionLabel(IntrigueState state, Hero prisoner, InterrogationMethod method)
        {
            string chance = "başarı %" + Chance(state, prisoner, method);
            switch (method)
            {
                case InterrogationMethod.Talk:
                    return "Konuştur (Charm): " + chance + ", sır şansı %" + InterrogationRules.SecretChance(method, Roguery(state));
                case InterrogationMethod.Pressure:
                    return "Baskı yap (Roguery): " + chance + ", sır şansı %" + InterrogationRules.SecretChance(method, Roguery(state))
                        + "; ilişki -" + InterrogationRules.PressureRelationPenalty + ", Merhamet -";
                default:
                    return "Serbest bırakma karşılığı anlaşma: " + chance + "; esir (ve fidyesi) gider, ilişki +" + InterrogationRules.DealRelationBonus;
            }
        }

        public static void Interrogate(IntrigueState state, Prisoner prisoner, InterrogationMethod method)
        {
            string blocked = BlockedReason(state, prisoner);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Hero hero = prisoner.Hero;
            int today = SchemeFactors.Today;
            int roguery = Roguery(state);
            bool success = CheatState.IntrigueAlwaysSucceed || state.Roll100() <= Chance(state, hero, method);
            state.InterrogatedUntil[hero.StringId] = today + InterrogationRules.CooldownDays;

            var parts = new List<string>();
            if (success)
            {
                // Önce bilgi: anlaşmada esir bırakılınca konumu değişir.
                IntelReport report = SchemeOutcomes.CreateReport(hero, today);
                state.Reports.RemoveAll(r => r.TargetId == hero.StringId);
                state.Reports.Add(report);
                parts.Add(SchemeOutcomes.ReportText(report));
                parts.Add(SecretLine(state, hero, method, roguery));
                string liegeLine = LiegeLine(state, hero, method, today);
                if (liegeLine != null) parts.Add(liegeLine);
                Hero.MainHero.AddSkillXp(method == InterrogationMethod.Pressure ? DefaultSkills.Roguery : DefaultSkills.Charm,
                    InterrogationRules.SuccessXp);
            }
            else
                parts.Add(method == InterrogationMethod.Pressure
                    ? hero.Name + " korkmadı ya da korkmuş gibi yapıp işe yaramaz şeyler anlattı."
                    : hero.Name + " kibarca dinledi ama işe yarar hiçbir şey söylemedi.");

            parts.Add("Sonuç: " + ApplyCosts(state, hero, method));
            SchemeOutcomes.Notify(state, "Sorgu (" + InterrogationRules.Name(method) + "): " + hero.Name,
                string.Join("\n\n", parts), hero, CardImages.Interrogation);
            state.AddLog(today, "Sorgu (" + InterrogationRules.Name(method).ToLower(Tr.Culture) + "): " + hero.Name
                + (success ? "" : " - sonuçsuz"));
        }

        private static string SecretLine(IntrigueState state, Hero hero, InterrogationMethod method, int roguery)
        {
            if (!hero.IsLord) return "Sırlar yalnızca lordlarda aranır.";
            if (state.Roll100() > InterrogationRules.SecretChance(method, roguery)) return "Bir sırrını ağzından kaçırmadı.";
            SecretKind? kind = SchemeOutcomes.FindSecret(state, hero, state.Roll100, count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count));
            if (kind == null) return "Anlattıklarına bakılırsa kayda değer bir sırrı yok.";
            state.Secrets.Add(new Secret { TargetId = hero.StringId, Kind = kind.Value, Day = SchemeFactors.Today });
            return "SIR BULUNDU: " + SecretRules.Name(kind.Value) + " - " + SecretRules.Description(kind.Value);
        }

        /// <summary>Efendisi (klan lideri, o kendisiyse hükümdarı) hakkında konuştuysa onun raporu.</summary>
        private static string LiegeLine(IntrigueState state, Hero hero, InterrogationMethod method, int today)
        {
            Hero liege = hero.Clan?.Leader != null && hero.Clan.Leader != hero ? hero.Clan.Leader : hero.Clan?.Kingdom?.Leader;
            if (liege == null || liege == hero || !liege.IsAlive || liege == Hero.MainHero || liege.Clan == Clan.PlayerClan) return null;
            if (state.Roll100() > InterrogationRules.LiegeIntelChance(method)) return null;
            IntelReport report = SchemeOutcomes.CreateReport(liege, today);
            state.Reports.RemoveAll(r => r.TargetId == liege.StringId);
            state.Reports.Add(report);
            return "Efendisi " + liege.Name + " hakkında da konuştu: onun için de güncel bir raporun var (suikast ve kaçırma listesine girer).";
        }

        private static string ApplyCosts(IntrigueState state, Hero hero, InterrogationMethod method)
        {
            switch (method)
            {
                case InterrogationMethod.Pressure:
                    SchemeOutcomes.Relate(hero, -InterrogationRules.PressureRelationPenalty);
                    TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Mercy, InterrogationRules.PressureMercy);
                    string result = hero.Name + " ile ilişki -" + InterrogationRules.PressureRelationPenalty + ", Merhamet -";
                    Hero leader = hero.Clan?.Leader;
                    if (leader != null && leader != hero && leader.IsAlive && state.Roll100() <= InterrogationRules.ClanHearsChance)
                    {
                        SchemeOutcomes.Relate(leader, -InterrogationRules.ClanHearsPenalty);
                        result += "; klanı duydu: " + leader.Name + " ile ilişki -" + InterrogationRules.ClanHearsPenalty;
                    }
                    return result + ".";
                case InterrogationMethod.Deal:
                    if (hero.IsPrisoner) EndCaptivityAction.ApplyByReleasedByChoice(hero, Hero.MainHero);
                    SchemeOutcomes.Relate(hero, InterrogationRules.DealRelationBonus);
                    return "sözünü tuttun: " + hero.Name + " serbest, ilişki +" + InterrogationRules.DealRelationBonus + ".";
                default:
                    return "bir bedeli olmadı.";
            }
        }
    }
}
