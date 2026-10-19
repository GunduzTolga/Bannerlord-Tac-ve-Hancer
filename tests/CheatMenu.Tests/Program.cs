using System;
using System.Collections.Generic;
using System.Linq;
using CheatMenu.Intrigue.Core;

namespace CheatMenu.Tests
{
    internal static class Program
    {
        private static int _failures;

        private static int Main()
        {
            // Temel sözleşme: pozitif tam sayı kabul, gerisi red.
            Expect("500", true, 500);
            Expect("  42  ", true, 42);
            Expect("", false);
            Expect("   ", false);
            Expect(null, false);
            Expect("abc", false);
            Expect("0", false);
            Expect("99999999999", false); // int taşması

            // NameRules: kırp, boş yalnızca allowEmpty ile, en fazla 50 karakter, { } yasak.
            ExpectName("Tolga", false, true, "Tolga");
            ExpectName("  Yeni Ad  ", false, true, "Yeni Ad");
            ExpectName("", false, false, "");
            ExpectName("   ", false, false, "");
            ExpectName("", true, true, "");
            ExpectName(null, true, true, "");
            ExpectName(new string('a', 50), false, true, new string('a', 50));
            ExpectName(new string('a', 51), false, false, "");
            ExpectName("Ad{X}", false, false, "");
            ExpectName("Cagri Sukru", false, true, "Cagri Sukru");

            // AmountParser.TryParseInRange: eksi destekli, [min, max] dahil.
            ExpectRange("-100", -100, 100, true, -100);
            ExpectRange("100", -100, 100, true, 100);
            ExpectRange("101", -100, 100, false);
            ExpectRange("-101", -100, 100, false);
            ExpectRange(" 5 ", 0, 10, true, 5);
            ExpectRange("0", 0, 10, true, 0);
            ExpectRange("+2", -2, 2, true, 2);
            ExpectRange("abc", 0, 10, false);
            ExpectRange("", 0, 10, false);
            ExpectRange(null, 0, 10, false);

            // ModSettings: okuma/yazma, izinli hızlar.
            Check("settings: null -> 4", ModSettings.Parse(null).FastForwardSpeed == 4);
            Check("settings: 8", ModSettings.Parse("FastForwardSpeed=8").FastForwardSpeed == 8);
            Check("settings: CRLF 16", ModSettings.Parse("FastForwardSpeed=16\r\n").FastForwardSpeed == 16);
            Check("settings: 32 not allowed -> 4", ModSettings.Parse("FastForwardSpeed=32").FastForwardSpeed == 4);
            Check("settings: garbage -> 4", ModSettings.Parse("FastForwardSpeed=abc").FastForwardSpeed == 4);
            Check("settings: comments/unknown keys", ModSettings.Parse("# yorum\nFoo=1\nFastForwardSpeed=8").FastForwardSpeed == 8);
            var roundTrip = new ModSettings();
            roundTrip.SetFastForwardSpeed(16);
            Check("settings: round trip", ModSettings.Parse(roundTrip.Format()).FastForwardSpeed == 16);
            Check("settings: skill xp 5", ModSettings.Parse("SkillXpMultiplier=5").SkillXpMultiplier == 5);
            Check("settings: skill xp 4 not allowed -> 1", ModSettings.Parse("SkillXpMultiplier=4").SkillXpMultiplier == 1);
            Check("settings: troop xp 10", ModSettings.Parse("TroopXpMultiplier=10").TroopXpMultiplier == 10);
            Check("settings: party size 200", ModSettings.Parse("PartySizeBonus=200").PartySizeBonus == 200);
            Check("settings: party size -50 -> 0", ModSettings.Parse("PartySizeBonus=-50").PartySizeBonus == 0);
            Check("settings: no learning limit 1", ModSettings.Parse("NoLearningLimit=1").NoLearningLimit);
            Check("settings: no learning limit true", ModSettings.Parse("NoLearningLimit=true").NoLearningLimit);
            Check("settings: no learning limit 0", !ModSettings.Parse("NoLearningLimit=0").NoLearningLimit);
            Check("settings: no companion limit 1", ModSettings.Parse("NoCompanionLimit=1").NoCompanionLimit);
            Check("settings: no companion limit default off", !ModSettings.Parse("NoLearningLimit=1").NoCompanionLimit);
            Check("settings: no companion limit garbage -> off", !ModSettings.Parse("NoCompanionLimit=evet").NoCompanionLimit);
            Check("settings: party limit bonus 7", ModSettings.Parse("PartyLimitBonus=7").PartyLimitBonus == 7);
            Check("settings: party limit bonus 100", ModSettings.Parse("PartyLimitBonus=100").PartyLimitBonus == 100);
            Check("settings: party limit bonus 101 -> 0", ModSettings.Parse("PartyLimitBonus=101").PartyLimitBonus == 0);
            Check("settings: party limit bonus -3 -> 0", ModSettings.Parse("PartyLimitBonus=-3").PartyLimitBonus == 0);
            Check("settings: no party limit 1", ModSettings.Parse("NoPartyLimit=1").NoPartyLimit);
            Check("settings: no party limit default off", !ModSettings.Parse("NoCompanionLimit=1").NoPartyLimit);
            Check("settings: intrigue difficulty default normal", ModSettings.Parse("").IntrigueDifficulty == SchemeMath.NormalDifficulty);
            Check("settings: intrigue difficulty hard", ModSettings.Parse("IntrigueDifficulty=2").IntrigueDifficulty == SchemeMath.HardDifficulty);
            Check("settings: intrigue difficulty 7 -> normal", ModSettings.Parse("IntrigueDifficulty=7").IntrigueDifficulty == 1);
            Check("settings: ai intrigue default few", ModSettings.Parse("").AiIntrigue == SchemeMath.AiFew);
            Check("settings: ai intrigue off", ModSettings.Parse("AiIntrigue=0").AiIntrigue == SchemeMath.AiOff);
            Check("settings: player targeted default off", !ModSettings.Parse("").PlayerCanBeTargeted);
            Check("settings: map button default top center", ModSettings.Parse("").MapButtonPosition == ModSettings.DefaultMapButtonPosition);
            Check("settings: map button hidden", ModSettings.Parse("MapButtonPosition=0").MapButtonPosition == 0);
            Check("settings: map button 9 -> default", ModSettings.Parse("MapButtonPosition=9").MapButtonPosition == ModSettings.DefaultMapButtonPosition);
            Check("settings: hotkey default Home", ModSettings.ScreenHotkeyNames[ModSettings.Parse("").ScreenHotkey] == "Home");
            Check("settings: hotkey none", ModSettings.Parse("ScreenHotkey=0").ScreenHotkey == 0);
            Check("settings: hotkey 99 -> default", ModSettings.Parse("ScreenHotkey=99").ScreenHotkey == ModSettings.DefaultScreenHotkey);
            Check("settings: event cards default on", ModSettings.Parse("").EventCards);
            Check("settings: event cards off", !ModSettings.Parse("EventCards=0").EventCards);
            Check("settings: romance secrets default on", ModSettings.Parse("").RomanceSecrets);
            Check("settings: romance secrets off", !ModSettings.Parse("RomanceSecrets=0").RomanceSecrets);
            Check("settings: night houses default off", !ModSettings.Parse("").NightHouses);
            Check("settings: night houses on", ModSettings.Parse("NightHouses=1").NightHouses);
            Check("settings: spouse infidelity default on", ModSettings.Parse("").SpouseInfidelity);
            Check("settings: spouse infidelity off", !ModSettings.Parse("SpouseInfidelity=0").SpouseInfidelity);
            Check("settings: tail in scene default off", !ModSettings.Parse("").TailInScene);
            Check("settings: tail in scene on", ModSettings.Parse("TailInScene=1").TailInScene);
            var full = new ModSettings();
            full.SetFastForwardSpeed(8);
            full.SetSkillXpMultiplier(3);
            full.SetTroopXpMultiplier(5);
            full.SetPartySizeBonus(100);
            full.NoLearningLimit = true;
            full.NoCompanionLimit = true;
            full.SetPartyLimitBonus(4);
            full.NoPartyLimit = true;
            full.SetIntrigueDifficulty(0);
            full.SetAiIntrigue(2);
            full.PlayerCanBeTargeted = true;
            full.SetMapButtonPosition(3);
            full.SetScreenHotkey(5);
            full.EventCards = false;
            full.RomanceSecrets = false;
            full.NightHouses = true;
            full.SpouseInfidelity = false;
            full.TailInScene = true;
            ModSettings back = ModSettings.Parse(full.Format());
            Check("settings: full round trip", back.FastForwardSpeed == 8 && back.SkillXpMultiplier == 3
                && back.TroopXpMultiplier == 5 && back.PartySizeBonus == 100 && back.NoLearningLimit && back.NoCompanionLimit
                && back.PartyLimitBonus == 4 && back.NoPartyLimit && back.IntrigueDifficulty == 0
                && back.AiIntrigue == 2 && back.PlayerCanBeTargeted && back.MapButtonPosition == 3 && back.ScreenHotkey == 5
                && !back.EventCards && !back.RomanceSecrets && back.NightHouses && !back.SpouseInfidelity && back.TailInScene);

            // TextRecords: kaçışlı satır biçimi, bozuk metne dayanıklılık.
            string tricky = "a\tb\nc\\d=e\r";
            var written = new List<TextRecord>
            {
                new TextRecord("one").Add("k", tricky).Add("empty", ""),
                new TextRecord("two").Add("x", "1"),
            };
            List<TextRecord> read = TextRecords.Parse(TextRecords.Write(written));
            Check("records: count", read.Count == 2);
            Check("records: escaped round trip", read.Count == 2 && read[0].Get("k") == tricky && read[0].Get("empty") == "");
            Check("records: second kind", read.Count == 2 && read[1].Kind == "two" && read[1].Get("x") == "1");
            Check("records: missing field -> null", read.Count == 2 && read[1].Get("nope") == null);
            Check("records: null -> empty", TextRecords.Parse(null).Count == 0);
            List<TextRecord> messy = TextRecords.Parse("\n\t=x\nbad\t=\tnoequals\t=y\r\nok\tk=v");
            Check("records: garbage tolerated", messy.Count == 2 && messy[0].Fields.Count == 0 && messy[1].Get("k") == "v");
            Check("records: trailing backslash kept", TextRecords.Parse("k\tv=a\\")[0].Get("v") == "a\\");

            // IntrigueState: kayıt testi işareti gidiş-dönüş; bozuk/boş metin boş duruma döner.
            var state = new IntrigueState { SaveTestMarker = "gün 12" };
            Check("intrigue: marker round trip", IntrigueState.Parse(state.Write()).SaveTestMarker == "gün 12");
            Check("intrigue: version written", state.Write().StartsWith("version\tv=" + IntrigueState.CurrentVersion));
            Check("intrigue: null -> empty", IntrigueState.Parse(null).SaveTestMarker == null);
            Check("intrigue: garbage -> empty", IntrigueState.Parse("çöp\\x\n\t\t").SaveTestMarker == null);

            IntrigueMathTests();
            IntrigueE2MathTests();
            IntrigueStateTests();
            TurkishSuffixTests();
            CardImageTests();
            InterrogationTests();
            LetterTests();
            RomanceTests();
            CrisisTests();
            HouseTests();
            RecruitTests();
            CourtshipTests();
            FidelityTests();
            DuelTests();
            PetitionTests();
            HostageTests();
            TailTests();
            TreasuryTests();
            PhysicianTests();
            MarshalTests();
            StewardTests();
            Check("dynasty: legitimize costs", DynastyRules.LegitimizeInfluenceCost(true) == 50 && DynastyRules.LegitimizeGoldCost(true) == 0
                && DynastyRules.LegitimizeInfluenceCost(false) == 0 && DynastyRules.LegitimizeGoldCost(false) == 2000);

            Console.WriteLine(_failures == 0 ? "ALL TESTS PASSED" : _failures + " TEST(S) FAILED");
            return _failures == 0 ? 0 : 1;
        }

        // Tasarım 5.4'teki örnek hesaplar. Bu testler bozulursa ya formül değişti ya da tasarım tablosu güncellenmeli.
        private static void IntrigueMathTests()
        {
            SchemeInputs early = new SchemeInputs { Roguery = 40, TargetRoguery = 30, TargetClanTier = 2 };
            CheckChance("murder A 2 phases", SchemeMath.MurderSuccess(Phases(early, 2)), 9);
            CheckChance("murder A 5 phases", SchemeMath.MurderSuccess(Phases(early, 5)), 27);
            early.HasReport = true;
            CheckChance("murder A 5 phases + report", SchemeMath.MurderSuccess(Phases(early, 5)), 37);
            CheckChance("discovery A", SchemeMath.MurderDiscovery(early), 11);

            SchemeInputs mid = new SchemeInputs
            {
                Roguery = 120, Calculating = 1, HasReport = true,
                TargetRoguery = 60, TargetClanTier = 4, TargetIsClanLeader = true, TargetInOwnSettlement = true,
            };
            CheckChance("murder B 2 phases", SchemeMath.MurderSuccess(Phases(mid, 2)), 9);
            CheckChance("murder B 5 phases", SchemeMath.MurderSuccess(Phases(mid, 5)), 27);
            mid.BribeLevel = 2;
            CheckChance("murder B 5 phases + medium bribe", SchemeMath.MurderSuccess(Phases(mid, 5)), 37);
            CheckChance("discovery B", SchemeMath.MurderDiscovery(mid), 9);

            SchemeInputs late = new SchemeInputs
            {
                Roguery = 250, Calculating = 2, HasReport = true,
                TargetRoguery = 100, TargetCalculating = 1, TargetClanTier = 6,
                TargetIsClanLeader = true, TargetIsRuler = true, TargetInOwnSettlement = true,
            };
            CheckChance("murder C 2 phases (floor)", SchemeMath.MurderSuccess(Phases(late, 2)), 5);
            CheckChance("murder C 5 phases", SchemeMath.MurderSuccess(Phases(late, 5)), 19);
            late.BribeLevel = 3;
            CheckChance("murder C 5 phases + big bribe", SchemeMath.MurderSuccess(Phases(late, 5)), 34);
            CheckChance("discovery C", SchemeMath.MurderDiscovery(late), 7);

            Check("cost A", SchemeMath.StartCost(SchemeKind.Murder, 2, false, false) == 2500);
            Check("cost B", SchemeMath.StartCost(SchemeKind.Murder, 4, true, false) == 5000);
            Check("cost C", SchemeMath.StartCost(SchemeKind.Murder, 6, true, true) == 13000);
            Check("bribe C big", SchemeMath.BribeCost(13000, 3) == 26000);
            Check("spy cost tier 3", SchemeMath.StartCost(SchemeKind.Spy, 3, true, false) == 1250);

            // Sınırlar
            SchemeInputs best = new SchemeInputs
            {
                Roguery = 300, Calculating = 2, HasReport = true, CompletedPhases = 5, EventSuccess = 99, BribeLevel = 3,
                TargetIsPlayersCaptive = true, Difficulty = SchemeMath.EasyDifficulty,
            };
            CheckChance("murder cap 85", SchemeMath.MurderSuccess(best), 85);
            best.TargetIsRuler = true;
            CheckChance("murder ruler cap 70", SchemeMath.MurderSuccess(best), 70);
            CheckChance("discovery floor 2", SchemeMath.MurderDiscovery(best), 2);
            SchemeInputs worst = new SchemeInputs
            {
                TargetRoguery = 300, TargetCalculating = 2, TargetClanTier = 6, EventDiscovery = 30, Difficulty = SchemeMath.HardDifficulty,
            };
            CheckChance("discovery cap 40", SchemeMath.MurderDiscovery(worst), 40);
            CheckChance("spy floor 20", SchemeMath.SpySuccess(worst), 20);
            CheckChance("spy discovery cap 25", SchemeMath.SpyDiscovery(worst), 25);
            CheckChance("spy unmet -10", SchemeMath.SpySuccess(new SchemeInputs { HasMet = false }), 40);
            Check("event success capped at 15", SchemeMath.MurderSuccess(new SchemeInputs { EventSuccess = 50 })
                == SchemeMath.MurderSuccess(new SchemeInputs { EventSuccess = 15 }));

            Check("phase days murder r0 = 7", SchemeMath.PhaseDays(SchemeKind.Murder, 0) == 7);
            Check("phase days murder r120 = 5", SchemeMath.PhaseDays(SchemeKind.Murder, 120) == 5);
            Check("phase days murder r300 = 4", SchemeMath.PhaseDays(SchemeKind.Murder, 300) == 4);
            Check("phase days spy r0 = 5", SchemeMath.PhaseDays(SchemeKind.Spy, 0) == 5);
            Check("phase days spy r300 = 3", SchemeMath.PhaseDays(SchemeKind.Spy, 300) == 3);
            Check("max schemes 1/2/3", SchemeMath.MaxActiveSchemes(124) == 1 && SchemeMath.MaxActiveSchemes(125) == 2
                && SchemeMath.MaxActiveSchemes(225) == 3);
            Check("secret chance", SchemeMath.SecretChance(0, 1) == 30 && SchemeMath.SecretChance(300, 1) == 80
                && SchemeMath.SecretChance(300, 2) == 95);
            Check("trace chance", SchemeMath.TraceChance(10, false, false) == 20 && SchemeMath.TraceChance(10, true, false) == 40
                && SchemeMath.TraceChance(10, false, true) == 10 && SchemeMath.TraceChance(40, true, false) == 90);
            Check("caught chance", SchemeMath.CaughtOnFailureChance(0) == 50 && SchemeMath.CaughtOnFailureChance(300) == 20);
            Check("opportunity", SchemeMath.Opportunity(SchemeKind.Murder, 1) == 0 && SchemeMath.Opportunity(SchemeKind.Murder, 4) == 12
                && SchemeMath.Opportunity(SchemeKind.Murder, 9) == 18 && SchemeMath.Opportunity(SchemeKind.Spy, 2) == 0);

            // Tohumlu zar: aynı tohum + sayaç aynı sonucu verir; dağılım kabaca düzgün.
            int c1 = 0, c2 = 0;
            Check("dice deterministic", SchemeRandom.Roll100(42, ref c1) == SchemeRandom.Roll100(42, ref c2) && c1 == 1);
            int counter = 0, low = 0, min = 100, max = 1;
            for (int i = 0; i < 10000; i++)
            {
                int roll = SchemeRandom.Roll100(7, ref counter);
                if (roll <= 30) low++;
                min = Math.Min(min, roll);
                max = Math.Max(max, roll);
            }
            Check("dice range 1..100", min == 1 && max == 100);
            Check("dice ~30% <= 30 (" + low + ")", low > 2700 && low < 3300);
        }

        private static void IntrigueE2MathTests()
        {
            // Kaçırma = suikast -5, kaçırmanın keşfi = suikast +3 (tasarım 15)
            SchemeInputs mid = new SchemeInputs
            {
                Roguery = 120, Calculating = 1, HasReport = true, CompletedPhases = 4,
                TargetRoguery = 60, TargetClanTier = 4, TargetIsClanLeader = true,
            };
            CheckChance("kidnap mid 4 phases", SchemeMath.KidnapSuccess(mid), SchemeMath.MurderSuccess(Phases(Copy(mid), 4)) - 5);
            CheckChance("kidnap discovery mid", SchemeMath.KidnapDiscovery(mid), SchemeMath.MurderDiscovery(mid) + 3);
            CheckChance("kidnap opportunity capped at 2 phases", SchemeMath.Opportunity(SchemeKind.Kidnap, 9), 12);
            SchemeInputs ruler = new SchemeInputs { Roguery = 300, Calculating = 2, HasReport = true, CompletedPhases = 4, EventSuccess = 15,
                BribeLevel = 3, NetworkSuccess = 15, HasHook = true, TargetIsRuler = true, Difficulty = SchemeMath.EasyDifficulty };
            CheckChance("kidnap ruler cap 60", SchemeMath.KidnapSuccess(ruler), 60);

            // Ağ ve koz suikasta eklenir
            SchemeInputs plain = new SchemeInputs { Roguery = 100, TargetClanTier = 3 };
            int baseChance = SchemeMath.MurderSuccess(plain);
            plain.NetworkSuccess = 15;
            plain.HasHook = true;
            CheckChance("network + hook +25", SchemeMath.MurderSuccess(plain), baseChance + 25);
            plain.NetworkDiscovery = 6;
            CheckChance("network discovery -6", SchemeMath.MurderDiscovery(plain), SchemeMath.MurderDiscovery(new SchemeInputs { Roguery = 100, TargetClanTier = 3 }) - 6);

            // Şantaj: 40 + R/5 - TR/10 - kademe*2 - hükümdar 10 + sır/2 (+15 2. aşamada)
            SchemeInputs blackmail = new SchemeInputs { Roguery = 50, TargetRoguery = 40, TargetClanTier = 3, SecretSeverity = 30, CompletedPhases = 1 };
            CheckChance("blackmail 1 phase", SchemeMath.BlackmailSuccess(blackmail), 55);
            blackmail.CompletedPhases = 2;
            CheckChance("blackmail 2 phases", SchemeMath.BlackmailSuccess(blackmail), 70);
            CheckChance("blackmail discovery", SchemeMath.BlackmailDiscovery(blackmail), 8);
            CheckChance("blackmail floor 15", SchemeMath.BlackmailSuccess(new SchemeInputs { TargetRoguery = 300, TargetClanTier = 6,
                TargetIsRuler = true, Difficulty = SchemeMath.HardDifficulty }), 15);

            Check("costs E2", SchemeMath.StartCost(SchemeKind.Blackmail, 3, true, false) == 900
                && SchemeMath.StartCost(SchemeKind.Kidnap, 2, false, false) == 2000
                && SchemeMath.StartCost(SchemeKind.Kidnap, 6, true, true) == 10400);
            Check("phases E2", SchemeRules.MinPhases(SchemeKind.Kidnap) == 2 && SchemeRules.MaxPhases(SchemeKind.Kidnap) == 4
                && SchemeRules.MinPhases(SchemeKind.Blackmail) == 1 && SchemeRules.MaxPhases(SchemeKind.Blackmail) == 2
                && SchemeRules.IsStrike(SchemeKind.Kidnap) && !SchemeRules.IsStrike(SchemeKind.Blackmail));

            // Gölge ağı
            Check("network bonus", SchemeMath.NetworkSuccess(3, true) == 15 && SchemeMath.NetworkSuccess(3, false) == 7
                && SchemeMath.NetworkDiscovery(2, true) == 4 && SchemeMath.NetworkDiscovery(2, false) == 2);
            Check("network cost/relation", SchemeMath.NetworkCost(1) == 3000 && SchemeMath.NetworkCost(3) == 12000
                && SchemeMath.NetworkRelationRequired(1) == 0 && SchemeMath.NetworkRelationRequired(3) == 20);
            Check("max networks", SchemeMath.MaxNetworks(0) == 1 && SchemeMath.MaxNetworks(150) == 3);

            // Yapay zekâ
            Check("ai weekly chance", SchemeMath.AiPlotWeeklyChance(3, SchemeMath.AiFew) == 9 && SchemeMath.AiPlotWeeklyChance(3, SchemeMath.AiNormal) == 18
                && SchemeMath.AiPlotWeeklyChance(10, SchemeMath.AiNormal) == 25 && SchemeMath.AiPlotWeeklyChance(5, SchemeMath.AiOff) == 0);
            Check("ai max plots", SchemeMath.AiMaxPlots(SchemeMath.AiOff) == 0 && SchemeMath.AiMaxPlots(SchemeMath.AiFew) == 1
                && SchemeMath.AiMaxPlots(SchemeMath.AiNormal) == 2);
            Check("detection", SchemeMath.DetectionChance(false, 300) == 5 && SchemeMath.DetectionChance(true, 0) == 20
                && SchemeMath.DetectionChance(true, 100) == 40 && SchemeMath.DetectionChance(true, 400) == 90);
        }

        private static SchemeInputs Copy(SchemeInputs i) => (SchemeInputs)typeof(SchemeInputs)
            .GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(i, null);

        private static void IntrigueStateTests()
        {
            var state = new IntrigueState { SpymasterId = "spc_x", NextSchemeId = 4, Seed = 99, Rolls = 3, SaveTestMarker = "gün 5" };
            state.Schemes.Add(new Scheme
            {
                Id = 3, Kind = SchemeKind.Murder, TargetId = "lord_1_1", StartDay = 10, Phase = 2, DaysIntoPhase = 3, PhaseDays = 6,
                StartCost = 2500, Seed = -12345, Rolls = 7, EventSuccess = 8, EventDiscovery = -3, UsedEvents = { 1, 3 },
                Method = SchemeMethod.Dagger, FeastRisk = true, PendingEvent = 5, AskedToExecute = true,
            });
            state.Reports.Add(new IntelReport
            {
                TargetId = "lord_1_1", TargetName = "Derthert", Day = 12, Location = "Pravend", Party = "120 asker",
                Gold = "~20.000", Relations = "hükümdar: 10\tsen: -5", Traits = "Onur +1",
            });
            state.Secrets.Add(new Secret { TargetId = "lord_1_1", Kind = SecretKind.Bribery, Day = 12, Exposed = true });
            state.Secrets.Add(new Secret { TargetId = "lord_4", Kind = SecretKind.Bastard, Day = 30, OtherId = "lord_5", ChildId = "lord_6", Exposed = true, Whispered = true });
            state.CooldownUntil["lord_2"] = 200;
            state.AlertUntil["lord_3"] = 300;
            state.Notices.Add(new Notice { Title = "Başlık", Text = "satır 1\nsatır 2", HeroId = "lord_1_1", Image = "sonuc_ifsa" });
            state.Notices.Add(new Notice { Title = "Resimsiz", Text = "t" });
            state.LiegeDemand = new LiegeDemand { KingdomId = "vlandia", VictimName = "X", Gold = 10000, Influence = 100 };
            state.AddLog(12, "olay");
            state.NextPlotId = 6;
            state.Hooks.Add(new Hook { TargetId = "lord_4", Day = 20, Until = 104 });
            state.AiPlots.Add(new AiPlot
            {
                Id = 5, PlotterId = "lord_5", TargetId = "spc_wife", Kind = SchemeKind.Kidnap, StartDay = 30, ResolveDay = 55,
                Seed = 77, Rolls = 2, Detected = true, Asked = false, Protected = true,
            });
            state.Networks.Add(new ShadowNetwork { SettlementId = "town_V1", Level = 2, Since = 40 });
            state.FeudUntil["lord_6"] = 400;
            state.InterrogatedUntil["lord_7"] = 230;
            state.InterrogatedUntil["lord_8"] = 300;
            state.Letters.Add(new Letter { Id = 4, Kind = LetterKind.HookDemand, TargetId = "lord_9", Use = 1, SentDay = 100, ArriveDay = 104 });
            state.Letters.Add(new Letter { Id = 5, Kind = LetterKind.HookDemand, TargetId = "lord_10", Use = 2, SentDay = 101, ArriveDay = 103 });
            state.NextLetterId = 7;
            state.Audiences.Add(new Audience { HeroId = "lord_13", Until = 245, Relation = 3, Asked = true });
            state.Audiences.Add(new Audience { HeroId = "lord_14", Until = 260 });
            state.GiftUntil["lord_13"] = 248;
            state.Tension["vlandia"] = 55;
            state.Tension["empire"] = 0;
            state.RecruitUntil["lord_7"] = 210;
            state.Courtships.Add(new Courtship { Id = 4, TargetId = "lord_8", AgentId = "main_hero_spouse", Mode = CourtshipMode.Intel, Stage = 2,
                DaysIntoStage = 3, StartDay = 150, Careful = true });
            state.Lovers.Add(new Lover { HeroId = "lord_9", Since = 160, NextMeetingDay = 174, Public = true });
            state.Pregnancies.Add(new Pregnancy { MotherId = "lord_9", FatherId = "main_hero", DueDay = 196 });
            state.Pregnancies.Add(new Pregnancy { MotherId = "spouse_1", FatherId = "lord_11", DueDay = 230, Hidden = true });
            state.SpouseAffair = new SpouseAffair { SpouseId = "spouse_1", LoverId = "lord_11", Since = 120, NextMeetingDay = 190, Discovered = true,
                How = "Casus başın buldu.", ChildIds = { "child_1", "child_2" } };
            state.SpouseWatchUntil = 205;
            state.NoChildren.Add("lord_12");
            state.DuelUntil["lord_14"] = 240;
            state.CourtDayUntil["town_V1"] = 250;
            state.TailUntil["lord_16"] = 260;
            state.TreasurerId = "companion_1";
            state.PhysicianId = "companion_2";
            state.MarshalId = "lord_17";
            state.StewardId = "companion_3";
            state.StewardWarned["town_V1|isyan"] = 320;
            state.MarshalWarned["party_3|town_V1"] = 310;
            state.PhysicianWarnUntil = 300;
            state.Conceived["spouse_1"] = 275;
            state.Embezzled = 450;
            state.EmbezzleFound = true;
            state.CaravanBooks.Add(new CaravanBook { PartyId = "caravan_1", TownId = "town_V1", Worth = 5400, Day = 270, SentHome = 120 });
            state.CaravanLegs.Add(new CaravanLeg { PartyId = "caravan_1", FromId = "town_V2", ToId = "town_V1", Profit = -80, Days = 4, Day = 270 });
            state.Hostages.Add(new HostageDeal { KingdomId = "vlandia", GivenId = "child_3", HeldId = "lord_15", Since = 100, Until = 184, Fostered = 6 });
            state.Hostages.Add(new HostageDeal { KingdomId = "sturgia", GivenId = "child_4", Since = 120, Until = 204 });
            state.AgeFrozen.Add("main_hero");
            state.AgeFrozen.Add("lord_13");
            state.SpouseApartWeeks = 3;
            state.RomanceUntil["lord_10"] = 220;
            state.NextCourtshipId = 5;
            state.Houses.Add(new House { TownId = "town_V1", Staff = 5, Share = HouseShare.Fair, Since = 120 });
            state.Crises.Add(new Crisis { KingdomId = "vlandia", TownId = "town_V1", Level = 2, Day = 140, Crime = 30, OwnerId = "lord_4_1" });
            state.Letters.Add(new Letter { Id = 6, Kind = LetterKind.Audience, TargetId = "lord_15", Use = 3000, SentDay = 102, ArriveDay = 105 });
            state.Mission = SpymasterMission.SpreadRumors;
            state.RumorTargetId = "lord_12";

            IntrigueState back = IntrigueState.Parse(state.Write());
            Scheme s = back.Schemes.Count == 1 ? back.Schemes[0] : null;
            Check("state: meta", back.SpymasterId == "spc_x" && back.NextSchemeId == 4 && back.Seed == 99 && back.Rolls == 3
                && back.SaveTestMarker == "gün 5");
            Check("state: scheme", s != null && s.Id == 3 && s.Kind == SchemeKind.Murder && s.TargetId == "lord_1_1"
                && s.StartDay == 10 && s.Phase == 2 && s.DaysIntoPhase == 3 && s.PhaseDays == 6 && s.StartCost == 2500
                && s.Seed == -12345 && s.Rolls == 7 && s.EventSuccess == 8 && s.EventDiscovery == -3
                && s.UsedEvents.Count == 2 && s.UsedEvents[1] == 3 && s.Method == SchemeMethod.Dagger
                && s.FeastRisk && s.PendingEvent == 5 && !s.AwaitingChoice && s.AskedToExecute);
            IntelReport r = back.Reports.Count == 1 ? back.Reports[0] : null;
            Check("state: report", r != null && r.TargetName == "Derthert" && r.Day == 12 && r.Relations == "hükümdar: 10\tsen: -5");
            Check("state: secret", back.Secrets.Count == 2 && back.Secrets[0].Kind == SecretKind.Bribery && back.Secrets[0].Exposed
                && back.Secrets[0].OtherId == null && back.Secrets[0].ChildId == null && !back.Secrets[0].Whispered);
            Check("state: romance secret", back.Secrets.Count == 2 && back.Secrets[1].Kind == SecretKind.Bastard && back.Secrets[1].OtherId == "lord_5"
                && back.Secrets[1].ChildId == "lord_6" && back.Secrets[1].Whispered && back.Secrets[1].Exposed);
            Check("state: cooldown/alert", back.CooldownUntil["lord_2"] == 200 && back.AlertUntil["lord_3"] == 300);
            Check("state: notice", back.Notices.Count == 2 && back.Notices[0].Text == "satır 1\nsatır 2" && back.Notices[0].HeroId == "lord_1_1");
            Check("state: notice image", back.Notices.Count == 2 && back.Notices[0].Image == "sonuc_ifsa" && back.Notices[1].Image == null
                && back.Notices[1].HeroId == null);
            Check("state: liege", back.LiegeDemand != null && back.LiegeDemand.Gold == 10000 && back.LiegeDemand.KingdomId == "vlandia");
            Check("state: log", back.Log.Count == 1 && back.Log[0].Text == "olay");
            Check("state: hook", back.Hooks.Count == 1 && back.ActiveHook("lord_4", 103) != null && back.ActiveHook("lord_4", 104) == null);
            AiPlot plot = back.AiPlots.Count == 1 ? back.AiPlots[0] : null;
            Check("state: ai plot", plot != null && plot.Id == 5 && plot.PlotterId == "lord_5" && plot.TargetId == "spc_wife"
                && plot.Kind == SchemeKind.Kidnap && plot.StartDay == 30 && plot.ResolveDay == 55 && plot.Seed == 77 && plot.Rolls == 2
                && plot.Detected && !plot.Asked && plot.Protected && back.NextPlotId == 6);
            Check("state: network", back.Network("town_V1") != null && back.Network("town_V1").Level == 2 && back.Network("town_V1").Since == 40);
            Check("state: feud", back.HasFeud("lord_6", 399) && !back.HasFeud("lord_6", 400));
            Check("state: interrogated", back.InterrogatedUntil.Count == 2 && back.InterrogatedUntil["lord_7"] == 230);
            Check("state: audiences", back.Audiences.Count == 2 && back.Audiences[0].Relation == 3 && back.Audiences[0].Asked
                && !back.Audiences[1].Asked && back.GiftUntil["lord_13"] == 248);
            Courtship ct = back.Courtships.Count == 1 ? back.Courtships[0] : null;
            Check("state: courtship", ct != null && ct.Id == 4 && ct.TargetId == "lord_8" && ct.AgentId == "main_hero_spouse"
                && ct.Mode == CourtshipMode.Intel && ct.Stage == 2 && ct.DaysIntoStage == 3 && ct.StartDay == 150 && ct.Careful && back.NextCourtshipId == 5);
            Check("state: lover", back.Lovers.Count == 1 && back.LoverOf("lord_9") != null && back.Lovers[0].Since == 160
                && back.Lovers[0].NextMeetingDay == 174 && back.Lovers[0].Public);
            Check("state: pregnancy", back.Pregnancies.Count == 2 && back.Pregnancies[0].MotherId == "lord_9" && back.Pregnancies[0].FatherId == "main_hero"
                && back.Pregnancies[0].DueDay == 196 && !back.Pregnancies[0].Hidden && back.Pregnancies[1].Hidden);
            SpouseAffair sa = back.SpouseAffair;
            Check("state: spouse affair", sa != null && sa.SpouseId == "spouse_1" && sa.LoverId == "lord_11" && sa.Since == 120 && sa.NextMeetingDay == 190
                && sa.Discovered && !sa.Asked && sa.How == "Casus başın buldu." && sa.ChildIds.Count == 2 && sa.ChildIds[1] == "child_2");
            Check("state: hostages", back.Hostages.Count == 2 && back.HostageDealWith("vlandia")?.HeldId == "lord_15"
                && back.HostageDealWith("vlandia").Fostered == 6 && back.HostageDealWith("vlandia").Until == 184
                && back.HostageDealWith("sturgia")?.HeldId == null && back.HostageDealWith("sturgia").GivenId == "child_4" && back.HostageDealWith("x") == null);
            Check("state: court day", back.CourtDayUntil.Count == 1 && back.CourtDayUntil["town_V1"] == 250);
            Check("state: tail cooldown", back.TailUntil.Count == 1 && back.TailUntil["lord_16"] == 260);
            Check("state: treasurer", back.TreasurerId == "companion_1" && back.Embezzled == 450 && back.EmbezzleFound);
            Check("state: steward", back.StewardId == "companion_3" && back.StewardWarned.Count == 1 && back.StewardWarned["town_V1|isyan"] == 320);
            Check("state: marshal", back.MarshalId == "lord_17" && back.MarshalWarned.Count == 1 && back.MarshalWarned["party_3|town_V1"] == 310);
            Check("state: physician", back.PhysicianId == "companion_2" && back.PhysicianWarnUntil == 300
                && back.Conceived.Count == 1 && back.Conceived["spouse_1"] == 275);
            Check("state: caravan book", back.CaravanBooks.Count == 1 && back.CaravanBooks[0].PartyId == "caravan_1" && back.CaravanBooks[0].TownId == "town_V1"
                && back.CaravanBooks[0].Worth == 5400 && back.CaravanBooks[0].Day == 270 && back.CaravanBooks[0].SentHome == 120);
            Check("state: caravan leg", back.CaravanLegs.Count == 1 && back.CaravanLegs[0].FromId == "town_V2" && back.CaravanLegs[0].ToId == "town_V1"
                && back.CaravanLegs[0].Profit == -80 && back.CaravanLegs[0].Days == 4 && back.CaravanLegs[0].Day == 270 && back.CaravanLegs[0].PartyId == "caravan_1");
            Check("state: duel cooldown", back.DuelUntil.Count == 1 && back.DuelUntil["lord_14"] == 240);
            Check("state: family editor", back.NoChildren.SetEquals(new[] { "lord_12" }) && back.AgeFrozen.SetEquals(new[] { "main_hero", "lord_13" }));
            Check("state: spouse watch", back.SpouseWatchUntil == 205 && back.SpouseApartWeeks == 3);
            Check("state: romance cooldown", back.RomanceUntil["lord_10"] == 220);
            Check("state: recruit cooldown", back.RecruitUntil.Count == 1 && back.RecruitUntil["lord_7"] == 210);
            Check("state: house", back.Houses.Count == 1 && back.HouseIn("town_V1") != null && back.Houses[0].Staff == 5
                && back.Houses[0].Share == HouseShare.Fair && back.Houses[0].Since == 120 && back.HouseIn("town_X") == null);
            Check("state: tension", back.TensionWith("vlandia") == 55 && !back.Tension.ContainsKey("empire") && back.TensionWith("nope") == 0);
            Check("state: crisis", back.Crises.Count == 1 && back.Crises[0].KingdomId == "vlandia" && back.Crises[0].TownId == "town_V1"
                && back.Crises[0].Level == 2 && back.Crises[0].Day == 140 && back.Crises[0].Crime == 30 && back.Crises[0].OwnerId == "lord_4_1");
            Check("state: audience letter", back.Letters.Count == 3 && back.Letters[2].Kind == LetterKind.Audience && back.Letters[2].Use == 3000);
            Check("state: letters", back.Letters.Count == 3 && back.NextLetterId == 7 && back.Letters[0].Kind == LetterKind.HookDemand
                && back.Letters[0].ArriveDay == 104 && back.Letters[1].Use == 2 && back.Letters[1].TargetId == "lord_10");
            Check("state: mission", back.Mission == SpymasterMission.SpreadRumors && back.RumorTargetId == "lord_12");
            Check("state: unknown letter kind dropped", IntrigueState.Parse("letter\tid=1\tkind=9\ttarget=x\tarrive=5").Letters.Count == 0);
            Check("state: mission default", IntrigueState.Parse("").Mission == SpymasterMission.None);

            Check("state: report valid at 72", back.HasValidReport("lord_1_1", 72) && !back.HasValidReport("lord_1_1", 73));
            Check("state: cooldown", back.IsOnCooldown("lord_2", 199) && !back.IsOnCooldown("lord_2", 200));
            back.RemoveExpired(250);
            Check("state: expired removed", !back.CooldownUntil.ContainsKey("lord_2") && back.AlertUntil.ContainsKey("lord_3")
                && back.Hooks.Count == 0 && back.FeudUntil.ContainsKey("lord_6")
                && !back.InterrogatedUntil.ContainsKey("lord_7") && back.InterrogatedUntil.ContainsKey("lord_8")
                && back.Letters.Count == 3 && back.Audiences.Count == 1 && back.Audiences[0].HeroId == "lord_14"
                && !back.GiftUntil.ContainsKey("lord_13"));

            int logged = IntrigueState.MaxLogEntries + 10;
            for (int i = 0; i < logged; i++) back.AddLog(i, "e" + i);
            Check("state: log capped", back.Log.Count == IntrigueState.MaxLogEntries && back.Log[back.Log.Count - 1].Text == "e" + (logged - 1));

            IntrigueState bad = IntrigueState.Parse("scheme\tkind=9\ttarget=x\nscheme\tkind=2\nsecret\tkind=77\ttarget=y\nmeta\tnext=-5");
            Check("state: bad records skipped", bad.Schemes.Count == 0 && bad.Secrets.Count == 0 && bad.NextSchemeId == 1);
        }

        private static void TurkishSuffixTests()
        {
            ExpectSuffix(Tr.Genitive("Derthert"), "Derthert'in");
            ExpectSuffix(Tr.Genitive("Caladog"), "Caladog'un");
            ExpectSuffix(Tr.Genitive("Rhagaea"), "Rhagaea'nın");
            ExpectSuffix(Tr.Genitive("Gölü"), "Gölü'nün");
            ExpectSuffix(Tr.Genitive("Ira"), "Ira'nın");
            ExpectSuffix(Tr.Dative("Derthert"), "Derthert'e");
            ExpectSuffix(Tr.Dative("Caladog"), "Caladog'a");
            ExpectSuffix(Tr.Dative("Rhagaea"), "Rhagaea'ya");
            ExpectSuffix(Tr.Accusative("Caladog"), "Caladog'u");
            ExpectSuffix(Tr.Accusative("Rhagaea"), "Rhagaea'yı");
            ExpectSuffix(Tr.Accusative("Lucon"), "Lucon'u");
            ExpectSuffix(Tr.Locative("Pravend"), "Pravend'de");
            ExpectSuffix(Tr.Locative("Sargot"), "Sargot'ta");
            ExpectSuffix(Tr.Locative("Derthert"), "Derthert'te");
            ExpectSuffix(Tr.Locative("Rhagaea"), "Rhagaea'da");
            ExpectSuffix(Tr.Locative("Epicrotea"), "Epicrotea'da");
            ExpectSuffix(Tr.Genitive("Büyük Rhagaea"), "Büyük Rhagaea'nın");
            Check("suffix: empty", Tr.Genitive("") == "" && Tr.Locative(null) == null);
        }

        private static void ExpectSuffix(string actual, string expected) => Check("suffix: " + actual + " (expected " + expected + ")", actual == expected);

        private static SchemeInputs Phases(SchemeInputs inputs, int phases)
        {
            inputs.CompletedPhases = phases;
            return inputs;
        }

        /// <summary>Haberci ve saray istihbaratı formülleri (P2.2).</summary>
        private static void LetterTests()
        {
            const int normal = SchemeMath.NormalDifficulty;
            CheckChance("letter: travel at hand", LetterRules.TravelDays(0), 1);
            CheckChance("letter: travel 100", LetterRules.TravelDays(100), 2);
            CheckChance("letter: travel 151", LetterRules.TravelDays(151), 3);
            CheckChance("letter: travel capped", LetterRules.TravelDays(5000), LetterRules.MaxTravelDays);
            CheckChance("letter: messenger cost", LetterRules.MessengerCost(2), 100);
            CheckChance("gift: none", LetterRules.GiftRelation(0), 0);
            CheckChance("gift: 1000", LetterRules.GiftRelation(1000), 2);
            CheckChance("gift: 3000", LetterRules.GiftRelation(3000), 3);
            CheckChance("gift: 6000", LetterRules.GiftRelation(6000), 5);
            CheckChance("gift: tiny still 1", LetterRules.GiftRelation(10), 1);
            CheckChance("gift: cap", LetterRules.GiftRelation(1000000), 10);
            CheckChance("persuasion: nothing known", LetterRules.PersuasionBonus(false, false), 0);
            CheckChance("persuasion: report", LetterRules.PersuasionBonus(true, false), 10);
            CheckChance("persuasion: report and secret", LetterRules.PersuasionBonus(true, true), 15);
            CheckChance("mission: detection counter-intel", MissionRules.Detection(true, 100, SpymasterMission.CounterIntel), 65);
            CheckChance("mission: detection other mission", MissionRules.Detection(true, 100, SpymasterMission.WatchDissent), 40);
            CheckChance("mission: detection idle spymaster", MissionRules.Detection(false, 100, SpymasterMission.CounterIntel), 5);
            CheckChance("mission: detection cap", MissionRules.Detection(true, 400, SpymasterMission.CounterIntel), 95);
            CheckChance("mission: dissent reports", MissionRules.DissentReportsPerWeek(0), 1);
            CheckChance("mission: dissent reports 300", MissionRules.DissentReportsPerWeek(300), 3);
            CheckChance("mission: rumor loss", MissionRules.RumorRelationLoss(100), 2);
            CheckChance("mission: rumor loss cap", MissionRules.RumorRelationLoss(500), 4);
            CheckChance("mission: rumor discovery", MissionRules.RumorDiscovery(100, normal), 6);
            CheckChance("mission: rumor discovery hard", MissionRules.RumorDiscovery(100, SchemeMath.HardDifficulty), 9);
            CheckChance("mission: rumor discovery floor", MissionRules.RumorDiscovery(600, SchemeMath.EasyDifficulty), 2);
            Check("court: moods", LetterRules.MoodName(-40) == "düşmanca" && LetterRules.MoodName(-10) == "hoşnutsuz"
                && LetterRules.MoodName(-9) == "kayıtsız" && LetterRules.MoodName(10) == "hoşnut" && LetterRules.MoodName(30) == "sadık");
            Check("court: discontent threshold", LetterRules.IsDiscontent(-10) && !LetterRules.IsDiscontent(-9));
        }

        /// <summary>Rehine anlaşmaları: kabul ve yakınlaşma.</summary>
        private static void HostageTests()
        {
            CheckChance("hostage: neutral, equal", HostageRules.ExchangeAcceptChance(0, 0, 500f, 500f), 40);
            CheckChance("hostage: friendly ruler, we are stronger", HostageRules.ExchangeAcceptChance(40, 0, 300f, 600f), 80);
            CheckChance("hostage: tense and they are stronger", HostageRules.ExchangeAcceptChance(0, 60, 900f, 400f), 5);
            CheckChance("hostage: cap", HostageRules.ExchangeAcceptChance(100, 0, 100f, 900f), 90);
            Check("hostage: foster gain capped", HostageRules.FosterGain(0) == 2 && HostageRules.FosterGain(29) == 1 && HostageRules.FosterGain(30) == 0);
            Check("hostage: one year", HostageRules.DurationDays == 84);
        }

        /// <summary>Saray günü: dilekçe verisi ve çekiliş.</summary>
        /// <summary>Kendin takip et (P2.11): sahne verisi, çekiliş ve formül örnekleri.</summary>
        private static void TailTests()
        {
            TailScene[] all = TailRules.All;
            Check("tail: unique ids", all.Select(t => t.Id).Distinct().Count() == all.Length);
            Check("tail: three options, texts name the target", all.All(t => t.Options.Length == 3 && t.Title.Length > 0
                && t.Text.Contains("{hedef}") && t.Options.All(o => o.Label.Length > 0)));
            Check("tail: every scene has a safe way", all.All(t => t.Options.Any(o => o.Stop || o.Suspicion <= 5)));
            Check("tail: stop options are empty", all.SelectMany(t => t.Options).Where(o => o.Stop)
                .All(o => o.Suspicion == 0 && o.Clues == 0 && o.Gold == 0 && o.Skill == TailSkill.None));
            Check("tail: numbers in sane bounds", all.SelectMany(t => t.Options).All(o => o.Suspicion >= 0 && o.Suspicion <= 50
                && o.Clues >= 0 && o.Clues <= 60 && o.Gold >= 0 && o.Gold <= 300));
            Check("tail: enough street and climax scenes", all.Count(t => !t.Climax) >= TailRules.StreetScenes && all.Any(t => t.Climax)
                && all.Where(t => t.Climax).All(t => t.Options.Any(o => o.Stop)));
            bool draws = true;
            for (int seed = 1; seed <= 20; seed++)
            {
                int rolls = 0;
                List<TailScene> drawn = TailRules.Draw(count => SchemeRandom.Pick(seed, ref rolls, count));
                draws &= drawn.Count == 3 && !drawn[0].Climax && !drawn[1].Climax && drawn[0].Id != drawn[1].Id && drawn[2].Climax;
            }
            Check("tail: draw two streets then a climax", draws);

            CheckChance("tail: plain step", TailRules.SuspicionGain(25, 0, 0, false, 0, 50), 25);
            CheckChance("tail: skilled vs wary target", TailRules.SuspicionGain(25, 150, 60, false, 0, 50), 20);
            CheckChance("tail: skill reduction capped", TailRules.SuspicionGain(25, 400, 0, false, 0, 50), 10);
            CheckChance("tail: night", TailRules.SuspicionGain(40, 0, 0, true, 0, 50), 30);
            CheckChance("tail: network level capped at 3", TailRules.SuspicionGain(40, 0, 0, false, 5, 50), 28);
            CheckChance("tail: no risk stays zero", TailRules.SuspicionGain(0, 0, 300, false, 0, 100), 0);
            CheckChance("tail: at least one", TailRules.SuspicionGain(1, 300, 0, true, 3, 1), 1);
            TailRules.SuspicionRange(20, 0, 0, false, 0, out int min, out int max);
            Check("tail: range " + min + "-" + max, min == 15 && max == 25);

            Check("tail: secret chance", TailRules.SecretChance(0) == 0 && TailRules.SecretChance(60) == 40 && TailRules.SecretChance(113) == 75
                && TailRules.SecretChance(200) == 75 && TailRules.SecretChance(-5) == 0);
            CheckChance("tail: run, unskilled", TailRules.EscapeChance(TailEscape.Run, 0, 0, 0, false, false), 30);
            CheckChance("tail: run, skilled at night", TailRules.EscapeChance(TailEscape.Run, 100, 100, 0, true, false), 75);
            CheckChance("tail: run cap", TailRules.EscapeChance(TailEscape.Run, 500, 500, 0, false, false), 90);
            CheckChance("tail: bluff, alerted", TailRules.EscapeChance(TailEscape.Bluff, 0, 0, 100, false, true), 35);
            CheckChance("tail: bluff floor", TailRules.EscapeChance(TailEscape.Bluff, 0, 0, 0, false, true), 10);
            CheckChance("tail: bluff cap", TailRules.EscapeChance(TailEscape.Bluff, 0, 0, 400, false, false), 85);
            CheckChance("tail: bribe always works", TailRules.EscapeChance(TailEscape.Bribe, 0, 0, 0, false, true), 100);
            Check("tail: bribe cost", TailRules.BribeCost(0) == 300 && TailRules.BribeCost(3) == 900 && TailRules.BribeCost(6) == 1500);

            Check("tail scene: not found", TailRules.SceneResult(false, 100f, 0f) == TailSceneResult.NotFound);
            Check("tail scene: success", TailRules.SceneResult(true, 70f, 2f) == TailSceneResult.Success);
            Check("tail scene: too close", TailRules.SceneResult(true, 70f, 8f) == TailSceneResult.Spotted);
            Check("tail scene: lost", TailRules.SceneResult(true, 30f, 0f) == TailSceneResult.Lost);
            Check("tail scene: clues", TailRules.SceneClues(0f) == 30 && TailRules.SceneClues(60f) == 60 && TailRules.SceneClues(200f) == 70);
        }

        /// <summary>Divan, hazinedar (P2.12): hata payı, fiyat gürültüsü, atölye tahmini, sefer defteri, zimmet.</summary>
        private static void TreasuryTests()
        {
            CheckChance("treasury: untrained", TreasuryRules.ErrorPercent(0, 0), 45);
            CheckChance("treasury: trader", TreasuryRules.ErrorPercent(120, 0), 25);
            CheckChance("treasury: calculating trader", TreasuryRules.ErrorPercent(150, 1), 15);
            CheckChance("treasury: master floor", TreasuryRules.ErrorPercent(300, 0), 5);
            CheckChance("treasury: impulsive cap", TreasuryRules.ErrorPercent(0, -4), 60);
            Check("treasury: confidence names", TreasuryRules.Confidence(5) == "kesin" && TreasuryRules.Confidence(15) == "güvenilir"
                && TreasuryRules.Confidence(25) == "tahmini" && TreasuryRules.Confidence(45) == "kulaktan dolma");
            Check("treasury: noise bounds", Math.Round(TreasuryRules.Noisy(100, 20, 1)) == 80 && Math.Round(TreasuryRules.Noisy(100, 20, 100)) == 120
                && Math.Abs(TreasuryRules.Noisy(100, 20, 50) - 100) < 1 && Math.Round(TreasuryRules.Noisy(100, 20, 500)) == 120);
            Check("treasury: known towns", TreasuryRules.KnownTowns(0) == 8 && TreasuryRules.KnownTowns(120) == 28 && TreasuryRules.KnownTowns(600) == 60);
            Check("treasury: spread", TreasuryRules.SpreadPercent(50, 75) == 50 && TreasuryRules.SpreadPercent(0, 10) == 0
                && TreasuryRules.SpreadPercent(40, 30) == -25);
            Check("treasury: workshop margin", Math.Abs(TreasuryRules.WorkshopMargin(new[] { (10.0, 30.0, 1.5), (5.0, 4.0, 1.0) }) - 29) < 0.001);
            Check("treasury: worth switching", TreasuryRules.WorthSwitching(50, 70) && !TreasuryRules.WorthSwitching(50, 59)
                && !TreasuryRules.WorthSwitching(200, 230) && TreasuryRules.WorthSwitching(-20, -5));
            CheckChance("treasury: leg profit", TreasuryRules.LegProfit(1000, 1300, 50), 350);
            var legs = new List<CaravanLeg>();
            for (int day = 0; day < 85; day++) TreasuryRules.AddLeg(legs, new CaravanLeg { FromId = "a", ToId = "b", Day = day });
            Check("treasury: ledger keeps the newest", legs.Count == TreasuryRules.MaxLegs && legs[0].Day == 5 && legs[legs.Count - 1].Day == 84);
            List<RouteStat> routes = TreasuryRules.Routes(new[]
            {
                new CaravanLeg { FromId = "A", ToId = "B", Profit = 100 }, new CaravanLeg { FromId = "A", ToId = "B", Profit = 300 },
                new CaravanLeg { FromId = "B", ToId = "C", Profit = -50 }, new CaravanLeg { FromId = "C", ToId = "C", Profit = 999 },
            });
            Check("treasury: routes", routes.Count == 2 && routes[0].FromId == "A" && routes[0].Trips == 2 && routes[0].Average == 200
                && routes[1].ToId == "C" && routes[1].Average == -50);
            Check("treasury: embezzle chance", TreasuryRules.EmbezzleChance(0) == 0 && TreasuryRules.EmbezzleChance(-1) == 25
                && TreasuryRules.EmbezzleChance(-2) == 50 && TreasuryRules.EmbezzleChance(2) == 0);
            Check("treasury: embezzle amount", TreasuryRules.EmbezzleAmount(1000, 1) == 50 && TreasuryRules.EmbezzleAmount(1000, 100) == 140
                && TreasuryRules.EmbezzleAmount(0, 50) == 0 && TreasuryRules.EmbezzleAmount(-5, 50) == 0);
            Check("treasury: embezzle detection", TreasuryRules.EmbezzleDetectChance(0, false) == 15 && TreasuryRules.EmbezzleDetectChance(200, true) == 55
                && TreasuryRules.EmbezzleDetectChance(900, true) == 75);
            Check("treasury: stable hash", TreasuryRules.Hash("") == -2128831035 && TreasuryRules.Hash("a") == -468965076
                && TreasuryRules.Hash("town_V1|iron") == 325742491);
            int r1 = TreasuryRules.WeeklyRoll(7, 3, "x|town_V1|iron"), r2 = TreasuryRules.WeeklyRoll(7, 3, "x|town_V1|iron");
            Check("treasury: weekly roll is stable", r1 == r2 && r1 >= 1 && r1 <= 100);
        }

        /// <summary>Divan, saray hekimi (P2.13): koruma yüzdeleri, şarlatan, olasılık uygulama, yıllık risk.</summary>
        private static void PhysicianTests()
        {
            Check("physician: quack below 30", PhysicianRules.IsQuack(29) && !PhysicianRules.IsQuack(30));
            Check("physician: longevity", PhysicianRules.LongevityPercent(0) == -6 && PhysicianRules.LongevityPercent(30) == 0
                && PhysicianRules.LongevityPercent(130) == 20 && PhysicianRules.LongevityPercent(280) == 50 && PhysicianRules.LongevityPercent(400) == 50);
            Check("physician: labor", PhysicianRules.LaborPercent(0) == -10 && PhysicianRules.LaborPercent(120) == 30
                && PhysicianRules.LaborPercent(270) == 80 && PhysicianRules.LaborPercent(400) == 80);
            Check("physician: poison save", PhysicianRules.PoisonSaveChance(0) == 0 && PhysicianRules.PoisonSaveChance(150) == 20
                && PhysicianRules.PoisonSaveChance(270) == 40 && PhysicianRules.PoisonSaveChance(400) == 40);
            Check("physician: hidden pregnancy", PhysicianRules.HiddenPregnancyChance(0) == 5 && PhysicianRules.HiddenPregnancyChance(120) == 25
                && PhysicianRules.HiddenPregnancyChance(300) == 50);
            Check("physician: error", PhysicianRules.ErrorPercent(0) == 45 && PhysicianRules.ErrorPercent(120) == 25 && PhysicianRules.ErrorPercent(300) == 5);
            Check("physician: apply", Math.Abs(PhysicianRules.Apply(0.1f, 50) - 0.05f) < 1e-6 && Math.Abs(PhysicianRules.Apply(0.1f, -10) - 0.11f) < 1e-6
                && PhysicianRules.Apply(0.95f, -10) == 1f && PhysicianRules.Apply(0.5f, 100) == 0f);
            double daily = 1 - Math.Pow(0.7, 1.0 / PhysicianRules.DaysPerYear);
            Check("physician: yearly risk", PhysicianRules.YearlyRisk(0) == 0 && PhysicianRules.YearlyRisk(1) == 1
                && Math.Abs(PhysicianRules.YearlyRisk(daily) - 0.3) < 1e-9);
        }

        /// <summary>Divan, serdar (P2.14): hata payı, görüş, uyarı, savunma, erzak, yol, yuvarlama, tavsiye.</summary>
        private static void MarshalTests()
        {
            Check("marshal: error", MarshalRules.ErrorPercent(0) == 45 && MarshalRules.ErrorPercent(120) == 25 && MarshalRules.ErrorPercent(300) == 5);
            Check("marshal: range", MarshalRules.KnownRange(0) == 200f && MarshalRules.KnownRange(100) == 400f && MarshalRules.KnownRange(500) == 900f);
            Check("marshal: warning", MarshalRules.WarningChance(0) == 30 && MarshalRules.WarningChance(100) == 55 && MarshalRules.WarningChance(300) == 95);
            Check("marshal: defense", MarshalRules.DefenseScore(100, 50, 2) == 195 && MarshalRules.DefenseScore(0, 0, 3) == 0
                && MarshalRules.DefenseScore(200, 100, 1) == 325);
            Check("marshal: food days", MarshalRules.FoodDays(100, -5) == 20 && MarshalRules.FoodDays(100, 0) == int.MaxValue
                && MarshalRules.FoodDays(100, 2) == int.MaxValue && MarshalRules.FoodDays(-10, -5) == 0 && MarshalRules.FoodDays(99, -10) == 9);
            Check("marshal: travel hours", MarshalRules.TravelHours(100, 4) == 25 && MarshalRules.TravelHours(0, 4) == 0 && MarshalRules.TravelHours(10, 0) == 20);
            Check("marshal: travel text", MarshalRules.TravelText(5) == "~5 saat" && MarshalRules.TravelText(0) == "~1 saat"
                && MarshalRules.TravelText(24) == "~1 gün" && MarshalRules.TravelText(60) == "~3 gün" && MarshalRules.TravelText(47) == "~2 gün");
            Check("marshal: rounding", MarshalRules.Round(43) == 45 && MarshalRules.Round(42.5) == 45 && MarshalRules.Round(437) == 440
                && MarshalRules.Round(1234) == 1250 && MarshalRules.Round(-5) == 0);
            Check("marshal: advice", MarshalRules.Advice(1.6).Contains("barış") && MarshalRules.Advice(1.2).Contains("savunmada")
                && MarshalRules.Advice(1.0).Contains("denk") && MarshalRules.Advice(0.7).Contains("saldırı") && MarshalRules.Advice(0.3).Contains("kalelerine"));
            Check("marshal: danger", MarshalRules.IsDangerous(1.15) && !MarshalRules.IsDangerous(1.1));
        }

        /// <summary>Divan, kâhya (P2.15): sadakat katkısı, yolsuzluk, eşiğe kalan gün, işaretli sayı.</summary>
        private static void StewardTests()
        {
            Check("steward: corrupt below 30", StewardRules.IsCorrupt(29) && !StewardRules.IsCorrupt(30));
            Check("steward: loyalty bonus", StewardRules.LoyaltyBonus(0) == -0.3f && StewardRules.LoyaltyBonus(30) == 0f
                && Math.Abs(StewardRules.LoyaltyBonus(130) - 0.5f) < 1e-6 && StewardRules.LoyaltyBonus(230) == 1f && StewardRules.LoyaltyBonus(400) == 1f);
            Check("steward: days until", StewardRules.DaysUntil(50, -2, 25) == 13 && StewardRules.DaysUntil(50, 0, 25) == int.MaxValue
                && StewardRules.DaysUntil(50, 1, 25) == int.MaxValue && StewardRules.DaysUntil(20, -1, 25) == 0 && StewardRules.DaysUntil(25, -1, 25) == 0
                && StewardRules.DaysUntil(26, -0.3f, 25) == 4);
            Check("steward: signed", StewardRules.Signed(0.5f, Tr.Culture) == "+0,5" && StewardRules.Signed(-1.2f, Tr.Culture) == "-1,2"
                && StewardRules.Signed(0f, Tr.Culture) == "0,0");
        }

        private static void PetitionTests()
        {
            Petition[] all = PetitionRules.All;
            Check("petition: unique ids", all.Select(p => p.Id).Distinct().Count() == all.Length);
            Check("petition: 2-3 options each, texts present", all.All(p => p.Options.Length >= 2 && p.Options.Length <= 3
                && p.Title.Length > 0 && p.Text.Length > 0 && p.Options.All(o => o.Label.Length > 0)));
            Check("petition: castles have enough", PetitionRules.Available(false).Count >= PetitionRules.PetitionsPerSession
                && PetitionRules.Available(false).All(p => !p.TownOnly) && PetitionRules.Available(true).Count == all.Length);
            Check("petition: effects in sane bounds", all.SelectMany(p => p.Options).All(o => Math.Abs(o.Effect.Loyalty) <= 10
                && Math.Abs(o.Effect.Security) <= 10 && Math.Abs(o.Effect.Prosperity) <= 100 && Math.Abs(o.Effect.Food) <= 60
                && Math.Abs(o.Effect.Gold) <= 1000 && Math.Abs(o.Effect.TraitXp) <= 50));
            Check("petition: other relation names a notable", all.SelectMany(p => p.Options)
                .All(o => o.Effect.OtherRelation == 0 || o.Effect.OtherKind != NotableKind.None));
            int seed = 7, rolls = 0;
            List<Petition> drawn = PetitionRules.Draw(false, count => SchemeRandom.Pick(seed, ref rolls, count));
            Check("petition: draw three distinct castle petitions", drawn.Count == 3 && drawn.Select(p => p.Id).Distinct().Count() == 3
                && drawn.All(p => !p.TownOnly));
            Check("petition: effect text", PetitionRules.EffectText(new PetitionEffect { Gold = -500, Loyalty = 3, Security = -2, Trait = TraitKind.Mercy, TraitXp = 40 })
                == "500 dinar, sadakat +3, güvenlik -2, Merhamet +" && PetitionRules.EffectText(new PetitionEffect()) == ""
                && PetitionRules.EffectText(new PetitionEffect { Gold = 400, Intel = true }) == "+400 dinar, rapor");
        }

        /// <summary>Düello: kabul olasılığı örnekleri.</summary>
        private static void DuelTests()
        {
            CheckChance("duel: neutral equal fighters", DuelRules.AcceptChance(0, 0, 0, 100, 100, DuelReason.Honor), 40);
            CheckChance("duel: brave honorable", DuelRules.AcceptChance(2, 1, 0, 100, 100, DuelReason.Honor), 80);
            CheckChance("duel: hates you", DuelRules.AcceptChance(0, 0, -60, 100, 100, DuelReason.Honor), 55);
            CheckChance("duel: you are far better", DuelRules.AcceptChance(0, 0, 0, 250, 50, DuelReason.Honor), 5);
            CheckChance("duel: feud wants revenge", DuelRules.AcceptChance(0, 0, -40, 100, 100, DuelReason.Feud), 70);
            CheckChance("duel: affair", DuelRules.AcceptChance(1, 0, -20, 120, 100, DuelReason.Affair), 65);
            CheckChance("duel: cowardly cap floor", DuelRules.AcceptChance(-2, -2, 40, 300, 0, DuelReason.Honor), 5);
            CheckChance("duel: cap", DuelRules.AcceptChance(2, 2, -100, 0, 300, DuelReason.Feud), 95);
            Check("duel: reason names", DuelRules.ReasonName(DuelReason.Affair) == "aldatma" && DuelRules.ReasonName(DuelReason.Feud) == "kan davası"
                && DuelRules.ReasonName(DuelReason.Honor) == "onur");
        }

        /// <summary>Eşinin gizli hayatı ve boşanma: tasarımdaki örnekler.</summary>
        private static void FidelityTests()
        {
            CheckChance("fidelity: happy marriage never strays", FidelityRules.WeeklyAffairChance(50, -2, 20), 0);
            CheckChance("fidelity: lukewarm", FidelityRules.WeeklyAffairChance(20, 0, 0), 1);
            CheckChance("fidelity: cold, dishonorable, lonely", FidelityRules.WeeklyAffairChance(-20, -1, 8), 8);
            CheckChance("fidelity: honorable resists", FidelityRules.WeeklyAffairChance(40, 1, 0), 0);
            CheckChance("fidelity: lonely adds", FidelityRules.WeeklyAffairChance(30, 0, 8), 3);
            CheckChance("fidelity: find base", FidelityRules.InvestigationFindChance(0), 40);
            CheckChance("fidelity: find skilled", FidelityRules.InvestigationFindChance(160), 80);
            CheckChance("fidelity: find cap", FidelityRules.InvestigationFindChance(600), 95);
            CheckChance("fidelity: confront loving honorable", FidelityRules.ConfrontEndChance(40, 1), 80);
            CheckChance("fidelity: confront cold", FidelityRules.ConfrontEndChance(-60, -1), 10);
            Check("fidelity: divorce family penalty", FidelityRules.DivorceFamilyPenalty(true) == 5 && FidelityRules.DivorceFamilyPenalty(false) == 20);
        }

        /// <summary>Kur, sevgili ve gebelik (P2.6): tasarımdaki örnekler ve 18+ sınırı.</summary>
        private static void CourtshipTests()
        {
            const int normal = SchemeMath.NormalDifficulty;
            CheckChance("court: charming, attractive, unmarried", RomanceRules.SuccessChance(100, 0, 60, 0, false, normal), 50);
            CheckChance("court: honorable and married", RomanceRules.SuccessChance(100, 0, 60, 2, true, normal), 24);
            CheckChance("court: floor", RomanceRules.SuccessChance(0, -60, 0, 2, true, SchemeMath.HardDifficulty), 5);
            CheckChance("court: cap", RomanceRules.SuccessChance(300, 100, 100, -2, false, SchemeMath.EasyDifficulty), 85);
            CheckChance("court: discovery unmarried", RomanceRules.DiscoveryChance(0, false, normal), 6);
            CheckChance("court: discovery married", RomanceRules.DiscoveryChance(100, true, normal), 10);
            CheckChance("court: discovery floor", RomanceRules.DiscoveryChance(600, false, SchemeMath.EasyDifficulty), 2);
            CheckChance("court: spouse agrees, loyal and pragmatic", RomanceRules.SpouseAgreeChance(40, 0, 1, 0), 60);
            CheckChance("court: honorable spouse refuses", RomanceRules.SpouseAgreeChance(40, 2, 0, 0), 20);
            CheckChance("court: meeting both married", RomanceRules.MeetingDiscovery(0, 2), 12);
            CheckChance("court: meeting skilled", RomanceRules.MeetingDiscovery(300, 0), 2);
            Check("court: careful lowers pregnancy", RomanceRules.PregnancyChanceFor(true) < RomanceRules.PregnancyChanceFor(false)
                && RomanceRules.PregnancyChanceFor(true) == 4 && RomanceRules.PregnancyChanceFor(false) == 15);
            Check("court: mothers are adults", Enumerable.Range(0, 18).All(age => !RomanceRules.CanConceive(age))
                && RomanceRules.CanConceive(18) && RomanceRules.CanConceive(45) && !RomanceRules.CanConceive(46));
            Check("court: stages", RomanceRules.StageName(0) == "Hediye ve iltifat" && RomanceRules.StageName(2) == "Gizli buluşma"
                && RomanceRules.DaysLeft(0, 0) == 21 && RomanceRules.DaysLeft(2, 3) == 4 && RomanceRules.DaysLeft(3, 0) == 0);
        }

        /// <summary>Esiri hizmetine davet: tasarımdaki örnekler.</summary>
        private static void RecruitTests()
        {
            const int normal = SchemeMath.NormalDifficulty;
            CheckChance("recruit: neutral, charm 100", RecruitRules.Chance(0, 0, 0, 0, 100, 3, 3, normal), 20);
            CheckChance("recruit: likes you, hates own leader", RecruitRules.Chance(30, -30, 0, 0, 100, 3, 3, normal), 50);
            CheckChance("recruit: honorable and loyal", RecruitRules.Chance(0, 40, 2, 0, 0, 3, 3, normal), 5);
            CheckChance("recruit: calculating, bigger clan", RecruitRules.Chance(10, 0, 0, 1, 50, 5, 2, normal), 40);
            CheckChance("recruit: cap", RecruitRules.Chance(100, -100, -2, 2, 300, 6, 1, SchemeMath.EasyDifficulty), 75);
            CheckChance("recruit: hard", RecruitRules.Chance(0, 0, 0, 0, 100, 3, 3, SchemeMath.HardDifficulty), 10);
        }

        /// <summary>Gece evi: tasarımdaki sayılar.</summary>
        private static void HouseTests()
        {
            Check("house: max staff by network level", HouseRules.MaxStaff(1) == 4 && HouseRules.MaxStaff(3) == 8 && HouseRules.MaxStaff(9) == 8);
            Check("house: shares", HouseRules.SharePercent(HouseShare.Low) == 25 && HouseRules.SharePercent(HouseShare.Standard) == 40
                && HouseRules.SharePercent(HouseShare.Fair) == 60);
            Check("house: gross", HouseRules.WeeklyGross(4, 5000f) == 360 && HouseRules.WeeklyGross(0, 5000f) == 0 && HouseRules.WeeklyGross(2, -50f) == 80);
            Check("house: net by share", HouseRules.WeeklyNet(4, 5000f, HouseShare.Standard) == 216 && HouseRules.WeeklyNet(4, 5000f, HouseShare.Fair) == 144
                && HouseRules.WeeklyNet(4, 5000f, HouseShare.Low) == 270);
            Check("house: workers leave only when underpaid", HouseRules.LeaveChance(HouseShare.Low) == 15 && HouseRules.LeaveChance(HouseShare.Standard) == 5
                && HouseRules.LeaveChance(HouseShare.Fair) == 0);
            Check("house: intel chance", HouseRules.IntelChance(4, HouseShare.Standard) == 20 && HouseRules.IntelChance(4, HouseShare.Fair) == 30
                && HouseRules.IntelChance(20, HouseShare.Fair) == 60 && HouseRules.IntelChance(0, HouseShare.Fair) == 0);
        }

        /// <summary>Ağ ifşasından diplomatik kriz (P2.5): tasarımdaki sayılar.</summary>
        private static void CrisisTests()
        {
            Check("crisis: exposure tension by level", CrisisRules.ExposureTension(1) == 10 && CrisisRules.ExposureTension(3) == 30 && CrisisRules.ExposureTension(9) == 30);
            Check("crisis: crime by level", CrisisRules.Crime(1) == 15 && CrisisRules.Crime(3) == 45);
            Check("crisis: reparations", CrisisRules.Reparations(1) == 2500 && CrisisRules.Reparations(2) == 4000 && CrisisRules.Reparations(3) == 5500);
            CheckChance("crisis: deny base", CrisisRules.DenyChance(0, 0, 1, SchemeMath.NormalDifficulty), 20);
            CheckChance("crisis: deny skilled", CrisisRules.DenyChance(150, 100, 1, SchemeMath.NormalDifficulty), 60);
            CheckChance("crisis: deny big network", CrisisRules.DenyChance(150, 100, 3, SchemeMath.NormalDifficulty), 40);
            CheckChance("crisis: deny floor", CrisisRules.DenyChance(0, 0, 3, SchemeMath.HardDifficulty), 5);
            CheckChance("crisis: deny cap", CrisisRules.DenyChance(400, 300, 1, SchemeMath.EasyDifficulty), 80);
            Check("crisis: pay cancels exposure", CrisisRules.TensionAfter(50, 2, CrisisAnswer.Pay, false) == 30);
            Check("crisis: deny success halves", CrisisRules.TensionAfter(50, 2, CrisisAnswer.Deny, true) == 40);
            Check("crisis: deny fail and defy raise", CrisisRules.TensionAfter(50, 2, CrisisAnswer.Deny, false) == 65
                && CrisisRules.TensionAfter(90, 1, CrisisAnswer.Defy, false) == 100);
            Check("crisis: tension never negative", CrisisRules.TensionAfter(5, 3, CrisisAnswer.Pay, false) == 0 && CrisisRules.Decay(3) == 0);
            Check("crisis: ruler penalties", CrisisRules.RulerPenalty(CrisisAnswer.Pay, false) == 5 && CrisisRules.RulerPenalty(CrisisAnswer.Deny, true) == 5
                && CrisisRules.RulerPenalty(CrisisAnswer.Deny, false) == 20 && CrisisRules.RulerPenalty(CrisisAnswer.Defy, false) == 15);
            Check("crisis: decay", CrisisRules.Decay(40) == 35);
            Check("crisis: war chance", CrisisRules.WeeklyWarChance(59) == 0 && CrisisRules.WeeklyWarChance(60) == 10
                && CrisisRules.WeeklyWarChance(75) == 25 && CrisisRules.WeeklyWarChance(100) == 40);
            Check("crisis: may propose war", CrisisRules.MayProposeWar(60, 500f, 900f, 1) && !CrisisRules.MayProposeWar(59, 500f, 900f, 0)
                && !CrisisRules.MayProposeWar(80, 500f, 900f, 2) && !CrisisRules.MayProposeWar(80, 400f, 900f, 0));
            Check("crisis: one defied level-3 exposure is not war yet", CrisisRules.TensionAfter(CrisisRules.ExposureTension(3), 3, CrisisAnswer.Defy, false) < CrisisRules.WarThreshold);
            Check("crisis: names", CrisisRules.TensionName(0) == "yok" && CrisisRules.TensionName(10) == "soğuk"
                && CrisisRules.TensionName(30) == "gergin" && CrisisRules.TensionName(60) == "savaş eşiğinde");
        }

        /// <summary>Gizli ilişki ve gayrimeşru çocuk (P2.4): 18+ kuralı kodda zorunlu, sınırlar burada denetlenir.</summary>
        private static void RomanceTests()
        {
            Check("romance: adult is 18", SecretRules.MinAdultAge == 18 && SecretRules.IsAdult(18) && !SecretRules.IsAdult(17));
            Check("romance: lovers both adult", SecretRules.CanBeLovers(18, 25) && !SecretRules.CanBeLovers(17, 25) && !SecretRules.CanBeLovers(30, 16));
            Check("romance: lovers never minors, any gap", Enumerable.Range(0, 18).All(minor =>
                Enumerable.Range(0, 90).All(other => !SecretRules.CanBeLovers(minor, other) && !SecretRules.CanBeLovers(other, minor))));
            Check("romance: plausible age gap", SecretRules.CanBeLovers(30, 50) && !SecretRules.CanBeLovers(25, 46));
            Check("romance: parent adult at conception", SecretRules.MinParentAgeAtBirth == 19
                && SecretRules.CouldBeParent(19, 0) && !SecretRules.CouldBeParent(18, 0) && SecretRules.CouldBeParent(40, 21) && !SecretRules.CouldBeParent(36, 18));
            Check("romance: parent age covers every child age", Enumerable.Range(0, 60).All(child =>
                Enumerable.Range(0, 100).All(parent => SecretRules.CouldBeParent(parent, child) == (parent - child >= 19))));
            Check("romance: negative child age rejected", !SecretRules.CouldBeParent(40, -1));
            Check("romance: kinds", SecretRules.IsRomantic(SecretKind.Affair) && SecretRules.IsRomantic(SecretKind.Bastard)
                && !SecretRules.IsRomantic(SecretKind.Bribery) && !SecretRules.IsRomantic(SecretKind.MurderPlot));
            Check("romance: bastard is the heaviest scandal", SecretRules.ExposurePenalty(SecretKind.Bastard) > SecretRules.ExposurePenalty(SecretKind.Affair)
                && SecretRules.Name(SecretKind.Bastard) == "Gayrimeşru çocuk");
        }

        /// <summary>Esir sorgulama formülleri (P2.1): örnek hesaplar ve sınırlar.</summary>
        private static void InterrogationTests()
        {
            const int normal = SchemeMath.NormalDifficulty;
            int Chance(InterrogationMethod m, int charm = 0, int roguery = 0, int relation = 0, int calc = 0, int valor = 0, int honor = 0,
                int difficulty = normal) => InterrogationRules.Chance(m, charm, roguery, relation, calc, valor, honor, difficulty);

            CheckChance("interrogate: talk charm 100", Chance(InterrogationMethod.Talk, charm: 100), 50);
            CheckChance("interrogate: talk hostile calculating -> floor", Chance(InterrogationMethod.Talk, relation: -40, calc: 2), 10);
            CheckChance("interrogate: talk cap", Chance(InterrogationMethod.Talk, charm: 300, relation: 100, calc: -2), 85);
            CheckChance("interrogate: pressure roguery 100", Chance(InterrogationMethod.Pressure, roguery: 100), 65);
            CheckChance("interrogate: pressure brave", Chance(InterrogationMethod.Pressure, valor: 2), 35);
            CheckChance("interrogate: pressure cap", Chance(InterrogationMethod.Pressure, roguery: 300), 90);
            CheckChance("interrogate: pressure easy", Chance(InterrogationMethod.Pressure, roguery: 100, difficulty: SchemeMath.EasyDifficulty), 75);
            CheckChance("interrogate: pressure hard", Chance(InterrogationMethod.Pressure, roguery: 100, difficulty: SchemeMath.HardDifficulty), 55);
            CheckChance("interrogate: deal neutral", Chance(InterrogationMethod.Deal), 70);
            CheckChance("interrogate: deal floor", Chance(InterrogationMethod.Deal, relation: -60, honor: -2), 40);
            CheckChance("interrogate: deal cap", Chance(InterrogationMethod.Deal, relation: 100, honor: 2), 95);
            CheckChance("interrogate: secret talk", InterrogationRules.SecretChance(InterrogationMethod.Talk, 60), 40);
            CheckChance("interrogate: secret pressure", InterrogationRules.SecretChance(InterrogationMethod.Pressure, 60), 60);
            CheckChance("interrogate: secret cap", InterrogationRules.SecretChance(InterrogationMethod.Pressure, 600), 95);
            Check("interrogate: pressure talks about liege most",
                InterrogationRules.LiegeIntelChance(InterrogationMethod.Pressure) > InterrogationRules.LiegeIntelChance(InterrogationMethod.Deal)
                && InterrogationRules.LiegeIntelChance(InterrogationMethod.Deal) > InterrogationRules.LiegeIntelChance(InterrogationMethod.Talk));
        }

        /// <summary>Olay kartı resim adları: benzersiz, dosya adına uygun ve docs/gorseller/olay-kartlari.md ile aynı liste.</summary>
        private static void CardImageTests()
        {
            var names = new List<string>(CardImages.All());
            Check("images: unique", new HashSet<string>(names).Count == names.Count);
            Check("images: file-safe names", names.TrueForAll(n => n.Length > 0 && n.ToCharArray().All(c => (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '_')));
            Check("images: every event has an image", Enumerable.Range(1, 10).All(id => CardImages.ForEvent(id) != null));
            Check("images: unknown event -> null", CardImages.ForEvent(99) == null);
            Check("images: kind mapping", CardImages.ForKind(SchemeKind.Kidnap) == CardImages.Kidnap && CardImages.ForKind(SchemeKind.Spy) == CardImages.Spy);
            Check("images: method mapping", CardImages.ForMurderMethod(SchemeMethod.Poison) == CardImages.MurderPoison
                && CardImages.ForMurderMethod(SchemeMethod.None) == CardImages.MurderDagger);

            string doc = FindRepoFile(System.IO.Path.Combine("docs", "gorseller", "olay-kartlari.md"));
            if (doc == null)
            {
                Console.WriteLine("SKIP images: docs list (file not found)");
                return;
            }
            string text = System.IO.File.ReadAllText(doc);
            List<string> documented = System.Text.RegularExpressions.Regex.Matches(text, "`([a-z0-9_]+)\\.png`")
                .Cast<System.Text.RegularExpressions.Match>().Select(m => m.Groups[1].Value).Distinct().ToList();
            Check("images: code names are documented", names.TrueForAll(documented.Contains));
            Check("images: documented names are used (or their variants)", documented.TrueForAll(n => CardImages.BaseOf(n) != null));
            Check("images: every card has variant prompts", names.TrueForAll(n => documented.Contains(CardImages.Variant(n, 2)) && documented.Contains(CardImages.Variant(n, 3))));
            Check("images: variant names", CardImages.Variant("sonuc_kur", 1) == "sonuc_kur" && CardImages.Variant("sonuc_kur", 3) == "sonuc_kur_3"
                && CardImages.Variants("tur_santaj").Count() == CardImages.MaxVariants);
            Check("images: base of variant", CardImages.BaseOf("sonuc_kur_3") == "sonuc_kur" && CardImages.BaseOf("sonuc_kur") == "sonuc_kur"
                && CardImages.BaseOf("sonuc_kur_9") == null && CardImages.BaseOf("sonuc_kur_1") == null && CardImages.BaseOf("yok_2") == null
                && CardImages.BaseOf("sonuc_kur_02") == null);
        }

        private static string FindRepoFile(string relative)
        {
            for (var dir = new System.IO.DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
            {
                string path = System.IO.Path.Combine(dir.FullName, relative);
                if (System.IO.File.Exists(path)) return path;
            }
            return null;
        }

        private static void CheckChance(string label, int actual, int expected) =>
            Check(label + ": " + actual + " (expected " + expected + ")", actual == expected);

        private static void Expect(string input, bool expectedOk, int expectedValue = 0)
        {
            bool ok = AmountParser.TryParse(input, out int value);
            bool pass = ok == expectedOk && (!expectedOk || value == expectedValue);
            if (!pass) _failures++;
            Console.WriteLine((pass ? "PASS " : "FAIL ") + "\"" + input + "\" -> ok=" + ok + ", value=" + value
                + " (expected ok=" + expectedOk + ", value=" + expectedValue + ")");
        }

        private static void ExpectName(string input, bool allowEmpty, bool expectedOk, string expectedName)
        {
            bool ok = NameRules.TryNormalize(input, allowEmpty, out string name, out string error);
            bool pass = ok == expectedOk && (!expectedOk || name == expectedName);
            if (!pass) _failures++;
            Console.WriteLine((pass ? "PASS " : "FAIL ") + "name \"" + input + "\" (allowEmpty=" + allowEmpty + ") -> ok=" + ok
                + ", name=\"" + name + "\"" + (error != null ? " [error]" : ""));
        }

        private static void ExpectRange(string input, int min, int max, bool expectedOk, int expectedValue = 0)
        {
            bool ok = AmountParser.TryParseInRange(input, min, max, out int value);
            Check("range \"" + input + "\" [" + min + "," + max + "] -> ok=" + ok + ", value=" + value,
                ok == expectedOk && (!expectedOk || value == expectedValue));
        }

        private static void Check(string label, bool pass)
        {
            if (!pass) _failures++;
            Console.WriteLine((pass ? "PASS " : "FAIL ") + label);
        }
    }
}
