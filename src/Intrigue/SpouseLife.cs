using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Bundle;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Eşinin gizli hayatı ve boşanma (tasarım: docs/superpowers/specs/2026-10-19-p2-esin-gizli-hayati.md). Eşin, seninle arası bozuksa ya
    /// da uzun süre ayrı kalırsanız kendi başına bir sevgili edinebilir (gerçek bir lord, P2.4 kurallarıyla); gebelik P2.6 sistemiyle
    /// olur ama gerçek baba sen öğrenene kadar gizlidir. Casus başın araştırırsa ya da karşı casusluktayken ortaya çıkar. Yüzleşir,
    /// görmezden gelir ya da boşarsın. Oyunda boşanma yok: eş bağı kaldırılır, eşin ailesinin klanına döner.
    /// İçine paketlenen Regard and Standing varsa eşinin ilişkisini ve gebeliğini o kurar; burada yalnızca izlenir (FollowRegardStanding).
    /// </summary>
    public static class SpouseLife
    {
        public static bool Enabled => RomanceSecrets.Enabled && SettingsStore.Current.SpouseInfidelity;

        private static int Today => SchemeFactors.Today;

        private static bool Together(Hero spouse)
        {
            MobileParty main = MobileParty.MainParty;
            if (main == null) return false;
            if (spouse.PartyBelongedTo == main) return true;
            return main.CurrentSettlement != null && spouse.CurrentSettlement == main.CurrentSettlement;
        }

        /// <summary>Sürmekte olan ilişki hâlâ geçerli mi (eş ve sevgili hayatta, hâlâ evlisiniz).</summary>
        private static bool Valid(SpouseAffair affair, Hero spouse)
        {
            Hero lover = Hero.Find(affair.LoverId);
            return spouse != null && affair.SpouseId == spouse.StringId && lover != null && lover.IsAlive;
        }

        // ---- Haftalık: ilişki başlar, buluşmalar, karşı casusluk ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            Hero spouse = Courtships.Spouse;
            if (state.SpouseAffair != null && !Valid(state.SpouseAffair, spouse))
            {
                state.SpouseAffair = null;
                return;
            }
            if (spouse == null)
            {
                state.SpouseApartWeeks = 0;
                return;
            }
            state.SpouseApartWeeks = Together(spouse) ? 0 : state.SpouseApartWeeks + 1;
            if (!Enabled || spouse.IsPrisoner) return;

            SpouseAffair affair;
            if (RomanceSecrets.FromRegardStanding)
            {
                affair = FollowRegardStanding(state, spouse);
                if (affair != null) CounterIntel(state, affair);
                return;
            }

            affair = state.SpouseAffair;
            if (affair == null)
            {
                if (!SecretRules.IsAdult(RomanceSecrets.Age(spouse))) return;
                int chance = FidelityRules.WeeklyAffairChance(spouse.GetRelation(Hero.MainHero), spouse.GetTraitLevel(DefaultTraits.Honor), state.SpouseApartWeeks);
                if (chance <= 0 || state.Roll100() > chance) return;
                List<Hero> lovers = RomanceSecrets.LoverCandidates(spouse);
                if (lovers.Count == 0) return;
                Hero lover = lovers[SchemeRandom.Pick(state.Seed, ref state.Rolls, lovers.Count)];
                state.SpouseAffair = new SpouseAffair { SpouseId = spouse.StringId, LoverId = lover.StringId, Since = Today, NextMeetingDay = Today };
                return;
            }

            Hero partner = Hero.Find(affair.LoverId);
            if (Today >= affair.NextMeetingDay && !partner.IsPrisoner)
            {
                affair.NextMeetingDay = Today + FidelityRules.MeetingDays;
                Courtships.TryConceive(state, spouse, partner, spouse.GetTraitLevel(DefaultTraits.Calculating) >= 1, hidden: true);
            }
            CounterIntel(state, affair);
        }

        private static void CounterIntel(IntrigueState state, SpouseAffair affair)
        {
            if (!affair.Discovered && state.Mission == SpymasterMission.CounterIntel && SchemeFactors.Spymaster(state) != null
                && state.Roll100() <= FidelityRules.CounterIntelWeeklyChance)
                Discover(state, "Karşı casusluk yapan casus başın tesadüfen öğrendi.");
        }

        /// <summary>
        /// Regard and Standing varken eşinin ilişkisini o kurar; burada yalnızca izlenir: sürmekte olan ilişkisi kayda alınır (araştırma,
        /// karşı casusluk, yüzleşme ve boşanma aynen çalışsın), sen öğrenmeden biterse ya da sevgili değişirse kayıt da değişir. O ilişkiden
        /// doğan çocuklar onun gerçek baba kayıtlarından eklenir.
        /// </summary>
        private static SpouseAffair FollowRegardStanding(IntrigueState state, Hero spouse)
        {
            Hero lover = RegardStandingCompat.CurrentLover(spouse);
            if (lover == Hero.MainHero) lover = null;
            SpouseAffair affair = state.SpouseAffair;
            if (affair != null && !affair.Discovered && (lover == null || lover.StringId != affair.LoverId))
                state.SpouseAffair = affair = null;
            if (affair == null && lover != null)
            {
                RegardStandingCompat.Affair record = RegardStandingCompat.Between(spouse, lover);
                state.SpouseAffair = affair = new SpouseAffair
                {
                    SpouseId = spouse.StringId,
                    LoverId = lover.StringId,
                    Since = record?.Day ?? Today,
                    NextMeetingDay = int.MaxValue,
                };
            }
            if (affair == null) return null;
            foreach (RegardStandingCompat.Parentage p in RegardStandingCompat.ParentageOf(spouse))
            {
                bool samePair = (p.Mother == spouse.StringId && p.Father == affair.LoverId) || (p.Father == spouse.StringId && p.Mother == affair.LoverId);
                if (!p.Born || !samePair) continue;
                foreach (string childId in p.Children)
                    if (Hero.Find(childId)?.IsAlive == true && !affair.ChildIds.Contains(childId)) affair.ChildIds.Add(childId);
            }
            return affair;
        }

        // ---- Araştırma ----

        public static string InvestigateBlocked(IntrigueState state)
        {
            if (!Enabled) return "Kurallar'dan kapalı (\"Eşin seni aldatabilir\")";
            Hero spouse = Courtships.Spouse;
            if (spouse == null) return "Klanında hayatta bir eşin yok";
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (spymaster == null) return "Önce bir casus başı ata";
            if (spymaster == spouse) return "Casus başın eşin: kendini araştırmaz";
            if (state.SpouseWatchUntil > Today) return "Araştırma sürüyor: " + (state.SpouseWatchUntil - Today) + " gün";
            if (state.SpouseAffair != null && state.SpouseAffair.Discovered) return "Zaten biliyorsun";
            return SchemeFactors.CanAfford(FidelityRules.InvestigationCost) ? null : "Yeterli altının yok (" + SchemeFactors.Money(FidelityRules.InvestigationCost) + ")";
        }

        public static int FindChance(IntrigueState state) => FidelityRules.InvestigationFindChance(SchemeFactors.Roguery(SchemeFactors.Spymaster(state)));

        public static void Investigate(IntrigueState state)
        {
            string blocked = InvestigateBlocked(state);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(FidelityRules.InvestigationCost);
            state.SpouseWatchUntil = Today + FidelityRules.InvestigationDays;
            state.AddLog(Today, "Casus başı eşini araştırıyor");
            MenuUI.Success("Casus başın eşini gözetliyor: " + FidelityRules.InvestigationDays + " gün");
        }

        /// <summary>Günlük: araştırma biter.</summary>
        public static void DailyTick()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.SpouseWatchUntil <= 0 || Today < state.SpouseWatchUntil) return;
            state.SpouseWatchUntil = 0;
            Hero spouse = Courtships.Spouse;
            if (spouse == null) return;
            SpouseAffair affair = state.SpouseAffair;
            if (affair != null && !affair.Discovered && Valid(affair, spouse) && state.Roll100() <= FindChance(state))
            {
                Discover(state, "Casus başın iki hafta boyunca " + spouse.Name + " eşini gözetledi.");
                return;
            }
            string text = "Casus başın iki hafta boyunca " + spouse.Name + " eşini gözetledi ve kayda değer bir şey bulamadı.";
            if (state.Roll100() <= FidelityRules.NoticeChance)
            {
                SchemeOutcomes.Relate(spouse, -FidelityRules.NoticeRelation);
                text += "\n\nAma " + spouse.Name + " izlendiğini fark etti ve ona güvenmediğin için kırgın: ilişki -" + FidelityRules.NoticeRelation + ".";
            }
            SchemeOutcomes.Notify(state, "Araştırma: " + spouse.Name, text, spouse, CardImages.Spy);
            state.AddLog(Today, "Eşinin araştırması: bir şey bulunmadı");
        }

        // ---- Ortaya çıkma ----

        /// <summary>İlişki ortaya çıkar: sevgiliye sır, çocuklar açıklanır; karar kartı (yüzleş / görmezden gel / boşa) sorulur.</summary>
        public static void Discover(IntrigueState state, string how)
        {
            SpouseAffair affair = state.SpouseAffair;
            Hero spouse = Courtships.Spouse;
            if (affair == null || affair.Discovered || !Valid(affair, spouse)) return;
            affair.Discovered = true;
            affair.Asked = false;
            Hero lover = Hero.Find(affair.LoverId);
            if (!state.Secrets.Any(s => s.TargetId == lover.StringId && s.Kind == SecretKind.Affair && s.OtherId == spouse.StringId))
                state.Secrets.Add(new Secret { TargetId = lover.StringId, Kind = SecretKind.Affair, Day = Today, OtherId = spouse.StringId });
            foreach (string childId in affair.ChildIds)
            {
                Hero child = Hero.Find(childId);
                if (child == null || state.Secrets.Any(s => s.Kind == SecretKind.Bastard && s.ChildId == childId)) continue;
                state.Secrets.Add(new Secret { TargetId = lover.StringId, Kind = SecretKind.Bastard, Day = Today, OtherId = spouse.StringId, ChildId = childId });
            }
            affair.How = how;
            state.AddLog(Today, "Eşinin gizli ilişkisi ortaya çıktı: " + lover.Name);
        }

        /// <summary>Karar kartının metni.</summary>
        public static string DiscoveryText(IntrigueState state)
        {
            SpouseAffair affair = state.SpouseAffair;
            Hero spouse = Hero.Find(affair.SpouseId);
            Hero lover = Hero.Find(affair.LoverId);
            int months = Math.Max(1, (Today - affair.Since) / 28);
            var lines = new List<string>
            {
                (affair.How ?? "Gerçek sonunda ortaya çıktı.") + " " + spouse.Name + " yaklaşık " + months + " aydır " + lover.Name + " ("
                    + SchemeFactors.Affiliation(lover) + ") ile gizli bir ilişki yaşıyor.",
            };
            List<Hero> children = affair.ChildIds.Select(Hero.Find).Where(c => c != null).ToList();
            foreach (Hero child in children)
                lines.Add(child.Name + (child.Mother == spouse ? " senin çocuğun değil: gerçek babası " + lover.Name + "." : ", " + lover.Name
                    + " ile eşinin çocuğu (" + child.Clan?.Name + " klanında büyüyor)."));
            lines.Add("Bu artık " + lover.Name + " için bir sır: Kişiler'den şantaj ya da ifşa için kullanabilirsin.");
            lines.Add("Yüzleşirsen eşin ilişkiyi %" + FidelityRules.ConfrontEndChance(spouse.GetRelation(Hero.MainHero), spouse.GetTraitLevel(DefaultTraits.Honor))
                + " ihtimalle bitirir. Boşarsan bu sefer haklı sayılırsın: eşinin ailesi az kırılır, onurlu lordlar seni suçlamaz.");
            return string.Join("\n\n", lines);
        }

        public static void Confront(IntrigueState state)
        {
            SpouseAffair affair = state.SpouseAffair;
            Hero spouse = Courtships.Spouse;
            if (affair == null || !Valid(affair, spouse)) return;
            affair.Asked = true;
            Hero lover = Hero.Find(affair.LoverId);
            if (state.Roll100() <= FidelityRules.ConfrontEndChance(spouse.GetRelation(Hero.MainHero), spouse.GetTraitLevel(DefaultTraits.Honor)))
            {
                state.SpouseAffair = null;
                if (RomanceSecrets.FromRegardStanding) RegardStandingCompat.BreakOff(spouse, lover);
                SchemeOutcomes.Relate(spouse, -FidelityRules.ConfrontEndRelation);
                SchemeOutcomes.Notify(state, "Yüzleşme: " + spouse.Name, spouse.Name + " suçunu kabul etti ve " + lover.Name
                    + " ile ilişkisini bitirdi. Aranızdaki soğukluk bir süre sürecek: ilişki -" + FidelityRules.ConfrontEndRelation + ".", spouse, CardImages.Betrayal);
                state.AddLog(Today, "Yüzleşme: eşin ilişkisini bitirdi");
                return;
            }
            SchemeOutcomes.Relate(spouse, -FidelityRules.ConfrontRefuseRelation);
            SchemeOutcomes.Notify(state, "Yüzleşme: " + spouse.Name, spouse.Name + " " + lover.Name + " ile görüşmeyi bırakmayacağını söyledi: ilişki -"
                + FidelityRules.ConfrontRefuseRelation + ". Hâlâ boşayabilirsin (kişi dosyasından).", spouse, CardImages.Betrayal);
            state.AddLog(Today, "Yüzleşme: eşin ilişkisini bitirmedi");
        }

        public static void Ignore(IntrigueState state)
        {
            if (state.SpouseAffair != null) state.SpouseAffair.Asked = true;
        }

        /// <summary>Gizli gebelikten doğan çocuk: ilişki kaydına yazılır (ortaya çıkınca açıklanır).</summary>
        internal static void OnHiddenBirth(IntrigueState state, Hero child)
        {
            if (state.SpouseAffair != null && !state.SpouseAffair.ChildIds.Contains(child.StringId)) state.SpouseAffair.ChildIds.Add(child.StringId);
        }

        // ---- Boşanma ----

        /// <summary>Haklı boşanma: eşinin ilişkisi ortaya çıktı.</summary>
        public static bool Justified(IntrigueState state) => state.SpouseAffair != null && state.SpouseAffair.Discovered;

        public static string DivorceBlocked(IntrigueState state)
        {
            Hero spouse = Courtships.Spouse;
            if (spouse == null) return "Klanında hayatta bir eşin yok";
            if (spouse.IsPregnant || state.Pregnancies.Any(p => p.MotherId == spouse.StringId)) return "Eşin hamile: doğumdan sonra";
            MobileParty party = spouse.PartyBelongedTo;
            if (party != null && party != MobileParty.MainParty && party.LeaderHero == spouse) return "Önce eşini parti liderliğinden al";
            if (Hero.MainHero.IsPrisoner) return "Esirken olmaz";
            return null;
        }

        /// <summary>Eşin döneceği klan: doğduğu klan, yoksa babasının ya da annesinin klanı; hiçbiri yoksa null (senin klanında kalır).</summary>
        public static Clan HomeClan(Hero spouse)
        {
            foreach (Clan clan in new[] { spouse.OriginClan, spouse.Father?.Clan, spouse.Mother?.Clan })
                if (clan != null && !clan.IsEliminated && clan != Clan.PlayerClan) return clan;
            return null;
        }

        public static void Divorce(IntrigueState state)
        {
            string blocked = DivorceBlocked(state);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Hero spouse = Courtships.Spouse;
            bool justified = Justified(state);
            Clan home = HomeClan(spouse);
            Hero familyLeader = home?.Leader;

            if (spouse.GovernorOf != null) ChangeGovernorAction.RemoveGovernorOf(spouse);
            Hero.MainHero.Spouse = null;
            spouse.Spouse = null;
            state.SpouseAffair = null;
            state.SpouseWatchUntil = 0;
            state.SpouseApartWeeks = 0;
            state.Courtships.RemoveAll(c => c.AgentId == spouse.StringId);

            var parts = new List<string>();
            SchemeOutcomes.Relate(spouse, -FidelityRules.DivorceRelation);
            parts.Add(spouse.Name + " ile ilişki -" + FidelityRules.DivorceRelation);
            if (familyLeader != null && familyLeader.IsAlive && familyLeader != spouse)
            {
                int penalty = FidelityRules.DivorceFamilyPenalty(justified);
                SchemeOutcomes.Relate(familyLeader, -penalty);
                parts.Add("ailesinin lideri " + familyLeader.Name + " ile -" + penalty);
            }
            if (!justified && Clan.PlayerClan.Kingdom != null)
            {
                List<Hero> honorable = Clan.PlayerClan.Kingdom.Clans
                    .Where(c => c != Clan.PlayerClan && !c.IsEliminated && c.Leader != null && c.Leader.IsAlive && c.Leader.GetTraitLevel(DefaultTraits.Honor) >= 1)
                    .Select(c => c.Leader).ToList();
                foreach (Hero lord in honorable) SchemeOutcomes.Relate(lord, -FidelityRules.DivorceHonorablePenalty);
                if (honorable.Count > 0) parts.Add("onurlu " + honorable.Count + " klan lideriyle -" + FidelityRules.DivorceHonorablePenalty + " (haksız boşama)");
            }

            string where;
            if (home != null)
            {
                spouse.Clan = home;
                spouse.UpdateHomeSettlement();
                if (spouse.PartyBelongedTo == MobileParty.MainParty) MakeHeroFugitiveAction.Apply(spouse, false);
                where = spouse.Name + " ailesinin yanına, " + home.Name + " klanına döndü.";
            }
            else where = "Dönecek bir ailesi olmadığı için " + spouse.Name + " senin klanında kaldı, ama artık eşin değil.";

            SchemeOutcomes.Notify(state, "Boşanma: " + spouse.Name,
                (justified ? "Aldatıldığın herkesçe bilindiği için boşanman haklı sayıldı. " : "Ortada bir suç yokken boşadın; saray bunu konuşuyor. ")
                + where + " Çocuklar senin klanında kalır. İstersen yeniden evlenebilirsin.\n\nSonuç: " + string.Join(", ", parts) + ".",
                spouse, CardImages.Divorce);
            state.AddLog(Today, "Boşanma: " + spouse.Name + (justified ? " (haklı)" : ""));
        }
    }
}
