using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.BaseTypes;

namespace CheatMenu.UI.Widgets
{
    /// <summary>
    /// Modülün Images klasöründen ad ile resim gösteren parça: &lt;TvhImageWidget ImageName="@Image" /&gt;.
    /// Resim yoksa hiçbir şey çizmez (yerleşim bozulmaz).
    /// </summary>
    public class TvhImageWidget : Widget
    {
        private string _imageName;

        public TvhImageWidget(UIContext context) : base(context)
        {
        }

        [Editor(false)]
        public string ImageName
        {
            get => _imageName;
            set
            {
                if (_imageName == value) return;
                _imageName = value;
                Sprite = TvhImages.Get(value);
            }
        }
    }
}
