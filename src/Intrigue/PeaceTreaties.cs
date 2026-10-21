using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Bundle;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Barış Masası (tasarım: docs/superpowers/specs/2026-10-21-baris-masasi.md): savaştığın krallıktan haraç, toplu altın, kale ya da şehir,
    /// garnizonlarından asker iste; ya da altın, haraç ver. Karşı taraf pazarlık gücüne (savaş yorgunluğu, güç, başka cepheler, oyunun barış
    /// puanı) ve şartların ağırlığına göre kabul eder. Kaybeden taraf bazen elçi gönderip kendisi barış ister. Kurallar: PeaceTreatyRules.
    /// Yalnızca hükümdar antlaşma yapar. Diplomacy'nin barış koşullarına (en kısa savaş süresi, görev, kuşatma, isyancı krallık) uyulur.
    /// Pencere: UI.PeaceTableScreen.
    /// </summary>
    public static class PeaceTreaties
    {
        private static int Today => SchemeFactors.Today;
        private static Kingdom Mine => Clan.PlayerClan.Kingdom;
        /// <summary>Masada gösterilen yerleşim sayısı (sınırına en yakınları).</summary>
        public const int MaxFiefsOnTable = 8;

        /// <summary>Masadaki şartlar. Haraçlar PeaceTreatyRules.TributeLevels sırası, altın ve asker sayı.</summary>
        public sealed class Terms
        {
            /// <summary>Onların sana ödeyeceği haraç (sıra).</summary>
            public int Tribute;
            /// <summary>Onların ödeyeceği toplu altın (savaş tazminatı).</summary>
            public int Gold;
            /// <summary>Garnizonlarından senin partine geçecek asker.</summary>
            public int Troops;
            /// <summary>Sana bırakacakları şehir ve kaleler (yerleşim kimliği).</summary>
            public readonly HashSet<string> Fiefs = new HashSet<string>();
            /// <summary>Senin vereceğin toplu altın.</summary>
            public int Gift;
            /// <summary>Senin ödeyeceğin haraç (sıra).</summary>
            public int Pay;

            public bool IsWhite => Tribute == 0 && Gold == 0 && Troops == 0 && Fiefs.Count == 0 && Gift == 0 && Pay == 0;
        }

        public sealed class Leverage
        {
            public int Willingness;
            /// <summary>Diplomacy'nin savaş yorgunluğu; Diplomacy yoksa -1.</summary>
            public float TheirExhaustion, OurExhaustion;
            public double Ratio;
            public int OtherWars;
            public bool GameLeans, Asked;
            /// <summary>Diplomatın katkısı (sarayda değilse 0).</summary>
            public int Diplomat;
        }

        // ---- Okumalar ----

        public static Leverage LeverageOf(IntrigueState state, Kingdom enemy)
        {
            Kingdom mine = Mine;
            float theirs = DiplomacyCompat.WarExhaustion(enemy, mine), ours = DiplomacyCompat.WarExhaustion(mine, enemy);
            var leverage = new Leverage
            {
                TheirExhaustion = theirs,
                OurExhaustion = ours,
                Ratio = mine.CurrentTotalStrength / Math.Max(1f, enemy.CurrentTotalStrength),
                OtherWars = Kingdom.All.Count(k => k != mine && k != enemy && !k.IsEliminated && FactionManager.IsAtWarAgainstFaction(k, enemy)),
                GameLeans = Campaign.Current.Models.DiplomacyModel.GetScoreOfDeclaringPeace(enemy, mine) > 0,
                Asked = state != null && state.PeaceTalkWith(enemy.StringId).AskedUntil >= Today,
                Diplomat = Intrigue.Diplomat.PeaceBonus(),
            };
            leverage.Willingness = PeaceTreatyRules.Willingness(Math.Max(0, theirs), Math.Max(0, ours), leverage.Ratio, leverage.OtherWars,
                leverage.GameLeans, leverage.Asked, leverage.Diplomat);
            return leverage;
        }

        /// <summary>Toplu altını hükümdar kendi kesesinden öder.</summary>
        public static int Treasury(Kingdom enemy) => enemy.Leader?.Gold ?? 0;

        private static int PartySpace() =>
            MobileParty.MainParty == null ? 0 : Math.Max(0, MobileParty.MainParty.Party.PartySizeLimit - MobileParty.MainParty.MemberRoster.TotalManCount);

        private static IEnumerable<MobileParty> Garrisons(Kingdom enemy) =>
            enemy.Fiefs.Select(t => t.GarrisonParty).Where(g => g != null && g.MemberRoster != null);

        private static int GarrisonTroops(Kingdom enemy) =>
            Garrisons(enemy).Sum(g => Enumerable.Range(0, g.MemberRoster.Count).Select(g.MemberRoster.GetElementCopyAtIndex)
                .Where(e => e.Character != null && !e.Character.IsHero).Sum(e => Math.Max(0, e.Number - e.WoundedNumber)));

        /// <summary>İstenebilecek asker: garnizonlarındaki sağlam asker ve partindeki boş yer kadar.</summary>
        public static int TroopsAvailable(Kingdom enemy) => Math.Min(GarrisonTroops(enemy), PartySpace());

        /// <summary>Masadaki şehir ve kaleleri: kuşatmada olmayanlar, senin topraklarına (yoksa sana) en yakından.</summary>
        public static List<Town> FiefsOnTable(Kingdom enemy)
        {
            List<Vec2> ours = Mine.Fiefs.Select(t => t.Settlement.GetPosition2D).ToList();
            if (ours.Count == 0 && MobileParty.MainParty != null) ours.Add(MobileParty.MainParty.GetPosition2D);
            float Distance(Town t) => ours.Count == 0 ? 0 : ours.Min(p => p.Distance(t.Settlement.GetPosition2D));
            return enemy.Fiefs.Where(t => t.Settlement != null && !t.Settlement.IsUnderSiege).OrderBy(Distance).Take(MaxFiefsOnTable).ToList();
        }

        public static int FiefWeight(Kingdom enemy, Town town) =>
            PeaceTreatyRules.FiefWeight(town.IsTown, town.OwnerClan != null && town.OwnerClan == enemy.RulingClan, false);

        /// <summary>Şartların toplam ağırlığı (verdiklerin düşer); bütün yerlerini istiyorsan +50.</summary>
        public static int Weight(Kingdom enemy, Terms terms)
        {
            int treasury = Treasury(enemy);
            var (daily, days) = PeaceTreatyRules.TributeLevels[terms.Tribute];
            var (payDaily, payDays) = PeaceTreatyRules.TributeLevels[terms.Pay];
            List<Town> fiefs = enemy.Fiefs.Where(t => terms.Fiefs.Contains(t.Settlement.StringId)).ToList();
            int weight = PeaceTreatyRules.TributeWeight(daily, days, treasury) + PeaceTreatyRules.GoldWeight(terms.Gold, treasury)
                + PeaceTreatyRules.TroopWeight(terms.Troops) + fiefs.Sum(t => FiefWeight(enemy, t))
                + (fiefs.Count > 0 && fiefs.Count >= enemy.Fiefs.Count ? PeaceTreatyRules.FiefWeight(false, false, true) - PeaceTreatyRules.FiefWeight(false, false, false) : 0)
                - PeaceTreatyRules.GiftRelief(terms.Gift, treasury) - PeaceTreatyRules.TributeRelief(payDaily, payDays, treasury);
            return weight;
        }

        public static int Chance(IntrigueState state, Kingdom enemy, Terms terms) =>
            PeaceTreatyRules.Chance(LeverageOf(state, enemy).Willingness, Weight(enemy, terms));

        /// <summary>null: bu krallıkla masaya oturulabilir; değilse neden.</summary>
        public static string Blocked(IntrigueState state, Kingdom enemy)
        {
            Kingdom mine = Mine;
            if (mine == null) return "Bir krallıkta değilsin";
            if (enemy == null || enemy.IsEliminated || enemy == mine || !FactionManager.IsAtWarAgainstFaction(mine, enemy)) return "Bu krallıkla savaşta değilsiniz";
            if (mine.Leader != Hero.MainHero) return "Barışı hükümdarın yapar (Krallık > Diplomasi'den barış önerebilirsin)";
            if (enemy.Leader == null) return "Hükümdarları yok";
            if (Hero.MainHero.IsPrisoner) return "Esirken masaya oturamazsın";
            string diplomacy = DiplomacyCompat.TreatyBlocked(mine, enemy);
            if (diplomacy != null) return diplomacy;
            int refused = state?.PeaceTalkWith(enemy.StringId).RefusedUntil ?? 0;
            return refused > Today ? "Son teklifini reddettiler: " + (refused - Today) + " gün sonra yeniden dene" : null;
        }

        /// <summary>null: şartlar yerine getirilebilir; değilse neden.</summary>
        public static string TermsBlocked(Kingdom enemy, Terms terms)
        {
            if (terms.Tribute > 0 && terms.Pay > 0) return "Hem haraç alıp hem haraç veremezsin";
            if (terms.Gold > Treasury(enemy)) return "Hükümdarlarının kesesi o kadar altına yetmez";
            if (terms.Gift > Hero.MainHero.Gold) return "Kesen o kadar altına yetmez";
            if (terms.Troops > TroopsAvailable(enemy)) return "O kadar askeri alamazsın (partinde yer ya da garnizonlarında asker yok)";
            return null;
        }

        public static string TributeText(int level)
        {
            var (daily, days) = PeaceTreatyRules.TributeLevels[SchemeMath.Clamp(level, 0, PeaceTreatyRules.TributeLevels.Count - 1)];
            return daily <= 0 ? "yok" : "günde " + daily.ToString("N0") + ", " + days + " gün";
        }

        /// <summary>"günde 250 haraç (60 gün) · 15.000 dinar · 50 asker · Pen Cannoc"; şartsızsa "Şartsız barış".</summary>
        public static string Describe(Kingdom enemy, Terms terms)
        {
            var theirs = new List<string>();
            if (terms.Tribute > 0) theirs.Add("haraç " + TributeText(terms.Tribute));
            if (terms.Gold > 0) theirs.Add(SchemeFactors.Money(terms.Gold) + " tazminat");
            if (terms.Troops > 0) theirs.Add(terms.Troops + " asker");
            theirs.AddRange(enemy.Fiefs.Where(t => terms.Fiefs.Contains(t.Settlement.StringId)).Select(t => t.Name + (t.IsTown ? " (şehir)" : " (kale)")));
            var ours = new List<string>();
            if (terms.Gift > 0) ours.Add(SchemeFactors.Money(terms.Gift));
            if (terms.Pay > 0) ours.Add("haraç " + TributeText(terms.Pay));
            if (theirs.Count == 0 && ours.Count == 0) return "Şartsız barış.";
            return (theirs.Count > 0 ? "Onlar verir: " + string.Join(", ", theirs) + "." : "")
                + (ours.Count > 0 ? (theirs.Count > 0 ? " " : "") + "Sen verirsin: " + string.Join(", ", ours) + "." : "");
        }

        // ---- Teklif ----

        public sealed class Outcome
        {
            public bool Accepted;
            public string Text;
        }

        /// <summary>Teklifi götürür: kabul ederlerse barış ve şartlar uygulanır, etmezlerse bir hafta bekletirler.</summary>
        public static Outcome Propose(IntrigueState state, Kingdom enemy, Terms terms)
        {
            string blocked = Blocked(state, enemy) ?? TermsBlocked(enemy, terms);
            if (blocked != null) throw new InvalidOperationException(blocked);
            int chance = Chance(state, enemy, terms);
            if (state.Roll100() <= chance) return new Outcome { Accepted = true, Text = Apply(state, enemy, terms) };

            state.PeaceTalkWith(enemy.StringId).RefusedUntil = Today + PeaceTreatyRules.RefuseCooldownDays;
            bool insulted = chance < 30;
            if (insulted) SchemeOutcomes.Relate(enemy.Leader, PeaceTreatyRules.RefuseRelation);
            state.AddLog(Today, enemy.Name + " barış teklifini reddetti");
            return new Outcome
            {
                Accepted = false,
                Text = enemy.Leader.Name + " teklifini reddetti" + (insulted ? ", şartlarını hakaret saydı (ilişki " + PeaceTreatyRules.RefuseRelation + ")" : "")
                    + ". Savaş sürüyor; " + PeaceTreatyRules.RefuseCooldownDays + " gün sonra yeniden teklif götürebilirsin.",
            };
        }

        /// <summary>Barış ve şartlar: önce barış (yerler barışta el değiştirsin), sonra yerler, altın, asker. Haraçı oyun her gün öder.</summary>
        public static string Apply(IntrigueState state, Kingdom enemy, Terms terms)
        {
            Kingdom mine = Mine;
            Hero ruler = enemy.Leader, me = Hero.MainHero;
            var (daily, days) = PeaceTreatyRules.TributeLevels[terms.Tribute];
            var (payDaily, payDays) = PeaceTreatyRules.TributeLevels[terms.Pay];
            int weight = Weight(enemy, terms);
            List<Town> fiefs = enemy.Fiefs.Where(t => terms.Fiefs.Contains(t.Settlement.StringId)).ToList();
            var lines = new List<string>();

            if (daily > 0) MakePeaceAction.ApplyByKingdomDecision(enemy, mine, daily, days);
            else if (payDaily > 0) MakePeaceAction.ApplyByKingdomDecision(mine, enemy, payDaily, payDays);
            else MakePeaceAction.ApplyByKingdomDecision(mine, enemy, 0, 0);
            lines.Add(mine.Name + " ile " + enemy.Name + " arasında barış imzalandı.");
            if (daily > 0) lines.Add(enemy.Name + " sana " + TributeText(terms.Tribute) + " haraç ödeyecek.");
            if (payDaily > 0) lines.Add("Sen " + Tr.Dative(enemy.Name.ToString()) + " " + TributeText(terms.Pay) + " haraç ödeyeceksin.");

            foreach (Town town in fiefs)
            {
                Hero formerOwner = town.OwnerClan?.Leader;
                ChangeOwnerOfSettlementAction.ApplyByBarter(me, town.Settlement);
                if (formerOwner != null && formerOwner != ruler) SchemeOutcomes.Relate(formerOwner, -10);
                lines.Add(town.Name + " (" + (town.IsTown ? "şehir" : "kale") + ") artık senin" + (formerOwner != null ? "; " + formerOwner.Name + " sana kırgın" : "") + ".");
            }
            if (terms.Gold > 0)
            {
                int paid = Math.Min(terms.Gold, ruler.Gold);
                if (paid > 0) GiveGoldAction.ApplyBetweenCharacters(ruler, me, paid);
                lines.Add(ruler.Name + " savaş tazminatı olarak " + SchemeFactors.Money(paid) + " ödedi.");
            }
            if (terms.Gift > 0)
            {
                int given = Math.Min(terms.Gift, me.Gold);
                if (given > 0) GiveGoldAction.ApplyBetweenCharacters(me, ruler, given);
                lines.Add(Tr.Dative(ruler.Name.ToString()) + " " + SchemeFactors.Money(given) + " verdin.");
            }
            if (terms.Troops > 0)
            {
                int moved = TakeTroops(enemy, Math.Min(terms.Troops, TroopsAvailable(enemy)));
                lines.Add(moved > 0 ? "Garnizonlarından " + moved + " asker partine katıldı." : "Garnizonlarında verecek asker kalmamıştı.");
            }
            if (weight >= 40) SchemeOutcomes.Relate(ruler, -Math.Min(10, weight / 10));

            PeaceTalk talk = state.PeaceTalkWith(enemy.StringId);
            talk.RefusedUntil = 0;
            talk.AskedUntil = 0;
            state.AddLog(Today, enemy.Name + " ile barış: " + Describe(enemy, terms));
            return string.Join("\n", lines);
        }

        /// <summary>Garnizonlarından sağlam, yüksek kademeli askerleri önce alır; partine ekler. Taşınan sayı.</summary>
        private static int TakeTroops(Kingdom enemy, int count)
        {
            int moved = 0;
            foreach (MobileParty garrison in Garrisons(enemy).OrderByDescending(g => g.MemberRoster.TotalHealthyCount).ToList())
            {
                if (moved >= count) break;
                TroopRoster roster = garrison.MemberRoster;
                var healthy = Enumerable.Range(0, roster.Count).Select(roster.GetElementCopyAtIndex)
                    .Where(e => e.Character != null && !e.Character.IsHero && e.Number - e.WoundedNumber > 0)
                    .OrderByDescending(e => e.Character.Tier).Select(e => (e.Character, Healthy: e.Number - e.WoundedNumber)).ToList();
                foreach (var (character, available) in healthy)
                {
                    int take = Math.Min(available, count - moved);
                    if (take <= 0) break;
                    roster.AddToCounts(character, -take);
                    MobileParty.MainParty.MemberRoster.AddToCounts(character, take);
                    moved += take;
                }
            }
            return moved;
        }

        // ---- Elçi: kaybeden taraf kendisi barış ister ----

        /// <summary>Haftada bir: hükümdarsan, savaştığın ve köşeye sıkışmış bir krallık elçi gönderebilir (haftada en fazla bir).</summary>
        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            Kingdom mine = Mine;
            if (state == null || mine == null || mine.Leader != Hero.MainHero) return;
            foreach (Kingdom enemy in Kingdom.All.Where(k => k != mine && !k.IsEliminated && FactionManager.IsAtWarAgainstFaction(k, mine))
                         .OrderBy(k => k.StringId, StringComparer.Ordinal).ToList())
            {
                if (Blocked(state, enemy) != null) continue;
                PeaceTalk talk = state.PeaceTalkWith(enemy.StringId);
                if (talk.EnvoyUntil > Today) continue;
                int willingness = LeverageOf(state, enemy).Willingness;
                int chance = PeaceTreatyRules.EnvoyChance(willingness);
                if (chance <= 0 || state.Roll100() > chance + Intrigue.Diplomat.EnvoyBonus()) continue;
                talk.EnvoyUntil = Today + PeaceTreatyRules.EnvoyCooldownDays;
                talk.AskedUntil = Today + PeaceTreatyRules.AskedDays;
                ShowEnvoy(state, enemy);
                return;
            }
        }

        /// <summary>Elçinin getirdiği teklif: kabul edeceklerinden emin oldukları kadar savaş tazminatı.</summary>
        public static Terms EnvoyOffer(IntrigueState state, Kingdom enemy) =>
            new Terms { Gold = PeaceTreatyRules.EnvoyGold(LeverageOf(state, enemy).Willingness, Treasury(enemy)) };

        private static void ShowEnvoy(IntrigueState state, Kingdom enemy)
        {
            Terms offer = EnvoyOffer(state, enemy);
            string text = enemy.Leader.Name + " elçisini gönderdi. " + enemy.Name + " bu savaştan yoruldu ve barış istiyor.\n\n"
                + "Teklifleri: " + (offer.Gold > 0 ? SchemeFactors.Money(offer.Gold) + " savaş tazminatı ve barış." : "şartsız barış.")
                + "\n\nKabul edebilir, Barış Masası'na oturup daha fazlasını isteyebilir (elçi gönderdikleri için elleri zayıf) ya da geri çevirebilirsin.";
            var refuse = new UI.EventCards.CardOption
            {
                Label = "Geri çevir: savaş sürsün",
                Apply = () => state.AddLog(Today, enemy.Name + " elçisi geri çevrildi"),
            };
            var options = new List<UI.EventCards.CardOption>
            {
                new UI.EventCards.CardOption
                {
                    Label = "Kabul et: " + (offer.Gold > 0 ? SchemeFactors.Money(offer.Gold) + " ve barış" : "şartsız barış"),
                    Apply = () => MenuUI.SafeRun("Barış (elçi)", () =>
                    {
                        string blocked = Blocked(state, enemy);
                        if (blocked != null) throw new InvalidOperationException(blocked);
                        string result = Apply(state, enemy, offer);
                        SchemeOutcomes.Notify(state, "Barış: " + enemy.Name, result, enemy.Leader, CardImages.Treaty);
                    }),
                },
                new UI.EventCards.CardOption
                {
                    Label = "Barış Masası'na otur: daha fazlasını iste",
                    Apply = () => MenuUI.Defer(() =>
                    {
                        if (!UI.PeaceTableScreen.Open(enemy, offer)) MenuUI.Error("Barış Masası açılamadı");
                    }),
                },
                refuse,
            };
            UI.EventCards.Show("BARIŞ ELÇİSİ", enemy.Name + " barış istiyor", text, CardImages.Envoy, enemy.Leader, options, refuse, null);
        }
    }
}
