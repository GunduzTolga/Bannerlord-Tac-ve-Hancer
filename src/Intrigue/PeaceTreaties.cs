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
    /// Arayüz: Entrika ekranı > Savaş ve barış > Savaşlar (WarsVM, PeaceTableVM).
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
            /// <summary>Ellerindeki lordlarını (senin krallığının) bıraksınlar.</summary>
            public bool ReleaseOurs;
            /// <summary>Klanının elindeki lordlarını bırakırsın.</summary>
            public bool ReleaseTheirs;
            /// <summary>Hanedanlarından barışın teminatı olarak sana gelecek rehine (Hostages).</summary>
            public bool Hostage;
            /// <summary>Saldırmazlık anlaşması (Diplomacy), gün; 0 yok.</summary>
            public int PactDays;
            /// <summary>Barıştan sonra ticaret anlaşması ve ittifak (oyunun kendi sistemi).</summary>
            public bool Trade, Alliance;

            public bool IsWhite => Tribute == 0 && Gold == 0 && Troops == 0 && Fiefs.Count == 0 && Gift == 0 && Pay == 0
                && !ReleaseOurs && !ReleaseTheirs && !Hostage && PactDays == 0 && !Trade && !Alliance;

            public void Reset()
            {
                Tribute = Gold = Troops = Gift = Pay = PactDays = 0;
                Fiefs.Clear();
                ReleaseOurs = ReleaseTheirs = Hostage = Trade = Alliance = false;
            }
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

        /// <summary>Senin krallığının, onların elinde esir olan lordları.</summary>
        public static List<Hero> OurLordsHeld(Kingdom enemy) =>
            Mine.AliveLords.Where(h => h.IsPrisoner && h.PartyBelongedToAsPrisoner?.MapFaction == enemy).ToList();

        /// <summary>Onların, senin klanının (partin ya da zindanların) elinde esir olan lordları.</summary>
        public static List<Hero> TheirLordsHeld(Kingdom enemy) =>
            enemy.AliveLords.Where(h => h.IsPrisoner && HeldByYourClan(h.PartyBelongedToAsPrisoner)).ToList();

        private static bool HeldByYourClan(PartyBase party) =>
            party != null && (party == PartyBase.MainParty || party.Settlement?.OwnerClan == Clan.PlayerClan || party.MobileParty?.ActualClan == Clan.PlayerClan);

        /// <summary>Barışın teminatı olabilecek kişi (hükümdarın çocuğu önce); yoksa null.</summary>
        public static Hero HostageCandidate(IntrigueState state, Kingdom enemy) =>
            state == null || Hostages.OurSettlement() == null || state.HostageDealWith(enemy.StringId) != null ? null : Hostages.TheirCandidate(state, enemy);

        private static bool IsRulersChild(Kingdom enemy, Hero hero) => hero != null && (hero.Father == enemy.Leader || hero.Mother == enemy.Leader);

        /// <summary>Oyunun gözüyle bu krallık seninle ticaret anlaşması ister mi.</summary>
        public static bool WantsTrade(Kingdom enemy) =>
            Campaign.Current.Models.TradeAgreementModel.GetScoreOfStartingTradeAgreement(enemy, Mine, enemy.RulingClan, out _, false) > 0;

        /// <summary>Oyunun gözüyle bu krallık seninle ittifak ister mi.</summary>
        public static bool WantsAlliance(Kingdom enemy) =>
            Campaign.Current.Models.AllianceModel.GetScoreOfStartingAlliance(enemy, Mine, out _, false).ResultNumber > 0;

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
                - PeaceTreatyRules.GiftRelief(terms.Gift, treasury) - PeaceTreatyRules.TributeRelief(payDaily, payDays, treasury)
                + (terms.ReleaseOurs ? PeaceTreatyRules.PrisonerWeight(OurLordsHeld(enemy).Count) : 0)
                - (terms.ReleaseTheirs ? PeaceTreatyRules.PrisonerWeight(TheirLordsHeld(enemy).Count) : 0)
                + PeaceTreatyRules.PactWeight(terms.PactDays);
            if (terms.Hostage)
            {
                Hero hostage = HostageCandidate(IntrigueBehavior.Current?.State, enemy);
                if (hostage != null) weight += PeaceTreatyRules.HostageWeight(IsRulersChild(enemy, hostage));
            }
            if (terms.Trade) weight += PeaceTreatyRules.TradeWeight(WantsTrade(enemy));
            if (terms.Alliance) weight += PeaceTreatyRules.AllianceWeight(WantsAlliance(enemy));
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
            if (terms.Hostage && HostageCandidate(IntrigueBehavior.Current?.State, enemy) == null) return "Rehine verecek kimseleri yok ya da onu tutacak yerin yok";
            if (terms.PactDays > 0 && !DiplomacyCompat.IsAvailable) return "Saldırmazlık anlaşması Diplomacy ister";
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
            if (terms.ReleaseOurs) theirs.Add("esir lordlarını bırakır");
            if (terms.Hostage) theirs.Add("rehine verir");
            var ours = new List<string>();
            if (terms.Gift > 0) ours.Add(SchemeFactors.Money(terms.Gift));
            if (terms.Pay > 0) ours.Add("haraç " + TributeText(terms.Pay));
            if (terms.ReleaseTheirs) ours.Add("esirlerini bırakırsın");
            var both = new List<string>();
            if (terms.PactDays > 0) both.Add(terms.PactDays + " gün saldırmazlık");
            if (terms.Trade) both.Add("ticaret anlaşması");
            if (terms.Alliance) both.Add("ittifak");
            if (theirs.Count == 0 && ours.Count == 0 && both.Count == 0) return "Şartsız barış.";
            var parts = new List<string>();
            if (theirs.Count > 0) parts.Add("Onlar: " + string.Join(", ", theirs) + ".");
            if (ours.Count > 0) parts.Add("Sen: " + string.Join(", ", ours) + ".");
            if (both.Count > 0) parts.Add("Birlikte: " + string.Join(", ", both) + ".");
            return string.Join(" ", parts);
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
            // Elçinin kendi teklifi (ya da ondan hafifi) geçerliyken masada da kabul edilir: kendi sözlerini geri çevirmezler.
            if (WithinOffer(state, enemy, terms)) return new Outcome { Accepted = true, Text = Apply(state, enemy, terms, true) };
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

        /// <summary>Elçinin getirdiği teklif hâlâ geçerli ve bu şartlar ondan ağır değil mi.</summary>
        public static bool WithinOffer(IntrigueState state, Kingdom enemy, Terms terms)
        {
            PeaceTalk talk = state.PeaceTalkWith(enemy.StringId);
            return talk.AskedUntil > Today && Weight(enemy, terms) <= Weight(enemy, new Terms { Gold = talk.OfferGold });
        }

        /// <summary>
        /// Barış ve şartlar: önce barış, sonra asker (devredilecek yerlerin garnizonları dahil, masada da öyle sayıldı), yerler, altın.
        /// Haraçı oyun her gün öder. theirOffer: şartlar onların kendi teklifi (ağır şart kırgınlığı yok).
        /// </summary>
        public static string Apply(IntrigueState state, Kingdom enemy, Terms terms, bool theirOffer = false)
        {
            Kingdom mine = Mine;
            Hero ruler = enemy.Leader, me = Hero.MainHero;
            var (daily, days) = PeaceTreatyRules.TributeLevels[terms.Tribute];
            var (payDaily, payDays) = PeaceTreatyRules.TributeLevels[terms.Pay];
            int weight = Weight(enemy, terms);
            List<Town> fiefs = enemy.Fiefs.Where(t => terms.Fiefs.Contains(t.Settlement.StringId)).ToList();
            Hero hostage = terms.Hostage ? HostageCandidate(state, enemy) : null;
            string summary = Describe(enemy, terms);
            var lines = new List<string>();

            if (daily > 0) MakePeaceAction.ApplyByKingdomDecision(enemy, mine, daily, days);
            else if (payDaily > 0) MakePeaceAction.ApplyByKingdomDecision(mine, enemy, payDaily, payDays);
            else MakePeaceAction.ApplyByKingdomDecision(mine, enemy, 0, 0);
            lines.Add(mine.Name + " ile " + enemy.Name + " arasında barış imzalandı.");
            if (daily > 0) lines.Add(enemy.Name + " sana " + TributeText(terms.Tribute) + " haraç ödeyecek.");
            if (payDaily > 0) lines.Add("Sen " + Tr.Dative(enemy.Name.ToString()) + " " + TributeText(terms.Pay) + " haraç ödeyeceksin.");

            if (terms.Troops > 0)
            {
                int moved = TakeTroops(enemy, Math.Min(terms.Troops, TroopsAvailable(enemy)));
                lines.Add(moved > 0 ? "Garnizonlarından " + moved + " asker partine katıldı." : "Garnizonlarında verecek asker kalmamıştı.");
            }
            foreach (Town town in fiefs)
            {
                Hero formerOwner = town.OwnerClan?.Leader;
                bool resents = formerOwner != null && formerOwner != ruler;
                ChangeOwnerOfSettlementAction.ApplyByBarter(me, town.Settlement);
                if (resents) SchemeOutcomes.Relate(formerOwner, -10);
                lines.Add(town.Name + " (" + (town.IsTown ? "şehir" : "kale") + ") artık senin" + (resents ? "; " + formerOwner.Name + " sana kırgın" : "") + ".");
            }
            if (terms.Gold > 0)
            {
                int paid = Math.Min(terms.Gold, ruler.Gold);
                if (paid > 0) GiveGoldAction.ApplyBetweenCharacters(ruler, me, paid);
                lines.Add(paid > 0 ? ruler.Name + " savaş tazminatı olarak " + SchemeFactors.Money(paid) + " ödedi."
                    : ruler.Name + " savaş tazminatı ödeyemedi: kesesi boş.");
            }
            if (terms.Gift > 0)
            {
                int given = Math.Min(terms.Gift, me.Gold);
                if (given > 0) GiveGoldAction.ApplyBetweenCharacters(me, ruler, given);
                lines.Add(Tr.Dative(ruler.Name.ToString()) + " " + SchemeFactors.Money(given) + " verdin.");
            }
            if (terms.ReleaseOurs)
            {
                List<Hero> freed = OurLordsHeld(enemy);
                foreach (Hero lord in freed) EndCaptivityAction.ApplyByPeace(lord, ruler);
                if (freed.Count > 0) lines.Add("Esir lordların serbest: " + string.Join(", ", freed.Select(h => h.Name)) + ".");
            }
            if (terms.ReleaseTheirs)
            {
                List<Hero> freed = TheirLordsHeld(enemy);
                foreach (Hero lord in freed) EndCaptivityAction.ApplyByPeace(lord, me);
                if (freed.Count > 0) lines.Add("Esirlerini bıraktın: " + string.Join(", ", freed.Select(h => h.Name)) + ".");
            }
            // Rehineyle birlikteyse tek anlaşma kurulur, rehinenin süresinden kısa olmaz (rehine anlaşması ayrıca kurmaz, bitirmez de).
            int pactDays = terms.PactDays > 0 && terms.Hostage ? Math.Max(terms.PactDays, HostageRules.DurationDays) : terms.PactDays;
            if (pactDays > 0)
                lines.Add(DiplomacyCompat.FormNonAggressionPact(mine, enemy, pactDays)
                    ? pactDays + " günlük saldırmazlık anlaşması." : "Saldırmazlık anlaşması kurulamadı (zaten var ya da Diplomacy izin vermedi).");
            if (terms.Hostage) lines.Add(Hostages.TakeForTreaty(state, enemy, hostage));
            if (terms.Trade) lines.Add(MakeTrade(mine, enemy));
            if (terms.Alliance) lines.Add(MakeAlliance(mine, enemy));
            if (!theirOffer && weight >= 40) SchemeOutcomes.Relate(ruler, -Math.Min(10, weight / 10));

            PeaceTalk talk = state.PeaceTalkWith(enemy.StringId);
            talk.RefusedUntil = 0;
            talk.AskedUntil = 0;
            talk.OfferGold = 0;
            state.AddLog(Today, enemy.Name + " ile barış: " + summary);
            return string.Join("\n", lines);
        }

        /// <summary>Barıştan sonra oyunun kendi ticaret anlaşması (oyun izin verirse); sonuç satırı.</summary>
        private static string MakeTrade(Kingdom mine, Kingdom enemy)
        {
            var model = Campaign.Current.Models.TradeAgreementModel;
            var trade = Campaign.Current.GetCampaignBehavior<TaleWorlds.CampaignSystem.CampaignBehaviors.ITradeAgreementsCampaignBehavior>();
            if (trade == null) return "Ticaret anlaşması kurulamadı (oyunun sistemi yok).";
            if (!model.CanMakeTradeAgreement(mine, enemy, false, out TaleWorlds.Localization.TextObject reason, true))
                return "Ticaret anlaşması kurulamadı: " + reason + ".";
            trade.MakeTradeAgreement(mine, enemy, model.GetTradeAgreementDurationInYears(mine, enemy));
            return "Ticaret anlaşması imzalandı: kervanlar iki krallıkta da kazanç getirir.";
        }

        /// <summary>Barıştan sonra oyunun kendi ittifakı (oyun izin verirse); sonuç satırı.</summary>
        private static string MakeAlliance(Kingdom mine, Kingdom enemy)
        {
            var model = Campaign.Current.Models.AllianceModel;
            var alliance = Campaign.Current.GetCampaignBehavior<TaleWorlds.CampaignSystem.CampaignBehaviors.IAllianceCampaignBehavior>();
            if (alliance == null) return "İttifak kurulamadı (oyunun sistemi yok).";
            if (!model.CanMakeAlliance(mine, enemy, mine, out TaleWorlds.Localization.TextObject reason, true))
                return "İttifak kurulamadı: " + reason + ".";
            alliance.StartAlliance(mine, enemy);
            return "İttifak kuruldu: birbirinizi savaşa çağırabilirsiniz.";
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
                ShowEnvoy(state, enemy, talk);
                return;
            }
        }

        /// <summary>Elçinin getirdiği teklif: kabul edeceklerinden emin oldukları kadar savaş tazminatı.</summary>
        public static Terms EnvoyOffer(IntrigueState state, Kingdom enemy) =>
            new Terms { Gold = PeaceTreatyRules.EnvoyGold(LeverageOf(state, enemy).Willingness, Treasury(enemy)) };

        private static void ShowEnvoy(IntrigueState state, Kingdom enemy, PeaceTalk talk)
        {
            Terms offer = EnvoyOffer(state, enemy);
            talk.OfferGold = offer.Gold;
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
                        string result = Apply(state, enemy, offer, true);
                        SchemeOutcomes.Notify(state, "Barış: " + enemy.Name, result, enemy.Leader, CardImages.Treaty);
                    }),
                },
                new UI.EventCards.CardOption
                {
                    Label = "Barış Masası'na otur: daha fazlasını iste",
                    Apply = () => UI.IntrigueScreen.OpenWar(enemy, offer),
                },
                refuse,
            };
            UI.EventCards.Show("BARIŞ ELÇİSİ", enemy.Name + " barış istiyor", text, CardImages.Envoy, enemy.Leader, options, refuse, null);
        }
    }
}
