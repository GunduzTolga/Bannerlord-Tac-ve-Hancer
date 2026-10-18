# Bannerlord single-player: player complaints, shortcomings and missing features (state as of Oct 2026, BL v1.4.8 stable / v1.5.x beta, War Sails v1.2.8 / v1.3.0 beta)

> Research date: 2026-10-18. Single-player campaign only.
>
> **Version context (needed to judge what is "fixed"):**
> - Bannerlord left Early Access on 25 Oct 2022 (v1.0). The "e1.x" numbers (e1.4, e1.5 ...) are **2020-21 Early Access builds**, not the current 1.4. Don't confuse e1.4.x (2020) patch notes with the 2026 v1.4.x patches. — [OpenCritic/launch info](https://opencritic.com/game/13880/mount-blade-ii-bannerlord), [GameBanshee e1.5 dev update, 2020](https://www.gamebanshee.com/34hcc)
> - **v1.3.0 beta**: 16 Sep 2025 (stealth, disguise, alliances, trade agreements, parley, patrols, deserters, morale/retreat, ~100 random events, sling). — [Wccftech](https://wccftech.com/mount-and-blade-ii-bannerlord-1-3-beta-update-stealth-diplomacy/amp/)
> - **War Sails DLC + base v1.3.4**: 26 Nov 2025 ($24.99). PC Gamer called the free patch's notes about 17,000 words long. War Sails versions run separately (WS v1.0.x paired with BL v1.3.5–1.3.11, WS v1.1.x paired with BL v1.3.13–1.3.14). — [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion), [Blue's News](https://www.bluesnews.com/s/291790/mount-blade-ii-bannerlord-war-sails-released), [PC Gamer](https://www.pcgamer.com/games/rpg/mount-and-blade-2-bannerlord-gets-free-patch-alongside-war-sails-dlc-thats-so-big-the-17-000-word-update-run-more-than-twice-the-length-of-age-of-empires-2s-legendarily-long-patch-notes/), [Patchbot](https://patchbot.io/games/mount-and-blade-ii-bannerlord)
> - **BL v1.4.0 / WS v1.2.0 beta**: 9 Apr 2026 (diplomacy/AI rework, naval AI). The stable line ran **v1.4.5 (21 May 2026) → v1.4.6 (11 Jun) → v1.4.7 (8 Jul) → v1.4.8 (10 Aug 2026, with the War Sails Modding Kit)**. — [GameDropDaily feed](https://gamedropdaily.com/updates/mount-and-blade-ii-bannerlord/), [PCGamesN](https://www.pcgamesn.com/mount-blade-ii-bannerlord/beta-war-sails-raids)
> - **BL v1.5.0 / WS v1.3.0 beta**: 19 Aug 2026. As of Oct 2026 it is still a Steam beta (the TaleWorlds forum lists beta notes v1.5.0–v1.5.5). It adds sandbox start options, clan-party toggles, Blood Feud, mooring, ferries and coastal castle ports. — [GameDropDaily](https://gamedropdaily.com/updates/mount-and-blade-ii-bannerlord/), [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update), [TaleWorlds forum featured threads](https://forums.taleworlds.com/index.php?threads%2Fbeta-1-4-3-peace-and-diplomacy-in-bannerlord.429159%2Flatest=)
>
> **Methodology caveat:** Reddit (reddit.com and the mirrors I tried) and forums.taleworlds.com / taleworlds.com/News blocked automated access (403, TLS or certificate errors). So the "loudness" measures below come mainly from the **Steam review API** (recent and most-helpful reviews, English), Steam discussions, Steam Workshop popularity rankings, and press. The per-category counts were tallied by a summarizing model over truncated JSON (about 50–96 reviews readable per batch). Treat them as **approximate signals, not statistics**.

---

## Q1. What are the top recurring complaints (Steam reviews, especially recent; community; forums)? Ranked and grouped by area, with status in 1.3/1.4

### Takeaway
The base game is still rated well overall: **Very Positive, 89% of 1,608 reviews in the last 30 days; 87% of 98,954 English reviews all-time**. But the loudest complaints, measured by upvotes, are consistent and **structural**:
- TaleWorlds is slow, the community does the real work, and the game "needs mods".
- Bugs, crashes and save loss, plus mods breaking on every patch.
- A hollow, repetitive late game.
- Shallow diplomacy, with AI kingdoms warring "for no reason" and never surrendering.
- Lifeless lords, companions and quests.
- A weak economy and progression layer.

v1.3 and v1.4 addressed several of the *systems* (alliances, trade agreements, war/peace scoring, army AI, patrols, stealth). Players in 2026 still describe the *experience* the same way.

### Cited Findings

#### Overall sentiment numbers (as of 18 Oct 2026)
- Base game: Recent reviews are **Very Positive (89% of 1,608, last 30 days)**. All English reviews are **Very Positive (87% of 98,954)**. — [Steam store](https://store.steampowered.com/app/261550/)
- Base game, all languages: 299,667 reviews, 262,797 positive / 36,870 negative (~87.7%), score label "Very Positive". — [Steam review API](https://store.steampowered.com/appreviews/261550?json=1&filter=all&language=all&purchase_type=all&num_per_page=1)
- War Sails DLC: Recent reviews are **Mostly Negative (32% of 87, last 30 days)**. All English reviews are **Mixed (54% of 2,524)**. It was on sale at $12.49 on 18 Oct 2026. — [Steam store, War Sails](https://store.steampowered.com/app/2927200/); the API shows 2,747 total reviews, 1,487 positive / 1,260 negative, "Mixed". — [Steam review API, WS](https://store.steampowered.com/appreviews/2927200?json=1&filter=recent&language=english&num_per_page=100&purchase_type=all)
- Critics at the 1.0 launch (2022): PC Gamer 80 ("shallow ancillary systems"), IGN 6/10 ("A shallow grind of an overworld strategy game"), Push Square 8 ("the interface can be unwieldy"), TheSixthAxis 7 ("busywork... can feel like a grind"). Top Critic Average 76. — [OpenCritic](https://opencritic.com/game/13880/mount-blade-ii-bannerlord)

#### "Loudness" signal A: the most-helpful negative English reviews of the last 365 days (Oct 2025–Oct 2026; about 50 readable)
Approximate tally by category (number of reviews / sum of upvotes):
- developer neglect, slow updates, "modders do their job": 26 / 3,108
- bugs and crashes: 15 / 1,214
- late game boring or repetitive: 5 / 1,112
- diplomacy and AI kingdoms: 14 / 968
- mods broken by updates: 14 / 884
- lords and NPCs lack personality: 9 / 878
- quests repetitive or few: 5 / 629
- character progression and skills: 6 / 551
- economy, workshops, caravans: 7 / 442
- battle AI: 2 / 311
- endless wars or snowballing: 2 / 306
- marriage and family: 3 / 296
- companions: 3 / 264
- War Sails: 5 / 218
- stealth: 4 / 93
- UI: 2 / 38
- performance: 3 / 35
- sieges: 2 / 34

— [Steam review API, most helpful negative, 365 days](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)

The top individual reviews in that set:
- **Katun (705 upvotes, ~Mar 2026):** the game "becomes boring in single-player once you capture a few castles and cities". The devs should listen to players instead of expanding the map.
- **wildh4rt (322, Jun 2026):** "not even near a finished product".
- **Kuzol (310, 27 Nov 2025):** "Modders have done more to expand, improve, and innovate the game than any patch delivered."
- **Solomos (297, Oct 2025):** "there is so little dialogue and quests the game feels so shallow".
- **Kreschavier (295, Jun 2026):** diplomacy, economy, governors and AI surrender are missing; "The AI will NEVER surrender to you"; the mods that fill the gaps break each update.
- **Bricky (244, Mar 2026):** "late game is tiresome repetetive slog with incompetant friendly AI".
- **Ace of Bastone (198, Dec 2025):** "Starting your own kingdom is basically a death sentence"; political gameplay is thin.
- **Fleecy Bear (169, Jan 2026):** sieges improved, but combat is "downgraded" vs Warband.

— [same source](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)

#### "Loudness" signal B: the 100 most recent negative English reviews (12 Sep – 17 Oct 2026, so the v1.4.8 / v1.5 beta era)
Approximate mentions:
- crashes, bugs, launch/save failures: ~30
- mods required or broken by updates: ~18
- late game boring or empty: ~11
- lack of content / unfinished: ~10
- developer criticism: ~10
- battle or AI behavior: ~9
- stealth missions: ~9
- unfavorable comparison to Warband: ~7
- AI lords and diplomacy: ~6
- sieges: ~6
- quests and dialogue repetitive: ~5
- performance: ~5
- UI: ~4
- marriage and family: ~4
- companions empty: ~4
- endless or overwhelming wars: ~4
- economy: ~3
- War Sails: ~3

— [Steam review API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)

#### "Loudness" signal C: caveats inside recent *positive* reviews (11–18 Oct 2026)
- shallow diplomacy and lord dialogue: ~6
- needs mods: ~4
- weak late game or endgame: ~4
- repetitive: ~3
- bugs: ~3
- hollow companions: ~3
- siege issues: ~3
- faction imbalance or clans switching sides: ~2
- grindy skills: ~2

Quotes: "You shouldn't have to mod it to make it not suck."; "Interacting with lords is painfully dry."; "you can't ask a Lord the location of somebody else in their faction anymore"; "awkward forced stealth segments". — [Steam review API, recent positive](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=positive&purchase_type=all)

#### Ranked list by area (rank reflects loudness across signals A–C plus Workshop demand; status reflects the v1.3/1.4/1.5β changes found)

**1. Meta: "needs mods / devs slow / mods break every patch / bugs and save loss"** (loudest by upvotes)
- In the past-year set, developer neglect is the top category by upvotes. "Mods broken by updates" appears in 14 of about 50 most-helpful reviews. — [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- In Sep–Oct 2026 reviews: "Every workshop update now bricks me from playing my active save"; "they won't even fix bugs that have been in the game for years"; "they're just counting on modders to do the job". — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Save-related complaints: "game deleted my saves multiple times" (Sep 2026); "save breaking bug" (Dec 2025); crashes to desktop "at the end of large battles" (Jul 2026). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Steam's busiest active discussion is the pinned "Base Game & War Sails – Launcher Issues" thread (652 replies, still active Oct 2026). Another thread: "i cant start a new save with warsails". — [Steam discussions](https://steamcommunity.com/app/261550/discussions/0/)
- Since 1.4.6/1.4.7, the patch notes warn that launching before both the base game and War Sails have updated causes a crash. — [GameDropDaily](https://gamedropdaily.com/updates/mount-and-blade-ii-bannerlord/)
- **Status:** persists. This is the key motivation for an all-in-one mod, but it also shows that the mod must survive patches.

**2. Late game / endgame emptiness and repetition** (see Q4 for detail)
- Status: persists in 2026 reviews (Katun 705 upvotes, Bricky 244, "the real trouble starts after 80 hours" May 2026). v1.3, v1.4 and v1.5β tune wars but add no endgame content. — [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)

**3. Diplomacy is shallow and AI kingdom war logic feels random**
- 2025–26 review quotes: "the AI still declares random wars non-stop" (Nov 2025); "Wars between kingdoms start for exactly zero reason" (Jan 2026); "Diplomacy is literally non existant" (May 2026); alliances: "in a few days they'll declare war on you" (May 2026). — [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Steam thread "My Honest Review" (7 Apr, year not shown; presumably 2026, post-1.3):
  - Married relatives still lead attacks on you; alliances and trade deals don't prevent war when they expire.
  - "no long-term memory or reputation system".
  - Allies don't coordinate, call you to war or relieve sieges.
  - Trade agreements are meaningless because trade is free anyway.
  - Diplomacy is "more symbolic than strategic".
  - The AI rarely adopts policies, so factions behave alike.
  - The player's kingdom gets the best policies without penalties ("cheat code").
  - [Steam thread](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/)
- The vanilla Diplomacy tab "lists only the player kingdom's own wars as flat rows". v1.3 alliances "are scattered across encyclopedia pages and notifications". There is no UI data for trade agreements (checked on v1.3.15 + War Sails, Jul 2026). — [GitHub mod PRD](https://github.com/sum117/bannerlord-diplomacy-overview/issues/1)
- Community (2024): diplomacy amounts to declaring war; requested features that mods already deliver include "diplomacy, sending messengers, a bank system, custom troops tree, building fiefs". — [r/Bannerlord mirror via Lemmy](https://lr.in.psf.lt/r/Bannerlord/comments/1gpnhvk/warband_or_bannerlord)
- **What was addressed:**
  - **v1.3** (Nov 2025): alliances (max 2 per kingdom, 1-year), trade agreements (max 2, 1-year), parley, and war/peace/tribute decisions reworked around a "war progress score" to reduce short wars. — [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion), [Wccftech](https://wccftech.com/mount-and-blade-ii-bannerlord-1-3-beta-update-stealth-diplomacy/amp/)
  - **v1.4.0β → stable v1.4.5+** (Apr–May 2026): honor and relation penalties for declaring war on an ally; a relation penalty for refusing an ally's Call to War; clearer alliance tooltips; trade agreements now weigh inter-clan marriages. TaleWorlds said alliances had become "a default that everyone opts into" and should respond "to actual threats". — [TaleWorlds Dev Update & Beta WS v1.2.0 / BL v1.4.0 (via search excerpt)](https://www.taleworlds.com/en/News/598), [games.gg](https://games.gg/news/bannerlord-beta-diplomacy-coastal-raids/), [PCGamesN](https://www.pcgamesn.com/mount-blade-ii-bannerlord/beta-war-sails-raids)
  - **v1.5.0β** (Aug 2026): "more responsive" diplomacy; lords "pursue meaningful wars"; Blood Feud system (executions trigger clan-wide vendettas that ban ransoms, defections and safe passage). — [Patchbot](https://patchbot.io/games/mount-and-blade-ii-bannerlord), [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update)
- **Still missing per players in 2026:** AI surrender or vassalization, non-aggression pacts (vanilla), messengers or remote diplomacy, a diplomacy overview UI, faction "doctrine" or identity, and memory of past favors.
- Popularity proxy: **Diplomacy (Bennyz) is the #4 most-subscribed Workshop mod of all time**, behind only three library mods (Harmony, UIExtenderEx, ButterLib). — [Steam Workshop, most subscribed](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=totaluniquesubscribers&section=readytouseitems&actualsort=totaluniquesubscribers&p=1)

**4. Lords, NPCs and companions are lifeless; quests are thin and repetitive**
- "characters are empty, you WON'T care about companions and lords" (Sep/Oct 2026); lords are "walking stat sticks with zero personality" (Oct 2026, positive review). — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all), [Steam API, recent positive](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=positive&purchase_type=all)
- "so little dialogue and quests" (297 upvotes); "small quests that has a garbage effort to worth ratio" (Dec 2025); "one voice actor for ALL CULTURES" (Dec 2025). — [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Companions need missions, opinions and reactions ("lifeless"). The gang/alley system is underdeveloped (alley bandits dismiss the ruler; guards ignore street attacks). — [Steam "My Honest Review" thread](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/)
- Status: persists. v1.3 added ~100 pop-up random events and notables as supporters; v1.5β adds trait gameplay effects and 39 naval events. None of this adds companion personality or quest depth. — [Wccftech](https://wccftech.com/mount-and-blade-ii-bannerlord-1-3-beta-update-stealth-diplomacy/amp/), [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion), [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update)

**5. Kingdom founding and vassal management (the player-ruler experience)**
- "Starting your own kingdom is basically a death sentence" (198 upvotes, Dec 2025). — [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Older threads (2020–24): a new kingdom gets 4 war declarations within seconds; influence can't stop it; peace requires repeated tribute. One cited cause is the main-quest conspiracy forcing factions into war. — [Steam thread](https://steamcommunity.com/app/261550/discussions/0/4139439021822232051), [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3493131640461031782)
- Vassal AI: "the AI of my lords cant create an army, cant defend, cant capture"; a lord "changes the kingdom and joins to the weakest faction" (Oct 2026). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Fief votes: fiefless clans keep winning votes, and the conqueror is sometimes not even a candidate. Clans with fiefs defect and take the fiefs with them (2020–23 threads; current state unverified). — [Steam thread](https://steamcommunity.com/app/261550/discussions/0/2144217924396178751), [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3198115500357297935)
- "There is STILL NO DEMOCRAY" (no alternative government types; Dec 2025). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Status: partly addressed. v1.5.0β replaces "vague" clan-party commands with direct toggles (raid villages, donate troops to garrisons, join armies, maintain fleets). TaleWorlds reportedly called the old system "unpredictable and largely ineffective" (the CalradiaWar page says this is a paraphrase). Fief revocation/granting UI, vassal orders, and the founding dogpile are not shown as addressed. — [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update), [GameDropDaily](https://gamedropdaily.com/updates/mount-and-blade-ii-bannerlord/)

**6. Economy, settlements and governors**
- "economy being micromanaged mess" (Nov 2025); "town and castle building is non u pick something and it never gets built" (Dec 2025); "Bartering with other lords is completely useless" (Sep–Oct 2026). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all), [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- The -3 culture loyalty penalty makes settlement management tedious. Counterpoint: policies (Forgiveness of Debts, Tribunes) negate it; one player says patch 1.3 improved fief management and towns rarely rebel. — [Steam "My Honest Review"](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/)
- Garrisons: there is no toggle to stop garrison troops auto-upgrading (the party has one), so players micromanage wages; Sturgian garrisons over-upgrade into cavalry. — [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3812909627186641050)
- Demand proxies: **Improved Garrisons is #5 most-subscribed ever; Governors Handle Issues #18**. — [Workshop most subscribed](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=totaluniquesubscribers&section=readytouseitems&actualsort=totaluniquesubscribers&p=1). **Village Enterprise, GoldRushConstruction and Bannerloot** trend in the past year. — [Workshop trending, 365 days](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=trend&section=readytouseitems&actualsort=trend&days=365&p=1)
- Status: partly addressed in v1.3. Caravans can change their home town and avoid kingdoms; caravan profit logic was tuned; settlement projects were reworked; Guard House patrols were added; notables can become supporters. v1.5β gives the Calculating trait more workshop income. — [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion), [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update)

**7. Character progression and skills**
- "only gained 2 lvl's at 9.5x XP and at lvl 26" (Nov 2025); "character development system is uninteresting, too balanced" (Nov 2025); "please... boost shipmaster xp gain". — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- The learning-limit system was called "anti-fun" and grindy in 2020–23. Riding, Leadership, Engineering and Medicine are singled out, and players use 2x XP mods. — [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3463857594041797912/), [Goonhammer review](https://goonhammer.com/video-game-review-mount-and-blade-ii-bannerlord)
- Demand proxy: **Adjustable Leveling and Both Perks are in the top 10 trending mods of the past year**. — [Workshop trending, 365 days](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=trend&section=readytouseitems&actualsort=trend&days=365&p=1)
- Status: partly addressed. v1.3 added an optional **Fast Mode module** (shorter years, faster aging, tuned skill progression for multi-generation play) and hero Import/Export via save files. — [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion). v1.5β adds starting roles and modifiers. — [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update)

**8. Battle AI and sieges**
- Sieges: "half your army just standing around, refusing to climb ladders or break down gates" (Sep–Oct 2026); "AI not using ladders or ladders just clipping and dropping off" (Dec 2025); defenders ignore the assault to target a lone archer. — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all), [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Another army joins "to hijack the siege" (Dec 2025). — [same](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- The ladder and tower AI complaints date back to 2020: "three dudes will climb up the ONE ladder". — [Steam thread, 2020](https://steamcommunity.com/app/261550/discussions/0/2998799976100851849)
- Field battles:
  - Troops "brainlessly charge" when the player dies (Nov 2025).
  - Shield walls don't adapt to arrows.
  - Horse-archer spam and kick exploits.
  - Combat feel "downgraded" from Warband.
  - Only partially fixed: v1.3 lets you take control of another troop after being knocked out.
  - [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all), [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion)
- Demand proxy: RBM (Realistic Battle Mod) #7 and RTS Camera/RTS Command are among the most popular mods. — [Workshop most subscribed](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=totaluniquesubscribers&section=readytouseitems&actualsort=totaluniquesubscribers&p=1), [Workshop trending](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=trend&section=readytouseitems&actualsort=trend&days=365&p=1)
- A reviewer says the Sep 2025 patch had "genuine fixes" (e.g., "soldiers actually using pikes"). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)

**9. Stealth / hideouts / forced missions (new complaint introduced by v1.3)**
- "get rid of the mandatory stealth mission" (Sep–Oct 2026). Stealth was mentioned in ~9 of 100 recent negative reviews. — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- "I can kill everyone, but since I am spotted I guess I just lose" (Nov 2025); "now im forced to b a stealth player or just send in my troop" (Oct 2026). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- v1.3 made stealth/disguise power prison breaks, *new hideout scenes* and hostile town infiltration. — [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion). The v1.4 beta made "stealth harder but pays more", and hideout clearing now factors relevant skills. — [PCGamesN](https://www.pcgamesn.com/mount-blade-ii-bannerlord/beta-war-sails-raids), [games.gg](https://games.gg/news/bannerlord-beta-diplomacy-coastal-raids/)
- Status: persists. An "optional stealth / classic hideout" toggle is a concrete mod opportunity.

**10. Marriage, family and clan members**
- "marrying them will remove them from my clan"; "There should be an option to turn off the marriage proposal for family members." (Sep–Oct 2026). — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Older threads (2020–24; current state unverified):
  - Companions can't be married off in vanilla.
  - The matchmaker offers the oldest single woman.
  - Clans founded by promoted companions are weak and die out.
  - Sisters' spouses join the husband's clan.
  - [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3824173836658245742), [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3191369524225967723), [Steam thread](https://steamcommunity.com/app/261550/discussions/0/4509877583652524639), [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3112530528180876720)
- Demand proxies: Family Tree (#24 all-time); Recruit Exiled Clans (#1 trending in the past year); Noble Families and Houses Extended (War Sails-compatible collections). — [Workshop most subscribed](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=totaluniquesubscribers&section=readytouseitems&actualsort=totaluniquesubscribers&p=1), [Workshop trending](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=trend&section=readytouseitems&actualsort=trend&days=365&p=1)
- Faction destruction (kingdoms can be eliminated) is now vanilla; a 2026 Steam poster credits community feedback for it. — [Steam "My Honest Review"](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/)

**11. UI / QoL**: see Q5.

**12. War Sails / naval**: see Q3.

### Inferences
- The best-rated base game combined with the loudest complaints being about *depth* suggests the audience is engaged long-term players. An all-in-one SP mod should target 50–300-hour campaigns: late-game politics, vassal management, NPC depth.
- Of the top three Workshop mods after the libraries, two fill system gaps: Diplomacy and Improved Garrisons. RBM is a battle overhaul. Workshop subscriptions are the clearest quantifiable demand signal available.
- TaleWorlds' 2025–26 work concentrated on war/peace scoring, alliances, army AI and naval AI. That makes it risky for a mod to rewrite those same systems (patch churn). Lower-risk, high-value targets are adjacent layers: diplomacy UI/overview, NAPs, surrender/vassalization, vassal orders, fief management, companion/NPC content, QoL.
- Stealth-mission complaints are a *new* 1.3-era gap and are cheap to fix (an option to bypass or auto-resolve).

### Gaps
- Reddit (r/Bannerlord, r/mountandblade) and the TaleWorlds forums could not be read, so I couldn't measure thread counts or upvotes there. The Steam API stands in as the quantitative proxy.
- The review category counts come from truncated JSON (~50–96 reviews per batch) tagged by a summarizing model. They are directional only.
- Many fief-vote, defection and marriage-mechanic complaints come from 2020–24 threads. Whether each still behaves the same in v1.4.8 was not verified.

---

## Q2. Which complaints persist in v1.3/v1.4 vs which were addressed (and by which patch)?

### Takeaway
TaleWorlds has directly targeted:
- **diplomacy structure** (v1.3 alliances, trade agreements, war-progress score; v1.4 alliance and Call-to-War penalties; v1.5β "meaningful wars" and Blood Feud)
- **army/party AI** (v1.3; v1.4 "fewer but stronger armies", fewer "prolonged, low-activity wars")
- **clan-party control** (v1.5β toggles)
- **generational pacing** (v1.3 Fast Mode)
- **sandbox variety** (v1.5β start options)

These remain unaddressed or are still loudly complained about:
- surrender/vassalization
- NPC, companion and quest depth
- the dogpile on newly founded kingdoms
- siege AI
- economy depth
- the diplomacy UI
- forced stealth
- patch-induced mod breakage

### Cited Findings

#### Addressed or partly addressed (with patch)
| Complaint | Change | Patch | Source |
|---|---|---|---|
| No alliances / no trade deals | Alliances (2 per kingdom, 1 yr; ends on war between them) and trade agreements (2 per kingdom, 1 yr; more caravan traffic) | v1.3.0β Sep 2025 → v1.3.4 live 26 Nov 2025 | [Wccftech](https://wccftech.com/mount-and-blade-ii-bannerlord-1-3-beta-update-stealth-diplomacy/amp/), [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion) |
| Many short pointless wars | War/peace/tribute decisions reworked around a "war progress score", "reducing the number of short wars" | v1.3.4 | [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion) |
| Can't talk to enemy lords without fighting | Parley with nobles in enemy settlements | v1.3 | [Wccftech](https://wccftech.com/mount-and-blade-ii-bannerlord-1-3-beta-update-stealth-diplomacy/amp/) |
| Armies/parties act suicidally | Armies and parties weigh strength, wounded and food when choosing targets / retreating / disbanding; morale & retreat systems | v1.3 | [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion), [Wccftech](https://wccftech.com/mount-and-blade-ii-bannerlord-1-3-beta-update-stealth-diplomacy/amp/) |
| Villages undefended | Guard House patrol parties; Deserter bands | v1.3 | [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion) |
| No caravan control | Change caravan home town, avoid kingdoms | v1.3 | [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion) |
| Children take too long / dynasty play slow | Optional "Fast Mode" module (shorter years, faster aging, tuned skills) | v1.3 | [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion) |
| No console/hero editing | Hero Import/Export via save files | v1.3 | [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion) |
| Battle over when player knocked out | Take control of another troop after knockout | v1.3 | [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion) |
| Alliances meaningless / auto-joined | Honor and relation penalties for warring on an ally; penalty for refusing Call to War; alliances "respond to actual threats"; clearer tooltip | v1.4.0β Apr 2026 → stable ~v1.4.5 May 2026 (inferred) | [TaleWorlds News/598 (search excerpt)](https://www.taleworlds.com/en/News/598), [games.gg](https://games.gg/news/bannerlord-beta-diplomacy-coastal-raids/) |
| Long inactive wars; armies drain manpower | Fewer "prolonged, low-activity wars"; armies "rarer but on average stronger" | v1.4.0β | [games.gg](https://games.gg/news/bannerlord-beta-diplomacy-coastal-raids/), [PCGamesN](https://www.pcgamesn.com/mount-blade-ii-bannerlord/beta-war-sails-raids) |
| Can't control clan/companion parties | Direct toggles: raid villages, donate troops to garrisons, join armies, maintain fleets | v1.5.0β Aug 2026 (still beta) | [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update), [GameDropDaily](https://gamedropdaily.com/updates/mount-and-blade-ii-bannerlord/) |
| Executions have no consequence; traits meaningless | Blood Feud; traits get gameplay effects (Cruel, Merciful, Calculating) | v1.5.0β | [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update) |
| Same start every time | Sandbox scenarios (United Empire, Invasion, Last Stand, War of Two Kings, Nord Invasion), 9 backgrounds, world modifiers, seeds | v1.5.0β | [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update) |
| Slow map travel | Land/sea speed +15%; "faster map movement" modifier | v1.5.0β | [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update) |
| Snowballing (Early Access era) | AI war/peace voting reworked; clans won't defect to kingdoms that captured their fiefs | e1.5.0 (2020); outdated framing | [GameBanshee 2020](https://www.gamebanshee.com/34hcc), [Steam thread](https://steamcommunity.com/app/261550/discussions/0/2264689848033111449) |

#### Persisting into the v1.4.8 era (evidence dated 2026)
- AI never surrenders; random wars; alliances collapse into war. (Jan–Jun 2026 reviews.) — [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Friendly/vassal AI incompetent; lords defect to the weakest faction (Oct 2026). — [same](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Siege AI (ladders, idle troops) (Sep–Oct 2026). — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- Lifeless NPCs and companions; thin quests (Oct 2026). — [Steam API, recent positive](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=positive&purchase_type=all)
- Diplomacy UI gaps (Jul 2026, v1.3.15). — [GitHub](https://github.com/sum117/bannerlord-diplomacy-overview/issues/1)
- "Last seen" locations "entirely wrong" (Jan 2026); you can no longer ask lords where other lords are (Oct 2026). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all), [Steam API, recent positive](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=positive&purchase_type=all)
- Some say the updates made things worse. "Recent updates have made things worse" (Nov 2025). Others see real gains: "had some amazing content" but "very drastic changes to the combat system" (Dec 2025). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- A player says patch 1.3 improved fief management: towns rarely rebel, and AI kingdoms can govern mixed-culture fiefs. — [Steam "My Honest Review"](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/)
- TheGamer (27 Nov 2025): the army and party AI rework was "generally received well". But the alliance/diplomacy changes "aren't the major diplomacy overhaul the community has requested". — [TheGamer](https://www.thegamer.com/mount-and-blade-2-dlc-war-sails-mixed-reviews-bugs/)

### Inferences
- Many v1.4/v1.5 fixes are recent (May–Aug 2026), and v1.5 is still beta. Reviews from Oct 2025 to mid-2026 partly describe states that are already patched. A mod plan should be re-checked against v1.5.x stable when it ships. Clan-party control, sandbox starts, trait effects and naval mooring could become redundant.
- The "fixed" items are mostly **AI scoring changes** (invisible to players). The persisting complaints are about **missing player-facing verbs**: demand surrender, sign NAP, send messenger, revoke/grant fief, order vassal, see diplomatic web. This explains why sentiment hasn't moved much.

### Gaps
- I could not access the full official v1.3.0, v1.4.x or v1.5.x patch notes (taleworlds.com returned 403). The changes above come from secondary summaries (MP1st, Wccftech, games.gg, PCGamesN, CalradiaWar) plus one official excerpt from search.
- Which v1.4.0β changes exactly reached stable in v1.4.5–1.4.8 is inferred, not confirmed.
- Whether v1.5.x has gone stable on PC/console by 18 Oct 2026 is unconfirmed. Forum listings show v1.5.0–1.5.5 beta notes.

---

## Q3. What are the main complaints about the War Sails / naval DLC?

### Takeaway
War Sails (26 Nov 2025) sits at **Mixed (54%) overall and Mostly Negative (32%) for the last 30 days**, a year after launch. The loudest complaints, by upvotes:
- **price/value**
- **thin, unfinished content and bugs**
- **naval play disconnected from the campaign**: AI lords rarely sail, which TaleWorlds itself admitted in Apr 2026
- **repetitive naval battles**: volley → ram → board brawl
- **clunky ramming and ship physics bugs**
- a **forced, long tutorial/questline**

TaleWorlds has since added seaborne raids (v1.4.5), lords sailing more (v1.4), and mooring, ferries and coastal castle ports (v1.5β).

### Cited Findings

#### Ratings
- Mostly Negative (32% of 87, last 30 days); Mixed (54% of 2,524 English). — [Steam store, War Sails](https://store.steampowered.com/app/2927200/)
- At launch, ~60% positive / 40% negative ("Mixed"). — [TheGamer, 27 Nov 2025](https://www.thegamer.com/mount-and-blade-2-dlc-war-sails-mixed-reviews-bugs/)
- Press scores:
  - TheSixthAxis: 5 ("this isn't a must-buy expansion"), 13 Jan 2026. — [TSA](https://www.thesixthaxis.com/2026/01/13/mount-blade-2-bannerlord-war-sails-review/)
  - JumpDashRoll: 7, 17 Dec 2025. — [JDR](https://jumpdashroll.com/article/mount-and-blade-ii-bannerlord---war-sails-review)
  - GamesHub NL: 3.5 stars, headline "a sea full of irritations" (search excerpt). — [GamesHub NL](https://www.gameshub.com/nl/?p=84745)

#### Loudest complaints: most-helpful negative War Sails reviews (top 15, ~5,139 total upvotes)
Category totals:
- **Price/value:** 9 reviews / 2,607 upvotes.
- **Lack of content:** 6 / 2,363.
- **Naval integration (AI lords not sailing, ships peripheral):** 6 / 1,959.
- **Bugs/crashes:** 5 / 1,895.
- **Naval battle AI / repetitive fights:** 4 / 1,598.
- **Ramming:** 2 / 1,155. Friendly AI rams allies, and ramming is weak.
- **Map changes:** 4 / 762.
- **Stealth:** 3 / 545.
- **Mods broken:** 2 / 507.
- **Ship handling:** 2 / 443.
- **Forced story/tutorial:** 2 / 442.
- **Nord roster:** 1 / 287.

— [Steam API, WS most helpful negative](https://store.steampowered.com/appreviews/2927200?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)

Key quotes from that set:
- Mayhem (914 upvotes, Dec 2025): "There's no purpose to the water besides coolness"; weather and storms have no effect.
- Rolfe Mosher Walker (781): charging "half the price of the game".
- Elesn (507): "virtually launched their DLC as early access".
- Lehfurd (484, Apr 2026): game-breaking bugs "unanswered for months on end".
- Kevin (284, Apr 2026): "no tie-ins to the core gameplay loop".
- Holden (241): "Every sea battle is the exact same as the last."; friendly ramming; upgrades locked to certain ports; perks not implemented.
- Zenith (287): lords rarely sail; weak Nord AI.
- TopGun... (118): "Ship combat is fun, but irrelevant."
- Yosh (200, Mar 2026): AI lords "freely embark/disemambark at will, from anywhere" while the player must return to the ship.
- AdamusPRO13 (103): stuck ballista view, wrong troop cap, desertion bugs.

— [same](https://store.steampowered.com/appreviews/2927200?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)

#### Recent War Sails reviews (~mid-Aug to mid-Oct 2026, 80 readable: ~48% positive)
Approximate mentions:
- bugs and clipping: 18
- price: 10
- forced/linear tutorial: 9
- crashes: 7
- lack of content: 6
- ship controls and physics: 6
- mods broken: 5
- Nord faction: 5
- naval map integration: 4
- ramming: 3
- ship cap of ~8 per side: 2
- overcrowded boats: 2

Quotes:
- "Ship physics bugs can force you to fail or spend 20 minutes swimming back to your vessel."
- "Sitting around waiting 2 minutes between 6 trivial raid waves is incredibly boring."
- "you will find yourself auto resolving it constantly"
- "The ramming tutorial story quest is nigh impossible to do"
- "theres no sailing battles after the campaign."
- "The limit is about 8 ships for each side"
- The Nords "doesnt feel that different from the sturgians".

Praised: boarding, ramming and fire pots; ship variety and visuals; weather/night effects; Nord aesthetics; trading and sea-raiding opportunities. — [Steam API, WS recent](https://store.steampowered.com/appreviews/2927200?json=1&filter=recent&language=english&num_per_page=100&purchase_type=all)

#### Press and launch-week specifics
- **Launch bugs:** random crashes; lag spikes at sea and entering villages; ship "spiraling" after ramming; **troops have no icons** (hard to organize); AI orders leave troops idle; bugged quests and stealth; forced quests "awful"; $25 too much. — [TheGamer](https://www.thegamer.com/mount-and-blade-2-dlc-war-sails-mixed-reviews-bugs/)
- **TheSixthAxis:** naval combat follows "the same tedious pattern" (missile volleys, then boarding) and "lacks any kind of pleasing tactical variety". Boarding is a mass brawl where it's "difficult to make friend from foe". The exposition "drag[s] on"; characters are "barely animated". Maritime skill requirements are "ambiguous and time consuming". — [TSA](https://www.thesixthaxis.com/2026/01/13/mount-blade-2-bannerlord-war-sails-review/)
- **JumpDashRoll:** "within that context, naval combat is almost entirely pointless". Fleets cost more than land armies ("better served by conscripting cannon fodder"). Nord and ship gear have no use on land. "almost identical to Viking Conquest". — [JDR](https://jumpdashroll.com/article/mount-and-blade-ii-bannerlord---war-sails-review)
- **GamesHub (search excerpt only; page fetch failed):** AI "oddly incompetent at using the ship-mounted artillery"; ships "struggle to disengage once they've locked onto their first target"; no naval party role; Shipmaster needs a very high level to unlock a 4th ship. — [GamesHub "Sea of Peeves"](https://www.gameshub.com/news/news/bannerlord-war-sails-review-sea-of-peeves-2847322/)
- **Steam thread "Warsails is actually a Downgrade" (21 Jan, presumably 2026):**
  - Nords have no cavalry or horse archers, and no villages sell horses.
  - AI ships stay near ports.
  - Sailing is "fit for nothing more than a bandit playthrough".
  - Rivers are too wide, with fewer roads and forests; Sturgia is cut in two.
  - Disputed: battle-terrain grid "not updated". A reply says the appropriate maps do load and calls Shipmaster gain "too slow".
  - [Steam thread](https://steamcommunity.com/app/261550/discussions/20/690873299732882059/)

#### Developer acknowledgment and fixes
- Apr 2026 (v1.4.0 / WS v1.2.0 beta): TaleWorlds said it was "too difficult to consistently locate the fleets of nobles" and aimed to "close gaps in the experience". Changes:
  - AI lords take longer sea journeys and sometimes pursue goals entirely at sea.
  - A new offensive naval patrol targets ports.
  - The Mariner skill reduces naval combat penalties.
  - Seaborne raids need shallow-draft ships.
  - [PCGamesN](https://www.pcgamesn.com/mount-blade-ii-bannerlord/beta-war-sails-raids), [games.gg](https://games.gg/news/bannerlord-beta-diplomacy-coastal-raids/), [TaleWorlds News/598 (search excerpt)](https://www.taleworlds.com/en/News/598)
- Seaborne village raids went live with v1.4.5 / WS v1.2.5 (21 May 2026). WS v1.2.6 fixed a "Leave" option appearing wrongly when helping a raided village. — [GameDropDaily](https://gamedropdaily.com/updates/mount-and-blade-ii-bannerlord/), [Patchbot](https://patchbot.io/games/mount-and-blade-ii-bannerlord)
- v1.5.0 / WS v1.3.0 beta (Aug 2026):
  - Mooring at allied or neutral ports; ferries for parties under 50 men; coastal castle ports with blockades.
  - 39 naval events and early pirate factions; cutting boarding ropes.
  - Lords go to sea, harass convoys and raid villages; piracy AI reworked.
  - Hostile village actions no longer auto-trigger war.
  - [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update), [Patchbot](https://patchbot.io/games/mount-and-blade-ii-bannerlord), [search excerpt of TaleWorlds beta notes](https://www.taleworlds.com/en/News/610)
- The Steam store page now lists "custom naval battles" among War Sails features. — [Steam store, War Sails](https://store.steampowered.com/app/2927200/)

### Inferences
- The structural naval complaints are fixable in a mod:
  - AI fleets absent from the campaign
  - fleets uneconomic vs land armies
  - ship cap (~8 per side) and 4th-ship Shipmaster gate
  - upgrades locked to specific ports
  - Nord roster lacking cavalry
  - friendly ramming, and artillery AI that won't disengage
  - long raid-wave waits
  - forced long tutorial: a skip/fast-start option
  
  Some are already moving in v1.4/v1.5β, especially AI sailing and mooring/ferries. Any naval mod work should wait for v1.5 stable.
- The price/value complaint can't be modded. But "naval content irrelevant to the core loop" can be, for example with sea trade routes, naval blockades with economic effect, and lord fleets as campaign actors.

### Gaps
- The GamesHub review page couldn't be fetched (connection reset). Its points come from a search-engine summary only.
- Which specific launch bugs (missing troop icons, ballista camera, friendly ramming) were fixed in WS v1.0.x–1.2.8 couldn't be verified, because the official patch notes were blocked.
- One recent review says War Sails released "in ~Aug 2026". That is wrong (release was 26 Nov 2025). The reviewer likely means the Aug 2026 update or modding kit. — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)

---

## Q4. What does the "late game is boring / snowballing / endless wars / lack of diplomacy" criticism concretely consist of?

### Takeaway
Concretely, it is a bundle of mechanics:
1. a fixed loop (fight → siege → recruit) with nothing new after ~80 hours
2. AI kingdoms that rebuild armies instantly, declare war without reasons and never capitulate, so conquest is a whack-a-mole grind
3. player kingdoms that get dogpiled at founding and whose vassals can't wage war competently
4. diplomacy reduced to war/peace/tribute, plus (since 1.3) short-term alliances and trade deals with no memory or coordination
5. fief and settlement management that is tedious rather than strategic
6. a huge map that dilutes victories

v1.3 and v1.4 tuned war frequency (fewer short wars, then fewer long inactive wars), but the bundle remains in 2026 reviews.

### Cited Findings
- **Repetitive core loop:**
  - "Fight, capture fief, recruit" (Steam thread; from a search summary, so the exact thread is unverified). — [Steam thread "Any interesting late game mechanics?"](https://steamcommunity.com/app/261550/discussions/0/4351116701414774560/)
  - "you have done all the base game has to offer" (Nov 2025); "the real trouble starts after 80 hours" (May 2026); "most of the time u will stay idle waiting for a war to happen" (Dec 2025). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
  - "it quickly devolves into a meaningless grind"; "you keep doing the same thing over a million times" (Sep–Oct 2026). — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- **Instant AI recovery ("whack-a-mole"):**
  - "Defeated enemies regain their armies in the blink of an eye and teleport around the map" (Nov 2025). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
  - In 2021: a 1,300-troop army appeared within minutes after ~30 lords were imprisoned; towns were retaken within a day; released nobles returned immediately with full armies ("Released noble prisoners should have to follow the same rules as the player"); "bigger does not equal better" (map size). — [Steam thread, 2021](https://steamcommunity.com/app/261550/discussions/0/3057365873425554010/)
  - AI cheats on recruiting and gets near-constant access to top-tier troops (2025–26). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- **Endless/relentless wars:**
  - "the late game stops being fun and starts being relentless" (War Sails review, Feb 2026). — [Steam API, WS most helpful negative](https://store.steampowered.com/appreviews/2927200?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
  - "it just got super overwhelming and impossible to defeat" (Sep–Oct 2026). — [Steam API, recent negative](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=negative&purchase_type=all)
  - A 2023 vassal asked for wars to be "longer, more decisive and less frequent" (from a search summary; exact thread unverified). — [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3835424220122659041)
- **No capitulation:** "The AI will NEVER surrender to you" (295 upvotes, Jun 2026). — [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- **Peace doesn't stick:** peace → a "million soldiers" attack again within ~10 days (older Steam threads, via search summary; which thread the quote comes from is unverified). Truce expiry → immediate war even with marriage ties (2026). — [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3551679689865854584), [Steam "My Honest Review"](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/)
- **Own-kingdom dogpile and weak vassals:**
  - "Starting your own kingdom is basically a death sentence" (Dec 2025).
  - Vassal lords "cant create an army, cant defend, cant capture"; lords join "the weakest faction" (Oct 2026).
  - Companion parties "act autonomously—often suicidally" (Nov 2025).
  - [Steam API, helpful negative](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- **Developer acknowledgment (Apr 2026):** TaleWorlds targeted "prolonged, low-activity wars", alliances as "a default that everyone opts into", and armies emptying troop pools. — [games.gg](https://games.gg/news/bannerlord-beta-diplomacy-coastal-raids/), [PCGamesN](https://www.pcgamesn.com/mount-blade-ii-bannerlord/beta-war-sails-raids)
- **Earlier pass (v1.3.4, Nov 2025):** a war-progress score to reduce short wars. — [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion)
- **Original pass (e1.5, 2020):** snowballing and "long periods of inactivity". — [GameBanshee 2020](https://www.gamebanshee.com/34hcc)
- **Snowballing specifically** is now a weaker theme. A 2022 veteran said no faction had a monopoly on snowballing, though Khuzait snowballed in year one. Recent reviews mention "faction imbalance / clans switching sides" (~2 of ~96 positive Oct 2026), and War Sails reviews call the Nords "too strong" in a few cases. — [Steam thread](https://steamcommunity.com/app/261550/discussions/0/2264689848033111449), [Steam API, recent positive](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=positive&purchase_type=all), [Steam API, WS recent](https://store.steampowered.com/appreviews/2927200?json=1&filter=recent&language=english&num_per_page=100&purchase_type=all)
- **Strategic/political thinness:**
  - AI rarely adopts policies, so factions behave alike; the player gets policies the AI can't use. — [Steam "My Honest Review"](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/)
  - No faction "phylisophical, material, regional, climate, or military doctrine" (Jan 2026). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
  - There's "no underlying vision" for the strategic layer; the army system is weaker than Warband's marshals (2021). — [Steam thread, 2021](https://steamcommunity.com/app/261550/discussions/0/3057365873425554010/)

### Inferences
- These late-game levers can be modded and match the complaints one-to-one:
  - war exhaustion / war goals (casus belli)
  - AI capitulation / vassalization of a beaten kingdom
  - a recovery delay for released or defeated lords and armies
  - NAPs and alliance memory
  - vassal army orders / campaign objectives
  - a kingdom-founding grace period / recognition
  - endgame events: civil wars, claimants, invasions. v1.5β "Invasion" and "Last Stand" scenarios exist only as *start* options.
- War frequency is being retuned by TaleWorlds every major patch. A mod that adds *layers* (war goals, exhaustion modifiers, surrender) on top of vanilla scoring is less likely to break than one that replaces the scoring.

### Gaps
- I found no 2026 source comparing late-game pacing before and after v1.4.5. It is unknown whether the v1.4 "fewer long inactive wars" change actually reduced the endless-war complaint.
- I found no current source on feasts, tournaments as an endgame, or civil war/claimant mechanics, so I don't know how often these are requested in 2026.

---

## Q5. Which UI/QoL annoyances are mentioned most (inventory, party screen, troop upgrades, encyclopedia, fast travel, waiting, settlement management)?

### Takeaway
UI complaints are *low in upvotes* in reviews (UI got only 2 most-helpful reviews and 38 upvotes in the past year). But QoL mods dominate Workshop demand: Better Time (#8 all-time), Improved Garrisons (#5), Governors Handle Issues (#18), Family Tree (#24), Party AI Controls, Item Quality Indicator, Camp Menu, RTS Camera. The concrete annoyances:
- too many clicks / confusing UI
- missing event logging and warnings
- no diplomacy overview
- garrison and settlement micromanagement
- party-screen bulk actions (handled by mods)
- slow time/travel
- unreliable lord-location info

### Cited Findings
- "Very confusing UI that takes tons of extra clicks"; "clunky strategy map mechanic"; events with no warning that are "not even logged in the tiny log history you have"; troops "randomly" dropped into AI towns; companions sell gear "in every village you visit" (Dec 2025). — [Steam API, helpful negative, part 2](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- "0 tutorial or real guiding" (Dec 2025). — [same](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
- **Lord location info:**
  - "the 'last seen' location is entirely wrong" (Jan 2026). — [same](https://store.steampowered.com/appreviews/261550?json=1&filter=all&day_range=365&language=english&num_per_page=100&review_type=negative&purchase_type=all)
  - "you can't ask a Lord the location of somebody else in their faction anymore" (Oct 2026). — [Steam API, recent positive](https://store.steampowered.com/appreviews/261550?json=1&filter=recent&language=english&num_per_page=100&review_type=positive&purchase_type=all)
- **Diplomacy UI:** own wars only; alliances scattered across encyclopedia pages and notifications; no trade-agreement display (v1.3.15, Jul 2026). — [GitHub](https://github.com/sum117/bannerlord-diplomacy-overview/issues/1)
- **Party screen:** Party Screen Enhancements (Hirtol) adds Upgrade All, Sort All, Recruit All Prisoners and configurable sorting. A Ctrl-click shortcut upgrades or recruits all without a popup. — [GamePressure mod page](https://www.gamepressure.com/download/mount-blade-ii-bannerlord-party-screen-enhancements-v1610-mod/z213086), [Nexus changelog (search excerpt)](https://www.nexusmods.com/mountandblade2bannerlord/mods/492/?tab=logs)
- **Garrisons and settlements:**
  - No garrison auto-upgrade toggle.
  - Players want security and project completion times shown in the overview "so players don't have to switch tab to tab".
  - AI lords can't afford garrison upkeep and run with 30–40 troops.
  - [Steam thread](https://steamcommunity.com/app/261550/discussions/0/3812909627186641050), [Improved Garrisons Workshop discussion (search excerpt)](https://steamcommunity.com/workshop/filedetails/discussion/2859265386/3596716230223349363)
- **Time and travel:**
  - Better Time (time-speed control) is the #8 most-subscribed mod ever. — [Workshop most subscribed](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=totaluniquesubscribers&section=readytouseitems&actualsort=totaluniquesubscribers&p=1)
  - TaleWorlds itself added +15% map speed and a "faster map movement" modifier in v1.5β. — [CalradiaWar](https://calradiawar.com/news/bannerlord-v150-beta-update)
  - Launch-era critics flagged travel and busywork (TheSixthAxis, IGN). — [OpenCritic](https://opencritic.com/game/13880/mount-blade-ii-bannerlord)
- **War Sails UI:** troops had no icons at launch, which made army organization hard. — [TheGamer](https://www.thegamer.com/mount-and-blade-2-dlc-war-sails-mixed-reviews-bugs/)
- **Past-year trending QoL/UI mods (demand signal):**
  - Item Quality Indicator, UI Mount Types Summary, Mod Configuration Menu v5, Camp Menu
  - RTS Camera (1.3.x / 1.4.x)
  - Party AI Controls 1.3.X, Party AI Controls Reforged, FieldCamp Train-Forage-Ambush
  - Commander, GameMaster
  - Adjustable Leveling, Both Perks
  - Player Settlement, Recruit Exiled Clans
  - [Workshop trending, 365 days](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=trend&section=readytouseitems&actualsort=trend&days=365&p=1)
- **All-time demand:** Cheats (#28) and BetterExceptionWindow (#30), a crash-report UI. Crash diagnostics are wanted too. — [Workshop most subscribed](https://steamcommunity.com/workshop/browse/?appid=261550&browsesort=totaluniquesubscribers&section=readytouseitems&actualsort=totaluniquesubscribers&p=1)
- **Console-command and editing demand:** a 2026 poster wants console commands "restored or expanded" (e.g., changing age to keep companions longer). v1.3 hero Import/Export partly covers this. — [Steam "My Honest Review"](https://steamcommunity.com/app/261550/discussions/0/800092529560144927/), [MP1st](https://mp1st.com/title-updates-and-patches/mount-blade-2-bannerlord-update-1-026-1-25-out-war-sails-expansion)
- **Launch-era review (2022):** "the interface can be unwieldy" (Push Square). — [OpenCritic](https://opencritic.com/game/13880/mount-blade-ii-bannerlord)
- **Menu performance:** opening party/trader screens stuttered (2021, e1.5/1.6 era; from a search summary; exact thread and current state unverified). — [Steam thread, 2021](https://steamcommunity.com/app/261550/discussions/0/3075377162314807766)

### Inferences
- QoL demand is better measured by Workshop popularity than by review text. Players who would complain about UI install mods instead.
- High-value, low-risk QoL targets for an all-in-one mod:
  - a time-speed and wait manager (Better Time-like)
  - garrison and fief overview with auto-upgrade toggles and project ETAs (Improved Garrisons-like)
  - governor issue handling
  - a diplomacy and relations overview
  - a persistent, filterable event log with alerts for big events (war declarations, fief loss, defections, family events)
  - reliable lord-location tracking
  - party-screen bulk actions and sorting
  - item-quality indicators
  - a family-tree view
  - companion gear locking (stop companions selling gear)
  - in-game cheat/editor
  - a crash-report window

### Gaps
- I couldn't check whether vanilla v1.4.8 already has "Upgrade All", "Recruit All", item locking or inventory sorting natively. The Party Screen Enhancements data is from older e1.x-era builds. **Verify in the current build before scoping.**
- There was no reliable 2026 source on encyclopedia complaints, apart from the indirect Family Tree demand and diplomacy-overview gaps.
- There was no reliable source on fast-travel requests specifically. Better Time popularity and v1.5β speed changes are indirect evidence.
- Nexus Mods (endorsement rankings) returned 403, so Nexus-side demand couldn't be measured. Workshop rankings showed titles only, without subscriber counts.
