using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class GeneralAndMiscTests {

        [TestMethod]
        public void DehumanizeTimeSpan() {
            // Test roundtripping of Humanizer examples

            Assert.AreEqual(
                TimeSpan.FromMilliseconds(1),
                TimeSpanParser.Parse("1 millisecond")); ;

            Assert.AreEqual(
                TimeSpan.FromMilliseconds(2),
                TimeSpanParser.Parse("2 milliseconds")); ;
        }

        [TestMethod]
        public void DehumanizeTimeSpanComplex() {
            Assert.AreEqual(
                TimeSpan.FromMilliseconds(1299630020),
                TimeSpanParser.Parse("2 weeks, 1 day, 1 hour, 30 seconds, 20 milliseconds"));
        }

        [TestMethod]
        public void CapsTests() {
            Assert.AreEqual(TimeSpan.FromHours(2), TimeSpanParser.Parse("2 hours"));
            Assert.AreEqual(TimeSpan.FromHours(2), TimeSpanParser.Parse("2 HOURS"));
            Assert.AreEqual(TimeSpan.FromHours(2), TimeSpanParser.Parse("  2 Hrs  "));
            Assert.AreEqual(TimeSpan.FromHours(2), TimeSpanParser.Parse("2 hOUr  "));
            Assert.AreEqual(TimeSpan.FromMinutes(2), TimeSpanParser.Parse("  2 MiNuTeS"));
        }


        [TestMethod]
        [DataRow("1:08:18:10", "1:08:18:10")]
        [DataRow("32:18:10", "1:08:18:10")]
        [DataRow("32:18h 10s", "1:08:18:10")] // "00:32:18:10"
        [DataRow("2:18h 10s", "2:18:10")]
        [DataRow("0:0:0.001", "0:0:0.001")]
        public void LargeColonedNumbers(string parseThis, string equalThis) {
            var expected = TimeSpan.Parse(equalThis);
            Assert.AreEqual(expected, TimeSpanParser.Parse(parseThis));
        }

        [TestMethod]
        [DataRow("8.64e+6 seconds", "100:0:0:0")]
        [DataRow("1 wk", "7:0:0:0")]
        [DataRow("1 ms", "0:0:0.001")]
        public void PeculiarNocolonsTest(string parseThis, string equalThis) {
            var expected = TimeSpan.Parse(equalThis);
            Assert.AreEqual(expected, TimeSpanParser.Parse(parseThis));
        }

        [TestMethod]
        [DataRow("-3:18:00", "-3:18:00")]
        [DataRow("-3h18m", "-3:18:00")]
        public void NegativeTests(string parseThis, string equalThis) {
            var expected = TimeSpan.Parse(equalThis);
            Assert.AreEqual(expected, TimeSpanParser.Parse(parseThis));
        }

        [TestMethod]
        //[TestCase("3:18:00")]
        [DataRow("0:3:18:00")] // AutoAdjusts from hours to days
        [DataRow("3:18:00")]
        [DataRow("3:18 hours")]
        [DataRow("3h18m")]
        [DataRow("3h 18m")]
        [DataRow("3.3hrs")]
        [DataRow("3.3 hour(s)")]
        [DataRow("3:18")]
        [DataRow("3 hours, 18 minutes")]
        [DataRow("3:18h 0s")]
        [DataRow("0.125 days and 1080 seconds")]
        [DataRow("0.125:00:00:1080.00 days")]
        public void MoreExamples(string parseThis) {
            var expected = new TimeSpan(3, 18, 0);
            Assert.AreEqual(expected, TimeSpanParser.Parse(parseThis));
        }

        [TestMethod]
        [DataRow("18:01")]
        public void DefaultUnitsMinutesTests(string parseThis) {
            var expected = new TimeSpan(0, 18, 1);
            Assert.AreEqual(expected, TimeSpanParser.Parse(parseThis, Units.Minutes, Units.Minutes));
        }


    }

}