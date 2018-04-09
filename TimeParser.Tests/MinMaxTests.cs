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
        [DataRow("00:00:00")] // default value
        [DataRow("0:00:00:00")]
        [DataRow("0:00:00:00.00000")]
        [DataRow("0:0:0")]
        [DataRow("0")]
        [DataRow("-0")]
        [DataRow("-0:0:0")]
        [DataRow("-00:00:00")]
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
        [DataRow("")]
        [DataRow(".")]
        [DataRow("/")]
        [DataRow(". . . . .")]
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




    }
}
