using CheatMenu.Menu;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Bilgi araçları: bir kahramanın gerçek konumu ve harita sisini kaldırma.
    /// Campaign.TrueSight kaydedilmeyen bir çalışma anı bayrağıdır (oyunun Ctrl+Alt+T hilesi de bunu kullanır);
    /// yüklemede sıfırlandığı için seçili durum her karede yeniden uygulanır.
    /// </summary>
    public static class MapInfo
    {
        private static bool _resetPending;

        public static void SetReveal(bool on)
        {
            CheatState.RevealMap = on;
            _resetPending = true;
            Tick();
        }

        public static void Tick()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null) return;
            if (!CheatState.RevealMap && !_resetPending) return;
            if (campaign.TrueSight != CheatState.RevealMap) campaign.TrueSight = CheatState.RevealMap;
            _resetPending = false;
        }

        /// <summary>Kahramanın gerçek konumunu yazar ve kamerayı oraya götürür.</summary>
        public static void FindHero(Hero hero)
        {
            if (!TryLocate(hero, out CampaignVec2 position, out string where))
            {
                MenuUI.Error(hero.Name + ": " + where);
                return;
            }
            MapScreen.Instance?.FastMoveCameraToPosition(position);
            MenuUI.Info(hero.Name + ": " + where);
        }

        /// <summary>Kahramanın gerçek konumunu anlatan metin (ör. "Pravend içinde", "seferde, Ocs Hall yakınında").</summary>
        public static string Where(Hero hero)
        {
            TryLocate(hero, out _, out string where);
            return where;
        }

        private static bool TryLocate(Hero hero, out CampaignVec2 position, out string where)
        {
            position = CampaignVec2.Invalid;

            PartyBase captor = hero.IsPrisoner ? hero.PartyBelongedToAsPrisoner : null;
            if (captor != null)
            {
                position = captor.Position;
                where = captor.IsSettlement ? captor.Name + " zindanında esir" : captor.Name + " elinde esir";
                return true;
            }

            MobileParty party = hero.PartyBelongedTo;
            Settlement settlement = party?.CurrentSettlement ?? hero.CurrentSettlement;
            if (settlement != null)
            {
                position = settlement.GatePosition;
                where = settlement.Name + " içinde";
                return true;
            }
            if (party != null)
            {
                position = party.Position;
                where = (party.IsCurrentlyAtSea ? "denizde, " : "seferde, ") + NearestSettlement(position) + " yakınında";
                return true;
            }

            where = "konumu bilinmiyor (haritada değil)";
            return false;
        }

        private static string NearestSettlement(CampaignVec2 position)
        {
            Vec2 point = position.ToVec2();
            Settlement nearest = null;
            float best = float.MaxValue;
            foreach (Settlement s in Settlement.All)
            {
                if (!s.IsTown && !s.IsCastle && !s.IsVillage) continue;
                float distance = point.DistanceSquared(s.GatePosition.ToVec2());
                if (distance < best)
                {
                    best = distance;
                    nearest = s;
                }
            }
            return nearest != null ? nearest.Name.ToString() : "?";
        }
    }
}
