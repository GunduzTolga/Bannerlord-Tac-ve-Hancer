using CheatMenu.Cheats;
using CheatMenu.Cheats.Models;
using CheatMenu.Menu;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CheatMenu
{
    public sealed class SubModule : MBSubModuleBase
    {
        // Ctrl+F1 değil: savaşta F1 "hareket emri" menüsünü açar.
        private const InputKey MenuKey = InputKey.Insert;

        private static bool _loadMessageShown;

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_loadMessageShown) return;
            _loadMessageShown = true;
            InformationManager.DisplayMessage(new InformationMessage("[CheatMenu] yüklendi - kampanyada Insert ile aç"));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter starter)
                RuleModels.Register(starter);
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
            if (game.GameType is Campaign) CleanSaveGuard.OnGameLoaded();
        }

        public override void OnNewGameCreated(Game game, object initializerObject)
        {
            base.OnNewGameCreated(game, initializerObject);
            if (game.GameType is Campaign) CleanSaveGuard.OnNewGame();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            if (Campaign.Current != null) mission.AddMissionBehavior(new CheatMissionLogic());
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            CheatState.ResetAll();
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (Campaign.Current == null) return;

            TimeSpeed.Tick();
            MapInfo.Tick();
            MenuUI.RunDeferred();
            CleanSaveGuard.Tick();

            if (Input.IsKeyPressed(MenuKey) && !InformationManager.IsAnyInquiryActive())
                MenuUI.SafeRun("Hile menüsü", Categories.ShowRoot);

            CursorTeleport.Tick();
        }
    }
}
