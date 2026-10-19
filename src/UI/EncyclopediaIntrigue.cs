using System;
using System.Collections.Generic;
using System.Linq;
using Bannerlord.UIExtenderEx;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;
using Bannerlord.UIExtenderEx.ViewModels;
using CheatMenu.Intrigue;
using CheatMenu.Intrigue.Core;
using CheatMenu.Menu;
using CheatMenu.UI.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Library;

namespace CheatMenu.UI
{
    /// <summary>
    /// Ansiklopedideki lord sayfasına "Entrika" bölümü (UIExtenderEx): hakkında bildiklerin (rapor, sırlar, koz,
    /// süren entrika) ve iki düğme: dosyasını aç, entrika başlat. Düğmeler ansiklopediyi kapatıp Entrika ekranını
    /// o kişiye odaklı açar. Oyun yaması sayfayı değiştirirse bölüm görünmez; başka bir şey bozulmaz.
    /// </summary>
    public static class EncyclopediaIntrigue
    {
        private static UIExtender _extender;

        /// <summary>SubModule yüklenirken bir kez (prefab'lar yüklenmeden önce).</summary>
        public static void Register()
        {
            if (_extender != null) return;
            UIExtender extender = UIExtender.Create(ModInfo.Id);
            extender.Register(typeof(EncyclopediaIntrigue).Assembly);
            extender.Enable();
            _extender = extender;
        }
    }

    /// <summary>Bölüm, sayfadaki "Müttefikler" başlığının hemen üstüne girer (aynı başlık/katlanma düzeniyle).</summary>
    [PrefabExtension("EncyclopediaHeroPage", "descendant::EncyclopediaDivider[@Id='AlliesDivider']")]
    public sealed class EncyclopediaHeroIntriguePrefab : PrefabExtensionInsertPatch
    {
        public override InsertType Type => InsertType.Prepend;

        // Başlık ile içerik aynı ListPanel'de: başlığın "..\TvhIntrigueContainer" yolu kardeşini bulsun.
        [PrefabExtensionText]
        public string Content => @"
<ListPanel Id='TvhIntrigueSection' WidthSizePolicy='StretchToParent' HeightSizePolicy='CoverChildren' StackLayout.LayoutMethod='VerticalTopToBottom' IsVisible='@TvhHasIntrigue'>
  <Children>
    <EncyclopediaDivider Id='TvhIntrigueDivider' MarginTop='50' Parameter.Title='@TvhIntrigueTitle' Parameter.ItemList='..\TvhIntrigueContainer' />
    <ListPanel Id='TvhIntrigueContainer' WidthSizePolicy='StretchToParent' HeightSizePolicy='CoverChildren' StackLayout.LayoutMethod='VerticalTopToBottom' MarginTop='10' MarginLeft='15'>
      <Children>
        <RichTextWidget WidthSizePolicy='StretchToParent' HeightSizePolicy='CoverChildren' MarginLeft='15' MarginRight='25' Brush='Encyclopedia.SubPage.Info.Text' Text='@TvhIntrigueText' ClipContents='false' />
        <ListPanel WidthSizePolicy='CoverChildren' HeightSizePolicy='CoverChildren' MarginTop='14' MarginLeft='15' StackLayout.LayoutMethod='HorizontalLeftToRight'>
          <Children>
            <ButtonWidget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='250' SuggestedHeight='38' Brush='Tvh.Button' Command.Click='ExecuteTvhOpenDossier' IsEnabled='@TvhCanOpenDossier' UpdateChildrenStates='true' DoNotPassEventsToChildren='true'>
              <Children>
                <TextWidget WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' MarginLeft='8' MarginRight='8' Brush='Tvh.Text.Button' Text='@TvhOpenDossierText' />
              </Children>
            </ButtonWidget>
            <ButtonWidget WidthSizePolicy='Fixed' HeightSizePolicy='Fixed' SuggestedWidth='250' SuggestedHeight='38' MarginLeft='12' Brush='Tvh.Button' Command.Click='ExecuteTvhStartScheme' IsEnabled='@TvhCanStartScheme' UpdateChildrenStates='true' DoNotPassEventsToChildren='true'>
              <Children>
                <TextWidget WidthSizePolicy='StretchToParent' HeightSizePolicy='StretchToParent' MarginLeft='8' MarginRight='8' Brush='Tvh.Text.Button' Text='@TvhStartSchemeText' />
              </Children>
            </ButtonWidget>
          </Children>
        </ListPanel>
        <RichTextWidget WidthSizePolicy='StretchToParent' HeightSizePolicy='CoverChildren' MarginTop='6' MarginLeft='15' MarginRight='25' Brush='Tvh.Text.Bad' Text='@TvhBlockedText' IsVisible='@TvhHasBlockedText' ClipContents='false' />
      </Children>
    </ListPanel>
  </Children>
</ListPanel>";
    }

    /// <summary>Lord sayfasının ViewModel'ine eklenen alanlar; sayfa her yenilendiğinde yeniden hesaplanır.</summary>
    [ViewModelMixin(nameof(EncyclopediaHeroPageVM.Refresh))]
    public sealed class EncyclopediaHeroIntrigueMixin : BaseViewModelMixin<EncyclopediaHeroPageVM>
    {
        private static readonly SchemeKind[] StartOrder = { SchemeKind.Spy, SchemeKind.Blackmail, SchemeKind.Kidnap, SchemeKind.Murder };

        private Hero _hero;
        private SchemeKind? _startKind;
        private bool _hasIntrigue, _canOpenDossier, _canStartScheme, _hasBlockedText;
        private string _text = "", _blockedText = "";

        public EncyclopediaHeroIntrigueMixin(EncyclopediaHeroPageVM vm) : base(vm)
        {
            // Düğme yazılarının fırçalarında yazı tipi boş; Entrika ekranı hiç açılmadan da doğru görünsünler.
            TvhUi.PrepareBrushes();
            Update();
        }

        [DataSourceProperty] public bool TvhHasIntrigue { get => _hasIntrigue; set => SetField(ref _hasIntrigue, value, nameof(TvhHasIntrigue)); }
        [DataSourceProperty] public string TvhIntrigueTitle => "Entrika";
        [DataSourceProperty] public string TvhIntrigueText { get => _text; set => SetField(ref _text, value, nameof(TvhIntrigueText)); }
        [DataSourceProperty] public bool TvhCanOpenDossier { get => _canOpenDossier; set => SetField(ref _canOpenDossier, value, nameof(TvhCanOpenDossier)); }
        [DataSourceProperty] public bool TvhCanStartScheme { get => _canStartScheme; set => SetField(ref _canStartScheme, value, nameof(TvhCanStartScheme)); }
        [DataSourceProperty] public string TvhOpenDossierText => "Dosyasını aç";
        [DataSourceProperty] public string TvhStartSchemeText => "Entrika başlat...";
        [DataSourceProperty] public bool TvhHasBlockedText { get => _hasBlockedText; set => SetField(ref _hasBlockedText, value, nameof(TvhHasBlockedText)); }
        [DataSourceProperty] public string TvhBlockedText { get => _blockedText; set => SetField(ref _blockedText, value, nameof(TvhBlockedText)); }

        public override void OnRefresh() => Update();

        [DataSourceMethod]
        public void ExecuteTvhOpenDossier()
        {
            if (TvhCanOpenDossier) OpenScreen(null);
        }

        [DataSourceMethod]
        public void ExecuteTvhStartScheme()
        {
            if (TvhCanStartScheme && _startKind != null) OpenScreen(_startKind);
        }

        private void OpenScreen(SchemeKind? kind)
        {
            Hero hero = _hero;
            if (hero != null) MenuUI.Defer(() => IntrigueScreen.OpenOn(hero, kind));
        }

        /// <summary>Sayfada istisna çıkarsa bölüm gizlenir; ansiklopedi açık kalır.</summary>
        private void Update()
        {
            try
            {
                Fill();
            }
            catch (Exception ex)
            {
                TvhHasIntrigue = false;
                Debug.Print("[CheatMenu] encyclopedia intrigue section failed: " + ex);
            }
        }

        private void Fill()
        {
            Hero hero = ViewModel?.Obj as Hero;
            IntrigueState state = IntrigueBehavior.Current?.State;
            int today = SchemeFactors.Today;
            bool listed = state != null && PeopleVM.IsListed(state, hero, today);
            _hero = hero;
            _startKind = null;
            // Kendi sayfanda yok; ölüler yalnızca hakkında bir şey biliyorsan.
            TvhHasIntrigue = state != null && hero != null && hero != Hero.MainHero && (hero.IsAlive || listed);
            if (!TvhHasIntrigue)
            {
                TvhCanOpenDossier = TvhCanStartScheme = TvhHasBlockedText = false;
                return;
            }

            TvhIntrigueText = string.Join("\n", Lines(state, hero, today));

            string cannotOpen = IntrigueScreen.CannotOpenReason();
            string blocked = cannotOpen;
            if (blocked == null)
            {
                if (!hero.IsAlive) blocked = "Hayatta değil";
                else
                {
                    blocked = SchemeEngine.CannotStartReason(state);
                    if (blocked == null)
                    {
                        // İlk serbest tür seçili gelir; ekranda başka türe geçilebilir.
                        string first = null;
                        foreach (SchemeKind kind in StartOrder)
                        {
                            string reason = SchemeFactors.BlockedReason(state, hero, kind);
                            if (reason == null)
                            {
                                _startKind = kind;
                                break;
                            }
                            first = first ?? reason;
                        }
                        blocked = _startKind == null ? first : null;
                    }
                }
            }
            TvhCanOpenDossier = cannotOpen == null && listed;
            TvhCanStartScheme = _startKind != null;
            TvhBlockedText = cannotOpen != null ? "Entrika ekranı: " + cannotOpen
                : blocked != null ? "Entrika başlatılamaz: " + blocked : "";
            TvhHasBlockedText = TvhBlockedText.Length > 0;
        }

        /// <summary>Bildiklerin, kişi dosyasındakiyle aynı ölçülerle (rapor yaşı, sırlar, koz süresi, süren entrika).</summary>
        private static IEnumerable<string> Lines(IntrigueState state, Hero hero, int today)
        {
            var lines = new List<string>();
            IntelReport report = state.LatestReport(hero.StringId);
            if (report != null)
            {
                int age = today - report.Day;
                lines.Add("İstihbarat raporu: " + age + " gün önce" + (age > SchemeMath.ReportValidDays ? " (eski, bonus vermez)" : " (geçerli)"));
            }
            List<Secret> secrets = state.Secrets.Where(s => s.TargetId == hero.StringId).ToList();
            if (secrets.Count > 0)
                lines.Add("Bildiğin sırlar: " + string.Join(", ", secrets.Select(s => SecretRules.Name(s.Kind) + (s.Exposed ? " (ifşa edildi)" : ""))));
            Hook hook = state.ActiveHook(hero.StringId, today);
            if (hook != null && hero.IsAlive) lines.Add("Elindeki koz: " + (hook.Until - today) + " gün daha geçerli");
            Scheme scheme = state.FindScheme(hero.StringId);
            if (scheme != null)
                lines.Add("Süren entrika: " + SchemeRules.Name(scheme.Kind) + ", aşama " + scheme.Phase + "/" + SchemeRules.MaxPhases(scheme.Kind));
            if (lines.Count == 0)
                lines.Add("Hakkında henüz bir şey bilmiyorsun. Casusluk raporu, sırlar ve kozlar burada görünür.");
            Hero ruler = hero.Clan?.Kingdom?.Leader;
            if (hero.IsAlive && ruler != null && ruler != hero) lines.Add("Hükümdarı " + ruler.Name + " ile: " + Court.MoodText(state, hero));
            if (hero == Courtships.Spouse) lines.Add("Eşin: araştırma, yüzleşme ve boşanma dosyasından.");
            return lines;
        }
    }
}
