using System;
using System.Linq;
using CheatMenu.Menu;
using HarmonyLib;
using SandBox.View.Map.Navigation;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.TwoDimension;

namespace CheatMenu.UI
{
    /// <summary>
    /// Entrika düğmesini oyunun kendi harita çubuğuna (sol alttaki Karakter / Envanter / Parti / Görevler / Klan / Krallık sırası)
    /// koyar. v1.4.8'de çubuk veriye dayalıdır: MapNavigationVM, MapNavigationHandler'ın her öğesi için bir düğme çizer
    /// (IconID = öğenin StringId'si, tıklama = OpenView, rozet = HasAlert, gri = Permission). Bu yüzden prefab'a dokunmadan
    /// handler'ın öğe listesine bir öğe eklemek yeter; NavalDLC'nin filo düğmesi de aynı yolla eklenir.
    /// İkon için oyunun fırçalarına (MapBar.Left.Icons, MapBar.Left.Button.Backgrounds) çalışma anında "intrigue" katmanı
    /// eklenir; fırçayı XML'de yeniden tanımlamayız ki oyunun ileride ekleyeceği düğmelerin ikonları ezilmesin.
    /// Herhangi bir adım tutmazsa öğe eklenmez ve MapButton eski üst-orta düğmesini yedek olarak gösterir.
    /// </summary>
    public static class MapBarButton
    {
        public const string ElementId = "intrigue";

        // Oyunun Roguery (hilekârlık) yetenek ikonu; her zaman yüklü ui_group1 kategorisinde.
        private const string IconSprite = @"SPGeneral\Skills\gui_skills_icon_roguery";
        private const string IconBrush = "MapBar.Left.Icons";
        private const string BackgroundBrush = "MapBar.Left.Button.Backgrounds";
        // Boyut ve hizalama ayarları bu oyun katmanlarından kopyalanır.
        private const string IconTemplateLayer = "quest";
        private const string BackgroundTemplateLayer = "clan";

        /// <summary>Şu anki harita çubuğuna öğe eklendi mi (eklenmediyse eski düğme yedek olarak görünür).</summary>
        public static bool Injected { get; private set; }

        public static bool Wanted => SettingsStore.Current.MapButtonPosition == ModSettings.MapBarButtonPosition;

        /// <summary>Kampanya kapanırken: bir sonraki harita çubuğu yeniden karar versin.</summary>
        public static void Reset() => Injected = false;

        /// <summary>Harita çubuğu kurulurken (MapNavigationHandler.OnCreateElements) çağrılır; hata fırlatmaz.</summary>
        internal static INavigationElement[] AddTo(MapNavigationHandler handler, INavigationElement[] elements)
        {
            try
            {
                if (!Wanted || elements == null || elements.Any(e => e?.StringId == ElementId)) return elements;
                if (!EnsureBrushLayers()) return elements;
                // Krallık düğmesinin hemen önüne (o yoksa sona): oyunun kendi sırası bozulmasın.
                var list = elements.ToList();
                int kingdom = list.FindIndex(e => e?.StringId == "kingdom");
                list.Insert(kingdom >= 0 ? kingdom : list.Count, new IntrigueNavigationElement(handler));
                Injected = true;
                return list.ToArray();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] map bar button could not be added: " + ex);
                return elements;
            }
        }

        /// <summary>İkon ve arka plan katmanlarını oyunun fırçalarına bir kez ekler.</summary>
        private static bool EnsureBrushLayers()
        {
            Sprite icon = UIResourceManager.SpriteData?.GetSprite(IconSprite);
            return icon != null
                && AddLayer(IconBrush, IconTemplateLayer, icon)
                && AddLayer(BackgroundBrush, BackgroundTemplateLayer, null);
        }

        private static bool AddLayer(string brushName, string templateName, Sprite sprite)
        {
            Brush brush = UIResourceManager.BrushFactory?.GetBrush(brushName);
            if (brush == null) return false;
            if (brush.GetLayer(ElementId) != null) return true;
            BrushLayer template = brush.GetLayer(templateName);
            if (template == null) return false;
            var layer = new BrushLayer();
            layer.FillFrom(template);
            layer.Name = ElementId;
            if (sprite != null) layer.Sprite = sprite;
            brush.AddLayer(layer);
            return true;
        }
    }

    /// <summary>Harita çubuğundaki Entrika düğmesi. Oyun her karede durumunu sorar; hiçbir üye hata fırlatmaz.</summary>
    public sealed class IntrigueNavigationElement : MapNavigationElementBase
    {
        public IntrigueNavigationElement(MapNavigationHandler handler) : base(handler) { }

        public override string StringId => MapBarButton.ElementId;

        public override bool IsActive => Safe(() => IntrigueScreen.IsOpen);

        // Ekran açıkken çubuktaki diğer ekranlar onun üstüne açılmasın.
        public override bool IsLockingNavigation => true;

        public override bool HasAlert => Safe(() => MapButton.PendingCount() > 0);

        public override void OpenView() => MenuUI.Defer(() => IntrigueScreen.Open());

        public override void OpenView(object[] parameters) => OpenView();

        public override void GoToLink() => OpenView();

        protected override NavigationPermissionItem GetPermission()
        {
            try
            {
                string reason = IntrigueScreen.CannotOpenReason();
                return new NavigationPermissionItem(reason == null, reason == null ? null : new TextObject(reason, null));
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] map bar permission failed: " + ex);
                return new NavigationPermissionItem(false, null);
            }
        }

        protected override TextObject GetTooltip() => new TextObject("Entrika", null);

        protected override TextObject GetAlertTooltip() =>
            new TextObject("Harekete geçmeye hazır entrika ya da sezilen tehlike: " + Safe(MapButton.PendingCount), null);

        private static T Safe<T>(Func<T> read)
        {
            try
            {
                return read();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] map bar state failed: " + ex);
                return default;
            }
        }
    }

    /// <summary>
    /// Harita çubuğu kurulurken öğe listesine Entrika düğmesini ekler. Taban metoda yamalanır: NavalDLC'nin handler'ı da
    /// base.OnCreateElements()'ı çağırıp filo düğmesini ekler, böylece iki durumda da çalışır.
    /// </summary>
    [HarmonyPatch(typeof(MapNavigationHandler), "OnCreateElements")]
    internal static class MapNavigationHandlerPatch
    {
        private static void Postfix(MapNavigationHandler __instance, ref INavigationElement[] __result) =>
            __result = MapBarButton.AddTo(__instance, __result);
    }
}
