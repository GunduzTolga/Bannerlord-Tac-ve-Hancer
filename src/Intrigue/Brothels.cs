using System;
using System.Collections.Generic;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Genelev, müşteri tarafı (tasarım: docs/superpowers/specs/2026-10-19-p2-genelev.md): herhangi bir şehrin genelevinde bir gece,
    /// askerlere bir gece ısmarlamak ve oradan kapılabilen hastalık. Yalnızca metin ve resimli kart: içerisi anlatılmaz, perde iner.
    /// Çalışanlar isimsiz, gönüllü ve ücretli yetişkinlerdir; hiçbir kahraman orada çalışmaz. Kurallar'daki "Genelev" ayarıyla açılır
    /// (varsayılan kapalı). İşletme tarafı: NightHouses.
    /// </summary>
    public static class Brothels
    {
        private static int Today => SchemeFactors.Today;

        public static void RegisterMenus(CampaignGameStarter starter)
        {
            starter.AddGameMenuOption("town", "tvh_brothel", "Genelevine uğra",
                args =>
                {
                    args.optionLeaveType = GameMenuOption.LeaveType.Submenu;
                    return NightHouses.Enabled && Settlement.CurrentSettlement?.IsTown == true;
                },
                _ => MenuUI.SafeRun("Genelev", () => Show(Settlement.CurrentSettlement)),
                false, 8);
        }

        /// <summary>Kapıdaki kart: gece, askerler, (hastaysan) hekim.</summary>
        public static void Show(Settlement town)
        {
            IntrigueState state = SchemeEngine.State;
            if (town == null || state == null) return;
            bool own = state.HouseIn(town.StringId) != null;
            int troops = Troops();
            var options = new List<EventCards.CardOption>
            {
                new EventCards.CardOption
                {
                    Label = "Geceyi burada geçir: sade bir oda (" + SchemeFactors.Money(HouseRules.VisitCost(false, own)) + ")",
                    DisabledHint = VisitBlocked(state, town, false),
                    Apply = () => Visit(SchemeEngine.State, town, false),
                },
                new EventCards.CardOption
                {
                    Label = "Geceyi burada geçir: en iyi oda (" + SchemeFactors.Money(HouseRules.VisitCost(true, own)) + ")",
                    DisabledHint = VisitBlocked(state, town, true),
                    Apply = () => Visit(SchemeEngine.State, town, true),
                },
                new EventCards.CardOption
                {
                    Label = "Askerlerine bir gece ısmarla (" + SchemeFactors.Money(HouseRules.TroopNightCost(troops)) + ", moral +" + HouseRules.TroopNightMorale + ")",
                    DisabledHint = TroopNightBlocked(state),
                    Apply = () => TroopNight(SchemeEngine.State, town),
                },
            };
            if (state.IsIll(Today))
                options.Add(new EventCards.CardOption
                {
                    Label = "Şehrin hekimine görün (" + SchemeFactors.Money(HouseRules.DoctorCost) + ", " + HouseRules.DoctorCureDays + " günde geçer)",
                    DisabledHint = SchemeFactors.CanAfford(HouseRules.DoctorCost) ? null : "Yeterli altının yok",
                    Apply = () => Doctor(SchemeEngine.State),
                });
            var leave = new EventCards.CardOption { Label = "Ayrıl", Apply = () => { } };
            options.Add(leave);

            string text = "Kırmızı fenerin altında aralık bir kapı; içeride lavta, şarap ve alçak sesle konuşmalar. Kapıdaki kadın parayı sayar, "
                + "adını sormaz. Burada çalışanlar kendi istekleriyle ve parasıyla çalışır."
                + (own ? "\n\nBu ev senin: gece yarı fiyatına." : "")
                + (state.IsIll(Today) ? "\n\n" + IllnessStatus(state) : "");
            EventCards.Show("Genelev", town.Name + " genelevi", text, CardImages.NightHouse, null, options, leave, null);
        }

        // ---- Bir gece ----

        public static string VisitBlocked(IntrigueState state, Settlement town, bool fine)
        {
            if (state.BrothelVisitUntil > Today) return "Bu gece zaten bir genelevdeydin";
            if (state.IsIll(Today)) return "Hastayken olmaz; önce iyileş";
            return SchemeFactors.CanAfford(HouseRules.VisitCost(fine, state.HouseIn(town.StringId) != null)) ? null : "Yeterli altının yok";
        }

        public static void Visit(IntrigueState state, Settlement town, bool fine)
        {
            string blocked = VisitBlocked(state, town, fine);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(HouseRules.VisitCost(fine, state.HouseIn(town.StringId) != null));
            state.BrothelVisitUntil = Today + 1;

            var parts = new List<string>();
            Hero heard = null;
            if (state.Roll100() <= HouseRules.VisitIntelChance(fine))
            {
                heard = NightHouses.Overhear(state, town, out string secretLine);
                if (heard != null)
                    parts.Add("Gece boyunca yan odada " + heard.Name + " konuşuldu; kulağına gelenler bir casusluk raporu kadar işe yarar (Kişiler)." + secretLine);
            }
            if (state.Roll100() <= HouseRules.VisitIllnessChance(fine))
            {
                FallIll(state);
                parts.Add("Birkaç gün sonra ateş ve halsizlik: buradan bir hastalık kaptın. " + IllnessStatus(state));
            }
            Hero spouse = Courtships.Spouse;
            if (spouse != null && spouse.IsAlive && spouse != Hero.MainHero)
            {
                bool near = spouse.CurrentSettlement == town || (spouse.PartyBelongedTo != null && spouse.PartyBelongedTo == MobileParty.MainParty);
                if (state.Roll100() <= (near ? HouseRules.SpouseHearsNearChance : HouseRules.SpouseHearsChance))
                {
                    SchemeOutcomes.Relate(spouse, -HouseRules.SpouseHearsPenalty);
                    parts.Add(spouse.Name + " geceyi nerede geçirdiğini duydu: aranız -" + HouseRules.SpouseHearsPenalty + ".");
                }
            }
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " genelevde bir gece");

            string text = (fine
                    ? "Ev sahibesi seni üst kattaki en iyi odaya çıkarıyor: kalın halılar, sıcak şarap, ağır perdeler. "
                    : "Daracık bir oda, ince bir duvar, tek bir mum. ")
                + "Kapı ardından kapanıyor; gerisi bu evin sırrı olarak kalır.\n\n"
                + (fine ? "Sabah kimseye görünmeden arka kapıdan çıkıyorsun." : "Sabaha karşı sokağa çıktığında şehir hâlâ uyuyor.")
                + (parts.Count > 0 ? "\n\n" + string.Join("\n\n", parts) : "\n\nKimse seni görmedi, kimse bir şey duymadı.");
            EventCards.Show("Genelev", "Bir gece: " + town.Name, text, CardImages.BrothelNight, heard, Ok(out EventCards.CardOption ok), ok, null);
        }

        // ---- Askerlere bir gece ----

        private static int Troops() => MobileParty.MainParty?.MemberRoster?.TotalRegulars ?? 0;

        public static string TroopNightBlocked(IntrigueState state)
        {
            if (state.TroopNightUntil > Today) return "Askerlerin daha yeni eğlendi (" + (state.TroopNightUntil - Today) + " gün)";
            if (Troops() <= 0) return "Partinde asker yok";
            return SchemeFactors.CanAfford(HouseRules.TroopNightCost(Troops())) ? null : "Yeterli altının yok";
        }

        public static void TroopNight(IntrigueState state, Settlement town)
        {
            string blocked = TroopNightBlocked(state);
            if (blocked != null) throw new InvalidOperationException(blocked);
            int troops = Troops();
            SchemeFactors.Pay(HouseRules.TroopNightCost(troops));
            MobileParty.MainParty.RecentEventsMorale += HouseRules.TroopNightMorale;
            state.TroopNightUntil = Today + HouseRules.TroopNightCooldownDays;
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " askerlere bir gece ısmarlandı");
            EventCards.Show("Genelev", "Askerlerin gecesi: " + town.Name,
                "Kese çözüldü: şehrin meyhaneleri ve genelevleri bu gece " + troops + " askerine senin hesabına açık. Şarkılar sabaha kadar "
                + "sürüyor; sabah yüzlerde mahmur bir gülümseme var.\n\nSonuç: partinin morali +" + HouseRules.TroopNightMorale
                + " (günler içinde azalır). Yeniden ısmarlamak için " + HouseRules.TroopNightCooldownDays + " gün.",
                CardImages.BrothelTroops, null, Ok(out EventCards.CardOption ok), ok, null);
        }

        // ---- Hastalık ----

        private static void FallIll(IntrigueState state)
        {
            if (!state.IsIll(Today)) state.IllnessSince = Today;
            state.IllnessUntil = Math.Max(state.IllnessUntil, Today + HouseRules.IllnessDays);
            CapHealth();
            state.AddLog(Today, "Hastalık kapıldı");
        }

        /// <summary>Hastalığın durumu: ne zaman geçeceği, kimin tedavi ettiği.</summary>
        public static string IllnessStatus(IntrigueState state)
        {
            Hero doctor = Physician.Current(state);
            int end = HouseRules.IllnessEnd(state.IllnessSince, state.IllnessUntil, doctor == null ? -1 : Physician.Medicine(doctor));
            int days = Math.Max(1, end - Today);
            return "Hastasın: canın %" + HouseRules.IllnessHealthCapPercent + "'i aşmıyor. "
                + (doctor != null
                    ? "Saray hekimin " + doctor.Name + " ilgileniyor: yaklaşık " + days + " günde geçer."
                    : "Kendiliğinden yaklaşık " + days + " günde geçer; saray hekimin olsaydı daha çabuk geçerdi, şehirlerin hekimi de tedavi eder.");
        }

        public static void Doctor(IntrigueState state)
        {
            if (!state.IsIll(Today)) return;
            if (!SchemeFactors.CanAfford(HouseRules.DoctorCost)) throw new InvalidOperationException("Yeterli altının yok");
            SchemeFactors.Pay(HouseRules.DoctorCost);
            state.IllnessUntil = Math.Min(state.IllnessUntil, Today + HouseRules.DoctorCureDays);
            EventCards.Show("Hekim", "Şehrin hekimi",
                "Hekim seni dinliyor, başını sallıyor; acı bir merhem ve kaynatılmış otlar veriyor. \"Şaraptan ve bu evlerden birkaç gün uzak dur.\"\n\n"
                + "Sonuç: hastalık " + HouseRules.DoctorCureDays + " gün içinde geçer.",
                CardImages.Illness, null, Ok(out EventCards.CardOption ok), ok, null);
        }

        /// <summary>Her gün (IntrigueBehavior): hastalığın canı sınırlaması ve geçmesi.</summary>
        public static void Daily()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.IllnessUntil <= 0) return;
            Hero doctor = Physician.Current(state);
            int end = HouseRules.IllnessEnd(state.IllnessSince, state.IllnessUntil, doctor == null ? -1 : Physician.Medicine(doctor));
            if (Today >= end)
            {
                state.IllnessSince = 0;
                state.IllnessUntil = 0;
                SchemeOutcomes.Notify(state, "İyileştin",
                    (doctor != null ? "Saray hekimin " + doctor.Name + " seni ayağa kaldırdı." : "Ateş düştü, halsizlik geçti.")
                    + " Hastalıktan eser kalmadı; canın yeniden tam dolabilir.",
                    doctor, CardImages.Illness);
                state.AddLog(Today, "Hastalık geçti");
                return;
            }
            CapHealth();
        }

        private static void CapHealth()
        {
            Hero me = Hero.MainHero;
            if (me == null || !me.IsAlive) return;
            int cap = HouseRules.HealthCap(me.MaxHitPoints);
            if (me.HitPoints > cap) me.HitPoints = cap;
        }

        private static List<EventCards.CardOption> Ok(out EventCards.CardOption ok)
        {
            ok = new EventCards.CardOption { Label = "Tamam", Apply = () => { } };
            return new List<EventCards.CardOption> { ok };
        }
    }
}
