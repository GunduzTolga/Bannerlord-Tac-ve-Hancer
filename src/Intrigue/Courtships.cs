using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Kur, sevgili, bilgi için kur ve gebelik (P2.6, tasarım: docs/superpowers/specs/2026-10-19-p2-sevgili.md). Yalnızca metin; iki taraf
    /// da yetişkin olmak zorunda. Sen kur yaparsan başarınca sevgilin olur. Eşini gönderirsen (o kabul ederse) bilgi için kur yapar;
    /// aralarında ne olacağı ikisinin kararı (zar). Oyunun kendi gebeliği yalnızca eşten olduğu için buradaki gebelikler ayrı izlenir;
    /// doğum oyunun kendi doğum işlemiyle (HeroCreator.DeliverOffSpring) olur.
    /// </summary>
    public static class Courtships
    {
        /// <summary>Gizli ilişki ayarıyla birlikte açılıp kapanır (yetişkin teması tek ayarda).</summary>
        public static bool Enabled => RomanceSecrets.Enabled;

        /// <summary>Klanındaki hayattaki eşin; yoksa null.</summary>
        public static Hero Spouse
        {
            get
            {
                Hero spouse = Hero.MainHero.Spouse;
                return spouse != null && spouse.IsAlive && spouse.Clan == Clan.PlayerClan ? spouse : null;
            }
        }

        public static Hero AgentFor(CourtshipMode mode) => mode == CourtshipMode.Lover ? Hero.MainHero : Spouse;

        private static int Difficulty => SettingsStore.Current.IntrigueDifficulty;

        private static bool Married(Hero hero) => hero.Spouse != null && hero.Spouse.IsAlive;

        // ---- Başlatma ----

        /// <summary>null: bu kur başlatılabilir; değilse neden.</summary>
        public static string StartBlocked(IntrigueState state, Hero target, CourtshipMode mode)
        {
            int today = SchemeFactors.Today;
            if (!Enabled) return "Kurallar'dan kapalı (gizli ilişki ayarı)";
            Hero agent = AgentFor(mode);
            if (agent == null) return "Klanında hayatta bir eşin yok";
            if (agent.IsPrisoner) return mode == CourtshipMode.Lover ? "Esirken olmaz" : "Eşin esir";
            if (target == null || !target.IsAlive) return "Hayatta değil";
            if (!target.IsLord || target.Clan == null || target.Clan == Clan.PlayerClan) return "Yalnızca başka klanların soyluları";
            if (target.IsChild || !SecretRules.IsAdult(RomanceSecrets.Age(target)) || !SecretRules.IsAdult(RomanceSecrets.Age(agent)))
                return "İki taraf da yetişkin olmalı";
            if (target.IsFemale == agent.IsFemale) return "Karşı cinsten biri olmalı (oyunun evlilik sistemi gibi)";
            if (RomanceSecrets.IsKin(agent, target)) return "Akraba";
            if (target.IsPrisoner) return "Esir";
            if (mode == CourtshipMode.Lover && state.LoverOf(target.StringId) != null) return "Zaten sevgilin";
            if (state.Courtships.Any(c => c.TargetId == target.StringId)) return "Ona zaten kur yapılıyor";
            if (state.Courtships.Any(c => c.AgentId == agent.StringId))
                return mode == CourtshipMode.Lover ? "Zaten birine kur yapıyorsun" : "Eşin zaten bir görevde";
            if (state.RomanceUntil.TryGetValue(target.StringId, out int until) && today < until)
                return "Seni geri çevirdi; " + (until - today) + " gün sonra";
            if (mode == CourtshipMode.Intel && state.RomanceUntil.TryGetValue(agent.StringId, out int refused) && today < refused)
                return "Eşin bunu reddetti; " + (refused - today) + " gün sonra yeniden sorabilirsin";
            return SchemeFactors.CanAfford(RomanceRules.GiftCost) ? null : "Yeterli altının yok (" + SchemeFactors.Money(RomanceRules.GiftCost) + ")";
        }

        public static int Chance(Hero agent, Hero target) =>
            RomanceRules.SuccessChance(agent.GetSkillValue(DefaultSkills.Charm), target.GetRelation(agent), Attraction(target, agent),
                target.GetTraitLevel(DefaultTraits.Honor), Married(target), Difficulty);

        public static int Discovery(Hero agent, Hero target) =>
            RomanceRules.DiscoveryChance(agent.GetSkillValue(DefaultSkills.Roguery), Married(target), Difficulty);

        public static int SpouseAgreeChance(Hero spouse, Hero target) =>
            RomanceRules.SpouseAgreeChance(spouse.GetRelation(Hero.MainHero), spouse.GetTraitLevel(DefaultTraits.Honor),
                spouse.GetTraitLevel(DefaultTraits.Calculating), spouse.GetRelation(target));

        /// <summary>Oyunun kendi çekicilik modeli (0-100); okunamazsa 50.</summary>
        private static int Attraction(Hero target, Hero agent)
        {
            try
            {
                return SchemeMath.Clamp(Campaign.Current.Models.RomanceModel.GetAttractionValuePercentage(target, agent), 0, 100);
            }
            catch (Exception)
            {
                return 50;
            }
        }

        public static void Start(IntrigueState state, Hero target, CourtshipMode mode, bool careful)
        {
            string blocked = StartBlocked(state, target, mode);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Hero agent = AgentFor(mode);
            int today = SchemeFactors.Today;
            if (mode == CourtshipMode.Intel && state.Roll100() > SpouseAgreeChance(agent, target))
            {
                state.RomanceUntil[agent.StringId] = today + RomanceRules.RefuseCooldownDays;
                SchemeOutcomes.Relate(agent, -RomanceRules.SpouseRefuseRelation);
                SchemeOutcomes.Notify(state, "Eşin reddetti",
                    agent.Name + " teklifini dinledi ve bunu yapmayacağını söyledi. Bu onun kararı.\n\nSonuç: " + agent.Name + " ile ilişki -"
                    + RomanceRules.SpouseRefuseRelation + "; " + RomanceRules.RefuseCooldownDays + " gün yeniden sorulmaz.",
                    agent, CardImages.Courtship);
                state.AddLog(today, "Eşin bilgi için kur teklifini reddetti");
                return;
            }
            SchemeFactors.Pay(RomanceRules.GiftCost);
            state.Courtships.Add(new Courtship
            {
                Id = state.NextCourtshipId++, TargetId = target.StringId, AgentId = agent.StringId, Mode = mode, StartDay = today, Careful = careful,
            });
            state.AddLog(today, (mode == CourtshipMode.Lover ? "Kur başladı: " : "Eşin bilgi için kur yapıyor: ") + target.Name);
            MenuUI.Success((mode == CourtshipMode.Lover ? "Kur başladı: " : agent.Name + " yola çıktı: ") + target.Name
                + " (" + RomanceRules.StageCount * RomanceRules.StageDays + " gün)");
        }

        public static void Cancel(IntrigueState state, Courtship courtship)
        {
            state.Courtships.Remove(courtship);
            state.AddLog(SchemeFactors.Today, "Kur bırakıldı: " + (Hero.Find(courtship.TargetId)?.Name.ToString() ?? courtship.TargetId));
            MenuUI.Success("Kur bırakıldı");
        }

        // ---- Günlük ----

        public static void DailyTick()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;
            foreach (Courtship courtship in state.Courtships.ToList())
            {
                Hero target = Hero.Find(courtship.TargetId);
                Hero agent = Hero.Find(courtship.AgentId);
                if (target == null || agent == null || !target.IsAlive || !agent.IsAlive || agent.Clan != Clan.PlayerClan)
                {
                    state.Courtships.Remove(courtship);
                    continue;
                }
                if (target.IsPrisoner || agent.IsPrisoner) continue;
                if (++courtship.DaysIntoStage < RomanceRules.StageDays) continue;
                courtship.DaysIntoStage = 0;
                if (state.Roll100() <= Discovery(agent, target))
                {
                    Caught(state, courtship, agent, target);
                    continue;
                }
                if (++courtship.Stage >= RomanceRules.StageCount) Resolve(state, courtship, agent, target);
            }
            foreach (Pregnancy pregnancy in state.Pregnancies.Where(p => p.DueDay <= SchemeFactors.Today).ToList())
            {
                state.Pregnancies.Remove(pregnancy);
                try
                {
                    Deliver(state, pregnancy);
                }
                catch (Exception ex)
                {
                    Hero mother = Hero.Find(pregnancy.MotherId);
                    if (mother != null) mother.IsPregnant = false;
                    TaleWorlds.Library.Debug.Print("[CheatMenu] delivery failed: " + ex);
                }
            }
        }

        /// <summary>Kur yakalandı: kimse yatağa girmeden dedikodu.</summary>
        private static void Caught(IntrigueState state, Courtship courtship, Hero agent, Hero target)
        {
            state.Courtships.Remove(courtship);
            int today = SchemeFactors.Today;
            state.RomanceUntil[target.StringId] = today + RomanceRules.RefuseCooldownDays;
            var parts = new List<string>();
            Hero targetSpouse = Married(target) ? target.Spouse : null;
            if (targetSpouse != null)
            {
                Between(target, targetSpouse, -SecretRules.LoverResentment, parts);
                Between(agent, targetSpouse, -SecretRules.LoverResentment, parts);
            }
            if (courtship.Mode == CourtshipMode.Lover && Spouse != null) Between(Hero.MainHero, Spouse, -SecretRules.LoverResentment, parts);
            if (courtship.Mode == CourtshipMode.Intel)
            {
                SchemeOutcomes.Relate(target, -SecretRules.SourceTracedPenalty);
                parts.Add(target.Name + " eşini ona casus olarak gönderdiğini anladı: seninle -" + SecretRules.SourceTracedPenalty);
            }
            SchemeOutcomes.Notify(state, "Kur duyuldu: " + target.Name,
                (courtship.Mode == CourtshipMode.Lover ? "Hediyelerin ve mektupların " : agent.Name + " ile " + target.Name + " arasındaki yakınlık ")
                + "dedikodu oldu; kur yarıda kaldı." + (parts.Count > 0 ? "\n\nSonuç: " + string.Join(", ", parts) + "." : ""),
                target, CardImages.Scandal);
            state.AddLog(today, "Kur duyuldu: " + target.Name);
        }

        private static void Resolve(IntrigueState state, Courtship courtship, Hero agent, Hero target)
        {
            state.Courtships.Remove(courtship);
            int today = SchemeFactors.Today;
            bool won = state.Roll100() <= Chance(agent, target);
            if (courtship.Mode == CourtshipMode.Lover)
            {
                if (!won)
                {
                    state.RomanceUntil[target.StringId] = today + RomanceRules.RefuseCooldownDays;
                    SchemeOutcomes.Relate(target, -RomanceRules.RefuseRelation);
                    SchemeOutcomes.Notify(state, "Kur tutmadı: " + target.Name,
                        target.Name + " hediyelerini kabul etti ama kalbini değil. Sonuç: ilişki -" + RomanceRules.RefuseRelation + "; "
                        + RomanceRules.RefuseCooldownDays + " gün yeniden denenmez.", target, CardImages.Courtship);
                    state.AddLog(today, "Kur tutmadı: " + target.Name);
                    return;
                }
                state.Lovers.Add(new Lover { HeroId = target.StringId, Since = today, NextMeetingDay = today });
                SchemeOutcomes.Relate(target, RomanceRules.LoverRelation);
                SchemeOutcomes.Notify(state, "Sevgilin oldu: " + target.Name,
                    "Hediyeler, şiirler, sonra gizli bir buluşma. " + target.Name + " artık sevgilin; gerisi ikinizin arasında kaldı.\n\n"
                    + "Sonuç: ilişki +" + RomanceRules.LoverRelation + ". Entrika ekranı > Kişiler ya da Saray'dan gizli buluşmalar ayarlayabilirsin "
                    + "(her " + RomanceRules.MeetingCooldownDays + " günde bir; gebelik ve yakalanma riski var).",
                    target, CardImages.SecretAffair);
                state.AddLog(today, "Sevgilin oldu: " + target.Name);
                return;
            }

            // Bilgi için kur: görev her durumda rapor getirir; yakınlık ikisinin kararı.
            IntelReport report = SchemeOutcomes.CreateReport(target, today);
            state.Reports.RemoveAll(r => r.TargetId == target.StringId);
            state.Reports.Add(report);
            var lines = new List<string> { agent.Name + " " + target.Name + " ile haftalarca vakit geçirdi ve döndü: hakkında güncel bir raporun var." };
            if (state.Roll100() <= RomanceRules.IntelSecretChance)
            {
                Secret secret = SchemeOutcomes.FindSecret(state, target, state.Roll100, count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count));
                if (secret != null)
                {
                    RomanceSecrets.Record(state, secret);
                    lines.Add(SchemeOutcomes.SecretFoundLine(secret));
                }
            }
            SchemeOutcomes.RelateBetween(agent, target, RomanceRules.IntelSpouseRelation);
            SchemeOutcomes.Relate(target, RomanceRules.IntelPlayerRelation);
            lines.Add("Sonuç: " + agent.Name + " ile " + target.Name + " arası +" + RomanceRules.IntelSpouseRelation + ", " + target.Name
                + " seninle +" + RomanceRules.IntelPlayerRelation + ".");
            if (won)
            {
                lines.Add("Aralarında bir yakınlık oldu; ayrıntılar ikisinin arasında kaldı.");
                if (Married(target) && !state.Secrets.Any(s => s.TargetId == target.StringId && s.Kind == SecretKind.Affair && s.OtherId == agent.StringId))
                {
                    state.Secrets.Add(new Secret { TargetId = target.StringId, Kind = SecretKind.Affair, Day = today, OtherId = agent.StringId });
                    lines.Add("Evli olduğu için bu artık onun bir sırrı: Kişiler'den kullanabilirsin.");
                }
                string pregnancy = TryConceive(state, agent, target, courtship.Careful);
                if (pregnancy != null) lines.Add(pregnancy);
            }
            SchemeOutcomes.Notify(state, "Eşinin görevi bitti: " + target.Name, string.Join("\n\n", lines), target, CardImages.Courtship);
            state.AddLog(today, "Eşinin bilgi görevi bitti: " + target.Name);
        }

        // ---- Sevgili ----

        public static string MeetBlocked(IntrigueState state, Lover lover)
        {
            Hero hero = Hero.Find(lover.HeroId);
            if (hero == null || !hero.IsAlive) return "Hayatta değil";
            if (Hero.MainHero.IsPrisoner) return "Esirken olmaz";
            if (hero.IsPrisoner) return "Esir";
            int today = SchemeFactors.Today;
            return today < lover.NextMeetingDay ? (lover.NextMeetingDay - today) + " gün sonra" : null;
        }

        public static int MeetingDiscovery(Hero hero) =>
            RomanceRules.MeetingDiscovery(Hero.MainHero.GetSkillValue(DefaultSkills.Roguery), (Spouse != null ? 1 : 0) + (Married(hero) ? 1 : 0));

        /// <summary>Gebelik ihtimali olan bir buluşma mı (karşı cins, anne yaşı, oyunun doğum ayarı).</summary>
        public static bool MayConceive(IntrigueState state, Hero a, Hero b)
        {
            if (CampaignOptions.IsLifeDeathCycleDisabled || a.IsFemale == b.IsFemale) return false;
            if (state.NoChildren.Contains(a.StringId) || state.NoChildren.Contains(b.StringId)) return false;
            Hero mother = a.IsFemale ? a : b;
            return !mother.IsPregnant && RomanceRules.CanConceive(RomanceSecrets.Age(mother)) && state.Pregnancies.All(p => p.MotherId != mother.StringId);
        }

        public static void Meet(IntrigueState state, Lover lover, bool careful)
        {
            string blocked = MeetBlocked(state, lover);
            if (blocked != null) throw new InvalidOperationException(blocked);
            Hero hero = Hero.Find(lover.HeroId);
            int today = SchemeFactors.Today;
            lover.NextMeetingDay = today + RomanceRules.MeetingCooldownDays;
            SchemeOutcomes.Relate(hero, RomanceRules.MeetingRelation);
            var lines = new List<string> { "Gizli bir yerde buluştunuz; gece ikinizin arasında kaldı. İlişki +" + RomanceRules.MeetingRelation + "." };
            if (!lover.Public && state.Roll100() <= MeetingDiscovery(hero))
            {
                lover.Public = true;
                List<string> parts = RomanceSecrets.ExposeEffects(new Secret { Kind = SecretKind.Affair, TargetId = hero.StringId, OtherId = Hero.MainHero.StringId });
                lines.Add("Ama görüldünüz: ilişkiniz artık herkesin ağzında." + (parts.Count > 0 ? " Sonuç: " + string.Join(", ", parts) + "." : ""));
                state.AddLog(today, "Sevgilinle görüldün: " + hero.Name);
            }
            string pregnancy = TryConceive(state, Hero.MainHero, hero, careful);
            if (pregnancy != null) lines.Add(pregnancy);
            SchemeOutcomes.Notify(state, "Gizli buluşma: " + hero.Name, string.Join("\n\n", lines), hero, lover.Public ? CardImages.Scandal : CardImages.SecretAffair);
        }

        public static void EndAffair(IntrigueState state, Lover lover)
        {
            Hero hero = Hero.Find(lover.HeroId);
            state.Lovers.Remove(lover);
            if (hero != null && hero.IsAlive) SchemeOutcomes.Relate(hero, -RomanceRules.EndRelation);
            state.AddLog(SchemeFactors.Today, "İlişki bitti: " + (hero?.Name.ToString() ?? lover.HeroId));
            MenuUI.Success("İlişki bitti" + (hero != null ? ": " + hero.Name + " ile ilişki -" + RomanceRules.EndRelation : ""));
        }

        // ---- Gebelik ve doğum ----

        /// <summary>Yakınlıktan gebelik zarı; olduysa kart satırı, olmadıysa null.</summary>
        /// <param name="hidden">Eşinin gizli ilişkisinden: gerçek baba öğrenilene kadar kartlarda ve kronikte görünmez.</param>
        internal static string TryConceive(IntrigueState state, Hero a, Hero b, bool careful, bool hidden = false)
        {
            if (!MayConceive(state, a, b)) return null;
            if (state.Roll100() > RomanceRules.PregnancyChanceFor(careful)) return null;
            Hero mother = a.IsFemale ? a : b;
            Hero father = a.IsFemale ? b : a;
            int days = Math.Max(1, (int)Math.Round(Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays));
            mother.IsPregnant = true;
            state.Pregnancies.Add(new Pregnancy { MotherId = mother.StringId, FatherId = father.StringId, DueDay = SchemeFactors.Today + days, Hidden = hidden });
            if (hidden) return null;
            state.AddLog(SchemeFactors.Today, mother.Name + " hamile (gerçek baba: " + father.Name + ")");
            return mother.Name + " hamile kaldı; doğum yaklaşık " + days + " gün sonra.";
        }

        private static void Deliver(IntrigueState state, Pregnancy pregnancy)
        {
            Hero mother = Hero.Find(pregnancy.MotherId);
            Hero father = Hero.Find(pregnancy.FatherId);
            if (mother == null || !mother.IsAlive || father == null) return;
            mother.IsPregnant = false;
            Hero husband = Married(mother) ? mother.Spouse : null;
            // Anne evliyse çocuk kocasının çocuğu sayılır; gerçek baba bir sır olur.
            Hero official = husband != null && husband != father ? husband : father;
            bool female = state.Roll100() <= (int)Math.Round(Campaign.Current.Models.PregnancyModel.DeliveringFemaleOffspringProbability * 100f);
            Hero child = HeroCreator.DeliverOffSpring(mother, official, female);
            // Gizli gebelikte çocuk annesinin klanında kalır (eşinin sevgilisinin çocuğu senin klanına kendiliğinden gelmez).
            bool ours = pregnancy.Hidden ? mother.Clan == Clan.PlayerClan
                : mother.Clan == Clan.PlayerClan || official.Clan == Clan.PlayerClan || (father == Hero.MainHero && husband == null);
            if (ours && child.Clan != Clan.PlayerClan) child.Clan = Clan.PlayerClan;
            else if (!ours && pregnancy.Hidden && mother.Clan != null && child.Clan != mother.Clan) child.Clan = mother.Clan;
            try
            {
                // Oyunun kendi doğum olayı: kayıt defteri, bildirimler ve doğumu dinleyen diğer sistemler.
                CampaignEventDispatcher.Instance.OnGivenBirth(mother, new List<Hero> { child }, 0);
            }
            catch (Exception ex)
            {
                TaleWorlds.Library.Debug.Print("[CheatMenu] OnGivenBirth failed: " + ex);
            }

            if (pregnancy.Hidden && state.SpouseAffair?.Discovered != true)
            {
                // Gerçek baba sır: eşinin ilişkisi ortaya çıkınca söylenir (SpouseLife.Discover).
                SpouseLife.OnHiddenBirth(state, child);
                if (mother.Clan == Clan.PlayerClan)
                    SchemeOutcomes.Notify(state, "Doğum: " + child.Name, mother.Name + " bir " + (female ? "kız" : "oğlan") + " doğurdu: " + child.Name
                        + ". Çocuk senin klanında büyüyecek.", child, CardImages.Birth);
                return;
            }

            string secretLine = "";
            if (official != father)
            {
                // Sır, oyuncunun klanından olmayan tarafta tutulur (kendi ailene şantaj olmaz).
                Hero holder = mother.Clan != Clan.PlayerClan ? mother : father.Clan != Clan.PlayerClan ? father : null;
                if (holder != null)
                {
                    state.Secrets.Add(new Secret
                    {
                        TargetId = holder.StringId, Kind = SecretKind.Bastard, Day = SchemeFactors.Today,
                        OtherId = (holder == mother ? father : mother).StringId, ChildId = child.StringId,
                    });
                    secretLine = "\n\nResmî babası " + official.Name + "; gerçeği bilenler az. Bu artık " + holder.Name + " için bir sır (Kişiler)."
                        + (father == Hero.MainHero ? " İstersen sonra meşrulaştırıp klanına alabilirsin (Editörler > Aile editörü ya da " + mother.Name + " dosyası)." : "");
                }
                else secretLine = "\n\nResmî babası " + official.Name + ".";
            }
            SchemeOutcomes.Notify(state, "Doğum: " + child.Name,
                mother.Name + " bir " + (female ? "kız" : "oğlan") + " doğurdu: " + child.Name + ". Gerçek babası " + father.Name + "."
                + secretLine + "\n\n" + (child.Clan == Clan.PlayerClan ? "Çocuk senin klanında büyüyecek." : "Çocuk " + child.Clan?.Name + " klanında büyüyecek."),
                child, CardImages.Birth);
            state.AddLog(SchemeFactors.Today, "Doğum: " + child.Name + " (anne " + mother.Name + ", baba " + father.Name + ")");
        }

        // ---- Yardımcılar ----

        private static void Between(Hero a, Hero b, int delta, List<string> parts)
        {
            if (a == null || b == null || !a.IsAlive || !b.IsAlive || a == b) return;
            if (a == Hero.MainHero) SchemeOutcomes.Relate(b, delta);
            else if (b == Hero.MainHero) SchemeOutcomes.Relate(a, delta);
            else SchemeOutcomes.RelateBetween(a, b, delta);
            parts.Add(a.Name + " ile " + b.Name + " arası " + delta);
        }

        /// <summary>Süren listesi için satırlar: kurlar, sevgililer, gebelikler.</summary>
        public static IEnumerable<(string Text, string Days)> Rows(IntrigueState state)
        {
            int today = SchemeFactors.Today;
            foreach (Courtship c in state.Courtships)
            {
                Hero target = Hero.Find(c.TargetId);
                Hero agent = Hero.Find(c.AgentId);
                yield return ((c.Mode == CourtshipMode.Lover ? "Kur: " : "Eşin " + agent?.Name + " bilgi için kur yapıyor: ") + target?.Name
                    + " · " + RomanceRules.StageName(c.Stage) + " (" + (c.Stage + 1) + "/" + RomanceRules.StageCount + ")",
                    RomanceRules.DaysLeft(c.Stage, c.DaysIntoStage) + " gün kaldı");
            }
            foreach (Lover l in state.Lovers)
            {
                Hero hero = Hero.Find(l.HeroId);
                yield return ("Sevgilin: " + hero?.Name + (l.Public ? " (herkes biliyor)" : ""),
                    today >= l.NextMeetingDay ? "buluşmaya hazır" : "buluşma " + (l.NextMeetingDay - today) + " gün sonra");
            }
            foreach (HostageDeal deal in state.Hostages)
            {
                Kingdom kingdom = Hostages.KingdomOf(deal);
                if (kingdom != null) yield return (Hostages.StatusLine(state, kingdom).Replace("Rehine anlaşması:", "Rehine (" + kingdom.Name + "):"),
                    Math.Max(0, deal.Until - today) + " gün kaldı");
            }
            if (state.SpouseWatchUntil > today)
                yield return ("Casus başın eşini araştırıyor", (state.SpouseWatchUntil - today) + " gün kaldı");
            foreach (Pregnancy p in state.Pregnancies)
            {
                Hero mother = Hero.Find(p.MotherId);
                // Gizli gebelik: eşinse sıradan bir gebelik gibi görünür, başkasıysa hiç görünmez.
                bool secret = p.Hidden && state.SpouseAffair?.Discovered != true;
                if (secret && mother?.Clan != Clan.PlayerClan) continue;
                yield return (mother?.Name + " hamile" + (secret ? "" : " (gerçek baba: " + Hero.Find(p.FatherId)?.Name + ")"),
                    "doğum " + Math.Max(0, p.DueDay - today) + " gün sonra");
            }
        }
    }
}
