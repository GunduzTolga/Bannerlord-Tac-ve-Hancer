using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>Oyundan entrika girdilerini toplar: casus başı, hedef kuralları (tasarım 5.1, 7), formül girdileri, etiketler.</summary>
    public static class SchemeFactors
    {
        public static int Today => (int)CampaignTime.Now.ToDays;

        // ---- Casus başı ----

        public static IEnumerable<Hero> SpymasterCandidates() =>
            Clan.PlayerClan.Heroes.Concat(Clan.PlayerClan.Companions).Distinct()
                .Where(h => h.IsAlive && !h.IsChild && h != Hero.MainHero);

        /// <summary>Kayıttaki casus başı hâlâ klanda ve hayattaysa onu, değilse null döner.</summary>
        public static Hero Spymaster(IntrigueState state)
        {
            if (state.SpymasterId == null) return null;
            Hero hero = Hero.Find(state.SpymasterId);
            return hero != null && hero.IsAlive && hero.Clan == Clan.PlayerClan ? hero : null;
        }

        /// <summary>
        /// null: kişi bir divan koltuğunda çalışabilir. Değilse neden (esir, rehine, vali, parti lideri, genelevde). Casus başı,
        /// hazinedar, hekim ve kâhya için ortak tanım; serdar parti başındayken de çalışır (Marshal), ama esir ya da rehineyken değil.
        /// </summary>
        public static string SpymasterBusyReason(Hero hero)
        {
            if (hero.IsPrisoner) return "esir";
            if (IsHostageAbroad(hero)) return "rehine olarak yabancı sarayda";
            if (hero.GovernorOf != null) return "vali";
            MobileParty party = hero.PartyBelongedTo;
            if (party != null && party.LeaderHero == hero && !party.IsMainParty) return "parti lideri";
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state != null && NightHouses.RoleOf(state, hero) != null) return NightHouses.RoleOf(state, hero);
            return null;
        }

        /// <summary>Rehine anlaşmasıyla başka bir krallığın sarayına gönderdiğin kişi.</summary>
        public static bool IsHostageAbroad(Hero hero)
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            return hero != null && state != null && state.Hostages.Any(d => d.GivenId == hero.StringId);
        }

        /// <summary>Rehine anlaşmasıyla senin yanında misafir olan kişi: ona el kaldırmak yemini bozar.</summary>
        public static bool IsHeldGuest(IntrigueState state, Hero hero) =>
            hero != null && state != null && state.Hostages.Any(d => d.HeldId == hero.StringId);

        public static int Roguery(Hero hero) => hero?.GetSkillValue(DefaultSkills.Roguery) ?? 0;

        // ---- Hedefler ----

        public static IEnumerable<Hero> Targets(IntrigueState state, SchemeKind kind) =>
            Hero.AllAliveHeroes.Where(h => BlockedReason(state, h, kind) == null);

        /// <summary>
        /// null: bu türde hedef olabilir. Casusluk: yetişkin lord ve gezginler (eşrafın sırrı ve gezisi yok).
        /// Şantaj: ifşa edilmemiş bir sırrını bildiğin, üzerinde kozun olmayan lord.
        /// Suikast: tanıştığın ya da geçerli raporun olan, yeri belli lord, gezgin ve eşraf. Kaçırma: aynısı, eşraf ve esirler hariç.
        /// </summary>
        public static string BlockedReason(IntrigueState state, Hero h, SchemeKind kind)
        {
            string basic = TargetBlockedReason(state, h);
            if (basic != null) return basic;
            switch (kind)
            {
                case SchemeKind.Spy:
                    return h.IsLord || h.IsWanderer ? null : "casusluk yalnızca lord ve gezginlere yapılır";
                case SchemeKind.Blackmail:
                    if (!h.IsLord) return "şantaj yalnızca lordlara yapılır";
                    if (BestSecret(state, h) == null) return "elinde onun ifşa edilmemiş, kozda kullanılmamış bir sırrı yok (önce casusluk)";
                    if (state.ActiveHook(h.StringId, Today) != null) return "zaten üzerinde bir kozun var";
                    return null;
            }
            if (kind == SchemeKind.Kidnap ? !h.IsLord && !h.IsWanderer : !h.IsLord && !h.IsWanderer && !h.IsNotable)
                return "bu kişi " + SchemeRules.Name(kind).ToLower(new System.Globalization.CultureInfo("tr-TR")) + " hedefi olamaz";
            if (kind == SchemeKind.Kidnap && h.IsPrisoner) return "hedef zaten birinin esiri";
            if (kind == SchemeKind.Murder && !CanBeMurdered(h)) return "bu kampanyada cinayetle ölüm kapalı (Doğum ve Ölüm ayarı)";
            if (!h.HasMet && !state.HasValidReport(h.StringId, Today)) return "tanışmadığın biri için güncel istihbarat raporu gerekir";
            if (h.IsFugitive || !IsLocatable(h)) return "hedefin yeri belli değil";
            return null;
        }

        /// <summary>Oyun bu kahramanın cinayetle ölmesine izin veriyor mu (Doğum ve Ölüm kapalıysa vermez).</summary>
        public static bool CanBeMurdered(Hero h) => h.CanDie(KillCharacterAction.KillCharacterActionDetail.Murdered);

        /// <summary>Hedefin ifşa edilmemiş en ağır sırrı (şantajın gücü buna bağlı); yoksa null.</summary>
        public static Secret BestSecret(IntrigueState state, Hero h) =>
            state.Secrets.Where(s => s.TargetId == h.StringId && !s.Exposed && !s.Spent)
                .OrderByDescending(s => SecretRules.ExposurePenalty(s.Kind)).FirstOrDefault();

        /// <summary>null: hedef olabilir (türden bağımsız temel kurallar). Değilse neden.</summary>
        public static string TargetBlockedReason(IntrigueState state, Hero h)
        {
            if (h == null || !h.IsAlive) return "hedef yaşamıyor";
            if (h == Hero.MainHero || h.Clan == Clan.PlayerClan) return "kendi klanına entrika kurulamaz";
            if (h.IsChild) return "çocuklara dokunulmaz";
            if (h.IsDisabled) return "hedef şu an oyunda değil";
            if (state.FindScheme(h.StringId) != null) return "bu hedefe zaten bir entrika sürüyor";
            if (IsHeldGuest(state, h)) return "rehine anlaşmasıyla misafirin: ona dokunmak yeminini bozar";
            if (state.IsOnCooldown(h.StringId, Today))
                return "bu hedef tetikte; " + (state.CooldownUntil[h.StringId] - Today) + " gün sonra tekrar dene";
            return null;
        }

        private static bool IsLocatable(Hero h) => h.IsPrisoner || h.PartyBelongedTo != null || h.CurrentSettlement != null;

        /// <summary>Senin esirin ya da senin yerleşiminde (tasarım 5.3: başarı +20, iz bırakma ×2).</summary>
        public static bool IsPlayersCaptive(Hero h)
        {
            if (IsHeldGuest(IntrigueBehavior.Current?.State, h)) return false;
            if (h.IsPrisoner)
            {
                PartyBase captor = h.PartyBelongedToAsPrisoner;
                return captor == PartyBase.MainParty || captor?.Settlement?.OwnerClan == Clan.PlayerClan;
            }
            return h.CurrentSettlement?.OwnerClan == Clan.PlayerClan;
        }

        /// <summary>Savaşın ya da kuşatmanın içindeyse harekete geçilemez.</summary>
        public static bool InBattle(Hero h)
        {
            MobileParty party = h.PartyBelongedTo;
            return party?.MapEvent != null || party?.SiegeEvent != null || h.CurrentSettlement?.SiegeEvent != null;
        }

        public static bool IsTraveling(Hero h) => !h.IsPrisoner && h.PartyBelongedTo != null && h.PartyBelongedTo.CurrentSettlement == null;

        // ---- Formül girdileri ----

        /// <param name="scheme">null ise önizleme: en az aşama, olay etkisi yok.</param>
        public static SchemeInputs Inputs(IntrigueState state, Hero spymaster, Hero target, SchemeKind kind, Scheme scheme,
            int bribeLevel = 0)
        {
            int today = Today;
            NetworkBonus(state, target, out int networkSuccess, out int networkDiscovery);
            Secret secret = kind == SchemeKind.Blackmail ? BestSecret(state, target) : null;
            return new SchemeInputs
            {
                Roguery = Roguery(spymaster),
                Calculating = spymaster?.GetTraitLevel(DefaultTraits.Calculating) ?? 0,
                CompletedPhases = scheme?.Phase ?? SchemeRules.MinPhases(kind),
                HasReport = state.HasValidReport(target.StringId, today),
                EventSuccess = scheme?.EventSuccess ?? 0,
                EventDiscovery = scheme?.EventDiscovery ?? 0,
                BribeLevel = bribeLevel,
                Difficulty = SettingsStore.Current.IntrigueDifficulty,
                TargetRoguery = target.GetSkillValue(DefaultSkills.Roguery),
                TargetCalculating = target.GetTraitLevel(DefaultTraits.Calculating),
                TargetClanTier = TargetTier(target),
                TargetIsClanLeader = target.IsClanLeader,
                TargetIsRuler = target.IsKingdomLeader,
                TargetInOwnSettlement = target.Clan != null && target.CurrentSettlement != null
                    && target.CurrentSettlement.OwnerClan == target.Clan,
                TargetInArmy = target.PartyBelongedTo?.Army != null,
                TargetIsPlayersCaptive = IsPlayersCaptive(target),
                TargetAlerted = state.IsAlerted(target.StringId, today),
                HasMet = target.HasMet,
                NetworkSuccess = networkSuccess,
                NetworkDiscovery = networkDiscovery,
                AgentSuccess = spymaster != null ? AgentRules.Success(state.Agents) : 0,
                AgentDiscovery = spymaster != null ? AgentRules.Discovery(state.Agents) : 0,
                HasHook = state.ActiveHook(target.StringId, today) != null,
                SecretSeverity = secret != null ? SecretRules.ExposurePenalty(secret.Kind) : 0,
            };
        }

        public static int TargetTier(Hero target) => target.Clan?.Tier ?? 0;

        // ---- Gölge ağı ----

        /// <summary>Hedefin şu an bulunduğu yerleşim (parti içindeyse partinin, esirse zindanın); yoksa null.</summary>
        public static Settlement TargetSettlement(Hero h) =>
            h.CurrentSettlement ?? h.PartyBelongedTo?.CurrentSettlement ?? (h.IsPrisoner ? h.PartyBelongedToAsPrisoner?.Settlement : null);

        /// <summary>Hedefe en çok yarayan gölge ağının bonusu: aynı şehirdeyse tam, aynı krallıktaysa yarısı.</summary>
        public static void NetworkBonus(IntrigueState state, Hero target, out int success, out int discovery)
        {
            success = discovery = 0;
            Settlement where = TargetSettlement(target);
            foreach (ShadowNetwork network in state.Networks)
            {
                Settlement town = Settlement.Find(network.SettlementId);
                if (town == null) continue;
                bool sameTown = where == town;
                if (!sameTown && (town.MapFaction == null || town.MapFaction != target.MapFaction)) continue;
                success = Math.Max(success, SchemeMath.NetworkSuccess(network.Level, sameTown));
                discovery = Math.Max(discovery, SchemeMath.NetworkDiscovery(network.Level, sameTown));
            }
        }

        public static int StartCost(SchemeKind kind, Hero target) =>
            SchemeMath.StartCost(kind, TargetTier(target), target.IsClanLeader, target.IsKingdomLeader);

        // ---- Etiketler ----

        public static string TargetLabel(Hero h) => h.Name + " (" + Affiliation(h) + ")";

        public static string Affiliation(Hero h)
        {
            if (h.IsNotable) return "eşraf, " + (h.CurrentSettlement ?? h.HomeSettlement)?.Name;
            if (h.Clan == null) return "gezgin";
            return h.Clan.Name + (h.Clan.Kingdom != null ? ", " + h.Clan.Kingdom.Name : "") + (h.IsKingdomLeader ? ", hükümdar" : "");
        }

        public static string SpymasterLabel(Hero h)
        {
            string busy = SpymasterBusyReason(h);
            return h.Name + " - Roguery " + Roguery(h) + ", Hesapçı " + Signed(h.GetTraitLevel(DefaultTraits.Calculating))
                + (busy != null ? "  [" + busy + "]" : "");
        }

        public static string TraitsText(Hero h)
        {
            List<string> parts = DefaultTraits.Personality
                .Select(t => new { t.Name, Level = h.GetTraitLevel(t) })
                .Where(t => t.Level != 0)
                .Select(t => t.Name + " " + Signed(t.Level))
                .ToList();
            return parts.Count > 0 ? string.Join(", ", parts) : "belirgin bir özelliği yok";
        }

        public static string Signed(int value) => value > 0 ? "+" + value : value.ToString();

        public static string Money(int value) => value.ToString("N0") + " dinar";

        /// <summary>Oyuncunun parası yetiyor mu.</summary>
        public static bool CanAfford(int cost) => Hero.MainHero.Gold >= cost;

        public static void Pay(int cost)
        {
            if (cost <= 0) return;
            if (!CanAfford(cost)) throw new InvalidOperationException("Yeterli altının yok (" + Money(cost) + " gerekiyor)");
            Hero.MainHero.ChangeHeroGold(-cost);
        }
    }
}
