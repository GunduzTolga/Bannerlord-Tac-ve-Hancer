using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// İleri sarma (">>") hızı. Oyun Campaign.SpeedUpMultiplier'ı kaydetmiyor ve her yüklemede 4'e sıfırlıyor;
    /// bu yüzden seçili değer her karede yeniden uygulanır. Seçili değer varsayılanken (4x) dokunulmaz,
    /// böylece Better Time gibi modlarla çakışılmaz.
    /// </summary>
    public static class TimeSpeed
    {
        private static bool _resetPending;

        public static int Current => SettingsStore.Current.FastForwardSpeed;

        public static void Set(int speed)
        {
            SettingsStore.Current.SetFastForwardSpeed(speed);
            _resetPending = true;
            Tick();
            if (SettingsStore.Save()) MenuUI.Success("İleri sarma hızı: " + Current + "x");
            else MenuUI.Error("İleri sarma hızı " + Current + "x oldu ama ayar dosyasına yazılamadı");
        }

        public static void Tick()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null) return;
            int wanted = Current;
            if (wanted == ModSettings.DefaultSpeed && !_resetPending) return;
            if (campaign.SpeedUpMultiplier != wanted) campaign.SpeedUpMultiplier = wanted;
            _resetPending = false;
        }
    }
}
