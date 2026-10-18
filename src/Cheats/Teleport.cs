using System;
using CheatMenu.Menu;
using Helpers;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.MountAndBlade;

namespace CheatMenu.Cheats
{
    /// <summary>
    /// Ana partiyi ışınlar. Adımlar oyunun hile modundaki Ctrl+tık ışınlanmasını
    /// (SandBox.View.Map.MapScreen.HandleLeftMouseButtonClick) izler; yalnızca oyunun kendi kaydettiği alanlar değişir.
    /// </summary>
    public static class Teleport
    {
        /// <summary>Işınlanmaya engel bir durum varsa nedenini, yoksa null döner.</summary>
        public static string GetBlocker()
        {
            MobileParty me = MobileParty.MainParty;
            if (me == null || Hero.MainHero == null) return "Ana parti bulunamadı";
            if (Mission.Current != null) return "Sahne veya savaş içindeyken ışınlanamazsın";
            if (me.MapEvent != null) return "Savaş sırasında ışınlanamazsın";
            if (me.SiegeEvent != null || me.BesiegedSettlement != null) return "Kuşatma sırasında ışınlanamazsın";
            if (me.IsInRaftState) return "Sal durumundayken ışınlanamazsın";
            if (me.Army != null && me.Army.LeaderParty != me) return "Başkasının ordusundayken ışınlanamazsın";
            if (Hero.MainHero.IsPrisoner) return "Esirken ışınlanamazsın";
            return null;
        }

        public static void ToSettlement(Settlement settlement)
        {
            MoveMainParty(settlement.GatePosition);
            MenuUI.Success(settlement.Name + " kapısına ışınlandın");
        }

        public static void ToHero(Hero hero)
        {
            if (hero.IsPrisoner) throw new InvalidOperationException(hero.Name + " esir, yanına ışınlanamazsın");

            MobileParty party = hero.PartyBelongedTo;
            if (party != null && party != MobileParty.MainParty && party.IsActive && party.CurrentSettlement == null)
            {
                if (party.IsCurrentlyAtSea && !MobileParty.MainParty.HasNavalNavigationCapability)
                    throw new InvalidOperationException(hero.Name + " denizde ve senin gemin yok");

                // Tam üstüne değil, 1-2 birim yanına: hemen karşılaşma başlamasın.
                MobileParty.NavigationType navigation = party.IsCurrentlyAtSea
                    ? MobileParty.NavigationType.Naval
                    : MobileParty.NavigationType.Default;
                CampaignVec2 near = NavigationHelper.FindPointAroundPosition(party.Position, navigation, 2f, 1f, false, false);
                MoveMainParty(near.IsValid() ? near : party.Position);
                MenuUI.Success(hero.Name + " yanına ışınlandın");
                return;
            }

            Settlement settlement = party?.CurrentSettlement ?? hero.CurrentSettlement;
            if (settlement == null) throw new InvalidOperationException(hero.Name + " şu an haritada değil");
            ToSettlement(settlement);
        }

        public static void ToPosition(CampaignVec2 target)
        {
            if (!target.IsOnLand && !MobileParty.MainParty.HasNavalNavigationCapability)
                throw new InvalidOperationException("Gemin olmadan denize ışınlanamazsın");
            MoveMainParty(target);
        }

        private static void MoveMainParty(CampaignVec2 target)
        {
            string blocker = GetBlocker();
            if (blocker != null) throw new InvalidOperationException(blocker);
            if (!target.IsValid()) throw new InvalidOperationException("Geçersiz hedef konum");

            MobileParty me = MobileParty.MainParty;

            // Oyunun "yerleşimden ayrıl" seçeneğinin yaptığı gibi önce karşılaşmayı kapat.
            if (PlayerEncounter.Current != null)
            {
                if (me.CurrentSettlement != null) PlayerEncounter.LeaveSettlement();
                PlayerEncounter.Finish(true);
            }
            if (me.CurrentSettlement != null) LeaveSettlementAction.ApplyForParty(me);

            me.Position = target;
            me.SetMoveModeHold();

            // Hedef partinin şu anki kara/deniz durumuna uymuyorsa durumu çevir (oyunun kendi kontrolü).
            MobileParty.NavigationType current = me.IsCurrentlyAtSea
                ? MobileParty.NavigationType.Naval
                : MobileParty.NavigationType.Default;
            if (!NavigationHelper.IsPositionValidForNavigationType(target, current)) me.ChangeIsCurrentlyAtSeaCheat();

            if (me.Army != null && me.Army.LeaderParty == me)
                foreach (MobileParty attached in me.AttachedParties)
                    attached.Position = target;

            foreach (MobileParty party in MobileParty.All) party.Party.UpdateVisibilityAndInspected(me.Position);
            foreach (Settlement settlement in Settlement.All) settlement.Party.UpdateVisibilityAndInspected(me.Position);

            MapScreen.Instance?.TeleportCameraToMainParty();
        }
    }
}
