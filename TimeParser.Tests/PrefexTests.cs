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
        TimeSpanParserOptions options = new TimeSpanParserOptions
        {
            ColonedDefault = Units.Minutes,
            UncolonedDefault = Units.Minutes,
            FailIfMoreTimeSpansFoundThanRequested = false
        };

        [TestMethod]
        [DataRow("starting in 3 minutes for 3:18 hours and 2 seconds ." )]
        [DataRow("3 5 wait 6")]
        [DataRow("11:10 5 0 wait 6")]
        [DataRow("10:20 30 now")]
        [DataRow("in 5, wait.5")]
        [DataRow("test for 3400 minutes 3 seconds in 2 HOURS delay=30 ")]
        [DataRow("Elapsed time: 0:00:00.0001497")]
        //[DataRow("Elapsed time: 0:00:00.0001497")]
        public void PrefixDummyTest(string testString) {
            Console.WriteLine(testString);

            Dictionary<string, TimeSpan?> matches;
            bool success = TimeSpanParser.TryParsePrefixed(testString, prefixes, options, out matches);

            char quot = '"';
            Console.WriteLine(string.Join("\n", matches
                .Select(m => $"expected[{quot}{m.Key}{quot}] = {prettyPrintTimeSpan(m.Value)};" )));

            //Assert.AreEqual(2, 2); //TODO
            Assert.IsTrue(success);
        }

        public static string prettyPrintTimeSpan(TimeSpan? timespan) {
            if (timespan == null)
                return "null";

            //return $"new TimeSpan(\"{timespan}\")";
            return $"TimeSpan.Parse(\"{timespan}\")";
            //return $"\"{timespan}\")";
        }

        [TestMethod]
        public void PrefixTesti1() {
            Dictionary<string, TimeSpan?> expected = new Dictionary<string, TimeSpan?>();
            expected["in"] = TimeSpan.Parse("00:05:00");
            expected["wait"] = TimeSpan.Parse("00:00:30");

            string parseThis = "in 5, wait.5";

            DoParseAndCompare(expected, parseThis);
        }

        [TestMethod]
        public void PrefixTesti2() {
            Dictionary<string, TimeSpan?> expected = new Dictionary<string, TimeSpan?>();
            //expected["0"] = TimeSpan.Parse("Elapsed time: 0:00:00.0001497");
            expected["0"] = TimeSpan.Parse("0:00:00.0001497");

            string parseThis = "00:00:00.0001497";

            DoParseAndCompare(expected, parseThis);
        }

        [TestMethod]
        public void PrefixTesti3() {
            Dictionary<string, TimeSpan?> expected = new Dictionary<string, TimeSpan?>();
            expected["for"] = TimeSpan.FromMinutes(3400) + TimeSpan.FromSeconds(3); // Parse("2.08:40:03");
            expected["in"] = TimeSpan.Parse("02:00:00");  // TimeSpan.Parse("00:02:00");
            expected["delay"] = TimeSpan.Parse("00:30:00");

            string parseThis = "test for 3400 minutes 3 seconds in 2 HOURS delay=30 ";

            DoParseAndCompare(expected, parseThis);
        }

        void DoParseAndCompare(Dictionary<string, TimeSpan?> expected, string parseThis) {
            Dictionary<string, TimeSpan?> matches;
            bool success = TimeSpanParser.TryParsePrefixed(parseThis, prefixes, options, out matches);

            Assert.IsTrue(success);

            bool itsAMatch = expected.OrderBy(kvp => kvp.Key)
                       .SequenceEqual(matches.OrderBy(kvp => kvp.Key));

            Assert.IsTrue(itsAMatch);

        }

    }

}
