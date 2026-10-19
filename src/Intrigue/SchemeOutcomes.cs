using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Cheats;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Entrika sonuçlarını oyunun kendi eylemleriyle uygular (tasarım 5.5, 6). Kişilik değişimi
    /// TraitLevelingHelper.OnIncidentResolved ile (oyunun rastgele olaylarının kullandığı yol).
    /// </summary>
    public static class SchemeOutcomes
    {
        private const int MurderSuccessXp = 1500;
        private const int KidnapSuccessXp = 1000;
        private const int StrikeFailXp = 300;
        private const int SpySuccessXp = 400;

        // Kişilik XP'si (oyunda denenince ayarlanacak)
        private const int MercyOnMurder = -40;
        private const int MercyOnSilencing = -20;
        private const int HonorOnLightExposure = -40;
        private const int HonorOnHeavyExposure = -150;
        private const int MercyOnHeavyExposure = -100;
        private const int HonorOnKidnap = -20;
        private const int HonorOnKidnapExposure = -80;

        // ---- Aşama sırasında yakalanma ----

        public static void Discovered(IntrigueState state, Scheme scheme, Hero target)
        {
            state.Schemes.Remove(scheme);
            int today = SchemeFactors.Today;
            state.CooldownUntil[target.StringId] = today + SchemeMath.CooldownDays;
            state.AddLog(today, SchemeRules.Name(scheme.Kind) + " ortaya çıktı: " + target.Name);

            if (!SchemeRules.IsStrike(scheme.Kind))
            {
                int penalty = scheme.Kind == SchemeKind.Blackmail ? 15 : 10;
                Relate(target, -penalty);
                Notify(state, SchemeRules.Name(scheme.Kind) + " ortaya çıktı: " + target.Name,
                    target.Name + " peşinde adamların olduğunu fark etti. Entrika dağıldı.\n\nSonuç: " + target.Name + " ile ilişki -" + penalty + ".",
                    target);
                return;
            }

            string consequences = ApplyLightExposure(state, target, 20, scheme.FeastRisk);
            Notify(state, SchemeRules.Name(scheme.Kind) + " planı ortaya çıktı: " + target.Name,
                Tr.Dative(target.Name.ToString()) + " karşı kurduğun plan, daha harekete geçmeden ortaya çıktı. Adamların dağıldı.\n\nSonuç: " + consequences,
                target);
        }

        // ---- Suikast ----

        public static void MurderSucceeded(IntrigueState state, Scheme scheme, Hero spymaster, Hero target, int discovery, bool captive)
        {
            int trace = SchemeMath.TraceChance(discovery, captive, scheme.Method == SchemeMethod.Accident);
            bool traced = !CheatState.IntrigueNeverDiscovered && scheme.Roll100() <= trace;
            Victim victim = Victim.Of(target);

            KillCharacterAction.ApplyByMurder(target, null, true);
            spymaster?.AddSkillXp(DefaultSkills.Roguery, MurderSuccessXp);
            TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Mercy, MercyOnMurder);

            string text = victim.Name + " " + MethodText(scheme.Method) + ".";
            if (traced) text += "\n\nAma arkanda iz kaldı ve herkes bunun arkasında senin olduğunu biliyor.\n\nSonuç: "
                + ApplyHeavyExposure(state, victim, scheme.FeastRisk, false);
            else text += "\n\nKimse senden şüphelenmiyor. Vicdanın ise o kadar sessiz değil (Merhamet -).";
            Notify(state, "Suikast başarılı: " + victim.Name, text, null);
            state.AddLog(SchemeFactors.Today, "Suikast: " + victim.Name + " öldürüldü" + (traced ? " (iz kaldı)" : ""));
        }

        /// <summary>Suikast ya da kaçırma tutmadı: hedef kurtuldu (suikastta yaralı), adamların yakalanmış olabilir.</summary>
        public static void StrikeFailed(IntrigueState state, Scheme scheme, Hero spymaster, Hero target, int roguery)
        {
            int today = SchemeFactors.Today;
            bool murder = scheme.Kind == SchemeKind.Murder;
            if (murder) target.HitPoints = Math.Max(1, target.MaxHitPoints / 5);
            spymaster?.AddSkillXp(DefaultSkills.Roguery, StrikeFailXp);
            state.CooldownUntil[target.StringId] = today + SchemeMath.CooldownDays;

            bool caught = !CheatState.IntrigueNeverDiscovered && scheme.Roll100() <= SchemeMath.CaughtOnFailureChance(roguery);
            string text = murder ? target.Name + " saldırıdan yaralı kurtuldu." : target.Name + " kaçırılmaktan son anda kurtuldu.";
            if (caught) text += "\n\nAdamlarından biri yakalandı ve konuştu.\n\nSonuç: " + ApplyLightExposure(state, target, 30, scheme.FeastRisk);
            else
            {
                state.AlertUntil[target.StringId] = today + SchemeMath.AlertDays;
                text += "\n\nAdamların kaçmayı başardı; kimse seni suçlamıyor. Ama " + target.Name
                    + " artık tetikte (" + SchemeMath.AlertDays + " gün boyunca ona karşı başarı -15).";
            }
            Notify(state, SchemeRules.Name(scheme.Kind) + " başarısız: " + target.Name, text, target);
            state.AddLog(today, SchemeRules.Name(scheme.Kind) + " başarısız: " + target.Name + (caught ? " (yakalandın)" : ""));
        }

        // ---- Kaçırma ----

        public static void KidnapSucceeded(IntrigueState state, Scheme scheme, Hero spymaster, Hero target, int discovery)
        {
            int trace = SchemeMath.TraceChance(discovery, false, false);
            bool traced = !CheatState.IntrigueNeverDiscovered && scheme.Roll100() <= trace;
            Victim victim = Victim.Of(target);

            TakePrisonerAction.Apply(PartyBase.MainParty, target);
            spymaster?.AddSkillXp(DefaultSkills.Roguery, KidnapSuccessXp);
            TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Honor, HonorOnKidnap);

            string text = victim.Name + " gece yarısı çuvala konup kaçırıldı; şimdi partindeki esirler arasında. "
                + "Fidye isteyebilir, serbest bırakabilir ya da elinde tutabilirsin.";
            if (traced) text += "\n\nAma adamların iz bıraktı ve herkes kaçıranın sen olduğunu biliyor.\n\nSonuç: "
                + ApplyHeavyExposure(state, victim, scheme.FeastRisk, true);
            else text += "\n\nKimin yaptığını kimse bilmiyor (Onur -).";
            Notify(state, "Kaçırma başarılı: " + victim.Name, text, target);
            state.AddLog(SchemeFactors.Today, "Kaçırma: " + victim.Name + (traced ? " (iz kaldı)" : ""));
        }

        // ---- Şantaj ----

        public static void BlackmailSucceeded(IntrigueState state, Scheme scheme, Hero spymaster, Hero target)
        {
            int today = SchemeFactors.Today;
            state.Hooks.RemoveAll(h => h.TargetId == target.StringId);
            state.Hooks.Add(new Hook { TargetId = target.StringId, Day = today, Until = today + SchemeMath.HookDays });
            Relate(target, -SchemeMath.HookUseRelationPenalty);
            spymaster?.AddSkillXp(DefaultSkills.Roguery, SpySuccessXp);

            string text = target.Name + " sırrının ortaya çıkmasındansa sana boyun eğmeyi seçti. " + SchemeMath.HookDays
                + " gün boyunca üzerinde bir kozun var.\n\nKullanmak için: Entrika > Kozlar, ya da onunla konuşup \"özel bir konu\" aç.";
            int abandoned = state.AiPlots.RemoveAll(p => p.PlotterId == target.StringId);
            if (abandoned > 0) text += "\n\nSana karşı kurduğu entrikadan da vazgeçti.";
            Notify(state, "Şantaj tuttu: " + target.Name, text + "\n\nSonuç: " + target.Name + " ile ilişki -" + SchemeMath.HookUseRelationPenalty + ".", target);
            state.AddLog(today, "Şantaj tuttu: " + target.Name + " üzerinde koz");
        }

        public static void BlackmailFailed(IntrigueState state, Hero target)
        {
            int today = SchemeFactors.Today;
            Secret secret = SchemeFactors.BestSecret(state, target);
            if (secret != null) state.Secrets.Remove(secret);
            Relate(target, -20);
            state.CooldownUntil[target.StringId] = today + SchemeMath.CooldownDays;
            Notify(state, "Şantaj tutmadı: " + target.Name,
                target.Name + " tehdidine boyun eğmedi ve izleri örtbas etti; elindeki sır artık işe yaramıyor.\n\nSonuç: ilişki -20.", target);
            state.AddLog(today, "Şantaj tutmadı: " + target.Name);
        }

        private static string MethodText(SchemeMethod method)
        {
            switch (method)
            {
                case SchemeMethod.Poison: return "akşam yemeğinden sonra bir daha uyanmadı";
                case SchemeMethod.Accident: return "talihsiz bir kazada öldü";
                case SchemeMethod.Dagger: return "karanlık bir koridorda hançerlenerek öldürüldü";
                default: return "öldürüldü";
            }
        }

        // ---- İfşa ----

        /// <summary>Girişim ya da plan ortaya çıktı, kimse ölmedi. Uygulanan cezaların özetini döner.</summary>
        public static string ApplyLightExposure(IntrigueState state, Hero target, int clanPenalty, bool feast)
        {
            int today = SchemeFactors.Today;
            int penalty = Scale(clanPenalty, feast);
            foreach (Hero hero in Adults(target.Clan).Append(target).Distinct()) Relate(hero, -penalty);

            var parts = new List<string> { target.Name + (target.Clan != null ? " ve klanıyla" : " ile") + " ilişki -" + penalty };
            Hero ruler = target.Clan?.Kingdom?.Leader;
            if (ruler != null && ruler != target && ruler != Hero.MainHero)
            {
                int rulerPenalty = Scale(10, feast);
                Relate(ruler, -rulerPenalty);
                parts.Add("hükümdarı " + ruler.Name + " ile -" + rulerPenalty);
            }
            if (AddCrime(target.MapFaction, 25)) parts.Add(target.MapFaction.Name + " krallığında suç puanı +25");
            TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Honor, HonorOnLightExposure);
            parts.Add("Onur -");

            state.CooldownUntil[target.StringId] = today + SchemeMath.CooldownDays;
            state.AlertUntil[target.StringId] = today + SchemeMath.AlertDays;
            return string.Join(", ", parts) + ".";
        }

        /// <summary>Cinayet ya da kaçırma ortaya çıktı (tasarım 5.5 "ağır ifşa"; kaçırmada cezalar daha hafif, kan davası yok).</summary>
        private static string ApplyHeavyExposure(IntrigueState state, Victim victim, bool feast, bool kidnap)
        {
            var parts = new List<string>();
            int clanPenalty = Scale(kidnap ? 35 : 50, feast);
            foreach (Hero hero in victim.ClanAdults.Where(h => h.IsAlive)) Relate(hero, -clanPenalty);
            if (victim.ClanAdults.Count > 0) parts.Add(victim.ClanName + " klanıyla ilişki -" + clanPenalty);

            Kingdom mine = Clan.PlayerClan.Kingdom;
            if (victim.Kingdom != null)
            {
                int kingdomPenalty = Scale(kidnap ? 15 : 20, feast);
                foreach (Hero leader in ClanLeaders(victim.Kingdom).Where(l => l.Clan != victim.Clan)) Relate(leader, -kingdomPenalty);
                parts.Add(victim.Kingdom.Name + " lordlarıyla -" + kingdomPenalty);
            }
            if (mine != null)
            {
                int honorPenalty = Scale(kidnap ? 5 : 10, feast);
                foreach (Hero leader in ClanLeaders(mine).Where(l => l.Clan != victim.Clan && l.GetTraitLevel(DefaultTraits.Honor) >= 1))
                    Relate(leader, -honorPenalty);
                parts.Add("krallığındaki onurlu lordlarla -" + honorPenalty);
            }
            int crime = kidnap ? 40 : 60;
            if (AddCrime(victim.Faction, crime)) parts.Add(victim.Faction.Name + " krallığında suç puanı +" + crime);
            if (kidnap)
            {
                TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Honor, HonorOnKidnapExposure);
                parts.Add("Onur --");
            }
            else
            {
                TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Honor, HonorOnHeavyExposure);
                TraitLevelingHelper.OnIncidentResolved(DefaultTraits.Mercy, MercyOnHeavyExposure);
                parts.Add("Onur ve Merhamet --");
                int today = SchemeFactors.Today;
                foreach (Hero relative in victim.Relatives) state.FeudUntil[relative.StringId] = today + SchemeMath.FeudDays;
                if (victim.Relatives.Count > 0)
                    parts.Add("kan davası: " + victim.Relatives.Count + " yakını " + SchemeMath.FeudDays / 84 + " yıl boyunca intikam peşinde");
            }

            if (victim.IsRuler && victim.Kingdom != null && victim.Kingdom != mine && (mine == null || Hero.MainHero.IsKingdomLeader))
            {
                IFaction me = Clan.PlayerClan.MapFaction;
                if (!FactionManager.IsAtWarAgainstFaction(me, victim.Kingdom))
                {
                    DeclareWarAction.ApplyByPlayerHostility(me, victim.Kingdom);
                    parts.Add(victim.Kingdom.Name + " sana savaş açtı");
                }
            }
            else if (victim.Kingdom != null && victim.Kingdom == mine && !Hero.MainHero.IsKingdomLeader)
            {
                if (Clan.PlayerClan.IsUnderMercenaryService)
                {
                    ChangeKingdomAction.ApplyByLeaveKingdomAsMercenary(Clan.PlayerClan, true);
                    parts.Add("paralı asker sözleşmen feshedildi");
                }
                else
                {
                    state.LiegeDemand = new LiegeDemand
                    {
                        KingdomId = mine.StringId,
                        VictimName = victim.Name,
                        Gold = SchemeMath.BloodPriceGold(victim.Tier) / (kidnap ? 2 : 1),
                        Influence = SchemeMath.BloodPriceInfluence,
                        Kidnap = kidnap,
                    };
                    parts.Add("hükümdarın hesap soracak");
                }
            }
            return string.Join(", ", parts) + ".";
        }

        // ---- Hükümdar hesap soruyor ----

        public static bool LiegeDemandStillValid(LiegeDemand demand) =>
            Clan.PlayerClan.Kingdom?.StringId == demand.KingdomId && !Hero.MainHero.IsKingdomLeader;

        public static void PayBloodPrice(IntrigueState state)
        {
            LiegeDemand demand = state.LiegeDemand;
            SchemeFactors.Pay(demand.Gold);
            ChangeClanInfluenceAction.Apply(Clan.PlayerClan, -demand.Influence);
            Hero ruler = Clan.PlayerClan.Kingdom?.Leader;
            if (ruler != null) Relate(ruler, -10);
            state.LiegeDemand = null;
            Notify(state, "Kan bedeli ödendi", "Hükümdar kan bedelini kabul etti; ama bu iş unutulmayacak.\n\nSonuç: "
                + SchemeFactors.Money(demand.Gold) + " ve " + demand.Influence + " etki ödendi, hükümdarla ilişki -10.", ruler);
            state.AddLog(SchemeFactors.Today, "Kan bedeli ödendi: " + demand.VictimName);
        }

        public static void DenyMurder(IntrigueState state)
        {
            LiegeDemand demand = state.LiegeDemand;
            Hero ruler = Clan.PlayerClan.Kingdom?.Leader;
            state.LiegeDemand = null;
            if (state.Roll100() <= SchemeMath.PersuadeChance(Hero.MainHero.GetSkillValue(DefaultSkills.Charm)))
            {
                if (ruler != null) Relate(ruler, -5);
                Notify(state, "Hükümdar sana inandı", "Ustaca bir savunma yaptın; hükümdar şimdilik sana inanıyor.\n\nSonuç: hükümdarla ilişki -5.", ruler);
                state.AddLog(SchemeFactors.Today, "İnkâr işe yaradı: " + demand.VictimName);
                return;
            }
            if (ruler != null) Relate(ruler, -20);
            ChangeKingdomAction.ApplyByLeaveKingdom(Clan.PlayerClan, true);
            Notify(state, "Krallıktan sürüldün", "Hükümdar yalanlarına inanmadı ve seni krallıktan sürdü.\n\nSonuç: hükümdarla ilişki -20.", ruler);
            state.AddLog(SchemeFactors.Today, "İnkâr işe yaramadı, sürüldün: " + demand.VictimName);
        }

        public static void LeaveOverMurder(IntrigueState state)
        {
            LiegeDemand demand = state.LiegeDemand;
            state.LiegeDemand = null;
            ChangeKingdomAction.ApplyByLeaveKingdom(Clan.PlayerClan, true);
            Notify(state, "Krallıktan ayrıldın", "Hesap vermek yerine krallıktan ayrılmayı seçtin.", null);
            state.AddLog(SchemeFactors.Today, "Kan davası yüzünden krallıktan ayrıldın: " + demand.VictimName);
        }

        // ---- Casusluk sonucu ----

        public static void SpySucceeded(IntrigueState state, Scheme scheme, Hero spymaster, Hero target)
        {
            int today = SchemeFactors.Today;
            IntelReport report = CreateReport(target, today);
            state.Reports.RemoveAll(r => r.TargetId == target.StringId);
            state.Reports.Add(report);
            spymaster?.AddSkillXp(DefaultSkills.Roguery, SpySuccessXp);

            string secretLine;
            if (!target.IsLord) secretLine = "Sırlar yalnızca lordlarda aranır.";
            else if (scheme.Roll100() > SchemeMath.SecretChance(SchemeFactors.Roguery(spymaster), scheme.Phase))
                secretLine = "Bir sır bulunamadı (belki vardır, belki yoktur).";
            else
            {
                SecretKind? kind = FindSecret(state, scheme, target);
                if (kind == null) secretLine = "Görünüşe göre temiz biri: kayda değer bir sırrı yok.";
                else
                {
                    state.Secrets.Add(new Secret { TargetId = target.StringId, Kind = kind.Value, Day = today });
                    secretLine = "SIR BULUNDU: " + SecretRules.Name(kind.Value) + " - " + SecretRules.Description(kind.Value)
                        + "\n(Entrika > İstihbarat raporları'ndan ifşa edebilirsin.)";
                }
            }

            Notify(state, "Casusluk raporu: " + target.Name, ReportText(report) + "\n\n" + secretLine, target);
            state.AddLog(today, "Casusluk raporu: " + target.Name);
        }

        public static void SpyFailed(IntrigueState state, Hero target)
        {
            Notify(state, "Casusluk sonuçsuz: " + target.Name, "Adamların işe yarar bir şey öğrenemeden geri döndü.", target);
            state.AddLog(SchemeFactors.Today, "Casusluk sonuçsuz: " + target.Name);
        }

        public static IntelReport CreateReport(Hero target, int today)
        {
            Hero ruler = target.Clan?.Kingdom?.Leader;
            int troops = target.PartyBelongedTo?.MemberRoster.TotalManCount ?? 0;
            return new IntelReport
            {
                TargetId = target.StringId,
                TargetName = target.Name.ToString(),
                Day = today,
                Location = MapInfo.Where(target),
                Party = target.IsPrisoner ? "esir" : troops > 0 ? troops + " kişilik bir partide" : "partisi yok",
                Gold = "yaklaşık " + SchemeFactors.Money((int)Math.Round(target.Gold / 1000.0) * 1000),
                Relations = (ruler != null && ruler != target ? "hükümdarı " + ruler.Name + " ile " + target.GetRelation(ruler) + ", " : "")
                    + "seninle " + Hero.MainHero.GetRelation(target),
                Traits = SchemeFactors.TraitsText(target),
            };
        }

        public static string ReportText(IntelReport report) =>
            "Gün " + report.Day + " itibarıyla:\n"
            + "Konum: " + report.Location + "\n"
            + "Parti: " + report.Party + "\n"
            + "Altın: " + report.Gold + "\n"
            + "İlişkiler: " + report.Relations + "\n"
            + "Kişilik: " + report.Traits;

        /// <summary>Lordun gerçek kişiliğinden ve durumundan türeyebilecek, henüz bilinmeyen bir sır (tasarım 6.2).</summary>
        private static SecretKind? FindSecret(IntrigueState state, Scheme scheme, Hero target)
        {
            var known = new HashSet<SecretKind>(state.Secrets.Where(s => s.TargetId == target.StringId).Select(s => s.Kind));
            Hero ruler = target.Clan?.Kingdom?.Leader;
            var candidates = new List<SecretKind>();
            void Consider(SecretKind kind, bool condition)
            {
                if (condition && !known.Contains(kind)) candidates.Add(kind);
            }

            Consider(SecretKind.Embezzlement, target.GetTraitLevel(DefaultTraits.Generosity) <= -1);
            Consider(SecretKind.TreasonPlot, target.IsClanLeader && ruler != null && ruler != target && target.GetRelation(ruler) <= -10);
            Consider(SecretKind.Bribery, target.GetTraitLevel(DefaultTraits.Honor) <= -1);
            Consider(SecretKind.Cruelty, target.GetTraitLevel(DefaultTraits.Mercy) <= -1);
            Consider(SecretKind.Cowardice, target.GetTraitLevel(DefaultTraits.Valor) <= -1);
            Consider(SecretKind.Debt, target.Gold < SecretRules.DebtGoldThreshold);
            Consider(SecretKind.Smuggling, target.GetSkillValue(DefaultSkills.Roguery) >= SecretRules.SmugglingRoguery);
            if (target.Spouse != null && !known.Contains(SecretKind.Affair) && scheme.Roll100() <= SecretRules.AffairChance)
                candidates.Add(SecretKind.Affair);

            return candidates.Count == 0 ? (SecretKind?)null : candidates[scheme.Pick(candidates.Count)];
        }

        // ---- Sır ifşası ----

        public static void ExposeSecret(IntrigueState state, Secret secret)
        {
            Hero target = Hero.Find(secret.TargetId);
            if (target == null || !target.IsAlive) throw new InvalidOperationException("Hedef artık yaşamıyor");
            if (secret.Exposed) throw new InvalidOperationException("Bu sır zaten ifşa edildi");

            int penalty = SecretRules.ExposurePenalty(secret.Kind);
            var parts = new List<string>();
            if (secret.Kind == SecretKind.Affair)
            {
                Hero spouse = target.Spouse;
                if (spouse != null && spouse.IsAlive)
                {
                    RelateBetween(target, spouse, -penalty);
                    parts.Add("eşi " + spouse.Name + " ile ilişkisi -" + penalty);
                    Hero inLaw = spouse.Clan?.Leader;
                    if (inLaw != null && inLaw != spouse && inLaw != target && inLaw.IsAlive)
                    {
                        RelateBetween(target, inLaw, -15);
                        parts.Add(inLaw.Name + " ile -15");
                    }
                }
            }
            else
            {
                Hero ruler = target.Clan?.Kingdom?.Leader;
                if (ruler != null && ruler != target)
                {
                    RelateBetween(target, ruler, -penalty);
                    parts.Add("hükümdarı " + ruler.Name + " ile ilişkisi -" + penalty);
                }
                else
                {
                    IEnumerable<Hero> peers = ruler == target ? ClanLeaders(target.Clan.Kingdom).Where(l => l.Clan != target.Clan)
                        : Adults(target.Clan).Where(h => h != target);
                    int half = Math.Max(1, penalty / 2);
                    foreach (Hero peer in peers.ToList()) RelateBetween(target, peer, -half);
                    parts.Add((ruler == target ? "lordlarıyla" : "klanıyla") + " ilişkisi -" + half);
                }
                if (secret.Kind == SecretKind.Cruelty && target.Clan?.Kingdom != null)
                {
                    foreach (Hero leader in ClanLeaders(target.Clan.Kingdom)
                                 .Where(l => l != target && l.GetTraitLevel(DefaultTraits.Honor) >= 1).ToList())
                        RelateBetween(target, leader, -5);
                    parts.Add("onurlu lordlarla -5");
                }
            }

            secret.Exposed = true;
            if (state.Roll100() <= SecretRules.SourceTracedChance)
            {
                Relate(target, -SecretRules.SourceTracedPenalty);
                parts.Add("kaynağın sen olduğun anlaşıldı: seninle -" + SecretRules.SourceTracedPenalty);
            }
            Notify(state, "Sır ifşa edildi: " + target.Name,
                SecretRules.Name(secret.Kind) + " herkesin ağzında.\n\nSonuç: " + (parts.Count > 0 ? string.Join(", ", parts) : "kimse umursamadı") + ".",
                target);
            state.AddLog(SchemeFactors.Today, "Sır ifşa edildi: " + target.Name + " (" + SecretRules.Name(secret.Kind) + ")");
        }

        // ---- Yardımcılar ----

        public static void Notify(IntrigueState state, string title, string text, Hero hero) =>
            state.Notices.Add(new Notice { Title = title, Text = text, HeroId = hero?.StringId });

        public static void Relate(Hero hero, int delta)
        {
            if (hero == null || hero == Hero.MainHero || !hero.IsAlive || delta == 0) return;
            ChangeRelationAction.ApplyPlayerRelation(hero, delta, false, false);
        }

        private static void RelateBetween(Hero a, Hero b, int delta) =>
            ChangeRelationAction.ApplyRelationChangeBetweenHeroes(a, b, delta, false);

        private static IEnumerable<Hero> Adults(Clan clan) =>
            clan == null ? Enumerable.Empty<Hero>() : clan.Heroes.Where(h => h.IsAlive && !h.IsChild && h != Hero.MainHero);

        private static IEnumerable<Hero> ClanLeaders(Kingdom kingdom) =>
            kingdom.Clans.Where(c => c != Clan.PlayerClan && !c.IsEliminated && c.Leader != null && c.Leader.IsAlive).Select(c => c.Leader);

        /// <returns>Suç puanı eklendiyse true (yalnızca başka bir krallıkta).</returns>
        private static bool AddCrime(IFaction faction, int amount)
        {
            if (!(faction is Kingdom kingdom) || kingdom == Clan.PlayerClan.Kingdom) return false;
            ChangeCrimeRatingAction.Apply(kingdom, amount, true);
            return true;
        }

        private static int Scale(int penalty, bool feast) => feast ? (int)Math.Round(penalty * 1.5) : penalty;

        /// <summary>Öldürmeden önce alınan anlık görüntü: ölümden sonra klan/krallık bilgisi değişebilir.</summary>
        private sealed class Victim
        {
            public string Name;
            public string ClanName;
            public Clan Clan;
            public List<Hero> ClanAdults;
            public Kingdom Kingdom;
            public IFaction Faction;
            public bool IsRuler;
            public int Tier;
            /// <summary>Kan davası adayları: yetişkin eş, çocuklar, anne, baba, kardeşler (oyuncu klanı hariç).</summary>
            public List<Hero> Relatives;

            public static Victim Of(Hero hero) => new Victim
            {
                Name = hero.Name.ToString(),
                ClanName = hero.Clan?.Name.ToString() ?? "",
                Clan = hero.Clan,
                ClanAdults = Adults(hero.Clan).Where(h => h != hero).ToList(),
                Kingdom = hero.Clan?.Kingdom,
                Faction = hero.MapFaction,
                IsRuler = hero.IsKingdomLeader,
                Tier = SchemeFactors.TargetTier(hero),
                Relatives = new[] { hero.Spouse, hero.Father, hero.Mother }.Concat(hero.Children).Concat(hero.Siblings)
                    .Where(h => h != null && h.IsAlive && !h.IsChild && h.Clan != Clan.PlayerClan && h != Hero.MainHero)
                    .Distinct().ToList(),
            };
        }
    }
}
