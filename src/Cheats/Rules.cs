using System;
using CheatMenu.Menu;

namespace CheatMenu.Cheats
{
    /// <summary>Kural ayarlarını değiştirir ve kaydeder. Modeller ayarları canlı okuduğu için değişiklik hemen etkilidir.</summary>
    public static class Rules
    {
        public static ModSettings Settings => SettingsStore.Current;

        /// <param name="successMessage">null ise başarı mesajı gösterilmez (aç/kapa satırı zaten gösterir).</param>
        public static void Change(Action<ModSettings> change, string successMessage)
        {
            change(SettingsStore.Current);
            if (!SettingsStore.Save()) MenuUI.Error("Ayar değişti ama ayar dosyasına yazılamadı");
            else if (successMessage != null) MenuUI.Success(successMessage);
        }
    }
}
