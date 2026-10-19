using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace CheatMenu.UI
{
    /// <summary>
    /// Resimli olay kartları (yol haritası P1): entrika olayları, kararlar ve sonuç bildirimleri düz popup yerine
    /// solda resim, sağda metin ve seçeneklerle gösterilir. Resim Images klasöründe yoksa kişinin portresi görünür.
    /// Kart gösterilemezse (ayar kapalı, arayüz hatası) aynı seçenekler oyunun seçim penceresiyle sorulur.
    /// </summary>
    public static class EventCards
    {
        private const string Movie = "TvhEventCard";
        private const int Order = 410;

        public sealed class CardOption
        {
            public string Label;
            /// <summary>null: seçilebilir; değilse neden seçilemediği (gri görünür).</summary>
            public string DisabledHint;
            public Action Apply;
        }

        private static readonly MovieLayer Layer = new MovieLayer();
        private static readonly EscapeKey Escape = new EscapeKey();
        private static readonly object PauseOwner = new object();
        private static string _title;
        private static Action _onClosed;
        private static CardOption _dismiss;
        private static bool _choosing;
        private static bool _failedOnce;

        public static bool IsOpen => Layer.IsOpen;

        /// <summary>Kartı gösterir; olmazsa seçim penceresine düşer. Seçilen seçenek bir sonraki karede uygulanır.</summary>
        /// <param name="dismiss">Esc ile seçilecek seçenek (bildirimlerde "Tamam"); null ise seçim zorunlu.</param>
        /// <param name="onClosed">Kart ya da pencere kapanınca (seçimden önce) çağrılır.</param>
        public static void Show(string caption, string title, string text, string image, Hero hero, List<CardOption> options,
            CardOption dismiss, Action onClosed)
        {
            if (TryShow(caption, title, text, image, hero, options, dismiss, onClosed)) return;
            ShowInquiry(title, text, options, dismiss, onClosed);
        }

        public static bool TryShow(string caption, string title, string text, string image, Hero hero, List<CardOption> options,
            CardOption dismiss, Action onClosed)
        {
            if (IsOpen || !SettingsStore.Current.EventCards || IntrigueScreen.CannotOpenReason() != null) return false;
            if (!TvhUi.ImageWidgetAvailable())
            {
                if (!_failedOnce) Debug.Print("[CheatMenu] TvhImageWidget is not registered; event cards fall back to inquiries");
                _failedOnce = true;
                return false;
            }
            TvhUi.PrepareBrushes();
            EventCardVM vm = null;
            try
            {
                vm = new EventCardVM(caption, title, text, image, hero, options, Choose);
                if (!Layer.Open(Movie, vm, Order, true)) return false;
            }
            catch (Exception ex)
            {
                vm?.OnFinalize();
                Layer.Close();
                Debug.Print("[CheatMenu] event card failed: " + ex);
                if (!_failedOnce) MenuUI.Error("Olay kartı gösterilemedi (" + ex.Message + "); düz pencere kullanılıyor.");
                _failedOnce = true;
                return false;
            }
            _title = title;
            _onClosed = onClosed;
            _dismiss = dismiss;
            _choosing = false;
            Escape.Reset();
            // Eski popup'lar gibi: kart açıkken gün ilerlemesin (karar eskimesin).
            TimePause.Acquire(PauseOwner);
            return true;
        }

        /// <summary>Suikast ya da kaçırmada "harekete geç": rüşvet seçimi kartı (Entrika ekranından).</summary>
        public static void ShowExecuteChoice(Scheme scheme, Action afterwards)
        {
            Hero target = Hero.Find(scheme.TargetId);
            if (target == null) return;
            string blocked = SchemeEngine.CannotExecuteReason(scheme);
            List<CardOption> options = SchemeEngine.BribeChoices(scheme)
                .Select(choice => new CardOption
                {
                    Label = choice.Label,
                    DisabledHint = blocked ?? (choice.Affordable ? null : "Yeterli altının yok"),
                    Apply = () =>
                    {
                        SchemeEngine.Execute(scheme, choice.Level);
                        afterwards?.Invoke();
                    },
                })
                .ToList();
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            bool kidnap = scheme.Kind == SchemeKind.Kidnap;
            Show(SchemeRules.Name(scheme.Kind).ToUpper(Tr.Culture) + " · KARAR", "Harekete geç: " + target.Name,
                "Aşama " + scheme.Phase + "/" + SchemeRules.MaxPhases(scheme.Kind) + " tamamlandı. "
                + (scheme.Phase < SchemeRules.MaxPhases(scheme.Kind) ? "Beklersen başarı artar, ama her aşamada yakalanma riski var.\n\n" : "\n\n")
                + "Rüşvet başarıyı artırır ama geri gelmez. Zar şimdi atılacak.",
                kidnap ? CardImages.Kidnap : CardImages.Murder, target, options, cancel, null);
        }

        /// <summary>Ulak gönder (P2.2): görüşme daveti, hediye seçimi.</summary>
        public static void ShowAudienceSend(Hero hero, Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            int days = Messengers.TravelDays(hero);
            int cost = LetterRules.MessengerCost(days);
            bool counts = Messengers.GiftCounts(state, hero);
            string blocked = Messengers.AudienceBlocked(state, hero);
            List<CardOption> options = LetterRules.GiftOptions
                .Select(gift => new CardOption
                {
                    Label = gift == 0
                        ? "Hediyesiz ulak: " + SchemeFactors.Money(cost)
                        : "Hediye " + SchemeFactors.Money(gift) + ": ilişki +" + (counts ? LetterRules.GiftRelation(gift) : 0)
                            + ", toplam " + SchemeFactors.Money(cost + gift),
                    DisabledHint = blocked ?? (SchemeFactors.CanAfford(cost + gift) ? null : "Yeterli altının yok"),
                    Apply = () =>
                    {
                        Messengers.SendAudience(SchemeEngine.State, hero, gift);
                        afterwards?.Invoke();
                    },
                })
                .ToList();
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            Show("ULAK", "Ulak gönder: " + hero.Name,
                "Ulağın yaklaşık " + days + " gün sonra " + Tr.Dative(hero.Name.ToString()) + " varır. Kabul ederse onunla haritadan konuşabilirsin: "
                + "lordu bulmaya gitmeden krallığına davet edebilir (oyunun ikna turları), evlilik ya da görev konuşabilirsin.\n\n"
                + "Sana çok düşmansa (ilişki " + LetterRules.AudienceRefuseRelation + " ya da kötü) geri çevirir ve hediyeni geri yollar."
                + (counts ? "" : "\n\nSon hediyenden beri " + LetterRules.GiftCooldownDays + " gün geçmedi: bu kez hediye ilişki getirmez."),
                CardImages.Messenger, hero, options, cancel, null);
        }

        /// <summary>Kur başlat (P2.6): sen ya da eşin. Eşin için önce onun kabulü gerekir.</summary>
        public static void ShowCourtshipStart(Hero target, CourtshipMode mode, Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            Hero agent = Courtships.AgentFor(mode);
            string blocked = Courtships.StartBlocked(state, target, mode);
            if (agent == null)
            {
                MenuUI.Error(blocked ?? "Kur yapacak kimse yok");
                return;
            }
            int chance = Courtships.Chance(agent, target);
            int discovery = Courtships.Discovery(agent, target);
            bool mayConceive = Courtships.MayConceive(state, agent, target);
            var options = new List<CardOption>();
            if (mode == CourtshipMode.Lover)
            {
                options.Add(new CardOption
                {
                    Label = "Kur yapmaya başla (" + SchemeFactors.Money(RomanceRules.GiftCost) + ", başarı %" + chance + ")", DisabledHint = blocked,
                    Apply = () => { Courtships.Start(SchemeEngine.State, target, mode, false); afterwards?.Invoke(); },
                });
            }
            else
            {
                options.Add(new CardOption
                {
                    Label = "Eşine teklif et (kabul %" + Courtships.SpouseAgreeChance(agent, target) + ")", DisabledHint = blocked,
                    Apply = () => { Courtships.Start(SchemeEngine.State, target, mode, false); afterwards?.Invoke(); },
                });
                if (mayConceive)
                    options.Add(new CardOption
                    {
                        Label = "Teklif et, tedbirli olsunlar (gebelik %" + RomanceRules.CarefulPregnancyChance + ")", DisabledHint = blocked,
                        Apply = () => { Courtships.Start(SchemeEngine.State, target, mode, true); afterwards?.Invoke(); },
                    });
            }
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            string text = mode == CourtshipMode.Lover
                ? "Üç aşama, her biri " + RomanceRules.StageDays + " gün: hediye ve iltifat, şiir ve mektuplar, gizli buluşma. Sonunda "
                  + target.Name + " sevgilin olur (%" + chance + ") ya da seni geri çevirir. Her aşamada %" + discovery + " ihtimalle dedikodu olur."
                  + (Courtships.Spouse != null ? " Eşin duyarsa sana kırılır." : "")
                : agent.Name + ", kendi isteğiyle kabul ederse, " + target.Name + " ile vakit geçirip bilgi toplar: sonunda güncel rapor, "
                  + "%" + RomanceRules.IntelSecretChance + " ihtimalle bir sır. Aralarında ne olacağı ikisinin kararı (%" + chance + "). "
                  + "Her aşamada %" + discovery + " ihtimalle yakalanır; o zaman " + target.Name + " seni casusluğa göndermekle suçlar."
                  + (mayConceive ? " Yakınlık olursa gebelik ihtimali var (tedbirsiz %" + RomanceRules.PregnancyChance + ")." : "")
                  + " Onurlu bir eş büyük ihtimalle reddeder; reddederse sana kırılır.";
            Show("KUR", (mode == CourtshipMode.Lover ? "Kur yap: " : "Eşini gönder: ") + target.Name,
                text + "\n\nYalnızca yetişkinler; olanlar metinle anlatılır, sahne yok.", CardImages.Courtship, target, options, cancel, null);
        }

        /// <summary>Rehine anlaşması kartı (P2.10): gönder, değiş ya da erken bitir.</summary>
        public static void ShowHostage(Kingdom kingdom, Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            HostageDeal deal = state.HostageDealWith(kingdom.StringId);
            var options = new List<CardOption>();
            string text;
            if (deal != null)
            {
                text = Hostages.StatusLine(state, kingdom) + "\n\nAnlaşma sürerken misafirler ev sahipleriyle yakınlaşır ve bu krallığın savaş meclisi casus "
                    + "skandalları için toplanmaz. Barışı bozan taraf misafirini esir olarak kaybeder.";
                options.Add(new CardOption
                {
                    Label = "Anlaşmayı erken bitir (" + kingdom.Leader.Name + " ile -" + HostageRules.EarlyEndPenalty + ")",
                    Apply = () => { Hostages.EndEarly(SchemeEngine.State, deal); afterwards?.Invoke(); },
                });
            }
            else
            {
                string blocked = Hostages.Blocked(state, kingdom);
                string exchangeBlocked = Hostages.ExchangeBlocked(state, kingdom);
                bool anyone = Hostages.OurCandidates(state).Count > 0;
                Hero theirs = Hostages.TheirCandidate(state, kingdom);
                text = "Barışın teminatı olarak klanından birini " + kingdom.Leader.Name + " sarayına misafir (esir değil) olarak gönderebilirsin; karşılıklı "
                    + "değişimde onların hanedanından biri de senin yanına gelir" + (theirs != null ? " (büyük ihtimalle " + theirs.Name + ")" : "") + ".\n\n"
                    + "Bir yıl sürer. Misafirler ev sahipleriyle yakınlaşır, casus gerilimi düşer ve bu krallığın savaş meclisi skandallar için toplanmaz. "
                    + "Barış bozulursa misafirler esir düşer; bozan sensen hükümdar ve onurlu lordlar seni affetmez.";
                options.Add(new CardOption
                {
                    Label = "Rehine gönder (tek taraflı, ilişki +" + HostageRules.GiveRulerRelation + ")...",
                    DisabledHint = blocked ?? (anyone ? null : "Gönderecek kimse yok"),
                    Apply = () => MenuUI.Defer(() => PickHostage(kingdom, false, afterwards)),
                });
                options.Add(new CardOption
                {
                    Label = "Rehine değişimi teklif et (kabul %" + Hostages.ExchangeChance(state, kingdom) + ")...",
                    DisabledHint = exchangeBlocked ?? (anyone ? null : "Gönderecek kimse yok"),
                    Apply = () => MenuUI.Defer(() => PickHostage(kingdom, true, afterwards)),
                });
            }
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            Show("REHİNE", "Rehine anlaşması: " + kingdom.Name, text, CardImages.Hostage, kingdom.Leader, options, cancel, null);
        }

        private static void PickHostage(Kingdom kingdom, bool exchange, Action afterwards)
        {
            Picker.Pick(exchange ? "Değişimde kimi göndereceksin?" : "Kimi rehine göndereceksin?", Hostages.OurCandidates(SchemeEngine.State),
                h => h.Name + ", " + (int)h.Age + " yaş" + (h.Father == Hero.MainHero || h.Mother == Hero.MainHero ? " (çocuğun)" : ""),
                h =>
                {
                    MenuUI.SafeRun("Rehine", () =>
                    {
                        if (exchange) Hostages.ProposeExchange(SchemeEngine.State, kingdom, h);
                        else Hostages.Give(SchemeEngine.State, kingdom, h);
                    });
                    afterwards?.Invoke();
                },
                () => { }, h => new CharacterImageIdentifier(CampaignUIHelper.GetCharacterCode(h.CharacterObject, false)), sort: false);
        }

        /// <summary>Düello: meydan okuma kartı.</summary>
        public static void ShowDuel(Hero target, Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            string blocked = Duels.Blocked(state, target);
            DuelReason reason = Duels.ReasonFor(state, target);
            int accept = Duels.AcceptChance(state, target);
            var options = new List<CardOption>
            {
                new CardOption
                {
                    Label = "Meydan oku (kabul %" + accept + ")", DisabledHint = blocked,
                    Apply = () => { Duels.Challenge(SchemeEngine.State, target); afterwards?.Invoke(); },
                },
            };
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            Show("DÜELLO", "Düello: " + target.Name,
                "Sebep: " + DuelRules.ReasonName(reason) + ". " + target.Name + " herkesin önünde meydan okumanı kabul ederse ikiniz şehrin arenasında "
                + "dövüşürsünüz; düşen kaybeder, kimse ölmez. Reddederse korkak görünür (ün +" + DuelRules.RefuseRenown + " sana).\n\n"
                + Duels.Stakes(state, target) + "\n\nKabul ihtimali Cesaretine, Onuruna, sana olan nefretine ve ikinizin dövüş becerisine bağlı.",
                CardImages.Duel, target, options, cancel, null);
        }

        /// <summary>Sevgilinle gizli buluşma (P2.6).</summary>
        public static void ShowMeeting(Hero lover, Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            Lover record = state.LoverOf(lover.StringId);
            if (record == null)
            {
                MenuUI.Error(lover.Name + " sevgilin değil");
                return;
            }
            string blocked = Courtships.MeetBlocked(state, record);
            bool mayConceive = Courtships.MayConceive(state, Hero.MainHero, lover);
            var options = new List<CardOption>
            {
                new CardOption
                {
                    Label = "Buluş" + (mayConceive ? " (gebelik %" + RomanceRules.PregnancyChance + ")" : ""), DisabledHint = blocked,
                    Apply = () => { Courtships.Meet(SchemeEngine.State, record, false); afterwards?.Invoke(); },
                },
            };
            if (mayConceive)
                options.Add(new CardOption
                {
                    Label = "Buluş, tedbirli ol (gebelik %" + RomanceRules.CarefulPregnancyChance + ")", DisabledHint = blocked,
                    Apply = () => { Courtships.Meet(SchemeEngine.State, record, true); afterwards?.Invoke(); },
                });
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            Show("SEVGİLİ", "Gizli buluşma: " + lover.Name,
                "İlişki +" + RomanceRules.MeetingRelation + "." + (record.Public ? " İlişkiniz zaten biliniyor."
                    : " %" + Courtships.MeetingDiscovery(lover) + " ihtimalle görülürsünüz: eşler kırılır, dedikodu yayılır.")
                + " Bir sonraki buluşma " + RomanceRules.MeetingCooldownDays + " gün sonra.\n\nOlanlar ikinizin arasında kalır; sahne yok.",
                CardImages.SecretAffair, lover, options, cancel, null);
        }

        /// <summary>Casus başı görevi seçimi (P2.3). Söylenti seçilirse ardından hedef listesi açılır.</summary>
        public static void ShowMissionChoice(Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            Hero spymaster = SchemeFactors.Spymaster(state);
            if (spymaster == null)
            {
                MenuUI.Error("Önce bir casus başı ata");
                return;
            }
            int roguery = SchemeFactors.Roguery(spymaster);
            int difficulty = SettingsStore.Current.IntrigueDifficulty;
            CardOption Option(SpymasterMission mission, string label, string blocked, Action apply) => new CardOption
            {
                Label = label + (state.Mission == mission ? "  [şu anki görev]" : ""),
                DisabledHint = blocked,
                Apply = () =>
                {
                    apply();
                    afterwards?.Invoke();
                },
            };
            var options = new List<CardOption>
            {
                Option(SpymasterMission.CounterIntel, "Karşı casusluk: sezme haftalık %" + MissionRules.Detection(true, roguery, SpymasterMission.CounterIntel),
                    null, () => SpymasterMissions.SetMission(SchemeEngine.State, SpymasterMission.CounterIntel)),
                Option(SpymasterMission.WatchDissent, "Muhalefeti izle: haftada " + MissionRules.DissentReportsPerWeek(roguery) + " rapor",
                    Clan.PlayerClan.Kingdom == null ? "Bir krallıkta değilsin" : null,
                    () => SpymasterMissions.SetMission(SchemeEngine.State, SpymasterMission.WatchDissent)),
                Option(SpymasterMission.SpreadRumors, "Söylenti yay...: hedef seç (haftada -" + MissionRules.RumorRelationLoss(roguery) + ")",
                    SpymasterMissions.RumorCandidates(state).Count == 0 ? "Sarayında kulağın olan bir lord yok (rapor ya da gölge ağı)" : null,
                    () => MenuUI.Defer(() => PickRumorTarget(afterwards))),
                Option(SpymasterMission.None, "Boşta: yalnızca entrikalar", null,
                    () => SpymasterMissions.SetMission(SchemeEngine.State, SpymasterMission.None)),
            };
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            string text = spymaster.Name + " entrikaları yürütürken arka planda bir göreve de bakabilir. Şu an: "
                + SpymasterMissions.StatusText(state) + ".\n\n"
                + "Karşı casusluk: " + SpymasterMissions.Description(SpymasterMission.CounterIntel, roguery, difficulty) + "\n\n"
                + "Muhalefeti izle: " + SpymasterMissions.Description(SpymasterMission.WatchDissent, roguery, difficulty) + "\n\n"
                + "Söylenti yay: " + SpymasterMissions.Description(SpymasterMission.SpreadRumors, roguery, difficulty);
            Show("CASUS BAŞI · GÖREV", "Görev: " + spymaster.Name, text, CardImages.Spy, spymaster, options, cancel, null);
        }

        private static void PickRumorTarget(Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            Picker.Pick("Söylenti yay: hedef", SpymasterMissions.RumorCandidates(state),
                h => h.Name + " (" + h.Clan.Name + ", " + h.Clan.Kingdom.Name + ") - hükümdarıyla " + Court.MoodText(state, h),
                h => MenuUI.SafeRun("Söylenti", () =>
                {
                    SpymasterMissions.SetMission(SchemeEngine.State, SpymasterMission.SpreadRumors, h);
                    afterwards?.Invoke();
                }),
                null, h => Categories.HeroPortrait(h), sort: false);
        }

        /// <summary>Esir sorgusu: yöntem seçimi kartı (Entrika ekranı > Esirler ve liste menüsü).</summary>
        public static void ShowInterrogation(Interrogation.Prisoner prisoner, Action afterwards)
        {
            IntrigueState state = SchemeEngine.State;
            Hero hero = prisoner.Hero;
            string blocked = Interrogation.BlockedReason(state, prisoner);
            List<CardOption> options = new[] { InterrogationMethod.Talk, InterrogationMethod.Pressure, InterrogationMethod.Deal }
                .Select(method => new CardOption
                {
                    Label = Interrogation.OptionLabel(state, hero, method),
                    DisabledHint = blocked,
                    Apply = () =>
                    {
                        Interrogation.Interrogate(SchemeEngine.State, prisoner, method);
                        afterwards?.Invoke();
                    },
                })
                .ToList();
            options.Add(new CardOption
            {
                Label = "Hizmetine davet et (%" + Recruitment.Chance(hero) + "): kabul ederse klanına katılır",
                DisabledHint = Recruitment.Blocked(state, prisoner),
                Apply = () =>
                {
                    Recruitment.Offer(SchemeEngine.State, prisoner);
                    afterwards?.Invoke();
                },
            });
            var cancel = new CardOption { Label = "Vazgeç", Apply = () => { } };
            options.Add(cancel);
            Show("ESİR", "Esir: " + hero.Name,
                hero.Name + " (" + SchemeFactors.Affiliation(hero) + ") " + prisoner.WhereText.ToLower(Tr.Culture) + ".\n\n"
                + "Konuşursa hakkında rapor alırsın; belki bir sırrını, belki efendisi hakkında bilgi de kaçırır. "
                + "Aynı esir " + InterrogationRules.CooldownDays + " günde bir sorgulanabilir.\n\n"
                + "Ya da hizmetine davet et: kendi isteğiyle kabul ederse klanına katılır, eski klanı bunu ihanet sayar. "
                + "Reddederse esir kalır (fidye oyunun kendi sistemi). Klan liderleri ve evliler kabul etmez.",
                CardImages.Interrogation, hero, options, cancel, null);
        }

        /// <summary>Her kare (SubModule): harita ekranı değiştiyse kartı kapatır, Esc ile kapatılabilir kartları kapatır.</summary>
        public static void Tick()
        {
            if (!IsOpen) return;
            try
            {
                if (!Layer.IsAlive)
                {
                    Action closed = _onClosed;
                    Close();
                    closed?.Invoke();
                    return;
                }
                if (Escape.Released(_dismiss != null && !_choosing && !InformationManager.IsAnyInquiryActive()))
                    Choose(_dismiss);
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] event card tick failed: " + ex);
                Action closed = _onClosed;
                Close();
                closed?.Invoke();
            }
        }

        /// <summary>Kampanya kapanırken: açık kart sessizce kapanır (kararlar kayıtta bekler, sonra yine sorulur).</summary>
        public static void CloseForGameEnd()
        {
            _onClosed = null;
            _dismiss = null;
            _choosing = false;
            Layer.Close();
            TimePause.Release(PauseOwner);
        }

        private static void Choose(CardOption option)
        {
            if (_choosing) return;
            _choosing = true;
            // Tıklama olayı sürerken katmanı kaldırmamak için bir sonraki karede.
            MenuUI.Defer(() =>
            {
                string title = _title;
                Action closed = _onClosed;
                Close();
                closed?.Invoke();
                MenuUI.SafeRun(title ?? "Olay", option.Apply);
            });
        }

        private static void Close()
        {
            _onClosed = null;
            _dismiss = null;
            _choosing = false;
            if (!Layer.IsOpen) return;
            Layer.Close();
            TimePause.Release(PauseOwner);
            IntrigueScreen.Refocus();
        }

        private static void ShowInquiry(string title, string text, List<CardOption> options, CardOption dismiss, Action onClosed)
        {
            List<InquiryElement> elements = options
                .Select(o => new InquiryElement(o, o.Label, null, o.DisabledHint == null, o.DisabledHint ?? string.Empty))
                .ToList();
            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(title, text, elements, dismiss != null, 1, 1, "Seç", dismiss != null ? "Kapat" : string.Empty,
                    selected =>
                    {
                        onClosed?.Invoke();
                        if (selected != null && selected.Count > 0 && selected[0].Identifier is CardOption option)
                            MenuUI.SafeRun(title, option.Apply);
                    },
                    _ =>
                    {
                        onClosed?.Invoke();
                        if (dismiss != null) MenuUI.SafeRun(title, dismiss.Apply);
                    }),
                true, false);
        }
    }
}
