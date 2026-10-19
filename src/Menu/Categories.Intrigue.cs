using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.Menu
{
    /// <summary>Entrika menüsü (tasarım 9.1) ve entrika hileleri.</summary>
    public static partial class Categories
    {
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
            if (scheme.Kind == SchemeKind.Murder)
            {
                var atMax = SchemeFactors.Inputs(state, spymaster, target, scheme.Kind, scheme);
                atMax.CompletedPhases = Math.Max(scheme.Phase, max);
                lines.Add("Başarı şu an: %" + SchemeMath.MurderSuccess(inputs) + " (" + max + ". aşamada %" + SchemeMath.MurderSuccess(atMax)
                    + "; harekete geçerken rüşvetle +5/+10/+15)");
                if (scheme.Method != SchemeMethod.None) lines.Add("Yöntem: " + MethodName(scheme.Method));
                if (scheme.FeastRisk) lines.Add("Ziyafette vurulacak: ortaya çıkarsa cezalar 1,5 kat");
            }
            else lines.Add("Başarı: %" + SchemeMath.SpySuccess(inputs));
            lines.Add("Her aşamada yakalanma: %" + SchemeMath.Discovery(scheme.Kind, inputs));
            if (spymaster == null) lines.Add("DURDU: casus başın yok");
            else if (SchemeFactors.SpymasterBusyReason(spymaster) != null)
                lines.Add("DURDU: casus başın şu an " + SchemeFactors.SpymasterBusyReason(spymaster));

            var entries = new List<MenuEntry>();
            if (scheme.Kind == SchemeKind.Murder)
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
                MenuEntry.Submenu("Suikast: hedefi öldür (2-5 aşama, çok yüksek risk)...", () => PickSchemeTarget(SchemeKind.Murder)),
            }, ShowNewScheme, "Geri", ShowIntrigue,
                "Casusluk raporu, o hedefe suikastta +10 başarı verir ve tanışmadığın birini de suikast listesine ekler.");
        }

        private static void PickSchemeTarget(SchemeKind kind)
        {
            IntrigueState state = SchemeEngine.State;
            Hero spymaster = SchemeFactors.Spymaster(state);
            List<Hero> targets = SchemeFactors.Targets(state, kind).ToList();
            if (targets.Count == 0)
            {
                MenuUI.Error(kind == SchemeKind.Murder
                    ? "Suikast için tanıştığın ya da hakkında güncel raporun olan, yeri belli bir hedef gerekir"
                    : "Casusluk yapılacak kimse bulunamadı");
                MenuUI.Defer(ShowNewScheme);
                return;
            }
            Picker.Pick(SchemeRules.Name(kind) + " - hedef seç", targets, hero => TargetChoiceLabel(state, spymaster, hero, kind),
                hero => ConfirmStart(kind, hero), ShowNewScheme);
        }

        private static string TargetChoiceLabel(IntrigueState state, Hero spymaster, Hero target, SchemeKind kind)
        {
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, kind, null);
            string chance;
            if (kind == SchemeKind.Murder)
            {
                int low = SchemeMath.MurderSuccess(inputs);
                inputs.CompletedPhases = SchemeRules.MaxPhases(kind);
                chance = "%" + low + "-" + SchemeMath.MurderSuccess(inputs);
            }
            else chance = "%" + SchemeMath.SpySuccess(inputs);
            return SchemeFactors.TargetLabel(target) + " - başarı " + chance + ", " + SchemeFactors.Money(SchemeFactors.StartCost(kind, target));
        }

        private static void ConfirmStart(SchemeKind kind, Hero target)
        {
            IntrigueState state = SchemeEngine.State;
            Hero spymaster = SchemeFactors.Spymaster(state);
            SchemeInputs inputs = SchemeFactors.Inputs(state, spymaster, target, kind, null);
            int cost = SchemeFactors.StartCost(kind, target);
            int days = SchemeMath.PhaseDays(kind, SchemeFactors.Roguery(spymaster));
            int today = SchemeFactors.Today;

            var lines = new List<string>
            {
                "Hedef: " + SchemeFactors.TargetLabel(target),
                "Maliyet: " + SchemeFactors.Money(cost) + " (sende " + SchemeFactors.Money(Hero.MainHero.Gold) + ")",
            };
            if (kind == SchemeKind.Murder)
            {
                int low = SchemeMath.MurderSuccess(inputs);
                inputs.CompletedPhases = SchemeRules.MaxPhases(kind);
                lines.Add("Aşamalar: en az 2, en fazla 5; her aşama yaklaşık " + days + " gün");
                lines.Add("Başarı: 2 aşamada %" + low + ", 5 aşamada %" + SchemeMath.MurderSuccess(inputs)
                    + " (harekete geçerken rüşvetle +5/+10/+15)");
                lines.Add("Her aşamada yakalanma: %" + SchemeMath.MurderDiscovery(inputs));
                lines.Add(state.HasValidReport(target.StringId, today)
                    ? "Güncel istihbarat raporun var: +10 başarı."
                    : "Güncel raporun yok: önce casusluk yaparsan +10 başarı.");
                if (inputs.TargetAlerted) lines.Add("Hedef tetikte: -15 başarı.");
                if (inputs.TargetIsPlayersCaptive) lines.Add("Hedef senin elinde: başarı +20, ama ölürse ilk senden şüphelenilir (iz bırakma ×2).");
                lines.Add("");
                lines.Add("Ortaya çıkarsa: ağır ilişki cezası, suç puanı ve Onur kaybı. Hedef hükümdarsa savaş çıkabilir; "
                    + "kendi krallığındansa hükümdarın hesap sorar.");
            }
            else
            {
                lines.Add("Aşamalar: 1-2; her aşama yaklaşık " + days + " gün (2. aşama yalnızca sır bulma şansını artırır)");
                lines.Add("Başarı: %" + SchemeMath.SpySuccess(inputs) + ", her aşamada yakalanma: %" + SchemeMath.SpyDiscovery(inputs));
                if (!target.HasMet) lines.Add("Tanışmadınız: başarı -10.");
                lines.Add("");
                lines.Add("Sonuç: " + SchemeMath.ReportValidDays + " gün geçerli rapor (konum, güç, altın, ilişkiler, kişilik)"
                    + (target.IsLord ? " ve belki bir sır." : ". Sırlar yalnızca lordlarda aranır."));
            }

            MenuUI.Confirm(SchemeRules.Name(kind) + " başlatılsın mı?", string.Join("\n", lines),
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
            foreach (Secret secret in state.Secrets.Where(s => s.TargetId == report.TargetId))
            {
                string name = SecretRules.Name(secret.Kind);
                entries.Add(secret.Exposed
                    ? MenuEntry.Simple("(ifşa edildi) " + name, () => MenuUI.Info(name + " zaten herkesin ağzında"))
                    : MenuEntry.Submenu("İfşa et: " + name + "...", () => MenuUI.Confirm("Sır ifşası: " + report.TargetName,
                        SecretRules.Description(secret.Kind) + "\n\nİfşa edilirse hükümdarıyla (gizli ilişkide eşiyle) arası açılır. "
                        + "%" + SecretRules.SourceTracedChance + " ihtimalle kaynağın sen olduğun anlaşılır (seninle ilişkisi -"
                        + SecretRules.SourceTracedPenalty + "). İfşa edilsin mi?",
                        () => SchemeOutcomes.ExposeSecret(state, secret),
                        () => ShowReport(report))));
            }
            if (target != null && SchemeFactors.BlockedReason(state, target, SchemeKind.Murder) == null)
                entries.Add(MenuEntry.Submenu("Bu hedefe suikast başlat...", () =>
                {
                    string reason = SchemeEngine.CannotStartReason(state);
                    if (reason != null)
                    {
                        MenuUI.Error(reason);
                        MenuUI.Defer(() => ShowReport(report));
                        return;
                    }
                    ConfirmStart(SchemeKind.Murder, target);
                }));
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

        private static ImageIdentifier HeroPortrait(Hero hero) =>
            new CharacterImageIdentifier(CampaignUIHelper.GetCharacterCode(hero.CharacterObject, false));
    }
}
