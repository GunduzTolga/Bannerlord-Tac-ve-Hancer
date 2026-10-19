using CheatMenu.Menu;
using TaleWorlds.Library;

namespace CheatMenu.UI.ViewModels
{
    public sealed class MapButtonVM : TvhViewModel
    {
        private bool _isTopLeft, _isTopCenter, _isTopRight, _hasBadge;
        private string _badgeText = "";

        [DataSourceProperty] public string Label => "ENTRİKA";
        [DataSourceProperty] public bool IsTopLeft { get => _isTopLeft; set => Set(ref _isTopLeft, value, nameof(IsTopLeft)); }
        [DataSourceProperty] public bool IsTopCenter { get => _isTopCenter; set => Set(ref _isTopCenter, value, nameof(IsTopCenter)); }
        [DataSourceProperty] public bool IsTopRight { get => _isTopRight; set => Set(ref _isTopRight, value, nameof(IsTopRight)); }
        [DataSourceProperty] public bool HasBadge { get => _hasBadge; set => Set(ref _hasBadge, value, nameof(HasBadge)); }
        [DataSourceProperty] public string BadgeText { get => _badgeText; set => Set(ref _badgeText, value, nameof(BadgeText)); }

        /// <param name="position">ModSettings.MapButtonPosition: 1 sol üst, 2 üst orta, 3 sağ üst.</param>
        public void Refresh(int position, int badge)
        {
            IsTopLeft = position == 1;
            IsTopCenter = position == 2;
            IsTopRight = position == 3;
            HasBadge = badge > 0;
            BadgeText = badge > 9 ? "9+" : badge.ToString();
        }

        public void ExecuteOpen() => MenuUI.Defer(() => IntrigueScreen.Open());
    }
}
