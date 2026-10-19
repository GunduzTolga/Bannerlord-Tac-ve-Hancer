using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Sabotaj ve kışkırtma (E3, tasarım: docs/superpowers/specs/2026-10-20-e3-sabotaj-kiskirtma.md): hedefi bir yerleşim olan entrikalar.
    /// Yaşam döngüsü SchemeEngine'de (başlat, aşama, harekete geç); hedef kuralları, girdiler, etkiler ve olaylar burada.
    /// Formüller SettlementSchemeRules'ta (saf, test edilir).
    /// </summary>
    public static class SettlementSchemes
    {
        private const int SuccessXp = 1000;
        private const int FailXp = 300;
        private const int HonorOnExposure = -40;
        private const int MercyOnSilencing = -20;

        private static int Today => SchemeFactors.Today;

        public static Settlement Find(Scheme scheme) => Settlement.Find(scheme.TargetId);

        /// <summary>Kartlarda ve listede gösterilen yüz: vali, yoksa sahibi klanın lideri.</summary>
        public static Hero PortraitHero(Settlement s) => s?.Town?.Governor ?? s?.OwnerClan?.Leader;

        // ---- Hedefler ----

        public static IEnumerable<Settlement> Targets(IntrigueState state, SchemeKind kind) =>
            Settlement.All.Where(s => BlockedReason(state, s, kind) == null);

        /// <summary>null: bu türde hedef olabilir; değilse neden.</summary>
        public static string BlockedReason(IntrigueState state, Settlement s, SchemeKind kind)
        {
            if (s?.Town == null || !(s.IsTown || s.IsCastle)) return "yalnızca şehir ve kaleler hedef olabilir";
            if (!s.IsTown && !SettlementSchemeRules.AllowsCastle(kind)) return "kışkırtma yalnızca şehirlerde olur (oyunda kaleler isyan etmez)";
            if (s.OwnerClan == null || s.OwnerClan == Clan.PlayerClan) return "kendi yerleşimine entrika kurulamaz";
            if (IsMyRealm(s)) return "kendi krallığının yerleşimine entrika kurulamaz";
            if (kind == SchemeKind.Incite && s.OwnerClan.IsRebelClan) return "şehir zaten isyancıların elinde";
            if (state.FindScheme(s.StringId) != null) return "bu yerleşime zaten bir entrika sürüyor";
            if (state.IsOnCooldown(s.StringId, Today))
                return "burası tetikte; " + (state.CooldownUntil[s.StringId] - Today) + " gün sonra tekrar dene";
            if (SettlementSchemeRules.NeedsNetwork(kind) && NetworkLevel(state, s) == 0)
                return "orada gölge ağın yok (elebaşları yeraltından gelir: önce oradaki çete lideriyle ağ kur)";
            return null;
        }

        /// <summary>Entrika sürerken hedef geçersiz olduysa neden (o zaman entrika sona erer).</summary>
        public static string GoneReason(IntrigueState state, Scheme scheme, Settlement s)
        {
            if (s?.Town == null) return "hedef yerleşim bulunamadı";
            if (s.OwnerClan == Clan.PlayerClan || IsMyRealm(s)) return s.Name + " artık senin krallığının";
            if (scheme.Kind == SchemeKind.Incite && s.OwnerClan != null && s.OwnerClan.IsRebelClan) return s.Name + " artık isyancıların elinde";
            if (SettlementSchemeRules.NeedsNetwork(scheme.Kind) && NetworkLevel(state, s) == 0) return "orada gölge ağın kalmadı";
            return null;
        }

        private static bool IsMyRealm(Settlement s) => Clan.PlayerClan.MapFaction != null && s.MapFaction == Clan.PlayerClan.MapFaction;

        public static int NetworkLevel(IntrigueState state, Settlement s) =>
            s == null ? 0 : state.Networks.FirstOrDefault(n => n.SettlementId == s.StringId)?.Level ?? 0;

        /// <summary>Sen (partinle ya da içinde olduğun orduyla) bu yerleşimi kuşatıyor musun.</summary>
        public static bool IsBesiegedByPlayer(Settlement s)
        {
            MobileParty me = MobileParty.MainParty;
            if (me == null || s?.SiegeEvent == null) return false;
            if (me.BesiegedSettlement == s || me.SiegeEvent?.BesiegedSettlement == s) return true;
            MobileParty leader = me.Army?.LeaderParty;
            return leader != null && me.AttachedTo == leader && leader.BesiegedSettlement == s;
        }

        // ---- Girdiler ----

        /// <param name="scheme">null ise önizleme: en az aşama, olay etkisi yok.</param>
        public static SettlementSchemeInputs Inputs(IntrigueState state, Hero spymaster, Settlement s, SchemeKind kind, Scheme scheme,
            int bribeLevel = 0)
        {
            Town town = s.Town;
            return new SettlementSchemeInputs
            {
                Roguery = SchemeFactors.Roguery(spymaster),
                Calculating = spymaster?.GetTraitLevel(DefaultTraits.Calculating) ?? 0,
                CompletedPhases = scheme?.Phase ?? SchemeRules.MinPhases(kind),
                EventSuccess = scheme?.EventSuccess ?? 0,
                EventDiscovery = scheme?.EventDiscovery ?? 0,
                BribeLevel = bribeLevel,
                Difficulty = SettingsStore.Current.IntrigueDifficulty,
                NetworkLevel = NetworkLevel(state, s),
                IsCastle = s.IsCastle,
                Security = (int)Math.Round(town?.Security ?? 0f),
                Loyalty = (int)Math.Round(town?.Loyalty ?? 50f),
                GovernorRoguery = town?.Governor?.GetSkillValue(DefaultSkills.Roguery) ?? 0,
                Alerted = state.IsAlerted(s.StringId, Today),
            };
        }

        public static int StartCost(SchemeKind kind, Settlement s) => SettlementSchemeRules.StartCost(kind, s.IsCastle);

        public static int Success(IntrigueState state, Scheme scheme, Settlement s, int bribeLevel = 0) =>
            SettlementSchemeRules.Success(scheme.Kind, Inputs(state, SchemeFactors.Spymaster(state), s, scheme.Kind, scheme, bribeLevel));

        public static int Discovery(IntrigueState state, Scheme scheme, Settlement s) =>
            SettlementSchemeRules.Discovery(scheme.Kind, Inputs(state, SchemeFactors.Spymaster(state), s, scheme.Kind, scheme));

        // ---- Etiketler ve önizleme ----

        public static string TargetLabel(Settlement s) => s.Name + " (" + Affiliation(s) + ")";

        public static string Affiliation(Settlement s)
        {
            string kind = s.IsCastle ? "kale" : "şehir";
            if (s.OwnerClan == null) return kind;
            return kind + ", " + s.OwnerClan.Name + (s.OwnerClan.Kingdom != null ? ", " + s.OwnerClan.Kingdom.Name : "");
        }

        public static string Tags(IntrigueState state, Settlement s)
        {
            var tags = new List<string>();
            int network = NetworkLevel(state, s);
            if (network > 0) tags.Add("gölge ağı " + network);
            if (s.Town != null) tags.Add("sadakat " + (int)Math.Round(s.Town.Loyalty) + ", güvenlik " + (int)Math.Round(s.Town.Security));
            if (IsBesiegedByPlayer(s)) tags.Add("kuşatıyorsun");
            else if (s.IsUnderSiege) tags.Add("kuşatmada");
            if (state.IsAlerted(s.StringId, Today)) tags.Add("tetikte");
            return string.Join(" · ", tags);
        }

        public static void SuccessRange(IntrigueState state, Hero spymaster, Settlement s, SchemeKind kind, out int low, out int high)
        {
            SettlementSchemeInputs inputs = Inputs(state, spymaster, s, kind, null);
            low = SettlementSchemeRules.Success(kind, inputs);
            inputs.CompletedPhases = SchemeRules.MaxPhases(kind);
            high = SettlementSchemeRules.Success(kind, inputs);
        }

        public static string ChanceText(IntrigueState state, Hero spymaster, Settlement s, SchemeKind kind)
        {
            SuccessRange(state, spymaster, s, kind, out int low, out int high);
            return low == high ? "%" + low : "%" + low + "-" + high;
        }

        public static List<string> PreviewLines(IntrigueState state, SchemeKind kind, Settlement s)
        {
            Hero spymaster = SchemeFactors.Spymaster(state);
            SettlementSchemeInputs inputs = Inputs(state, spymaster, s, kind, null);
            int cost = StartCost(kind, s);
            int days = SchemeMath.PhaseDays(kind, SchemeFactors.Roguery(spymaster));
            int min = SchemeRules.MinPhases(kind), max = SchemeRules.MaxPhases(kind);
            int low = SettlementSchemeRules.Success(kind, inputs);
            inputs.CompletedPhases = max;
            int high = SettlementSchemeRules.Success(kind, inputs);

            var lines = new List<string>
            {
                "Hedef: " + TargetLabel(s),
                "Maliyet: " + SchemeFactors.Money(cost) + " (sende " + SchemeFactors.Money(Hero.MainHero.Gold) + ")",
                "Aşamalar: en az " + min + ", en fazla " + max + "; her aşama yaklaşık " + days + " gün",
                "Başarı: " + min + " aşamada %" + low + ", " + max + " aşamada %" + high + " (harekete geçerken rüşvetle +5/+10/+15)",
                "Her aşamada yakalanma: %" + SettlementSchemeRules.Discovery(kind, inputs),
            };
            if (inputs.NetworkLevel > 0) lines.Add("Gölge ağın burada (seviye " + inputs.NetworkLevel + "): başarı +" + 5 * inputs.NetworkLevel
                + ", yakalanma -" + 2 * inputs.NetworkLevel + ".");
            if (inputs.GovernorRoguery > 0) lines.Add("Vali " + s.Town.Governor.Name + " (Roguery " + inputs.GovernorRoguery + ") işini zorlaştırır.");
            if (inputs.Alerted) lines.Add("Burası tetikte: -15 başarı.");
            lines.Add("");
            if (kind == SchemeKind.Sabotage)
                lines.Add("Başlarken neyi vuracağını seçersin: erzak ambarı (erzakın %70'i gider), garnizon (sağlıklı askerlerin %20'si ölür) "
                    + "ya da surlar (her bölüm yarıya iner, çok hasarlısı çöker). Vurmak için " + s.Name + " kuşatılırken orada olmalısın; "
                    + "plan hazırsa surların dibine gelince sorulur. Hazırlıkta ortaya çıkarsa sahibiyle ilişki -10, barıştaysanız suç puanı ve gerilim.");
            else
                lines.Add("Her aşamada sadakat -" + SettlementSchemeRules.LoyaltyPerPhase + ". Başarılı olursa sadakat -" + SettlementSchemeRules.LoyaltyOnSuccess
                    + ", güvenlik -" + SettlementSchemeRules.SecurityOnSuccess + "; ardından oyunun kendi isyan denetimi bakar (sadakat "
                    + RebellionThreshold() + "'in altında ve milis garnizondan güçlüyse şehir ayaklanır). Ortaya çıkarsa sahibi ve hükümdarıyla "
                    + "ilişki cezası, Onur kaybı; barıştaysanız suç puanı ve casus gerilimi.");
            return lines;
        }

        public static string KindDescription(SchemeKind kind) =>
            kind == SchemeKind.Sabotage
                ? "Kuşatacağın bir yerleşimin erzakını, garnizonunu ya da surlarını içeriden vur: 1-3 aşama, orta risk. "
                    + "Önceden hazırlanır, yalnızca kuşatırken vurulur. Orada gölge ağın varsa kolaylaşır."
                : "Bir şehrin halkını sahibine karşı kışkırt: 2-4 aşama, orta risk. Orada gölge ağın olmalı. "
                    + "Sadakat düşer; oyunun kendi isyan koşulları tutarsa şehir ayaklanır.";

        public static string NoTargetsText(SchemeKind kind) =>
            kind == SchemeKind.Incite
                ? "Kışkırtma için başka bir krallığın, gölge ağın olan bir şehri gerekir (Entrika > Gölge ağları)."
                : "Sabotaj için başka bir krallığın şehri ya da kalesi gerekir.";

        // ---- Aşamalar ----

        /// <summary>Tamamlanan bir aşamanın yerleşime etkisi (kışkırtmada sadakat düşer). Bilgi satırı döner.</summary>
        public static string OnPhaseCompleted(Scheme scheme, Settlement s)
        {
            if (scheme.Kind != SchemeKind.Incite || s.Town == null) return null;
            s.Town.Loyalty = SettlementSchemeRules.LoyaltyAfter(s.Town.Loyalty, -SettlementSchemeRules.LoyaltyPerPhase);
            return "sadakat -" + SettlementSchemeRules.LoyaltyPerPhase + " (şimdi " + (int)Math.Round(s.Town.Loyalty) + ")";
        }

        /// <summary>null: harekete geçilebilir; değilse neden (yalnızca yerleşime özgü engeller).</summary>
        public static string CannotExecuteReason(Scheme scheme, Settlement s)
        {
            if (scheme.Kind == SchemeKind.Sabotage)
                return IsBesiegedByPlayer(s) ? null : "Sabotaj ancak " + s.Name + " kuşatılırken yapılır: önce kuşat";
            if (s.Party.MapEvent != null || s.IsUnderSiege) return s.Name + " şu an kuşatmada ya da savaşta; bitince tekrar dene";
            if (Settlement.CurrentSettlement == s) return "Sen şehrin içindeyken halk ayaklanmaz: önce şehirden çık";
            return null;
        }

        // ---- Yakalanma (aşama sırasında) ----

        public static void Discovered(IntrigueState state, Scheme scheme, Settlement s)
        {
            state.Schemes.Remove(scheme);
            MarkAlert(state, s);
            bool incite = scheme.Kind == SchemeKind.Incite;
            string consequences = incite
                ? Expose(state, s, SettlementSchemeRules.InciteOwnerPenalty, 0, SettlementSchemeRules.InciteCaughtCrime,
                    SettlementSchemeRules.InciteCaughtTension, true)
                : Expose(state, s, SettlementSchemeRules.SabotageOwnerPenalty, 0, SettlementSchemeRules.SabotageDiscoveredCrime,
                    SettlementSchemeRules.SabotageDiscoveredTension, false);
            state.AddLog(Today, SchemeRules.Name(scheme.Kind) + " ortaya çıktı: " + s.Name);
            SchemeOutcomes.Notify(state, SchemeRules.Name(scheme.Kind) + " ortaya çıktı: " + s.Name,
                (incite ? "Elebaşlarından biri " + Tr.Locative(s.Name.ToString()) + " tutuklandı ve seni ele verdi."
                    : "Adamların " + Tr.Locative(s.Name.ToString()) + " daha hazırlık yaparken yakalandı.")
                + " Plan dağıldı.\n\nSonuç: " + consequences, PortraitHero(s), CardImages.Caught);
        }

        // ---- Harekete geç ----

        /// <summary>Zar ve sonuç. Rüşvet ödenmiş, entrika listeden çıkarılmamış olmalı (burada çıkarılır).</summary>
        public static void Execute(IntrigueState state, Scheme scheme, Hero spymaster, Settlement s, int bribeLevel)
        {
            SettlementSchemeInputs inputs = Inputs(state, spymaster, s, scheme.Kind, scheme, bribeLevel);
            int chance = CheatState.IntrigueAlwaysSucceed ? 100 : SettlementSchemeRules.Success(scheme.Kind, inputs);
            int discovery = SettlementSchemeRules.Discovery(scheme.Kind, inputs);
            state.Schemes.Remove(scheme);
            bool success = scheme.Roll100() <= chance;
            if (scheme.Kind == SchemeKind.Sabotage)
            {
                if (success) SabotageSucceeded(state, scheme, spymaster, s, discovery);
                else Failed(state, scheme, spymaster, s, inputs.Roguery);
            }
            else if (success) InciteSucceeded(state, scheme, spymaster, s, discovery);
            else Failed(state, scheme, spymaster, s, inputs.Roguery);
        }

        private static void SabotageSucceeded(IntrigueState state, Scheme scheme, Hero spymaster, Settlement s, int discovery)
        {
            SchemeMethod method = SettlementSchemeRules.IsSabotageMethod(scheme.Method) ? scheme.Method : SettlementSchemeRules.DefaultSabotageMethod;
            string text = ApplySabotage(s, method);
            spymaster?.AddSkillXp(DefaultSkills.Roguery, SuccessXp);
            bool traced = !CheatState.IntrigueNeverDiscovered && scheme.Roll100() <= SettlementSchemeRules.TraceChance(discovery);
            if (traced) text += "\n\nAma arkanda iz kaldı: " + Tr.Genitive(s.Name.ToString()) + " sahibi bunu kimin yaptığını biliyor.\n\nSonuç: "
                + Expose(state, s, SettlementSchemeRules.SabotageOwnerPenalty, 0, 0, 0, false);
            else text += "\n\nAdamların kimseye görünmeden çıktı.";
            SchemeOutcomes.Notify(state, "Sabotaj başarılı: " + s.Name, text, PortraitHero(s), CardImages.ForSabotage(method));
            state.AddLog(Today, "Sabotaj (" + SettlementSchemeRules.MethodName(method) + "): " + s.Name + (traced ? " (iz kaldı)" : ""));
        }

        private static string ApplySabotage(Settlement s, SchemeMethod method)
        {
            Town town = s.Town;
            switch (method)
            {
                case SchemeMethod.SabotageGarrison:
                {
                    int killed = KillGarrison(town.GarrisonParty?.MemberRoster);
                    return killed > 0
                        ? Tr.Genitive(s.Name.ToString()) + " kuyuları bozuldu: garnizondan " + killed + " asker öldü, kalanlar hasta ve korkulu."
                        : Tr.Genitive(s.Name.ToString()) + " kuyuları bozuldu, ama garnizonda zaten sağlıklı asker kalmamıştı.";
                }
                case SchemeMethod.SabotageWalls:
                {
                    int breaches = 0;
                    for (int i = 0; i < s.WallSectionCount && i < s.SettlementWallSectionHitPointsRatioList.Count; i++)
                    {
                        float after = SettlementSchemeRules.WallAfterSabotage(s.SettlementWallSectionHitPointsRatioList[i]);
                        if (after <= 0f) breaches++;
                        s.SetWallSectionHitPointsRatioAtIndex(i, after);
                    }
                    return Tr.Genitive(s.Name.ToString()) + " surlarının temelleri oyuldu: her bölümün sağlamlığı yarıya indi"
                        + (breaches > 0 ? "; " + breaches + " bölüm çöktü, gedik açıldı." : ".");
                }
                default:
                {
                    float before = town.FoodStocks;
                    town.FoodStocks = SettlementSchemeRules.FoodAfterSabotage(before);
                    return Tr.Genitive(s.Name.ToString()) + " ambarı gece yarısı alev aldı: erzak " + (int)Math.Round(before) + "'dan "
                        + (int)Math.Round(town.FoodStocks) + "'a düştü. Kuşatma açlıkla kısalacak.";
                }
            }
        }

        /// <summary>Kahraman olmayan sağlıklı askerlerin yaklaşık %20'si; büyük yığınlardan başlayarak. Ölen sayısını döner.</summary>
        private static int KillGarrison(TroopRoster roster)
        {
            if (roster == null) return 0;
            var stacks = new List<KeyValuePair<CharacterObject, int>>();
            int healthy = 0;
            for (int i = 0; i < roster.Count; i++)
            {
                CharacterObject troop = roster.GetCharacterAtIndex(i);
                int fit = roster.GetElementNumber(i) - roster.GetElementWoundedNumber(i);
                if (troop == null || troop.IsHero || fit <= 0) continue;
                stacks.Add(new KeyValuePair<CharacterObject, int>(troop, fit));
                healthy += fit;
            }
            int remaining = SettlementSchemeRules.GarrisonLoss(healthy);
            int killed = 0;
            foreach (KeyValuePair<CharacterObject, int> stack in stacks.OrderByDescending(x => x.Value))
            {
                if (remaining <= 0) break;
                int loss = Math.Min(Math.Min(stack.Value, remaining), Math.Max(1, (int)Math.Ceiling(stack.Value * SettlementSchemeRules.GarrisonLossPercent / 100.0)));
                roster.AddToCounts(stack.Key, -loss);
                remaining -= loss;
                killed += loss;
            }
            return killed;
        }

        private static void InciteSucceeded(IntrigueState state, Scheme scheme, Hero spymaster, Settlement s, int discovery)
        {
            Town town = s.Town;
            float before = town.Loyalty;
            town.Loyalty = SettlementSchemeRules.LoyaltyAfter(before, -SettlementSchemeRules.LoyaltyOnSuccess);
            town.Security = Math.Max(0f, town.Security - SettlementSchemeRules.SecurityOnSuccess);
            spymaster?.AddSkillXp(DefaultSkills.Roguery, SuccessXp);

            string text = "Elebaşların meydanlarda halkı " + Tr.Genitive(s.OwnerClan.Name.ToString()) + " adamlarına karşı ayağa kaldırdı: sadakat "
                + (int)Math.Round(before) + " → " + (int)Math.Round(town.Loyalty) + ", güvenlik -" + SettlementSchemeRules.SecurityOnSuccess + ".";
            // İz önce: isyan çıkarsa şehrin sahibi değişir.
            bool traced = !CheatState.IntrigueNeverDiscovered && scheme.Roll100() <= SettlementSchemeRules.TraceChance(discovery);
            string exposure = traced
                ? Expose(state, s, SettlementSchemeRules.InciteOwnerPenalty, SettlementSchemeRules.InciteRulerPenalty,
                    SettlementSchemeRules.InciteTracedCrime, SettlementSchemeRules.InciteTracedTension, true)
                : null;
            Hero portrait = PortraitHero(s);

            if (TryStartRebellion(s))
                text += "\n\n" + s.Name + " ayaklandı: halk kapıları kapattı ve kendi efendisini seçti.";
            else
                text += "\n\nHenüz isyan yok: oyun, sadakat " + RebellionThreshold() + "'in altına inip milis garnizondan güçlü olunca ayaklanma "
                    + "çıkarır. Şehir kaynıyor; her gün kendiliğinden patlayabilir.";
            text += traced ? "\n\nAma elebaşlarından biri konuştu.\n\nSonuç: " + exposure : "\n\nKimse arkasında senin olduğunu bilmiyor.";
            SchemeOutcomes.Notify(state, "Kışkırtma başarılı: " + s.Name, text, portrait, CardImages.Uprising);
            state.AddLog(Today, "Kışkırtma: " + s.Name + (traced ? " (iz kaldı)" : ""));
        }

        private static void Failed(IntrigueState state, Scheme scheme, Hero spymaster, Settlement s, int roguery)
        {
            spymaster?.AddSkillXp(DefaultSkills.Roguery, FailXp);
            MarkAlert(state, s);
            bool incite = scheme.Kind == SchemeKind.Incite;
            if (incite && s.Town != null) s.Town.Loyalty = SettlementSchemeRules.LoyaltyAfter(s.Town.Loyalty, SettlementSchemeRules.LoyaltyOnFailure);
            bool caught = !CheatState.IntrigueNeverDiscovered && scheme.Roll100() <= SchemeMath.CaughtOnFailureChance(roguery);

            string text = incite
                ? "Elebaşların tutuklandı; halk " + Tr.Genitive(s.OwnerClan?.Name.ToString() ?? s.Name.ToString()) + " yanında saf tuttu (sadakat +"
                    + SettlementSchemeRules.LoyaltyOnFailure + ")."
                : "Adamların " + Tr.Locative(s.Name.ToString()) + " iş göremeden geri çekilmek zorunda kaldı.";
            if (caught)
                text += "\n\nBiri yakalandı ve konuştu.\n\nSonuç: " + (incite
                    ? Expose(state, s, SettlementSchemeRules.InciteOwnerPenalty, SettlementSchemeRules.InciteRulerPenalty,
                        SettlementSchemeRules.InciteCaughtCrime, SettlementSchemeRules.InciteCaughtTension, true)
                    : Expose(state, s, SettlementSchemeRules.SabotageOwnerPenalty, 0, 0, 0, false));
            else text += "\n\nKimse seni suçlamıyor, ama burası artık tetikte (" + SchemeMath.AlertDays + " gün boyunca başarı -15).";
            SchemeOutcomes.Notify(state, SchemeRules.Name(scheme.Kind) + " başarısız: " + s.Name, text, PortraitHero(s), CardImages.Caught);
            state.AddLog(Today, SchemeRules.Name(scheme.Kind) + " başarısız: " + s.Name + (caught ? " (yakalandın)" : ""));
        }

        private static void MarkAlert(IntrigueState state, Settlement s)
        {
            state.CooldownUntil[s.StringId] = Today + SchemeMath.CooldownDays;
            state.AlertUntil[s.StringId] = Today + SchemeMath.AlertDays;
        }

        /// <summary>
        /// Ortaya çıkmanın bedeli: sahibiyle (ve istenirse hükümdarıyla) ilişki; barıştaki başka bir krallıksa suç puanı ve casus gerilimi (P2.5).
        /// Uygulananların özetini döner.
        /// </summary>
        private static string Expose(IntrigueState state, Settlement s, int ownerPenalty, int rulerPenalty, int crime, int tension, bool honor)
        {
            var parts = new List<string>();
            Hero owner = s.OwnerClan?.Leader;
            if (owner != null && owner != Hero.MainHero && ownerPenalty > 0)
            {
                SchemeOutcomes.Relate(owner, -ownerPenalty);
                parts.Add(owner.Name + " ile ilişki -" + ownerPenalty);
            }
            Kingdom kingdom = s.OwnerClan?.Kingdom;
            Hero ruler = kingdom?.Leader;
            if (rulerPenalty > 0 && ruler != null && ruler != owner && ruler != Hero.MainHero)
            {
                SchemeOutcomes.Relate(ruler, -rulerPenalty);
                parts.Add("hükümdarı " + ruler.Name + " ile -" + rulerPenalty);
            }
            if (kingdom != null && !kingdom.IsEliminated && kingdom != Clan.PlayerClan.Kingdom
                && !FactionManager.IsAtWarAgainstFaction(kingdom, Clan.PlayerClan.MapFaction))
            {
                if (crime > 0)
                {
                    ChangeCrimeRatingAction.Apply(kingdom, crime, true);
                    parts.Add(kingdom.Name + " krallığında suç puanı +" + crime);
                }
                if (tension > 0)
                {
                    int now = CrisisRules.AddTension(state.TensionWith(kingdom.StringId), tension);
                    state.Tension[kingdom.StringId] = now;
                    parts.Add(kingdom.Name + " ile casus gerilimi +" + tension + " (" + CrisisRules.TensionName(now) + ")");
                }
            }
            if (honor)
            {
                TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Honor, HonorOnExposure);
                parts.Add("Onur -");
            }
            return parts.Count > 0 ? string.Join(", ", parts) + "." : "kimse seni suçlayamadı.";
        }

        // ---- Oyunun kendi isyanı ----

        public static int RebellionThreshold() => Campaign.Current?.Models?.SettlementLoyaltyModel?.RebellionStartLoyaltyThreshold ?? 0;

        /// <summary>
        /// Oyunun kendi isyan denetimi (RebellionsCampaignBehavior.CheckRebellionEvent: sadakat eşiğin altında, milis garnizondan ve lord
        /// partilerinden güçlü) tutarsa oyunun kendi isyanını başlatır. Oyunda isyan kapalıysa, şehir kuşatmadaysa ya da sen içerideysen
        /// oyun gibi başlatmaz. İsyan başladıysa true.
        /// </summary>
        private static bool TryStartRebellion(Settlement s)
        {
            try
            {
                var behavior = Campaign.Current?.GetCampaignBehavior<RebellionsCampaignBehavior>();
                if (behavior == null || !s.IsTown || s.OwnerClan == null || s.OwnerClan.IsRebelClan) return false;
                if (s.Party.MapEvent != null || s.Party.SiegeEvent != null || Settlement.CurrentSettlement == s) return false;
                const BindingFlags hidden = BindingFlags.NonPublic | BindingFlags.Instance;
                FieldInfo enabled = typeof(RebellionsCampaignBehavior).GetField("_rebellionEnabled", hidden);
                if (enabled != null && !(bool)enabled.GetValue(behavior)) return false;
                MethodInfo check = typeof(RebellionsCampaignBehavior).GetMethod("CheckRebellionEvent", BindingFlags.NonPublic | BindingFlags.Static,
                    null, new[] { typeof(Settlement) }, null);
                if (check == null || !(bool)check.Invoke(null, new object[] { s })) return false;
                behavior.StartRebellionEvent(s);
                return true;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] incite rebellion failed: " + ex);
                return false;
            }
        }

        // ---- Olaylar (aşamalar arasında; numaralar CardImages.Events'te) ----

        private sealed class Context
        {
            public IntrigueState State;
            public Scheme Scheme;
            public Settlement Settlement;
            public Hero Spymaster;

            public string Name => Settlement.Name.ToString();
            public int Cost(double factor) => Math.Max(100, (int)Math.Round(Scheme.StartCost * factor));
        }

        private sealed class EventDef
        {
            public int Id;
            public string Title;
            public Func<Context, bool> Condition = _ => true;
            public Func<Context, string> Text;
            public Func<Context, List<IntrigueEvents.Option>> Options;
        }

        private static readonly List<EventDef> Events = new List<EventDef>
        {
            new EventDef
            {
                Id = 101, Title = "Rüşvetçi nöbetçi",
                Text = c => Tr.Genitive(c.Name) + " kapı nöbetçilerinden biri, adamlarına geceleri hangi kapının açık kaldığını söylemek için para istiyor.",
                Options = c => new List<IntrigueEvents.Option>
                {
                    Paid(c, 0.3, "Öde", "+8 başarı", () => c.Scheme.EventSuccess += 8),
                    Free("Tehdit et: +5 başarı, yakalanma riski +4", () => { c.Scheme.EventSuccess += 5; c.Scheme.EventDiscovery += 4; }),
                    Free("Reddet", () => { }),
                },
            },
            new EventDef
            {
                Id = 102, Title = "Elebaşı yakalandı",
                Condition = c => c.Scheme.Kind == SchemeKind.Incite,
                Text = c => "Elebaşlarından biri " + Tr.Locative(c.Name) + " meydanda nutuk atarken muhafızlara yakalandı. Henüz adını vermedi... henüz.",
                Options = c => new List<IntrigueEvents.Option>
                {
                    Paid(c, 0.5, "Kurtar", "rüşvetle zindandan kaçırt", () => { }),
                    Free("Sustur: konuşmadan ölsün (Merhamet -)", () => TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Mercy, MercyOnSilencing)),
                    Free("Kendi haline bırak: %35 ihtimalle konuşur", () =>
                    {
                        if (!CheatState.IntrigueNeverDiscovered && c.Scheme.Roll100() <= 35) Discovered(c.State, c.Scheme, c.Settlement);
                    }),
                },
            },
            new EventDef
            {
                Id = 103, Title = "Kaçakçı tüneli",
                Condition = c => c.Scheme.Kind == SchemeKind.Sabotage,
                Text = c => "Bir kaçakçı, " + Tr.Genitive(c.Name) + " surlarının altından ambarlara uzanan eski bir tünel bildiğini söylüyor.",
                Options = c => new List<IntrigueEvents.Option>
                {
                    Paid(c, 0.4, "Tüneli satın al", "+10 başarı", () => c.Scheme.EventSuccess += 10),
                    new IntrigueEvents.Option
                    {
                        Label = "Hemen vur: aşama sınırına bakılmaz, başarı +10",
                        DisabledHint = SchemeEngine.CannotExecuteReason(c.Scheme, true),
                        Apply = () => SchemeEngine.Execute(c.Scheme, 0, 10, true),
                    },
                    Free("Geç", () => { }),
                },
            },
            new EventDef
            {
                Id = 104, Title = "Ekmek kuyruğu",
                Condition = c => c.Scheme.Kind == SchemeKind.Incite,
                Text = c => Tr.Locative(c.Name) + " ekmek pahalandı; fırınların önündeki kuyruklar her sabah biraz daha öfkeli.",
                Options = c => new List<IntrigueEvents.Option>
                {
                    Free("Ateşi körükle: başarı +10, yakalanma riski +4", () => { c.Scheme.EventSuccess += 10; c.Scheme.EventDiscovery += 4; }),
                    Free("Geç", () => { }),
                },
            },
        };

        /// <summary>Uygun ve bu entrikada henüz çıkmamış bir olay seçer ve kullanıldı diye işaretler; yoksa 0.</summary>
        public static int PickEvent(Scheme scheme, Settlement s, Hero spymaster)
        {
            var context = new Context { Scheme = scheme, Settlement = s, Spymaster = spymaster };
            List<EventDef> eligible = Events.Where(e => !scheme.UsedEvents.Contains(e.Id) && e.Condition(context)).ToList();
            if (eligible.Count == 0) return 0;
            EventDef picked = eligible[scheme.Pick(eligible.Count)];
            scheme.UsedEvents.Add(picked.Id);
            return picked.Id;
        }

        /// <returns>Olay artık gösterilemiyorsa (yerleşim ya da casus başı yok) false.</returns>
        public static bool TryDescribeEvent(IntrigueState state, Scheme scheme, out string title, out string text, out List<IntrigueEvents.Option> options)
        {
            title = text = null;
            options = null;
            EventDef def = Events.FirstOrDefault(e => e.Id == scheme.PendingEvent);
            Settlement s = Find(scheme);
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (def == null || s?.Town == null || spymaster == null) return false;
            var context = new Context { State = state, Scheme = scheme, Settlement = s, Spymaster = spymaster };
            if (!def.Condition(context)) return false;
            title = SchemeRules.Name(scheme.Kind) + " (" + s.Name + "): " + def.Title;
            text = def.Text(context);
            options = def.Options(context);
            return true;
        }

        private static IntrigueEvents.Option Free(string label, Action apply) => new IntrigueEvents.Option { Label = label, Apply = apply };

        private static IntrigueEvents.Option Paid(Context c, double factor, string verb, string effect, Action apply)
        {
            int cost = c.Cost(factor);
            return new IntrigueEvents.Option
            {
                Label = verb + " (" + SchemeFactors.Money(cost) + "): " + effect,
                DisabledHint = SchemeFactors.CanAfford(cost) ? null : "Yeterli altının yok",
                Apply = () =>
                {
                    SchemeFactors.Pay(cost);
                    apply();
                },
            };
        }

        // ---- Sabotaj seçimi ----

        public static readonly SchemeMethod[] SabotageMethods = { SchemeMethod.SabotageFood, SchemeMethod.SabotageGarrison, SchemeMethod.SabotageWalls };

        public static string MethodLabel(SchemeMethod method) =>
            char.ToUpper(SettlementSchemeRules.MethodName(method)[0], Tr.Culture) + SettlementSchemeRules.MethodName(method).Substring(1)
            + ": " + SettlementSchemeRules.MethodEffect(method);
    }
}
