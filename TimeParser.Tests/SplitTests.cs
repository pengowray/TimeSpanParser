using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class SplitTests
    {
        [TestMethod]
        public void BigToLittleViolationTest() {

            Assert.AreEqual(
                TimeSpanParser.Parse("10 minutes 40 seconds 7 days 1 hour"), 
                TimeSpan.Parse("00:10:40"));
        }

        [TestMethod]
        public void LittleToBigSplitTest() {

            var success = TimeSpanParser.TryParse("10 minutes 40 seconds 7 days 1 hour", out TimeSpan[] timeSpans);

            Assert.IsTrue(success);
            Assert.AreEqual(timeSpans.Length, 2);
            Assert.AreEqual(timeSpans[0], TimeSpan.Parse("0.00:10:40"));
            Assert.AreEqual(timeSpans[1], TimeSpan.Parse("7.01:00:00"));
        }

        [TestMethod]
        public void LittleToBigColonSplitTest() {

            var success = TimeSpanParser.TryParse("10:40h 7:45h", out TimeSpan[] timeSpans);

            Assert.IsTrue(success);
            Assert.AreEqual(timeSpans.Length, 2);
            Assert.AreEqual(timeSpans[0], TimeSpan.Parse("10:40:00"));
            Assert.AreEqual(timeSpans[1], TimeSpan.Parse("7:45:00"));
        }

        [TestMethod]
        public void RepeatedSplitTest() {
            var success = TimeSpanParser.TryParse("10 minutes 40 seconds 20 minutes 30 seconds", out TimeSpan[] timeSpans);

            Assert.IsTrue(success);
            Assert.AreEqual(timeSpans.Length, 2);
            Assert.AreEqual(timeSpans[0], TimeSpan.Parse("00:10:40"));
            Assert.AreEqual(timeSpans[1], TimeSpan.Parse("00:20:30"));
        }

        [TestMethod]
        public void AntiSplitTest1() {
            var options = new TimeSpanParserOptions();
            options.StrictBigToSmall = false;

            var success = TimeSpanParser.TryParse("10 minutes 15 seconds 20 minutes 30 seconds", out TimeSpan[] timeSpans, options);

            Assert.IsTrue(success);
            Assert.AreEqual(timeSpans.Length, 1);
            Assert.AreEqual(timeSpans[0], TimeSpan.Parse("00:30:45"));
        }

        [TestMethod]
        public void AntiSplitTest2() {
            var options = new TimeSpanParserOptions();
            options.StrictBigToSmall = false;

            var success = TimeSpanParser.TryParse("10 days 15 seconds 20 hours 30 minutes", out TimeSpan[] timeSpans, options);

            Assert.IsTrue(success);
            Assert.AreEqual(timeSpans.Length, 1);
            Assert.AreEqual(timeSpans[0], TimeSpan.Parse("10.20:30:15"));
        }

        [TestMethod]
        public void AntiSplitTest3() {
            var options = new TimeSpanParserOptions();
            options.StrictBigToSmall = false;

            var success = TimeSpanParser.TryParse("10 days 15 seconds 20:00:00 10:00 minutes 00:20:00", out TimeSpan[] timeSpans, options);

            Assert.IsTrue(success);
            Assert.AreEqual(timeSpans.Length, 1);
            Assert.AreEqual(timeSpans[0], TimeSpan.Parse("10.20:30:15"));
        }

    }
}
