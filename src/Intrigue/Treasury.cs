using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Extensions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.Settlements.Workshops;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Divan: hazinedar (P2.12, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-hazinedar.md). Klanından birini hazinedar yaparsın:
    /// atölyelerin ve kervanların gelirini (oyunun kendi hesabı), kervanların sefer defterini, atölyeler için üretim önerisini, ticaret
    /// fırsatlarını ve kervan kurmaya uygun şehirleri gösterir. Fiyatları hazinedarın gözüyle görürsün: Trade'i düşükse hata payı büyür
    /// ve önerileri yanlış olabilir. Onuru düşükse zimmete para geçirebilir; casus başın fark edebilir.
    /// </summary>
    public static class Treasury
    {
        private static int Today => SchemeFactors.Today;
        private static int Week => Today / 7;

        // ---- Koltuk ----

        public static Hero Treasurer(IntrigueState state)
        {
            if (state.TreasurerId == null) return null;
            Hero hero = Hero.Find(state.TreasurerId);
            return hero != null && hero.IsAlive && hero.Clan == Clan.PlayerClan ? hero : null;
        }

        public static IEnumerable<Hero> Candidates(IntrigueState state) => Council.Candidates(state, "hazinedarın");

        public static int Trade(Hero hero) => hero?.GetSkillValue(DefaultSkills.Trade) ?? 0;

        public static int Error(Hero hero) =>
            hero == null ? 60 : TreasuryRules.ErrorPercent(Trade(hero), hero.GetTraitLevel(DefaultTraits.Calculating));

        public static string Label(Hero hero)
        {
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            return hero.Name + " - Trade " + Trade(hero) + ", Hesapçı " + SchemeFactors.Signed(hero.GetTraitLevel(DefaultTraits.Calculating))
                + ", tahmin payı %" + Error(hero) + (busy != null ? "  [" + busy + "]" : "");
        }

        /// <summary>null: hazinedar çalışıyor; değilse neden (yok, esir, vali, parti lideri).</summary>
        public static string Blocked(IntrigueState state)
        {
            Hero treasurer = Treasurer(state);
            if (treasurer == null) return "Hazinedarın yok";
            string busy = SchemeFactors.SpymasterBusyReason(treasurer);
            return busy != null ? treasurer.Name + " şu an " + busy + "; hazineye bakamıyor" : null;
        }

        public static void Assign(IntrigueState state, Hero hero)
        {
            Council.EnsureFree(state, hero, "hazinedarın");
            if (state.EmbezzleFound && state.TreasurerId != hero.StringId) throw new InvalidOperationException("Önce zimmetin hesabını sor");
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            if (busy != null) throw new InvalidOperationException(hero.Name + " şu an " + busy + "; hazinedar olamaz");
            if (state.TreasurerId != hero.StringId)
            {
                state.Embezzled = 0;
                state.EmbezzleFound = false;
            }
            state.TreasurerId = hero.StringId;
            _board = null;
            state.AddLog(Today, hero.Name + " hazinedar oldu");
            MenuUI.Success(hero.Name + " artık hazinedarın");
        }

        public static void Dismiss(IntrigueState state)
        {
            if (state.TreasurerId == null) throw new InvalidOperationException("Hazinedarın yok");
            if (state.EmbezzleFound) throw new InvalidOperationException("Önce zimmetin hesabını sor");
            Hero old = Treasurer(state);
            state.TreasurerId = null;
            state.Embezzled = 0;
            _board = null;
            state.AddLog(Today, (old?.Name.ToString() ?? "Hazinedar") + " görevden alındı");
            MenuUI.Success("Hazinedar görevden alındı");
        }

        // ---- Gelir (oyunun kendi hesabı: kesin) ----

        public static List<Workshop> Workshops() =>
            Hero.MainHero.OwnedWorkshops.Where(w => w?.Settlement?.Town != null && w.WorkshopType != null).ToList();

        public static List<MobileParty> Caravans() =>
            Clan.PlayerClan.Heroes.Where(h => h.IsAlive).SelectMany(h => h.OwnedCaravans)
                .Select(c => c?.MobileParty).Where(p => p != null && p.IsActive).Distinct().ToList();

        public static int WorkshopIncome(Workshop workshop) => Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromWorkshop(workshop);

        public static int CaravanIncome(MobileParty caravan) => Campaign.Current.Models.ClanFinanceModel.CalculateOwnerIncomeFromCaravan(caravan);

        /// <summary>Atölyelerin ve kervanların sana günlük getirisi.</summary>
        public static int DailyIncome() => Workshops().Sum(WorkshopIncome) + Caravans().Sum(CaravanIncome);

        // ---- Hazinedarın fiyat tahtası ----

        /// <summary>Hazinedarın bildiği şehirlerdeki fiyatlar, onun hata payıyla. Günde bir kurulur.</summary>
        public sealed class Board
        {
            public string TreasurerId;
            public int Day;
            public int Error;
            public List<Settlement> Towns = new List<Settlement>();
            public List<ItemCategory> TradeGoods = new List<ItemCategory>();
            public Dictionary<ItemCategory, ItemObject> Samples = new Dictionary<ItemCategory, ItemObject>();
            private readonly Dictionary<string, (double buy, double sell)> _prices = new Dictionary<string, (double buy, double sell)>();
            internal int Seed;
            internal int WeekNo;

            /// <summary>Hazinedarın gözüyle o şehirde bir malın alış ve satış fiyatı (bilmediği şehir için de tahmin eder).</summary>
            public (double buy, double sell) Price(Settlement town, ItemCategory category)
            {
                string key = town.StringId + "|" + category.StringId;
                if (_prices.TryGetValue(key, out (double buy, double sell) price)) return price;
                price = (0, 0);
                if (Samples.TryGetValue(category, out ItemObject item) && town.Town != null)
                {
                    int roll = TreasuryRules.WeeklyRoll(Seed, WeekNo, TreasurerId + "|" + key);
                    price = (TreasuryRules.Noisy(TruePrice(town.Town, item, false), Error, roll),
                        TreasuryRules.Noisy(TruePrice(town.Town, item, true), Error, roll));
                }
                _prices[key] = price;
                return price;
            }
        }

        private static Board _board;

        /// <summary>Kampanya yüklenince: önceki kampanyanın fiyat tahtası kullanılmasın.</summary>
        public static void ResetCache() => _board = null;

        public static Board CurrentBoard(IntrigueState state)
        {
            Hero treasurer = Treasurer(state);
            if (treasurer == null) return null;
            if (_board != null && _board.Day == Today && _board.TreasurerId == treasurer.StringId) return _board;
            var board = new Board { TreasurerId = treasurer.StringId, Day = Today, Error = Error(treasurer), Seed = state.Seed, WeekNo = Week };
            foreach (IGrouping<ItemCategory, ItemObject> group in Items.All.Where(i => i.ItemCategory != null && !i.NotMerchandise).GroupBy(i => i.ItemCategory))
                board.Samples[group.Key] = group.OrderBy(i => i.Value).First();
            board.TradeGoods = board.Samples.Keys.Where(c => c.IsTradeGood && c.IsValid && !c.IsAnimal)
                .OrderBy(c => c.StringId, StringComparer.Ordinal).ToList();
            Vec2 here = MobileParty.MainParty.GetPosition2D;
            board.Towns = Town.AllTowns.Select(t => t.Settlement).Where(s => s != null && s.IsTown && !s.IsUnderSiege)
                .OrderBy(s => s.GetPosition2D.Distance(here)).Take(TreasuryRules.KnownTowns(Trade(treasurer))).ToList();
            _board = board;
            return board;
        }

        private static double TruePrice(Town town, ItemObject item, bool selling)
        {
            try
            {
                return town.GetItemPrice(item, null, selling);
            }
            catch (Exception)
            {
                return town.GetItemPrice(item, MobileParty.MainParty, selling);
            }
        }

        public static string ItemName(Board board, ItemCategory category) =>
            board.Samples.TryGetValue(category, out ItemObject item) ? item.Name.ToString() : category.StringId;

        // ---- Atölye önerisi ----

        public sealed class WorkshopAdvice
        {
            public Workshop Workshop;
            public int Income;
            public double Current;
            /// <summary>Diğer türler, hazinedarın tahminiyle en kârlıdan başlayarak.</summary>
            public List<(WorkshopType type, double estimate)> Options = new List<(WorkshopType type, double estimate)>();
            public bool Worth => Options.Count > 0 && TreasuryRules.WorthSwitching(Current, Options[0].estimate);
        }

        public static WorkshopAdvice Advise(Board board, Workshop workshop)
        {
            var advice = new WorkshopAdvice { Workshop = workshop, Income = WorkshopIncome(workshop), Current = Estimate(board, workshop, workshop.WorkshopType) };
            advice.Options = WorkshopType.All.Where(t => !t.IsHidden && t != workshop.WorkshopType && t.Productions.Count > 0)
                .Select(t => (t, Estimate(board, workshop, t))).OrderByDescending(o => o.Item2).ToList();
            return advice;
        }

        /// <summary>Hazinedarın tahmini: o şehirdeki girdi ve çıktı fiyatlarıyla bir günlük üretimin kârı (atölye gideri hariç).</summary>
        public static double Estimate(Board board, Workshop workshop, WorkshopType type)
        {
            Settlement town = workshop.Settlement;
            return TreasuryRules.WorkshopMargin(type.Productions.Select(p => (
                p.Inputs.Sum(i => board.Price(town, i.Item1).buy * i.Item2),
                p.Outputs.Sum(o => board.Price(town, o.Item1).sell * o.Item2),
                (double)Speed(workshop, p.ConversionSpeed))));
        }

        private static float Speed(Workshop workshop, float speed)
        {
            try
            {
                return Campaign.Current.Models.WorkshopModel.GetEffectiveConversionSpeedOfProduction(workshop, speed, false).ResultNumber;
            }
            catch (Exception)
            {
                return speed;
            }
        }

        public static int ChangeCost(WorkshopType type) =>
            Campaign.Current.Models.WorkshopModel.GetConvertProductionCost(type) + TreasuryRules.RemoteOrderFee;

        /// <summary>Hazinedar ustabaşına mektup yazar: üretim oyunun kendi işlemiyle değişir (oyunun bedeli + ulak ücreti).</summary>
        public static void ChangeProduction(IntrigueState state, Workshop workshop, WorkshopType type)
        {
            string blocked = Blocked(state);
            if (blocked != null) throw new InvalidOperationException(blocked);
            if (workshop.Owner != Hero.MainHero) throw new InvalidOperationException("Bu atölye artık senin değil");
            int cost = ChangeCost(type);
            SchemeFactors.Pay(cost);
            ChangeProductionTypeOfWorkshopAction.Apply(workshop, type, true);
            _board = null;
            state.AddLog(Today, Tr.Locative(workshop.Settlement.Name.ToString()) + " atölye uzaktan değişti: " + type.Name);
            MenuUI.Success(Tr.Locative(workshop.Settlement.Name.ToString()) + " atölyen artık " + type.Name + " (" + SchemeFactors.Money(cost) + ")");
        }

        // ---- Ticaret fırsatları ve kervan önerisi ----

        public sealed class Opportunity
        {
            public ItemCategory Category;
            public Settlement Buy;
            public Settlement Sell;
            public double BuyPrice;
            public double SellPrice;
            public int Spread;
        }

        /// <summary>Her mal için hazinedarın bildiği en iyi al-sat şehir çifti (kervan menzilinde), en büyük farktan başlayarak.</summary>
        public static List<Opportunity> Opportunities(Board board, int count)
        {
            var result = new List<Opportunity>();
            foreach (ItemCategory category in board.TradeGoods)
            {
                Opportunity best = null;
                foreach (Settlement buy in board.Towns)
                {
                    double buyPrice = board.Price(buy, category).buy;
                    if (buyPrice <= 0) continue;
                    foreach (Settlement sell in board.Towns)
                    {
                        if (sell == buy || buy.GetPosition2D.Distance(sell.GetPosition2D) > TreasuryRules.TradeRange) continue;
                        double sellPrice = board.Price(sell, category).sell;
                        int spread = TreasuryRules.SpreadPercent(buyPrice, sellPrice);
                        if (best == null || spread > best.Spread)
                            best = new Opportunity { Category = category, Buy = buy, Sell = sell, BuyPrice = buyPrice, SellPrice = sellPrice, Spread = spread };
                    }
                }
                if (best != null && best.Spread >= TreasuryRules.MinSpreadPercent) result.Add(best);
            }
            return result.OrderByDescending(o => o.Spread).Take(count).ToList();
        }

        /// <summary>Kervan kurmaya uygun şehirler: oradan alınıp yakın şehirlerde satılacak en iyi üç malın ortalama farkı.</summary>
        public static List<(Settlement town, int spread, List<string> goods)> CaravanHomes(Board board, int count)
        {
            var result = new List<(Settlement town, int spread, List<string> goods)>();
            foreach (Settlement home in board.Towns)
            {
                var best = new List<(int spread, string good)>();
                foreach (ItemCategory category in board.TradeGoods)
                {
                    double buyPrice = board.Price(home, category).buy;
                    if (buyPrice <= 0) continue;
                    int spread = board.Towns.Where(t => t != home && home.GetPosition2D.Distance(t.GetPosition2D) <= TreasuryRules.TradeRange)
                        .Select(t => TreasuryRules.SpreadPercent(buyPrice, board.Price(t, category).sell)).DefaultIfEmpty(0).Max();
                    best.Add((spread, ItemName(board, category)));
                }
                List<(int spread, string good)> top = best.OrderByDescending(b => b.spread).Take(3).ToList();
                if (top.Count == 0 || top.Average(b => b.spread) < TreasuryRules.MinSpreadPercent) continue;
                result.Add((home, (int)top.Average(b => b.spread), top.Select(b => b.good).ToList()));
            }
            return result.OrderByDescending(r => r.spread).Take(count).ToList();
        }

        // ---- Kervan defteri ----

        public static bool IsOurs(MobileParty party) =>
            party != null && party.IsCaravan && party.PartyComponent is CaravanPartyComponent caravan && caravan.Owner?.Clan == Clan.PlayerClan;

        /// <summary>Kesedeki altın + yükün oyundaki temel değeri.</summary>
        public static int Worth(MobileParty party)
        {
            int worth = party.PartyTradeGold;
            ItemRoster items = party.ItemRoster;
            for (int i = 0; i < items.Count; i++)
            {
                ItemObject item = items.GetItemAtIndex(i);
                if (item != null) worth += item.Value * items.GetElementNumber(i);
            }
            return worth;
        }

        /// <summary>Kervan bir şehirden ayrılınca: önceki çıkıştan bu yana sefer deftere yazılır.</summary>
        public static void OnSettlementLeft(MobileParty party, Settlement settlement)
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || settlement?.Town == null || !settlement.IsTown || !IsOurs(party)) return;
            int worth = Worth(party);
            CaravanBook book = state.CaravanBooks.FirstOrDefault(b => b.PartyId == party.StringId);
            if (book != null && book.TownId != settlement.StringId)
                TreasuryRules.AddLeg(state.CaravanLegs, new CaravanLeg
                {
                    PartyId = party.StringId, FromId = book.TownId, ToId = settlement.StringId,
                    Profit = TreasuryRules.LegProfit(book.Worth, worth, book.SentHome), Days = Today - book.Day, Day = Today,
                });
            if (book == null) state.CaravanBooks.Add(book = new CaravanBook { PartyId = party.StringId });
            book.TownId = settlement.StringId;
            book.Worth = worth;
            book.Day = Today;
            book.SentHome = 0;
        }

        /// <summary>Kervanın sana gönderdiği pay sefer kârına eklenir (oyun bu payı kervanın kesesinden alır).</summary>
        public static void DailyTick()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.CaravanBooks.Count == 0) return;
            foreach (MobileParty caravan in Caravans())
            {
                CaravanBook book = state.CaravanBooks.FirstOrDefault(b => b.PartyId == caravan.StringId);
                if (book != null) book.SentHome += Math.Max(0, CaravanIncome(caravan));
            }
        }

        public static List<CaravanLeg> LegsOf(IntrigueState state, MobileParty caravan) =>
            state.CaravanLegs.Where(l => l.PartyId == caravan.StringId).ToList();

        public static string TownName(string id) => Settlement.Find(id)?.Name.ToString() ?? id;

        // ---- Haftalık: zimmet ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            HashSet<string> alive = new HashSet<string>(Caravans().Select(p => p.StringId));
            state.CaravanBooks.RemoveAll(b => !alive.Contains(b.PartyId));

            Hero treasurer = Treasurer(state);
            if (treasurer == null)
            {
                if (state.TreasurerId != null)
                {
                    Hero lost = Hero.Find(state.TreasurerId);
                    state.AddLog(Today, "Hazinedar koltuğu boşaldı" + (lost != null ? ": " + lost.Name + (lost.IsAlive ? " artık klanında değil" : " öldü") : ""));
                    state.TreasurerId = null;
                    state.Embezzled = 0;
                    state.EmbezzleFound = false;
                }
                return;
            }
            if (state.EmbezzleFound || Blocked(state) != null) return;

            int weekly = DailyIncome() * 7;
            if (state.Roll100() <= TreasuryRules.EmbezzleChance(treasurer.GetTraitLevel(DefaultTraits.Honor)))
            {
                int amount = Math.Min(TreasuryRules.EmbezzleAmount(weekly, state.Roll100()), Hero.MainHero.Gold);
                if (amount > 0)
                {
                    Hero.MainHero.ChangeHeroGold(-amount);
                    treasurer.ChangeHeroGold(amount);
                    state.Embezzled += amount;
                }
            }
            if (state.Embezzled <= 0) return;
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (spymaster == null || SchemeFactors.SpymasterBusyReason(spymaster) != null) return;
            int chance = TreasuryRules.EmbezzleDetectChance(SchemeFactors.Roguery(spymaster), state.Mission == SpymasterMission.CounterIntel);
            if (state.Roll100() > chance) return;
            state.EmbezzleFound = true;
            SchemeOutcomes.Notify(state, "Hazinede açık: " + treasurer.Name,
                spymaster.Name + " hesap defterlerinde tutmayan rakamlar buldu: " + treasurer.Name + " gelirlerinden "
                + SchemeFactors.Money(state.Embezzled) + " zimmetine geçirmiş.\n\nEntrika ekranı > Divan > \"Hesap sor\" ile karar ver.",
                treasurer, CardImages.Embezzle);
            state.AddLog(Today, "Hazinedarın zimmeti ortaya çıktı: " + treasurer.Name + ", " + SchemeFactors.Money(state.Embezzled));
        }

        public enum Reckoning
        {
            /// <summary>Parayı geri al, görevden al.</summary>
            Dismiss = 1,
            /// <summary>Parayı geri al, görevde kalsın.</summary>
            Keep = 2,
            /// <summary>Affet: para onda kalır.</summary>
            Forgive = 3,
        }

        public static void Reckon(IntrigueState state, Reckoning choice)
        {
            Hero treasurer = Treasurer(state) ?? throw new InvalidOperationException("Hazinedarın yok");
            if (!state.EmbezzleFound) throw new InvalidOperationException("Ortada bir zimmet yok");
            int back = choice == Reckoning.Forgive ? 0 : Math.Min(state.Embezzled, treasurer.Gold);
            if (back > 0)
            {
                treasurer.ChangeHeroGold(-back);
                Hero.MainHero.ChangeHeroGold(back);
            }
            string text;
            switch (choice)
            {
                case Reckoning.Dismiss:
                    SchemeOutcomes.Relate(treasurer, -TreasuryRules.EmbezzleRefundRelation);
                    text = treasurer.Name + " görevden alındı; " + SchemeFactors.Money(back) + " geri alındı. İlişki -" + TreasuryRules.EmbezzleRefundRelation + ".";
                    state.TreasurerId = null;
                    break;
                case Reckoning.Keep:
                    SchemeOutcomes.Relate(treasurer, -TreasuryRules.EmbezzleKeepRelation);
                    text = SchemeFactors.Money(back) + " geri alındı; " + treasurer.Name + " görevde kaldı ama artık gözün üstünde. İlişki -"
                        + TreasuryRules.EmbezzleKeepRelation + ".";
                    break;
                default:
                    SchemeOutcomes.Relate(treasurer, TreasuryRules.EmbezzleForgiveRelation);
                    text = treasurer.Name + " affedildi; para onda kaldı. İlişki +" + TreasuryRules.EmbezzleForgiveRelation + ".";
                    break;
            }
            state.Embezzled = 0;
            state.EmbezzleFound = false;
            _board = null;
            state.AddLog(Today, "Zimmetin hesabı soruldu: " + text);
            MenuUI.Success(text);
        }

        // ---- Düz metin rapor (liste menüsü ve kart için) ----

        public static string ReportText(IntrigueState state)
        {
            string blocked = Blocked(state);
            if (blocked != null) return blocked + ".";
            Board board = CurrentBoard(state);
            var lines = new List<string>
            {
                "Tahmin payı: %" + board.Error + " (" + TreasuryRules.Confidence(board.Error) + "), " + board.Towns.Count + " şehrin fiyatlarını biliyor.",
                "Günlük gelir (oyunun hesabı): atölyeler " + Signed(Workshops().Sum(WorkshopIncome)) + ", kervanlar " + Signed(Caravans().Sum(CaravanIncome)) + ".",
            };
            foreach (Workshop workshop in Workshops())
            {
                WorkshopAdvice advice = Advise(board, workshop);
                lines.Add("· " + workshop.Settlement.Name + " " + workshop.WorkshopType.Name + ": günde " + Signed(advice.Income)
                    + (advice.Worth ? "; öneri: " + advice.Options[0].type.Name + " (tahmini " + Signed((int)advice.Options[0].estimate) + ")" : "; böyle iyi"));
            }
            List<Opportunity> opportunities = Opportunities(board, 5);
            if (opportunities.Count > 0) lines.Add("Fırsatlar:");
            foreach (Opportunity o in opportunities) lines.Add("· " + OpportunityText(board, o));
            return string.Join("\n", lines);
        }

        public static string OpportunityText(Board board, Opportunity o) =>
            ItemName(board, o.Category) + ": " + Tr.Locative(o.Buy.Name.ToString()) + " al ~" + (int)o.BuyPrice + ", "
            + Tr.Locative(o.Sell.Name.ToString()) + " sat ~" + (int)o.SellPrice + " (+%" + o.Spread + ")";

        public static string Signed(int value) => value > 0 ? "+" + value : value.ToString();
    }
}
