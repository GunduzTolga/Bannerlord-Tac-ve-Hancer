using System.Linq;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;

namespace CheatMenu
{
    /// <summary>
    /// Başarımların açık olduğu bir kayıt bu modla yüklenirse kullanıcıyı uyarır.
    /// Başarım kontrolünü ATLATMAZ; yalnızca yanlışlıkla temiz bir kaydı işaretlemeyi önler.
    /// </summary>
    public static class CleanSaveGuard
    {
        private static bool _pending;
        private static bool _isNewGame;

        public static void OnGameLoaded()
        {
            _pending = true;
            _isNewGame = false;
        }

        public static void OnNewGame()
        {
            _pending = true;
            _isNewGame = true;
        }

        /// <summary>Harita ekranı ilk kez aktif olduğunda bir kez çalışır.</summary>
        public static void Tick()
        {
            if (!_pending) return;
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState)) return;
            _pending = false;

            if (_isNewGame)
            {
                MenuUI.Info(ModInfo.DisplayName + " aktif: bu kampanyada başarımlar kapalı olacak.");
                return;
            }
            if (!IsSaveClean()) return;

            InformationManager.ShowInquiry(
                new InquiryData(
                    ModInfo.DisplayName + " - Başarım uyarısı",
                    "Bu kayıt temiz görünüyor (başarımlar açık).\n\n"
                    + ModInfo.DisplayName + " aktifken KAYDEDERSEN bu kayıtta başarımlar kalıcı olarak kapanır.\n\n"
                    + "Başarımları korumak istiyorsan kaydetmeden çık ve Launcher'da " + ModInfo.DisplayName + " modunu kapat.",
                    true, false, "Anladım", string.Empty, () => { }, null),
                true, false);
        }

        // Kimlik ayrıştırması oyunun DumpIntegrityCampaignBehavior.CheckIfModulesAreDefault mantığıyla aynı.
        private static bool IsSaveClean()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null || campaign.EnabledCheatsBefore) return false;

            MBList<string> official = ModuleHelper.GetOfficialModuleIds();
            foreach (string entry in campaign.PreviouslyUsedModules)
            {
                string id = entry.Split(MBSaveLoad.ModuleCodeSeperator)[0].Split(MBSaveLoad.ModuleVersionSeperator)[0];
                if (id != ModInfo.Id && !official.Contains(id)) return false;
            }
            return true;
        }
    }
}
