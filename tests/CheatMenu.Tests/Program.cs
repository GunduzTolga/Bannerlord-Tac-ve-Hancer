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
            ModSettings back = ModSettings.Parse(full.Format());
            Check("settings: full round trip", back.FastForwardSpeed == 8 && back.SkillXpMultiplier == 3
                && back.TroopXpMultiplier == 5 && back.PartySizeBonus == 100 && back.NoLearningLimit && back.NoCompanionLimit
                && back.PartyLimitBonus == 4 && back.NoPartyLimit && back.IntrigueDifficulty == 0
                && back.AiIntrigue == 2 && back.PlayerCanBeTargeted && back.MapButtonPosition == 3 && back.ScreenHotkey == 5
                && !back.EventCards && !back.RomanceSecrets && back.NightHouses && !back.SpouseInfidelity);

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
