using System;
using System.Collections.Generic;
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
        /// Oyunun kendi fırçaları: "bizim=oyunun" çiftleri. Açılışta oyunun fırçası bizimkinin üstüne kopyalanır (katmanlar,
        /// durumlar, sesler, renkler), böylece sekme, düğme ve panel oyunun Klan ekranındakinin aynısı olur. Oyunda o ad yoksa
        /// (başka sürüm) bizim fırça dosyamızdaki karşılığı kalır; hiçbir şey bozulmaz. Yazı fırçalarında ("Tvh.Text.") boyut
        /// ve hizalama bizde kalır, yalnızca görünüş gelir. tools/GuiCheck --game bu adları oyunun dosyalarında doğrular.
        /// </summary>
        public const string GameLooks =
            "Tvh.Tab=Header.Tab.Center;Tvh.Text.Tab=Clan.TabControl.Text;" +
            "Tvh.Button.Primary=Popup.Done.Button;Tvh.Text.Button=Popup.Button.Text;" +
            "Tvh.Panel=Frame1Brush";

        /// <summary>Yedek ayrı ekranın kendisi yüklediği oyun sprite kategorisi (Klan ekranı açıkken oyun zaten yükler).</summary>
        public const string ClanSpriteCategory = "ui_clan";

        private static readonly HashSet<Brush> Adopted = new HashSet<Brush>();

        /// <summary>
        /// Fırçalarımızda yazı tipi adı yazmıyoruz (oyun sürümleri arasında ad değişirse fırça bozulmasın diye).
        /// Yazı tipi boş kalan fırçalara oyunun varsayılan yazı tipi atanır; Türkçe karakter eşlemesini oyun kendisi yapar.
        /// Önce oyunun fırçaları alınır (GameLooks).
        /// </summary>
        public static void PrepareBrushes()
        {
            AdoptGameLooks();
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


        private static void AdoptGameLooks()
        {
            BrushFactory factory = UIResourceManager.BrushFactory;
            if (factory == null) return;
            foreach (string pair in GameLooks.Split(';'))
            {
                string[] names = pair.Split('=');
                try
                {
                    Brush ours = Find(factory, names[0]), game = Find(factory, names[1]);
                    if (ours == null || game == null || Adopted.Contains(ours)) continue;
                    bool text = names[0].StartsWith(BrushPrefix + "Text.", StringComparison.Ordinal);
                    int size = ours.FontSize;
                    TextHorizontalAlignment horizontal = ours.TextHorizontalAlignment;
                    TextVerticalAlignment vertical = ours.TextVerticalAlignment;
                    ours.FillFrom(game);
                    ours.Name = names[0];
                    if (text)
                    {
                        ours.FontSize = size;
                        ours.TextHorizontalAlignment = horizontal;
                        ours.TextVerticalAlignment = vertical;
                        foreach (Style style in ours.Styles) style.FontSize = size;
                    }
                    Adopted.Add(ours);
                }
                catch (Exception ex)
                {
                    Debug.Print("[CheatMenu] game brush " + pair + " not adopted: " + ex);
                }
            }
        }

        /// <summary>Adıyla kayıtlı fırça; yoksa null (GetBrush yoksa oyunun varsayılan fırçasını döndürür).</summary>
        private static Brush Find(BrushFactory factory, string name)
        {
            Brush brush = factory.GetBrush(name);
            return brush != null && brush != factory.DefaultBrush && brush.Name == name ? brush : null;
        }

        /// <summary>Kategori yüklü değilse yükler; yüklediyse onu döndürür (kapanınca Release ile boşaltılır), değilse null.</summary>
        public static SpriteCategory AcquireSprites(string categoryName)
        {
            try
            {
                SpriteCategory category = UIResourceManager.GetSpriteCategory(categoryName);
                if (category == null || category.IsLoaded) return null;
                category.Load(UIResourceManager.ResourceContext, UIResourceManager.ResourceDepot);
                return category;
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] sprite category " + categoryName + " not loaded: " + ex);
                return null;
            }
        }

        public static void ReleaseSprites(SpriteCategory category)
        {
            try
            {
                if (category != null && category.IsLoaded) category.Unload();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] sprite category unload failed: " + ex);
            }
        }
    }
}
