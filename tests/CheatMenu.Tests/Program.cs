using System;
using System.Collections.Generic;
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
            var full = new ModSettings();
            full.SetFastForwardSpeed(8);
            full.SetSkillXpMultiplier(3);
            full.SetTroopXpMultiplier(5);
            full.SetPartySizeBonus(100);
            full.NoLearningLimit = true;
            full.NoCompanionLimit = true;
            full.SetPartyLimitBonus(4);
            full.NoPartyLimit = true;
            ModSettings back = ModSettings.Parse(full.Format());
            Check("settings: full round trip", back.FastForwardSpeed == 8 && back.SkillXpMultiplier == 3
                && back.TroopXpMultiplier == 5 && back.PartySizeBonus == 100 && back.NoLearningLimit && back.NoCompanionLimit
                && back.PartyLimitBonus == 4 && back.NoPartyLimit);

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

            Console.WriteLine(_failures == 0 ? "ALL TESTS PASSED" : _failures + " TEST(S) FAILED");
            return _failures == 0 ? 0 : 1;
        }

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
