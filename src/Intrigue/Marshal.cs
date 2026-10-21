using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Divan: başkomutan (P2.14, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-baskomutan.md). Savaş meclisi: savaştığın krallıkların gücü
    /// ve başkomutanın tavsiyesi, düşman orduları (nerede, kaç kişi, nereye), zayıf düşman kaleleri, kendi savunman; şehrine, kalene ya da
    /// köyüne yürüyen bir orduya erken uyarı. Sayılar başkomutanın gözüyle: Tactics düşükse yanılır; Scouting ne kadar uzağı gördüğünü ve
    /// uyarının olasılığını belirler. Başkomutan sahadayken de (parti başında) çalışır; yalnızca esirken durur.
    /// </summary>
    public static class Marshal
    {
        public const string Seat = "başkomutanın";
        private static int Today => SchemeFactors.Today;

        // ---- Koltuk ----

        public static Hero Current(IntrigueState state)
        {
            if (state?.MarshalId == null) return null;
            Hero hero = Hero.Find(state.MarshalId);
            return hero != null && hero.IsAlive && hero.Clan == Clan.PlayerClan ? hero : null;
        }

        public static Hero Working(IntrigueState state)
        {
            Hero hero = Current(state);
            return hero != null && !hero.IsPrisoner && !SchemeFactors.IsHostageAbroad(hero) ? hero : null;
        }

        public static string Blocked(IntrigueState state)
        {
            Hero hero = Current(state);
            if (hero == null) return "Başkomutanın yok";
            if (hero.IsPrisoner) return hero.Name + " esir; haber alamıyor";
            return SchemeFactors.IsHostageAbroad(hero) ? hero.Name + " rehine olarak yabancı sarayda; haber alamıyor" : null;
        }

        public static int Tactics(Hero hero) => hero?.GetSkillValue(DefaultSkills.Tactics) ?? 0;
        public static int Scouting(Hero hero) => hero?.GetSkillValue(DefaultSkills.Scouting) ?? 0;

        public static IEnumerable<Hero> Candidates(IntrigueState state) => Council.Candidates(state, Seat);

        public static string Label(Hero hero) =>
            hero.Name + " - Tactics " + Tactics(hero) + ", Scouting " + Scouting(hero) + ", tahmin payı %" + MarshalRules.ErrorPercent(Tactics(hero))
            + (hero.IsPrisoner ? "  [esir]" : "");

        public static string DetailText(Hero hero) =>
            "Tahmin payı %" + MarshalRules.ErrorPercent(Tactics(hero)) + " · görüş " + (int)MarshalRules.KnownRange(Scouting(hero))
            + " · erken uyarı %" + MarshalRules.WarningChance(Scouting(hero));

        public static void Assign(IntrigueState state, Hero hero)
        {
            Council.EnsureFree(state, hero, Seat);
            if (hero.IsPrisoner) throw new InvalidOperationException(hero.Name + " esir; başkomutan olamaz");
            state.MarshalId = hero.StringId;
            state.AddLog(Today, hero.Name + " başkomutan oldu");
            MenuUI.Success(hero.Name + " artık başkomutanın");
        }

        public static void Dismiss(IntrigueState state)
        {
            if (state.MarshalId == null) throw new InvalidOperationException("Başkomutanın yok");
            Hero old = Current(state);
            state.MarshalId = null;
            state.AddLog(Today, (old?.Name.ToString() ?? "Başkomutan") + " görevden alındı");
            MenuUI.Success("Başkomutan görevden alındı");
        }

        // ---- Başkomutanın gözü ----

        private static IFaction Mine => Clan.PlayerClan.MapFaction;

        public static List<Kingdom> Enemies() =>
            Kingdom.All.Where(k => !k.IsEliminated && Mine != null && k != Mine && FactionManager.IsAtWarAgainstFaction(k, Mine))
                .OrderBy(k => k.StringId, StringComparer.Ordinal).ToList();

        /// <summary>
        /// Diplomacy'nin Savaş Raporu'na eklenen satırlar (BundleShim): orta sütunda başlık, solda biz, sağda onlar. Başkomutan yoksa tek satır.
        /// Satırlar kısa tutulur: rapor penceresinin genişliği sabit.
        /// </summary>
        public static List<(string Text, string Ours, string Theirs)> WarReportRows(Kingdom enemy)
        {
            var rows = new List<(string Text, string Ours, string Theirs)>();
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || enemy == null || Mine == null) return rows;
            Hero marshal = Working(state);
            if (marshal == null)
            {
                rows.Add((Current(state) == null ? "Başkomutanın yok (Divan'dan ata)" : "Başkomutanın haber alamıyor", "", ""));
                return rows;
            }
            WarLine war = Wars(state, marshal).FirstOrDefault(w => w.Kingdom == enemy);
            rows.Add(("Başkomutan " + marshal.Name + " (tahmin payı %" + MarshalRules.ErrorPercent(Tactics(marshal)) + ")", "", ""));
            if (war == null) return rows;
            int ourArmies = Mine is Kingdom kingdom ? kingdom.Armies.Count : 0;
            rows.Add(("Güç (başkomutanın tahmini)", "~" + war.Ours, "~" + war.Theirs));
            rows.Add(("Sahadaki ordular", ourArmies.ToString(), war.Armies.ToString()));
            List<ArmyLine> threats = Threats(state, marshal).Where(a => a.Party?.MapFaction == enemy).ToList();
            rows.Add(("Topraklarına yürüyen", "", threats.Count == 0 ? "yok" : threats.Count + ", " + MarshalRules.TravelText(threats.Min(a => a.HoursToTarget))));
            rows.Add((MarshalRules.Advice(war.Ratio), "", ""));
            return rows;
        }

        private static bool IsEnemy(IFaction faction) => faction != null && Mine != null && faction != Mine && FactionManager.IsAtWarAgainstFaction(faction, Mine);

        /// <summary>Başkomutanın gördüğü sayı: gerçeği ± hata payı (haftalık sabit), yuvarlanmış.</summary>
        public static int Seen(IntrigueState state, Hero marshal, double value, string key)
        {
            int roll = TreasuryRules.WeeklyRoll(state.Seed, Today / 7, marshal.StringId + "|war|" + key);
            return MarshalRules.Round(TreasuryRules.Noisy(value, MarshalRules.ErrorPercent(Tactics(marshal)), roll));
        }

        private static Vec2 Here => MobileParty.MainParty.GetPosition2D;

        private static float Range(Hero marshal) => MarshalRules.KnownRange(Scouting(marshal));

        public sealed class WarLine
        {
            public Kingdom Kingdom;
            public int Theirs;
            public int Ours;
            public int Armies;
            public double Ratio => Ours <= 0 ? 9.9 : (double)Theirs / Ours;
        }

        public static List<WarLine> Wars(IntrigueState state, Hero marshal)
        {
            int ours = (int)Mine.CurrentTotalStrength;
            return Enemies().Select(k => new WarLine
            {
                Kingdom = k,
                Theirs = Seen(state, marshal, k.CurrentTotalStrength, k.StringId),
                Ours = MarshalRules.Round(ours),
                Armies = k.Armies.Count,
            }).ToList();
        }

        public sealed class ArmyLine
        {
            public MobileParty Party;
            public string Name;
            public int Men;
            public Settlement Near;
            public Settlement Target;
            public string Intent;
            public float Distance;
            public int HoursToTarget;
        }

        /// <summary>Düşman orduları ve ordusuz büyük lord partileri (100+), başkomutanın görüş menzilinde; sana en yakından.</summary>
        public static List<ArmyLine> Armies(IntrigueState state, Hero marshal)
        {
            float range = Range(marshal);
            var lines = new List<ArmyLine>();
            foreach (Kingdom kingdom in Enemies())
            {
                foreach (Army army in kingdom.Armies.Where(a => a.LeaderParty != null && a.LeaderParty.IsActive))
                    Add(lines, state, marshal, army.LeaderParty, army.Name.ToString(), army.TotalManCount, TargetOf(army.LeaderParty, army), range);
                foreach (MobileParty party in kingdom.AllParties.Where(p => p.IsActive && p.IsLordParty && p.Army == null && p.LeaderHero != null
                             && p.MemberRoster.TotalManCount >= 100))
                    Add(lines, state, marshal, party, party.Name.ToString(), party.MemberRoster.TotalManCount, party.TargetSettlement, range);
            }
            return lines.OrderBy(l => l.Distance).ToList();
        }

        private static void Add(List<ArmyLine> lines, IntrigueState state, Hero marshal, MobileParty party, string name, int men, Settlement target, float range)
        {
            Vec2 at = party.GetPosition2D;
            float distance = at.Distance(Here);
            if (distance > range) return;
            string intent = IntentOf(party, target);
            lines.Add(new ArmyLine
            {
                Party = party,
                Name = name,
                Men = Seen(state, marshal, men, party.StringId),
                Near = Nearest(at),
                Target = intent != null ? target : null,
                Intent = intent,
                Distance = distance,
                HoursToTarget = target != null ? MarshalRules.TravelHours(at.Distance(target.GetPosition2D), party.Speed) : 0,
            });
        }

        /// <summary>"Pravend'i kuşatmaya", "Tirby köyünü yağmaya".</summary>
        public static string Goal(ArmyLine line) =>
            (line.Target.IsVillage ? line.Target.Name + " köyünü" : Tr.Accusative(line.Target.Name.ToString())) + " " + line.Intent;

        private static Settlement TargetOf(MobileParty leader, Army army) => leader.TargetSettlement ?? army.AiBehaviorObject as Settlement;

        /// <summary>Başkasının yerleşimine kuşatma ya da yağma için gidiyorsa niyeti ("kuşatmaya", "yağmaya"); değilse null.</summary>
        private static string IntentOf(MobileParty party, Settlement target)
        {
            if (target == null || target.MapFaction == party.MapFaction) return null;
            AiBehavior behavior = party.DefaultBehavior;
            if (behavior == AiBehavior.BesiegeSettlement || behavior == AiBehavior.AssaultSettlement) return "kuşatmaya";
            return behavior == AiBehavior.RaidSettlement ? "yağmaya" : null;
        }

        private static Settlement Nearest(Vec2 at) =>
            Settlement.All.Where(s => s.IsTown || s.IsCastle || s.IsVillage).OrderBy(s => s.GetPosition2D.DistanceSquared(at)).FirstOrDefault();

        public sealed class FortLine
        {
            public Settlement Settlement;
            public int Garrison;
            public int Militia;
            public int Wall;
            public int FoodDays;
            public int Defense;
            public float Distance;
            public List<string> Threats = new List<string>();
        }

        /// <summary>Menzildeki düşman şehir ve kaleleri, en zayıftan; sayılar başkomutanın gözüyle.</summary>
        public static List<FortLine> WeakTargets(IntrigueState state, Hero marshal, int count)
        {
            float range = Range(marshal);
            return Settlement.All.Where(s => s.IsFortification && s.Town != null && IsEnemy(s.MapFaction) && s.GetPosition2D.Distance(Here) <= range)
                .Select(s =>
                {
                    int garrison = Seen(state, marshal, s.Town.GarrisonParty?.MemberRoster.TotalManCount ?? 0, s.StringId + "|g");
                    int militia = Seen(state, marshal, s.Town.Militia, s.StringId + "|m");
                    int food = MarshalRules.FoodDays(s.Town.FoodStocks, s.Town.FoodChange);
                    return new FortLine
                    {
                        Settlement = s, Garrison = garrison, Militia = militia, Wall = s.Town.GetWallLevel(),
                        FoodDays = food == int.MaxValue ? food : Seen(state, marshal, food, s.StringId + "|f"),
                        Defense = MarshalRules.DefenseScore(garrison, militia, s.Town.GetWallLevel()),
                        Distance = s.GetPosition2D.Distance(Here),
                    };
                })
                .OrderBy(f => f.Defense).Take(count).ToList();
        }

        /// <summary>Klanının şehir ve kaleleri (kesin sayılar) ve onlara yürüyen, başkomutanın gördüğü ordular.</summary>
        public static List<FortLine> OwnDefense(IntrigueState state, Hero marshal)
        {
            List<ArmyLine> threats = Threats(state, marshal);
            return Clan.PlayerClan.Fiefs.Select(t => t.Settlement).Where(s => s != null).Select(s => new FortLine
            {
                Settlement = s,
                Garrison = s.Town.GarrisonParty?.MemberRoster.TotalManCount ?? 0,
                Militia = (int)s.Town.Militia,
                Wall = s.Town.GetWallLevel(),
                FoodDays = MarshalRules.FoodDays(s.Town.FoodStocks, s.Town.FoodChange),
                Defense = MarshalRules.DefenseScore(s.Town.GarrisonParty?.MemberRoster.TotalManCount ?? 0, (int)s.Town.Militia, s.Town.GetWallLevel()),
                Threats = threats.Where(a => a.Target == s || a.Target?.Village?.Bound == s)
                    .Select(a => a.Name + " (~" + a.Men + ") " + Goal(a) + " geliyor, " + MarshalRules.TravelText(a.HoursToTarget)).ToList(),
            }).ToList();
        }

        /// <summary>Klanının yerleşimlerine yürüyen düşmanlar (menzil sınırı yok: gözcüler senin topraklarını izler).</summary>
        public static List<ArmyLine> Threats(IntrigueState state, Hero marshal)
        {
            var lines = new List<ArmyLine>();
            bool Ours(Settlement target) => target != null && target.OwnerClan == Clan.PlayerClan;
            foreach (Kingdom kingdom in Enemies())
            {
                foreach (Army army in kingdom.Armies.Where(a => a.LeaderParty != null && a.LeaderParty.IsActive && Ours(TargetOf(a.LeaderParty, a))))
                    Add(lines, state, marshal, army.LeaderParty, army.Name.ToString(), army.TotalManCount, TargetOf(army.LeaderParty, army), float.MaxValue);
                foreach (MobileParty party in kingdom.AllParties.Where(p => p.IsActive && p.IsLordParty && p.Army == null && p.LeaderHero != null
                             && Ours(p.TargetSettlement)))
                    Add(lines, state, marshal, party, party.Name.ToString(), party.MemberRoster.TotalManCount, party.TargetSettlement, float.MaxValue);
            }
            return lines.Where(l => l.Intent != null).ToList();
        }

        // ---- Günlük: erken uyarı ----

        public static void Daily()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            Hero marshal = Working(state);
            if (marshal == null || Enemies().Count == 0) return;
            foreach (ArmyLine threat in Threats(state, marshal))
            {
                string key = threat.Party.StringId + "|" + threat.Target.StringId;
                if (state.MarshalWarned.TryGetValue(key, out int until) && Today < until) continue;
                state.MarshalWarned[key] = Today + MarshalRules.WarnMemoryDays;
                if (state.Roll100() > MarshalRules.WarningChance(Scouting(marshal))) continue;
                marshal.AddSkillXp(DefaultSkills.Scouting, MarshalRules.WarningXp);
                Settlement fief = threat.Target.IsVillage ? threat.Target.Village.Bound : threat.Target;
                SchemeOutcomes.Notify(state, "Başkomutanın uyarıyor: " + threat.Target.Name,
                    marshal.Name + " gözcülerinden haber aldı: " + threat.Name + " (~" + threat.Men + " kişi) " + Goal(threat)
                    + " geliyor; " + MarshalRules.TravelText(threat.HoursToTarget)
                    + " uzakta.\n\n" + fief.Name + ": garnizon " + (fief.Town?.GarrisonParty?.MemberRoster.TotalManCount ?? 0) + ", milis "
                    + (int)(fief.Town?.Militia ?? 0) + ", erzak " + FoodText(fief) + ".\n\nAsker gönder, garnizonu doldur ya da yardım çağır.",
                    threat.Party.LeaderHero, CardImages.Marshal);
                state.AddLog(Today, "Başkomutan uyardı: " + threat.Name + ", " + Goal(threat));
            }
        }

        public static string FoodText(Settlement s)
        {
            if (s?.Town == null) return "?";
            int days = MarshalRules.FoodDays(s.Town.FoodStocks, s.Town.FoodChange);
            return days == int.MaxValue ? "yeterli" : days + " gün";
        }

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.MarshalId == null) return;
            if (Current(state) == null)
            {
                Hero lost = Hero.Find(state.MarshalId);
                state.AddLog(Today, "Başkomutan koltuğu boşaldı" + (lost != null ? ": " + lost.Name + (lost.IsAlive ? " artık klanında değil" : " öldü") : ""));
                state.MarshalId = null;
                return;
            }
            Working(state)?.AddSkillXp(DefaultSkills.Tactics, MarshalRules.WeeklyXp);
        }

        // ---- Liste menüsü için düz metin ----

        public static string ReportText(IntrigueState state)
        {
            string blocked = Blocked(state);
            if (blocked != null) return blocked + ".";
            Hero marshal = Working(state);
            var lines = new List<string> { DetailText(marshal) };
            List<WarLine> wars = Wars(state, marshal);
            if (wars.Count == 0) lines.Add("Kimseyle savaşta değilsin.");
            foreach (WarLine war in wars)
                lines.Add("· " + war.Kingdom.Name + ": güç ~" + war.Theirs + " / biz " + war.Ours + ". " + MarshalRules.Advice(war.Ratio) + ".");
            foreach (ArmyLine threat in Threats(state, marshal))
                lines.Add("! " + threat.Name + " (~" + threat.Men + ") " + Goal(threat) + " geliyor, " + MarshalRules.TravelText(threat.HoursToTarget));
            return string.Join("\n", lines);
        }
    }
}
