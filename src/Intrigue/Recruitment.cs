using System;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Esiri hizmetine davet (tasarım: docs/superpowers/specs/2026-10-19-p2-esir-sorgulama.md, "Hizmetine davet"). Esir lord ya da leydi
    /// kendi isteğiyle klanına katılır: ona oyunun kendi görevlerini verirsin (parti, vali, kervan, casus başı). Reddederse esir kalır;
    /// fidye ve serbest bırakma oyunun kendi sistemi.
    /// </summary>
    public static class Recruitment
    {
        /// <summary>null: teklif edilebilir; değilse neden.</summary>
        public static string Blocked(IntrigueState state, Interrogation.Prisoner prisoner)
        {
            Hero hero = prisoner.Hero;
            int today = SchemeFactors.Today;
            if (Hero.MainHero.IsPrisoner) return "esirken teklif yapamazsın";
            if (!hero.IsLord || hero.Clan == null) return "yalnızca soylular";
            if (hero.IsChild || !SecretRules.IsAdult(RomanceSecrets.Age(hero))) return "çocuklara teklif yapılmaz";
            if (hero.IsClanLeader || hero.IsKingdomLeader) return "klan lideri klanını bırakmaz";
            if (hero.Spouse != null && hero.Spouse.IsAlive) return "evli: eşini bırakmaz";
            if (state.RecruitUntil.TryGetValue(hero.StringId, out int until) && today < until)
                return "teklifini reddetti; " + (until - today) + " gün sonra yeniden";
            if (prisoner.Prison != null && MobileParty.MainParty?.CurrentSettlement != prisoner.Prison)
                return prisoner.Prison.Name + " uzakta: oraya git ya da esiri partine al";
            return null;
        }

        public static int Chance(Hero hero)
        {
            Hero leader = hero.Clan?.Leader;
            return RecruitRules.Chance(Hero.MainHero.GetRelation(hero), leader != null && leader != hero ? hero.GetRelation(leader) : 0,
                hero.GetTraitLevel(DefaultTraits.Honor), hero.GetTraitLevel(DefaultTraits.Calculating),
                Hero.MainHero.GetSkillValue(DefaultSkills.Charm), Clan.PlayerClan.Tier, hero.Clan?.Tier ?? 0,
                SettingsStore.Current.IntrigueDifficulty);
        }

        /// <summary>Teklif: kabul ederse serbest kalır, klanına ve partine katılır; etmezse esir kalır.</summary>
        public static void Offer(IntrigueState state, Interrogation.Prisoner prisoner)
        {
            string blocked = Blocked(state, prisoner);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Hero hero = prisoner.Hero;
            int today = SchemeFactors.Today;
            int chance = Chance(hero);
            if (state.Roll100() > chance)
            {
                state.RecruitUntil[hero.StringId] = today + RecruitRules.CooldownDays;
                SchemeOutcomes.Notify(state, "Teklif reddedildi: " + hero.Name,
                    hero.Name + " teklifini dinledi ama " + (hero.Clan?.Name.ToString() ?? "ailesi") + " için esir kalmayı seçti. "
                    + "Esirin olarak kalıyor: fidye ya da serbest bırakma oyunun kendi seçenekleri. "
                    + RecruitRules.CooldownDays + " gün sonra yeniden sorabilirsin.",
                    hero, CardImages.Interrogation);
                state.AddLog(today, "Hizmet teklifi reddedildi: " + hero.Name);
                return;
            }

            Clan former = hero.Clan;
            Hero formerLeader = former?.Leader;
            if (hero.GovernorOf != null) ChangeGovernorAction.RemoveGovernorOf(hero);
            EndCaptivityAction.ApplyByReleasedByChoice(hero, Hero.MainHero);
            hero.Clan = Clan.PlayerClan;
            hero.UpdateHomeSettlement();
            hero.ChangeState(Hero.CharacterStates.Active);
            AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
            SchemeOutcomes.Relate(hero, RecruitRules.JoinRelation);
            string text = hero.Name + " sana bağlılık yemini etti ve klanına katıldı. Artık partinde: oyunun klan ekranından ona görev ver "
                + "(parti, vali, kervan) ya da entrika ekranında casus başı yap.";
            if (formerLeader != null && formerLeader.IsAlive && formerLeader != hero)
            {
                SchemeOutcomes.RelateBetween(hero, formerLeader, -RecruitRules.FormerLeaderPenalty);
                SchemeOutcomes.Relate(formerLeader, -RecruitRules.FormerLeaderPlayerPenalty);
                text += "\n\nSonuç: " + hero.Name + " ile ilişki +" + RecruitRules.JoinRelation + "; " + (former?.Name.ToString() ?? "eski klanı")
                    + " lideri " + formerLeader.Name + " bunu ihanet sayıyor (onunla -" + RecruitRules.FormerLeaderPenalty + ", seninle -"
                    + RecruitRules.FormerLeaderPlayerPenalty + ").";
            }
            SchemeOutcomes.Notify(state, "Hizmetine katıldı: " + hero.Name, text, hero, CardImages.Oath);
            state.AddLog(today, "Hizmetine katıldı: " + hero.Name + (former != null ? " (" + former.Name + " klanından)" : ""));
        }
    }
}
