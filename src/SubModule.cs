using CheatMenu.Cheats;
using CheatMenu.Cheats.Models;
using System;
using System.Collections.Generic;
using CheatMenu.Intrigue;
using CheatMenu.Mcm;
using CheatMenu.Patches;
using CheatMenu.Menu;
using CheatMenu.UI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
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
        private static readonly List<string> _initErrors = new List<string>();

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            // Lambda: bağımlılığın türleri bu metot derlenirken değil, try içindeki çağrıda yüklensin.
            SafeInit("Harmony", () => HarmonyPatches.Apply());
            SafeInit("MCM", () => TacVeHancerSettings.Initialize());
            SafeInit("UIExtenderEx", () => EncyclopediaIntrigue.Register());
        }

        /// <summary>
        /// Bağımlılık (Harmony/MCM) eksik ya da uyumsuzsa mod yine yüklensin: hata günlüğe yazılır, ilk ekranda bildirilir.
        /// Ayrı metot: bağımlılığın türleri yalnızca bu çağrı çalışırken yüklenir, eksikse istisna burada yakalanır.
        /// </summary>
        private static void SafeInit(string what, Action init)
        {
            try
            {
                init();
            }
            catch (Exception ex)
            {
                _initErrors.Add(what);
                Debug.Print("[CheatMenu] " + what + " init failed: " + ex);
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            if (_loadMessageShown) return;
            _loadMessageShown = true;
            // Arayüz fabrikası bu noktada hazır. Kayıt olmazsa yalnızca olay kartları düz pencereye düşer; uyarı gerekmez.
            try
            {
                UiStartup.RegisterWidgets();
            }
            catch (Exception ex)
            {
                Debug.Print("[CheatMenu] widget registration via UIExtenderEx failed: " + ex);
            }
            string hotkey = ModSettings.ScreenHotkeyNames[SettingsStore.Current.ScreenHotkey];
            InformationManager.DisplayMessage(new InformationMessage("[" + ModInfo.DisplayName + "] yüklendi - kampanyada Insert: menü"
                + (SettingsStore.Current.ScreenHotkey > 0 ? ", " + hotkey + ": entrika ekranı" : "")));
            if (_initErrors.Count > 0)
                InformationManager.DisplayMessage(new InformationMessage("[" + ModInfo.DisplayName + "] başlatılamayan bağımlılık: "
                    + string.Join(", ", _initErrors) + ". Launcher'da Harmony, ButterLib, UIExtenderEx ve MCM'in kurulu ve bu modun üstünde olduğundan emin ol.",
                    Color.FromUint(0xFFFF6B6B)));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            if (game.GameType is Campaign && gameStarterObject is CampaignGameStarter starter)
            {
                RuleModels.Register(starter);
                starter.AddBehavior(new IntrigueBehavior());
                // Bilgi, oyunun kendi ikna konuşmalarına bonus verir (lordu krallığa davet vb.).
                starter.AddModel<PersuasionModel>(new IntriguePersuasionModel());
                // Aile editörü: "çocuk istemiyorlar" işaretli çiftte oyunun gebeliği olmaz.
                starter.AddModel<PregnancyModel>(new Cheats.Models.FamilyPregnancyModel());
                // Divan, hekim: klanında yaşlılıktan ölüm hekimin Medicine'ine göre azalır.
                starter.AddModel<HeroDeathProbabilityCalculationModel>(new Cheats.Models.PhysicianDeathModel());
                // Divan, kâhya: klanının yerleşimlerinde günlük sadakate kâhyanın satırı.
                starter.AddModel<SettlementLoyaltyModel>(new Cheats.Models.StewardLoyaltyModel());
            }
        }

        public override void OnGameLoaded(Game game, object initializerObject)
        {
            base.OnGameLoaded(game, initializerObject);
            if (game.GameType is Campaign)
            {
                CleanSaveGuard.OnGameLoaded();
                HarmonyPatches.ReportFailures();
            }
        }

        public override void OnNewGameCreated(Game game, object initializerObject)
        {
            base.OnNewGameCreated(game, initializerObject);
            if (game.GameType is Campaign) CleanSaveGuard.OnNewGame();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            if (Campaign.Current == null) return;
            mission.AddMissionBehavior(new CheatMissionLogic());
            Tailing.OnMissionStarting(mission);
        }

        public override void OnGameEnd(Game game)
        {
            base.OnGameEnd(game);
            IntrigueScreen.Close();
            EventCards.CloseForGameEnd();
            MapButton.Close();
            TimePause.Reset();
            CheatState.ResetAll();
        }

        protected override void OnApplicationTick(float dt)
        {
            base.OnApplicationTick(dt);
            if (Campaign.Current == null) return;

            TimeSpeed.Tick();
            MapInfo.Tick();
            MenuUI.RunDeferred();
            IntriguePopups.Tick();
            EventCards.Tick();
            IntrigueScreen.Tick();
            MapButton.Tick();
            CleanSaveGuard.Tick();

            bool popupOpen = InformationManager.IsAnyInquiryActive() || EventCards.IsOpen;
            if (Input.IsKeyPressed(MenuKey) && !popupOpen)
                MenuUI.SafeRun("Ana menü", Categories.ShowRoot);
            if (!popupOpen && UiStartup.ScreenHotkeyPressed())
                MenuUI.SafeRun("Entrika ekranı", IntrigueScreen.Toggle);

            CursorTeleport.Tick();
        }
    }
}
