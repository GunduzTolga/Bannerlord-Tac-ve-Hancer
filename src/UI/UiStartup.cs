using System;
using Bannerlord.UIExtenderEx.ResourceManager;
using CheatMenu.UI.Widgets;
using TaleWorlds.InputSystem;
using TaleWorlds.ScreenSystem;

namespace CheatMenu.UI
{
    /// <summary>Arayüzün mod yüklenirken yapılan kayıtları ve kısayol tuşu eşlemesi.</summary>
    public static class UiStartup
    {
        private static readonly InputKey[] HotkeyKeys =
        {
            InputKey.Invalid, InputKey.Home, InputKey.End, InputKey.PageUp, InputKey.PageDown,
            InputKey.U, InputKey.Y, InputKey.O, InputKey.F9, InputKey.F10,
        };

        /// <summary>
        /// Özel resim parçamızı (TvhImageWidget) oyunun parça fabrikasına UIExtenderEx üzerinden de tanıtır. Oyun mod DLL'lerindeki
        /// parçaları kendisi de tarar; bu ikinci yol, tarama modumuz yüklenmeden yapılmışsa diye. Zaten kayıtlıysa bir şey yapmaz.
        /// UIExtenderEx yoksa çağıran yakalar (tür yüklenemez).
        /// </summary>
        public static void RegisterWidgets()
        {
            if (!TvhUi.ImageWidgetAvailable()) Register();
        }

        // Ayrı metot: UIExtenderEx türleri yalnızca gerçekten gerekince yüklensin.
        private static void Register() => WidgetFactoryManager.Register(typeof(TvhImageWidget));

        /// <summary>Ayarlardaki kısayol (ModSettings.ScreenHotkeyNames ile aynı sıra); 0 ya da geçersizse Invalid.</summary>
        public static InputKey ScreenHotkey(int index) => index > 0 && index < HotkeyKeys.Length ? HotkeyKeys[index] : InputKey.Invalid;

        /// <summary>
        /// Kısayol bu karede basıldı mı. Metin kutusuna yazarken sayılmaz; ekran kapalıyken yalnızca açılabileceği yerde
        /// (haritada) sayılır, böylece savaşta ya da başka ekranda basınca hata mesajı çıkmaz.
        /// </summary>
        public static bool ScreenHotkeyPressed()
        {
            InputKey key = ScreenHotkey(SettingsStore.Current.ScreenHotkey);
            if (key == InputKey.Invalid || !Input.IsKeyPressed(key)) return false;
            if (ScreenManager.FocusedLayer?.IsFocusedOnInput() == true) return false;
            return IntrigueScreen.IsOpen || IntrigueScreen.CannotOpenReason() == null;
        }
    }
}
