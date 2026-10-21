using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Bundle;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.Localization;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Krallığının öteki krallıklarla bağları (Savaş ve barış > Krallıklar; tasarım: docs/superpowers/specs/2026-10-21-kralliklar-sekmesi.md):
    /// durum, kalan süreler, karşı tarafın istekleri ve hükümdarın doğrudan işleri. Savaş ilanı ve saldırmazlık Diplomacy'nin (koşulları ve
    /// bedeli onun), ittifak, ticaret ve savaşa çağırma oyunun kendi sistemi (ittifak bedeli Diplomacy'nin hesabıyla). Divansız: Diplomacy'nin
    /// "divanı toplamadan" düğmeleri gibi yalnızca hükümdar yapar; öteki üyeler oyunun Krallık ekranından divana sunar.
    /// </summary>
    public static class KingdomTies
    {
        public static Kingdom Mine => Clan.PlayerClan.Kingdom;
        public static bool IsRuler => Mine != null && Mine.Leader == Hero.MainHero;

        private static IAllianceCampaignBehavior Alliances => Campaign.Current.GetCampaignBehavior<IAllianceCampaignBehavior>();
        private static ITradeAgreementsCampaignBehavior Trades => Campaign.Current.GetCampaignBehavior<ITradeAgreementsCampaignBehavior>();

        public static bool AtWar(Kingdom kingdom) => Clan.PlayerClan.MapFaction != null && FactionManager.IsAtWarAgainstFaction(Clan.PlayerClan.MapFaction, kingdom);

        public static bool Allied(Kingdom kingdom) => Mine != null && Mine.IsAllyWith(kingdom);

        /// <summary>Saldırmazlık anlaşmasının kalan günü; yoksa -1.</summary>
        public static float PactDays(Kingdom kingdom) => Mine == null ? -1f : DiplomacyCompat.PactDaysLeft(Mine, kingdom);

        public static bool HasTrade(Kingdom kingdom) => Mine != null && Trades != null && Trades.HasTradeAgreement(Mine, kingdom, out _);

        public static float TradeDays(Kingdom kingdom) => HasTrade(kingdom) ? Math.Max(0f, Trades.GetTradeAgreementEndDate(Mine, kingdom).RemainingDaysFromNow) : -1f;

        public static float AllianceDays(Kingdom kingdom) =>
            Allied(kingdom) && Alliances != null ? Math.Max(0f, Alliances.GetAllianceEndDate(Mine, kingdom).RemainingDaysFromNow) : -1f;

        public static int Order(Kingdom kingdom) => KingdomsRules.Order(AtWar(kingdom), Allied(kingdom), PactDays(kingdom) >= 0, HasTrade(kingdom));

        public static string StanceText(Kingdom kingdom) => KingdomsRules.StanceText(AtWar(kingdom), Allied(kingdom), PactDays(kingdom) >= 0, HasTrade(kingdom));

        /// <summary>Bu krallığın sana katılmış müttefik olarak kime karşı savaştığı (savaşa çağrı anlaşması); yoksa null.</summary>
        public static List<Kingdom> CalledAgainst(Kingdom ally) =>
            Mine == null || Alliances == null ? new List<Kingdom>()
                : Kingdom.All.Where(e => !e.IsEliminated && e != Mine && e != ally && Alliances.IsAtWarByCallToWarAgreement(ally, e, out Kingdom caller) && caller == Mine).ToList();

        // ---- Karşı tarafın gözüyle ----

        /// <summary>Oyunun (ve Diplomacy'nin) ittifak puanı: null istekli; değilse nedeni (tek satır).</summary>
        public static string AllianceRefusal(Kingdom kingdom)
        {
            if (Mine == null) return "Bir krallığın yok";
            float score = Campaign.Current.Models.AllianceModel.GetScoreOfStartingAlliance(kingdom, Mine, out TextObject why, true).ResultNumber;
            return score > 0 ? null : Or(why, kingdom.Name + " şu an seninle ittifak düşünmüyor");
        }

        /// <summary>Oyunun ticaret anlaşması puanı: null istekli; değilse nedeni.</summary>
        public static string TradeRefusal(Kingdom kingdom)
        {
            if (Mine == null || kingdom.RulingClan == null) return "Bir krallığın yok";
            float score = Campaign.Current.Models.TradeAgreementModel.GetScoreOfStartingTradeAgreement(kingdom, Mine, kingdom.RulingClan, out TextObject why, true);
            return score > 0 ? null : Or(why, kingdom.Name + " şu an ticaret anlaşması istemiyor");
        }

        private static string Or(TextObject reason, string fallback)
        {
            string text = KingdomsRules.Compact(reason?.ToString());
            return text.Length > 0 ? text : fallback;
        }

        // ---- Hükümdarın işleri (null: yapılabilir; değilse neden) ----

        private static string RulerBlocked() =>
            Mine == null ? "Bir krallığın yok"
            : !IsRuler ? "Yalnızca hükümdar yapar (sen oyunun Krallık ekranından divana sunabilirsin)"
            : null;

        private static string Afford(int influence, int gold) =>
            influence > 0 && Clan.PlayerClan.Influence < influence ? "Nüfuzun yetmez (" + influence + " gerekli)"
            : gold > 0 && Hero.MainHero.Gold < gold ? "Kesen yetmez (" + SchemeFactors.Money(gold) + " gerekli)"
            : null;

        public static string CostText(int influence, int gold)
        {
            var parts = new List<string>();
            if (influence > 0) parts.Add(influence + " nüfuz");
            if (gold > 0) parts.Add(SchemeFactors.Money(gold));
            return parts.Count == 0 ? "bedelsiz" : string.Join(", ", parts);
        }

        internal static DiplomacyCompat.Cost WarCost() => Mine == null ? null : DiplomacyCompat.DeclareWarCost(Mine);

        public static string WarBlocked(Kingdom kingdom) =>
            RulerBlocked() ?? (AtWar(kingdom) ? "Zaten savaştasınız" : !DiplomacyCompat.IsAvailable ? "Diplomacy paketli değil" : DiplomacyCompat.DeclareWarBlocked(Mine, kingdom));

        public static void DeclareWar(Kingdom kingdom) => DiplomacyCompat.DeclareWar(Mine, kingdom);

        internal static DiplomacyCompat.Cost PactCost(Kingdom kingdom) => Mine == null ? null : DiplomacyCompat.PactCost(Mine, kingdom);

        public static string PactBlocked(Kingdom kingdom) =>
            RulerBlocked() ?? (PactDays(kingdom) >= 0 ? "Zaten saldırmazlık anlaşmanız var" : !DiplomacyCompat.IsAvailable ? "Diplomacy paketli değil"
                : DiplomacyCompat.PactBlocked(Mine, kingdom));

        public static void ProposePact(Kingdom kingdom) => DiplomacyCompat.ProposePact(Mine, kingdom);

        /// <summary>İttifak bedeli: Diplomacy'nin hesabı (nüfuz ve altın); okunamazsa oyunun önerme bedeli (nüfuz).</summary>
        public static (int Influence, int Gold) AllianceCost(Kingdom kingdom)
        {
            DiplomacyCompat.Cost cost = Mine == null ? null : DiplomacyCompat.AllianceCost(Mine, kingdom);
            return cost != null ? (cost.Influence, cost.Gold)
                : (Campaign.Current.Models.AllianceModel.GetInfluenceCostOfProposingStartingAlliance(Clan.PlayerClan), 0);
        }

        public static string AllianceBlocked(Kingdom kingdom)
        {
            string blocked = RulerBlocked() ?? (Allied(kingdom) ? "Zaten müttefiksiniz" : AtWar(kingdom) ? "Savaştasınız: önce barış" : Alliances == null ? "Oyunun ittifak sistemi yok" : null);
            if (blocked != null) return blocked;
            if (!Campaign.Current.Models.AllianceModel.CanMakeAlliance(Mine, kingdom, Mine, out TextObject reason, true))
                return Or(reason, "İttifak kurulamaz");
            var (influence, gold) = AllianceCost(kingdom);
            return AllianceRefusal(kingdom) ?? Afford(influence, gold);
        }

        public static void MakeAlliance(Kingdom kingdom)
        {
            DiplomacyCompat.Cost cost = DiplomacyCompat.AllianceCost(Mine, kingdom);
            if (cost != null) cost.Pay();
            else ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -AllianceCost(kingdom).Influence);
            Alliances.StartAlliance(Mine, kingdom);
        }

        public static string BreakAllianceBlocked(Kingdom kingdom) => RulerBlocked() ?? (Allied(kingdom) ? null : "Müttefik değilsiniz");

        public static void BreakAlliance(Kingdom kingdom) => Alliances.EndAlliance(Mine, kingdom);

        public static int TradeCost() => Campaign.Current.Models.TradeAgreementModel.GetInfluenceCostOfProposingTradeAgreement(Clan.PlayerClan);

        public static string TradeBlocked(Kingdom kingdom)
        {
            string blocked = RulerBlocked() ?? (HasTrade(kingdom) ? "Zaten ticaret anlaşmanız var" : AtWar(kingdom) ? "Savaştasınız: önce barış"
                : Trades == null ? "Oyunun ticaret sistemi yok" : null);
            if (blocked != null) return blocked;
            if (!Campaign.Current.Models.TradeAgreementModel.CanMakeTradeAgreement(Mine, kingdom, true, out TextObject reason, true))
                return Or(reason, "Ticaret anlaşması kurulamaz");
            return Afford(TradeCost(), 0);
        }

        public static void MakeTrade(Kingdom kingdom)
        {
            ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -TradeCost());
            Trades.MakeTradeAgreement(Mine, kingdom, Campaign.Current.Models.TradeAgreementModel.GetTradeAgreementDurationInYears(Mine, kingdom));
        }

        /// <summary>Müttefiki çağırabileceğin savaşlar (oyunun listesi: onun henüz savaşmadığı düşmanların).</summary>
        public static List<Kingdom> CallTargets(Kingdom ally) =>
            Mine == null || Alliances == null || !Allied(ally) ? new List<Kingdom>() : Alliances.GetKingdomsToCallToWarAgainst(Mine, ally) ?? new List<Kingdom>();

        public static int CallInfluence() => Campaign.Current.Models.AllianceModel.GetInfluenceCostOfCallingToWar(Clan.PlayerClan);

        public static int CallGold(Kingdom ally, Kingdom enemy) => Campaign.Current.Models.AllianceModel.GetCallToWarCost(Mine, ally, enemy);

        public static string CallBlocked(Kingdom ally, Kingdom enemy)
        {
            string blocked = RulerBlocked();
            if (blocked != null) return blocked;
            float score = Campaign.Current.Models.AllianceModel.GetScoreOfCallingToWar(Mine, ally, enemy, ally, out TextObject reason);
            if (score <= 0) return Or(reason, ally.Name + " bu savaşa girmek istemiyor");
            return Afford(CallInfluence(), CallGold(ally, enemy));
        }

        /// <summary>Müttefiki savaşa çağırır: nüfuz bizden, savaş bedeli oyunun kuralıyla (oyuncu öder).</summary>
        public static void CallToWar(Kingdom ally, Kingdom enemy)
        {
            int gold = CallGold(ally, enemy);
            ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -CallInfluence());
            Alliances.StartCallToWarAgreement(Mine, ally, enemy, gold, true);
        }
    }
}
