using System;
using CheatMenu.Menu;
using SandBox.View.Map;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.ScreenSystem;

namespace CheatMenu.Cheats
{
    /// <summary>Ctrl+Alt + Sol tık: kampanya haritasında imlecin altına ışınlanma.</summary>
    public static class CursorTeleport
    {
        public static void Tick()
        {
            if (!Input.IsKeyPressed(InputKey.LeftMouseButton)) return;
            if (!IsEitherDown(InputKey.LeftControl, InputKey.RightControl)) return;
            if (!IsEitherDown(InputKey.LeftAlt, InputKey.RightAlt)) return;
            if (!(ScreenManager.TopScreen is MapScreen mapScreen)) return;
            if (!(Game.Current?.GameStateManager?.ActiveState is MapState mapState) || mapState.AtMenu) return;
            if (InformationManager.IsAnyInquiryActive()) return;

            MenuUI.SafeRun("Fareyle ışınlanma", () =>
            {
                if (!TryGetCursorPosition(mapScreen, out CampaignVec2 target))
                    throw new InvalidOperationException("İmlecin altı ışınlanılabilir bir yer değil");
                Teleport.ToPosition(target);
                MenuUI.Success("Işınlandın");
            });
        }

        private static bool IsEitherDown(InputKey a, InputKey b) => Input.IsKeyDown(a) || Input.IsKeyDown(b);

        /// <summary>Oyunun MapScreen.HandleMouse içindeki imleç → harita noktası hesabı.</summary>
        private static bool TryGetCursorPosition(MapScreen mapScreen, out CampaignVec2 position)
        {
            Vec3 near = Vec3.Zero;
            Vec3 far = Vec3.Zero;
            mapScreen.SceneLayer.SceneView.TranslateMouse(ref near, ref far, -1f);

            PathFaceRecord face = PathFaceRecord.NullFaceRecord;
            mapScreen.GetCursorIntersectionPoint(ref near, ref far, out float _, out Vec3 point, ref face, out bool isOnLand);

            position = new CampaignVec2(point.AsVec2, isOnLand);
            return face.IsValid();
        }
    }
}
