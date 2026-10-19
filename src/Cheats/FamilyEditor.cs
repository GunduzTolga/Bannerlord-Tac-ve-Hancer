using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Aile editörü (P2.7, tasarım: docs/superpowers/specs/2026-10-19-p2-aile-editoru.md): evlendir, boşa, gebelik başlat, "çocuk
    /// istemiyorlar", çocuğu büyüt, yaşlanmayı dondur. Oyunun kendi kurallarına uyar: evlilik oyunun uygunluk kontrolünden geçer,
    /// gebelik yalnızca evli ve 18-45 yaş arası bir kadın için ve oyunun Doğum ve Ölüm ayarı açıksa.
    /// </summary>
    public static class FamilyEditor
    {
        private static IntrigueState State => SchemeEngine.State;

        // ---- Evlilik ----

        public static string MarryBlocked(Hero hero)
        {
            if (hero.Spouse != null && hero.Spouse.IsAlive) return "Zaten evli";
            return Campaign.Current.Models.MarriageModel.IsSuitableForMarriage(hero) ? null : "Oyunun kurallarına göre şu an evlenemez (yaş, durum)";
        }

        /// <summary>Oyunun uygunluk kontrolünden geçen eş adayları (yaş, akrabalık, cins, medeni durum).</summary>
        public static List<Hero> MarriageCandidates(Hero hero) =>
            Hero.AllAliveHeroes
                .Where(h => h != hero && (h.IsLord || h.IsWanderer || h.IsPlayerCompanion || h == Hero.MainHero)
                    && Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(hero, h))
                .OrderBy(h => h.Name.ToString(), StringComparer.CurrentCultureIgnoreCase)
                .ToList();

        public static void Marry(Hero a, Hero b)
        {
            if (!Campaign.Current.Models.MarriageModel.IsCoupleSuitableForMarriage(a, b))
                throw new InvalidOperationException("Oyunun kurallarına göre bu çift evlenemez");
            MarriageAction.Apply(a, b, true);
            MenuUI.Success(a.Name + " ile " + b.Name + " evlendi");
        }

        // ---- Boşanma ----

        public static string DivorceBlocked(Hero hero)
        {
            Hero spouse = hero.Spouse;
            if (spouse == null || !spouse.IsAlive) return "Evli değil";
            if (hero == Hero.MainHero || spouse == Hero.MainHero) return SpouseLife.DivorceBlocked(State);
            Hero wife = hero.IsFemale ? hero : spouse;
            if (wife.IsPregnant || State.Pregnancies.Any(p => p.MotherId == wife.StringId)) return "Hamile: doğumdan sonra";
            Hero mover = Mover(hero, spouse);
            if (mover != null && mover.PartyBelongedTo != null && mover.PartyBelongedTo.LeaderHero == mover && mover.PartyBelongedTo != MobileParty.MainParty)
                return mover.Name + " bir partiye liderlik ediyor";
            return null;
        }

        /// <summary>Boşanınca klanından ayrılacak olan: evlilikle klana girmiş, lider olmayan taraf; yoksa null (kimse taşınmaz).</summary>
        private static Hero Mover(Hero a, Hero b)
        {
            foreach (Hero hero in new[] { a, b })
                if (hero.Clan != null && hero.OriginClan != hero.Clan && !hero.IsClanLeader && Home(hero) != null) return hero;
            return null;
        }

        private static Clan Home(Hero hero)
        {
            foreach (Clan clan in new[] { hero.OriginClan, hero.Father?.Clan, hero.Mother?.Clan })
                if (clan != null && !clan.IsEliminated && clan != hero.Clan) return clan;
            return null;
        }

        /// <summary>Oyuncunun kendi evliliğinde entrikadaki boşanma (sonuçlarıyla); başkalarında eş bağı kalkar, gelen taraf ailesine döner.</summary>
        public static void Divorce(Hero hero)
        {
            string blocked = DivorceBlocked(hero);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Hero spouse = hero.Spouse;
            if (hero == Hero.MainHero || spouse == Hero.MainHero)
            {
                SpouseLife.Divorce(State);
                return;
            }
            Hero mover = Mover(hero, spouse);
            hero.Spouse = null;
            spouse.Spouse = null;
            if (mover != null)
            {
                if (mover.GovernorOf != null) ChangeGovernorAction.RemoveGovernorOf(mover);
                Clan home = Home(mover);
                mover.Clan = home;
                mover.UpdateHomeSettlement();
                MenuUI.Success(hero.Name + " ile " + spouse.Name + " boşandı; " + mover.Name + " " + home.Name + " klanına döndü");
            }
            else MenuUI.Success(hero.Name + " ile " + spouse.Name + " boşandı");
        }

        // ---- Gebelik ----

        public static string PregnancyBlocked(Hero hero)
        {
            if (CampaignOptions.IsLifeDeathCycleDisabled) return "Oyunun Doğum ve Ölüm ayarı kapalı";
            if (!hero.IsFemale) return "Yalnızca kadın kahramanlar";
            if (hero.Spouse == null || !hero.Spouse.IsAlive) return "Evli değil (gebelik oyunun kuralıyla eşten olur)";
            if (!RomanceRules.CanConceive(RomanceSecrets.Age(hero))) return "Yaşı uygun değil (18-" + RomanceRules.MaxMotherAge + ")";
            if (hero.IsPregnant || State.Pregnancies.Any(p => p.MotherId == hero.StringId)) return "Zaten hamile";
            return WantsNoChildren(hero) ? "\"Çocuk istemiyorlar\" açık" : null;
        }

        /// <summary>Oyunun kendi gebeliği: baba eşi, süre oyunun gebelik süresi.</summary>
        public static void StartPregnancy(Hero mother)
        {
            string blocked = PregnancyBlocked(mother);
            if (blocked != null) throw new InvalidOperationException(blocked);
            MakePregnantAction.Apply(mother);
            MenuUI.Success(mother.Name + " hamile (baba: " + mother.Spouse.Name + ")");
        }

        /// <summary>Çiftten biri "çocuk istemiyoruz" dediyse oyunun ve kurun gebeliği olmaz.</summary>
        public static bool WantsNoChildren(Hero hero)
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || hero == null) return false;
            return state.NoChildren.Contains(hero.StringId) || (hero.Spouse != null && state.NoChildren.Contains(hero.Spouse.StringId));
        }

        public static void SetNoChildren(Hero hero, bool value)
        {
            if (value) State.NoChildren.Add(hero.StringId);
            else
            {
                State.NoChildren.Remove(hero.StringId);
                if (hero.Spouse != null) State.NoChildren.Remove(hero.Spouse.StringId);
            }
        }

        // ---- Yaş ----

        public static int ComesOfAge => Campaign.Current.Models.AgeModel.HeroComesOfAge;

        /// <summary>Çocuğu büyütür (en fazla yetişkinlik yaşına). Geçişleri oyunun yaşlanma sistemi ertesi gün işler.</summary>
        public static void Grow(Hero child, int years)
        {
            if (!child.IsChild) throw new InvalidOperationException(child.Name + " çocuk değil");
            int age = Math.Min((int)child.Age + Math.Max(1, years), ComesOfAge);
            child.SetBirthDay(HeroHelper.GetRandomBirthDayForAge(age));
            MenuUI.Success(child.Name + " artık " + (int)child.Age + " yaşında" + (age >= ComesOfAge ? " (yarın yetişkin sayılır)" : ""));
        }

        public static bool IsFrozen(Hero hero) => IntrigueBehavior.Current?.State?.AgeFrozen.Contains(hero.StringId) == true;

        public static void SetFrozen(Hero hero, bool value)
        {
            if (value) State.AgeFrozen.Add(hero.StringId);
            else State.AgeFrozen.Remove(hero.StringId);
        }

        /// <summary>Günlük: yaşlanması dondurulanların doğum günü bir gün ileri alınır (yaşları sabit kalır).</summary>
        public static void DailyTick()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.AgeFrozen.Count == 0) return;
            foreach (string id in state.AgeFrozen.ToList())
            {
                Hero hero = Hero.Find(id);
                if (hero == null || !hero.IsAlive)
                {
                    state.AgeFrozen.Remove(id);
                    continue;
                }
                hero.SetBirthDay(hero.BirthDay + CampaignTime.Days(1f));
            }
        }
    }
}
