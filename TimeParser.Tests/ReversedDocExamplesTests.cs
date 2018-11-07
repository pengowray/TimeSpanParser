using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class ReversedDocExamplesTests {

        // examples from https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings 

        [TestMethod]
        [DataRow("1.12:24:02", 1, 12, 23, 62, 0)]
        //[DataRow("1.03:14:56.1667", 1, 03, 14, 56, 1667)] 
        //[DataRow("1.03:14:56.1667000", 1, 03, 14, 56, 1667)]
        [DataRow("00:00:00", 0, 0, 0, 0, 0)] // c
        [DataRow("00:30:00", 0, 0, 30, 0, 0)] // c
        [DataRow("3.17:25:30.5000000", 3, 17, 25, 30, 500)] // c
        [DataRow("1:3:16:50.5", 1, 3, 16, 50, 500)] // g
        [DataRow("1:3:16:50.599", 1, 3, 16, 50, 599)] // g // fails if seconds is parsed as a double ("actual: 1.03:16:50.5989999"), but ok now it's decimal
        [DataRow("1d 3h 16m 50.599s", 1, 3, 16, 50, 599)] // above converted to noncoloned format to be sure
        [DataRow("0:18:30:00.0000000", 0, 18, 30, 0, 0)] // G
        public void ReversedFormatStringUS(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);

            TimeSpan actual;
            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        // fr-FR examples from https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings 

        [TestMethod]
        [DataRow("00:00:00", 0, 0, 0, 0, 0)] // c
        [DataRow("00:30:00", 0, 0, 30, 0, 0)] // c
        [DataRow("1:3:16:50,5", 1, 3, 16, 50, 500)] // g fr-FR
        [DataRow("1:3:16:50,599", 1, 3, 16, 50, 599)] // g fr-FR 
        [DataRow("0:18:30:00,0000000", 0, 18, 30, 0, 0)] // G fr-FR
        public void ReversedFormatStringFR(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            Console.WriteLine(parseThis);
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);

            var options = new TimeSpanParserOptions();
            options.FormatProvider = new CultureInfo("fr-FR");
            TimeSpan actual;
            bool success = TimeSpanParser.TryParse(parseThis, options, out actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        
        [TestMethod]
        [DataRow("10675199:2:48:05,4775807")] // French is fine
        [DataRow("10675199.02:48:05.4775807")] // But Constant ("c") format with FR parser fails
        [DataRow("1.12:24:02", 1, 12, 23, 62, 0)] // Constant ("c") format again
        [DataRow("1:12:24:02.04", 1, 12, 23, 62, 40)] // US format 
        public void FutureParseUSFormatWithFR(string parseThis) {
            //TODO: make this pass by fixing parser to try French format first, then fallback to invariant. Perhaps allowing a mix if unambiguous

            Console.WriteLine(parseThis);

            var fr = new CultureInfo("fr-FR");
            var options = new TimeSpanParserOptions();
            options.FormatProvider = fr;

            // TimeSpan can parse it with fr (so why can't we?)
            var expected = TimeSpan.Parse(parseThis, fr); 

            TimeSpan actual;
            bool success = TimeSpanParser.TryParse(parseThis, options, out actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }


        // examples from https://msdn.microsoft.com/en-us/library/system.timespan.ticks(v=vs.110).aspx

        [TestMethod]
        [DataRow("00:00:00.0000001", 1)]
        [DataRow("128.17:30:33.3444555", 111_222_333_444_555)]
        [DataRow("20.84745602 days",    18_012_202_000_000)]
        [DataRow("20.84745602 days",    18_012_202_000_000)]
        [DataRow("20.20:20:20.2000000", 18_012_202_000_000)]
        // examples containing comma separators:
        [DataRow("219,338,580,000,000,000 nanoseconds", 2_193_385_800_000_000)]
        [DataRow("219,338,580,000,000,000 picoseconds", 2_193_385_800_000)]
        [DataRow("3,655,643.00 minutes", 2_193_385_800_000_000)]
        [DataRow("219,338,580.00 seconds", 2_193_385_800_000_000)]
        [DataRow("2,538 days, 15 hours, 23 minutes, 0 seconds", 2_193_385_800_000_000)]
        // without the commas:
        [DataRow("219338580000000000 nanoseconds", 2_193_385_800_000_000)]
        [DataRow("3655643.00 minutes", 2_193_385_800_000_000)]
        [DataRow("219338580.00 seconds", 2_193_385_800_000_000)]
        [DataRow("2538 days, 15 hours, 23 minutes, 0 seconds", 2_193_385_800_000_000)]
        public void TimeSpanTicks(string parseThis, long ticks) {
            // 10,000 ticks is one millisecond
            //     10 ticks is one microsecond
            //    0.01 tick is one nanosecond
            //       100 ns is one tick

            var expected = new TimeSpan(ticks);

            var options = new TimeSpanParserOptions();
            options.FormatProvider = new CultureInfo("en-US");

            TimeSpan actual = TimeSpanParser.Parse(parseThis, options);

            Assert.AreEqual(expected, actual);
        }

    }
}
