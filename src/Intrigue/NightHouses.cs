using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Genelev işletmesi (tasarım: docs/superpowers/specs/2026-10-19-p2-genelev.md). Gölge ağın olan şehirde çete lideri
    /// üzerinden açılır. Çalışanlar adlı, gönüllü ve ücretli yetişkinlerdir ama oyunun kahramanı değildir: pay düşükse ayrılırlar.
    /// Bir yoldaş evi işletebilir (yönetici, müşteriye çıkmaz); kendisi kabul eder, payı kendi kesesine gider. Haftalık gelir ve
    /// müşterilerden duyulanlar (rapor, bazen sır) getirir. Yalnızca metin; ayardan açılır (varsayılan kapalı).
    /// Yönetim kartı (ShowManage): kasaba menüsü, Gölgelerde iş çevir ve Entrika > Ağlar'dan açılır.
    /// </summary>
    public static class NightHouses
    {
        public static bool Enabled => SettingsStore.Current.NightHouses;

        /// <summary>Divan koltuklarıyla aynı kural: yönetici başka bir koltukta oturamaz (Council.SeatOf).</summary>
        public const string Seat = "genelev yöneticin";

        private static int Today => SchemeFactors.Today;

        /// <summary>null: bu şehirde genelev açılabilir; değilse neden.</summary>
        public static string OpenBlocked(IntrigueState state, Settlement town)
        {
            if (!Enabled) return "Kurallar'dan (MCM > Entrika) açılmalı";
            if (town == null || !town.IsTown) return "Yalnızca şehirlerde";
            if (state.HouseIn(town.StringId) != null) return "Burada zaten bir genelevin var";
            if (state.Network(town.StringId) == null) return "Önce burada bir gölge ağı kur (çete lideri işletir)";
            return SchemeFactors.CanAfford(HouseRules.OpenCost) ? null : "Yeterli altının yok (" + SchemeFactors.Money(HouseRules.OpenCost) + ")";
        }

        public static void Open(IntrigueState state, Settlement town)
        {
            string blocked = OpenBlocked(state, town);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(HouseRules.OpenCost);
            var house = new House { TownId = town.StringId, Share = HouseShare.Standard, Since = Today };
            for (int i = 0; i < HouseRules.StartStaff; i++) house.Workers.Add(NewWorker(state, town, house));
            state.Houses.Add(house);
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " genelev açıldı");
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " genelev açıldı: " + string.Join(", ", house.Workers.Select(w => w.Name)));
        }

        public static int MaxStaff(IntrigueState state, Settlement town) => HouseRules.MaxStaff(state.Network(town.StringId)?.Level ?? 1);

        public static string HireBlocked(IntrigueState state, Settlement town)
        {
            House house = state.HouseIn(town.StringId);
            if (house == null) return "Burada genelevin yok";
            if (Settlement.CurrentSettlement != town) return "Çalışan almak için " + town.Name + " şehrinde olmalısın";
            if (house.Staff >= MaxStaff(state, town)) return "Ağın bu kadarını gizleyebilir (en fazla " + MaxStaff(state, town) + "; ağı güçlendir)";
            return SchemeFactors.CanAfford(HouseRules.HireCost) ? null : "Yeterli altının yok";
        }

        /// <summary>Gönüllü bir çalışan daha: kapıya gelen adaylardan seçilen.</summary>
        public static void Hire(IntrigueState state, Settlement town, HouseWorker worker)
        {
            string blocked = HireBlocked(state, town);
            if (blocked != null) throw new InvalidOperationException(blocked);
            SchemeFactors.Pay(HouseRules.HireCost);
            House house = state.HouseIn(town.StringId);
            worker.Since = Today;
            house.Workers.Add(worker);
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " genelevde yeni çalışan: " + worker.Name);
            MenuUI.Success(worker.Name + " evde çalışmaya başladı: " + house.Staff + " kişi");
        }

        public static void SetShare(IntrigueState state, Settlement town, HouseShare share)
        {
            House house = state.HouseIn(town.StringId) ?? throw new InvalidOperationException("Burada genelevin yok");
            house.Share = share;
            MenuUI.Success("Çalışanların payı: " + HouseRules.ShareName(share));
        }

        public static void Close(IntrigueState state, Settlement town)
        {
            House house = state.HouseIn(town.StringId) ?? throw new InvalidOperationException("Burada genelevin yok");
            string freed = ReleaseManager(house, town);
            state.Houses.Remove(house);
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " genelev kapatıldı");
            MenuUI.Success(Tr.Locative(town.Name.ToString()) + " genelev kapatıldı" + (freed != null ? "; " + freed : ""));
        }

        /// <summary>Ağ çöker ya da dağılırsa genelev de kapanır (çete işletiyordu); yönetici şehirde kalır.</summary>
        public static void OnNetworkLost(IntrigueState state, string townId)
        {
            House house = state.HouseIn(townId);
            if (house == null) return;
            ReleaseManager(house, Settlement.Find(townId));
            state.Houses.Remove(house);
        }

        /// <summary>Şehrin gölge ağı açıklamasına eklenen satır.</summary>
        public static string DescribeLine(IntrigueState state, Settlement town)
        {
            House house = state.HouseIn(town.StringId);
            if (house == null) return Enabled ? "Genelev: yok (" + SchemeFactors.Money(HouseRules.OpenCost) + " ile açılır)." : null;
            Hero manager = Manager(house);
            float prosperity = town.Town?.Prosperity ?? 0f;
            return "Genelev: " + house.Staff + "/" + MaxStaff(state, town) + " çalışan, payları " + HouseRules.ShareName(house.Share)
                + (manager != null ? ", yönetici " + manager.Name : "")
                + ", haftalık sana ~" + SchemeFactors.Money(HouseRules.Net(house, prosperity, Charm(manager)))
                + ", müşterilerden haber %" + HouseRules.Intel(house, Roguery(manager))
                + (Enabled ? "" : " (Kurallar'dan kapalı: çalışmıyor)") + ".";
        }

        // ---- Çalışanların adları ----

        private static HouseWorker NewWorker(IntrigueState state, Settlement town, House house) =>
            HouseRules.NewWorker(FemaleNames(town), MaleNames(town), state.Pick, Today, house.Workers.Select(w => w.Name).ToList());

        private static List<string> FemaleNames(Settlement town) => Names(town?.Culture?.FemaleNameList);

        private static List<string> MaleNames(Settlement town) => Names(town?.Culture?.MaleNameList);

        private static List<string> Names(IEnumerable<TextObject> list) =>
            list?.Select(t => t?.ToString()).Where(n => !string.IsNullOrWhiteSpace(n)).Distinct().ToList() ?? new List<string>();

        /// <summary>Eski kayıttan gelen (yalnızca sayı olan) çalışanlara şehrin kültüründen ad verir.</summary>
        public static void EnsureNamed(IntrigueState state)
        {
            foreach (House house in state.Houses.Where(h => h.Workers.Any(w => w.Name == null)))
            {
                Settlement town = Settlement.Find(house.TownId);
                HouseRules.NameUnnamed(house, FemaleNames(town), MaleNames(town), state.Pick);
            }
        }

        // ---- Yönetici (evi işleten yoldaş) ----

        /// <summary>Evi şu an işleten yoldaş: hayatta, klanında, esir değil ve bir partide değil; yoksa null.</summary>
        public static Hero Manager(House house)
        {
            Hero hero = house?.ManagerId != null ? Hero.Find(house.ManagerId) : null;
            return hero != null && ManagerGoneReason(hero) == null ? hero : null;
        }

        /// <summary>null: yönetici hâlâ evin başında. Değilse neden ayrıldığı.</summary>
        private static string ManagerGoneReason(Hero hero)
        {
            if (!hero.IsAlive) return "artık hayatta değil";
            if (hero.Clan != Clan.PlayerClan) return "artık klanında değil";
            if (hero.IsPrisoner) return "esir düştü";
            if (hero.PartyBelongedTo != null) return "bir partiye katıldı";
            if (hero.GovernorOf != null) return "vali oldu";
            return null;
        }

        public static House HouseManagedBy(IntrigueState state, Hero hero) =>
            hero == null ? null : state.Houses.FirstOrDefault(h => h.ManagerId == hero.StringId);

        private static int Charm(Hero manager) => manager?.GetSkillValue(DefaultSkills.Charm) ?? -1;

        private static int Roguery(Hero manager) => manager?.GetSkillValue(DefaultSkills.Roguery) ?? -1;

        /// <summary>Yönetici adayları: klanındaki yetişkin yoldaşlar; parti lideri, vali, divan üyesi ya da rehine olmayanlar.</summary>
        public static List<Hero> ManagerCandidates(IntrigueState state) =>
            Clan.PlayerClan.Companions
                .Where(h => h.IsAlive && !h.IsChild && !h.IsPrisoner && h != Hero.MainHero && h.GovernorOf == null
                    && !(h.PartyBelongedTo != null && h.PartyBelongedTo.LeaderHero == h)
                    && Council.SeatOf(state, h) == null && state.Hostages.All(d => d.GivenId != h.StringId))
                .OrderBy(h => h.Name.ToString())
                .ToList();

        /// <summary>null: yoldaş kabul eder; değilse nedeni (kendi kararı).</summary>
        public static string Refusal(IntrigueState state, Hero hero)
        {
            Hero me = Hero.MainHero;
            bool partner = hero == me.Spouse || hero.Spouse == me || state.Lovers.Any(l => l.HeroId == hero.StringId);
            return HouseRules.ManagerRefusal((int)hero.Age, partner, hero.GetTraitLevel(DefaultTraits.Honor), hero.GetRelation(me));
        }

        public static void SetManager(IntrigueState state, Settlement town, Hero hero)
        {
            House house = state.HouseIn(town.StringId) ?? throw new InvalidOperationException("Burada genelevin yok");
            if (Manager(house) != null) throw new InvalidOperationException("Evin zaten bir yöneticisi var: " + Manager(house).Name);
            string refusal = Refusal(state, hero);
            if (refusal != null) throw new InvalidOperationException(hero.Name + " istemiyor: " + refusal);
            Council.EnsureFree(state, hero, Seat);
            if (hero.GovernorOf != null) ChangeGovernorAction.RemoveGovernorOf(hero);
            // Partideyse (lideri değil) önce partiden çıkar, sonra şehre (rehineler gibi hemen).
            if (hero.PartyBelongedTo != null && hero.PartyBelongedTo.LeaderHero != hero)
                hero.PartyBelongedTo.MemberRoster.AddToCounts(hero.CharacterObject, -1);
            if (hero.CurrentSettlement != town) TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, town);
            house.ManagerId = hero.StringId;
            house.ManagerSince = Today;
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " genelevi artık " + hero.Name + " işletiyor");
        }

        /// <summary>Yöneticiyi geri çağırır: şehirdeysen hemen partine katılır, değilse sana doğru yola çıkar.</summary>
        public static string RecallManager(IntrigueState state, Settlement town)
        {
            House house = state.HouseIn(town.StringId) ?? throw new InvalidOperationException("Burada genelevin yok");
            Hero hero = Manager(house) ?? throw new InvalidOperationException("Evin yöneticisi yok");
            house.ManagerId = null;
            house.ManagerSince = 0;
            state.AddLog(Today, hero.Name + " genelevden ayrıldı");
            if (MobileParty.MainParty == null) return hero.Name + " evden ayrıldı ve " + Tr.Locative(town.Name.ToString()) + " kaldı.";
            if (Settlement.CurrentSettlement == town && hero.CurrentSettlement == town)
            {
                AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
                return hero.Name + " evi bıraktı ve partine katıldı.";
            }
            TeleportHeroAction.ApplyDelayedTeleportToParty(hero, MobileParty.MainParty);
            return hero.Name + " evi bıraktı ve sana katılmak için yola çıktı.";
        }

        /// <summary>Ev kapanınca yönetici şehirde kalır (partisi yok); kartta söylenecek cümle ya da null.</summary>
        private static string ReleaseManager(House house, Settlement town)
        {
            Hero hero = Manager(house);
            house.ManagerId = null;
            if (hero == null) return null;
            return hero.Name + " " + (town != null ? Tr.Locative(town.Name.ToString()) : "şehirde") + " seni bekliyor";
        }

        /// <summary>Yönetici ayrıldıysa (partiye katıldı, esir düştü...) kaydı temizler ve bildirir.</summary>
        private static void CheckManager(IntrigueState state, House house, Settlement town)
        {
            if (house.ManagerId == null) return;
            Hero hero = Hero.Find(house.ManagerId);
            string gone = hero == null ? "artık yok" : ManagerGoneReason(hero);
            if (gone == null)
            {
                // Yolda değilse ve şehirden ayrıldıysa geri döner (evin başında durur).
                if (hero.HeroState != Hero.CharacterStates.Traveling && hero.CurrentSettlement != town && !hero.IsPrisoner)
                    TeleportHeroAction.ApplyImmediateTeleportToSettlement(hero, town);
                return;
            }
            house.ManagerId = null;
            house.ManagerSince = 0;
            SchemeOutcomes.Notify(state, "Genelevin yöneticisi yok: " + town.Name,
                (hero?.Name.ToString() ?? "Yöneticin") + " " + gone + "; " + Tr.Locative(town.Name.ToString()) + " ev artık yöneticisiz.", hero, CardImages.NightHouse);
        }

        // ---- Yönetim kartı ----

        /// <summary>Genelevin kartı: çalışanlar adlarıyla, yönetici, pay, kazanç; çalışan al, pay, yönetici, kapat.</summary>
        public static void ShowManage(Settlement town)
        {
            IntrigueState state = SchemeEngine.State;
            House house = town != null ? state?.HouseIn(town.StringId) : null;
            if (house == null)
            {
                MenuUI.Info("Burada genelevin yok");
                return;
            }
            EnsureNamed(state);
            CheckManager(state, house, town);
            Hero manager = Manager(house);
            float prosperity = town.Town?.Prosperity ?? 0f;
            int gross = HouseRules.Gross(house, prosperity, Charm(manager));
            int net = HouseRules.Net(house, prosperity, Charm(manager));
            int managerPay = manager != null ? HouseRules.ManagerPay(gross) : 0;

            var lines = new List<string>();
            if (manager != null)
                lines.Add("Yönetici: " + manager.Name + ". Evin işini görür, müşteriye çıkmaz. Charm " + Charm(manager) + ": kazanç +%" + HouseRules.ManagerIncomeBonus(Charm(manager))
                    + "; Roguery " + Roguery(manager) + ": duyulma %" + HouseRules.ExposeChanceWith(Roguery(manager))
                    + ". Kazancın %" + HouseRules.ManagerCutPercent + "'u kendi kesesine (haftada ~" + SchemeFactors.Money(managerPay) + ").");
            else
                lines.Add("Yönetici yok. Bir yoldaşın evi işletebilir: kazanç ve güvenlik artar, kendi kesesine para kazanır (isterse).");
            lines.Add("");
            lines.Add("Çalışanlar (" + house.Staff + "/" + MaxStaff(state, town) + "), payları " + HouseRules.ShareName(house.Share) + ":");
            if (house.Staff == 0) lines.Add("  kimse kalmadı");
            foreach (HouseWorker worker in house.Workers)
                lines.Add("  · " + HouseRules.WorkerLine(worker, Today) + " (" + Math.Max(0, Today - worker.Since) / 7 + " haftadır)");
            lines.Add("");
            lines.Add("Haftalık: toplam ~" + SchemeFactors.Money(gross) + ", çalışanlara %" + HouseRules.SharePercent(house.Share)
                + (manager != null ? ", yöneticiye %" + HouseRules.ManagerCutPercent : "") + ", sana ~" + SchemeFactors.Money(net) + ".");
            lines.Add("Müşterilerden haber %" + HouseRules.Intel(house, Roguery(manager)) + ", duyulma %" + HouseRules.ExposeChanceWith(Roguery(manager))
                + ", ayrılma %" + HouseRules.LeaveChance(house.Share) + "/hafta (sadık olanlar daha az).");
            lines.Add("");
            lines.Add("Burada çalışanlar kendi istekleriyle ve parasıyla çalışır; payı az bulan gider.");

            var options = new List<EventCards.CardOption>
            {
                new EventCards.CardOption
                {
                    Label = "Çalışan al... (" + SchemeFactors.Money(HouseRules.HireCost) + ")",
                    DisabledHint = HireBlocked(state, town),
                    Apply = () => MenuUI.Defer(() => ShowHire(town)),
                },
                new EventCards.CardOption { Label = "Çalışanların payı...", Apply = () => MenuUI.Defer(() => ShowShare(town)) },
                manager != null
                    ? new EventCards.CardOption
                    {
                        Label = "Yöneticiyi geri çağır: " + manager.Name,
                        Apply = () =>
                        {
                            MenuUI.Success(RecallManager(SchemeEngine.State, town));
                            MenuUI.Defer(() => ShowManage(town));
                        },
                    }
                    : new EventCards.CardOption
                    {
                        Label = "Yönetici ata...",
                        DisabledHint = ManagerCandidates(state).Count == 0 ? "Yönetebilecek bir yoldaşın yok (divanda, valilikte ya da parti başında olmayan)" : null,
                        Apply = () => MenuUI.Defer(() => PickManager(town)),
                    },
                new EventCards.CardOption
                {
                    Label = "Genelevi kapat...",
                    Apply = () => MenuUI.Defer(() => MenuUI.Confirm("Genelevi kapat", "Açılış parası geri gelmez; çalışanlar dağılır. Kapatılsın mı?",
                        () => Close(SchemeEngine.State, town), () => MenuUI.Defer(() => ShowManage(town)))),
                },
            };
            var leave = new EventCards.CardOption { Label = "Kapat", Apply = () => { } };
            options.Add(leave);
            EventCards.Show("Genelev", town.Name + " genelevi", string.Join("\n", lines), CardImages.NightHouse, manager, options, leave, null);
        }

        /// <summary>Kapıya gelen adaylar: adı, yaşı, öne çıkan yanı.</summary>
        private static void ShowHire(Settlement town)
        {
            IntrigueState state = SchemeEngine.State;
            House house = state.HouseIn(town.StringId);
            if (house == null) return;
            var taken = house.Workers.Select(w => w.Name).ToList();
            var options = new List<EventCards.CardOption>();
            var lines = new List<string> { "Kapıya iş arayan " + HouseRules.Candidates + " kişi geldi. Hepsi yetişkin, kendi isteğiyle ve parayla çalışacak.", "" };
            for (int i = 0; i < HouseRules.Candidates; i++)
            {
                HouseWorker candidate = HouseRules.NewWorker(FemaleNames(town), MaleNames(town), state.Pick, Today, taken);
                taken.Add(candidate.Name);
                string hint = HouseRules.TraitHint(candidate.Trait);
                lines.Add("· " + HouseRules.WorkerLine(candidate, Today) + (hint.Length > 0 ? ": " + hint : ""));
                options.Add(new EventCards.CardOption
                {
                    Label = HouseRules.WorkerLine(candidate, Today) + " (" + SchemeFactors.Money(HouseRules.HireCost) + ")",
                    DisabledHint = HireBlocked(state, town),
                    Apply = () =>
                    {
                        Hire(SchemeEngine.State, town, candidate);
                        MenuUI.Defer(() => ShowManage(town));
                    },
                });
            }
            var back = new EventCards.CardOption { Label = "Kimseyi alma", Apply = () => MenuUI.Defer(() => ShowManage(town)) };
            options.Add(back);
            EventCards.Show("Genelev", "Çalışan al: " + town.Name, string.Join("\n", lines), CardImages.NightHouse, null, options, back, null);
        }

        private static void ShowShare(Settlement town)
        {
            House house = SchemeEngine.State.HouseIn(town.StringId);
            if (house == null) return;
            var options = new List<EventCards.CardOption>();
            foreach (HouseShare share in new[] { HouseShare.Fair, HouseShare.Standard, HouseShare.Low })
                options.Add(new EventCards.CardOption
                {
                    Label = HouseRules.ShareName(share) + (HouseRules.LeaveChance(share) > 0 ? ": ayrılma %" + HouseRules.LeaveChance(share) + "/hafta"
                        : ": kimse ayrılmaz, daha çok haber"),
                    DisabledHint = share == house.Share ? "Şu anki pay" : null,
                    Apply = () =>
                    {
                        SetShare(SchemeEngine.State, town, share);
                        MenuUI.Defer(() => ShowManage(town));
                    },
                });
            var back = new EventCards.CardOption { Label = "Geri", Apply = () => MenuUI.Defer(() => ShowManage(town)) };
            options.Add(back);
            EventCards.Show("Genelev", "Çalışanların payı: " + town.Name,
                "Kazancın ne kadarı çalışanlara kalsın? Düşük payda ayrılanlar olur (sadık olanlar daha az); adil payda kimse ayrılmaz ve "
                + "müşterilerden duyulanlar sana daha çok gelir.\n\nŞu an: " + HouseRules.ShareName(house.Share) + ".",
                CardImages.NightHouse, null, options, back, null);
        }

        /// <summary>Yoldaş seçimi: istemeyenin nedeni listede yazar; seçilen kendisi karar verir.</summary>
        private static void PickManager(Settlement town)
        {
            IntrigueState state = SchemeEngine.State;
            Picker.Pick("Genelevi kim işletsin?", ManagerCandidates(state),
                h => h.Name + " - Charm " + h.GetSkillValue(DefaultSkills.Charm) + ", Roguery " + h.GetSkillValue(DefaultSkills.Roguery)
                    + (Refusal(state, h) is string why ? "  [istemez: " + why + "]" : ""),
                h => MenuUI.Defer(() => AskManager(town, h)),
                () => MenuUI.Defer(() => ShowManage(town)), h => Categories.HeroPortrait(h), sort: false);
        }

        /// <summary>Yoldaşa sorulur: kabul ederse evin başına geçer, etmezse bir şey değişmez.</summary>
        private static void AskManager(Settlement town, Hero hero)
        {
            IntrigueState state = SchemeEngine.State;
            var ok = new EventCards.CardOption { Label = "Tamam", Apply = () => MenuUI.Defer(() => ShowManage(town)) };
            string refusal = Refusal(state, hero);
            if (refusal != null)
            {
                EventCards.Show("Genelev", hero.Name + " istemiyor",
                    "Teklifini dinliyor ve başını sallıyor: \"Bu iş bana göre değil.\" (" + refusal + ")\n\nKimse zorlanmaz; ilişkiniz değişmedi.",
                    null, hero, new List<EventCards.CardOption> { ok }, ok, null);
                return;
            }
            SetManager(state, town, hero);
            EventCards.Show("Genelev", hero.Name + " evi işletecek",
                hero.Name + " teklifi tartıyor, sonra gülümsüyor: \"Hesabı ben tutarım, kapıda da ben dururum. Kimse kimseyi zorlamaz, "
                + "kimse parasını kaptırmaz.\"\n\n" + hero.Name + " " + Tr.Locative(town.Name.ToString()) + " evin başına geçti; müşteriye çıkmaz, evi işletir."
                + " Kazancın %" + HouseRules.ManagerCutPercent + "'u kendi kesesine gider. Geri çağırmak için genelevin kartı.",
                CardImages.NightHouse, hero, new List<EventCards.CardOption> { ok }, ok, null);
        }

        // ---- Haftalık ----

        public static void Weekly()
        {
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null || state.Houses.Count == 0 || !Enabled) return;
            EnsureNamed(state);
            int income = 0;
            foreach (House house in state.Houses.ToList())
            {
                Settlement town = Settlement.Find(house.TownId);
                if (town == null || state.Network(house.TownId) == null)
                {
                    if (town != null) ReleaseManager(house, town);
                    state.Houses.Remove(house);
                    continue;
                }
                CheckManager(state, house, town);
                Hero manager = Manager(house);
                float prosperity = town.Town?.Prosperity ?? 0f;
                income += HouseRules.Net(house, prosperity, Charm(manager));
                if (manager != null) manager.ChangeHeroGold(HouseRules.ManagerPay(HouseRules.Gross(house, prosperity, Charm(manager))));
                Leavers(state, house, town);
                if (state.Roll100() <= HouseRules.ExposeChanceWith(Roguery(manager)))
                {
                    Exposed(state, house, town);
                    continue;
                }
                if (state.Roll100() <= HouseRules.Intel(house, Roguery(manager))) Intel(state, house, town);
            }
            if (income > 0)
            {
                Hero.MainHero.ChangeHeroGold(income);
                state.AddLog(Today, "Genelevler: haftalık +" + SchemeFactors.Money(income));
                MenuUI.Success("Genelevlerden haftalık gelir: " + SchemeFactors.Money(income));
            }
        }

        /// <summary>Payını az bulanlar ayrılır (gönüllüler: kimse tutulamaz); sadık olanlar daha az.</summary>
        private static void Leavers(IntrigueState state, House house, Settlement town)
        {
            List<HouseWorker> left = house.Workers.Where(w => HouseRules.LeaveChance(house.Share, w.Trait) > 0
                && state.Roll100() <= HouseRules.LeaveChance(house.Share, w.Trait)).ToList();
            if (left.Count == 0) return;
            foreach (HouseWorker worker in left) house.Workers.Remove(worker);
            SchemeOutcomes.Notify(state, "Genelevden ayrılanlar: " + town.Name,
                string.Join(", ", left.Select(w => w.Name)) + " payını az bulup ayrıldı; kalan " + house.Staff + ". Pay artarsa kimse ayrılmaz.",
                null, CardImages.NightHouse);
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " genelevden ayrılanlar: " + string.Join(", ", left.Select(w => w.Name)));
        }

        /// <summary>Kendi şehrinde skandal; başka şehirde baskın (kapanır, şehir sahibiyle ilişki ve suç puanı).</summary>
        private static void Exposed(IntrigueState state, House house, Settlement town)
        {
            Clan owner = town.OwnerClan;
            if (owner == Clan.PlayerClan)
            {
                Kingdom kingdom = Clan.PlayerClan.Kingdom;
                List<Hero> honorable = kingdom == null ? new List<Hero>()
                    : kingdom.Clans.Where(c => c != Clan.PlayerClan && !c.IsEliminated && c.Leader != null && c.Leader.IsAlive
                            && c.Leader.GetTraitLevel(DefaultTraits.Honor) >= 1)
                        .Select(c => c.Leader).ToList();
                foreach (Hero leader in honorable) SchemeOutcomes.Relate(leader, -HouseRules.ScandalPenalty);
                SchemeOutcomes.Notify(state, "Dedikodu: " + town.Name,
                    Tr.Locative(town.Name.ToString()) + " genelevin sana ait olduğu duyuldu. Senin şehrin, senin kuralın; ev açık kalıyor."
                    + (honorable.Count > 0 ? "\n\nSonuç: onurlu " + honorable.Count + " klan lideriyle ilişki -" + HouseRules.ScandalPenalty + "." : ""),
                    null, CardImages.BrothelGossip);
                state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " genelev dedikodusu");
                return;
            }
            string freed = ReleaseManager(house, town);
            state.Houses.Remove(house);
            var parts = new List<string> { "ev kapandı; " + (house.Staff > 0 ? string.Join(", ", house.Workers.Select(w => w.Name)) + " dağıldı" : "kimse kalmamıştı") };
            if (freed != null) parts.Add(freed);
            Hero lord = owner?.Leader;
            if (lord != null && lord.IsAlive)
            {
                SchemeOutcomes.Relate(lord, -HouseRules.RaidOwnerPenalty);
                parts.Add("şehrin sahibi " + lord.Name + " ile ilişki -" + HouseRules.RaidOwnerPenalty);
            }
            Kingdom theirs = owner?.Kingdom;
            if (theirs != null && theirs != Clan.PlayerClan.Kingdom && !FactionManager.IsAtWarAgainstFaction(theirs, Clan.PlayerClan.MapFaction))
            {
                ChangeCrimeRatingAction.Apply(theirs, HouseRules.RaidCrime, true);
                parts.Add(theirs.Name + " suç puanı +" + HouseRules.RaidCrime);
            }
            SchemeOutcomes.Notify(state, "Baskın: " + town.Name,
                "Muhafızlar " + Tr.Locative(town.Name.ToString()) + " geneleve baskın yaptı; izler sana çıktı.\n\nSonuç: " + string.Join(", ", parts) + ".",
                lord, CardImages.BrothelRaid);
            state.AddLog(Today, Tr.Locative(town.Name.ToString()) + " geneleve baskın");
        }

        /// <summary>Müşteriler konuştu: şehirdeki bir lord hakkında rapor, bazen bir sır. Haberi getiren çalışan adıyla yazar.</summary>
        private static void Intel(IntrigueState state, House house, Settlement town)
        {
            Hero target = Overhear(state, town, out string secretLine);
            if (target == null) return;
            List<HouseWorker> listeners = house.Workers.Where(w => w.Trait == WorkerTrait.Listener).ToList();
            List<HouseWorker> pool = listeners.Count > 0 ? listeners : house.Workers;
            string teller = pool.Count > 0 ? pool[state.Pick(pool.Count)].Name : null;
            SchemeOutcomes.Notify(state, "Genelevde duyulanlar: " + target.Name,
                Tr.Locative(town.Name.ToString()) + " genelevde " + target.Name + " hakkında konuşuldu; "
                + (teller != null ? teller + " duyduklarını sana iletti. " : "çalışanların duyduklarını sana iletti. ")
                + "Hakkında güncel bir raporun var." + secretLine,
                target, CardImages.BrothelGossip);
            state.AddLog(Today, "Genelev raporu: " + target.Name + (teller != null ? " (" + teller + ")" : ""));
        }

        /// <summary>
        /// Genelevde konuşulanlar: şehirdeki bir yabancı lord hakkında güncel rapor yazılır, bazen bir sırrı da (secretLine dolar).
        /// Şehirde lord yoksa null. İşletmenin haftalık haberi ve müşteri olarak geçirilen gece bunu kullanır.
        /// </summary>
        internal static Hero Overhear(IntrigueState state, Settlement town, out string secretLine)
        {
            secretLine = "";
            List<Hero> lords = LordsIn(town);
            if (lords.Count == 0) return null;
            Hero target = lords[SchemeRandom.Pick(state.Seed, ref state.Rolls, lords.Count)];
            IntelReport report = SchemeOutcomes.CreateReport(target, Today);
            state.Reports.RemoveAll(r => r.TargetId == target.StringId);
            state.Reports.Add(report);
            if (state.Roll100() <= HouseRules.SecretChance)
            {
                Secret secret = SchemeOutcomes.FindSecret(state, target, state.Roll100, count => SchemeRandom.Pick(state.Seed, ref state.Rolls, count));
                if (secret != null)
                {
                    RomanceSecrets.Record(state, secret);
                    secretLine = "\n\n" + SchemeOutcomes.SecretFoundLine(secret);
                }
            }
            return target;
        }

        /// <summary>Şehirdeki (partisiyle ya da partisiz) yabancı lordlar; sıralı (tohumlu seçim kayıttan bağımsız olsun).</summary>
        private static List<Hero> LordsIn(Settlement town) =>
            town.HeroesWithoutParty
                .Concat(town.Parties.Select(p => p.LeaderHero))
                .Where(h => h != null && h.IsAlive && h.IsLord && !h.IsChild && h != Hero.MainHero && h.Clan != Clan.PlayerClan)
                .Distinct()
                .OrderBy(h => h.StringId, StringComparer.Ordinal)
                .ToList();
    }
}
