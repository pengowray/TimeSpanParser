using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;
using System.Globalization;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class MinMaxTests {

        [TestMethod]
        [DataRow("00:00")]
        [DataRow("00:00.")]
        [DataRow("00:00:00")] // default value
        [DataRow("00:00:00.")]
        [DataRow("00:00:00.0")]
        [DataRow("0:00:00:00")]
        [DataRow("0.00:00:00.00")]
        [DataRow("0:00:00:00.")]
        [DataRow("0:00:00:00.00000")]
        [DataRow("0:0:0")]
        [DataRow("0")]
        [DataRow("-0")]
        [DataRow("-0:0:0")]
        [DataRow("-00:00:00")]
        [DataRow("-00:00:00.")]
        [DataRow("-0:00:00:00")]
        public void ZeroTest(string parseThis) {
            var expected = TimeSpan.Zero;
            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("0")]
        [DataRow("0:00")]
        [DataRow("0:0:0:0:0:0")]
        [DataRow("0 0 0 0 0")] // everything after first 0 should be ignored
        [DataRow("0000.00000000:00.0:0.00:00.00")]
        [DataRow("0:0:0:0:0:0:0:0:0:0.0")]
        [DataRow("0 weeks 0 days 0 hours 0 minutes 0 seconds")]
        [DataRow("0 picoseconds")]
        public void ZeroOnlyWeirdnessTest(string parseThis) {
            Console.WriteLine(parseThis);

            var expected = TimeSpan.Zero;
            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("0000.00000000:00.0:0.00:00:00:00:00.1")]
        [DataRow("0:0:0:0:0:0:2")]
        [DataRow("0.0:0:0:0:0:0:2")]
        [DataRow(".0:0:0:0:0:0:2")]
        [DataRow("0:0:0:0:0:0:2.")]
        public void ZeroOnlyFailTest(string parseThis) {
            Console.WriteLine(parseThis);

            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);
            Assert.IsFalse(success);
        }

        [TestMethod]
        [DataRow("1 year")]
        [DataRow("2 months")]
        [DataRow("1.1 years")]
        [DataRow("3.33 months")]
        [DataRow("1.2:33:44 years")]
        [DataRow("10:33:44 months")]
        public void YearMonthFailTests(string parseThis) {
            Console.WriteLine(parseThis);

            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);
            Assert.IsFalse(success);
        }

        [TestMethod]
        [DataRow("0 years")]
        [DataRow("0 years 0 days")]
        [DataRow("0:00 months")]
        [DataRow("0:0:0:0:0:0 y")]
        [DataRow("0 years 0 months 0 days")]
        [DataRow("0000.00000000:00.0:0.00:00.00 years")]
        [DataRow("0:0:0:0:0:0:0:0:0:0.0 years")]
        public void ZeroOnlyYearMonth(string parseThis) {
            Console.WriteLine(parseThis);

            var expected = TimeSpan.Zero;
            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("-")]
        [DataRow("-:")]
        [DataRow("")]
        [DataRow(".")]
        [DataRow("/")]
        [DataRow(". . . . .")]
        [DataRow(":.")]
        [DataRow(".:")]
        [DataRow(".:.")]
        [DataRow(".:.:.:.")]
        [DataRow(null)]
        public void NothingTest(string parseThis) {

            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual); ;

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void MinTest() {
            string MinimumTimeSpan = "-10675199.02:48:05.4775808";
            bool success = TimeSpanParser.TryParse(MinimumTimeSpan, timeSpan: out TimeSpan actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(TimeSpan.MinValue, actual);
        }

        [TestMethod]
        public void MaxTest() {
            string MaximumTimeSpan = "10675199:02:48:05.4775807";

            var expected1 = TimeSpan.Parse(MaximumTimeSpan);
            var expected2 = TimeSpan.MaxValue;

            TimeSpan actual;
            bool success = TimeSpanParser.TryParse(MaximumTimeSpan, timeSpan: out actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(expected1, actual);
        }

        [TestMethod]
        public void NearMaxTest() {
            //A somewhat redundant test. Same number of digits as TimeSpan.MaxValue
            string NearMaximumTimeSpan = "10111111.11:11:11.1111111";

            var expected = TimeSpan.Parse(NearMaximumTimeSpan);
            TimeSpan actual;
            bool success = TimeSpanParser.TryParse(NearMaximumTimeSpan, timeSpan: out actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        protected IEnumerable<string> DigitStringsDepthFirst(char[] tryDigits, int maxLen = 4, string text = "") {
            if (maxLen < 0)
                yield break;

            yield return text;

            foreach (var digit in tryDigits) {
                //yield return text + digit.ToString(); // the "" option
                foreach (var d2 in DigitStringsDepthFirst(tryDigits, maxLen - 1, text + digit)) {
                    yield return d2;
                }
            }
        }

        protected IEnumerable<string> DigitStrings(char[] tryDigits, int maxLen = 4, string text = "") {
            return Enumerable.Range(0, (int)Math.Pow(tryDigits.Length, maxLen)).Select(n => text + IntToString(n, tryDigits)).Where(s => !s.Contains(' '));
        }

        public static string IntToString(int value, char[] baseChars) {
            string result = string.Empty;
            int targetBase = baseChars.Length;

            do {
                result = baseChars[value % targetBase] + result;
                value = value / targetBase;
            }
            while (value > 0);

            return result;
        }

        [TestMethod]
        public void OverflowExceptionValuesNonTest() {
            //char[] baseChars = { '5', '0', '1', '8', '9' };
            char[] baseChars = { ' ', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] baseCharsShort = { ' ', '0', '1', '9' };

            string start = "0:00:00.000000";
            //foreach (var text in DigitStringsDepthFirst(baseChars, 3, start)) {
            foreach (var text in DigitStrings(baseChars, 2, start)) {
                var decimals = text.Split('.')[1];
                var nonZeroDecimals = decimals.TrimEnd('0');
                var nonredundant = nonZeroDecimals.Length;
                var redundant = decimals.Length - nonredundant;

                string dp = $"// dp: {decimals.Length} ({redundant} redundant)";

                TimeSpan tspParsed;
                long tspTicks = 0;
                bool tspSuccess = false; ;

                try {
                    tspParsed = TimeSpanParser.Parse(text);
                    tspTicks = tspParsed.Ticks;
                    tspSuccess = true;

                } catch (OverflowException e) {
                    tspSuccess = false;
                }

                long ticks = 0;
                bool success = false;
                try {
                    var parsed = TimeSpan.Parse(text);
                    ticks = parsed.Ticks;
                    success = true;
                    

                } catch (OverflowException e) {
                    success = false;
                }

                bool overflowedByZeroes = !success && nonredundant <= 7 && redundant >= 1;
                bool earlyOverflow = !overflowedByZeroes && !success && tspTicks > 0; // note: excludes overflowedByZeroes 
                bool wrongCount = success && (ticks != tspTicks);

                string command = $"TimeSpan.Parse(\"{text}\"); //";
                string commandTicks = $"TimeSpan.Parse(\"{text}\").Ticks; //";
                if (overflowedByZeroes) {
                    Console.WriteLine($"{command} OverflowException due to zeroes. {nonredundant} d.p. + {redundant} zeroes. Ticks:{tspTicks}");
                } else if (earlyOverflow) {
                    Console.WriteLine($"{commandTicks} OverflowException, but could have returned {tspTicks}");
                } else if (wrongCount) {
                    Console.WriteLine($"{commandTicks} == {ticks} but should give {tspTicks}");
                }

                //Console.WriteLine($"[DataRow(\"{text}\", {tspTicks}, {tspSuccess}, {ticks}, {success})] {dp} // {(success ? "OK" : "OVERFLOW")}");

                
            }
        }

        [TestMethod]
        [DataRow("Inf:00:00")]
        [DataRow("-Inf:00:00")]
        [DataRow("10:-Inf:00")]
        [DataRow("10:00:Inf")]
        [DataRow("10:00:.Inf")]
        [DataRow("10:00:.-Inf")]
        [DataRow("10:NaN:00")]
        [DataRow("10 Inf seconds")]
        [DataRow("Inf seconds")]
        [DataRow("-Inf seconds")]
        [DataRow("NaN seconds")]
        public void NotANumberInfinityTest(string parseThis) {
            TimeSpanParser.TryParse(parseThis, out var timeSpan); // only for displaying in the error

            Assert.ThrowsException<ArgumentException>(() => TimeSpanParser.Parse(parseThis), 
                $"Maybe random strings in the middle of a time should raise exceptions. {parseThis} parsed as: '{timeSpan}'");
        }


        // "The smallest unit of time is the tick, which is equal to 100 nanoseconds or one ten-millionth of a second. There are 10,000 ticks in a millisecond."
        // -- https://msdn.microsoft.com/en-us/library/system.timespan.ticks(v=vs.110).aspx
        // "ff - Optional fractional seconds, consisting of one to seven decimal digits."
        // -- https://docs.microsoft.com/en-us/dotnet/api/system.timespan.parse?redirectedfrom=MSDN&view=netcore-2.1#System_TimeSpan_Parse_System_String_

        // Decimals with 8 digits are buggy in .NET Core 2.x. 
        // Reported and discussed in much detail here:
        // https://github.com/dotnet/coreclr/pull/21077
        // It was fixed for .Net Core 3.0:
        // https://github.com/dotnet/coreclr/pull/21968
        [TestMethod]
        [DataRow("    100000 ps", "0:00:00.0000001", 1, true)]    // ok
        [DataRow("     10000 ps", "0:00:00.00000001", 0, false)]  // ought to "overflow" or truncate to zero, but instead TimeSpan.Parse() returns 1 tick
        [DataRow("      1000 ps", "0:00:00.000000001", 0, false)] // overflow

        [DataRow("       100 ns", "0:00:00.0000001", 1, true)]    // ok 
        [DataRow("        10 ns", "0:00:00.00000001", 0, false)]  // ought to overflow (or truncate to zero) 
        [DataRow("        90 ns", "0:00:00.00000009", 0, false)]  // TimeSpan.Parse() overflows as expected
        [DataRow("         1 ns", "0:00:00.000000001", 0, false)] // overflow

        [DataRow("        .1 μs", "0:00:00.0000001", 1, true)]    // ok
        [DataRow("       .01 μs", "0:00:00.00000001", 0, false)]  // ought to overflow
        [DataRow("      .001 μs", "0:00:00.000000001", 0, false)] // does overflow

        [DataRow("     .0001 ms", "0:00:00.0000001", 1, true)]    // ok
        [DataRow("    .00001 ms", "0:00:00.00000001", 0, false)]  // ought to overflow
        [DataRow("   .000001 ms", "0:00:00.000000001", 0, false)] // overflow

        [DataRow("  .0000001 s ", "0:00:00.0000001", 1, true)]    // ok
        [DataRow(" .00000001 s ", "0:00:00.00000001", 0, false)]  // ought to overflow 
        [DataRow(".000000001 s ", "0:00:00.000000001", 0, false)] // overflow
        public void OverflowExceptionTest(string tsp, string traditional, int ticks, bool successExpected) {
            if (successExpected) {
                var expected = TimeSpan.Parse(traditional);
                var actual = TimeSpanParser.Parse(tsp);

                Console.WriteLine($"actual:");
                Console.WriteLine(tsp);
                Console.WriteLine($"parsed: {actual:G}");
                Console.WriteLine($" ticks: {actual.Ticks}");

                Console.WriteLine($"expected:");
                Console.WriteLine(traditional);
                Console.WriteLine($"parsed: {expected:G}");
                Console.WriteLine($" ticks: {expected.Ticks}");

                Assert.AreEqual(expected, actual);
                Assert.AreEqual(ticks, expected.Ticks);

            } else {
                try {
                    var parsed = TimeSpan.Parse(traditional);
                    Console.WriteLine(tsp);
                    Console.WriteLine($"expected:");
                    Console.WriteLine(traditional);
                    Console.WriteLine($"parsed: {parsed:G}");
                    Console.WriteLine($" ticks: {parsed.Ticks}");
                } catch (OverflowException e) { }

                // TimeSpan.Parse() does not reliably throw OverflowExceptions, so ignore this Assert
                //Assert.ThrowsException<OverflowException>(() => TimeSpan.Parse(traditional));

                Assert.ThrowsException<OverflowException>(() => TimeSpanParser.Parse(tsp));
            }


        }
    }
}