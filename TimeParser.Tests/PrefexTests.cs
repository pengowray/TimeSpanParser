using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class PrefexTests {
        string[] prefixes = new string[] { "for", "in", "delay", "wait", "now" };
        TimeSpanParserOptions minuteOptions = new TimeSpanParserOptions
        {
            ColonedDefault = Units.Minutes,
            UncolonedDefault = Units.Minutes,
            FailIfMoreTimeSpansFoundThanRequested = false // not needed for Prefixed?
        };
        TimeSpanParserOptions hoursOptions = new TimeSpanParserOptions
        {
            ColonedDefault = Units.Hours,
            UncolonedDefault = Units.Hours,
            FailIfMoreTimeSpansFoundThanRequested = false // not needed for Prefixed?
        };

        TimeSpanParserOptions defaultOptions = new TimeSpanParserOptions();
        TimeSpanParserOptions nofailOptions = new TimeSpanParserOptions
        {
            FailOnUnitlessNumber = false
        };

        [TestMethod]
        [DataRow("starting in 3 minutes for 3:18 hours and 2 seconds ." )] // 6
        [DataRow("3 5 wait 6")]
        [DataRow("11:10 5 0 wait 6")] // 5
        [DataRow("10:20 30 now")] // 4
        [DataRow("in 5, wait.5")] // 1
        [DataRow("test for 3400 minutes 3 seconds in 2 HOURS delay=30 ")] // 3
        [DataRow("Elapsed time: 0:00:00.0001497")] // 2
        [DataRow("1:10 2:20")] // 7
        [DataRow("1:10 in 2:20 for 3:30")]

        //[DataRow("Elapsed time: 0:00:00.0001497")]
        public void PrefixDummyTest(string testString) {
            Console.WriteLine(testString);

            Dictionary<string, TimeSpan?> minuteMatches;
            bool success2 = TimeSpanParser.TryParsePrefixed(testString, prefixes, minuteOptions, out minuteMatches);
            Console.WriteLine(PrettyPrintTimeDict(minuteMatches, "minuteExpected"));
            Assert.IsTrue(success2);

            Dictionary<string, TimeSpan?> matches;
            bool success = TimeSpanParser.TryParsePrefixed(testString, prefixes, defaultOptions, out matches);
            Console.WriteLine(PrettyPrintTimeDict(matches, "expected"));
            //Assert.IsTrue(success);
        }

        public static string PrettyPrintTimeDict(Dictionary<string, TimeSpan?> dict, string variableName) {
            char quot = '"';
            return string.Join("\n", dict
                .Select(m => $"{variableName}[{quot}{m.Key}{quot}] = {PrettyPrintTimeSpan(m.Value)};"));

        }
        public static string PrettyPrintTimeSpan(TimeSpan? timespan) {
            if (timespan == null)
                return "null";

            //return $"new TimeSpan(\"{timespan}\")";
            return $"TimeSpan.Parse(\"{timespan}\")";
            //return $"\"{timespan}\")";
        }

        [TestMethod]
        public void PrefixTest_1() {
            string parseThis = "in 5, wait.5";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["in"] = TimeSpan.Parse("00:05:00");
            expected["wait"] = TimeSpan.Parse("00:00:30");

            DoParseAndCompare(expected, parseThis, minuteOptions);
        }

        [TestMethod]
        public void PrefixTest_2() {
            string parseThis = "Elapsed time: 0:00:00.0001497";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["0"] = TimeSpan.Parse("0:00:00.0001497");

            DoParseAndCompare(expected, parseThis, minuteOptions);
            DoParseAndCompare(expected, parseThis, defaultOptions);
            DoParseAndCompare(expected, parseThis, nofailOptions);
        }

        [TestMethod]
        public void PrefixTest_3_minutes() {
            string parseThis = "test for 3400 minutes 3 seconds in 2 HOURS delay=30 ";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["for"] = TimeSpan.FromMinutes(3400) + TimeSpan.FromSeconds(3); // Parse("2.08:40:03");
            expected["in"] = TimeSpan.Parse("02:00:00");  // TimeSpan.Parse("00:02:00");
            expected["delay"] = TimeSpan.Parse("00:30:00");

            DoParseAndCompare(expected, parseThis, minuteOptions);
        }

        [TestMethod]
        public void PrefixTest_3() {
            string parseThis = "test for 3400 minutes 3 seconds in 2 HOURS delay=30m ";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["for"] = TimeSpan.FromMinutes(3400) + TimeSpan.FromSeconds(3); // Parse("2.08:40:03");
            expected["in"] = TimeSpan.Parse("02:00:00");  // TimeSpan.Parse("00:02:00");
            expected["delay"] = TimeSpan.Parse("00:30:00");

            DoParseAndCompare(expected, parseThis, defaultOptions);
        }


        [TestMethod]
        public void PrefixTest_4_minutes() {
            string parseThis = "10:20 30 now";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["0"] = TimeSpan.Parse("00:10:20");
            expected["1"] = TimeSpan.Parse("00:30:00");
            expected["now"] = null;

            DoParseAndCompare(expected, parseThis, minuteOptions);
        }

        [TestMethod]
        public void PrefixTest_4_default() {
            string parseThis = "10:20 30 now";

            // "30" should cause failure (has no default units)

            DoParseAndCompare(null, parseThis, defaultOptions);
        }

        [TestMethod]
        public void PrefixTest_4_nofailzero() {
            string parseThis = "10:20 0 now";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["0"] = TimeSpan.Parse("10:20:00");
            // "30" ignored because no units
            expected["now"] = null;

            DoParseAndCompare(expected, parseThis, nofailOptions);
        }

        [TestMethod]
        public void PrefixTest_4_nofail() {
            string parseThis = "10:20 30 now";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["0"] = TimeSpan.Parse("10:20:00");
            // "30" ignored because no units
            expected["now"] = null;

            DoParseAndCompare(expected, parseThis, nofailOptions);
        }


        [TestMethod]
        public void PrefixTest_5() {
            string parseThis = "11:10 5 0 wait 6";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["0"] = TimeSpan.Parse("00:11:10");
            expected["1"] = TimeSpan.Parse("00:05:00");
            expected["2"] = TimeSpan.Parse("00:00:00");
            expected["wait"] = TimeSpan.Parse("00:06:00");

            DoParseAndCompare(expected, parseThis, minuteOptions);
        }

        [TestMethod]
        public void PrefixTest_6() {
            string parseThis = "starting in 3 minutes for 3:18 hours and 2 seconds .";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["in"] = TimeSpan.Parse("00:03:00");
            expected["for"] = TimeSpan.Parse("03:18:02");

            DoParseAndCompare(expected, parseThis, defaultOptions);
            DoParseAndCompare(expected, parseThis, minuteOptions);
            DoParseAndCompare(expected, parseThis, nofailOptions);

        }

        [TestMethod]
        public void PrefixTest_7_minutes() {
            string parseThis = "1:10 2:20";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["0"] = TimeSpan.Parse("00:01:10");
            expected["1"] = TimeSpan.Parse("00:02:20");

            DoParseAndCompare(expected, parseThis, minuteOptions);
        }

        [TestMethod]
        public void PrefixTest_7_hours() {
            string parseThis = "1:10 2:20";

            var expected = new Dictionary<string, TimeSpan?>();
            expected["0"] = TimeSpan.Parse("01:10:00");
            expected["1"] = TimeSpan.Parse("02:20:00");

            DoParseAndCompare(expected, parseThis, defaultOptions);
            DoParseAndCompare(expected, parseThis, hoursOptions);
        }


        void DoParseAndCompare(Dictionary<string, TimeSpan?> expected, string parseThis, TimeSpanParserOptions options) {
            Console.WriteLine(parseThis);

            Dictionary<string, TimeSpan?> matches;
            bool success = TimeSpanParser.TryParsePrefixed(parseThis, prefixes, options, out matches);

            if (expected == null) { // use expected = null to expect failure
                if (success) Console.WriteLine(PrettyPrintTimeDict(matches, "actual"));
                Assert.IsFalse(success);
                return;
            }

            Assert.IsTrue(success);

            Console.WriteLine(PrettyPrintTimeDict(expected, "expected"));
            Console.WriteLine(PrettyPrintTimeDict(matches, "actual"));

            bool itsAMatch = expected.OrderBy(kvp => kvp.Key)
                       .SequenceEqual(matches.OrderBy(kvp => kvp.Key));
            Assert.IsTrue(itsAMatch);
        }

    }

}
