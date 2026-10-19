using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Divan: saray hekimi (P2.13, tasarım: docs/superpowers/specs/2026-10-19-p2-divan-hekim.md). Klanından birini hekim yaparsın:
    /// oyunun kendi ölüm modelleri üzerinden klanında doğumda ve yaşlılıktan ölümü azaltır, sana karşı başarılı bir suikastte kurbanı
    /// kurtarabilir, eşinin gizli gebeliğini fark edebilir; sağlık raporu ve yaşlılık uyarısı verir. Medicine'i 30'un altındaysa şarlatandır.
    /// </summary>
    public static class Physician
    {
        public const string Seat = "hekimin";
        private static int Today => SchemeFactors.Today;

        // ---- Koltuk ----

        public static Hero Current(IntrigueState state)
        {
            if (state?.PhysicianId == null) return null;
            Hero hero = Hero.Find(state.PhysicianId);
            return hero != null && hero.IsAlive && hero.Clan == Clan.PlayerClan ? hero : null;
        }

        /// <summary>Koltukta ve çalışabiliyorsa hekim; değilse null.</summary>
        public static Hero Working(IntrigueState state)
        {
            Hero hero = Current(state);
            return hero != null && SchemeFactors.SpymasterBusyReason(hero) == null ? hero : null;
        }

        public static int Medicine(Hero hero) => hero?.GetSkillValue(DefaultSkills.Medicine) ?? 0;

        public static IEnumerable<Hero> Candidates(IntrigueState state) => Council.Candidates(state, Seat);

        public static string Label(Hero hero)
        {
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            int medicine = Medicine(hero);
            return hero.Name + " - Medicine " + medicine + (PhysicianRules.IsQuack(medicine) ? " (şarlatan)" : "")
                + ", ömür -%" + Math.Max(0, PhysicianRules.LongevityPercent(medicine)) + (busy != null ? "  [" + busy + "]" : "");
        }

        /// <summary>null: hekim çalışıyor; değilse neden.</summary>
        public static string Blocked(IntrigueState state)
        {
            Hero hero = Current(state);
            if (hero == null) return "Hekimin yok";
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            return busy != null ? hero.Name + " şu an " + busy + "; sarayda değil" : null;
        }

        public static void Assign(IntrigueState state, Hero hero)
        {
            Council.EnsureFree(state, hero, Seat);
            string busy = SchemeFactors.SpymasterBusyReason(hero);
            if (busy != null) throw new InvalidOperationException(hero.Name + " şu an " + busy + "; hekim olamaz");
            state.PhysicianId = hero.StringId;
            state.AddLog(Today, hero.Name + " saray hekimi oldu");
            MenuUI.Success(hero.Name + " artık saray hekimin" + (PhysicianRules.IsQuack(Medicine(hero)) ? " (dikkat: Medicine'i düşük, şarlatan)" : ""));
        }

        public static void Dismiss(IntrigueState state)
        {
            if (state.PhysicianId == null) throw new InvalidOperationException("Hekimin yok");
            Hero old = Current(state);
            state.PhysicianId = null;
            state.AddLog(Today, (old?.Name.ToString() ?? "Hekim") + " görevden alındı");
            MenuUI.Success("Hekim görevden alındı");
        }

        /// <summary>Koltuk kartında: koruma özeti.</summary>
        public static string ProtectionText(Hero hero)
        {
            int medicine = Medicine(hero);
            if (PhysicianRules.IsQuack(medicine))
                return "Şarlatan: doğumda ölüm +%" + -PhysicianRules.LaborPercent(medicine) + ", yaşlılık +%" + -PhysicianRules.LongevityPercent(medicine);
            return "Doğumda ölüm -%" + PhysicianRules.LaborPercent(medicine) + ", yaşlılık -%" + PhysicianRules.LongevityPercent(medicine)
                + ", suikastte kurtarma %" + PhysicianRules.PoisonSaveChance(medicine);
        }

        // ---- Oyunun modellerine bağlanan etkiler ----

        private static IntrigueState State => IntrigueBehavior.Current?.State;

        private static bool InPlayerClan(Hero hero) => hero != null && Clan.PlayerClan != null && hero.Clan == Clan.PlayerClan;

        /// <summary>Oyunun günlük yaşlılıktan ölüm olasılığı (HeroDeathProbabilityCalculationModel): klanındakiler için hekimin etkisiyle.</summary>
        public static float DeathProbability(Hero hero, float probability)
        {
            if (probability <= 0f || !InPlayerClan(hero)) return probability;
            Hero physician = Working(State);
            return physician == null ? probability : PhysicianRules.Apply(probability, PhysicianRules.LongevityPercent(Medicine(physician)));
        }

        // Oyun doğumda önce doğum olayını yayınlıyor, sonra annenin ölüm zarını atıyor (oyunda denenecek): olay anneyi bildirir,
        // modelin "doğumda ölüm" değeri aynı anda okunursa o anneye göre hesaplanır.
        private static Hero _lastMother;
        private static CampaignTime _lastBirth;

        public static void OnGivenBirth(Hero mother)
        {
            _lastMother = mother;
            _lastBirth = CampaignTime.Now;
            IntrigueState state = State;
            if (state != null && mother != null) state.Conceived.Remove(mother.StringId);
        }

        /// <summary>Oyunun doğumda ölüm olasılığı (PregnancyModel): az önce doğuran klanındaki bir anneyse hekimin etkisiyle.</summary>
        public static float LaborMortality(float probability)
        {
            Hero mother = _lastMother;
            bool fresh = mother != null && _lastBirth == CampaignTime.Now;
            _lastMother = null;
            if (!fresh || probability <= 0f || !InPlayerClan(mother)) return probability;
            Hero physician = Working(State);
            return physician == null ? probability : PhysicianRules.Apply(probability, PhysicianRules.LaborPercent(Medicine(physician)));
        }

        public static void OnChildConceived(Hero mother)
        {
            IntrigueState state = State;
            if (state != null && InPlayerClan(mother)) state.Conceived[mother.StringId] = Today;
        }

        /// <summary>Yapay zekânın sana karşı başarılı suikastinde: hekim kurbanı kurtarabilir (ağır yaralı kalır). Kurtardıysa true.</summary>
        public static bool TrySave(IntrigueState state, AiPlot plot, Hero target, string who)
        {
            Hero physician = Working(state);
            if (physician == null || physician == target) return false;
            if (plot.Roll100() > PhysicianRules.PoisonSaveChance(Medicine(physician))) return false;
            target.HitPoints = Math.Max(1, target.MaxHitPoints / 5);
            physician.AddSkillXp(DefaultSkills.Medicine, PhysicianRules.SaveXp);
            string name = target.Name.ToString();
            SchemeOutcomes.Notify(state, "Suikastten kurtuldu: " + name,
                Tr.Dative(name) + " suikast düzenlendi ve ağır yaralandı. Ama hekimin " + physician.Name
                + " yetişti: önceden hazırladığı panzehir ve dikişleriyle hayatını kurtardı. " + name + " bir süre yatakta kalacak." + who,
                target, CardImages.Antidote);
            state.AddLog(Today, physician.Name + " suikast kurbanı " + name + " hayatını kurtardı");
            return true;
        }

        // ---- Haftalık: gizli gebelik, yaşlılık uyarısı ----

        public static void Weekly()
        {
            IntrigueState state = State;
            if (state == null) return;
            foreach (string id in state.Conceived.Where(p => !(Hero.Find(p.Key) is Hero h) || !h.IsAlive || !h.IsPregnant
                         || Today - p.Value > 2 * PhysicianRules.DaysPerYear).Select(p => p.Key).ToList())
                state.Conceived.Remove(id);
            if (state.PhysicianId != null && Current(state) == null)
            {
                Hero lost = Hero.Find(state.PhysicianId);
                state.AddLog(Today, "Hekim koltuğu boşaldı" + (lost != null ? ": " + lost.Name + (lost.IsAlive ? " artık klanında değil" : " öldü") : ""));
                state.PhysicianId = null;
                return;
            }
            Hero physician = Working(state);
            if (physician == null) return;
            physician.AddSkillXp(DefaultSkills.Medicine, PhysicianRules.WeeklyXp);
            int medicine = Medicine(physician);

            Hero spouse = Courtships.Spouse;
            SpouseAffair affair = state.SpouseAffair;
            if (spouse != null && spouse != physician && affair != null && !affair.Discovered
                && state.Pregnancies.Any(p => p.Hidden && p.MotherId == spouse.StringId)
                && state.Roll100() <= PhysicianRules.HiddenPregnancyChance(medicine))
            {
                SpouseLife.Discover(state, "Hekimin " + physician.Name + " " + spouse.Name + " eşini muayene etti: gebeliğin başladığı haftalarda "
                    + "sen yanında değildin; hesap tutmuyor.");
                return;
            }

            Hero me = Hero.MainHero;
            if (me == physician || Today < state.PhysicianWarnUntil) return;
            int risk = YearlyRiskSeen(state, physician, me);
            if (risk < PhysicianRules.WarnYearlyRisk) return;
            state.PhysicianWarnUntil = Today + PhysicianRules.WarnCooldownDays;
            SchemeOutcomes.Notify(state, "Hekimin uyarıyor",
                physician.Name + " seni muayene etti ve yüzü asıldı: \"Yaşın ilerledi efendim. Bu yıl seni kaybetme ihtimalimiz %" + risk
                + " kadar.\"\n\nVarisini şimdiden düşün: Editörler > Aile editörü > Klanı devret ya da varis seçimi. Hekimin ne kadar iyiyse riski o kadar düşürür.",
                physician, CardImages.Physician);
            state.AddLog(Today, physician.Name + " sağlığın için uyardı (yıllık risk ~%" + risk + ")");
        }

        // ---- Sağlık raporu ----

        /// <summary>Hekimin gözüyle yıllık ölüm riski (%): oyunun modeli (hekimin etkisi dahil), hekimin hata payıyla, haftalık sabit.</summary>
        public static int YearlyRiskSeen(IntrigueState state, Hero physician, Hero hero)
        {
            double daily = Campaign.Current.Models.HeroDeathProbabilityCalculationModel.CalculateHeroDeathProbability(hero);
            double yearly = PhysicianRules.YearlyRisk(daily) * 100;
            int roll = TreasuryRules.WeeklyRoll(state.Seed, Today / 7, physician.StringId + "|health|" + hero.StringId);
            return (int)Math.Round(Math.Max(0, TreasuryRules.Noisy(yearly, PhysicianRules.ErrorPercent(Medicine(physician)), roll)));
        }

        public sealed class HealthLine
        {
            public Hero Hero;
            public string Text;
            public int Risk;
        }

        /// <summary>Klanındaki yetişkinler (ve gebe olanlar): sen, eşin, sonra yaşa göre.</summary>
        public static List<HealthLine> Report(IntrigueState state)
        {
            Hero physician = Working(state);
            var lines = new List<HealthLine>();
            if (physician == null) return lines;
            Hero spouse = Courtships.Spouse;
            IEnumerable<Hero> people = Clan.PlayerClan.Heroes.Where(h => h.IsAlive && (!h.IsChild || h.IsPregnant))
                .OrderBy(h => h == Hero.MainHero ? 0 : h == spouse ? 1 : 2).ThenByDescending(h => h.Age);
            foreach (Hero hero in people)
            {
                var parts = new List<string> { (int)hero.Age + " yaşında", "can " + hero.HitPoints + "/" + hero.MaxHitPoints };
                if (hero.IsWounded) parts.Add("yaralı");
                if (hero.IsPregnant) parts.Add("gebe" + DueText(state, hero));
                if (hero.IsPrisoner) parts.Add("esir");
                int risk = YearlyRiskSeen(state, physician, hero);
                lines.Add(new HealthLine
                {
                    Hero = hero,
                    Risk = risk,
                    Text = hero.Name + " · " + string.Join(" · ", parts)
                        + "\n" + (risk >= PhysicianRules.WarnYearlyRisk ? "Hekim endişeli: bu yıl ölüm riski ~%" + risk
                            : risk > 0 ? "Yaşlılık başlıyor: yıllık risk ~%" + risk : "Sağlıklı"),
                });
            }
            return lines;
        }

        private static string DueText(IntrigueState state, Hero mother)
        {
            Pregnancy ours = state.Pregnancies.FirstOrDefault(p => p.MotherId == mother.StringId);
            if (ours != null) return ", doğuma ~" + Math.Max(0, ours.DueDay - Today) + " gün";
            if (!state.Conceived.TryGetValue(mother.StringId, out int day)) return "";
            int due = day + (int)Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays;
            return ", doğuma ~" + Math.Max(0, due - Today) + " gün";
        }

        /// <summary>Liste menüsü için düz metin.</summary>
        public static string ReportText(IntrigueState state)
        {
            string blocked = Blocked(state);
            if (blocked != null) return blocked + ".";
            Hero physician = Current(state);
            return ProtectionText(physician) + "\n" + string.Join("\n", Report(state).Select(l => "· " + l.Text.Replace("\n", ": ")));
        }
    }
}
