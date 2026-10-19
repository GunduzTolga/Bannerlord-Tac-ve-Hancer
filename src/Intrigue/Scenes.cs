using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SceneInformationPopupTypes;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace CheatMenu.Intrigue
{
    /// <summary>
    /// Oyunun kendi 3B sahne bildirimleri (SceneInformationPopupTypes). Oyun çoğunu kendi olaylarında zaten gösterir:
    /// kendi düğünün, klanındaki evlilik ve doğumlar (haritadaki bildirimden), bir klanın krallığına katılması (kozla
    /// geçirdiğin lord dahil: iltica ile katılım), kral olman, idamlar. Aile editörü ve gebelik oyunun kendi işlemlerini
    /// kullandığı için onlarınki de kendiliğinden çıkar. Burada yalnızca oyunun göstermediği bizim olaylarımız var.
    /// </summary>
    public static class Scenes
    {
        /// <summary>Bir sahne açıkken bildirim kuyruğu bekler (kart sahnenin üstüne binmesin).</summary>
        public static bool IsShowing
        {
            get
            {
                try
                {
                    return MBInformationManager.GetIsAnySceneNotificationActive() == true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Başka klandaki sevgilinden doğan çocuğun: oyun doğum sahnesini yalnızca anne senin klanındaysa gösterir.
        /// </summary>
        public static void NewBorn(Hero father, Hero mother)
        {
            try
            {
                MBInformationManager.ShowSceneNotification(new NewBornSceneNotificationItem(father, mother, CampaignTime.Now));
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] newborn scene failed: " + ex);
            }
        }
    }
}
