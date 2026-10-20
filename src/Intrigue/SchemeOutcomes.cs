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
            string agentLost = SpymasterMissions.LoseAgent(state);

            if (!SchemeRules.IsStrike(scheme.Kind))
            {
                int penalty = scheme.Kind == SchemeKind.Blackmail ? 15 : 10;
                Relate(target, -penalty);
                Notify(state, SchemeRules.Name(scheme.Kind) + " ortaya çıktı: " + target.Name,
                    target.Name + " peşinde adamların olduğunu fark etti. Entrika dağıldı.\n\nSonuç: " + target.Name + " ile ilişki -" + penalty + "." + agentLost,
                    target, CardImages.Caught);
                return;
            }

            string consequences = ApplyLightExposure(state, target, 20, scheme.FeastRisk);
            Notify(state, SchemeRules.Name(scheme.Kind) + " planı ortaya çıktı: " + target.Name,
                Tr.Dative(target.Name.ToString()) + " karşı kurduğun plan, daha harekete geçmeden ortaya çıktı. Adamların dağıldı.\n\nSonuç: " + consequences + agentLost,
                target, CardImages.Caught);
        }

        // ---- Suikast ----

        public static void MurderSucceeded(IntrigueState state, Scheme scheme, Hero spymaster, Hero target, int discovery, bool captive)
        {
            if (!SchemeFactors.CanBeMurdered(target))
            {
                Notify(state, "Suikast olmadı: " + target.Name, "Bu kampanyada cinayetle ölüm kapalı (Doğum ve Ölüm ayarı); plan dağıldı.", target);
                return;
            }
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
            Notify(state, "Suikast başarılı: " + victim.Name, text, null, CardImages.ForMurderMethod(scheme.Method));
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
            Notify(state, SchemeRules.Name(scheme.Kind) + " başarısız: " + target.Name, text, target, CardImages.Caught);
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
            Notify(state, "Kaçırma başarılı: " + victim.Name, text, target, CardImages.Kidnap);
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
                + " gün boyunca üzerinde bir kozun var.\n\nKullanmak için: Entrika ekranı > Kişiler, ya da onunla konuşup \"özel bir konu\" aç.";
            int abandoned = state.AiPlots.RemoveAll(p => p.PlotterId == target.StringId);
            if (abandoned > 0) text += "\n\nSana karşı kurduğu entrikadan da vazgeçti.";
            Notify(state, "Şantaj tuttu: " + target.Name, text + "\n\nSonuç: " + target.Name + " ile ilişki -" + SchemeMath.HookUseRelationPenalty + ".", target,
                CardImages.Blackmail);
            state.AddLog(today, "Şantaj tuttu: " + target.Name + " üzerinde koz");
        }

        public static void BlackmailFailed(IntrigueState state, Hero target)
        {
            int today = SchemeFactors.Today;
            Secret secret = SchemeFactors.BestSecret(state, target);
            if (secret != null)
                foreach (Secret linked in RomanceSecrets.Linked(state, secret)) state.Secrets.Remove(linked);
            Relate(target, -20);
            state.CooldownUntil[target.StringId] = today + SchemeMath.CooldownDays;
            Notify(state, "Şantaj tutmadı: " + target.Name,
                target.Name + " tehdidine boyun eğmedi ve izleri örtbas etti; elindeki sır artık işe yaramıyor.\n\nSonuç: ilişki -20.", target, CardImages.Exposed);
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
                    if (Bundle.DiplomacyCompat.EndNonAggressionPact(me as Kingdom, victim.Kingdom))
                        parts.Add("saldırmazlık anlaşmanız bozuldu");
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
            Notify(state, "Krallıktan ayrıldın", "Hesap vermek yerine krallıktan ayrılmayı seçtin.", null, CardImages.LiegeDemand);
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
            Secret found = null;
            if (!target.IsLord) secretLine = "Sırlar yalnızca lordlarda aranır.";
            else if (scheme.Roll100() > SchemeMath.SecretChance(SchemeFactors.Roguery(spymaster), scheme.Phase))
                secretLine = "Bir sır bulunamadı (belki vardır, belki yoktur).";
            else
            {
                found = FindSecret(state, scheme, target);
                if (found == null) secretLine = "Görünüşe göre temiz biri: kayda değer bir sırrı yok.";
                else
                {
                    RomanceSecrets.Record(state, found);
                    secretLine = SecretFoundLine(found) + "\n(Entrika ekranı > Kişiler'den ifşa edebilir ya da şantaja çevirebilirsin.)";
                }
            }

            Notify(state, "Casusluk raporu: " + target.Name, ReportText(report) + "\n\n" + secretLine, target,
                found == null ? CardImages.Spy : SecretImage(found));
            state.AddLog(today, "Casusluk raporu: " + target.Name);
        }

        public static void SpyFailed(IntrigueState state, Hero target)
        {
            Notify(state, "Casusluk sonuçsuz: " + target.Name, "Adamların işe yarar bir şey öğrenemeden geri döndü.", target, CardImages.Spy);
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
        private static Secret FindSecret(IntrigueState state, Scheme scheme, Hero target) =>
            FindSecret(state, target, scheme.Roll100, scheme.Pick);

        /// <summary>"SIR BULUNDU: ad - isimli açıklama".</summary>
        public static string SecretFoundLine(Secret secret) =>
            "SIR BULUNDU: " + SecretRules.Name(secret.Kind) + " - " + RomanceSecrets.Describe(secret);

        public static string SecretImage(Secret secret) => SecretRules.IsRomantic(secret.Kind) ? CardImages.SecretAffair : CardImages.SecretFound;

        /// <summary>
        /// Hedefin gerçek kişiliğinden ve durumundan çıkan, henüz bilinmeyen bir sır (kaydedilmemiş); yoksa null.
        /// Zarlar çağıranın tohumundan (entrika ya da sorgu): kaydı yükleyip tekrar denemek sonucu değiştirmez.
        /// Kaydetmek için RomanceSecrets.Record (romantik sırlar iki tarafa da yazılır).
        /// </summary>
        public static Secret FindSecret(IntrigueState state, Hero target, Func<int> roll100, Func<int, int> pick)
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
            RomanceSecrets.Consider(state, target, known, roll100, candidates);
            if (candidates.Count == 0) return null;

            var secret = new Secret { TargetId = target.StringId, Kind = candidates[pick(candidates.Count)], Day = SchemeFactors.Today };
            RomanceSecrets.Fill(state, secret, target, pick);
            return secret;
        }

        // ---- Sır ifşası ----

        public static void ExposeSecret(IntrigueState state, Secret secret)
        {
            Hero target = Hero.Find(secret.TargetId);
            if (target == null || !target.IsAlive) throw new InvalidOperationException("Hedef artık yaşamıyor");
            if (secret.Exposed) throw new InvalidOperationException("Bu sır zaten ifşa edildi");

            int penalty = SecretRules.ExposurePenalty(secret.Kind);
            var parts = new List<string>();
            bool romance = SecretRules.IsRomantic(secret.Kind) && secret.OtherId != null;
            if (romance)
            {
                parts.AddRange(RomanceSecrets.ExposeEffects(secret));
            }
            else if (secret.Kind == SecretKind.Affair)
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

            foreach (Secret linked in RomanceSecrets.Linked(state, secret)) linked.Exposed = true;
            if (state.Roll100() <= SecretRules.SourceTracedChance)
            {
                Relate(target, -SecretRules.SourceTracedPenalty);
                Hero other = romance ? Hero.Find(secret.OtherId) : null;
                if (other != null && other.IsAlive) Relate(other, -SecretRules.SourceTracedPenalty);
                parts.Add("kaynağın sen olduğun anlaşıldı: " + (other != null ? "ikisiyle de" : "seninle") + " -" + SecretRules.SourceTracedPenalty);
            }
            Notify(state, (romance ? "Skandal: " : "Sır ifşa edildi: ") + target.Name,
                (romance ? RomanceSecrets.Describe(secret) + " Artık bütün saray konuşuyor." : SecretRules.Name(secret.Kind) + " herkesin ağzında.")
                + "\n\nSonuç: " + (parts.Count > 0 ? string.Join(", ", parts) : "kimse umursamadı") + ".",
                target, romance ? CardImages.Scandal : CardImages.Exposed);
            state.AddLog(SchemeFactors.Today, "Sır ifşa edildi: " + target.Name + " (" + SecretRules.Name(secret.Kind) + ")");
        }

        // ---- Yardımcılar ----

        /// <param name="image">Olay kartı resmi (CardImages); null ise kişinin portresi gösterilir.</param>
        public static void Notify(IntrigueState state, string title, string text, Hero hero, string image = null) =>
            state.Notices.Add(new Notice { Title = title, Text = text, HeroId = hero?.StringId, Image = image });

        public static void Relate(Hero hero, int delta)
        {
            if (hero == null || hero == Hero.MainHero || !hero.IsAlive || delta == 0) return;
            ChangeRelationAction.ApplyPlayerRelation(hero, delta, false, false);
        }

        internal static void RelateBetween(Hero a, Hero b, int delta) =>
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
