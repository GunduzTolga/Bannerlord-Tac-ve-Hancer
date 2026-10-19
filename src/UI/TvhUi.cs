using System;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension;

namespace CheatMenu.UI
{
    /// <summary>Arayüz katmanının ortak hazırlıkları.</summary>
    public static class TvhUi
    {
        /// <summary>Fırça dosyamızdaki (GUI/Brushes/TacVeHancer.xml) adların ortak öneki.</summary>
        public const string BrushPrefix = "Tvh.";

        /// <summary>
        /// Özel resim parçamız (TvhImageWidget) oyunun parça fabrikasında kayıtlı mı. Değilse olay kartı açılmaz,
        /// olaylar oyunun seçim penceresiyle sorulur (Entrika ekranı bu parçayı kullanmaz, etkilenmez).
        /// </summary>
        public static bool ImageWidgetAvailable()
        {
            try
            {
                return UIResourceManager.WidgetFactory?.IsBuiltinType(nameof(Widgets.TvhImageWidget)) == true;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] widget lookup failed: " + ex);
                return false;
            }
        }

        /// <summary>
        /// Fırçalarımızda yazı tipi adı yazmıyoruz (oyun sürümleri arasında ad değişirse fırça bozulmasın diye).
        /// Yazı tipi boş kalan fırçalara oyunun varsayılan yazı tipi atanır; Türkçe karakter eşlemesini oyun kendisi yapar.
        /// </summary>
        public static void PrepareBrushes()
        {
            try
            {
                Font font = UIResourceManager.FontFactory?.DefaultFont;
                if (font == null || UIResourceManager.BrushFactory == null) return;
                foreach (Brush brush in UIResourceManager.BrushFactory.Brushes)
                {
                    if (brush?.Name == null || !brush.Name.StartsWith(BrushPrefix, StringComparison.Ordinal)) continue;
                    if (brush.Font == null) brush.Font = font;
                    foreach (Style style in brush.Styles)
                        if (style.Font == null) style.Font = font;
                }
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] brush preparation failed: " + ex);
            }
        }
    }
}
