# Technical feasibility of QoL/cheat features as a C# mod for Mount & Blade II: Bannerlord v1.4.8 (installed build, checked October 2026)

How I checked things. The installed build is confirmed as `Singleplayer Value="v1.4.8"` ([local: Version.xml][ver]). "Local reflection" means I loaded the installed DLLs with .NET reflection and listed their members, accessibility, optional parameters and save attributes (`SaveableField`/`SaveableProperty`). "IL call listing" means I read the bytecode of a method and listed the calls, field reads/writes and strings it uses. That is a lightweight disassembly, not a full decompile, so branch logic is my inference. The exception is methods marked "full IL", where I also read the branches and constants. I tested nothing at runtime. Every API marked verified exists with that exact signature in the installed v1.4.8 files. I compare against older versions using apidoc.bannerlord.com v1.2.12 pages. That site also hosts v1.4.7/v1.4.8 docs ([apidoc index][apidoc]), but I did not use those pages.

## 1. Teleporting the main party (to a settlement, and to the clicked or cursor map position, with sea/land handling)

### Takeaway
Both features work without Harmony. `MobileParty.Position` is a public `CampaignVec2` setter. The game itself contains a cheat-mode "Ctrl+click teleport" in `MapScreen.HandleLeftMouseButtonClick`, and our mod can copy its exact steps. That covers reading the cursor with `MapScreen.GetCursorIntersectionPoint`, checking the navmesh face, and toggling sea/land with `ChangeIsCurrentlyAtSeaCheat()`. Teleporting to a settlement is easy (use `GatePosition` or `PortPosition`). Teleporting to the cursor is medium, because of naval state, armies and encounters. Position and sea state are saved by the game's own fields. No custom data is needed, so removing the mod is safe.

### Cited Findings
- **Position API (v1.4.8):** `MobileParty.Position` is `CampaignVec2 { get; public set; }`. Its setter only writes `_position` and calls `Campaign.Current.MobilePartyLocator.UpdateLocator`. It does not touch sea/land state, AI or visuals (IL call listing) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Old API (v1.2.12):** `MobileParty` had `Vec2 Position2D { get; set; }`, and movement orders lived on `MobilePartyAi`, e.g. `SetMoveGoToPoint(Vec2 point)` and `SetMoveModeHold()` — [apidoc 1.2.12 MobileParty][api1212-mp]; [apidoc 1.2.12 MobilePartyAi][api1212-ai]
- **What changed by v1.4.8:**
  - `Position2D` is gone. Only `Vec2 GetPosition2D { get; }` and `VisualPosition2DWithoutError` remain.
  - The move orders now sit on `MobileParty` itself and take a `NavigationType`: `SetMoveModeHold()`, `SetMoveGoToPoint(CampaignVec2 point, NavigationType navigationType)`, `SetMoveGoToSettlement(Settlement, NavigationType, bool isTargetingThePort)`, `SetMoveToNearestLand(Settlement)`.
  - `MobilePartyAi` keeps only `DisableAi()`, `EnableAi()`, `DisableForHours(int)`, `SetDoNotMakeNewDecisions(bool)`, `SetInitiative(...)`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **CampaignVec2 (v1.4.8):**
  - Struct with constructor `CampaignVec2(Vec2 pos, bool isOnLand)` and a public field `bool IsOnLand`.
  - Properties: `PathFaceRecord Face`, `X`, `Y`, static `Invalid` and `Zero`.
  - Methods: `ToVec2()`, `AsVec3()`, `IsValid()`, `Distance(...)`, `NearlyEquals(...)`.
  - `PathFaceRecord` lives in `TaleWorlds.Library`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **NavigationType:** the nested enum `MobileParty.NavigationType` has values `None`, `Default`, `Naval`, `All` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Naval state members on MobileParty (v1.4.8):**
  - State: `bool IsCurrentlyAtSea { get; public set; }`, `bool IsInRaftState { get; set; }`, `bool IsTransitionInProgress`, `AnchorPoint Anchor`, `MBReadOnlyList<Ship> Ships`.
  - Capability: `bool HasNavalNavigationCapability`, `bool HasLandNavigationCapability`, `NavigationType NavigationCapability`, `PathFaceRecord CurrentNavigationFace`.
  - Actions: `SetSailAtPosition(CampaignVec2)`, `DisembarkToPosition(CampaignVec2)`, `CancelNavigationTransition()`, `ChangeIsCurrentlyAtSeaCheat()`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **What the naval setters do (IL call listing):**
  - The `IsCurrentlyAtSea` setter passes the value on to attached parties. It also calls `UpdateVersionNo`, raises `CampaignEventDispatcher.OnMobilePartyNavigationStateChanged`, and calls `Party.SetVisualAsDirty()` and `SetNavalVisualAsDirty()`.
  - `ChangeIsCurrentlyAtSeaCheat()` flips `IsCurrentlyAtSea` and resets the anchor position of the party and its attached parties.
  - `SetSailAtPosition` and `DisembarkToPosition` set `IsCurrentlyAtSea`, `Position` and `TargetPosition`, and move attached parties.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **The game's own cursor teleport** is in `SandBox.View.Map.MapScreen.HandleLeftMouseButtonClick(MapEntityVisual, CampaignVec2 intersectionPoint, PathFaceRecord, bool)` (IL call listing). In order, it:
  1. Checks `Input.IsControlDown()`, `Game.Current.CheatMode` and `!MapState.AtMenu`.
  2. Checks `PathFaceRecord.IsValid()` and `!MobileParty.MainParty.IsInRaftState`.
  3. Sets `MainParty.Position = …` and calls `MainParty.SetMoveModeHold()`.
  4. Builds `new CampaignVec2(pos.ToVec2(), …)` and tests `NavigationHelper.IsPositionValidForNavigationType(…, MainParty.IsCurrentlyAtSea…)`. If the position is invalid for the current mode, it calls `MainParty.ChangeIsCurrentlyAtSeaCheat()`.
  5. If the main party leads an army, sets `Position` on every attached party.
  6. Calls `PartyBase.UpdateVisibilityAndInspected(MainParty.Position)` for every `MobileParty.All` and `Settlement.All`.
  7. Logs `"main party cheat move! - X Y"`.
  - Source: [local: SandBox.View.dll][sbv]
- **Getting the map position under the mouse (v1.4.8):**
  - `MapScreen.Instance` is a public static property.
  - `MapScreen.SceneLayer` is a `TaleWorlds.Engine.Screens.SceneLayer`.
  - Signature: `public void GetCursorIntersectionPoint(ref Vec3 clippedMouseNear, ref Vec3 clippedMouseFar, out float closestDistanceSquared, out Vec3 intersectionPoint, ref PathFaceRecord currentFace, out bool isOnland, BodyFlags excludedBodyFlags = …)`. I confirmed which parameters are `ref` and which are `out` by reflection.
  - In `MapScreen.HandleMouse` the game first calls `SceneLayer.SceneView.TranslateMouse(ref near, ref far)`, starting from `Vec3.Zero` and `PathFaceRecord.NullFaceRecord`. It then calls `GetCursorIntersectionPoint` and builds `new CampaignVec2(intersectionPoint.AsVec2, isOnLand)`.
  - `SceneView.TranslateMouse(ref Vec3 worldMouseNear, ref Vec3 worldMouseFar, float maxDistance = -1)` is public.
  - Sources: [local: SandBox.View.dll][sbv]; [local: TaleWorlds.Engine.dll][eng]
- **Other MapScreen members:**
  - `MapCursor` and `MapCameraView` are exposed.
  - Camera: `FastMoveCameraToPosition(CampaignVec2)`, `TeleportCameraToMainParty()`, `ResetCamera(bool resetDistance, bool teleportToMainParty)`.
  - `GetCursorIntersectionPoint` is also used by `MapCursor.BeforeTick` and `MapScreen.CheckCursorState`, found by scanning which methods call it.
  - Source: [local: SandBox.View.dll][sbv]
- **Settlement positions (v1.4.8):** `Settlement.Position`, `GatePosition` and `PortPosition` are `CampaignVec2` with private setters, and `bool HasPort` is exposed. `Settlement.Deserialize` reads `posX/posY`, `gate_posX/gate_posY` and `port_posX/port_posY` from XML — [local: TaleWorlds.CampaignSystem.dll][cs]
- **How the game moves the party when leaving a settlement:** `PlayerTownVisitCampaignBehavior.game_menu_settlement_leave_on_consequence` sets `MainParty.Position = MainParty.CurrentSettlement.GatePosition`. It moves army-attached parties to the same spot, then calls `PlayerEncounter.LeaveSettlement()`, `PlayerEncounter.Finish(true)`, `MainParty.SetMoveModeHold()` and `SaveHandler.SignalAutoSave()` (full IL) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **How War Sails teleports the party home:** `NavalDLC.Storyline.NavalStorylineData.TeleportMainPartyBackToBase` (full IL):
  1. Sets `MainParty.Position = HasNavalNavigationCapability ? Home.PortPosition : Home.GatePosition`.
  2. Sets `MainParty.IsCurrentlyAtSea = HasNavalNavigationCapability`.
  3. Calls `PlayerEncounter.Finish(true)` if an encounter is active.
  4. Calls `RaftStateChangeAction.DeactivateRaftStateForParty` if the party is on a raft.
  5. Ends captivity if the hero is a prisoner.
  6. Resets the anchor with `Anchor.SetPosition(new CampaignVec2(Vec2.Invalid, false))`.
  7. Calls `EncounterManager.StartSettlementEncounter(MainParty, home)`, then `SetMoveModeHold()`.
  8. Calls `MapState.Handler.TeleportCameraToMainParty()` and updates visibility.
  - Source: [local: NavalDLC.dll][naval]
- **Navigation helpers (v1.4.8):**
  - `Helpers.NavigationHelper` has public statics: `IsPositionValidForNavigationType(CampaignVec2 or PathFaceRecord, NavigationType)`, `CanPlayerNavigateToPosition(CampaignVec2, out NavigationType)`, `GetClosestNavMeshFaceCenterPositionForPosition(CampaignVec2, int[] excludedFaceIds)`, `FindReachablePointAroundPosition(CampaignVec2, NavigationType, float maxDistance, …)` and `FindPointAroundPosition(…)`.
  - `Campaign.MapSceneWrapper` (`IMapScene`) adds `GetFaceIndex(ref CampaignVec2)`, `GetAccessiblePointNearPosition(ref CampaignVec2, float radius)`, `GetNearestFaceCenterForPosition(…)`, `GetHeightAtPoint(…)` and `GetTerrainTypeAtPosition(…)`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Hero teleport (separate API):** `TeleportHeroAction` moves heroes, not parties. Methods: `ApplyImmediateTeleportToSettlement(Hero, Settlement)`, `ApplyImmediateTeleportToParty(Hero, MobileParty)`, `ApplyImmediateTeleportToPartyAsPartyLeader`, and `ApplyDelayed…` variants — [local: TaleWorlds.CampaignSystem.dll][cs]. A BUTR reference lists the same signatures — [BUTR GABS campaign-actions][gabs]
- **No console teleport:** the v1.4.8 console cheat list has no party-teleport command. The closest is `campaign.print_player_party_position`, which prints `MainParty.Position.X/Y` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Save data:** `MobileParty._position` is saved (`SaveableField(225)`) and so is `_isCurrentlyAtSea` (`SaveableField(1110)`). A teleport therefore persists through the game's own fields — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Hotkey input:** `TaleWorlds.InputSystem.Input.IsKeyPressed(InputKey)`, `IsKeyDown` and `MousePositionPixel` are public, so a hotkey can be polled in `OnApplicationTick` — [local: TaleWorlds.InputSystem.dll][input]

### Inferences
- **Settlement teleport recipe (easy):**
  1. Make sure there is no active `PlayerEncounter`, or finish it first.
  2. Choose `pos = (party.IsCurrentlyAtSea && s.HasPort) ? s.PortPosition : s.GatePosition`.
  3. Set `MobileParty.MainParty.Position = pos`. Then set `IsCurrentlyAtSea = pos == s.PortPosition` (or follow the War Sails rule).
  4. Call `SetMoveModeHold()` and move army-attached parties.
  5. Call `MapScreen.Instance.TeleportCameraToMainParty()`.
  6. Optionally call `EncounterManager.StartSettlementEncounter(MobileParty.MainParty, s)` to enter the settlement.
- **Cursor teleport recipe (medium):**
  1. On a hotkey while `ScreenManager.TopScreen is MapScreen` and `!MapState.AtMenu`, call `TranslateMouse`, then `GetCursorIntersectionPoint`.
  2. Require `face.IsValid()`.
  3. Build `new CampaignVec2(p.AsVec2, isOnLand)`.
  4. Copy the game's steps: set `Position`, call `SetMoveModeHold()`, toggle `ChangeIsCurrentlyAtSeaCheat()` when `IsPositionValidForNavigationType` fails, move attached parties, call `UpdateVisibilityAndInspected`.
- The mod must reference `SandBox.View.dll`, `TaleWorlds.Engine.dll` and `TaleWorlds.Library.dll`. Our CheatMenu already depends on the SandBox module in SubModule.xml.
- **Risks:**
  - A land-only party (no ships) placed on a sea face must not be left with `IsCurrentlyAtSea=true`. Either refuse the move, or snap to land with `NavigationHelper.FindReachablePointAroundPosition(pos, NavigationType.Default, r)`.
  - The game's cheat refuses to move while the party is in raft state. Do the same.
  - A bad position (an invalid face) is written into the save. Validate the target before setting it.
- Removing the mod afterwards is safe, because only the game's own fields change.

### Gaps
- Not tested at runtime. In particular I did not check whether setting `Position` while the party is inside a settlement (`CurrentSettlement != null`) or during a map event leaves the encounter state inconsistent. Calling `PlayerEncounter.LeaveSettlement()`/`Finish()` first is the cautious default, based on what the leave-settlement code does.
- The call listing does not show the exact branch order inside `HandleLeftMouseButtonClick`, for example which navigation type is passed to `IsPositionValidForNavigationType`.

## 2. Renaming the main hero, companions/any hero, clan, kingdom, settlement and party: does the name persist in the save?

### Takeaway
All six renames work in v1.4.8 without Harmony, and all persist in the save through the game's own fields:
- **Heroes:** `Hero.SetName(fullName, firstName)`.
- **Clans:** `Clan.ChangeClanName(name, informalName)`.
- **Kingdoms:** `Kingdom.ChangeKingdomName(name, informalName)`.
- **Settlements and parties:** `PartyBase.SetCustomName(TextObject)`, called on `settlement.Party` or `mobileParty.Party`.

There is no `Settlement.SetName`. Overwriting the private `Settlement._name` by reflection would NOT persist, because that field is not saved and is reloaded from XML. Difficulty: easy.

### Cited Findings
- **Hero:** `public void Hero.SetName(TextObject fullName, TextObject firstName)` writes `_name` (`SaveableField(181)`) and `_firstName` (`SaveableField(180)`). It also calls `PartyBelongedTo.PartyComponent.ClearCachedName()` when the hero leads a party (IL call listing) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **The game's clan-screen rename for heroes:** `ClanManagement.ClanLordItemVM.OnNamingHeroOver(string)`:
  1. Validates with `CampaignUIHelper.IsStringApplicableForHeroName`.
  2. Builds `GameTexts.FindText("str_generic_character_name")` with `CHARACTER_NAME`/`CHARACTER_GENDER` variables, and the first-name equivalent.
  3. Calls `Hero.SetName`.
  4. Calls `Army.UpdateName()` if the hero leads an army.
  - Source: [local: TaleWorlds.CampaignSystem.ViewModelCollection.dll][vmc]
- **Clan:** `public void Clan.ChangeClanName(TextObject name, TextObject informalName)` calls the private `set_Name`/`set_InformalName`. `Name` and `InformalName` are `SaveableProperty(51/52)`. The clan screen uses it (`ClanManagementVM.OnChangeClanNameDone`) — [local: TaleWorlds.CampaignSystem.dll][cs]; [local: TaleWorlds.CampaignSystem.ViewModelCollection.dll][vmc]
- **Kingdom:** `public void Kingdom.ChangeKingdomName(TextObject name, TextObject informalName)`. `Name`/`InformalName` are `SaveableProperty(1/2)`. The kingdom screen uses it (`KingdomManagementVM.OnChangeKingdomNameDone`) — [local: TaleWorlds.CampaignSystem.dll][cs]; [local: TaleWorlds.CampaignSystem.ViewModelCollection.dll][vmc]
- **Settlement:**
  - `Settlement.Name` has only a getter, and no `SetName` method exists.
  - The getter returns `Party.CustomName` when it is not empty, otherwise the private `_name` (IL call listing).
  - `_name` has NO save attribute, and `Settlement.Deserialize` reloads it from the XML `name` attribute.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **PartyBase.SetCustomName:** `public void SetCustomName(TextObject name)` sets `CustomName` (`SaveableProperty(216)`). For a settlement party it also calls `TextObjectExtensions.SetSettlementProperties(...)` and `SetVisualAsDirty()` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Party names:** `MobileParty.Name` returns `Party.CustomName` when it is not empty, otherwise `PartyComponent.Name` (`LordPartyComponent` caches its generated name in `_cachedName`) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **The game overwrites custom names in some flows:** `DisbandPartyAction.StartDisband`/`CancelDisband`, `TeleportHeroAction.ApplyInternal`, `MobileParty.OnLateLoad` and several quest spawners all call `PartyBase.SetCustomName` (found by scanning which methods call it) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Text input:** `InformationManager.ShowTextInquiry(TextInquiryData, bool pauseGameActiveState=false, bool prioritize=false)`. The `TextInquiryData` constructor takes `… Func<string, Tuple<bool,string>> textCondition, string soundEventPath, string defaultInputText` — [local: TaleWorlds.Library.dll][lib]
- **Community report:** one Steam user said renames kept resetting no matter how often they renamed and saved. That thread was about clan members, and I could not verify its version or cause. I saw it only as a search-result summary, so the exact thread URL is uncertain — [Steam discussion (search result)][steam-rename]

### Inferences
- **Hero rename:** use `hero.SetName(new TextObject(full), new TextObject(first))`. To match the game, wrap the name with `GameTexts.FindText("str_generic_character_name")` as `ClanLordItemVM` does, so gendered string variables still work.
- **Settlement rename:** `settlement.Party.SetCustomName(new TextObject(name))` is the only option that persists without our own save data. Passing null or an empty text should restore the XML name, because the getter falls back to `_name` when `CustomName` is empty.
- **Party rename:** `party.Party.SetCustomName(...)`. Game flows such as disbanding or teleporting a hero into a party leader may overwrite the custom name.
- Every name is a plain game `TextObject` in saved game fields. Removing the mod leaves the names in the save and does no harm. Custom names are literal text, so they will not be translated.

### Gaps
- I could not check whether `CancelDisband` restores an earlier custom name or clears it (the IL listing has no branch detail).
- I did not verify whether `MobileParty.OnLateLoad` resets the `CustomName` of some party types on load, for example by a version-migration check.

## 3. Editing a hero: age, appearance (face generator after creation), attributes, skills, perks, traits

### Takeaway
Most hero editing uses public v1.4.8 APIs and the game's saved fields, without Harmony:
- **Age:** `SetBirthDay`.
- **Skills:** `SetSkillValue` or `HeroDeveloper.SetInitialSkillLevel`.
- **Attributes and focus:** `HeroDeveloper.AddAttribute`, `HeroDeveloper.AddFocus`.
- **Perks:** `HeroDeveloper.AddPerk`.
- **Traits:** `SetTraitLevel`.

Appearance editing is easy for the main hero, because `BarberState` and the game's face generator screen exist. For companions it is harder. The barber screen is hardwired to `Hero.MainHero`, and `CharacterObject.UpdatePlayerCharacterBodyProperties` only writes back `if (IsPlayerCharacter && IsHero)`. Editing a companion's face therefore needs either a custom screen that copies `BodyGenerator.CurrentBodyProperties` into `Hero.StaticBodyProperties`, or a Harmony patch.

### Cited Findings
- **Age:**
  - `Hero.Age` is get-only. `public void Hero.SetBirthDay(CampaignTime)` writes `_birthDay` (`SaveableField(440)`) and `_defaultAge` (`SaveableField(441)`).
  - `Helpers.HeroHelper.GetRandomBirthDayForAge(float age)` is public static.
  - The console cheat `campaign.add_player_age` does `Hero.MainHero.SetBirthDay(HeroHelper.GetRandomBirthDayForAge(age))`, bounded by `AgeModel.HeroComesOfAge` and `AgeModel.MaxAge`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Appearance data:**
  - `Hero.StaticBodyProperties` (public set, `SaveableProperty(100)`), `Hero.Weight` (public set, `SaveableProperty(111)`), `Hero.Build` (public set, `SaveableProperty(112)`), `Hero.IsFemale` (public set).
  - Also `Hero.ModifyHair(int hair, int beard, int tattoo)` and `BodyProperties Hero.BodyProperties { get; }`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Opening the game's face generator from the map:**
  - `MapScreen.OpenFaceGeneratorScreen()` runs when `Campaign.Current.IsFaceGenEnabled` is true. It calls `Game.Current.GameStateManager.CreateState<…>(Hero.MainHero.CharacterObject, CharacterHelper.GetFaceGeneratorFilter())` and then `PushState` (full IL). It is called from `MapScreen.TickNavigationInput` and `OpenFacegenScreenAux`.
  - `Campaign.OnSessionStart` sets `IsFaceGenEnabled = true`. `Campaign.IsFaceGenEnabled` has a public getter and setter.
  - `Helpers.CharacterHelper.GetFaceGeneratorFilter()` is public static.
  - Sources: [local: SandBox.View.dll][sbv]; [local: TaleWorlds.CampaignSystem.dll][cs]
- **BarberState:**
  - `TaleWorlds.CampaignSystem.GameState.BarberState` has a public constructor `(BasicCharacterObject character, IFaceGeneratorCustomFilter filter)` and a public field `Character`.
  - The barber feature uses it: `SandBox.CampaignBehaviors.BarberCampaignBehavior.GivePlayerAHaircut` calls `GameStateManager.CreateState<BarberState>(Hero.MainHero.CharacterObject, GetFaceGenFilter())`, then `PushState`.
  - Sources: [local: TaleWorlds.CampaignSystem.dll][cs]; [local: SandBox.dll][sb]
- **The barber screen is main-hero only:** the constructor `SandBox.GauntletUI.GauntletBarberScreen(BarberState state)` passes `Hero.MainHero.CharacterObject` and `state.Filter` to the body-generator view. It does not read `BarberState.Character` (IL call listing) — [local: SandBox.GauntletUI.dll][sbg]
- **The general face generator screen (v1.4.8):**
  - `TaleWorlds.MountAndBlade.GauntletUI.BodyGenerator.GauntletBodyGeneratorScreen` has a public constructor `(BasicCharacterObject character, bool openedFromMultiplayer, IFaceGeneratorCustomFilter filter)` and a private field `BodyGeneratorView _facegenLayer`.
  - `BodyGeneratorView` exposes public `BodyGen`, `DataSource` (`FaceGenVM`), `GauntletLayer` and `SceneLayer`. Its public constructor takes affirmative and negative `ControlCharacterCreationStage` delegates, the character, a filter, an equipment set and a `FaceGenHistory`.
  - `TaleWorlds.MountAndBlade.BodyGenerator` has a public field `BodyProperties CurrentBodyProperties`, public fields `Race` and `IsFemale`, and a method `SaveCurrentCharacter()`.
  - Sources: [local: TaleWorlds.MountAndBlade.GauntletUI.dll][mbg]; [local: TaleWorlds.MountAndBlade.dll][mb]
- **Done button:** `BodyGeneratorView` `IFaceGeneratorHandler.Done()` calls `BodyGen.SaveCurrentCharacter()`, updates `Mission.Current.MainAgent` if a mission is running, then invokes the affirmative action (full IL) — [local: TaleWorlds.MountAndBlade.GauntletUI.dll][mbg]
- **Why companion edits are lost:** `CharacterObject.UpdatePlayerCharacterBodyProperties(BodyProperties, int race, bool isFemale)` only writes back when `IsPlayerCharacter && IsHero` (full IL; both checks branch to `ret`). In that case it sets `HeroObject.StaticBodyProperties`, `Weight`, `Build`, `Race` and `IsFemale`, and raises `OnPlayerBodyPropertiesChanged`. For a non-player hero the generator's result is thrown away — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Attributes and focus (v1.4.8):**
  - `HeroDeveloper.AddAttribute(CharacterAttribute, int changeAmount, bool checkUnspentPoints = true)`, `RemoveAttribute(CharacterAttribute, int)`, `AddFocus(SkillObject, int, bool checkUnspentFocusPoints = true)`, `RemoveFocus`, `GetFocus`.
  - `UnspentAttributePoints` and `UnspentFocusPoints` have public setters.
  - `Hero.GetAttributeValue` is public. `Hero.SetAttributeValueInternal` is internal.
  - `DefaultCharacterAttributes.Vigor/Control/Endurance/Cunning/Social/Intelligence` are static.
  - Sources: [local: TaleWorlds.CampaignSystem.dll][cs]; [local: TaleWorlds.Core.dll][core]
- **Skills:**
  - `Hero.SetSkillValue(SkillObject, int)` and `GetSkillValue` are public.
  - `HeroDeveloper.SetInitialSkillLevel(SkillObject, int)`, `InitializeSkillXp(SkillObject)`, `ChangeSkillLevel(SkillObject, int, bool shouldNotify = true)`, `AddSkillXp(...)`, `SetInitialLevel(int)` and `CheckLevel(bool)`.
  - The cheat `campaign.set_skill_player` uses `SetInitialSkillLevel` followed by `InitializeSkillXp`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Perks:**
  - `HeroDeveloper.AddPerk(PerkObject)`, `Hero.GetPerkValue(PerkObject)` and `Hero.ClearPerks()` are public. `Hero.SetPerkValueInternal` is internal.
  - `PerkObject.All`, `.Skill`, `.RequiredSkillValue` and `.AlternativePerk` are public.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Traits:**
  - `Hero.SetTraitLevel(TraitObject, int)` clamps to `MinValue..MaxValue`. `GetTraitLevel` is public.
  - `DefaultTraits.Mercy/Valor/Honor/Generosity/Calculating` and `DefaultTraits.Personality` are exposed.
  - The cheats `campaign.set_hero_trait` and `campaign.set_player_trait` call `SetTraitLevel`, then for the main hero `TraitLevelingHelper.UpdateTraitXPAccordingToTraitLevels()` and `CampaignEventDispatcher.Instance.OnPlayerTraitChanged(trait, previousLevel)`. Both are public.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Save fields:** `_heroTraits` (270), `_heroPerks` (280), `_heroSkills` (290), `_characterAttributes` (301) and `Level` (370) are all `SaveableField` — [local: TaleWorlds.CampaignSystem.dll][cs]

### Inferences
- **Main hero appearance:** push the same state the game uses, `GameStateManager.Current.PushState(Game.Current.GameStateManager.CreateState<BarberState>(Hero.MainHero.CharacterObject, CharacterHelper.GetFaceGeneratorFilter()))`. No Harmony is needed, and the result persists through the saved `StaticBodyProperties`.
- **Companion or other hero appearance, option A (no Harmony):**
  1. Write a small `ScreenBase` that hosts a `BodyGeneratorView` built with `hero.CharacterObject`.
  2. In the affirmative callback, copy `view.BodyGen.CurrentBodyProperties` into `hero.StaticBodyProperties = bp.StaticProperties; hero.Weight = bp.Weight; hero.Build = bp.Build`.
  3. Pop the screen.
  - Alternatively push `GauntletBodyGeneratorScreen` and read its private `_facegenLayer` by reflection when it closes.
- **Companion appearance, option B:** a Harmony postfix on `CharacterObject.UpdatePlayerCharacterBodyProperties` that also writes back for non-player heroes. This adds a dependency on Bannerlord.Harmony.
- For exact values, use `SetInitialSkillLevel` + `InitializeSkillXp` for skills. Use `AddAttribute(attr, delta, checkUnspentPoints:false)` for attributes. For traits on the main hero, call `TraitLevelingHelper.UpdateTraitXPAccordingToTraitLevels()` after setting them, as the cheat does. Otherwise trait XP may later move the level back. This last point is my inference from the cheat's code.
- Removing a single perk needs reflection on internal `Hero.SetPerkValueInternal(perk, false)`. `ClearPerks()` removes all.
- Every edit lands in the game's own saved fields, so removing the mod is safe.
- **Difficulty:** age, skills, attributes and traits are easy. Perks are easy to add and medium to remove. Main-hero appearance is easy. Companion appearance is medium-hard.

### Gaps
- Not tested at runtime: whether `BarberState` opened from the map outside a settlement behaves correctly, and whether the face generator's age slider changes the hero's real age or only the visual age.
- I did not check how changing `BirthDay` interacts with the BirthAndDeath official module (pregnancy/aging), or whether `BodyProperties.Age` is recomputed from `Hero.Age`.

## 4. Relations: setting the relation between the player and any hero

### Takeaway
Easy, no Harmony. `ChangeRelationAction.ApplyPlayerRelation`/`ApplyRelationChangeBetweenHeroes` apply a *delta*. That delta is scaled by `DiplomacyModel.GetRelationIncreaseFactor` and redirected to "effective" heroes such as clan leaders. To set an *exact* value, use the public `Hero.SetPersonalRelation(otherHero, value)` or `CharacterRelationManager.SetHeroRelation(h1, h2, value)`. Relations are saved natively.

### Cited Findings
- **Methods:** `ChangeRelationAction.ApplyPlayerRelation(Hero gainedRelationWith, int relation, bool affectRelatives = true, bool showQuickNotification = true)`, `ApplyRelationChangeBetweenHeroes(Hero, Hero, int relationChange, bool showQuickNotification = true)`, `ApplyEmissaryRelation(Hero, Hero, int, bool)` — [local: TaleWorlds.CampaignSystem.dll][cs]; the same list appears in [BUTR GABS campaign-actions][gabs]
- **What ApplyInternal does (IL call listing):**
  1. Multiplies the change by `DiplomacyModel.GetRelationIncreaseFactor` with `MBRandom.RoundRandomized`.
  2. Resolves target heroes with `DiplomacyModel.GetHeroesForEffectiveRelation`.
  3. Clamps the result.
  4. Calls `Hero.SetPersonalRelation`.
  5. Raises `CampaignEventDispatcher.OnHeroRelationChanged`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Exact setters:** `public void Hero.SetPersonalRelation(Hero otherHero, int value)` clamps to `DiplomacyModel.MinRelationLimit..MaxRelationLimit` and calls `CharacterRelationManager.SetHeroRelation`. Static `CharacterRelationManager.SetHeroRelation(Hero, Hero, int)` and `GetHeroRelation(Hero, Hero)` are public. `Hero.GetRelation(Hero)`, `GetBaseHeroRelation(Hero)` and `GetRelationWithPlayer()` exist — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Console cheat:** `campaign.add_hero_relation [Hero] | [OtherHero] | [Value]` uses `ChangeRelationAction.ApplyRelationChangeBetweenHeroes`. It also supports `all` over `Hero.AllAliveHeroes` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Event:** `CampaignEventDispatcher.OnHeroRelationChanged(Hero effectiveHero, Hero effectiveHeroGainedRelationWith, int relationChange, bool showNotification, ChangeRelationDetail detail, Hero originalHero, Hero originalGainedRelationWith)` is public — [local: TaleWorlds.CampaignSystem.dll][cs]

### Inferences
- **Set exactly:** `Hero.MainHero.SetPersonalRelation(target, v)`. Optionally raise `OnHeroRelationChanged` so UI and listeners refresh.
- **Add a delta the game's way:** `ChangeRelationAction.ApplyPlayerRelation(target, delta, affectRelatives:false, showQuickNotification:true)`.
- Because of the effective-relation logic, a relation set with a clan member may actually show up as the relation with that clan's leader. Test this in game.
- Relations live in the game's relation manager, so removing the mod is safe.

### Gaps
- I did not inspect what `GetHeroesForEffectiveRelation` returns for companions vs lords.
- I did not dump the relation limits (likely ±100, unverified).

## 5. Settlements: change owner, prosperity, loyalty, security, food, finish building projects

### Takeaway
All easy and without Harmony:
- **Owner:** `ChangeOwnerOfSettlementAction.ApplyByDefault(hero, settlement)`, the same call the game's console cheat uses.
- **Stats:** `Town.Prosperity`, `Town.Loyalty` and `Town.Security` have public setters, apparently clamped; `Fief.FoodStocks`, `Settlement.Militia` and `Village.Hearth` have public setters.
- **Buildings:** set `Building.BuildingProgress` to `GetConstructionCost()`, then call the public `Helpers.BuildingHelper.CheckIfBuildingIsComplete(building)` in queue order.

Everything is saved in game fields.

### Cited Findings
- **Owner change methods:** `ChangeOwnerOfSettlementAction.ApplyByDefault(Hero, Settlement)`, `ApplyByKingDecision(Hero, Settlement)`, `ApplyBySiege(Hero newOwner, Hero capturerHero, Settlement)`, `ApplyByLeaveFaction`, `ApplyByBarter`, `ApplyByRebellion`, `ApplyByDestroyClan(Settlement, Hero newOwner)`, `ApplyByGift(Settlement, Hero newOwner)` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Console cheat:** `campaign.give_settlement_to_player` uses `ChangeOwnerOfSettlementAction.ApplyByDefault(Hero.MainHero, settlement)`. It refuses villages ("Settlement must be castle or town") and refuses while `Clan.PlayerClan.IsUnderMercenaryService` ("Player cannot take ownership of a settlement during mercenary service"). The keyword "calradia" gives every town and castle — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Town stats:**
  - `Town.Prosperity`, `Security` and `Loyalty` have public setters.
  - The Security and Loyalty setters write their backing field three times and Prosperity's twice. That suggests clamping, which I did not confirm.
  - The cheat `campaign.set_prosperity_of_settlement` calls `Town.set_Prosperity` and refuses villages.
  - Related cheats: `set_loyalty_of_settlement`, `set_security_of_settlement`, `set_food_of_settlement`, `set_militia_of_settlement`, `set_hearth_of_settlement`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Other stats:** `Fief.FoodStocks` has a public setter with no clamp; it is an auto-property. `Town.FoodStocksUpperLimit()` exists. `Settlement.Militia` has a public setter. `Village.Hearth` and `Village.VillageState` have public setters — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Save fields:** `Town._prosperity` (`SaveableField(1017)`), `_security` (1015), `_loyalty` (1014), `Buildings` (1006), `BuildingsInProgress` (1007); `Fief.FoodStocks` (`SaveableProperty(100)`); `Building.BuildingProgress` (`SaveableField(1)`), `_currentLevel` (3) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Building API:** `Town.Buildings` (`MBList<Building>`), `Town.BuildingsInProgress` (`Queue<Building>`), `Town.CurrentBuilding`, `Town.BoostBuildingProcess`; `Building.CurrentLevel` (public set), `LevelUp()`, `LevelDown()`, `GetConstructionCost()`, public field `BuildingProgress`, `IsCurrentlyDefault`, `BuildingType`; `BuildingType.MaxLevel` (static field) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **How the game completes a building:** `Helpers.BuildingHelper.CheckIfBuildingIsComplete(Building)` is public static (full IL):
  - If `BuildingProgress >= GetConstructionCost()`: when `CurrentLevel < 3` it calls `LevelUp()`. When the level is now 3 it sets `BuildingProgress = GetConstructionCost()`. Finally it calls `building.Town.BuildingsInProgress.Dequeue()`.
  - `Building.LevelUp()` raises `CampaignEventDispatcher.OnBuildingLevelChanged`.
  - Related helpers: `BuildingHelper.BoostBuildingProcessWithGold(int gold, Town)`, `ChangeCurrentBuildingQueue(List<Building>, Town)`, `ChangeDefaultBuilding(Building, Town)`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Console cheat:** `campaign.add_building_level` sets `CurrentLevel++` and raises `OnBuildingLevelChanged(town, building, 1)`. It refuses settlements that are under siege, being raided, or in a map event — [local: TaleWorlds.CampaignSystem.dll][cs]

### Inferences
- **Finish all queued projects:** `while (town.BuildingsInProgress.Count > 0) { var b = town.BuildingsInProgress.Peek(); b.BuildingProgress = b.GetConstructionCost(); BuildingHelper.CheckIfBuildingIsComplete(b); }`. Always use `Peek()`, because the helper dequeues the head of the queue whatever building it is given.
- Guard against siege, raid and map event, as the game's cheat does.
- Owner changes through the action raise the proper events (governor, kingdom ownership and so on). Assigning `Town.OwnerClan` directly (public setter) would skip them. Prefer the action.
- Everything persists in the game's own fields, so removing the mod is safe.

### Gaps
- I did not confirm the exact clamp limits of `Loyalty` and `Security` (likely 0..100) or the prosperity minimum.
- I did not check whether `ApplyByDefault` on a village is meaningful, since villages follow their bound town.

## 6. Kingdom and clan: tier/renown, influence, join/leave kingdom, war/peace, create kingdom

### Takeaway
All available as public v1.4.8 actions, without Harmony:
- **Renown and tier:** `GainRenownAction.Apply` or `Clan.AddRenown` (tier recalculates).
- **Influence:** `ChangeClanInfluenceAction.Apply` or the `Clan.Influence` setter.
- **Join/leave:** `ChangeKingdomAction.ApplyByJoinToKingdom`/`ApplyByLeaveKingdom`/`ApplyByJoinFactionAsMercenary`.
- **War/peace:** `DeclareWarAction.ApplyByDefault`, `MakePeaceAction.Apply`.
- **Create kingdom:** `Campaign.Current.KingdomManager.CreateKingdom(...)`, the same as the console cheat `create_player_kingdom`.
- **Become ruler:** `ChangeRulingClanAction.Apply`.

`Clan.Tier` has a private setter, so set an exact tier by setting renown and then recalculating. Difficulty: easy, except create-kingdom, which is medium because of edge cases.

### Cited Findings
- **Renown and tier:**
  - `Clan.Renown` has a public setter (`SaveableProperty(88)`), but setting it does not update the tier.
  - `Clan.AddRenown(float value, bool shouldNotify = true)` adds renown, recalculates the tier with `ClanTierModel.CalculateTier(clan)`, sets the private `Tier` (`_tier`, `SaveableField(91)`) and raises `OnClanTierChanged`.
  - `GainRenownAction.Apply(Hero, float renownValue, bool doNotNotify = false)` is the action form.
  - `ClanTierModel.GetRequiredRenownForTier(int tier)`, `MinClanTier` and `MaxClanTier` are public.
  - `Clan.ResetClanRenown()` exists.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Influence:** `Clan.Influence` has a public setter. When `Leader` is set, the setter calls `SkillLevelingManager.OnInfluenceSpent` (IL call listing). `ChangeClanInfluenceAction.Apply(Clan, float)` is the action form — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Join/leave kingdom:**
  - `ChangeKingdomAction.ApplyByJoinToKingdom(Clan, Kingdom, CampaignTime shouldStayInKingdomUntil = default, bool showNotification = true)`, `ApplyByJoinToKingdomByDefection(Clan, Kingdom old, Kingdom new, …)`, `ApplyByJoinFactionAsMercenary(Clan, Kingdom, CampaignTime = default, int awardMultiplier = 50, bool = true)`, `ApplyByLeaveKingdom(Clan, bool)`, `ApplyByLeaveKingdomAsMercenary(Clan, bool)`, `ApplyByLeaveWithRebellionAgainstKingdom(Clan, bool)`, `ApplyByCreateKingdom(Clan, Kingdom, bool)`.
  - Also `Clan.ClanLeaveKingdom(bool giveBackFiefs = false)`.
  - The cheat `campaign.join_kingdom` refuses when `Hero.MainHero.IsKingdomLeader`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **War and peace:**
  - `DeclareWarAction.ApplyByDefault(IFaction, IFaction)`, plus `ApplyByKingdomDecision`, `ApplyByPlayerHostility`, `ApplyByRebellion`, `ApplyByCrimeRatingChange`, `ApplyByKingdomCreation`, `ApplyByClaimOnThrone`, `ApplyByCallToWarAgreement`.
  - `MakePeaceAction.Apply(IFaction, IFaction)` and `ApplyByKingdomDecision(IFaction, IFaction, int dailyTributeFrom1To2, int dailyTributeDuration)`.
  - The cheat `campaign.declare_peace` checks that the factions differ, that each is a map faction (not a clan bound to a kingdom), that they are not at permanent war (`FactionManager.IsAtConstantWarAgainstFaction`, e.g. bandits) and that they are actually at war. Then it calls `MakePeaceAction.Apply`.
  - Sources: [local: TaleWorlds.CampaignSystem.dll][cs]; the same list is in [BUTR GABS campaign-actions][gabs]
- **Create kingdom:**
  - Signature: `KingdomManager.CreateKingdom(TextObject kingdomName, TextObject informalName, CultureObject culture, Clan founderClan, MBReadOnlyList<PolicyObject> initialPolicies = null, TextObject encyclopediaText = null, TextObject encyclopediaTitle = null, TextObject encyclopediaRulerTitle = null)`. `Campaign.KingdomManager` is a public field.
  - Internally (IL call listing):
    1. Creates the object with `Kingdom.CreateKingdom("new_kingdom")`. If no name is given it defaults to "Kingdom of {NAME}".
    2. Calls `InitializeKingdom(...)` with the founder clan's banner, `Color`/`Color2` and `HomeSettlement`.
    3. Calls `ChangeKingdomAction.ApplyByCreateKingdom`.
    4. Calls `DeclareWarAction.ApplyByKingdomCreation` for the clan's existing wars.
    5. Calls `AddPolicy` for each initial policy, then `OnKingdomCreated`.
  - The cheat `campaign.create_player_kingdom` passes `Clan.PlayerClan.Name`, `InformalName`, `Culture` and `PlayerClan`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **Ruler and kingdom management:** `ChangeRulingClanAction.Apply(Kingdom, Clan)` (used by the cheats `lead_kingdom`/`rule_your_faction`), `DestroyKingdomAction.Apply(Kingdom)`, `Kingdom.AddPolicy`/`RemovePolicy`, `Kingdom.RulingClan` (public set), `ChangeClanLeaderAction.ApplyWithSelectedNewLeader(Clan, Hero)` — [local: TaleWorlds.CampaignSystem.dll][cs]

### Inferences
- **Exact tier:** `clan.Renown = Campaign.Current.Models.ClanTierModel.GetRequiredRenownForTier(t); clan.AddRenown(0f);`. `AddRenown` recalculates the tier, and the `Tier` setter is private. This is my inference from the IL, and only from `AddRenown`'s body.
- **Creating a kingdom while already a vassal:** I did not check whether `ApplyByCreateKingdom` first takes the clan out of its current kingdom. The cautious order is `ApplyByLeaveKingdom` → `CreateKingdom`. The founder clan should have a valid `HomeSettlement` (it feeds `InitializeKingdom`).
- A newly created `Kingdom` is a regular game object saved by the game. Removing the mod does not break it.

### Gaps
- I did not inspect `ClanTierModel.CalculateTier` for whether lowering renown also lowers the tier.
- Not tested: what `CreateKingdom` does when the player clan is a mercenary or vassal, or has no fiefs.

## 7. Time control: campaign speed beyond the game's 4x

### Takeaway
Easy, no Harmony. `Campaign.SpeedUpMultiplier` is a public `float` with no cap in its setter. The game resets it to 4 in the `Campaign` constructor and again in `Campaign.OnLoad`, and it is NOT saved. The mod must reapply it after every load or new game. The game's console cheat caps it at 15, or 30 in development mode. Our mod can exceed that, with a risk of coarse simulation steps.

### Cited Findings
- **The property:** `Campaign.SpeedUpMultiplier { get; public set; }` is an auto-property with no attribute, so it is not saved. Its setter only writes the backing field. The `Campaign` constructor and `Campaign.OnLoad` both execute `ldc.r4 4; set_SpeedUpMultiplier` (full IL), so the default is 4 and it resets on every load — [local: TaleWorlds.CampaignSystem.dll][cs]
- **How it is applied:** `Campaign.TickMapTime(float realDt)` (full IL) computes `dt = 0.25 * realDt` for normal play and `0.25 * realDt * SpeedUpMultiplier` for fast-forward modes, unless the main party is waiting. It stores `_dt` and calls `MapTimeTracker.Tick(4320 * dt)` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **The console cheat:** `campaign.set_campaign_speed_multiplier [value]` uses a maximum of 15, or 30 when `Game.Current.IsDevelopmentMode`. It clamps to the maximum with the message "…which is the maximum value for speed up multiplier!" (full IL) — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Time modes:** `Campaign.TimeControlMode` (public set; the setter honours `TimeControlModeLock`), `Campaign.SetTimeSpeed(int)` and `SetTimeControlModeLock(bool)`. `CampaignTimeControlMode` values are `Stop`, `UnstoppablePlay`, `UnstoppableFastForward`, `StoppablePlay`, `StoppableFastForward`, `UnstoppableFastForwardForPartyWaitTime`, `FastForwardStop` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Who writes it:** only `Campaign.OnLoad` and `CampaignCheats.SetCampaignSpeed` write `SpeedUpMultiplier` in CampaignSystem, SandBox, SandBox.View, SandBox.GauntletUI, NavalDLC and FastMode — [local: TaleWorlds.CampaignSystem.dll][cs]
- **Saving our own settings:** `CampaignBehaviorBase` has `virtual void SyncData(IDataStore dataStore)`. `IDataStore` has `bool SyncData<T>(string key, ref T data)`, `IsSaving` and `IsLoading`. `CampaignGameStarter.AddBehavior(CampaignBehaviorBase)` registers a behavior. Custom saveable types require a `SaveableTypeDefiner` subclass with `AddClassDefinition(Type, int saveId, …)` — [local: TaleWorlds.CampaignSystem.dll][cs]; [local: TaleWorlds.SaveSystem.dll][savesys]
- **Removal risk of custom save types:** the official docs warn that if custom data types are saved into the game's storage, the save will not load after the mod is removed. They advise shipping a cleanup feature or mod — [docs.bannerlordmodding.com Save System][docs-save]
- **FastMode module:** the installed build ships an official optional module "Fast Mode" (`FastMode`, `ModuleType OfficialOptional`, `TaleWorlds.CampaignSystem.FastMode.dll`). It exposes only `FastModeOptionsProvider.GetGameplayCampaignOptions()`/`GetCharacterCreationCampaignOptions()`. I did not analyse what it changes — [local: FastMode SubModule.xml][fast]

### Inferences
- **Recipe:** keep our multiplier in mod state. Apply it in `OnGameLoaded`/`OnNewGameCreated`, or better on `CampaignEvents.OnSessionLaunchedEvent`, because `OnLoad` resets it to 4. Reapply when the user changes it. Our existing `CleanSaveGuard` tick hook is another option.
- **Risks above about 16x:** `dt` per tick grows, which coarsens AI decisions and party movement and may make parties skip encounters. This is my inference from the formula; not tested.
- An alternative that also speeds up normal play is not possible through this property. The 1x mode does not use the multiplier.
- Nothing is saved, so removing the mod has no effect on saves.
- If we want a per-campaign multiplier, store it as a plain `float` with `SyncData`. My inference: a primitive needs no `SaveableTypeDefiner` and so avoids the removal risk that applies to custom types. I did not test removing a mod that saved primitive SyncData; storing the setting in a config file is the zero-risk option.
- The same rule covers the whole feature set. Features 1–8 can all be built on game-saved fields only, so CheatMenu needs no custom save types.

### Gaps
- I found no source measuring stability at high multipliers in 1.3/1.4.
- I did not analyse the FastMode module's options.

## 8. Items: add any item to the inventory, with a searchable list

### Takeaway
Easy, no Harmony, no XML. Enumerate `MBObjectManager.Instance.GetObjectTypeList<ItemObject>()`, then add with `MobileParty.MainParty.ItemRoster.AddToCounts(item, n)`, as the game's cheat does. The game's `MultiSelectionInquiryData` in v1.4.8 has an optional `isSeachAvailable` parameter (TaleWorlds' spelling) that adds a search box. Item icons now use `TaleWorlds.Core.ImageIdentifiers.ItemImageIdentifier`.

### Cited Findings
- **Object manager:** `MBObjectManager.Instance` is static. Generic methods: `GetObjectTypeList<T>()` returns `MBReadOnlyList<T>`; `GetObject<T>(string objectName)`, `GetObjects<T>(Func<T,bool>)` and `GetObject<T>(Func<T,bool>)` also exist — [local: TaleWorlds.ObjectSystem.dll][obj]
- **The game's add-item cheat:** `campaign.add_item_to_player_party [ItemId] | [ModifierId] | [Amount]` uses `Game.Current.ObjectManager.GetObject<ItemObject>(id)`, then `MobileParty.MainParty.ItemRoster.AddToCounts(item, amount)`, or `AddToCounts(new EquipmentElement(item, modifier), amount)` when a modifier is given — [local: TaleWorlds.CampaignSystem.dll][cs]
- **ItemRoster:** it lives in `TaleWorlds.CampaignSystem.Roster.ItemRoster`, with `AddToCounts(ItemObject, int)`, `AddToCounts(EquipmentElement, int)`, `Add(ItemRosterElement)`, `Remove(ItemRosterElement)` and `RemoveIf(Func<…>)` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **ItemObject properties useful for filters:** `Name`, `ItemType` (`ItemTypeEnum`), public field `Type`, `Tier` (`ItemTiers`), `Tierf`, `Value`, `Culture`, `ItemCategory`, `NotMerchandise`, `IsCraftedByPlayer`, `IsCraftedWeapon`, `WeaponDesign` — [local: TaleWorlds.Core.dll][core]
- **Popup with search:** the v1.4.8 constructor is `MultiSelectionInquiryData(string titleText, string descriptionText, List<InquiryElement> inquiryElements, bool isExitShown, int minSelectableOptionCount, int maxSelectableOptionCount, string affirmativeText, string negativeText, Action<List<InquiryElement>> affirmativeAction, Action<List<InquiryElement>> negativeAction, string soundEventPath = "", bool isSeachAvailable = false)`. Public field `IsSeachAvailable` — [local: TaleWorlds.Core.dll][core]
- **List entries and icons:**
  - `InquiryElement(object identifier, string title, ImageIdentifier imageIdentifier)` and `(…, bool isEnabled, string hint)` constructors.
  - `ImageIdentifier` is now `TaleWorlds.Core.ImageIdentifiers.ImageIdentifier`, a base class with a parameterless constructor and `Id`/`TextureProviderName`/`AdditionalArgs`.
  - Concrete subclasses: `ItemImageIdentifier(ItemObject item, string bannerCode = null)`, `CharacterImageIdentifier(CharacterCode)`, `BannerImageIdentifier(Banner, bool nineGrid = false)`, `CraftingPieceImageIdentifier`, `EmptyImageIdentifier`, `PlayerAvatarImageIdentifier`.
  - Source: [local: TaleWorlds.Core.dll][core]
- **Old namespace:** in v1.2.12 `InquiryElement` took a `TaleWorlds.Core.ImageIdentifier`, and the namespace `ImageIdentifiers` did not exist in that link structure — [apidoc 1.2.12 InquiryElement][api1212-ie]

### Inferences
- **Recipe:**
  1. Open a category step first (`ItemType` or weapon/armor/horse/goods), then a list filtered by type with `isSeachAvailable: true`.
  2. Show each entry as `new InquiryElement(item, $"{item.Name} (T{(int)item.Tier+1}, {item.Value}d)", new ItemImageIdentifier(item))`.
  3. Allow multi-select and ask for the amount with `ShowTextInquiry`.
  - Showing all items (several thousand) in one popup is possible but slow and hard to scroll.
- Added items are plain roster entries and persist natively. The usual Bannerlord caveat applies: items defined by *other* mods break saves if that other mod is later removed. Our mod adds no new types, so removing it is safe.
- Filter out items that should not be handed out freely, such as quest/special items, `NotMerchandise`, or crafted designs with `IsCraftedByPlayer`, unless the user asks for them. This is my judgement; I did not confirm side effects.

### Gaps
- I did not confirm when `isSeachAvailable` was added. The v1.2.12 constructor was not fetched.
- I did not test the popup's search box, its matching behaviour, or performance with more than 1,000 elements.

## 9. UI options for a large all-in-one mod: MultiSelectionInquiry popups vs custom Gauntlet UI vs MCM; minimal Gauntlet overlay in 1.3/1.4

### Takeaway
- **Popups** need no XML, survive patches best and now have built-in search. They are fine for action menus, but navigating them one step at a time gets awkward at scale.
- **A custom Gauntlet panel** is fully possible in v1.4.8. It needs a prefab XML in `Modules/<Id>/GUI/Prefabs`, a `ViewModel`, and a `GauntletLayer` added to `ScreenManager.TopScreen` with input focus. The Gauntlet API changed in 1.3/1.4: the `GauntletLayer` constructor now requires a name first, and `LoadMovie` returns a `GauntletMovieIdentifier`. Many older tutorials are therefore out of date.
- **MCM** (v5.13.x supports v1.0.x–v1.5.x) suits persistent settings such as toggles and multipliers, not entity pickers. It adds Harmony/ButterLib/UIExtenderEx dependencies.

A hybrid is the pragmatic choice: popups (or one Gauntlet panel) for actions, MCM or our own settings for toggles.

### Cited Findings
- **v1.4.8 Gauntlet API:**
  - Constructor `GauntletLayer(string name, int localOrder, bool shouldClear = false)`; `name` is required and only `shouldClear` is optional.
  - `GauntletMovieIdentifier LoadMovie(string movieName, ViewModel dataSource)`, `ReleaseMovie(GauntletMovieIdentifier)`, `GetMovieIdentifier(string movieName)`, `UIContext UIContext { get; }`.
  - Source: [local: TaleWorlds.Engine.GauntletUI.dll][eg]
- **v1.2.12 Gauntlet API:** `GauntletLayer(int localOrder, string categoryId = "GauntletLayer", bool shouldClear = false)`, `IGauntletMovie LoadMovie(string movieName, ViewModel dataSource)`, `void ReleaseMovie(IGauntletMovie movie)` — [apidoc 1.2.12 GauntletLayer][api1212-gl]
- **The community docs example is pre-1.3:** it uses `new GauntletLayer(100) { IsFocusLayer = true }`, `AddLayer`, `InputRestrictions.SetInputRestrictions()`, `_movie = _gauntletLayer.LoadMovie("MyExampleMovie", _dataSource)` (stored as a `GauntletMovie`), `ScreenManager.TrySetFocus`/`TryLoseFocus`, `RemoveLayer` in `OnFinalize`, and `ScreenManager.PushScreen(ViewCreatorManager.CreateScreenView<MyExampleScreen>())`. The page states no game version — [docs.bannerlordmodding.com ScreenBase][docs-screen]
- **Screen system (v1.4.8):**
  - `ScreenManager.TopScreen` (static get), `PushScreen`, `PopScreen`, `ReplaceTopScreen`, `TrySetFocus(ScreenLayer)`, `TryLoseFocus(ScreenLayer)`, `AddGlobalLayer(GlobalLayer, bool isFocusable)`, `RemoveGlobalLayer`, `SetSuspendLayer`.
  - `ScreenBase.AddLayer`, `RemoveLayer`, `HasLayer`, `FindLayer<T>()`, `Layers`.
  - `ScreenLayer.IsFocusLayer` (public set), `InputRestrictions`, `Input`.
  - `InputRestrictions.SetInputRestrictions(bool isMouseVisible = true, InputUsageMask mask = All)`, `ResetInputRestrictions()`, `SetMouseVisibility(bool)`.
  - Source: [local: TaleWorlds.ScreenSystem.dll][ss]
- **ViewModel base:** `TaleWorlds.Library.ViewModel` (abstract) has `OnPropertyChanged(string)`, `OnPropertyChangedWithValue<T>(T, string)` and overloads, protected `SetField<T>(ref T, T, string)`, and virtual `RefreshValues()`/`OnFinalize()`. The `DataSourceProperty` attribute exists — [local: TaleWorlds.Library.dll][lib]
- **Prefab loading:** `GauntletMovie.Load(UIContext, WidgetFactory, string movieName, IViewModel, bool doNotUseGeneratedPrefabs, bool hotReloadEnabled)` exists. `UIResourceManager` exposes `ResourceDepot`, `WidgetFactory`, `SpriteData`, `BrushFactory` and `LoadSpriteCategory(string)` — [local: TaleWorlds.GauntletUI.Data.dll][gd]; [local: TaleWorlds.Engine.GauntletUI.dll][eg]
- **Folder layout:** official modules ship UI as `GUI/Prefabs` and `GUI/Brushes` folders (Native: 149 prefab XMLs, SandBox: 177, NavalDLC: 30). Mods put their movie XML in `Modules/<ModId>/GUI/Prefabs/<MovieName>.xml` — [local: Modules folder][mods]
- **Overriding official UI:** the official tutorial says mods that overwrite Gauntlet UIs from official modules must load before that module — [docs.bannerlordmodding.com (search summary)][docs-gauntlet-nocs]
- **Popup APIs (v1.4.8):** `MBInformationManager.ShowMultiSelectionInquiry(MultiSelectionInquiryData, bool pauseGameActiveState = false, bool prioritize = false)`, `InformationManager.ShowInquiry(...)`, `ShowTextInquiry(...)`, `IsAnyInquiryActive()`, `HideInquiry()` — [local: TaleWorlds.Core.dll][core]; [local: TaleWorlds.Library.dll][lib]
- **MCM:** latest MCM is 5.13.1 (dated 17 Oct, year not shown), stating "For v1.0.x/v1.1.x/v1.2.x/v1.3.x/v1.4.x/v1.5.x". 5.11.4 (24 Apr) was the first listed with v1.4.x — [MCM releases (GitHub)][mcm]
- **Harmony:** the latest BUTR Bannerlord.Harmony release is 2.4.2.248 (21 Jul). The releases page states no game version — [Bannerlord.Harmony releases][harmony]. Bannerlord.Harmony is not installed locally; the Modules folder contains only official modules plus CheatMenu — [local: Modules folder][mods]
- **ViewModel mixins:** UIExtenderLib explains that Gauntlet fills prefabs from `ViewModel` descendants and that mixins can add properties to existing ViewModels. That is the original shdwp library; BUTR maintains a successor — [UIExtenderLib wiki][uiext]

### Inferences
- **Minimal v1.4.8 overlay (no Harmony, no new screen):**
  ```csharp
  var vm = new CheatPanelVM(onClose);
  var layer = new GauntletLayer("CheatMenuPanel", 1000);   // name first in 1.3+/1.4
  var movieId = layer.LoadMovie("CheatMenuPanel", vm);      // Modules/CheatMenu/GUI/Prefabs/CheatMenuPanel.xml
  layer.InputRestrictions.SetInputRestrictions();           // mouse visible, all input
  layer.IsFocusLayer = true;
  ScreenManager.TopScreen.AddLayer(layer);
  ScreenManager.TrySetFocus(layer);
  // close:
  layer.InputRestrictions.ResetInputRestrictions();
  layer.IsFocusLayer = false; ScreenManager.TryLoseFocus(layer);
  layer.ReleaseMovie(movieId); ScreenManager.TopScreen.RemoveLayer(layer); vm.OnFinalize();
  ```
  - Close it automatically if the top screen changes, for example when entering a mission or menu.
  - The prefab can reuse the game's brushes and sprites (e.g. `Popup.*`, `ButtonBrush*`) instead of shipping new ones. Scrolling lists need `ScrollablePanel` + `ListPanel` with `DataSource="{Items}"`.
- **Trade-offs for a large all-in-one mod:**
  - **Popups:** about zero UI maintenance across patches, but no multi-column layout and the "back" flow is clunky. CheatMenu's deferred-popup queue already works around popup re-entrancy.
  - **Gauntlet:** best UX (tabs, search, sliders), but the prefab XML and `ViewModel` binding names must be maintained, and this is exactly what broke in 1.3. Effort is medium-high.
  - **MCM:** ideal for persistent toggles and multipliers (e.g. campaign speed). It cannot replace pickers, and it forces users to install Harmony + ButterLib + UIExtenderEx + MCM.
- **Recommendation:** keep popups for actions now and improve them with `isSeachAvailable` and item icons. Add optional MCM settings later, or one Gauntlet panel if UX demands it.

### Gaps
- I did not fetch a working 1.3/1.4 open-source Gauntlet example to confirm that runtime XML prefab loading from a mod's `GUI/Prefabs` still works unchanged now that official modules ship `*.GauntletUI.AutoGenerated*.dll` code-generated prefabs. The local `GauntletMovie.Load(..., bool doNotUseGeneratedPrefabs, ...)` parameter suggests that XML and generated prefabs coexist.
- I did not verify UIExtenderEx and ButterLib support for v1.4.8. The Steam/Nexus pages were blocked (HTTP 403/429).

## 10. API changes in 1.3/1.4 that commonly broke older mods

### Takeaway
- **War Sails (1.3.0, Nov 2025)** brought naval navigation into the campaign core. Positions moved from `Vec2` to `CampaignVec2` (`MobileParty.Position2D` was removed), and party movement orders moved from `MobilePartyAi` to `MobileParty` with a `NavigationType` parameter. Parties also gained sea/raft/anchor state.
- **UI:** `ImageIdentifier` was split into `TaleWorlds.Core.ImageIdentifiers.*` subclasses, and `GauntletLayer`'s constructor and `LoadMovie`'s return type changed.

Mods compiled against 1.2.x hit these as `MissingMethodException`/`TypeLoadException` at load or first call.

### Cited Findings
- **War Sails release:** it launched on November 26, 2025 alongside Bannerlord patch 1.3.0 — [mp1st (War Sails update)][mp1st-ws]; [TaleWorlds "War Sails & Patch v1.3.4 Release"][tw-ws]
- **Versions on apidoc.bannerlord.com:** v1.4.8, v1.4.7, v1.3.14, v1.3.4, v1.2.12, v1.2.10, v1.2.9, v1.2.7, v1.1.0, v1.0.3, v1.0.0 — [apidoc index][apidoc]
- **Vec2 → CampaignVec2:**
  - 1.2.12: `Vec2 MobileParty.Position2D { get; set; }`, `MobilePartyAi.SetMoveGoToPoint(Vec2)`, `SetMoveGoToSettlement(Settlement)` and the other `SetMove*` methods with no navigation type — [apidoc 1.2.12 MobileParty][api1212-mp]; [apidoc 1.2.12 MobilePartyAi][api1212-ai]
  - 1.4.8: `CampaignVec2 MobileParty.Position { get; set; }` with no `Position2D`. `MobileParty.SetMoveGoToPoint(CampaignVec2, NavigationType)` and `SetMoveGoToSettlement(Settlement, NavigationType, bool isTargetingThePort)`, plus the `SetMoveEngageParty`/`SetMoveEscortParty`/`SetMovePatrolAroundPoint`/`SetMoveRaidSettlement`/`SetMoveBesiegeSettlement`/`SetMoveDefendSettlement` family, all on `MobileParty` with `NavigationType`.
  - 1.4.8: `Settlement.Position`, `GatePosition` and `PortPosition` are `CampaignVec2`. `Settlement.StartFindingLocatablesAroundPosition(Vec2, float)` still takes `Vec2`.
  - Source: [local: TaleWorlds.CampaignSystem.dll][cs]
- **BUTR's 1.3-line reference uses CampaignVec2:** for example `SetPartyAiAction.GetActionForPatrollingAroundPoint(MobileParty owner, CampaignVec2 position, NavigationType navigationType, Boolean isFromPort)` and `GetActionForMovingToNearestLand(MobileParty, Settlement)` — [BUTR GABS campaign-actions][gabs]
- **New naval state:** on `MobileParty`: `IsCurrentlyAtSea`, `IsInRaftState`, `Anchor`, `Ships`, `HasNavalNavigationCapability`, `HasLandNavigationCapability`, `NavigationCapability`, `DesiredAiNavigationType`, `IsTransitionInProgress`, `EndPositionForNavigationTransition`, `SetSailAtPosition`, `DisembarkToPosition`, `SetLandNavigationAccess(bool)`. Also `PartyNavigationModel.CanPlayerNavigateToPosition`/`HasNavalNavigationCapability`/`IsTerrainTypeValidForNavigationType`, and naval-speed estimates on `Campaign` such as `EstimatedAverageLordPartyNavalSpeed` — [local: TaleWorlds.CampaignSystem.dll][cs]
- **ImageIdentifier:** 1.2.12 `InquiryElement` took a `TaleWorlds.Core.ImageIdentifier` — [apidoc 1.2.12 InquiryElement][api1212-ie]. 1.4.8 has the abstract-style base `TaleWorlds.Core.ImageIdentifiers.ImageIdentifier` (parameterless constructor) plus `ItemImageIdentifier`, `CharacterImageIdentifier`, `BannerImageIdentifier`, `CraftingPieceImageIdentifier`, `EmptyImageIdentifier`, `PlayerAvatarImageIdentifier` — [local: TaleWorlds.Core.dll][core]
- **Gauntlet:** 1.2.12 had `GauntletLayer(int localOrder, string categoryId = "GauntletLayer", bool shouldClear = false)` and `IGauntletMovie LoadMovie(...)` — [apidoc 1.2.12 GauntletLayer][api1212-gl]. 1.4.8 has `GauntletLayer(string name, int localOrder, bool shouldClear = false)` and `GauntletMovieIdentifier LoadMovie(...)` — [local: TaleWorlds.Engine.GauntletUI.dll][eg]
- **Symptom in the wild:** Steam comments on a Bannerlord mod page report "Method not found" errors after game updates, and an older Nexus build that would not load its DLL on the current version. I saw this only as a search summary; the exact page and the versions involved are unverified — [Steam workshop comments (search summary)][steam-mnf]
- **Game version gate:** War Sails and Bannerlord versions must match, otherwise the game blocks launch or crashes (verifying files is the suggested fix) — [TaleWorlds patch notes WS v1.0.3 / BL v1.3.7 (search summary)][tw-137]

### Inferences
- **Porting checklist for 1.2-era cheat/teleport code:**
  - `Position2D = v` → `Position = new CampaignVec2(v, isOnLand)`.
  - `party.Ai.SetMoveModeHold()` → `party.SetMoveModeHold()`.
  - `Ai.SetMoveGoToPoint(v)` → `SetMoveGoToPoint(cv, NavigationType.Default)`.
  - `new ImageIdentifier(item)` → `new ItemImageIdentifier(item)`.
  - `new GauntletLayer(order)` → `new GauntletLayer("Name", order)`.
  - Keep the `GauntletMovieIdentifier` from `LoadMovie` for `ReleaseMovie`.
  - Handle `IsCurrentlyAtSea` in anything that moves parties.
- **Our CheatMenu:** it uses `InquiryElement(e, e.Label, null)` and a chained `PartySpeedModel`, so it is not affected by the ImageIdentifier change (it passes null). Its MultiSelectionInquiry constructor call (10 arguments) still matches, because the last two parameters are optional.

### Gaps
- I found no official TaleWorlds "modding API changelog" listing 1.3/1.4 breaking changes. The comparison above is from apidoc v1.2.12 against local v1.4.8 reflection, not from release notes.
- I did not diff 1.3.x against 1.4.x. Some changes, such as the `GauntletLayer` name parameter, may have landed in 1.3.0 or later in 1.4.0. apidoc has v1.3.14 and v1.3.4 pages I did not fetch.

[ver]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/Version.xml
[cs]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.CampaignSystem.dll
[core]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Core.dll
[lib]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Library.dll
[eng]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Engine.dll
[eg]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.Engine.GauntletUI.dll
[ss]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.ScreenSystem.dll
[gd]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.GauntletUI.Data.dll
[mb]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.MountAndBlade.dll
[obj]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.ObjectSystem.dll
[input]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.InputSystem.dll
[vmc]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.CampaignSystem.ViewModelCollection.dll
[sbv]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/SandBox/bin/Win64_Shipping_Client/SandBox.View.dll
[sb]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/SandBox/bin/Win64_Shipping_Client/SandBox.dll
[sbg]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/SandBox/bin/Win64_Shipping_Client/SandBox.GauntletUI.dll
[mbg]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/Native/bin/Win64_Shipping_Client/TaleWorlds.MountAndBlade.GauntletUI.dll
[naval]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/NavalDLC/bin/Win64_Shipping_Client/NavalDLC.dll
[fast]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/FastMode/SubModule.xml
[savesys]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/bin/Win64_Shipping_Client/TaleWorlds.SaveSystem.dll
[docs-save]: https://docs.bannerlordmodding.com/_csharp-api/savesystem/
[mods]: file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Mount%20%26%20Blade%20II%20Bannerlord/Modules/
[apidoc]: https://apidoc.bannerlord.com/
[api1212-gl]: https://apidoc.bannerlord.com/v/1.2.12/class_tale_worlds_1_1_engine_1_1_gauntlet_u_i_1_1_gauntlet_layer.html
[api1212-mp]: https://apidoc.bannerlord.com/v/1.2.12/class_tale_worlds_1_1_campaign_system_1_1_party_1_1_mobile_party.html
[api1212-ai]: https://apidoc.bannerlord.com/v/1.2.12/class_tale_worlds_1_1_campaign_system_1_1_party_1_1_mobile_party_ai.html
[api1212-ie]: https://apidoc.bannerlord.com/v/1.2.12/class_tale_worlds_1_1_core_1_1_inquiry_element.html
[docs-screen]: https://docs.bannerlordmodding.com/_gauntlet/screenbase
[docs-gauntlet-nocs]: https://docs.bannerlordmodding.com/_tutorials/modding-gauntlet-without-csharp
[mcm]: https://github.com/Aragas/Bannerlord.MBOptionScreen/releases
[harmony]: https://github.com/BUTR/Bannerlord.Harmony/releases
[gabs]: https://github.com/BUTR/Bannerlord.GABS/blob/master/docs/campaign-actions.md
[uiext]: https://github.com/shdwp/UIExtenderLib/wiki
[mp1st-ws]: https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion
[tw-ws]: https://www.taleworlds.com/en/News/587
[tw-137]: https://www.taleworlds.com/en/News/590
[steam-mnf]: https://steamcommunity.com/sharedfiles/filedetails/comments/2859265386
[steam-rename]: https://steamcommunity.com/app/261550/discussions/0/5696507684086792840
