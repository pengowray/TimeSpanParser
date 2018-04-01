using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class PrefexTests {

        [TestMethod]
        [DataRow("please wait 3 minutes in 3:18 hours and 2 seconds .")]
        [DataRow("3 5 wait 6")]
        [DataRow("10:20 30 now")]
        [DataRow("in 5, wait.5")]
        [DataRow("test for 3400 minutes 3 seconds in 2 HOURS delay=30 ")]
        public void PrefixTest(string testString) {
            string[] prefixes = new string[] { "for", "in", "delay", "wait", "now" };

            Dictionary<string, TimeSpan?> matches;
            TimeSpanParser.TryParsePrefixed(testString, Units.Minutes, Units.Minutes, prefixes, out matches); ;

            Console.WriteLine($"test:{testString}");
            char quot = '"';
            Console.WriteLine(string.Join("\n", matches
                .Select(m => $"matches[{quot}{m.Key}{quot}] = {prettyPrintTimeSpan(m.Value)}" )));

            Assert.AreEqual(2, 2); //TODO
        }

        public static string prettyPrintTimeSpan(TimeSpan? timespan) {
            if (timespan == null)
                return "null";

            return $"new TimeSpan(\"{timespan}\")";

        }

    }
}
