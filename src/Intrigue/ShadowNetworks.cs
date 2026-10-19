using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Gölge ağları (tasarım 8.3): şehirlerde çete lideri üzerinden kurulan muhbir ağları. Kasaba menüsüne
    /// "Gölgelerde iş çevir" seçeneği eklenir (CampaignGameStarter.AddGameMenuOption, Harmony gerekmez).
    /// Haftalık bakım ödenmezse ağ zayıflar; yapay zekânın karşı casusluğu ağı bulup dağıtabilir.
    /// </summary>
    public static class ShadowNetworks
    {
        public const int GiftCost = 1000;
        public const int GiftRelation = 5;

        public static void RegisterMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption("town", "tvh_shadows", "Gölgelerde iş çevir",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    Settlement town = Settlement.CurrentSettlement;
                    return town != null && town.IsTown && GangLeaders(town).Count > 0;
                },
                _ => MenuUI.SafeRun("Gölge ağı", () => Categories.ShowShadowTown(Settlement.CurrentSettlement, true)),
                false, 5);
        }

        public static List<Hero> GangLeaders(Settlement town) =>
            town.Notables.Where(n => n.IsGangLeader && n.IsAlive).ToList();

        /// <summary>Şehirdeki en iyi ilişkideki çete lideri.</summary>
        public static Hero BestGangLeader(Settlement town) =>
            GangLeaders(town).OrderByDescending(n => Hero.MainHero.GetRelation(n)).FirstOrDefault();

        /// <summary>null: bu şehirde ağ kurulabilir ya da bir üst seviyeye çıkarılabilir; değilse neden.</summary>
        public static string UpgradeBlocked(IntrigueState state, Settlement town)
        {
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (spymaster == null) return "Önce bir casus başı ata";
            ShadowNetwork network = state.Network(town.StringId);
            int level = (network?.Level ?? 0) + 1;
            if (level > SchemeMath.MaxNetworkLevel) return "Ağ zaten en üst seviyede";
            int max = SchemeMath.MaxNetworks(SchemeFactors.Roguery(spymaster));
            if (network == null && state.Networks.Count >= max)
                return "En fazla " + max + " ağ yönetebilirsin (casus başının Roguery'si her 75'te bir ağ daha)";
            Hero leader = BestGangLeader(town);
            int needed = SchemeMath.NetworkRelationRequired(level);
            if (leader == null) return "Bu şehirde çete lideri yok";
            if (Hero.MainHero.GetRelation(leader) < needed)
                return leader.Name + " ile ilişkin en az " + needed + " olmalı (şu an " + Hero.MainHero.GetRelation(leader) + ")";
            if (!SchemeFactors.CanAfford(SchemeMath.NetworkCost(level))) return "Yeterli altının yok";
            return null;
        }

        public static void Upgrade(IntrigueState state, Settlement town)
        {
            string blocked = UpgradeBlocked(state, town);
            if (blocked != null) throw new InvalidOperationException(blocked);
            ShadowNetwork network = state.Network(town.StringId);
            int level = (network?.Level ?? 0) + 1;
            SchemeFactors.Pay(SchemeMath.NetworkCost(level));
            if (network == null) state.Networks.Add(network = new ShadowNetwork { SettlementId = town.StringId, Since = SchemeFactors.Today });
            network.Level = level;
            state.AddLog(SchemeFactors.Today, Tr.Locative(town.Name.ToString()) + " gölge ağı: seviye " + level);
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " gölge ağı seviye " + level + " oldu");
        }

        public static void Disband(IntrigueState state, Settlement town)
        {
            ShadowNetwork network = state.Network(town.StringId) ?? throw new InvalidOperationException("Bu şehirde ağın yok");
            state.Networks.Remove(network);
            state.AddLog(SchemeFactors.Today, Tr.Locative(town.Name.ToString()) + " gölge ağı dağıtıldı");
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " gölge ağı dağıtıldı");
        }

        /// <summary>Çete liderine hediye: ağ kurmak için gereken ilişkiye ulaşmanın yolu.</summary>
        public static void Gift(Settlement town)
        {
            Hero leader = BestGangLeader(town) ?? throw new InvalidOperationException("Bu şehirde çete lideri yok");
            SchemeFactors.Pay(GiftCost);
            SchemeOutcomes.Relate(leader, GiftRelation);
            MenuUI.Success(leader.Name + " hediyeni aldı: ilişki " + Hero.MainHero.GetRelation(leader));
        }

        public static string Describe(IntrigueState state, Settlement town)
        {
            ShadowNetwork network = state.Network(town.StringId);
            var lines = new List<string>
            {
                "Çete liderleri: " + string.Join(", ", GangLeaders(town).Select(n => n.Name + " (ilişki " + Hero.MainHero.GetRelation(n) + ")")),
                network == null
                    ? "Burada ağın yok."
                    : "Ağ seviyesi: " + network.Level + "/" + SchemeMath.MaxNetworkLevel + ", haftalık bakım " + SchemeFactors.Money(SchemeMath.NetworkUpkeep(network.Level)),
                "Etkisi: bu şehirdeki hedeflere başarı +5/+10/+15 ve yakalanma -2/-4/-6 (seviyeye göre); "
                    + "aynı krallıktaki diğer hedeflere bunların yarısı.",
                "Yapay zekâ her hafta ağı bulup dağıtabilir (seviye başına %" + SchemeMath.NetworkExposeChance(1) + ").",
                "Yabancı bir krallıkta çökertilirse diplomatik kriz: o krallıkta suç puanı (seviye başına +" + CrisisRules.CrimePerLevel
                    + ") ve hükümdar hesap sorar (tazminat, inkâr ya da ret). Gerilim " + CrisisRules.WarThreshold
                    + "'ı aşarsa savaş meclisi toplanabilir.",
            };
            return string.Join("\n", lines);
        }

        // ---- Haftalık ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.Networks.Count == 0) return;
            int today = SchemeFactors.Today;

            foreach (ShadowNetwork network in state.Networks.ToList())
            {
                Settlement town = Settlement.Find(network.SettlementId);
                if (town == null || GangLeaders(town).Count == 0)
                {
                    state.Networks.Remove(network);
                    SchemeOutcomes.Notify(state, "Gölge ağı dağıldı", (town != null ? Tr.Locative(town.Name.ToString()) + " " : "") + "artık güvenebileceğin bir çete yok.", null,
                        CardImages.ShadowNetwork);
                    continue;
                }
                if (state.Roll100() > SchemeMath.NetworkExposeChance(network.Level)) continue;
                state.Networks.Remove(network);
                Hero owner = town.OwnerClan?.Leader;
                bool penalized = owner != null && owner.Clan != Clan.PlayerClan;
                if (penalized) SchemeOutcomes.Relate(owner, -10);
                state.AddLog(today, Tr.Locative(town.Name.ToString()) + " gölge ağı çökertildi");
                // Yabancı krallıkta: diplomatik kriz kartı her şeyi anlatır (P2.5).
                if (DiplomaticCrises.OnNetworkExposed(state, town, network.Level, penalized ? owner : null, out string ownKingdomLine)) continue;
                SchemeOutcomes.Notify(state, "Gölge ağın çökertildi: " + town.Name,
                    Tr.Genitive(town.Name.ToString()) + " muhafızları ağını ortaya çıkardı; muhbirlerin dağıldı."
                    + (penalized ? "\n\nSonuç: şehrin sahibi " + owner.Name + " ile ilişki -10." : "")
                    + (ownKingdomLine != null ? "\n\n" + ownKingdomLine : ""), owner,
                    CardImages.ShadowNetwork);
            }

            int upkeep = state.Networks.Sum(n => SchemeMath.NetworkUpkeep(n.Level));
            if (upkeep <= 0) return;
            if (SchemeFactors.CanAfford(upkeep))
            {
                SchemeFactors.Pay(upkeep);
                return;
            }
            foreach (ShadowNetwork network in state.Networks.ToList())
                if (--network.Level <= 0) state.Networks.Remove(network);
            SchemeOutcomes.Notify(state, "Muhbirlerin parasız kaldı",
                "Gölge ağlarının haftalık bakımını (" + SchemeFactors.Money(upkeep) + ") ödeyemedin; her ağ bir seviye zayıfladı.", null,
                CardImages.ShadowNetwork);
            state.AddLog(today, "Ağların bakımı ödenemedi; ağlar zayıfladı");
        }
    }
}
