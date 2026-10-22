using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Rehine anlaşmaları (P2.10, tasarım: docs/superpowers/specs/2026-10-19-p2-rehine.md). Barıştaki bir krallığa barışın teminatı olarak
    /// klanından birini gönderirsin (tek taraflı) ya da karşılıklı değişim yaparsın. Rehineler esir değil misafirdir: ev sahibinin
    /// yerleşiminde yaşar, haftalar geçtikçe ev sahibiyle yakınlaşır, bir yıl sonra döner. Barış bozulursa (kim bozarsa) misafirler oyunun
    /// kendi esirlik sistemine düşer; barışı bozan sensen onurlu lordlar ve hükümdar seni affetmez.
    /// </summary>
    public static class Hostages
    {
        private static int Today => SchemeFactors.Today;

        public static Kingdom KingdomOf(HostageDeal deal) => Kingdom.All.FirstOrDefault(k => k.StringId == deal.KingdomId);

        /// <summary>null: bu krallıkla anlaşma yapılabilir; değilse neden.</summary>
        public static string Blocked(IntrigueState state, Kingdom kingdom)
        {
            if (kingdom == null || kingdom.IsEliminated || kingdom.Leader == null) return "Krallık yok";
            Kingdom mine = Clan.PlayerClan.Kingdom;
            if (kingdom == mine) return "Kendi krallığın";
            if (mine != null && mine.Leader != Hero.MainHero) return "Yalnızca hükümdar ya da bağımsız klan lideri yapabilir";
            if (FactionManager.IsAtWarAgainstFaction(kingdom, Clan.PlayerClan.MapFaction)) return "Savaştasınız: önce barış";
            if (state.HostageDealWith(kingdom.StringId) != null) return "Bu krallıkla zaten bir anlaşman var";
            return null;
        }

        /// <summary>Gönderebileceğin aile üyeleri: klanındaki hayattaki, esir olmayan, parti yönetmeyen, eşin olmayan, divanda ya da genelevde işi olmayan kişiler.</summary>
        public static List<Hero> OurCandidates(IntrigueState state) =>
            Clan.PlayerClan.Heroes
                .Where(h => h.IsAlive && h != Hero.MainHero && h != Hero.MainHero.Spouse && !h.IsPrisoner && !h.IsPregnant
                    && !(h.PartyBelongedTo != null && h.PartyBelongedTo.LeaderHero == h && h.PartyBelongedTo != MobileParty.MainParty)
                    && !state.Hostages.Any(d => d.GivenId == h.StringId || d.HeldId == h.StringId)
                    && Council.SeatOf(state, h) == null)
                .OrderBy(h => h.Age)
                .ToList();

        /// <summary>Hükümdarın vereceği rehine: hükümdarın çocuğu öncelikli, sonra hanedanın en genç üyesi.</summary>
        public static Hero TheirCandidate(IntrigueState state, Kingdom kingdom)
        {
            Hero ruler = kingdom.Leader;
            if (ruler == null) return null;
            return kingdom.RulingClan?.Heroes
                .Where(h => h.IsAlive && h != ruler && h != ruler.Spouse && !h.IsPrisoner && !h.IsPregnant
                    && !(h.PartyBelongedTo != null && h.PartyBelongedTo.LeaderHero == h)
                    && !state.Hostages.Any(d => d.GivenId == h.StringId || d.HeldId == h.StringId))
                .OrderByDescending(h => h.Father == ruler || h.Mother == ruler)
                .ThenBy(h => h.Age)
                .FirstOrDefault();
        }

        /// <summary>Misafirin kalacağı yer: krallığın hanedanının evi, yoksa hükümdarın evi.</summary>
        public static Settlement HostSettlement(Kingdom kingdom) =>
            kingdom.RulingClan?.HomeSettlement ?? kingdom.Leader?.HomeSettlement ?? kingdom.Fiefs.FirstOrDefault()?.Settlement;

        /// <summary>Onların rehinesini tutacağın yer: evin, yoksa ilk şehrin ya da kalen; yoksa null.</summary>
        public static Settlement OurSettlement()
        {
            Settlement home = Clan.PlayerClan.HomeSettlement;
            if (home != null && home.OwnerClan == Clan.PlayerClan && home.Town != null) return home;
            return Clan.PlayerClan.Fiefs.FirstOrDefault()?.Settlement;
        }

        public static int ExchangeChance(IntrigueState state, Kingdom kingdom)
        {
            float ours = Clan.PlayerClan.Kingdom?.CurrentTotalStrength ?? Clan.PlayerClan.CurrentTotalStrength;
            return HostageRules.ExchangeAcceptChance(Hero.MainHero.GetRelation(kingdom.Leader), state.TensionWith(kingdom.StringId),
                kingdom.CurrentTotalStrength, ours);
        }

        public static string ExchangeBlocked(IntrigueState state, Kingdom kingdom) =>
            Blocked(state, kingdom) ?? (OurSettlement() == null ? "Onların rehinesini tutacak şehrin ya da kalen yok"
                : TheirCandidate(state, kingdom) == null ? kingdom.Leader.Name + " hanedanında verecek kimse yok" : null);

        /// <summary>Tek taraflı teminat: aileden birini gönderirsin; her zaman kabul edilir.</summary>
        public static void Give(IntrigueState state, Kingdom kingdom, Hero hero)
        {
            string blocked = Blocked(state, kingdom);
            if (blocked != null) throw new InvalidOperationException(blocked);
            if (!OurCandidates(state).Contains(hero)) throw new InvalidOperationException(hero.Name + " gönderilemez");
            Settlement host = HostSettlement(kingdom) ?? throw new InvalidOperationException("Misafiri ağırlayacak bir yer yok");
            Send(hero, host);
            var deal = new HostageDeal { KingdomId = kingdom.StringId, GivenId = hero.StringId, Since = Today, Until = Today + HostageRules.DurationDays };
            state.Hostages.Add(deal);
            SchemeOutcomes.Relate(kingdom.Leader, HostageRules.GiveRulerRelation);
            Relieve(state, kingdom);
            string pact = FormPact(kingdom, deal);
            SchemeOutcomes.Notify(state, "Rehine gönderildi: " + hero.Name,
                hero.Name + ", barışın teminatı olarak " + kingdom.Leader.Name + " sarayında, " + host.Name + " kentinde misafir olarak yaşayacak. "
                + "Bir yıl sonra döner; bu sürede ev sahibiyle yakınlaşır.\n\nSonuç: " + kingdom.Leader.Name + " ile ilişki +" + HostageRules.GiveRulerRelation
                + ", casus gerilimi -" + HostageRules.TensionRelief + "." + pact + " Barış bozulursa " + hero.Name + " onların esiri olur.",
                hero, CardImages.Hostage);
            state.AddLog(Today, "Rehine gönderildi: " + hero.Name + ", " + kingdom.Name);
        }

        /// <summary>Karşılıklı değişim: hükümdar kabul ederse onların hanedanından biri de senin yanına gelir.</summary>
        public static void ProposeExchange(IntrigueState state, Kingdom kingdom, Hero hero)
        {
            string blocked = ExchangeBlocked(state, kingdom);
            if (blocked != null) throw new InvalidOperationException(blocked);
            if (!OurCandidates(state).Contains(hero)) throw new InvalidOperationException(hero.Name + " gönderilemez");
            if (state.Roll100() > ExchangeChance(state, kingdom))
            {
                SchemeOutcomes.Notify(state, "Değişim reddedildi: " + kingdom.Name,
                    kingdom.Leader.Name + " rehine değişimini kabul etmedi; sana o kadar güvenmiyor ya da gerek görmüyor. İstersen tek taraflı rehine gönderebilirsin.",
                    kingdom.Leader, CardImages.Hostage);
                return;
            }
            Hero theirs = TheirCandidate(state, kingdom);
            Settlement host = HostSettlement(kingdom) ?? throw new InvalidOperationException("Misafiri ağırlayacak bir yer yok");
            Settlement ours = OurSettlement();
            Send(hero, host);
            Send(theirs, ours);
            var deal = new HostageDeal
            {
                KingdomId = kingdom.StringId, GivenId = hero.StringId, HeldId = theirs.StringId, Since = Today, Until = Today + HostageRules.DurationDays,
            };
            state.Hostages.Add(deal);
            SchemeOutcomes.Relate(kingdom.Leader, HostageRules.ExchangeRulerRelation);
            Relieve(state, kingdom);
            string exchangePact = FormPact(kingdom, deal);
            SchemeOutcomes.Notify(state, "Rehine değişimi: " + kingdom.Name,
                hero.Name + " " + host.Name + " kentine, " + theirs.Name + " " + ours.Name + " kentine misafir olarak gitti. Bir yıl sonra ikisi de döner.\n\n"
                + "Sonuç: " + kingdom.Leader.Name + " ile ilişki +" + HostageRules.ExchangeRulerRelation + ", casus gerilimi -" + HostageRules.TensionRelief
                + "." + exchangePact + " Barışı bozan taraf misafirini esir olarak kaybeder.",
                theirs, CardImages.Hostage);
            state.AddLog(Today, "Rehine değişimi: " + kingdom.Name + " (" + hero.Name + " / " + theirs.Name + ")");
        }

        /// <summary>
        /// Barış Masası'nın şartı: onların hanedanından biri barışın teminatı olarak senin sarayına gelir (tek taraflı). Barış yeni imzalandığı
        /// için Blocked'daki "savaştasınız" engeli burada yok. Sonuç metni (antlaşma kartı için) döner.
        /// </summary>
        public static string TakeForTreaty(IntrigueState state, Kingdom kingdom, Hero theirs)
        {
            Settlement ours = OurSettlement();
            if (theirs == null || !theirs.IsAlive || theirs.IsPrisoner || ours == null) return "Rehine gelemedi" + (ours == null ? ": onu tutacak şehrin ya da kalen yok." : ".");
            if (state.HostageDealWith(kingdom.StringId) != null) return "Bu krallıkla zaten bir rehine anlaşman var; yeni rehine gelmedi.";
            Send(theirs, ours);
            var deal = new HostageDeal { KingdomId = kingdom.StringId, HeldId = theirs.StringId, Since = Today, Until = Today + HostageRules.DurationDays };
            state.Hostages.Add(deal);
            Relieve(state, kingdom);
            string pact = FormPact(kingdom, deal);
            state.AddLog(Today, "Barış teminatı: " + theirs.Name + " (" + kingdom.Name + ") sarayında");
            return theirs.Name + " barışın teminatı olarak " + ours.Name + " kentinde senin misafirin; " + HostageRules.DurationDays
                + " gün sonra döner." + pact + " Barışı bozarlarsa esirin olur.";
        }

        /// <summary>Anlaşmayı erken bitir: misafirler döner, hükümdar kırılır.</summary>
        public static void EndEarly(IntrigueState state, HostageDeal deal)
        {
            Kingdom kingdom = KingdomOf(deal);
            Close(state, deal);
            // Yalnızca bu anlaşmanın kurduğu saldırmazlık biter (barış antlaşmasının ya da Diplomacy'den ayrıca alınanın değil).
            if (deal.Pact && Clan.PlayerClan.Kingdom != null) Bundle.DiplomacyCompat.EndNonAggressionPact(Clan.PlayerClan.Kingdom, kingdom);
            if (kingdom?.Leader != null && kingdom.Leader.IsAlive) SchemeOutcomes.Relate(kingdom.Leader, -HostageRules.EarlyEndPenalty);
            MenuUI.Success("Rehine anlaşması bitti; misafirler evlerine dönüyor" + (kingdom?.Leader != null ? " (" + kingdom.Leader.Name + " ile -" + HostageRules.EarlyEndPenalty + ")" : ""));
        }

        // ---- Haftalık ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.Hostages.Count == 0) return;
            foreach (HostageDeal deal in state.Hostages.ToList())
            {
                Kingdom kingdom = KingdomOf(deal);
                if (kingdom == null || kingdom.IsEliminated || kingdom.Leader == null)
                {
                    Close(state, deal);
                    continue;
                }
                Hero given = deal.GivenId != null ? Hero.Find(deal.GivenId) : null;
                Hero held = deal.HeldId != null ? Hero.Find(deal.HeldId) : null;
                if (Today >= deal.Until)
                {
                    Close(state, deal);
                    SchemeOutcomes.Relate(kingdom.Leader, HostageRules.ReturnRulerRelation);
                    SchemeOutcomes.Notify(state, "Rehineler döndü: " + kingdom.Name,
                        "Bir yıllık anlaşma sona erdi; misafirler evlerine döndü." + (given != null ? " " + given.Name + " " + kingdom.Leader.Name + " ile dost olarak döndü." : "")
                        + "\n\nSonuç: " + kingdom.Leader.Name + " ile ilişki +" + HostageRules.ReturnRulerRelation + ".",
                        given ?? held, CardImages.Hostage);
                    continue;
                }
                // Misafirler yerinde kalsın (oyun boştaki kahramanları evlerine taşıyabilir).
                Keep(given, HostSettlement(kingdom));
                Keep(held, OurSettlement());
                int gain = HostageRules.FosterGain(deal.Fostered);
                if (gain <= 0) continue;
                deal.Fostered += gain;
                if (given != null && given.IsAlive) SchemeOutcomes.RelateBetween(given, kingdom.Leader, gain);
                if (held != null && held.IsAlive) SchemeOutcomes.Relate(held, gain);
            }
        }

        /// <summary>Savaş ilanı: anlaşmalı krallıkla savaş çıktıysa misafirler esir düşer; ilan eden sensen yemin bozulmuştur.</summary>
        public static void OnWarDeclared(IFaction faction1, IFaction faction2, DeclareWarAction.DeclareWarDetail detail)
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.Hostages.Count == 0) return;
            IFaction mine = Clan.PlayerClan.MapFaction;
            IFaction other = faction1 == mine ? faction2 : faction2 == mine ? faction1 : null;
            if (other == null) return;
            HostageDeal deal = state.HostageDealWith(other.StringId);
            if (deal == null) return;
            Kingdom kingdom = KingdomOf(deal);
            state.Hostages.Remove(deal);
            // Diplomacy saldırmazlığı savaş ilanında kendiliğinden bitirmez; bu anlaşmanın kurduğu kâğıt üstünde kalmasın.
            if (deal.Pact && mine is Kingdom ourKingdom && kingdom != null) Bundle.DiplomacyCompat.EndNonAggressionPact(ourKingdom, kingdom);
            bool weBroke = faction1 == mine;
            var parts = new List<string>();
            Hero given = deal.GivenId != null ? Hero.Find(deal.GivenId) : null;
            Hero held = deal.HeldId != null ? Hero.Find(deal.HeldId) : null;
            Settlement host = kingdom != null ? HostSettlement(kingdom) : null;
            Settlement ours = OurSettlement();
            if (given != null && given.IsAlive && !given.IsPrisoner && host?.Party != null)
            {
                TakePrisonerAction.Apply(host.Party, given);
                parts.Add(given.Name + " artık " + host.Name + " zindanında");
            }
            if (held != null && held.IsAlive && !held.IsPrisoner && ours?.Party != null)
            {
                TakePrisonerAction.Apply(ours.Party, held);
                parts.Add(held.Name + " artık senin esirin (" + ours.Name + ")");
            }
            if (weBroke && kingdom?.Leader != null)
            {
                SchemeOutcomes.Relate(kingdom.Leader, -HostageRules.BreachRulerPenalty);
                int count = 0;
                foreach (Hero lord in HonorableLords())
                {
                    SchemeOutcomes.Relate(lord, -HostageRules.BreachHonorablePenalty);
                    count++;
                }
                parts.Add(kingdom.Leader.Name + " ile -" + HostageRules.BreachRulerPenalty + (count > 0 ? ", onurlu " + count + " lordla -" + HostageRules.BreachHonorablePenalty : ""));
            }
            SchemeOutcomes.Notify(state, (weBroke ? "Yeminini bozdun: " : "Barış bozuldu: ") + other.Name,
                (weBroke ? "Rehinelerle güvence verdiğin barışı sen bozdun. " : other.Name + " rehinelerle güvence altındaki barışı bozdu. ")
                + "Misafirler artık esir." + (parts.Count > 0 ? "\n\nSonuç: " + string.Join(", ", parts) + "." : ""),
                given ?? held, CardImages.Hostage);
            state.AddLog(Today, (weBroke ? "Rehineli barışı bozdun: " : "Rehineli barışı bozdular: ") + other.Name);
        }

        /// <summary>Saray sekmesinde ve kartta anlaşmanın durumu ("" ise yok).</summary>
        public static string StatusLine(IntrigueState state, Kingdom kingdom)
        {
            HostageDeal deal = state.HostageDealWith(kingdom.StringId);
            if (deal == null) return "";
            Hero given = deal.GivenId != null ? Hero.Find(deal.GivenId) : null;
            Hero held = deal.HeldId != null ? Hero.Find(deal.HeldId) : null;
            return "Rehine anlaşması: " + (given != null ? given.Name + " onlarda" : "") + (given != null && held != null ? ", " : "")
                + (held != null ? held.Name + " sende" : "") + "; " + Math.Max(0, deal.Until - Today) + " gün kaldı.";
        }

        // ---- Yardımcılar ----

        /// <summary>
        /// Rehine anlaşması süresince Diplomacy'de saldırmazlık anlaşması (krallığın varsa ve Diplomacy paketliyse); kartta söylenecek
        /// cümle ya da boş. Anlaşma süresi dolunca kendiliğinden biter; erken bitirilirse onu da bitiririz.
        /// </summary>
        private static string FormPact(Kingdom kingdom, HostageDeal deal)
        {
            Kingdom mine = Clan.PlayerClan.Kingdom;
            if (mine == null || mine == kingdom) return "";
            deal.Pact = Bundle.DiplomacyCompat.FormNonAggressionPact(mine, kingdom, HostageRules.DurationDays);
            return deal.Pact ? " Rehineler süresince " + HostageRules.DurationDays + " günlük saldırmazlık anlaşması." : "";
        }

        private static void Send(Hero hero, Settlement settlement)
        {
            if (hero.GovernorOf != null) ChangeGovernorAction.RemoveGovernorOf(hero);
            // Partindeyse önce partiden çıkar (genelevdeki yoldaşlar gibi); yoksa hem partide hem sarayda görünür.
            if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero != hero)
                hero.PartyBelongedTo.MemberRoster.AddToCounts(hero.CharacterObject, -1);
            TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, settlement);
        }

        private static void Keep(Hero hero, Settlement settlement)
        {
            if (hero == null || settlement == null || !hero.IsAlive || hero.IsPrisoner) return;
            if (hero.CurrentSettlement == settlement || hero.PartyBelongedTo != null) return;
            TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, settlement);
        }

        /// <summary>Misafirler evlerine döner, anlaşma kalkar.</summary>
        private static void Close(IntrigueState state, HostageDeal deal)
        {
            state.Hostages.Remove(deal);
            foreach (string id in new[] { deal.GivenId, deal.HeldId })
            {
                Hero hero = id != null ? Hero.Find(id) : null;
                if (hero == null || !hero.IsAlive || hero.IsPrisoner || hero.PartyBelongedTo != null) continue;
                Settlement home = hero.Clan?.HomeSettlement;
                if (home != null && home != hero.CurrentSettlement) TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, home);
            }
        }

        private static void Relieve(IntrigueState state, Kingdom kingdom)
        {
            int tension = state.TensionWith(kingdom.StringId);
            if (tension > 0) state.Tension[kingdom.StringId] = CrisisRules.AddTension(tension, -HostageRules.TensionRelief);
        }

        private static IEnumerable<Hero> HonorableLords()
        {
            Kingdom mine = Clan.PlayerClan.Kingdom;
            if (mine == null) return Enumerable.Empty<Hero>();
            return mine.Clans.Where(c => c != Clan.PlayerClan && !c.IsEliminated && c.Leader != null && c.Leader.IsAlive
                    && c.Leader.GetTraitLevel(DefaultTraits.Honor) >= 1)
                .Select(c => c.Leader).ToList();
        }
    }
}
