using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Entrika popup'ları (tasarım 9.2). Kuyruk yok: her karede durumdaki bekleyen bildirim ve kararlara bakılır,
    /// yalnızca haritadayken ve başka popup açık değilken sıradaki gösterilir. Durum kayda yazıldığı için
    /// kaydet-yükle sonrası da sorulur. Popup açıkken oyun durur. P1: hepsi resimli olay kartıyla (UI.EventCards)
    /// gösterilir; kart açılamazsa oyunun seçim penceresine düşer.
    /// </summary>
    public static class IntriguePopups
    {
        // Popup'ın kapandığını callback'ten öğreniyoruz; callback hiç gelmezse (oyun durumu değişti vs.)
        // ekranda popup kalmamışsa birkaç saniye sonra bayrak kendiliğinden düşer.
        private const int StaleOpenMs = 2000;
        // Bir hata olursa her karede kırmızı mesaj basmamak için bir süre susulur.
        private const int ErrorPauseMs = 10000;

        private static bool _open;
        private static int _lastSeenActive;
        private static bool _paused;
        private static int _pausedSince;

        public static void Tick()
        {
            // TickCount ~25 günde bir negatife döner; bu yüzden mutlak değil, geçen süre karşılaştırılır.
            if (_paused && Environment.TickCount - _pausedSince < ErrorPauseMs) return;
            _paused = false;
            try
            {
                TickCore();
            }
            catch (Exception ex)
            {
                _open = false;
                _paused = true;
                _pausedSince = Environment.TickCount;
                MenuUI.Error("Entrika popup'ı gösterilemedi: " + ex.Message);
                Debug.Print("[CheatMenu] intrigue popup failed: " + ex);
            }
        }

        private static void TickCore()
        {
            if (_open)
            {
                if (InformationManager.IsAnyInquiryActive() || EventCards.IsOpen) _lastSeenActive = Environment.TickCount;
                else if (Environment.TickCount - _lastSeenActive > StaleOpenMs) _open = false;
                return;
            }
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState)) return;
            if (InformationManager.IsAnyInquiryActive() || EventCards.IsOpen || MenuUI.HasPending) return;
            if (Campaign.Current?.ConversationManager?.IsConversationInProgress == true) return;
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;

            if (state.Notices.Count > 0)
            {
                ShowNotice(state, state.Notices[0]);
                return;
            }
            Audience audience = state.Audiences.FirstOrDefault(a => !a.Asked && a.Until > SchemeFactors.Today);
            if (audience != null)
            {
                ShowAudience(state, audience);
                return;
            }
            if (state.LiegeDemand != null)
            {
                ShowLiegeDemand(state);
                return;
            }
            Crisis crisis = state.Crises.FirstOrDefault();
            if (crisis != null)
            {
                ShowCrisis(state, crisis);
                return;
            }
            AiPlot plot = state.AiPlots.FirstOrDefault(p => p.Detected && !p.Asked);
            if (plot != null)
            {
                ShowPlotDetected(state, plot);
                return;
            }
            Scheme next = state.Schemes.FirstOrDefault(s => s.PendingEvent > 0);
            if (next != null)
            {
                ShowEvent(state, next);
                return;
            }
            next = state.Schemes.FirstOrDefault(s => s.AwaitingChoice);
            if (next != null)
            {
                ShowSpyChoice(state, next);
                return;
            }
            // Hedef savaştaysa ya da casus başı esirse sorulmaz; engel kalkınca sorulur (tasarım 5.2.4).
            next = state.Schemes.FirstOrDefault(s => SchemeRules.IsStrike(s.Kind) && !s.AskedToExecute
                && s.Phase >= SchemeRules.MaxPhases(s.Kind) && SchemeEngine.CannotExecuteReason(s) == null);
            if (next != null) ShowExecuteAsk(next);
        }

        private static void ShowNotice(IntrigueState state, Notice notice)
        {
            Hero hero = notice.HeroId != null ? Hero.Find(notice.HeroId) : null;
            void Done()
            {
                state.Notices.Remove(notice);
                _open = false;
            }

            MarkOpen();
            var ok = new EventCards.CardOption { Label = "Tamam", Apply = () => { } };
            if (EventCards.TryShow("BİLDİRİM", notice.Title, notice.Text, notice.Image, hero, new List<EventCards.CardOption> { ok }, ok, Done))
                return;
            if (hero == null)
            {
                InformationManager.ShowInquiry(
                    new InquiryData(notice.Title, notice.Text, true, false, "Tamam", string.Empty, Done, null),
                    true, false);
                return;
            }
            // Portreli bildirim: seçilemeyen tek satırlık liste (kişinin resmi + adı), altında metin.
            // "Kapat" düğmesi de var: oyun "Tamam" için seçim isterse bile pencere her zaman kapanabilsin.
            var portrait = new InquiryElement(hero, hero.Name.ToString(), Portrait(hero), false, string.Empty);
            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(notice.Title, notice.Text, new List<InquiryElement> { portrait },
                    true, 0, 1, "Tamam", "Kapat", _ => Done(), _ => Done()),
                true, false);
        }

        /// <summary>Ulak döndü: lord görüşmeye hazır. Şimdi konuşma penceresi açılır ya da sonra Entrika ekranından.</summary>
        private static void ShowAudience(IntrigueState state, Audience audience)
        {
            Hero hero = Hero.Find(audience.HeroId);
            if (hero == null || !hero.IsAlive)
            {
                state.Audiences.Remove(audience);
                return;
            }
            int days = Math.Max(1, audience.Until - SchemeFactors.Today);
            var options = new List<IntrigueEvents.Option>
            {
                new IntrigueEvents.Option
                {
                    Label = "Şimdi görüş: konuşma penceresi açılır", DisabledHint = hero.IsPrisoner ? "Şu an esir" : null,
                    Apply = () => Messengers.OpenAudience(state, hero),
                },
                new IntrigueEvents.Option { Label = "Sonra: " + days + " gün içinde Entrika ekranı > Saray ya da Kişiler'den", Apply = () => { } },
                new IntrigueEvents.Option { Label = "Görüşmeyi iptal et", Apply = () => state.Audiences.Remove(audience) },
            };
            foreach (IntrigueEvents.Option option in options)
            {
                Action apply = option.Apply;
                option.Apply = () =>
                {
                    audience.Asked = true;
                    apply();
                };
            }
            int bonus = IntriguePersuasionModel.Bonus(hero);
            ShowChoice("ULAK", "Ulak döndü: " + hero.Name,
                hero.Name + " (" + SchemeFactors.Affiliation(hero) + ") ulağını kabul etti ve seni dinlemeye hazır."
                + (audience.Relation > 0 ? " Hediyen ulaştı: ilişki +" + audience.Relation + "." : "")
                + "\n\nKonuşmada oyunun kendi seçenekleri var: krallığına davet (ikna turları ve pazarlık), evlilik, görevler."
                + (bonus > 0 ? " Hakkında bildiklerin sayesinde ikna turlarında başarı +" + bonus + "." : ""),
                options, CardImages.Messenger, hero);
        }

        /// <summary>Yabancı krallıkta ağ çökertildi: tazminat, inkâr ya da ret (P2.5).</summary>
        private static void ShowCrisis(IntrigueState state, Crisis crisis)
        {
            if (!DiplomaticCrises.StillValid(crisis))
            {
                state.Crises.Remove(crisis);
                return;
            }
            Kingdom kingdom = DiplomaticCrises.KingdomOf(crisis);
            int gold = CrisisRules.Reparations(crisis.Level);
            int deny = DiplomaticCrises.DenyChance(state, crisis);
            var options = new List<IntrigueEvents.Option>
            {
                new IntrigueEvents.Option
                {
                    Label = "Tazminat öde: " + SchemeFactors.Money(gold) + " (suç silinir, gerilim yatışır, ilişki -" + CrisisRules.PayRulerPenalty + ")",
                    DisabledHint = SchemeFactors.CanAfford(gold) ? null : "Yeterli altının yok",
                    Apply = () => DiplomaticCrises.Answer(state, crisis, CrisisAnswer.Pay),
                },
                new IntrigueEvents.Option
                {
                    Label = "İnkâr et (%" + deny + "): tutarsa ilişki -" + CrisisRules.DenyRulerPenalty + "; tutmazsa -"
                        + CrisisRules.DenyFailRulerPenalty + " ve gerilim +" + CrisisRules.DenyFailTension,
                    Apply = () => DiplomaticCrises.Answer(state, crisis, CrisisAnswer.Deny),
                },
                new IntrigueEvents.Option
                {
                    Label = "Reddet: ilişki -" + CrisisRules.DefyRulerPenalty + ", gerilim +" + CrisisRules.DefyTension + ", suç kalır",
                    Apply = () => DiplomaticCrises.Answer(state, crisis, CrisisAnswer.Defy),
                },
            };
            ShowChoice("KRİZ", "Diplomatik kriz: " + kingdom.Name, DiplomaticCrises.CardText(state, crisis), options, CardImages.Crisis, kingdom.Leader);
        }

        private static void ShowLiegeDemand(IntrigueState state)
        {
            LiegeDemand demand = state.LiegeDemand;
            if (!SchemeOutcomes.LiegeDemandStillValid(demand))
            {
                state.LiegeDemand = null;
                return;
            }
            Hero ruler = Clan.PlayerClan.Kingdom.Leader;
            int deny = SchemeMath.PersuadeChance(Hero.MainHero.GetSkillValue(DefaultSkills.Charm));
            var options = new List<IntrigueEvents.Option>
            {
                new IntrigueEvents.Option
                {
                    Label = "Kan bedeli öde: " + SchemeFactors.Money(demand.Gold) + " ve " + demand.Influence + " etki",
                    DisabledHint = SchemeFactors.CanAfford(demand.Gold) ? null : "Yeterli altının yok",
                    Apply = () => SchemeOutcomes.PayBloodPrice(state),
                },
                new IntrigueEvents.Option
                {
                    Label = "İnkâr et (Charm, %" + deny + "): olmazsa krallıktan sürülürsün",
                    Apply = () => SchemeOutcomes.DenyMurder(state),
                },
                new IntrigueEvents.Option { Label = "Krallıktan ayrıl", Apply = () => SchemeOutcomes.LeaveOverMurder(state) },
            };
            ShowChoice("HESAP", "Hükümdar hesap soruyor",
                ruler?.Name + ", " + Tr.Genitive(demand.VictimName) + (demand.Kidnap ? " kaçırılmasının" : " ölümünün")
                    + " arkasında senin olduğunu öğrendi ve hesap soruyor.",
                options, CardImages.LiegeDemand, ruler);
        }

        private static void ShowPlotDetected(IntrigueState state, AiPlot plot)
        {
            Hero plotter = Hero.Find(plot.PlotterId);
            Hero target = Hero.Find(plot.TargetId);
            if (plotter == null || target == null)
            {
                plot.Asked = true;
                return;
            }
            var options = PlotOptions(state, plot);
            foreach (IntrigueEvents.Option option in options)
            {
                Action apply = option.Apply;
                option.Apply = () =>
                {
                    plot.Asked = true;
                    apply();
                };
            }
            string what = plot.Kind == SchemeKind.Kidnap ? "kaçırmak" : "öldürmek";
            ShowChoice("KARŞI CASUSLUK", "Sana karşı entrika sezildi",
                "Casus başın kötü haberlerle geldi: " + plotter.Name + " (" + SchemeFactors.Affiliation(plotter) + "), "
                + (target == Hero.MainHero ? "seni" : Tr.Accusative(target.Name.ToString())) + " " + what + " için adam tutmuş.\n\n"
                + "Hakkında rapor ve \"suikast planı\" sırrı artık elinde (şantajla koza çevirebilirsin; koz alırsan planından vazgeçer).",
                options, CardImages.PlotDetected, plotter);
        }

        /// <summary>Sezilen bir entrikaya karşı seçenekler; Entrika ekranındaki "Sana karşı" sekmesi ve Insert menüsü de bunu kullanır.</summary>
        public static List<IntrigueEvents.Option> PlotOptions(IntrigueState state, AiPlot plot) => new List<IntrigueEvents.Option>
        {
            new IntrigueEvents.Option { Label = "İfşa et: plan biter, entrikacının hükümdarıyla arası açılır", Apply = () => AiIntrigue.ExposePlot(state, plot) },
            new IntrigueEvents.Option
            {
                Label = "Koruma tut (" + SchemeFactors.Money(SchemeMath.ProtectionCost) + "): başarı şansı -" + SchemeMath.ProtectionBonus,
                DisabledHint = plot.Protected ? "Zaten koruma tuttun" : SchemeFactors.CanAfford(SchemeMath.ProtectionCost) ? null : "Yeterli altının yok",
                Apply = () => AiIntrigue.Protect(state, plot),
            },
            new IntrigueEvents.Option
            {
                Label = "Karşı suikast başlat (+10 başarı)",
                DisabledHint = AiIntrigue.CounterMurderBlocked(state, plot),
                Apply = () => AiIntrigue.CounterMurder(state, plot),
            },
            new IntrigueEvents.Option { Label = "Şimdilik bekle (Entrika ekranı > Sana karşı)", Apply = () => { } },
        };

        private static void ShowEvent(IntrigueState state, Scheme scheme)
        {
            if (!IntrigueEvents.TryDescribe(state, scheme, out string title, out string text, out List<IntrigueEvents.Option> options))
            {
                scheme.PendingEvent = 0;
                return;
            }
            // Seçim uygulanmadan önce bekleyen olay temizlenir: seçenek entrikayı bitirebilir (ihbar, harekete geç).
            foreach (IntrigueEvents.Option option in options)
            {
                Action apply = option.Apply;
                option.Apply = () =>
                {
                    scheme.PendingEvent = 0;
                    apply();
                };
            }
            Hero target = Hero.Find(scheme.TargetId);
            ShowChoice(SchemeRules.Name(scheme.Kind).ToUpper(Tr.Culture) + " · OLAY", title, text, options,
                CardImages.ForEvent(scheme.PendingEvent), target);
        }

        private static void ShowSpyChoice(IntrigueState state, Scheme scheme)
        {
            Hero target = Hero.Find(scheme.TargetId);
            if (target == null || !target.IsAlive)
            {
                scheme.AwaitingChoice = false;
                return;
            }
            Hero spymaster = SchemeFactors.Spymaster(state);
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
            SchemeInputs next = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
            next.CompletedPhases++;
            bool blackmail = scheme.Kind == SchemeKind.Blackmail;
            string risk = scheme.PhaseDays + " gün, yakalanma %" + SchemeMath.Discovery(scheme.Kind, inputs);
            var options = new List<IntrigueEvents.Option>
            {
                new IntrigueEvents.Option
                {
                    Label = (blackmail ? "Şimdi tehdit et" : "Raporu al") + " (başarı %" + SchemeMath.Success(scheme.Kind, inputs) + ")",
                    Apply = () => SchemeEngine.ChooseContinuation(scheme, false),
                },
                new IntrigueEvents.Option
                {
                    Label = blackmail
                        ? "Bir aşama daha delil topla: başarı %" + SchemeMath.Success(scheme.Kind, next) + " olur, " + risk
                        : "Bir aşama daha sır ara: sır bulma şansı +20, " + risk,
                    Apply = () => SchemeEngine.ChooseContinuation(scheme, true),
                },
            };
            string caption = SchemeRules.Name(scheme.Kind).ToUpper(Tr.Culture) + " · KARAR";
            if (blackmail)
                ShowChoice(caption, "Şantaj (" + target.Name + "): deliller hazır",
                    "Adamların " + Tr.Genitive(target.Name.ToString()) + " sırrını kanıtlayacak kadar delil topladı. Şimdi mi yüzüne vurursun, "
                    + "yoksa biraz daha mı beklersin?", options, CardImages.Blackmail, target);
            else
                ShowChoice(caption, "Casusluk (" + target.Name + "): ilk bilgiler geldi",
                    "Adamların " + target.Name + " hakkında rapor yazacak kadar bilgi topladı. Şimdi mi alırsın, yoksa sırlarını mı kurcalasınlar?",
                    options, CardImages.Spy, target);
        }

        private static void ShowExecuteAsk(Scheme scheme)
        {
            Hero target = Hero.Find(scheme.TargetId);
            if (target == null || !target.IsAlive)
            {
                scheme.AskedToExecute = true;
                return;
            }
            string blocked = SchemeEngine.CannotExecuteReason(scheme);
            var options = SchemeEngine.BribeChoices(scheme)
                .Select(choice => new IntrigueEvents.Option
                {
                    Label = choice.Label,
                    DisabledHint = blocked ?? (choice.Affordable ? null : "Yeterli altının yok"),
                    Apply = () => SchemeEngine.Execute(scheme, choice.Level),
                })
                .ToList();
            options.Add(new IntrigueEvents.Option { Label = "Sonra karar ver (Entrika ekranı > Süren entrikalar)", Apply = () => { } });
            foreach (IntrigueEvents.Option option in options)
            {
                Action apply = option.Apply;
                option.Apply = () =>
                {
                    scheme.AskedToExecute = true;
                    apply();
                };
            }
            ShowChoice(SchemeRules.Name(scheme.Kind).ToUpper(Tr.Culture) + " · PLAN HAZIR", SchemeRules.Name(scheme.Kind) + " (" + target.Name + "): plan hazır",
                "Hazırlıklar tamamlandı; daha fazla beklemek bir şey kazandırmaz. Ne zaman vuracağına sen karar ver.", options,
                CardImages.ForKind(scheme.Kind), target);
        }

        /// <summary>Zorunlu seçim (kapatma yok); seçilemeyen seçenekler ipucuyla gri görünür. Resimli kart, olmazsa seçim penceresi.</summary>
        private static void ShowChoice(string caption, string title, string text, List<IntrigueEvents.Option> options, string image, Hero hero)
        {
            List<EventCards.CardOption> cardOptions = options
                .Select(o => new EventCards.CardOption { Label = o.Label, DisabledHint = o.DisabledHint, Apply = o.Apply })
                .ToList();
            MarkOpen();
            EventCards.Show(caption, title, text, image, hero, cardOptions, null, () => _open = false);
        }

        private static void MarkOpen()
        {
            _open = true;
            _lastSeenActive = Environment.TickCount;
        }

        private static ImageIdentifier Portrait(Hero hero) =>
            new CharacterImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));
    }
}
