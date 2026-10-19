# Bannerlord native features (v1.1 to v1.4, War Sails, v1.5 beta) and built-in cheat/console commands, as of 18 Oct 2026

Research method note (read first):
- The official TaleWorlds site (taleworlds.com/en/News/...) and forums sit behind a Cloudflare bot check, so they could not be fetched. The same official patch notes are published as Steam announcements. Those were pulled in full through the public Steam News API (`https://api.steampowered.com/ISteamNews/GetNewsForApp/v2/?appid=261550&count=300&maxlength=0&feeds=steam_community_announcements`). Each Steam citation below links to the announcement as `https://store.steampowered.com/news/app/261550/view/<id>`.
- The console-command list, hotkeys, cheat-menu entries and achievement logic were checked directly against the game installed on this PC. The install is Steam Bannerlord **v1.4.8** (`bin/Win64_Shipping_Client/Version.xml`) with NavalDLC (War Sails) **v1.2.8**. I loaded the game DLLs reflection-only and read the `[CommandLineArgumentFunction]` attributes, the IL string literals and the localization XML. These are cited as "local game files (v1.4.8)" with the file name. This is primary evidence for the current live build. It was not checked in-game.
- Versioning: since War Sails, TaleWorlds numbers releases as "WS vX / BL vY". BL v1.3.x shipped with WS v1.0–1.1, BL v1.4.x with WS v1.2.x, and BL v1.5.0 (beta) with WS v1.3.0. Do not confuse these with the 2020 Early Access "e1.x" patches. GameBanshee's "e1.2.0/e1.3.0/e1.4.2" articles are 2020 Early Access, not relevant.

## What did each major patch (1.1, 1.2, 1.3, 1.4, plus the 1.5 beta) add or change for the single-player campaign?

### Takeaway
After the October 2022 release, TaleWorlds added a large amount of what used to be mod territory:
- **v1.1:** party-screen sorting, retirement/heir end screen, alleys, raid rework.
- **v1.2:** weather, formation targeting, warehouses, kingdom destruction, a console cheat menu.
- **v1.3:** alliances, trade agreements and call-to-war; stealth and disguise; deserters and patrols; random events; parley; caravan control; notable supporters; Fast Mode; hero import/export cheats.
- **v1.4:** a diplomacy and army AI rework, two party roles per clan member, abstain on votes, hideout success chances, and sea raids/naval roles for War Sails.
- **v1.5 beta (Aug 2026):** Advanced Starting Options (start as king/vassal/merchant, scenarios, seeds, Fast Mode built in), trait effects, blood feuds, battle sites, and on/off toggles for clan parties.

### Cited Findings

**Timeline (live vs beta)**
- v1.0.0 "Official Release" posted 25 Oct 2022. Birth/aging options already existed by then: the notes fix "death-related functions … when disabling birth and aging options" — [Steam: Official Release](https://store.steampowered.com/news/app/261550/view/4714731755429514639)
- v1.1.0: beta 12 Jan 2023, live 1 Mar 2023. Hotfixes ran to v1.1.6 (18 Sep 2023) — [Steam: Patch Notes v1.1.0](https://store.steampowered.com/news/app/261550/view/5070528176557709918)
- v1.2.0: beta 23 Jun 2023. The full notes were too long for Steam and were posted only on the website/forum — [Steam: Beta Patch Notes v1.2.0](https://store.steampowered.com/news/app/261550/view/5124581515258212808); website copy (not fetchable here): [TaleWorlds News/535](https://www.taleworlds.com/en/News/535)
- v1.2.7 went live 14 Dec 2023 and "includes all prior Beta versions from v1.2.0 to v1.2.6". Later fix patches: 1.2.8 (21 Dec 2023), 1.2.9 (1 Feb 2024), 1.2.10 (27 Jun 2024), 1.2.12 (2 Dec 2024) — [Steam: v1.2.7](https://store.steampowered.com/news/app/261550/view/7585814657461328138); [Steam: v1.2.12](https://store.steampowered.com/news/app/261550/view/1784506359140994)
- v1.3.0: public beta 15 Sep 2025. v1.3.4 went live with War Sails on 26 Nov 2025. Hotfixes ran 1.3.5–1.3.14 (to 3 Feb 2026) — [Steam: Beta Patch Notes v1.3.0](https://store.steampowered.com/news/app/261550/view/1810503566552360); [Steam: War Sails & Patch v1.3.4 Release](https://store.steampowered.com/news/app/261550/view/1817483467032776)
- v1.4.0: beta 9 Apr 2026 (with WS v1.2.0). v1.4.5 went live on PC 21 May 2026, with consoles "planned for early June". Then 1.4.6 (11 Jun), 1.4.7 (8 Jul) and 1.4.8 (10 Aug 2026, alongside the War Sails Modding Kit) — [Steam: Dev Update & Beta WS v1.2.0 / BL v1.4.0](https://store.steampowered.com/news/app/261550/view/1829528821306653); [Steam: Patch Notes WS v1.2.5 / BL v1.4.5](https://store.steampowered.com/news/app/261550/view/1833334318569123); [Steam: WS Modding Kit & Patch WS v1.2.8 / BL v1.4.8](https://store.steampowered.com/news/app/261550/view/1840310314351373)
- v1.5.0 entered beta 19 Aug 2026 (with WS v1.3.0). No newer Steam announcement existed when the API was queried on 18 Oct 2026, so the **live branch is v1.4.8**, matching the local install — [Steam: Beta Patch Notes WS v1.3.0 / BL v1.5.0](https://store.steampowered.com/news/app/261550/view/1841579228662390); local game files (v1.4.8) `Version.xml`

**v1.1.0 (live 1 Mar 2023): key single-player features** — all from [Steam: Patch Notes v1.1.0](https://store.steampowered.com/news/app/261550/view/5070528176557709918)
- Party screen troop sorting:
  - Both rosters sort by Type, Name, Count and Tier.
  - The last sort is remembered and heroes are grouped at the top.
  - Adds a troop-type rundown (counts of infantry, archers, cavalry and horse archers).
- "Fog of War" for heroes: skills, traits, family and owned settlements stay hidden until you meet the hero (tooltips, portraits, encyclopedia).
- Retirement: a new settlement "The Retreat" near Lageta with an NPC "The Hermit". You can retire and continue as an heir, or end the run. A new end-of-campaign statistics window shows kill count, play time, troops recruited, etc.
- Perk overhaul: all perk descriptions rewritten, 15+ perks renamed, and the Trade tree reordered.
- Alley mechanics redesign:
  - Claim an alley by clearing it and assigning a clan member with suitable traits and Roguery.
  - Owned alleys give gold and bandit troops but generate crime rating.
  - Neighbouring gangs attack them.
- Village raid rework:
  - Loot is based on hearth damage and paid as denars, everyday goods and the village's main product.
  - Villages take longer to recover.
  - Completed raids no longer leave you disorganised.
- Other additions: troop selection for keep battles; "Sally out ambush" mission (destroy besieger siege engines); wanderer life-cycle change (at least one of each wanderer type always exists).
- Settlement prisoner limit raised from 30 to 60 (+30 per wall level).
- Cheat change: "Re-enabled the campaign.multiply_campaign_speed cheat but capped its maximum value to 15 and changed its name to set_campaign_speed_multiplier."
- New tutorials and save-size optimisation (save files now grow linearly).

**v1.2.x (beta Jun 2023, live v1.2.7 Dec 2023): headline features** — from TaleWorlds' video/text summary [Steam: Development Update #12](https://store.steampowered.com/news/app/261550/view/5124582150127345736)
- Weather:
  - Rain and snow on the world map and in missions.
  - Wet or snowy ground cuts cavalry and mounted-infantry party speed bonuses and doubles siege preparation time.
  - In battle, mounts are slower and active rain/snow slows arrows and bolts.
- Fog of war improvements: pay the tavern keeper for clan info. Marriage offers, companion inquiries and asking about bachelors also unlock encyclopedia info. New marriage offer screen.
- Battle changes:
  - Pushable siege ladders.
  - Melee, ranged and cavalry AI rework.
  - **Formation Targeting**: Charge/Engage orders can target a specific enemy formation.
  - Auto-battle calculations now use terrain and mission type, plus captain-perk bonuses.
- **Kingdom destruction**: a kingdom that loses all fiefs is destroyed. Its clans go independent and join other kingdoms or wither after about 4 weeks.
- **Warehouses**: supply workshops with raw materials and collect processed goods. Managed via the Clan screen.
- 6 new battle terrains (76 total) and 4 new hideout scenes.
- Consoles: "Birth and Aging" settings arrive on consoles in v1.2.0. "We've also added 15 different cheats. They can be enabled through the Campaign Options or by holding the Left Bumper, Right Trigger, and D-Pad Down. Please note that you will disable achievements by enabling cheats."
- Per a search-engine snippet of the official Beta v1.2.0 notes (the page itself could not be fetched): workshops can now only be bought directly from notables, warehouse management sits under Clan → Others, and income/expenses were regrouped into categories (e.g. Caravans) — [TaleWorlds News/535 (snippet only)](https://www.taleworlds.com/en/News/535)
- 1.2.8–1.2.12 were mostly crash and bug fixes. Example: 1.2.12 fixed "party roles … reset after the Rival Gang Leader quest battle" — [Steam: v1.2.12](https://store.steampowered.com/news/app/261550/view/1784506359140994)

**v1.3.0 beta (15 Sep 2025), shipped as live v1.3.4 (26 Nov 2025): the biggest base-game update** — [Steam: Beta Patch Notes v1.3.0](https://store.steampowered.com/news/app/261550/view/1810503566552360); highlights in [Steam: War Sails & Patch v1.3.4 Release](https://store.steampowered.com/news/app/261550/view/1817483467032776)
- **Alliances** (kingdom decision):
  - Driven by ruler relations, shared/separate wars, tributes, ruler traits and more.
  - Up to 2 alliances per kingdom, lasting 1 in-game year (84 days).
  - **Call to War** can include a gold payment. Players can call an ally to war through dialogue but pay the gold themselves. An accepted call binds for at least 42 days.
- **Trade Agreements**: raise caravan traffic between partners, up to 2 at a time, lasting 1 year, broken by war.
- **Reworked war, peace and tribute scoring**: a "war effort" score built from sieges, raids and casualties. Expect fewer, longer and more logical wars. White peace now takes war effort into account.
- **Stealth & Disguise**:
  - Stealth prison breaks and hideouts.
  - Enemy alert states (Neutral → Cautious → Searching → Fully Alarmed), sight/sound/light/elevation, distractions (Bag of Rocks), body dragging.
  - Stealth bonuses on gear and backstab attacks; a third "stealth" equipment layout.
  - Disguised infiltration of hostile towns via a contact.
  - Separate stealth difficulty option.
- **Hideouts**: sentry camps before the main camp. "Send troops" option, auto-cleared after 6 hours with loot but no XP. Day or night attacks. Troop requirement 25–40.
- **Deserters**: a new bandit faction spawned after lord battles. Deserter parties can merge into large bands.
- **Patrol Parties**: spawned by the new "Guard House" town project. They pay no wages, add security and hunt bandits around the town and its villages.
- **Random Events**: "up to 100 different events" offering choices that affect traits, skills, party, items or gold.
- **Parley**: hold Left-Alt and left-click an enemy settlement that has a noble inside, from a distance. Requires line of sight and clan tier ≥ 3.
- **Caravan control**: new dialogue with player caravans to change the home settlement and ban trade with specific kingdoms. Caravans also now do larger bulk trades.
- **Notable supporters**: recruit notables as supporters via dialogue (enough relation plus a one-time payment). Each gives daily influence and loyalty effects, and can withdraw support.
- **Reworked town and castle projects**: settlements now start with a military, economic or balanced focus.
- **Clan member equipment**: edit the equipment and inventory of clan members who lead parties or caravans, through dialogue.
- Economy changes:
  - Town trade commission ×5 (tariffs now about a third of town income).
  - Village taxes +30%, town and castle taxes +130%.
  - Faster village hearth recovery; grain and fish substitution.
  - New Felt and Planks goods.
  - Workshops in rebellious towns (loyalty < 25) stop producing.
- Simulated battles gain morale and retreat. "Removed the defender advantage factor from siege battles". Map speed of all parties −20%.
- Armies:
  - Disband below 40% average party size.
  - Need 12 days of food.
  - Army raiding disabled.
  - Clan members no longer generate influence for the army leader (exploit fix).
- Prisoners heal over time. Wanderers spawn younger and live longer. New Land Grants for Veterans policy. Slings added as a new weapon type.
- **Cheats**:
  - "Added Import/Export hero cheats": `campaign.export_main_hero`, `campaign.import_main_hero`, `campaign.export_hero`. Covered fields: "equipment, perks, skills, attributes, traits, unspent focus and attribute points, body(age, build, weight, race, face), name, culture, gold". Files sit in the Game Saves folder and can be edited by hand.
  - "Organized campaign player cheats: standardized names, removed unnecessary entries, and added precondition checks. Also added the 'campaign.add_building_level' and 'kill_hero' cheats."
  - Fixed "party wages to become a profit instead of a cost when using cheats".
- Fast Mode added as a launcher module (see the optional-modules question).
- Further QoL items:
  - "Added the ability to delete corrupted save files".
  - The message log is hidden rather than cleared.
  - "Transfer All" stops at the settlement's gold.
  - Grandchildren shown in the encyclopedia.
  - Party leaders shown in battle outfits.
  - Removed the main hero's parents in Sandbox mode.
- New modding support: FMOD audio, moddable atmospheres and campaign time, an XML Editor, and a new XML merging system replacing XSLT — [Steam: War Sails & Patch v1.3.4 Release](https://store.steampowered.com/news/app/261550/view/1817483467032776); [Steam: Announcing War Sails & Base Game Plans](https://store.steampowered.com/news/app/261550/view/1794102528312864)
- v1.3.12 beta, released live as 1.3.13 (Dec 2025):
  - Reintroduced the hideout battle as an "Assault" option during the day (8–15 troops), with "Sneak In" at night (minimum lowered to 20).
  - "Enabled the inventory screen search bar".
  - "Re-enabled Order of Battle save/load functionality while in an army".
  - "Restored upgrade and recruit shortcut buttons to the party screen pop-up menus".
  - Toggle between formation distance and scale displays.
  - Encyclopedia clan members sorted by age.
  - — [Steam: Patch Notes WS v1.1.1 / BL v1.3.13](https://store.steampowered.com/news/app/261550/view/1819386365106738)

**v1.4.0 beta (9 Apr 2026), live as v1.4.5 (21 May 2026) through v1.4.8 (Aug 2026)** — [Steam: Dev Update & Beta BL v1.4.0](https://store.steampowered.com/news/app/261550/view/1829528821306653); [Steam: BL v1.4.5](https://store.steampowered.com/news/app/261550/view/1833334318569123)
- **Party roles**: "each clan member [can] hold up to two party roles at the same time, reducing pressure on companion limits". This applies to the base game and War Sails.
- **Army AI rework**:
  - Minimum army size scales with kingdom strength.
  - Minimum influence to create an army rises from 50 to 100.
  - Armies no longer form for defence only and disband after 2 days if too weak to besiege.
  - v1.4.3: armies need ≥ 1000 strength and about 30% of a faction's parties stay outside armies.
  - "Your army will now be disbanded if you kick out all the army members."
- **Diplomacy**:
  - No wars between kingdoms without shared borders.
  - Alliance renewal decisions.
  - Alliances weigh marriages, occupation of another culture's fiefs and trade agreements.
  - Honor and relation penalties for attacking an ally or trade partner; relation penalty for refusing a call to war.
  - Trade agreements only between neighbours. Caravans and convoys now "generate gold for the owning kingdom on each settlement visit", paid daily to vassals and shown on the Diplomacy screen.
  - Initial lord relations are more varied, and the enemy threshold is now −50.
  - Vote support is shown as Low/Slight/Good/Strong.
  - **"Added an option to abstain when voting on kingdom decisions as the ruler"**.
  - Allied kingdoms' settlements and parties are highlighted blue.
- **Hideouts**:
  - Sending troops now has a success chance based on Tactics and Roguery.
  - Assault gives battle loot only; Stealth gives extra stash loot.
  - Roguery XP for reaching the campfire undetected.
  - Hideout icons show the number of bandit parties.
- **Stealth tweaks**: a countdown when spotted in prison breaks. Trading in hostile towns is allowed with the Smuggler Connection perk.
- **Map visibility**: depends on party size and terrain (forests hide parties).
- **UI**:
  - A new Campaign option for double-click time control (default / Play↔Fast-Forward toggle / disabled).
  - Cross icon to remove items from the town build queue.
  - Zoom in item inspection.
  - Warning when cancelling inventory or party changes.
  - "Hide Battle UI" now also hides the crosshair and name markers.
  - Keybinding swap and conflict display.
- **Battles**:
  - Reverted the 1.3.3 survival-chance nerf (more wounded, fewer dead).
  - Auto-resolved sieges take twice as long.
  - "Removed the default cavalry bonus from auto-resolve battles".
  - Two-handed maces can knock down.
- Players can no longer recruit from or trade with villages of factions they are at war with.
- **Save & Load**: "Added marks to the save games to show if achievements are enabled or disabled."
- **Modding**: a pop-up warning at launch about mod conflicts and which mod causes them. Expanded FMOD sound modding.
- v1.4.1: "Re-introduced the level progress bar on the Character screen. It now shows the current XP instead of skill points." v1.4.3: a new ruler automatically switches to ruler-tier equipment — [Steam: BL v1.4.5 (includes beta hotfix log)](https://store.steampowered.com/news/app/261550/view/1833334318569123)
- TaleWorlds says it is reviewing "party clan management, as we believe it still has significant potential to improve the quality of life", plus more visible trait effects — [Steam: Dev Update BL v1.4.0](https://store.steampowered.com/news/app/261550/view/1829528821306653)

**v1.5.0 beta (19 Aug 2026; NOT yet live as of 18 Oct 2026)** — [Steam: Beta Patch Notes WS v1.3.0 / BL v1.5.0](https://store.steampowered.com/news/app/261550/view/1841579228662390)
- **Advanced Starting Options (ASO)**, set at Sandbox start and not changeable later:
  - **Campaign Scenarios**: Default, United Empire, Invasion, Last Stand, 2 Faction War, Randomized Calradia, [WS] Nord Invasion.
  - **Player Starts**: Default, King, Vassal, Mercenary, Trader (workshop plus caravan), Outlaw, Beggar, [WS] Fleet Admiral, [WS] Merchant Venturer. Some must be unlocked by reaching that rank in normal play.
  - **Modifiers**: Bandit Surge, Civil Unrest, Recruit Shortage, Swift Travel, [WS] Personal Vessel.
  - A shareable **Seed system**.
- "Fast Mode has been moved to ASO, is no longer a separate Module, and is intended to become available on Consoles through this as well."
- **Battle Sites & Wreckages**: post-battle map sites you can investigate for wounded recruitable survivors (scaled by Medicine), denars, trade goods and hideout maps.
- **Personality trait effects**: each trait level gives about two bonuses and one malus. Examples: Calculating +10/20% workshop income; Generous +25/50% mercenaries; Cruel +20/40% raid loot. The encyclopedia shows trait tooltips.
- **Blood Feuds**: executions trigger retaliatory executions (AI clans can execute the player at relation ≤ −50). Settled via Ransom Brokers or when relations recover to 0.
- **Clan Party Controls**: "Replaced party stances with behavior toggles" covering raiding villages, donating troops to garrisons, joining other armies and [WS] owning a fleet.
- **Clan party recovery system**: defeated clan parties respawn with the same configuration.
- "Removed the finance screen from the Clan tab".
- Passive XP: "Nobles, player companions, and wanderers now gain XP daily based on their focus points"; Leadership XP from high morale.
- Land and sea speed +15%.
- "Removed the loyalty penalty for assigning a governor whose culture differs from the settlement's culture."
- New popup with hero details when choosing party roles. Clan and Kingdom screens reopen the last-used tab. Max party size is shown when picking a party leader.
- War Sails: mooring ships at ports, ferries between coastal villages, castle ports and blockades, 39 naval random events, smaller pirate parties.
- New "Modding Animation Resources" module.

### Inferences
- Features the base game now covers, which a new mod should not re-implement (mod names are from general community knowledge; a Nexus snippet confirms only the Diplomacy mod's feature list):
  - Diplomacy-style alliances, call-to-war and trade pacts (Diplomacy mod: [Nexus search result](https://nexusmods.com/mountandblade2bannerlord/mods/832)).
  - Party-screen sorting (Party Screen Enhancements–style).
  - Hero import/export (Character Reload–style).
  - Retirement and heir succession.
  - Ruler-tier gear on succession.
  - Caravan routing control.
  - Garrison patrols and deserter bands.
  - Birth/aging toggle and fast aging (Fast Mode / ASO).
  - Two roles per clan member.
  - Start-as-king/vassal options (v1.5 ASO).
  - Clan party restrictions (no raiding / no joining armies, v1.5).
  - Hideout "send troops".
  - Abstain voting.
- Native features that still look weak or missing, so they remain mod opportunities:
  - Non-aggression pacts, granting fiefs freely, war exhaustion UI, civil wars, messengers. The Diplomacy mod listed these, and only alliances, call-to-war, trade agreements and parley are native.
  - Renaming settlements, clans or heroes through the UI (only via hero export/import files).
  - Bulk relation editing.
  - Party clan management QoL, which TaleWorlds itself flags as needing work.
  - A finance overview, since v1.5 beta removes the Clan-tab finance screen.
- Several 1.5-beta features (ASO, trait effects, behaviour toggles) may change before live. A mod targeting v1.4.8 should avoid overlapping them, or be ready to retire them when 1.5 goes live.

### Gaps
- The full v1.2.0–v1.2.7 changelog (website/forum only) could not be fetched because of the Cloudflare check. Only the dev-update summary and a search snippet are available, so smaller 1.2.x QoL items (e.g. exact clan/party screen changes) are unconfirmed.
- No release date or live notes yet for v1.5.0. The Fast Mode → ASO move and the finance-screen removal are beta-only for now.
- No source states exactly when each 2020-era cheat command was removed (see the console question); only the v1.3.0 "organized … removed unnecessary entries" line is official.

## What does the War Sails DLC add, and which of its systems are commonly criticized?

### Takeaway
War Sails (released 26 Nov 2025, single-player only) adds:
- a Nord faction and northern region;
- real-time ship-to-ship combat with wind physics, ramming, boarding and fire;
- 20 ships (later 22) with upgrades, shipyards, fleets and naval blockades;
- three naval skills, mariner troops and a pirate questline.

Critics found the naval battles repetitive and the systems weakly integrated with the land campaign. TaleWorlds itself acknowledged three problems: lords rarely sailed, naval content was gated behind progression, and battles dragged. Patches 1.2.x and 1.3 address these with sea raids, naval patrols, delegate-command, mooring, ferries and new victory conditions.

### Cited Findings
- Launch content:
  - Nords/Northern Kingdom faction; Calradia extended north with a settled island, coastal routes and open sea.
  - Storms and currents; naval trade and piracy.
  - Sieges of coastal settlements need a fleet blockade.
  - "20 ships: 15 warships, 4 trade ships, and a civilian fishing vessel" with upgrades (siege engines, cargo holds, sails, rams). Capture ships and figureheads; shipyards stock ships by town culture and development.
  - Custom naval battles (River/Coastal/Open Sea).
  - Three nautical skills, mariner troop branches, seafaring companions, a questline against a pirate confederacy.
  - — [Steam: War Sails & Patch v1.3.4 Release](https://store.steampowered.com/news/app/261550/view/1817483467032776)
- Originally announced for 17 June 2025, then delayed to "early Fall 2025". TaleWorlds also cancelled a story DLC and moved its mechanics (stealth etc.) into the base game — [Steam: Announcing War Sails & Base Game Plans](https://store.steampowered.com/news/app/261550/view/1794102528312864)
- WS v1.1.0 (Dec 2025):
  - Faster sail rotation; improved boarding bridges; crew auto-handles hooks.
  - −60% missile damage to the player while steering.
  - Shipmaster XP ×0.6 → ×1.4.
  - Undermanned speed penalty −60%.
  - New Ships encyclopedia page.
  - — [Steam: Beta Patch Notes WS v1.1.0 / BL v1.3.12](https://store.steampowered.com/news/app/261550/view/1818752592127996)
- WS v1.1.1 added an option to ransom the player's sister (10,000 denars) to end the War Sails questline early, and shortened storyline quests by removing encounters. This suggests feedback that the questline was too long — [Steam: Patch Notes WS v1.1.1 / BL v1.3.13](https://store.steampowered.com/news/app/261550/view/1819386365106738)
- TaleWorlds' own diagnosis (Apr 2026):
  - "it is currently too difficult to consistently locate the fleets of nobles".
  - "our lords are not setting sail frequently enough, and when they do, it is often short-lived".
  - Mission work aims at "resolving issues that unnecessarily prolong the engagement".
  - — [Steam: Dev Update BL v1.4.0](https://store.steampowered.com/news/app/261550/view/1829528821306653)
- WS v1.2.x fixes:
  - Offensive naval patrols by lords; new First Mate (Boatswain) and Navigator (Shipmaster) party roles.
  - +50% troop XP in naval encounters; lower wages and food at sea; +50% river speed.
  - "Delegate command" of your ship.
  - Victory once all of one side's ships are out of action.
  - Partial-engagement disengage logic.
  - Two new ships: Battanian Barlinnger and Nord Battle Knarr.
  - — [Steam: Dev Update BL v1.4.0](https://store.steampowered.com/news/app/261550/view/1829528821306653)
- WS v1.2.1+ added **Seaborne Village Raids**:
  - Raid coastal or river villages from ships; requires 15 healthy troops and shallow-draft ships.
  - 16 new amphibious raid scenes.
  - Make port in villages to trade, recruit and resupply.
  - — [Steam: BL v1.4.5](https://store.steampowered.com/news/app/261550/view/1833334318569123)
- WS v1.3.0 beta (Aug 2026) says naval play "requires substantial progress … This … gates much of the naval content". It adds a ship start option, ferries, mooring, castle ports, smaller pirates and naval random events — [Steam: BL v1.5.0 beta](https://store.steampowered.com/news/app/261550/view/1841579228662390)
- Critic review (TheSixthAxis, 13 Jan 2026, score 5):
  - Naval battles "all tend to follow the same tedious pattern".
  - Naval warfare "lacks any kind of pleasing tactical variety".
  - The story's exposition "tends to drag on".
  - Maritime skill requirements are "ambiguous and time consuming".
  - Not a must-buy.
  - — [TheSixthAxis review](https://www.thesixthaxis.com/2026/01/13/mount-blade-2-bannerlord-war-sails-review/)
- A search summary of other reviews says naval combat is "almost entirely pointless in the wider game, since land armies are cheaper", and that the reviewer noted "many of the islands are now gone" on the reworked map — [JumpDashRoll review](https://jumpdashroll.com/article/mount-and-blade-ii-bannerlord---war-sails-review) / [NoobFeed review](https://noobfeed.com/reviews/mount-and-blade-ii-bannerlord-war-sails-review) (snippet-level, not fully read)
- Steam user reviews for the War Sails app (2927200): English-language summary "Mixed", 1,487 positive vs 1,259 negative (2,746 total), queried 18 Oct 2026 — [Steam appreviews API](https://store.steampowered.com/appreviews/2927200?json=1&language=english&filter=recent&num_per_page=1&purchase_type=all)
- The War Sails Modding Kit was released 10 Aug 2026. It supports naval world maps, flowmaps, new ships, Nord assets and raid scenes — [Steam: WS Modding Kit & BL v1.4.8](https://store.steampowered.com/news/app/261550/view/1840310314351373)

### Inferences
- The most-criticised areas are:
  - naval battle repetition and length;
  - weak campaign payoff for fleets;
  - progression gating;
  - lords rarely at sea;
  - questline length.
- Fleet management QoL (moored-fleet overview, convoy management) and naval encounter variety still look like open mod opportunities, although v1.5 mooring partly covers fleet storage.

### Gaps
- No aggregated professional score (Metacritic/OpenCritic) was retrieved.
- The "islands removed" map criticism comes from a search snippet only.

## What official optional modules ship with the game now (BirthAndDeath, FastMode, NavalDLC), and what do they do?

### Takeaway
On v1.4.8 the `Modules` folder contains Official modules (Native, SandBoxCore, Sandbox, StoryMode, CustomBattle, Multiplayer) and three "OfficialOptional" modules:
- **BirthAndDeath**: "Birth and Aging Options". Adds the campaign option "Enable Birth and Aging".
- **FastMode**: "Fast Mode". Adds "Fast Mode Is Enabled" for accelerated time, aging and progression.
- **NavalDLC**: War Sails v1.2.8.

The v1.5 beta folds Fast Mode into Advanced Starting Options and removes it as a module.

### Cited Findings
- Local install (v1.4.8) `Modules/`: BirthAndDeath, CustomBattle, FastMode, Multiplayer, Native, NavalDLC, SandBox, SandBoxCore, StoryMode, plus a non-official "CheatMenu" (ModuleType "Community", v1.0.0), which is the user's own mod build — local game files (v1.4.8) `Modules/*/SubModule.xml`
- **BirthAndDeath**:
  - SubModule `<Name value="Birth and Aging Options"/>`, `<Id value="BirthAndDeath"/>`, `<ModuleType value="OfficialOptional"/>`; DLL `TaleWorlds.CampaignSystem.ViewModelCollection.BirthAndDeath.dll` with `BirthAndDeathOptionsProvider` and option id `IsLifeDeathCycleEnabled`.
  - UI text: "Enable Birth and Aging" — "Choose if the heroes are able to age and reproduce."
  - — local game files (v1.4.8) `Modules/BirthAndDeath/SubModule.xml`, `Modules/SandBox/ModuleData/module_strings.xml`
- Birth/aging options already existed at v1.0.0 release (Oct 2022) and reached consoles in v1.2.0 — [Steam: Official Release](https://store.steampowered.com/news/app/261550/view/4714731755429514639); [Steam: Development Update #12](https://store.steampowered.com/news/app/261550/view/5124582150127345736)
- **FastMode**:
  - SubModule "Fast Mode", Id FastMode, OfficialOptional; DLL `TaleWorlds.CampaignSystem.FastMode.dll` with `FastModeOptionsProvider` and option `IsFastModeEnabled` ("Fast Mode Is Enabled" — "Accelerated gameplay.").
  - It also references `DefaultCharacterDevelopmentModel`, consistent with tuned skill progression.
  - — local game files (v1.4.8) `Modules/FastMode/`
- Official description: "Fast Mode is a new optional module that can be selected from the launcher on PC. It speeds up the passage of time: years are shorter, characters age faster, while skill progression and population turnover are tuned to match, making multi-generation dynasties and succession easier to experience in a single playthrough." — [Steam: War Sails & Patch v1.3.4 Release](https://store.steampowered.com/news/app/261550/view/1817483467032776)
- In v1.5.0 beta: "Fast Mode has been moved to ASO, is no longer a separate Module" — [Steam: BL v1.5.0 beta](https://store.steampowered.com/news/app/261550/view/1841579228662390)
- **NavalDLC**: Name/Id NavalDLC, Version v1.2.8, `RequiredBaseVersion v1.4.8`, `DefaultModule true`, OfficialOptional. Depends on Native, SandBoxCore, Sandbox and StoryMode — local game files (v1.4.8) `Modules/NavalDLC/SubModule.xml`
- Other merged or optional modules: the "Dedicated Custom Server Helper module has been merged into the base game" (v1.2.0) — [Steam: Development Update #12](https://store.steampowered.com/news/app/261550/view/5124582150127345736). v1.5 beta adds a "Modding Animation Resources" module — [Steam: BL v1.5.0 beta](https://store.steampowered.com/news/app/261550/view/1841579228662390)

### Inferences
- Both optional modules work only through campaign-option providers (`ICampaignOptionProvider`). A mod can follow the same pattern to add its own toggles to the native Campaign Options screen instead of building a separate settings UI.
- Avoid shipping "disable aging/birth" or "faster time" toggles; native already covers them. `campaign.set_campaign_speed_multiplier` (max 15) covers map-speed changes.

### Gaps
- I could not confirm whether disabling BirthAndDeath stops only births and aging or also battle deaths. The option text says only "age and reproduce".

## Exactly which developer console commands exist for the single-player campaign in v1.3+/v1.4 (current v1.4.8)?

### Takeaway
I extracted 155 console functions from the v1.4.8 DLLs. About 80 are `campaign.*` cheats in `TaleWorlds.CampaignSystem.CampaignCheats`, and there are more `campaign.*` view cheats in SandBox.View, `naval.*` (War Sails), `mission.*` and `storymode.*`. All campaign cheats call `CheckCheatUsage`, which fails with "Cheat mode is disabled!" unless the game is in cheat mode.

There is **no console command to teleport the main party**; use the Ctrl+Left-click map hotkey instead. There is no rename command; use export/import of the hero file. Relations, troops, items, settlement and workshop ownership, kingdom membership, war and peace, trade agreements, marriage, pregnancy, skills, traits, culture and settlement stats are all covered. Many 2020-era commands (e.g. `campaign.teleport_hero_to_settlement`, `campaign.give_troops`, `campaign.destroy_kindgom`, `campaign.start_world_war`) are gone.

### Cited Findings
How they were obtained: reflection-only load of v1.4.8 DLLs, enumerating `[CommandLineFunctionality.CommandLineArgumentFunction(name, group)]` attributes, plus the "Format is …" help strings embedded in the DLLs. Syntax: `group.name arg1 | arg2`; `help` as the argument lists options on several commands — local game files (v1.4.8) `TaleWorlds.CampaignSystem.dll`, `SandBox.View.dll`, `NavalDLC.dll`, `NavalDLC.View.dll`, `StoryMode.dll`, `TaleWorlds.MountAndBlade.dll`

**`campaign.*` in TaleWorlds.CampaignSystem.dll (CampaignCheats)**, with the in-DLL usage string. All are from local game files (v1.4.8) `TaleWorlds.CampaignSystem.dll`.

Gold, influence, renown, morale:
- `add_gold_to_hero [HeroName] | [PositiveNumber]` — default 1000; no hero means the player; gold capped at 0–100,000,000.
- `add_influence [Number]` — default 100.
- `add_renown_to_clan [ClanName] | [PositiveNumber]` — help text says "campaign.add_renown".
- `add_morale_to_party [HeroName] | [Number]`
- `boost_cohesion_of_army [ArmyLeaderName]`

Hero stats and skills:
- `add_skill_xp_to_hero [HeroName] | [SkillName] | [PositiveNumber]`
- `add_attribute_points_to_hero [HeroName] | [PositiveNumber]`
- `add_focus_points_to_hero [HeroName] | [PositiveNumber]`
- `set_skill_player [SkillName] | [LevelValue]`
- `set_skills_of_hero [HeroName] | [Level]`
- `set_all_heroes_skills [LevelValue]`
- `set_all_companion_skills [LevelValue]`
- `set_skill_of_all_companions [SkillName] | [LevelValue]`

Traits, culture, age, crafting:
- `set_player_trait [Trait] | [Number]`
- `set_hero_trait [HeroName] | [Trait] | [Value]`
- `print_player_traits`
- `set_hero_culture [HeroName] | [CultureName]`
- `set_clan_culture [ClanName] | [CultureName]`
- `add_player_age [PositiveNumber]`
- `set_hero_crafting_stamina [HeroName] | [Stamina]`
- `add_crafting_materials`
- `unlock_all_crafting_pieces`

Relations:
- `add_hero_relation [HeroName]/All | [OtherHeroName(optional)] | [Value]`
- `show_relation [HeroName] | [HeroName]`

Party, troops, items:
- `add_troops [TroopId] | [Number] | [PartyName]` — party name optional; `campaign.add_troops help` lists troops.
- `add_prisoner_to_party [PrisonerName] | [CapturerName]`
- `add_xp_to_player_party_troops [Amount]`
- `add_xp_to_player_party_prisoners [Amount]`
- `heal_player_party`
- `kick_companion [CompanionName]|all|(none)`
- `add_item_to_player_party [ItemId] | [ModifierId] | [Amount]`

Settlements and workshops:
- `give_settlement_to_player [SettlementName/SettlementId]` — "Calradia" gives all settlements; "help" lists them.
- `give_settlement_to_kingdom [SettlementName] | [KingdomName]`
- `give_workshop_to_player [SettlementName] | [workshop_name]`
- `add_building_level [SettlementName] | [Building]`
- `set_prosperity_of_settlement` / `set_security_of_settlement` / `set_food_of_settlement` / `set_hearth_of_settlement` / `set_militia_of_settlement` — all take `[SettlementName/SettlementID] | [Value]`.
- `set_loyalty_of_settlement [SettlementName] | [loyalty]`
- `clear_settlement_defense [SettlementName]`
- `remove_militias_from_settlement [SettlementName]`
- `add_power_to_notable [HeroName] | [Number]`
- `add_supporters_for_main_hero [Number]`

Kingdoms, clans, diplomacy:
- `join_kingdom [KingdomName/FirstTwoChars]` and `join_kingdom_as_mercenary [...]` — `help` lists kingdoms.
- `create_player_kingdom`
- `create_clan [KingdomName]`
- `lead_kingdom`
- `rule_your_faction`
- `activate_all_policies_for_player_kingdom`
- `declare_war [Faction1] | [Faction2]`
- `declare_peace [Faction1] | [Faction2]`
- `make_trade_agreement [Kingdom1] | [Kingdom2]`
- `rebellion_enabled [1/0]`

Marriage and children:
- `marry_player_with_hero [HeroName]`
- `marry_hero_to_hero [HeroName] | [HeroName]`
- `is_hero_suitable_for_marriage_with_player [HeroName]`
- `print_heroes_suitable_for_marriage`
- `conceive_child` — "You need to be married to have a child, use campaign.marry_player_with_hero …".

Map visibility and safety:
- `set_parties_visible [1/0]`
- `show_hideouts [1/2]` / `hide_hideouts`
- `show_settlements` / `hide_settlements`
- `toggle_information_restrictions` — fog-of-war info.
- `set_main_party_attackable [1/0]`

Time:
- `set_campaign_speed_multiplier [positive multiplier]` — renamed from `multiply_campaign_speed` in v1.1.0, max 15.

Quests:
- `cancel_quest [quest name]`
- `print_all_issues`

Hero import/export (v1.3.0):
- `export_main_hero [file]`
- `import_main_hero [file]`
- `export_hero [file] | [hero]`

Diagnostics:
- `print_player_party_position`
- `print_criminal_ratings`
- `print_strength_of_factions`
- `print_strength_of_lord_parties`
- `print_tournaments`
- `print_gameplay_statistics` — help text says "statistics.print_gameplay_statistics".

**`campaign.*` in SandBox.View.dll (SandBoxViewCheats)** — local game files (v1.4.8) `SandBox.View.dll`
- `kill_hero [HeroName]` — added in v1.3.0.
- `make_clan_mercenary_of_kingdom [clan] | [kingdom] | [days]`
- Camera focus commands: `focus_hero [HeroName]`, `focus_mobile_party [PartyName]`, `focus_hostile_army`, `focus_infested_hideout [Optional: troops]`, `focus_issue [IssueName]`, `focus_tournament`.

**`naval.*` (War Sails)** — local game files (v1.4.8) `NavalDLC.dll`, `NavalDLC.View.dll`
- `add_ship_to_player [ShipName] | [Count]` — empty adds 1 random ship.
- `damage_player_ships`
- `list_all_ship_names`
- `list_all_figurehead_names`
- `unlock_figurehead [figurehead_id or all]`
- `focus_ship [ShipHullId/Name]`
- `focus_player_anchor`
- Developer test-mission commands: `start_mission`, `set_mission_scene`, `set_mission_ships`, `set_mission_troop_counts`, `set_maximize_troop_counts`, `get_mission_settings`, `reset_mission_settings`.
- Also `campaign.show_drop_off` (fishing).

**`storymode.*`**: `add_family_members` — local game files (v1.4.8) `StoryMode.dll`

**`mission.*` (battle)** — local game files (v1.4.8) `TaleWorlds.MountAndBlade.dll`, `SandBox.View.dll`
- `flee_enemies`
- `flee_team [Attacker|AttackerAlly|Defender|DefenderAlly] [FormationNo]`
- `kill_all_allies`
- `kill_n_allies [count]`
- `killAgent`
- `toggleDisableDying`, `toggleDisableDyingTeam`
- `set_battering_ram_speed`, `set_siege_tower_speed`
- `set_camera_position`, `fix_camera_toggle`, `set_shift_camera_speed`
- `kill_all_sentries` — hideout ambush only. The HideoutAmbush mission was disabled in v1.4.3 because of a crash ([Steam: BL v1.4.5](https://store.steampowered.com/news/app/261550/view/1833334318569123)).
- Other groups: `console.clear`, `chatlog.clear`, `ui.toggle_ui`, `localization.change_language`, `benchmark.cpu_benchmark`, etc.

**Removed or renamed versus 2020 guides.** The 2020 gameplay.tips list (dated 5 Aug 2020) includes many commands that are **absent in v1.4.8**:
- campaign.teleport_hero_to_settlement, campaign.give_troops (now `add_troops`), campaign.give_item_to_main_party (now `add_item_to_player_party`), campaign.change_hero_relation (now `add_hero_relation`).
- campaign.destroy_kindgom, campaign.destroy_clan, campaign.start_world_war, campaign.make_peace_with_every_faction, campaign.leave_faction, campaign.change_clan_leader, campaign.add_companion, campaign.add_caravan_to_hero, campaign.add_gold_to_all_heroes, campaign.set_criminal_rating, campaign.set_hero_age (now only `add_player_age`).
- campaign.list_active_quests and campaign.complete_active_quest (only `cancel_quest` remains), campaign.heal_main_party (now `heal_player_party`), campaign.multiply_campaign_speed.
- storymode.skip_tutorial_quests and similar.
- — [gameplay.tips (2020)](https://gameplay.tips/guides/7215-bannerlord-cheats.html) compared against local game files (v1.4.8)
- Official confirmation of the cleanup: v1.3.0 "Organized campaign player cheats: standardized names, removed unnecessary entries, and added precondition checks" — [Steam: Beta Patch Notes v1.3.0](https://store.steampowered.com/news/app/261550/view/1810503566552360)
- Other 2020-era guides still circulating (e.g. GamerEmpire, PC Gamer) list old names such as `campaign.destroy_kindgom` and `campaign.start_world_war` without version dates. Treat them as outdated — [GamerEmpire](https://gamerempire.net/mount-blade-2-bannerlord-all-cheats-console-commands-list/); [PC Gamer](https://www.pcgamer.com/mount-blade-2-bannerlord-cheats/)

### Inferences
- Good built-ins to wrap in a nicer UI, since they are one-liners against the native API and already guarded by native preconditions:
  - gold, influence, renown;
  - skill/attribute/focus edits;
  - `add_hero_relation` (supports "All");
  - `add_troops` / `add_item_to_player_party` with ItemId/ModifierId pickers;
  - `give_settlement_to_player` / `give_settlement_to_kingdom`;
  - `give_workshop_to_player`;
  - `join_kingdom` / `create_player_kingdom`;
  - `declare_war` / `declare_peace` / `make_trade_agreement`;
  - marriage and `conceive_child`;
  - `set_*_of_settlement` stats;
  - `set_hero_trait` / `set_hero_culture`;
  - `export_/import_main_hero` (rename and appearance editing through the file);
  - `add_ship_to_player` / `unlock_figurehead` for War Sails.
- Calling the underlying static methods directly (e.g. `CampaignCheats.AddGoldToHero(List<string>)`) would inherit the "Cheat mode is disabled!" gate. A mod would need to call the underlying actions instead (`GiveGoldAction`, `ChangeOwnerOfSettlementAction`, etc.), or accept that cheat mode, and the flag that disables achievements, must be on.
- Gaps in native that a mod could fill:
  - main-party teleport to a named settlement (only the Ctrl+click map hotkey exists);
  - renaming clans, settlements or kingdoms in the UI;
  - setting another hero's age;
  - destroying a kingdom or clan;
  - non-aggression pacts and alliance cheats (there is no `make_alliance` command; only trade agreements have a cheat);
  - completing quests (only cancel exists);
  - crime rating;
  - changing clan leader.

### Gaps
- The `config.*` and other engine-native console commands live in C++ (TaleWorlds.Native.dll) and could not be enumerated. One guide claims `config.cheat_mode 1` can be typed in the console; this is unverified. TaleWorlds.Native.dll does contain the string "cheat_mode" — local game files (v1.4.8)
- The argument formats above come from in-DLL help strings and were not tested in-game. Several older commands might be implemented under different names in other DLLs that I filtered out (Multiplayer DLLs were excluded on purpose).

## How does the built-in cheat mode work in the current version (config, Enable Cheats / Cheat Menu, hotkeys, console key, achievements)?

### Takeaway
There are three layers:
1. **Engine cheat mode** (PC): set `cheat_mode = 1` in `Documents\Mount and Blade II Bannerlord\Configs\engine_config.txt`. This makes `Game.Current.CheatMode` true, which unlocks:
   - the `campaign.*` console cheats (developer console, widely reported as Alt + ~);
   - map hotkeys: **Ctrl + Left-click** teleports the main party (and its army) to the cursor; **Ctrl + Alt + T** toggles "true sight";
   - battle hotkeys (Ctrl+F4 knock out target enemy, Ctrl+Alt+F4 all enemies, Ctrl+H heal, etc.);
   - "(Cheat)" menu shortcuts.
2. **Gameplay Cheat Menu** (built for consoles in v1.2): about 15 one-click cheats. Opened from Campaign Options → "Open Cheat Menu", which only appears when a gamepad is active, or by holding LB + RT + D-Pad Down on the map.
3. **Achievements**: the campaign is permanently flagged as soon as cheat mode is seen. "Achievements are disabled due to cheat usage." Unofficial modules or a version downgrade also disable them, and saves show this mark since v1.4.

### Cited Findings
**Enabling cheat mode**
- The PC config line exists as `cheat_mode = 0` in this user's `Documents/Mount and Blade II Bannerlord/Configs/engine_config.txt` (line 14) — local config file
- Guides agree: open Documents/Mount and Blade II Bannerlord/Configs, then "engine_config.txt", and change "cheat_mode = 0" to 1 — [gameplay.tips](https://gameplay.tips/guides/7215-bannerlord-cheats.html); GamerEmpire per search summary — [GamerEmpire](https://gamerempire.net/mount-blade-2-bannerlord-how-to-enable-cheats/)
- Console key: PC Gamer says Alt + ~ (tilde) opens the dev console; another guide says just "~". Sources conflict and this was not verified in code (native layer) — [PC Gamer via search](https://pcgamer.com/mount-blade-2-bannerlord-cheats)
- Console cheats are gated by `CampaignCheats.CheckCheatUsage`. It returns "Campaign was not started." outside a campaign and "Cheat mode is disabled!" when `Game.Current.CheatMode` is false. Mission cheats print "Cheat mode is not enabled." — local game files (v1.4.8) `TaleWorlds.CampaignSystem.dll`, `TaleWorlds.MountAndBlade.dll` (IL)

**Map hotkeys (v1.4.8, verified from IL)**
- **Teleport the main party**: in `MapScreen.HandleLeftMouseButtonClick`, if `Input.IsControlDown` and `Game.Current.CheatMode`, the main party's position is set to the clicked point. Attached army parties move too, and the sea/land state is switched via `ChangeIsCurrentlyAtSeaCheat` (War Sails). So the binding is **Ctrl + Left-click**, not Ctrl+Alt — local game files (v1.4.8) `SandBox.View.dll`. Matches the 2020 guide: "Ctrl + Left Click: Teleports you on the map" — [gameplay.tips](https://gameplay.tips/guides/7215-bannerlord-cheats.html)
- **True sight (reveal all parties)**: hotkey `MapScreenHotkeySwitchCampaignTrueSight` = T with Ctrl+Alt (modifier flags Ctrl=4, Alt=2, Shift=1), only when `Game.Current.CheatMode` — local game files (v1.4.8) `TaleWorlds.Engine.dll` (DebugHotKeyCategory), `SandBox.View.dll` (MapScreen.Tick)

**Battle and mission hotkeys**: registered in `TaleWorlds.Engine.CheatsHotKeyCategory`, as key + modifiers (+ excluded modifiers). Decoded from local game files (v1.4.8) `TaleWorlds.Engine.dll`:
- Kill/knock-out enemy agent: Ctrl+F4. All enemy agents: Ctrl+Alt+F4. Enemy horse: Ctrl+Shift+F4. All enemy horses: Ctrl+Alt+Shift+F4.
- Friendly agent: Ctrl+F2. All friendly agents: Ctrl+Alt+F2. Friendly horse: Ctrl+Shift+F2. All friendly horses: Ctrl+Alt+Shift+F2.
- Kill yourself: Ctrl+F3. Kill your horse: Ctrl+Shift+F3.
- Heal yourself: Ctrl+H (not with Shift). Heal your horse: Ctrl+Shift+H.
- Ghost cam: Ctrl+K. Teleport main agent: Alt+X.
- Pause: Ctrl+F11.
- Slow motion: Ctrl+F9; slow-motion factor: Shift+Numpad+/−.
- Switch agent to AI: Ctrl+F5. Control followed agent: Ctrl+Alt+Numpad5.
- Camera speed: Ctrl+Up / Ctrl+Down; reset: Ctrl+Middle-mouse.
- The 2020 guide matches the core set: Ctrl+F4 knocks out an enemy, Ctrl+Alt+F4 the entire enemy army, Ctrl+H heal, Ctrl+Shift+H heal horse, Ctrl+F2 one of your soldiers, Ctrl+F3 the player, Ctrl+Shift+F3 your horse — [gameplay.tips](https://gameplay.tips/guides/7215-bannerlord-cheats.html)

**Screen hotkeys from 2020 guides**
- The 2020 guides list Ctrl+X in inventory (+1000 gold), Ctrl+X in clan (+1000 influence), Ctrl+A / Ctrl+1–6 in the character screen, Ctrl+L level-up, and Ctrl+H in the party screen (add troops) — [gameplay.tips](https://gameplay.tips/guides/7215-bannerlord-cheats.html)
- In v1.4.8, no cheat-mode-gated handler was found in SandBox.GauntletUI.dll or SandBox.View.dll. In TaleWorlds.CampaignSystem.ViewModelCollection.dll, the only CheatMode check is in `SPInventoryVM.HandleDone`. These screen hotkeys are therefore likely removed or relocated (unverified in-game) — local game files (v1.4.8)

**"(Cheat)" menu and dialogue options**: game text in v1.4.8 includes "Enter the dungeon (Cheat)", "Go to the dungeon (Cheat)", "Go to the lord's hall (Cheat)", "I want to speak with a prisoner (Cheat).", "Clear past persuasion attempts (CHEAT)" and "Cheat Used: {CHEAT}" — local game files (v1.4.8) `Modules/*/ModuleData` localization XML

**Gameplay Cheat Menu**
- Campaign option id `EnableCheats`, labelled "Open Cheat Menu", with the description "Cheat Menu can also be enabled by holding 'Left Bumper + Right Trigger + D-Pad Down' in map." (PlayStation variant: L1 + R2 + Down).
- In `DefaultCampaignOptionsProvider.GetGameplayCampaignOptions` the option is added only after an `Input.IsGamepadActive` check. Executing it calls `MapState.Handler.OnGameplayCheatsEnabled()` → `MapScreen.OpenGameplayCheats()`.
- `MapScreen.HandleCheatMenuInput` opens it after holding ControllerLBumper + ControllerRTrigger + ControllerLDown.
- — local game files (v1.4.8) `TaleWorlds.CampaignSystem.ViewModelCollection.dll`, `SandBox.View.dll`, `SandBox/ModuleData/module_strings.xml`, `Native/ModuleData/consoles.xml`
- Cheat Menu entries (`SandBox.GameplayCheatsManager`):
  - Map list: "Add 1000 Gold", "Add 100 Influence", "Add 100 Renown", "Add 10 Crafting Materials Each", "Boost Skill" (group), "Complete All Building Projects in {SETTLEMENT_NAME}" (current settlement, all buildings set to level 3), "Fill Crafting Stamina", "Give 5 Troops", "Give 10 Grain", Give 10 Warhorses, "Heal Player Party", "Unlock All Crafting Recipes", "Unlock Fog of War" (all heroes known).
  - Mission list includes "Heal Main Hero" and "Wound All Enemies".
  - — local game files (v1.4.8) `SandBox.dll` (types and IL)
- Confirmation text: "Enabling cheats will disable the achievements this game. Do you want to proceed?" — local game files (v1.4.8) `StoryMode/ModuleData/Languages/std_StoryMode_GauntletUI.xml`
- Introduced for consoles in v1.2.0 ("15 different cheats … Campaign Options or by holding the Left Bumper, Right Trigger, and D-Pad Down … you will disable achievements by enabling cheats") — [Steam: Development Update #12](https://store.steampowered.com/news/app/261550/view/5124582150127345736)

**Achievement side effects**
- `SandBox.CampaignBehaviors.DumpIntegrityCampaignBehavior.IsGameIntegrityAchieved` fails with three reasons: "{=sO8Zh3ZH}Achievements are disabled due to cheat usage.", "{=R0AbAxqX}Achievements are disabled due to unofficial modules." and "{=dt00CQCM}Achievements are disabled due to version downgrade."
- `CheckCheatUsage` sets the saved flag `Campaign.EnabledCheatsBefore = Game.Current.CheatMode` if it is not already set. The save is therefore marked once the campaign runs with cheat mode on, even if no cheat was typed, and the mark is permanent.
- — local game files (v1.4.8) `SandBox.dll` (IL)
- Since v1.4.0, saves show whether achievements are enabled or disabled: "Added marks to the save games to show if achievements are enabled or disabled." — [Steam: Dev Update BL v1.4.0](https://store.steampowered.com/news/app/261550/view/1829528821306653)
- Guides also warn that once cheats are enabled you stop earning Steam achievements — search summary citing [GamerEmpire](https://gamerempire.net/mount-blade-2-bannerlord-how-to-enable-cheats/)

### Inferences
- For the user's CheatMenu mod (installed as a "Community" module): achievements are disabled anyway by "unofficial modules", so the mod adds no extra achievement cost beyond that.
- Turning on engine `cheat_mode` permanently taints the save. A mod that calls native actions directly, without requiring cheat_mode, gives users finer control.
- High-value wrapping targets:
  - the native Gameplay Cheat Menu: a PC keyboard shortcut or button calling `IMapStateHandler.OnGameplayCheatsEnabled()`, since the option is hidden without a gamepad;
  - a "teleport to settlement" picker, since native only has Ctrl+click;
  - a true-sight toggle;
  - a hotkey cheat-sheet overlay for battle cheats.
- Do not re-implement as new logic: the 15 Cheat Menu actions (gold/influence/renown/troops/grain/warhorses/heal/fog-of-war/crafting/building completion), the battle kill/heal hotkeys, and the console cheats listed above. Wrapping them is better.

### Gaps
- The console-opening key (Alt+~ vs ~) is engine-native and not verified from code; sources disagree.
- I did not verify in-game whether the Ctrl+Left-click teleport also needs the "MapFollowModifier" key or other conditions (the IL shows Ctrl plus CheatMode checks, then position-validity checks).
- I did not confirm whether the gameplay Cheat Menu on PC (via gamepad) also sets `Game.CheatMode`, or only flags achievements through its confirmation flow.
- I could not determine whether `CheatsHotKeyCategory` battle hotkeys are processed only when cheat mode is on. This is assumed from the category name and the 2020 guides, not traced in the mission-screen code.
