using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core.ViewModelCollection.ImageIdentifiers;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    /// <summary>
    /// "Saray" sekmesi (P2.2): bir krallığın klan liderleri ve hükümdarlarıyla ilişkileri, bildiğin kadarıyla
    /// (rapor: tam sayı, gölge ağı ya da kendi sarayın: söylenti, yoksa bilinmiyor). Kendi tarafına çekmek oyunun ikna konuşmasıyla;
    /// buradan lord haritada izlenir, hakkında söylenti yayılır ya da tam bilgi için casusluk başlatılır.
    /// </summary>
    public sealed class CourtVM : TvhViewModel
    {
        private readonly Action _changed;
        private readonly Action<SchemeKind, Hero> _startScheme;
        private int _index;
        private string _kingdomName = "", _rulerName = "", _rulerLine = "", _sourceText = "";
        private bool _hasKingdom, _hasNoLords, _canSwitch;
        private ImageIdentifierVM _rulerPortrait;

        public CourtVM(Action changed, Action<SchemeKind, Hero> startScheme)
        {
            _changed = changed;
            _startScheme = startScheme;
            Lords = new MBBindingList<CourtLordItemVM>();
            _rulerPortrait = PortraitOf(null);
        }

        [DataSourceProperty] public MBBindingList<CourtLordItemVM> Lords { get; }
        [DataSourceProperty] public bool HasKingdom
        {
            get => _hasKingdom;
            set
            {
                if (Set(ref _hasKingdom, value, nameof(HasKingdom))) OnPropertyChangedWithValue(!value, nameof(HasNoKingdom));
            }
        }
        [DataSourceProperty] public bool HasNoKingdom => !_hasKingdom;
        [DataSourceProperty] public bool HasNoLords { get => _hasNoLords; set => Set(ref _hasNoLords, value, nameof(HasNoLords)); }
        [DataSourceProperty] public bool CanSwitch { get => _canSwitch; set => Set(ref _canSwitch, value, nameof(CanSwitch)); }
        [DataSourceProperty] public string KingdomName { get => _kingdomName; set => Set(ref _kingdomName, value, nameof(KingdomName)); }
        [DataSourceProperty] public ImageIdentifierVM RulerPortrait { get => _rulerPortrait; set => Set(ref _rulerPortrait, value, nameof(RulerPortrait)); }
        [DataSourceProperty] public string RulerName { get => _rulerName; set => Set(ref _rulerName, value, nameof(RulerName)); }
        [DataSourceProperty] public string RulerLine { get => _rulerLine; set => Set(ref _rulerLine, value, nameof(RulerLine)); }
        [DataSourceProperty] public string SourceText { get => _sourceText; set => Set(ref _sourceText, value, nameof(SourceText)); }
        [DataSourceProperty] public string GuideText => Intrigue.Court.RecruitGuide;
        [DataSourceProperty] public string PrevText => "« Önceki";
        [DataSourceProperty] public string NextText => "Sonraki »";
        [DataSourceProperty] public string EmptyText => "Bu krallıkta hükümdardan başka klan lideri yok.";
        [DataSourceProperty] public string NoKingdomText => "Calradia'da ayakta bir krallık yok.";

        /// <summary>Senin krallığın önce, sonra diğerleri ada göre.</summary>
        private static List<Kingdom> Ordered() =>
            Court.Kingdoms().OrderBy(k => k == Clan.PlayerClan.Kingdom ? 0 : 1).ThenBy(k => k.Name.ToString()).ToList();

        public void Refresh(IntrigueState state)
        {
            List<Kingdom> kingdoms = Ordered();
            Clear(Lords);
            HasKingdom = kingdoms.Count > 0;
            CanSwitch = kingdoms.Count > 1;
            if (kingdoms.Count == 0) return;
            _index = (_index % kingdoms.Count + kingdoms.Count) % kingdoms.Count;
            Kingdom kingdom = kingdoms[_index];
            Hero ruler = kingdom.Leader;

            KingdomName = kingdom.Name + (kingdom == Clan.PlayerClan.Kingdom ? " (senin krallığın)" : "") + "   " + (_index + 1) + "/" + kingdoms.Count;
            _rulerPortrait?.OnFinalize();
            RulerPortrait = PortraitOf(ruler);
            RulerName = ruler.Name.ToString();
            float ours = Clan.PlayerClan.Kingdom?.CurrentTotalStrength ?? 0f;
            RulerLine = "Hükümdar  ·  seninle ilişkisi " + SchemeFactors.Signed(Hero.MainHero.GetRelation(ruler))
                + "  ·  krallığın gücü " + ((int)kingdom.CurrentTotalStrength).ToString("N0")
                + (Clan.PlayerClan.Kingdom != null && kingdom != Clan.PlayerClan.Kingdom ? " (seninki " + ((int)ours).ToString("N0") + ")" : "");
            SourceText = Court.SourceText(state, kingdom) + DiplomaticCrises.TensionLine(state, kingdom);

            // Önce bilinen hoşnutsuzlar (en kötüsü en üstte), sonra söylentiler, en sonda bilinmeyenler.
            IEnumerable<Hero> ordered = Court.ClanLeaders(kingdom)
                .OrderByDescending(h => (int)Court.KnowledgeOf(state, h))
                .ThenBy(h => Court.KnowledgeOf(state, h) == Court.Knowledge.Unknown ? 0 : Court.RelationWithRuler(h))
                .ThenBy(h => h.Name.ToString());
            foreach (Hero lord in ordered) Lords.Add(new CourtLordItemVM(state, lord, _changed, _startScheme));
            HasNoLords = Lords.Count == 0;
        }

        public void ExecutePrevKingdom()
        {
            _index--;
            Refresh(SchemeEngine.State);
        }

        public void ExecuteNextKingdom()
        {
            _index++;
            Refresh(SchemeEngine.State);
        }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Clear(Lords);
            _rulerPortrait?.OnFinalize();
        }
    }

    /// <summary>Saraydaki bir klan lideri.</summary>
    public sealed class CourtLordItemVM : TvhViewModel
    {
        public CourtLordItemVM(IntrigueState state, Hero lord, Action changed, Action<SchemeKind, Hero> startScheme)
        {
            int today = SchemeFactors.Today;
            Portrait = PortraitOf(lord);
            Name = lord.Name.ToString();
            ClanText = lord.Clan.Name + "  ·  kademe " + lord.Clan.Tier + (lord.Clan.IsUnderMercenaryService ? "  ·  paralı asker" : "");
            Court.Knowledge knowledge = Court.KnowledgeOf(state, lord);
            int relation = Court.RelationWithRuler(lord);
            MoodText = "Hükümdarıyla: " + Court.MoodText(state, lord);
            IsDiscontent = knowledge != Court.Knowledge.Unknown && LetterRules.IsDiscontent(relation);
            IsNotDiscontent = !IsDiscontent;
            YouText = "Seninle: " + SchemeFactors.Signed(Hero.MainHero.GetRelation(lord)) + (lord.HasMet ? "" : "  ·  tanışmadınız");
            var tags = new List<string>();
            IntelReport report = state.LatestReport(lord.StringId);
            if (report != null && state.HasValidReport(lord.StringId, today)) tags.Add("rapor: gün " + report.Day);
            if (state.ActiveHook(lord.StringId, today) != null) tags.Add("üzerinde kozun var");
            if (Messengers.PendingTo(state, lord) != null) tags.Add("haberci yolda");
            int bonus = IntriguePersuasionModel.Bonus(lord);
            if (bonus > 0) tags.Add("ikna +" + bonus);
            TagsText = string.Join("  ·  ", tags);

            Actions = new MBBindingList<ActionItemVM>();
            Actions.Add(LordActions.Audience(state, lord, changed));
            foreach (ActionItemVM action in LordActions.Romance(state, lord, changed)) Actions.Add(action);
            Actions.Add(new ActionItemVM("Haritada izle (konuşmaya git)", null, () =>
            {
                string missing = Intrigue.Court.Track(lord);
                if (missing != null) throw new InvalidOperationException(missing);
                Menu.MenuUI.Success(lord.Name + " haritada izleniyor");
            }));
            bool rumoring = state.Mission == SpymasterMission.SpreadRumors && state.RumorTargetId == lord.StringId;
            Actions.Add(new ActionItemVM(rumoring ? "Söylenti yayılıyor (casus başının görevi)" : "Hakkında söylenti yay (casus başı görevi)",
                rumoring ? "zaten bu lord" : SchemeFactors.Spymaster(state) == null ? "Önce bir casus başı ata" : SpymasterMissions.RumorBlocked(state, lord),
                () =>
                {
                    SpymasterMissions.SetMission(SchemeEngine.State, SpymasterMission.SpreadRumors, lord);
                    changed();
                }));
            if (knowledge != Intrigue.Court.Knowledge.Exact)
                Actions.Add(new ActionItemVM("Casusluk (tam bilgi için)...",
                    SchemeEngine.CannotStartReason(state) ?? SchemeFactors.BlockedReason(state, lord, SchemeKind.Spy),
                    () => startScheme(SchemeKind.Spy, lord)));
        }

        [DataSourceProperty] public ImageIdentifierVM Portrait { get; }
        [DataSourceProperty] public string Name { get; }
        [DataSourceProperty] public string ClanText { get; }
        [DataSourceProperty] public string MoodText { get; }
        [DataSourceProperty] public bool IsDiscontent { get; }
        [DataSourceProperty] public bool IsNotDiscontent { get; }
        [DataSourceProperty] public string YouText { get; }
        [DataSourceProperty] public string TagsText { get; }
        [DataSourceProperty] public MBBindingList<ActionItemVM> Actions { get; }

        public override void OnFinalize()
        {
            base.OnFinalize();
            Portrait?.OnFinalize();
        }
    }

    /// <summary>Saray ve kişi dosyasında ortak lord eylemleri.</summary>
    internal static class LordActions
    {
        /// <summary>Kur ve sevgili (P2.6): sevgilinse buluşma ve bitirme; kur sürüyorsa bırakma; değilse kur yap ve eşini gönder.</summary>
        public static IEnumerable<ActionItemVM> Romance(IntrigueState state, Hero lord, Action changed)
        {
            if (!Courtships.Enabled) yield break;
            Lover lover = state.LoverOf(lord.StringId);
            if (lover != null)
            {
                yield return new ActionItemVM("Gizli buluşma...", Courtships.MeetBlocked(state, lover), () => EventCards.ShowMeeting(lord, changed));
                yield return new ActionItemVM("İlişkiyi bitir", null, () => Menu.MenuUI.Confirm("İlişkiyi bitir", lord.Name + " ile ilişkin bitsin mi? (ilişki -"
                    + RomanceRules.EndRelation + ")", () => { Courtships.EndAffair(SchemeEngine.State, lover); changed?.Invoke(); }, null), true);
                yield break;
            }
            Courtship running = state.Courtships.FirstOrDefault(c => c.TargetId == lord.StringId);
            if (running != null)
            {
                yield return new ActionItemVM("Kuru bırak (" + RomanceRules.StageName(running.Stage).ToLower(Tr.Culture) + ")", null, () =>
                {
                    Courtships.Cancel(SchemeEngine.State, running);
                    changed?.Invoke();
                }, true);
                yield break;
            }
            yield return new ActionItemVM("Kur yap...", Courtships.StartBlocked(state, lord, CourtshipMode.Lover),
                () => EventCards.ShowCourtshipStart(lord, CourtshipMode.Lover, changed));
            if (Courtships.Spouse != null)
                yield return new ActionItemVM("Eşini gönder: bilgi için kur...", Courtships.StartBlocked(state, lord, CourtshipMode.Intel),
                    () => EventCards.ShowCourtshipStart(lord, CourtshipMode.Intel, changed));
        }

        /// <summary>Görüşmeye hazırsa "Görüş", değilse "Ulak gönder".</summary>
        public static ActionItemVM Audience(IntrigueState state, Hero lord, Action changed)
        {
            Audience audience = Messengers.AudienceWith(state, lord);
            if (audience != null)
                return new ActionItemVM("Görüş: konuşma penceresi (ulak bekliyor, " + Math.Max(1, audience.Until - SchemeFactors.Today) + " gün)",
                    lord.IsPrisoner ? "Şu an esir" : null, () => Menu.MenuUI.Defer(() => Menu.MenuUI.SafeRun("Görüşme", () => Messengers.OpenAudience(SchemeEngine.State, lord))));
            Letter pending = Messengers.PendingTo(state, lord);
            return new ActionItemVM("Ulak gönder: görüşme + hediye...",
                pending != null ? "Ona giden bir ulak yolda (" + Math.Max(0, pending.ArriveDay - SchemeFactors.Today) + " gün)" : Messengers.AudienceBlocked(state, lord),
                () => EventCards.ShowAudienceSend(lord, changed));
        }
    }

    /// <summary>Kur, sevgili ya da gebelik satırı ("Süren" sekmesinde, haberci satırıyla aynı görünüm).</summary>
    public sealed class RomanceRowVM : TvhViewModel
    {
        public RomanceRowVM(string text, string days)
        {
            Text = text;
            DaysText = days;
        }

        [DataSourceProperty] public string Text { get; }
        [DataSourceProperty] public string DaysText { get; }
    }

    /// <summary>Yoldaki haberci satırı ("Süren" sekmesinde).</summary>
    public sealed class LetterItemVM : TvhViewModel
    {
        public LetterItemVM(Letter letter)
        {
            Hero target = Hero.Find(letter.TargetId);
            string name = target?.Name.ToString() ?? letter.TargetId;
            Text = letter.Kind == LetterKind.Audience
                ? "Görüşme daveti" + (letter.Use > 0 ? " (hediye " + SchemeFactors.Money(letter.Use) + ")" : "") + ": " + name
                : "Koz talebi (" + HookUseName((HookActions.Use)letter.Use) + "): " + name;
            int left = Math.Max(0, letter.ArriveDay - SchemeFactors.Today);
            DaysText = left == 0 ? "bugün varıyor" : left + " gün sonra varır";
        }

        [DataSourceProperty] public string Text { get; }
        [DataSourceProperty] public string DaysText { get; }

        private static string HookUseName(HookActions.Use use)
        {
            switch (use)
            {
                case HookActions.Use.Gold: return "altın";
                case HookActions.Use.Influence: return "etki";
                case HookActions.Use.ReleasePrisoners: return "esirleri bıraksın";
                case HookActions.Use.DefectToMe: return "krallığıma katılsın";
                case HookActions.Use.LeaveKingdom: return "krallığından ayrılsın";
                default: return "kozu bırak";
            }
        }
    }
}
