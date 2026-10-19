using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.Menu
{
    /// <summary>Entrika menüsü (tasarım 9.1) ve entrika hileleri.</summary>
    public static partial class Categories
    {
        /// <summary>Entrika ekranı açılamazsa ya da oyuncu isterse: eski popup menüsü.</summary>
        public static void ShowIntrigueMenu() => ShowIntrigue();

        private static void ShowIntrigue()
        {
            IntrigueState state = SchemeEngine.State;
            Hero spymaster = SchemeFactors.Spymaster(state);
            int max = SchemeMath.MaxActiveSchemes(SchemeFactors.Roguery(spymaster));

            var entries = new List<MenuEntry>
            {
                MenuEntry.Submenu(spymaster != null ? "Casus başı: " + SchemeFactors.SpymasterLabel(spymaster) : "Casus başı ata...",
                    PickSpymaster),
                MenuEntry.Submenu("Süren entrikalar (" + state.Schemes.Count + "/" + max + ")...", ShowSchemes),
                MenuEntry.Submenu("Yeni entrika...", ShowNewScheme),
                MenuEntry.Submenu("İstihbarat raporları (" + state.Reports.Count + ")...", ShowReports),
                MenuEntry.Submenu("Kozlar (" + state.Hooks.Count(h => h.Until > SchemeFactors.Today) + ")...", ShowHooks),
                MenuEntry.Submenu("Ulak ve görüşme (" + state.Audiences.Count(x => x.Until > SchemeFactors.Today) + " hazır)...", ShowAudiences),
                MenuEntry.Submenu("Esirleri sorgula (" + Interrogation.Prisoners().Count + ")...", ShowPrisoners),
                MenuEntry.Submenu("Kendin gizlice takip et (bulunduğun şehirde)...", () =>
                {
                    Settlement here = Settlement.CurrentSettlement;
                    if (here == null || !here.IsTown)
                    {
                        MenuUI.Error("Önce bir şehre gir: takip edeceğin lordla aynı şehirde olmalısın");
                        MenuUI.Defer(ShowIntrigue);
                        return;
                    }
                    Tailing.ShowPick(here, ShowIntrigue);
                }),
                MenuEntry.Submenu("Kur ve sevgililer (" + (state.Courtships.Count + state.Lovers.Count) + ")...", ShowRomance),
                MenuEntry.Submenu("Rehine anlaşmaları (" + state.Hostages.Count + ")...", () => Picker.Pick("Hangi krallıkla?",
                    Kingdom.All.Where(k => !k.IsEliminated && k != Clan.PlayerClan.Kingdom && k.Leader != null).ToList(),
                    k => k.Name + (state.HostageDealWith(k.StringId) != null ? " (anlaşma sürüyor)" : "")
                        + (Hostages.Blocked(state, k) is string why && state.HostageDealWith(k.StringId) == null ? "  [" + why + "]" : ""),
                    k => UI.EventCards.ShowHostage(k, null), ShowIntrigue)),
                MenuEntry.Submenu("Casus başı görevi: " + SpymasterMissions.StatusText(state) + "...", () => UI.EventCards.ShowMissionChoice(null)),
                MenuEntry.Submenu("Gölge ağları (" + state.Networks.Count + "/" + SchemeMath.MaxNetworks(SchemeFactors.Roguery(spymaster)) + ")...",
                    ShowNetworks),
                MenuEntry.Submenu("Sana karşı entrikalar (" + state.AiPlots.Count(p => p.Detected) + " sezilen)...", ShowPlotsAgainstMe),
                MenuEntry.Submenu("Geçmiş...", ShowIntrigueLog),
            };
            if (spymaster != null) entries.Add(MenuEntry.Simple("Casus başını görevden al", SchemeEngine.DismissSpymaster));
            entries.Add(MenuEntry.Simple("Kayıt testi: işaret bırak (geçici)", () =>
            {
                state.SaveTestMarker = "gün " + SchemeFactors.Today;
                MenuUI.Success("İşaret bırakıldı: " + state.SaveTestMarker + ". Şimdi kaydet ve kaydı yeniden yükle.");
            }));

            ShowCategory("Entrika", entries, ShowIntrigue,
                (spymaster == null ? "Entrikaya başlamak için önce klanından birini casus başı yap (Roguery'si yüksek olan iyi). " : "")
                + "Zorluk: " + SchemeMath.DifficultyName(SettingsStore.Current.IntrigueDifficulty) + " (Kurallar'dan değişir). "
                + "Kayıt testi işareti: " + (state.SaveTestMarker ?? "yok"));
        }

        // ---- Casus başı ----

        private static void PickSpymaster()
        {
            Picker.Pick("Casus başı seç", SchemeFactors.SpymasterCandidates().OrderByDescending(SchemeFactors.Roguery).ToList(),
                SchemeFactors.SpymasterLabel,
                hero =>
                {
                    MenuUI.SafeRun("Casus başı", () => SchemeEngine.AssignSpymaster(hero));
                    MenuUI.Defer(ShowIntrigue);
                },
                ShowIntrigue, HeroPortrait, sort: false);
        }

        // ---- Süren entrikalar ----

        private static void ShowSchemes()
        {
            IntrigueState state = SchemeEngine.State;
            if (state.Schemes.Count == 0)
            {
                MenuUI.Info("Süren bir entrika yok");
                MenuUI.Defer(ShowIntrigue);
                return;
            }
            List<MenuEntry> entries = state.Schemes
                .Select(scheme => MenuEntry.Submenu(SchemeLine(scheme), () => ShowScheme(scheme)))
                .ToList();
            MenuUI.ShowList("Süren entrikalar", entries, ShowSchemes, "Geri", ShowIntrigue);
        }

        private static string SchemeLine(Scheme scheme) =>
            SchemeRules.Name(scheme.Kind) + ": " + Hero.Find(scheme.TargetId)?.Name + " - aşama " + scheme.Phase + "/"
            + SchemeRules.MaxPhases(scheme.Kind) + (SchemeEngine.IsWaiting(scheme) ? "  [karar bekliyor]" : "");

        private static void ShowScheme(Scheme scheme)
        {
            IntrigueState state = SchemeEngine.State;
            Hero target = Hero.Find(scheme.TargetId);
            if (!state.Schemes.Contains(scheme) || target == null)
            {
                MenuUI.Defer(ShowSchemes);
                return;
            }
            Hero spymaster = SchemeFactors.Spymaster(state);
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
            int max = SchemeRules.MaxPhases(scheme.Kind);

            var lines = new List<string> { "Hedef: " + SchemeFactors.TargetLabel(target) };
            lines.Add("Aşama: " + scheme.Phase + "/" + max + " (en az " + SchemeRules.MinPhases(scheme.Kind) + ")"
                + (SchemeEngine.IsWaiting(scheme) ? " - karar bekliyor" : " - sonraki aşamaya " + (scheme.PhaseDays - scheme.DaysIntoPhase) + " gün"));
            if (SchemeRules.IsStrike(scheme.Kind))
            {
                var atMax = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
                atMax.CompletedPhases = Math.Max(scheme.Phase, max);
                lines.Add("Başarı şu an: %" + SchemeMath.Success(scheme.Kind, inputs) + " (" + max + ". aşamada %"
                    + SchemeMath.Success(scheme.Kind, atMax) + "; harekete geçerken rüşvetle +5/+10/+15)");
                if (scheme.Method != SchemeMethod.None) lines.Add("Yöntem: " + MethodName(scheme.Method));
                if (scheme.FeastRisk) lines.Add("Ziyafette vurulacak: ortaya çıkarsa cezalar 1,5 kat");
            }
            else lines.Add("Başarı: %" + SchemeMath.Success(scheme.Kind, inputs));
            if (inputs.NetworkSuccess > 0) lines.Add("Gölge ağı desteği: başarı +" + inputs.NetworkSuccess + ", yakalanma -" + inputs.NetworkDiscovery);
            lines.Add("Her aşamada yakalanma: %" + SchemeMath.Discovery(scheme.Kind, inputs));
            if (spymaster == null) lines.Add("DURDU: casus başın yok");
            else if (SchemeFactors.SpymasterBusyReason(spymaster) != null)
                lines.Add("DURDU: casus başın şu an " + SchemeFactors.SpymasterBusyReason(spymaster));

            var entries = new List<MenuEntry>();
            if (SchemeRules.IsStrike(scheme.Kind))
            {
                string blocked = SchemeEngine.CannotExecuteReason(scheme);
                entries.Add(blocked == null
                    ? MenuEntry.Submenu("Harekete geç...", () => ShowExecute(scheme))
                    : MenuEntry.Simple("Harekete geç (" + blocked + ")", () => MenuUI.Error(blocked)));
            }
            entries.Add(MenuEntry.Submenu("İptal et...", () => MenuUI.Confirm("Entrikayı iptal et",
                "Harcanan para geri gelmez. İptal edilsin mi?",
                () =>
                {
                    SchemeEngine.Cancel(scheme);
                    MenuUI.Defer(ShowIntrigue);
                },
                () => ShowScheme(scheme))));

            MenuUI.ShowList(SchemeRules.Name(scheme.Kind) + ": " + target.Name, entries, () => ShowScheme(scheme), "Geri", ShowSchemes,
                string.Join("\n", lines));
        }

        private static void ShowExecute(Scheme scheme)
        {
            List<MenuEntry> entries = SchemeEngine.BribeChoices(scheme)
                .Select(choice => MenuEntry.Submenu(choice.Label + (choice.Affordable ? "" : "  (yetersiz altın)"), () =>
                {
                    bool done = false;
                    MenuUI.SafeRun("Suikast", () =>
                    {
                        SchemeEngine.Execute(scheme, choice.Level);
                        done = true;
                    });
                    // Başarılıysa menü kapalı kalır: sonuç popup'ı hemen görünsün.
                    if (!done) MenuUI.Defer(() => ShowExecute(scheme));
                }))
                .ToList();
            MenuUI.ShowList("Harekete geç: " + Hero.Find(scheme.TargetId)?.Name, entries, () => ShowExecute(scheme), "Geri",
                () => ShowScheme(scheme), "Rüşvet başarıyı artırır ama geri gelmez. Zar şimdi atılacak.");
        }

        private static string MethodName(SchemeMethod method) =>
            method == SchemeMethod.Poison ? "zehir" : method == SchemeMethod.Dagger ? "hançer" : method == SchemeMethod.Accident ? "kaza süsü" : "-";

        // ---- Yeni entrika ----

        private static void ShowNewScheme()
        {
            string reason = SchemeEngine.CannotStartReason(SchemeEngine.State);
            if (reason != null)
            {
                MenuUI.Error(reason);
                MenuUI.Defer(ShowIntrigue);
                return;
            }
            MenuUI.ShowList("Yeni entrika", new List<MenuEntry>
            {
                MenuEntry.Submenu("Casusluk: rapor ve sır (1-2 aşama, düşük risk)...", () => PickSchemeTarget(SchemeKind.Spy)),
                MenuEntry.Submenu("Şantaj: bildiğin bir sırrı koza çevir (1-2 aşama, orta risk)...", () => PickSchemeTarget(SchemeKind.Blackmail)),
                MenuEntry.Submenu("Kaçırma: hedefi esir al (2-4 aşama, çok yüksek risk)...", () => PickSchemeTarget(SchemeKind.Kidnap)),
                MenuEntry.Submenu("Suikast: hedefi öldür (2-5 aşama, çok yüksek risk)...", () => PickSchemeTarget(SchemeKind.Murder)),
            }, ShowNewScheme, "Geri", ShowIntrigue,
                "Casusluk raporu, o hedefe suikast ve kaçırmada +10 başarı verir ve tanışmadığın birini de listeye ekler. "
                + "Şantaj için önce casuslukla bir sır bulmalısın.");
        }

        private static void PickSchemeTarget(SchemeKind kind)
        {
            IntrigueState state = SchemeEngine.State;
            Hero spymaster = SchemeFactors.Spymaster(state);
            List<Hero> targets = SchemeFactors.Targets(state, kind).ToList();
            if (targets.Count == 0)
            {
                MenuUI.Error(SchemePreview.NoTargetsText(kind));
                MenuUI.Defer(ShowNewScheme);
                return;
            }
            Picker.Pick(SchemeRules.Name(kind) + " - hedef seç", targets, hero => TargetChoiceLabel(state, spymaster, hero, kind),
                hero => ConfirmStart(kind, hero), ShowNewScheme);
        }

        private static string TargetChoiceLabel(IntrigueState state, Hero spymaster, Hero target, SchemeKind kind) =>
            SchemeFactors.TargetLabel(target) + " - başarı " + SchemePreview.ChanceText(state, spymaster, target, kind) + ", "
            + SchemeFactors.Money(SchemeFactors.StartCost(kind, target));

        private static void ConfirmStart(SchemeKind kind, Hero target)
        {
            MenuUI.Confirm(SchemeRules.Name(kind) + " başlatılsın mı?", string.Join("\n", SchemePreview.Lines(SchemeEngine.State, kind, target)),
                () =>
                {
                    SchemeEngine.Start(kind, target);
                    MenuUI.Defer(ShowIntrigue);
                },
                () => PickSchemeTarget(kind));
        }

        // ---- Raporlar ve sırlar ----

        private static void ShowReports()
        {
            IntrigueState state = SchemeEngine.State;
            int today = SchemeFactors.Today;
            Picker.Pick("İstihbarat raporları", state.Reports.OrderByDescending(r => r.Day).ToList(),
                r => r.TargetName + " - gün " + r.Day + (today - r.Day > SchemeMath.ReportValidDays ? " (eski)" : "")
                    + SecretCountText(state, r.TargetId),
                ShowReport, ShowIntrigue, sort: false);
        }

        private static string SecretCountText(IntrigueState state, string targetId)
        {
            int open = state.Secrets.Count(s => s.TargetId == targetId && !s.Exposed);
            return open > 0 ? ", " + open + " sır" : "";
        }

        private static void ShowReport(IntelReport report)
        {
            IntrigueState state = SchemeEngine.State;
            Hero target = Hero.Find(report.TargetId);
            var entries = new List<MenuEntry>();
            foreach (Secret secret in state.Secrets.Where(s => s.TargetId == report.TargetId).ToList())
            {
                string name = SecretRules.Name(secret.Kind);
                string described = RomanceSecrets.Describe(secret);
                bool blackmailRunning = state.FindScheme(report.TargetId)?.Kind == SchemeKind.Blackmail || RomanceSecrets.BlackmailRunning(state, secret);
                if (!secret.Exposed && blackmailRunning)
                {
                    entries.Add(MenuEntry.Simple("İfşa et: " + name + " (şantaj sürerken olmaz)", () => MenuUI.Error("Önce şantajı bitir ya da iptal et")));
                    continue;
                }
                entries.Add(secret.Exposed
                    ? MenuEntry.Simple((secret.Whispered ? "(eşine söylendi) " : "(ifşa edildi) ") + name, () => MenuUI.Info(described))
                    : MenuEntry.Submenu("İfşa et: " + name + "...", () => MenuUI.Confirm("Sır ifşası: " + report.TargetName,
                        described + "\n\n" + RomanceSecrets.ExposePreview(secret) + " "
                        + "%" + SecretRules.SourceTracedChance + " ihtimalle kaynağın sen olduğun anlaşılır (ilişki -"
                        + SecretRules.SourceTracedPenalty + "). İfşa edilsin mi?",
                        () => SchemeOutcomes.ExposeSecret(state, secret),
                        () => ShowReport(report))));
                Hero betrayed = secret.Exposed ? null : RomanceSecrets.Betrayed(secret);
                if (betrayed != null && RomanceSecrets.WhisperBlocked(state, secret) == null)
                    entries.Add(MenuEntry.Submenu("Eşine söyle (" + betrayed.Name + "): " + name + "...", () => MenuUI.Confirm("Eşine söyle: " + betrayed.Name,
                        described + "\n\nSaray yerine yalnızca aldatılan eşe söylenir: aldatanla ve sevgiliyle arası bozulur, sana minnettar kalır (+"
                        + SecretRules.WhisperGratitude + "). Kayınlar ve hükümdar duymaz. Söylensin mi?",
                        () => RomanceSecrets.Whisper(state, secret),
                        () => ShowReport(report))));
            }
            if (target != null && SchemeFactors.BlockedReason(state, target, SchemeKind.Blackmail) == null)
                entries.Add(MenuEntry.Submenu("Bu sırla şantaj başlat...", () => StartFromReport(report, SchemeKind.Blackmail)));
            if (target != null && target.IsAlive && target.IsLord && target.Clan != Clan.PlayerClan)
            {
                string duelBlocked = Duels.Blocked(state, target);
                entries.Add(duelBlocked == null ? MenuEntry.Simple("Düelloya davet et...", () => UI.EventCards.ShowDuel(target, null))
                    : MenuEntry.Simple("Düelloya davet et - " + duelBlocked, () => MenuUI.Error(duelBlocked)));
            }
            if (target != null && SchemeFactors.BlockedReason(state, target, SchemeKind.Murder) == null)
                entries.Add(MenuEntry.Submenu("Bu hedefe suikast başlat...", () => StartFromReport(report, SchemeKind.Murder)));
            if (target != null && SchemeFactors.BlockedReason(state, target, SchemeKind.Kidnap) == null)
                entries.Add(MenuEntry.Submenu("Bu hedefi kaçırt...", () => StartFromReport(report, SchemeKind.Kidnap)));
            entries.Add(MenuEntry.Simple("Raporu sil", () =>
            {
                state.Reports.Remove(report);
                MenuUI.Success("Rapor silindi");
            }));

            string secretsNote = state.Secrets.Any(s => s.TargetId == report.TargetId) ? "" : "\n\nBilinen bir sırrı yok.";
            MenuUI.ShowList("Rapor: " + report.TargetName, entries,
                () => { if (state.Reports.Contains(report)) ShowReport(report); else ShowReports(); },
                "Geri", ShowReports, SchemeOutcomes.ReportText(report) + secretsNote);
        }

        private static void StartFromReport(IntelReport report, SchemeKind kind)
        {
            string reason = SchemeEngine.CannotStartReason(SchemeEngine.State);
            Hero target = Hero.Find(report.TargetId);
            if (reason != null || target == null)
            {
                MenuUI.Error(reason ?? "Hedef bulunamadı");
                MenuUI.Defer(() => ShowReport(report));
                return;
            }
            ConfirmStart(kind, target);
        }

        // ---- Kozlar ----

        private static void ShowHooks()
        {
            IntrigueState state = SchemeEngine.State;
            int today = SchemeFactors.Today;
            List<Hook> hooks = state.Hooks.Where(h => h.Until > today && Hero.Find(h.TargetId)?.IsAlive == true).ToList();
            if (hooks.Count == 0)
            {
                MenuUI.Info("Kimse üzerinde kozun yok. Önce casuslukla bir sır bul, sonra şantaj yap.");
                MenuUI.Defer(ShowIntrigue);
                return;
            }
            Picker.Pick("Kozlar", hooks, h => SchemeFactors.TargetLabel(Hero.Find(h.TargetId)) + " - " + (h.Until - today) + " gün kaldı",
                ShowHook, ShowIntrigue, h => HeroPortrait(Hero.Find(h.TargetId)), sort: false);
        }

        private static void ShowHook(Hook hook)
        {
            IntrigueState state = SchemeEngine.State;
            Hero target = Hero.Find(hook.TargetId);
            if (target == null || !state.Hooks.Contains(hook))
            {
                MenuUI.Defer(ShowHooks);
                return;
            }
            List<MenuEntry> entries = HookActions.Options(target)
                .Select(option => option.Blocked == null
                    ? MenuEntry.Submenu(option.Label + Messengers.HookRouteText(target, option.Use), () =>
                    {
                        // Başarılıysa menü kapalı kalır: sonuç popup'ı hemen görünsün.
                        bool done = false;
                        MenuUI.SafeRun("Koz", () =>
                        {
                            Messengers.UseHook(state, target, option.Use);
                            done = true;
                        });
                        if (!done) MenuUI.Defer(() => ShowHook(hook));
                    })
                    : MenuEntry.Simple(option.Label + " (" + option.Blocked + ")", () => MenuUI.Error(option.Blocked)))
                .ToList();
            MenuUI.ShowList("Koz: " + target.Name, entries, () => ShowHook(hook), "Geri", ShowHooks,
                (hook.Until - SchemeFactors.Today) + " gün geçerli. Bırakma dışındaki her talep kozu harcar ve ilişkiyi -"
                + SchemeMath.HookUseRelationPenalty + " düşürür. Aynı şeyleri onunla konuşurken \"özel bir konu\" açarak da yapabilirsin.");
        }

        // ---- Ulak ve görüşme (P2.2) ----

        private static void ShowAudiences()
        {
            IntrigueState state = SchemeEngine.State;
            var entries = new List<MenuEntry>();
            foreach (Audience audience in state.Audiences.Where(x => x.Until > SchemeFactors.Today).ToList())
            {
                Hero hero = Hero.Find(audience.HeroId);
                if (hero == null) continue;
                entries.Add(MenuEntry.Simple("Görüş: " + hero.Name + " (" + (audience.Until - SchemeFactors.Today) + " gün bekler)",
                    () => MenuUI.SafeRun("Görüşme", () => Messengers.OpenAudience(SchemeEngine.State, hero))));
            }
            entries.Add(MenuEntry.Submenu("Ulak gönder (görüşme daveti + hediye)...", PickAudienceTarget));
            MenuUI.ShowList("Ulak ve görüşme", entries, ShowAudiences, "Geri", ShowIntrigue,
                "Uzaktaki bir klan liderine ulak gönder: kabul ederse lordu bulmaya gitmeden onunla konuşursun (krallığına davet, evlilik, "
                + "görevler: oyunun kendi seçenekleri). Hediye ilişkiyi artırır (" + LetterRules.GiftCooldownDays + " günde bir).");
        }

        private static void PickAudienceTarget()
        {
            IntrigueState state = SchemeEngine.State;
            List<Hero> leaders = Hero.AllAliveHeroes
                .Where(h => h.IsLord && h.Clan != null && h.Clan != Clan.PlayerClan && h.Clan.Leader == h && !h.Clan.IsEliminated)
                .ToList();
            Picker.Pick("Ulak gönder", leaders,
                h => h.Name + " - " + SchemeFactors.Affiliation(h) + ", ilişki " + Hero.MainHero.GetRelation(h)
                    + (Messengers.AudienceBlocked(state, h) is string why ? "  [" + why + "]" : ""),
                h => UI.EventCards.ShowAudienceSend(h, null), ShowAudiences, HeroPortrait);
        }

        // ---- Kur ve sevgililer (P2.6) ----

        private static void ShowRomance()
        {
            IntrigueState state = SchemeEngine.State;
            if (!Courtships.Enabled)
            {
                MenuUI.Info("Kurallar'dan kapalı: \"Gizli ilişki ve gayrimeşru çocuk sırları\" ayarı");
                MenuUI.Defer(ShowIntrigue);
                return;
            }
            var entries = new List<MenuEntry>();
            foreach (Courtship c in state.Courtships.ToList())
            {
                Hero target = Hero.Find(c.TargetId);
                entries.Add(MenuEntry.Submenu("Kuru bırak: " + target?.Name + " (" + RomanceRules.StageName(c.Stage) + ")",
                    () => MenuUI.Confirm("Kuru bırak", "Harcanan hediyeler geri gelmez. Bırakılsın mı?", () => Courtships.Cancel(SchemeEngine.State, c), ShowRomance)));
            }
            foreach (Lover lover in state.Lovers.ToList())
            {
                Hero hero = Hero.Find(lover.HeroId);
                if (hero == null) continue;
                string blocked = Courtships.MeetBlocked(state, lover);
                entries.Add(MenuEntry.Simple("Gizli buluşma: " + hero.Name + (blocked != null ? " [" + blocked + "]" : ""), () => UI.EventCards.ShowMeeting(hero, null)));
                entries.Add(MenuEntry.Submenu("İlişkiyi bitir: " + hero.Name + "...",
                    () => MenuUI.Confirm("İlişkiyi bitir", hero.Name + " ile ilişkin bitsin mi?", () => Courtships.EndAffair(SchemeEngine.State, lover), ShowRomance)));
            }
            foreach (Pregnancy p in state.Pregnancies.Where(p => !p.Hidden || Hero.Find(p.MotherId)?.Clan == Clan.PlayerClan))
                entries.Add(MenuEntry.Simple(Hero.Find(p.MotherId)?.Name + " hamile: doğum " + Math.Max(0, p.DueDay - SchemeFactors.Today) + " gün sonra",
                    () => MenuUI.Info("Doğum yaklaşınca kartla haber gelir")));
            if (Courtships.Spouse != null)
            {
                if (SpouseLife.Enabled)
                {
                    string blocked = SpouseLife.InvestigateBlocked(state);
                    string label = "Eşini araştır (casus başı, " + SchemeFactors.Money(FidelityRules.InvestigationCost) + ", " + FidelityRules.InvestigationDays + " gün)";
                    entries.Add(blocked == null ? MenuEntry.Simple(label, () => SpouseLife.Investigate(SchemeEngine.State))
                        : MenuEntry.Simple(label + " - " + blocked, () => MenuUI.Error(blocked)));
                }
                if (state.SpouseAffair != null && state.SpouseAffair.Discovered)
                    entries.Add(MenuEntry.Simple("Yüzleş: eşinden ilişkiyi bitirmesini iste", () => SpouseLife.Confront(SchemeEngine.State)));
                string divorceBlocked = SpouseLife.DivorceBlocked(state);
                entries.Add(divorceBlocked == null ? MenuEntry.Submenu("Boşa...", () => UI.ViewModels.DossierVM.ConfirmDivorce(null))
                    : MenuEntry.Simple("Boşa - " + divorceBlocked, () => MenuUI.Error(divorceBlocked)));
            }
            entries.Add(MenuEntry.Submenu("Kur yap...", () => PickCourtshipTarget(CourtshipMode.Lover)));
            if (Courtships.Spouse != null) entries.Add(MenuEntry.Submenu("Eşini gönder: bilgi için kur...", () => PickCourtshipTarget(CourtshipMode.Intel)));
            MenuUI.ShowList("Kur ve sevgililer", entries, ShowRomance, "Geri", ShowIntrigue,
                "Kur üç aşamada (" + RomanceRules.StageCount * RomanceRules.StageDays + " gün) tutar ya da tutmaz. Eşin, kabul ederse, bilgi için kur yapar. "
                + "Yalnızca yetişkinler; olanlar metinle anlatılır, sahne yok.");
        }

        private static void PickCourtshipTarget(CourtshipMode mode)
        {
            IntrigueState state = SchemeEngine.State;
            Hero agent = Courtships.AgentFor(mode);
            if (agent == null)
            {
                MenuUI.Error("Klanında hayatta bir eşin yok");
                MenuUI.Defer(ShowRomance);
                return;
            }
            List<Hero> lords = Hero.AllAliveHeroes
                .Where(h => h.IsLord && !h.IsChild && h.Clan != null && h.Clan != Clan.PlayerClan && h.IsFemale != agent.IsFemale
                    && SecretRules.IsAdult(RomanceSecrets.Age(h)))
                .ToList();
            Picker.Pick(mode == CourtshipMode.Lover ? "Kime kur yapacaksın?" : "Eşin kime kur yapsın?", lords,
                h => h.Name + " - " + SchemeFactors.Affiliation(h) + ", " + RomanceSecrets.Age(h) + " yaş"
                    + (Courtships.StartBlocked(state, h, mode) is string why ? "  [" + why + "]" : "  (%" + Courtships.Chance(agent, h) + ")"),
                h => UI.EventCards.ShowCourtshipStart(h, mode, null), ShowRomance, HeroPortrait);
        }

        // ---- Esir sorgusu ----

        private static void ShowPrisoners()
        {
            IntrigueState state = SchemeEngine.State;
            List<Interrogation.Prisoner> prisoners = Interrogation.Prisoners();
            if (prisoners.Count == 0)
            {
                MenuUI.Info("Elinde esir bir kahraman yok");
                MenuUI.Defer(ShowIntrigue);
                return;
            }
            Picker.Pick("Esirleri sorgula", prisoners,
                p => p.Hero.Name + " - " + p.WhereText + (Interrogation.BlockedReason(state, p) is string why ? "  [" + why + "]" : ""),
                p => UI.EventCards.ShowInterrogation(p, null), ShowIntrigue, p => HeroPortrait(p.Hero), sort: false);
        }

        // ---- Gölge ağları ----

        private static void ShowNetworks()
        {
            IntrigueState state = SchemeEngine.State;
            if (state.Networks.Count == 0)
            {
                MenuUI.Info("Gölge ağın yok. Çete lideri olan bir şehre git: kasaba menüsünde \"Gölgelerde iş çevir\".");
                MenuUI.Defer(ShowIntrigue);
                return;
            }
            Picker.Pick("Gölge ağları", state.Networks.ToList(),
                n => Settlement.Find(n.SettlementId)?.Name + " - seviye " + n.Level + ", haftalık " + SchemeFactors.Money(SchemeMath.NetworkUpkeep(n.Level)),
                n =>
                {
                    Settlement town = Settlement.Find(n.SettlementId);
                    if (town != null) ShowShadowTown(town, false);
                    else MenuUI.Defer(ShowNetworks);
                },
                ShowIntrigue, sort: false);
        }

        /// <param name="inTown">Şehirdeyken kurulup güçlendirilir; uzaktan yalnızca bakılır ve dağıtılır.</param>
        public static void ShowShadowTown(Settlement town, bool inTown)
        {
            IntrigueState state = SchemeEngine.State;
            ShadowNetwork network = state.Network(town.StringId);
            var entries = new List<MenuEntry>();
            if (inTown)
            {
                int level = (network?.Level ?? 0) + 1;
                if (level <= SchemeMath.MaxNetworkLevel)
                {
                    string blocked = ShadowNetworks.UpgradeBlocked(state, town);
                    string label = (network == null ? "Ağ kur" : "Ağı güçlendir: seviye " + level) + " (" + SchemeFactors.Money(SchemeMath.NetworkCost(level))
                        + ", çete lideriyle ilişki en az " + SchemeMath.NetworkRelationRequired(level) + ")";
                    entries.Add(blocked == null
                        ? MenuEntry.Simple(label, () => ShadowNetworks.Upgrade(state, town))
                        : MenuEntry.Simple(label + " - " + blocked, () => MenuUI.Error(blocked)));
                }
                entries.Add(MenuEntry.Simple("Çete liderine hediye gönder (" + SchemeFactors.Money(ShadowNetworks.GiftCost) + ", ilişki +"
                    + ShadowNetworks.GiftRelation + ")", () => ShadowNetworks.Gift(town)));
            }
            if (network != null && NightHouses.Enabled) AddHouseEntries(state, town, inTown, entries);
            if (network != null)
                entries.Add(MenuEntry.Submenu("Ağı dağıt...", () => MenuUI.Confirm("Ağı dağıt",
                    "Harcanan para geri gelmez." + (state.HouseIn(town.StringId) != null ? " Gece evi de kapanır." : "") + " Dağıtılsın mı?",
                    () => ShadowNetworks.Disband(state, town), () => ShowShadowTown(town, inTown))));
            if (entries.Count == 0) entries.Add(MenuEntry.Simple("Ağ kurmak için bu şehre git", () => { }));

            MenuUI.ShowList("Gölgeler: " + town.Name, entries, () => ShowShadowTown(town, inTown), inTown ? "Kapat" : "Geri",
                inTown ? (Action)null : ShowNetworks, ShadowNetworks.Describe(state, town));
        }

        /// <summary>Gece evi: açmak ve çalışan almak şehirdeyken; payı değiştirmek ve kapatmak uzaktan da.</summary>
        private static void AddHouseEntries(IntrigueState state, Settlement town, bool inTown, List<MenuEntry> entries)
        {
            House house = state.HouseIn(town.StringId);
            if (house == null)
            {
                if (!inTown) return;
                string blocked = NightHouses.OpenBlocked(state, town);
                string label = "Gece evi aç (" + SchemeFactors.Money(HouseRules.OpenCost) + ", " + HouseRules.StartStaff + " gönüllü çalışan)";
                entries.Add(blocked == null
                    ? MenuEntry.Simple(label, () => NightHouses.Open(state, town))
                    : MenuEntry.Simple(label + " - " + blocked, () => MenuUI.Error(blocked)));
                return;
            }
            if (inTown)
            {
                string blocked = NightHouses.HireBlocked(state, town);
                string label = "Çalışan al (" + SchemeFactors.Money(HouseRules.HireCost) + "; " + house.Staff + "/" + NightHouses.MaxStaff(state, town) + ")";
                entries.Add(blocked == null
                    ? MenuEntry.Simple(label, () => NightHouses.Hire(state, town))
                    : MenuEntry.Simple(label + " - " + blocked, () => MenuUI.Error(blocked)));
            }
            foreach (HouseShare share in new[] { HouseShare.Fair, HouseShare.Standard, HouseShare.Low })
            {
                if (share == house.Share) continue;
                entries.Add(MenuEntry.Simple("Çalışanların payı: " + HouseRules.ShareName(share)
                    + (HouseRules.LeaveChance(share) > 0 ? " (ayrılma %" + HouseRules.LeaveChance(share) + "/hafta)" : " (kimse ayrılmaz, daha çok haber)"),
                    () => NightHouses.SetShare(state, town, share)));
            }
            entries.Add(MenuEntry.Submenu("Gece evini kapat...", () => MenuUI.Confirm("Gece evini kapat", "Açılış parası geri gelmez. Kapatılsın mı?",
                () => NightHouses.Close(state, town), () => ShowShadowTown(town, inTown))));
        }

        // ---- Sana karşı entrikalar ----

        private static void ShowPlotsAgainstMe()
        {
            IntrigueState state = SchemeEngine.State;
            List<AiPlot> known = state.AiPlots.Where(p => p.Detected).ToList();
            if (known.Count == 0)
            {
                MenuUI.Info("Sezdiğin bir entrika yok. Casus başının Roguery'si yüksekse sana karşı kurulanları daha kolay sezer.");
                MenuUI.Defer(ShowIntrigue);
                return;
            }
            List<MenuEntry> entries = known.Select(plot => MenuEntry.Submenu(AiIntrigue.PlotLine(plot), () => ShowPlot(plot))).ToList();
            MenuUI.ShowList("Sana karşı entrikalar", entries, ShowPlotsAgainstMe, "Geri", ShowIntrigue,
                "Yalnızca casus başının sezdikleri görünür. Yapay zekâ entrikaları Kurallar'dan kapatılabilir ya da azaltılabilir.");
        }

        private static void ShowPlot(AiPlot plot)
        {
            IntrigueState state = SchemeEngine.State;
            if (!state.AiPlots.Contains(plot))
            {
                MenuUI.Defer(ShowPlotsAgainstMe);
                return;
            }
            List<MenuEntry> entries = IntriguePopups.PlotOptions(state, plot)
                .Where(o => !o.Label.StartsWith("Şimdilik"))
                .Select(o => o.DisabledHint == null
                    ? MenuEntry.Submenu(o.Label, () =>
                    {
                        MenuUI.SafeRun("Karşı hamle", o.Apply);
                        MenuUI.Defer(ShowPlotsAgainstMe);
                    })
                    : MenuEntry.Simple(o.Label + " (" + o.DisabledHint + ")", () => MenuUI.Error(o.DisabledHint)))
                .ToList();
            MenuUI.ShowList(AiIntrigue.PlotLine(plot), entries, () => ShowPlot(plot), "Geri", ShowPlotsAgainstMe);
        }

        private static void ShowIntrigueLog()
        {
            IntrigueState state = SchemeEngine.State;
            string text = state.Log.Count == 0
                ? "Henüz bir şey olmadı."
                : string.Join("\n", Enumerable.Reverse(state.Log).Select(e => "Gün " + e.Day + ": " + e.Text));
            InformationManager.ShowInquiry(
                new InquiryData("Entrika geçmişi", text, true, false, "Geri", string.Empty, () => MenuUI.Defer(ShowIntrigue), null),
                true, false);
        }

        // ---- Hileler ----

        private static void ShowIntrigueCheats()
        {
            ShowCategory("Entrika hileleri", new List<MenuEntry>
            {
                MenuEntry.Simple("Süren entrikaların aşamalarını tamamla", SchemeEngine.CompleteAllPhases),
                MenuEntry.Toggle("Asla fark edilme", () => CheatState.IntrigueNeverDiscovered, v => CheatState.IntrigueNeverDiscovered = v),
                MenuEntry.Toggle("Suikast ve casusluk her zaman başarılı", () => CheatState.IntrigueAlwaysSucceed,
                    v => CheatState.IntrigueAlwaysSucceed = v),
            }, ShowIntrigueCheats, "Aç/kapa hileler oturumluktur: oyunu kapatınca kapanır.", ShowCheats);
        }

        internal static ImageIdentifier HeroPortrait(Hero hero) =>
            new CharacterImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));
    }
}
