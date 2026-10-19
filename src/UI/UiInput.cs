using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.InputSystem;

namespace CheatMenu.UI
{
    /// <summary>
    /// Ekran ve kartların ortak oyun duraklatması: en az biri açıkken kampanya zamanı durur ve zaman düğmeleri kilitlenir.
    /// Sahip sayılır; son sahip bırakınca kilit (yalnızca biz koyduysak) kalkar.
    /// </summary>
    public static class TimePause
    {
        private static readonly HashSet<object> Owners = new HashSet<object>();
        private static bool _locked;

        public static void Acquire(object owner)
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null) return;
            campaign.TimeControlMode = CampaignTimeControlMode.Stop;
            if (Owners.Add(owner) && Owners.Count == 1 && !campaign.TimeControlModeLock)
            {
                campaign.SetTimeControlModeLock(true);
                _locked = true;
            }
        }

        public static void Release(object owner)
        {
            if (!Owners.Remove(owner) || Owners.Count > 0) return;
            Unlock();
        }

        /// <summary>Kampanya kapanırken: bütün sahipler düşer.</summary>
        public static void Reset()
        {
            Owners.Clear();
            Unlock();
        }

        private static void Unlock()
        {
            if (_locked) Campaign.Current?.SetTimeControlModeLock(false);
            _locked = false;
        }
    }

    /// <summary>
    /// Esc ile kapatma: tuş katman açıkken basılıp bırakıldığında bir kez true döner. Basışta değil bırakışta kapatılır
    /// (kapanan katmanın bıraktığı tuşu haritanın kaçış menüsü yakalamasın), açıldığı karedeki basış sayılmaz.
    /// </summary>
    public sealed class EscapeKey
    {
        private bool _armed;
        private bool _skipFrame;

        /// <summary>Katman açılınca çağrılır.</summary>
        public void Reset()
        {
            _armed = false;
            _skipFrame = true;
        }

        /// <param name="active">Katman açık ve üstünde başka pencere yok.</param>
        public bool Released(bool active)
        {
            if (!active)
            {
                _armed = false;
                return false;
            }
            if (_skipFrame)
            {
                _skipFrame = false;
                return false;
            }
            if (Input.IsKeyPressed(InputKey.Escape)) _armed = true;
            if (!_armed || !Input.IsKeyReleased(InputKey.Escape)) return false;
            _armed = false;
            return true;
        }
    }
}
