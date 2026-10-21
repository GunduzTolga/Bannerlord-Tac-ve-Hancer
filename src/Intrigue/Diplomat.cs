using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Divan: diplomat (tasarım: docs/superpowers/specs/2026-10-21-divan-diplomat.md). Charm'ına göre Barış Masası'nda pazarlık gücü,
    /// elçilerin sıklığı, çağrılan ve ziyafete davet edilen lordların gelmesi, hediyenin etkisi. Haftalık: "ilişkileri geliştir" görevi
    /// ve sana savaş açmaya hazırlanan krallığa erken uyarı (oyunun kendi savaş ilanı puanından, diplomatın gözüyle). Sarayda olmalı.
    /// </summary>
    public static class Diplomat
    {
        public const string Seat = "diplomatın";
        private static int Today => SchemeFactors.Today;

        // ---- Koltuk ----

        public static Hero Current(IntrigueState state)
        {
            if (state?.DiplomatId == null) return null;
            Hero hero = Hero.Find(state.DiplomatId);
            return hero != null && hero.IsAlive && hero.Clan == Clan.PlayerClan ? hero : null;
        }

        public static Hero Working(IntrigueState state)
        {
            Hero hero = Current(state);
            return hero != null && SchemeFactors.SpymasterBusyReason(hero) == null ? hero : null;
        }

        public static string Blocked(IntrigueState state)
        {
            Hero hero = Current(state);
            if (hero == null) return "Diplomatın yok";
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            return busy != null ? hero.Name + " şu an " + busy + "; sarayda değil" : null;
        }

        public static int Charm(Hero hero) => hero?.GetSkillValue(DefaultSkills.Charm) ?? 0;

        public static IEnumerable<Hero> Candidates(IntrigueState state) => Council.Candidates(state, Seat);

        public static string Label(Hero hero)
        {
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            int charm = Charm(hero);
            return hero.Name + " - Charm " + charm + ", pazarlık +" + DiplomatRules.PeaceBonus(charm) + ", gelme +%" + DiplomatRules.InviteBonus(charm)
                + (busy != null ? "  [" + busy + "]" : "");
        }

        public static string DetailText(Hero hero)
        {
            int charm = Charm(hero);
            return "Barış Masası +" + DiplomatRules.PeaceBonus(charm) + " · elçi +%" + DiplomatRules.EnvoyBonus(charm) + " · çağrı ve davet +%"
                + DiplomatRules.InviteBonus(charm) + (DiplomatRules.GiftBonus(charm) > 0 ? " · hediye +" + DiplomatRules.GiftBonus(charm) : "")
                + " · tahmin payı %" + DiplomatRules.ErrorPercent(charm);
        }

        /// <summary>Divan kartı için kısa hâli.</summary>
        public static string CardText(Hero hero)
        {
            int charm = Charm(hero);
            return "Pazarlık +" + DiplomatRules.PeaceBonus(charm) + " · elçi +%" + DiplomatRules.EnvoyBonus(charm) + " · davet +%" + DiplomatRules.InviteBonus(charm)
                + " · hata %" + DiplomatRules.ErrorPercent(charm);
        }

        public static void Assign(IntrigueState state, Hero hero)
        {
            Council.EnsureFree(state, hero, Seat);
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            if (busy != null) throw new InvalidOperationException(hero.Name + " şu an " + busy + "; diplomat olamaz");
            if (state.DiplomatId != hero.StringId) state.DiplomatTargetId = null;
            state.DiplomatId = hero.StringId;
            state.AddLog(Today, hero.Name + " diplomat oldu");
            MenuUI.Success(hero.Name + " artık diplomatın");
        }

        public static void Dismiss(IntrigueState state)
        {
            if (state.DiplomatId == null) throw new InvalidOperationException("Diplomatın yok");
            Hero old = Current(state);
            state.DiplomatId = null;
            state.DiplomatTargetId = null;
            state.AddLog(Today, (old?.Name.ToString() ?? "Diplomat") + " görevden alındı");
            MenuUI.Success("Diplomat görevden alındı");
        }

        // ---- Başka sistemlere katkısı (diplomat sarayda değilse 0) ----

        private static int Bonus(Func<int, int> rule)
        {
            Hero diplomat = Working(IntrigueBehavior.Current?.State);
            return diplomat == null ? 0 : rule(Charm(diplomat));
        }

        public static int PeaceBonus() => Bonus(DiplomatRules.PeaceBonus);
        public static int EnvoyBonus() => Bonus(DiplomatRules.EnvoyBonus);
        public static int InviteBonus() => Bonus(DiplomatRules.InviteBonus);
        public static int GiftBonus() => Bonus(DiplomatRules.GiftBonus);

        // ---- Görev: ilişkileri geliştir ----

        public static Hero Target(IntrigueState state) => state?.DiplomatTargetId == null ? null : Hero.Find(state.DiplomatTargetId);

        /// <summary>Görevin hedefi olabilecekler: tanıştığın, başka klandan yetişkin lordlar; hükümdarlar ve klan liderleri önce.</summary>
        public static List<Hero> TargetCandidates() =>
            Hero.AllAliveHeroes.Where(h => h.IsLord && !h.IsChild && h != Hero.MainHero && h.Clan != Clan.PlayerClan && h.Clan != null && h.HasMet)
                .OrderByDescending(h => h.IsKingdomLeader).ThenByDescending(h => h.Clan.Leader == h).ThenBy(h => h.Name.ToString()).ToList();

        public static string TargetBlocked(IntrigueState state, Hero target)
        {
            if (Working(state) == null) return Blocked(state) ?? "Diplomatın sarayda değil";
            if (target == null || !target.IsAlive) return "Hayatta değil";
            if (target.Clan == Clan.PlayerClan) return "Kendi klanından";
            return Hero.MainHero.GetRelation(target) >= DiplomatRules.MissionCap ? "Aranız zaten çok iyi" : null;
        }

        public static void SetTarget(IntrigueState state, Hero target)
        {
            string blocked = TargetBlocked(state, target);
            if (blocked != null) throw new InvalidOperationException(blocked);
            state.DiplomatTargetId = target.StringId;
            state.AddLog(Today, Current(state)?.Name + " " + target.Name + " ile ilişkileri geliştirmeye başladı");
            MenuUI.Success(Current(state)?.Name + " " + target.Name + " ile ilişkileri geliştirecek (haftada +"
                + DiplomatRules.RelationPerWeek(Charm(Current(state))) + ")");
        }

        public static void ClearTarget(IntrigueState state) => state.DiplomatTargetId = null;

        public static string MissionText(IntrigueState state)
        {
            Hero diplomat = Current(state), target = Target(state);
            if (diplomat == null) return "";
            if (target == null) return "Görevi yok: bir lordla ilişkileri geliştirmesini isteyebilirsin.";
            return target.Name + " ile ilişkileri geliştiriyor: haftada +" + DiplomatRules.RelationPerWeek(Charm(diplomat)) + ", şimdi "
                + SchemeFactors.Signed(Hero.MainHero.GetRelation(target)) + " (" + DiplomatRules.MissionCap + "'a kadar).";
        }

        // ---- Savaş tehdidi ----

        public sealed class ThreatLine
        {
            public Kingdom Kingdom;
            public int Seen;
            public string Reason;
        }

        private static IFaction Mine => Clan.PlayerClan.MapFaction;

        /// <summary>Barışta olduğun krallıklar ve diplomatın gözüyle sana karşı savaş hazırlığı (haftalık sabit).</summary>
        public static List<ThreatLine> Threats(IntrigueState state, Hero diplomat)
        {
            var lines = new List<ThreatLine>();
            IFaction mine = Mine;
            if (mine == null || diplomat == null) return lines;
            var model = Campaign.Current.Models.DiplomacyModel;
            foreach (Kingdom kingdom in Kingdom.All.Where(k => !k.IsEliminated && k != mine && k.RulingClan != null && !FactionManager.IsAtWarAgainstFaction(k, mine))
                         .OrderBy(k => k.StringId, StringComparer.Ordinal))
            {
                float score = model.GetScoreOfDeclaringWar(kingdom, mine, kingdom.RulingClan, out TextObject reason, true);
                int roll = TreasuryRules.WeeklyRoll(state.Seed, Today / 7, diplomat.StringId + "|threat|" + kingdom.StringId);
                lines.Add(new ThreatLine
                {
                    Kingdom = kingdom,
                    Seen = DiplomatRules.SeenThreat(DiplomatRules.Threat(score), Charm(diplomat), roll),
                    Reason = reason?.ToString() ?? "",
                });
            }
            return lines.OrderByDescending(l => l.Seen).ToList();
        }

        /// <summary>Haftada bir: görevdeki ilişki kazancı ve savaşa hazırlanan krallık için erken uyarı.</summary>
        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            Hero diplomat = Working(state);
            if (diplomat == null) return;
            int charm = Charm(diplomat);

            Hero target = Target(state);
            if (target != null)
            {
                if (!target.IsAlive || target.Clan == Clan.PlayerClan) ClearTarget(state);
                else
                {
                    SchemeOutcomes.Relate(target, DiplomatRules.RelationPerWeek(charm));
                    diplomat.AddSkillXp(DefaultSkills.Charm, 50);
                    if (Hero.MainHero.GetRelation(target) >= DiplomatRules.MissionCap)
                    {
                        ClearTarget(state);
                        SchemeOutcomes.Notify(state, "Diplomatın başardı: " + target.Name,
                            diplomat.Name + " haftalardır " + target.Name + " ile mektuplaşıyor, armağanlar gönderiyordu. Aranız artık çok iyi ("
                            + SchemeFactors.Signed(Hero.MainHero.GetRelation(target)) + "). Ona yeni bir görev verebilirsin.", target, CardImages.Diplomat);
                    }
                }
            }

            foreach (ThreatLine threat in Threats(state, diplomat).Where(t => t.Seen >= 2))
            {
                string key = threat.Kingdom.StringId;
                if (state.DiplomatWarned.TryGetValue(key, out int until) && Today < until) continue;
                state.DiplomatWarned[key] = Today + DiplomatRules.WarningCooldownDays;
                if (state.Roll100() > DiplomatRules.WarningChance(charm)) continue;
                SchemeOutcomes.Notify(state, "Diplomatın uyarıyor: " + threat.Kingdom.Name,
                    diplomat.Name + " haber aldı: " + threat.Kingdom.Name + " sarayında sana savaş açmak konuşuluyor"
                    + (threat.Reason.Length > 0 ? " (" + threat.Reason + ")" : "") + ".\n\nHükümdarları " + threat.Kingdom.Leader?.Name
                    + " ile ilişkin " + SchemeFactors.Signed(threat.Kingdom.Leader?.GetRelation(Hero.MainHero) ?? 0)
                    + ". Hediye gönder, diplomatına onunla ilişkileri geliştirmesini söyle ya da sınır kalelerini güçlendir.",
                    threat.Kingdom.Leader, CardImages.Diplomat);
                state.AddLog(Today, "Diplomat uyardı: " + threat.Kingdom.Name + " savaşa hazırlanıyor");
            }
        }

        // ---- Rapor (Divan ve Insert menüsü) ----

        public static string ReportText(IntrigueState state)
        {
            string blocked = Blocked(state);
            if (blocked != null) return blocked + ".";
            Hero diplomat = Working(state);
            var lines = new List<string> { DetailText(diplomat), MissionText(state) };
            foreach (ThreatLine threat in Threats(state, diplomat))
                lines.Add("· " + threat.Kingdom.Name + ": " + DiplomatRules.ThreatText(threat.Seen) + "; hükümdarıyla "
                    + SchemeFactors.Signed(threat.Kingdom.Leader?.GetRelation(Hero.MainHero) ?? 0) + ".");
            return string.Join("\n", lines);
        }
    }
}
