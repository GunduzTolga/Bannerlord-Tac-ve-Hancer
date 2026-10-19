using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Ağ ifşasından diplomatik kriz (P2.5, tasarım: docs/superpowers/specs/2026-10-19-p2-diplomatik-kriz.md). Yabancı bir krallıkta
    /// çökertilen gölge ağı o krallıkta oyunun kendi suç puanını yükseltir ve hükümdar hesap sorar (tazminat, inkâr, ret). Gerilim
    /// birikirse hükümdar savaş meclisini toplar: savaşı oyunun kendi krallık kararı (lordların oyu) belirler.
    /// </summary>
    public static class DiplomaticCrises
    {
        /// <summary>
        /// Ağ çökertildiğinde çağrılır (ağ listeden çıkmış olmalı). Kriz kartı açıldıysa true (ayrıca bildirim gerekmez).
        /// </summary>
        public static bool OnNetworkExposed(IntrigueState state, Settlement town, int level, Hero penalizedOwner, out string ownKingdomLine)
        {
            ownKingdomLine = null;
            Kingdom kingdom = town.OwnerClan?.Kingdom;
            if (kingdom == null || kingdom.IsEliminated) return false;
            Kingdom mine = Clan.PlayerClan.Kingdom;
            if (kingdom == mine)
            {
                Hero ruler = kingdom.Leader;
                if (ruler == null || ruler == Hero.MainHero) return false;
                int penalty = CrisisRules.OwnKingdomRulerPenaltyPerLevel * level;
                SchemeOutcomes.Relate(ruler, -penalty);
                ownKingdomLine = "Kendi krallığında lordlarını gözetlediğin duyuldu: hükümdarın " + ruler.Name + " ile ilişki -" + penalty + ".";
                return false;
            }
            if (FactionManager.IsAtWarAgainstFaction(kingdom, Clan.PlayerClan.MapFaction)) return false;

            int crime = CrisisRules.Crime(level);
            ChangeCrimeRatingAction.Apply(kingdom, crime, true);
            state.Tension[kingdom.StringId] = CrisisRules.AddTension(state.TensionWith(kingdom.StringId), CrisisRules.ExposureTension(level));
            state.Crises.Add(new Crisis
            {
                KingdomId = kingdom.StringId, TownId = town.StringId, Level = level, Day = SchemeFactors.Today, Crime = crime,
                OwnerId = penalizedOwner?.StringId,
            });
            state.AddLog(SchemeFactors.Today, "Diplomatik kriz: " + kingdom.Name + " (" + town.Name + " ağı)");
            return true;
        }

        /// <summary>Kriz hâlâ cevaplanabilir mi (krallık, hükümdar duruyor; savaşta değilsiniz).</summary>
        public static bool StillValid(Crisis crisis)
        {
            Kingdom kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == crisis.KingdomId);
            return kingdom != null && !kingdom.IsEliminated && kingdom.Leader != null && kingdom.Leader.IsAlive
                && kingdom != Clan.PlayerClan.Kingdom && !FactionManager.IsAtWarAgainstFaction(kingdom, Clan.PlayerClan.MapFaction);
        }

        public static Kingdom KingdomOf(Crisis crisis) => Kingdom.All.FirstOrDefault(k => k.StringId == crisis.KingdomId);

        public static int DenyChance(IntrigueState state, Crisis crisis) =>
            CrisisRules.DenyChance(SchemeFactors.Roguery(SchemeFactors.Spymaster(state)), Hero.MainHero.GetSkillValue(DefaultSkills.Charm),
                crisis.Level, SettingsStore.Current.IntrigueDifficulty);

        /// <summary>Kart metni: ne oldu, gerilim, seçeneklerin anlamı.</summary>
        public static string CardText(IntrigueState state, Crisis crisis)
        {
            Kingdom kingdom = KingdomOf(crisis);
            Settlement town = crisis.TownId != null ? Settlement.Find(crisis.TownId) : null;
            Hero owner = crisis.OwnerId != null ? Hero.Find(crisis.OwnerId) : null;
            int tension = state.TensionWith(crisis.KingdomId);
            return (town != null ? Tr.Genitive(town.Name.ToString()) + " muhafızları" : "Muhafızlar") + " gölge ağını ortaya çıkardı; muhbirlerin dağıldı. "
                + kingdom.Leader.Name + " izlerin sana çıktığını biliyor ve elçisini gönderdi: casusluğun bedelini istiyor.\n\n"
                + kingdom.Name + " seni suçlu sayıyor (oyunun suç puanı +" + crisis.Crime + ": muhafızlar sana karşı sertleşir)."
                + (owner != null ? " Şehrin sahibi " + owner.Name + " ile ilişki -10." : "") + "\n\n"
                + "Gerilim: " + CrisisRules.TensionName(tension) + " (" + tension + "/" + CrisisRules.MaxTension + "). "
                + CrisisRules.WarThreshold + "'ı aşarsa " + kingdom.Leader.Name + " savaş meclisini toplayabilir"
                + (Clan.PlayerClan.Kingdom != null ? "; savaşa lordları oy verir." : "; krallığın yokken asıl tehlike suç puanı.");
        }

        public static void Answer(IntrigueState state, Crisis crisis, CrisisAnswer answer)
        {
            int gold = CrisisRules.Reparations(crisis.Level);
            if (answer == CrisisAnswer.Pay && StillValid(crisis) && !SchemeFactors.CanAfford(gold)) throw new InvalidOperationException("Yeterli altının yok");
            state.Crises.Remove(crisis);
            if (!StillValid(crisis)) return;
            Kingdom kingdom = KingdomOf(crisis);
            Hero ruler = kingdom.Leader;

            bool denied = answer == CrisisAnswer.Deny && state.Roll100() <= DenyChance(state, crisis);
            int penalty = CrisisRules.RulerPenalty(answer, denied);
            var parts = new List<string>();
            if (answer == CrisisAnswer.Pay)
            {
                SchemeFactors.Pay(gold);
                parts.Add(SchemeFactors.Money(gold) + " tazminat ödendi");
            }
            if (answer == CrisisAnswer.Pay || denied)
            {
                float remove = Math.Min(answer == CrisisAnswer.Pay ? crisis.Crime : crisis.Crime / 2f, kingdom.MainHeroCrimeRating);
                if (remove > 0f)
                {
                    ChangeCrimeRatingAction.Apply(kingdom, -remove, true);
                    parts.Add("suç puanı -" + (int)remove);
                }
            }
            SchemeOutcomes.Relate(ruler, -penalty);
            parts.Add(ruler.Name + " ile ilişki -" + penalty);
            int before = state.TensionWith(kingdom.StringId);
            int after = CrisisRules.TensionAfter(before, crisis.Level, answer, denied);
            state.Tension[kingdom.StringId] = after;
            parts.Add("gerilim " + before + " -> " + after);

            string title = answer == CrisisAnswer.Pay ? "Tazminat ödendi" : answer == CrisisAnswer.Defy ? "Elçi eli boş döndü" : denied ? "İnkâr tuttu" : "İnkâr tutmadı";
            string text = answer == CrisisAnswer.Pay ? ruler.Name + " altını aldı; mesele kapandı ama unutulmadı."
                : answer == CrisisAnswer.Defy ? "Elçiye hiçbir şey vermedin. " + ruler.Name + " bunu bir hakaret saydı."
                : denied ? "Elçi kanıtların zayıf olduğunu kabul etti; " + ruler.Name + " şüpheli ama elinde bir şey yok."
                : "Yalanın ortaya çıktı: " + ruler.Name + " artık sana hem casus hem yalancı diyor.";
            SchemeOutcomes.Notify(state, title + ": " + kingdom.Name, text + "\n\nSonuç: " + string.Join(", ", parts) + ".", ruler, CardImages.Crisis);
            state.AddLog(SchemeFactors.Today, "Kriz cevabı (" + kingdom.Name + "): " + title.ToLower(Tr.Culture));
        }

        /// <summary>Saray sekmesinde krallığın satırına eklenen gerilim notu ("" ise gerilim yok).</summary>
        public static string TensionLine(IntrigueState state, Kingdom kingdom)
        {
            int tension = state.TensionWith(kingdom.StringId);
            return tension <= 0 ? "" : "  ·  casus gerilimi: " + CrisisRules.TensionName(tension) + " (" + tension + "/" + CrisisRules.MaxTension + ")";
        }

        // ---- Haftalık ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            foreach (string id in state.Tension.Keys.ToList())
            {
                int value = CrisisRules.Decay(state.Tension[id]);
                if (value <= 0) state.Tension.Remove(id);
                else state.Tension[id] = value;
            }
            state.Crises.RemoveAll(c => !StillValid(c));

            Kingdom mine = Clan.PlayerClan.Kingdom;
            if (mine == null) return;
            foreach (KeyValuePair<string, int> pair in state.Tension.ToList())
            {
                Kingdom kingdom = Kingdom.All.FirstOrDefault(k => k.StringId == pair.Key);
                if (kingdom == null || kingdom.IsEliminated || kingdom == mine || kingdom.Leader == null) continue;
                if (FactionManager.IsAtWarAgainstFaction(kingdom, mine)) continue;
                // Rehineyle güvence altındaki barışta savaş meclisi toplanmaz.
                if (state.HostageDealWith(kingdom.StringId) != null) continue;
                int wars = Kingdom.All.Count(k => k != kingdom && !k.IsEliminated && FactionManager.IsAtWarAgainstFaction(kingdom, k));
                if (!CrisisRules.MayProposeWar(pair.Value, kingdom.CurrentTotalStrength, mine.CurrentTotalStrength, wars)) continue;
                if (state.Roll100() > CrisisRules.WeeklyWarChance(pair.Value)) continue;
                try
                {
                    ProposeWar(state, kingdom, mine);
                }
                catch (Exception ex)
                {
                    TaleWorlds.Library.Debug.Print("[CheatMenu] war proposal failed: " + ex);
                }
            }
        }

        /// <summary>Hükümdar, oyunun kendi "savaş ilan et" kararını kendi meclisine sunar; lordlar oy verir.</summary>
        private static void ProposeWar(IntrigueState state, Kingdom kingdom, Kingdom mine)
        {
            if (kingdom.UnresolvedDecisions.Any(d => d is DeclareWarDecision w && w.FactionToDeclareWarOn == mine)) return;
            var decision = new DeclareWarDecision(kingdom.RulingClan, mine);
            if (!decision.IsAllowed()) return;
            kingdom.AddDecision(decision, true);
            state.Tension[kingdom.StringId] = CrisisRules.TensionAfterWarProposal;

            string text = kingdom.Leader.Name + " casus skandallarını artık hoş görmüyor ve savaş meclisini topladı: " + kingdom.Name
                + " lordları " + Tr.Dative(mine.Name.ToString()) + " savaş açılıp açılmayacağını oylayacak. Kararı oyunun kendi meclisi verir.";
            Hero myRuler = mine.Leader;
            if (myRuler != null && myRuler != Hero.MainHero)
            {
                SchemeOutcomes.Relate(myRuler, -CrisisRules.OwnRulerWarAnger);
                text += "\n\nHükümdarın " + myRuler.Name + ", krallığı senin casusların yüzünden savaşın eşiğine getirdiğin için kızgın: ilişki -"
                    + CrisisRules.OwnRulerWarAnger + ".";
            }
            SchemeOutcomes.Notify(state, "Savaş meclisi: " + kingdom.Name, text, kingdom.Leader, CardImages.WarCouncil);
            state.AddLog(SchemeFactors.Today, "Savaş meclisi: " + kingdom.Name + " sana savaş açmayı oyluyor");
        }
    }
}
