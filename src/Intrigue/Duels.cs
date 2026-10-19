using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Locations;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Düello (tasarım: docs/superpowers/specs/2026-10-19-duello.md). Oyunun kendi arena düellosu görevi (SandBoxMissions.OpenArenaDuelMission)
    /// açılır; kazananı oyun bildirir. Aynı şehirde olmalısınız ve aranızda bir husumet olmalı (kötü ilişki, kan davası ya da eşinin
    /// sevgilisi). Arenada kimse ölmez: düşen kaybeder. Sonuç sebebe göre: aldatma biter, kan davası kapanır, ün kazanılır ya da kaybedilir.
    /// </summary>
    public static class Duels
    {
        private static int Today => SchemeFactors.Today;

        public static DuelReason ReasonFor(IntrigueState state, Hero target)
        {
            SpouseAffair affair = state.SpouseAffair;
            if (affair != null && affair.Discovered && affair.LoverId == target.StringId) return DuelReason.Affair;
            if (state.FeudUntil.TryGetValue(target.StringId, out int until) && until > Today) return DuelReason.Feud;
            return DuelReason.Honor;
        }

        /// <summary>Düellonun yapılacağı şehir: senin bulunduğun, arenası olan şehir; yoksa null.</summary>
        private static Settlement Town()
        {
            Settlement town = Settlement.CurrentSettlement;
            return town != null && town.IsTown && town.LocationComplex?.GetLocationWithId("arena") != null ? town : null;
        }

        private static bool InTown(Hero hero, Settlement town) =>
            hero.CurrentSettlement == town || hero.PartyBelongedTo?.CurrentSettlement == town;

        /// <summary>null: meydan okunabilir; değilse neden.</summary>
        public static string Blocked(IntrigueState state, Hero target)
        {
            if (target == null || !target.IsAlive) return "Hayatta değil";
            if (!target.IsLord || target.Clan == null || target.Clan == Clan.PlayerClan || target.IsChild) return "Yalnızca başka klanların yetişkin soyluları";
            if (target.IsPrisoner) return "Esir";
            if (Hero.MainHero.IsPrisoner || Hero.MainHero.IsWounded) return "Şu an dövüşemezsin";
            if (state.DuelUntil.TryGetValue(target.StringId, out int until) && Today < until) return "Yakında düello edildi; " + (until - Today) + " gün sonra";
            if (ReasonFor(state, target) == DuelReason.Honor && Hero.MainHero.GetRelation(target) > DuelRules.GrievanceRelation)
                return "Aranızda bir husumet yok (ilişki " + DuelRules.GrievanceRelation + " ya da kötü olmalı)";
            Settlement town = Town();
            if (town == null) return "Arenası olan bir şehirde olmalısın";
            if (!InTown(target, town)) return "Aynı şehirde olmalısınız (" + (target.CurrentSettlement?.Name.ToString() ?? "yeri belli değil") + "; Haritada izle)";
            return null;
        }

        private static int Melee(Hero hero) =>
            new[] { DefaultSkills.OneHanded, DefaultSkills.TwoHanded, DefaultSkills.Polearm }.Max(hero.GetSkillValue);

        public static int AcceptChance(IntrigueState state, Hero target) =>
            DuelRules.AcceptChance(target.GetTraitLevel(DefaultTraits.Valor), target.GetTraitLevel(DefaultTraits.Honor), target.GetRelation(Hero.MainHero),
                Melee(Hero.MainHero), Melee(target), ReasonFor(state, target));

        /// <summary>Kartta yazan sonuçlar.</summary>
        public static string Stakes(IntrigueState state, Hero target)
        {
            switch (ReasonFor(state, target))
            {
                case DuelReason.Affair:
                    return "Kazanırsan " + target.Name + " eşinden uzak durur: ilişki biter. Kaybedersen sürer ve rezil olursun.";
                case DuelReason.Feud:
                    return "Kazanırsan kan davası kapanır; sana karşı kurduğu entrikalardan da vazgeçer. Kaybedersen sürer.";
                default:
                    return "Kazanırsan ün +" + DuelRules.WinRenown + "; kaybedersen ün -" + DuelRules.LoseRenown + ".";
            }
        }

        /// <summary>Meydan okuma: reddederse korkak görünür; kabul ederse arena düellosu hemen başlar.</summary>
        public static void Challenge(IntrigueState state, Hero target)
        {
            string blocked = Blocked(state, target);
            if (blocked != null) throw new InvalidOperationException(blocked);
            DuelReason reason = ReasonFor(state, target);
            state.DuelUntil[target.StringId] = Today + DuelRules.CooldownDays;
            if (state.Roll100() > AcceptChance(state, target))
            {
                Refused(state, target, reason);
                return;
            }
            Settlement town = Town();
            Location arena = town.LocationComplex.GetLocationWithId("arena");
            string scene = town.LocationComplex.GetScene("arena", town.Town.GetWallLevel());
            state.AddLog(Today, "Düello (" + DuelRules.ReasonName(reason) + "): " + target.Name + ", " + town.Name + " arenası");
            UI.IntrigueScreen.Close();
            SandBoxMissions.OpenArenaDuelMission(scene, arena, target.CharacterObject, false, false,
                winner => MenuUI.SafeRun("Düello sonucu", () => Ended(target, reason, winner)), DuelRules.AgentHealth, "");
        }

        private static void Refused(IntrigueState state, Hero target, DuelReason reason)
        {
            var parts = new List<string>();
            GainRenownAction.Apply(Hero.MainHero, DuelRules.RefuseRenown, true);
            parts.Add("ün +" + DuelRules.RefuseRenown);
            SchemeOutcomes.Relate(target, -DuelRules.RefuseRelation);
            parts.Add(target.Name + " ile ilişki -" + DuelRules.RefuseRelation);
            Hero spouse = Courtships.Spouse;
            if (reason == DuelReason.Affair && spouse != null)
            {
                SchemeOutcomes.RelateBetween(spouse, target, -DuelRules.AffairRefuseSpousePenalty);
                parts.Add("eşin onu korkak buldu: aralarında -" + DuelRules.AffairRefuseSpousePenalty);
            }
            SchemeOutcomes.Notify(state, "Düello reddedildi: " + target.Name,
                target.Name + " meydan okumanı herkesin önünde geri çevirdi. Kimse onu cesur bulmadı.\n\nSonuç: " + string.Join(", ", parts) + ".",
                target, CardImages.Duel);
            state.AddLog(Today, "Düello reddedildi: " + target.Name);
        }

        private static void Ended(Hero target, DuelReason reason, CharacterObject winner)
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            bool won = winner == CharacterObject.PlayerCharacter;
            var parts = new List<string>();
            if (won)
            {
                GainRenownAction.Apply(Hero.MainHero, DuelRules.WinRenown, true);
                parts.Add("ün +" + DuelRules.WinRenown);
                if (reason == DuelReason.Affair && state.SpouseAffair != null && state.SpouseAffair.LoverId == target.StringId)
                {
                    Hero spouse = Hero.Find(state.SpouseAffair.SpouseId);
                    state.SpouseAffair = null;
                    if (spouse != null && spouse.IsAlive) SchemeOutcomes.RelateBetween(spouse, target, -DuelRules.AffairLoverSpousePenalty);
                    parts.Add(target.Name + " eşinden uzak duracak: gizli ilişki bitti");
                }
                if (reason == DuelReason.Feud)
                {
                    state.FeudUntil.Remove(target.StringId);
                    int plots = state.AiPlots.RemoveAll(p => p.PlotterId == target.StringId);
                    parts.Add("kan davası kapandı" + (plots > 0 ? "; sana karşı entrikasından vazgeçti" : ""));
                }
            }
            else
            {
                GainRenownAction.Apply(Hero.MainHero, -DuelRules.LoseRenown, true);
                parts.Add("ün -" + DuelRules.LoseRenown);
                if (reason == DuelReason.Affair) parts.Add("gizli ilişki sürüyor ve herkes konuşuyor");
            }
            SchemeOutcomes.Notify(state, (won ? "Düelloyu kazandın: " : "Düelloyu kaybettin: ") + target.Name,
                (won ? target.Name + " arenanın kumuna düştü; zafer senin." : "Arenanın kumuna düşen sendin; " + target.Name + " kazandı.")
                + " Kimse ölmedi.\n\nSonuç: " + string.Join(", ", parts) + ".",
                target, CardImages.Duel);
            state.AddLog(Today, (won ? "Düello kazanıldı: " : "Düello kaybedildi: ") + target.Name);
        }
    }
}
