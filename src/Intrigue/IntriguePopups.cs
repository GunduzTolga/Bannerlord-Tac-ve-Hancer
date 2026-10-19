using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
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
    /// kaydet-yükle sonrası da sorulur. Popup açıkken oyun durur.
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
        private static int _pausedUntil;

        public static void Tick()
        {
            if (Environment.TickCount - _pausedUntil < 0) return;
            try
            {
                TickCore();
            }
            catch (Exception ex)
            {
                _open = false;
                _pausedUntil = Environment.TickCount + ErrorPauseMs;
                MenuUI.Error("Entrika popup'ı gösterilemedi: " + ex.Message);
                Debug.Print("[CheatMenu] intrigue popup failed: " + ex);
            }
        }

        private static void TickCore()
        {
            if (_open)
            {
                if (InformationManager.IsAnyInquiryActive()) _lastSeenActive = Environment.TickCount;
                else if (Environment.TickCount - _lastSeenActive > StaleOpenMs) _open = false;
                return;
            }
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState)) return;
            if (InformationManager.IsAnyInquiryActive() || MenuUI.HasPending) return;
            if (Campaign.Current?.ConversationManager?.IsConversationInProgress == true) return;
            IntrigueState state = IntrigueBehavior.Current?.State;
            if (state == null) return;

            if (state.Notices.Count > 0)
            {
                ShowNotice(state, state.Notices[0]);
                return;
            }
            if (state.LiegeDemand != null)
            {
                ShowLiegeDemand(state);
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
            next = state.Schemes.FirstOrDefault(s => s.Kind == SchemeKind.Murder && !s.AskedToExecute
                && s.Phase >= SchemeRules.MaxPhases(s.Kind));
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
            ShowChoice("Hükümdar hesap soruyor",
                ruler?.Name + ", " + Tr.Genitive(demand.VictimName) + (demand.Kidnap ? " kaçırılmasının" : " ölümünün")
                    + " arkasında senin olduğunu öğrendi ve hesap soruyor.",
                options);
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
            ShowChoice("Sana karşı entrika sezildi",
                "Casus başın kötü haberlerle geldi: " + plotter.Name + " (" + SchemeFactors.Affiliation(plotter) + "), "
                + (target == Hero.MainHero ? "seni" : Tr.Accusative(target.Name.ToString())) + " " + what + " için adam tutmuş.\n\n"
                + "Hakkında rapor ve \"suikast planı\" sırrı artık elinde (şantajla koza çevirebilirsin; koz alırsan planından vazgeçer).",
                options);
        }

        /// <summary>Sezilen bir entrikaya karşı seçenekler; Entrika > Sana karşı entrikalar menüsü de bunu kullanır.</summary>
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
            new IntrigueEvents.Option { Label = "Şimdilik bekle (Entrika > Sana karşı entrikalar)", Apply = () => { } },
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
            ShowChoice(title, text, options);
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
            if (blackmail)
                ShowChoice("Şantaj (" + target.Name + "): deliller hazır",
                    "Adamların " + Tr.Genitive(target.Name.ToString()) + " sırrını kanıtlayacak kadar delil topladı. Şimdi mi yüzüne vurursun, "
                    + "yoksa biraz daha mı beklersin?", options);
            else
                ShowChoice("Casusluk (" + target.Name + "): ilk bilgiler geldi",
                    "Adamların " + target.Name + " hakkında rapor yazacak kadar bilgi topladı. Şimdi mi alırsın, yoksa sırlarını mı kurcalasınlar?",
                    options);
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
            options.Add(new IntrigueEvents.Option { Label = "Sonra karar ver (Entrika > Süren entrikalar)", Apply = () => { } });
            foreach (IntrigueEvents.Option option in options)
            {
                Action apply = option.Apply;
                option.Apply = () =>
                {
                    scheme.AskedToExecute = true;
                    apply();
                };
            }
            ShowChoice("Suikast (" + target.Name + "): plan hazır",
                "Hazırlıklar tamamlandı; daha fazla beklemek bir şey kazandırmaz. Ne zaman vuracağına sen karar ver.", options);
        }

        /// <summary>Zorunlu seçim: kapatma düğmesi yok; seçilemeyen seçenekler ipucuyla gri görünür.</summary>
        private static void ShowChoice(string title, string text, List<IntrigueEvents.Option> options)
        {
            List<InquiryElement> elements = options
                .Select(o => new InquiryElement(o, o.Label, null, o.DisabledHint == null, o.DisabledHint ?? string.Empty))
                .ToList();

            MarkOpen();
            MBInformationManager.ShowMultiSelectionInquiry(
                new MultiSelectionInquiryData(title, text, elements, false, 1, 1, "Seç", string.Empty,
                    selected =>
                    {
                        _open = false;
                        if (selected != null && selected.Count > 0 && selected[0].Identifier is IntrigueEvents.Option option)
                            MenuUI.SafeRun(title, option.Apply);
                    },
                    _ => _open = false),
                true, false);
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
