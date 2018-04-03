using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

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
            bool success = TimeSpanParser.TryParse(parseThis, out TimeSpan actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("0")]
        [DataRow("0:00")]
        [DataRow("0:0:0:0:0:0")]
        [DataRow("0 0 0 0 0")] // everything after first 0 should be ignored
        [DataRow("0000.00000000:00.0:0.00:00.00")]
        public void ZeroOnlyWeirdnessTest(string parseThis) {
            Console.WriteLine(parseThis);

            var expected = TimeSpan.Zero;
            bool success = TimeSpanParser.TryParse(parseThis, out TimeSpan actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("0000.00000000:00.0:0.00:00:00:00:.00.1")]
        [DataRow("0:0:0:0:0:0:2")]
        public void ZeroOnlyFailTest(string parseThis) {
            Console.WriteLine(parseThis);

            bool success = TimeSpanParser.TryParse(parseThis, out TimeSpan actual);
            Assert.IsFalse(success);
        }


        [TestMethod]
        [DataRow("-")]
        [DataRow("")]
        [DataRow(".")]
        [DataRow("/")]
        [DataRow(". . . . .")]
        [DataRow(null)]
        public void NothingTest(string parseThis) {

            bool success = TimeSpanParser.TryParse(parseThis, out TimeSpan actual); ;

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void MinTest() {
            string MinimumTimeSpan = "-10675199.02:48:05.4775808";
            bool success = TimeSpanParser.TryParse(MinimumTimeSpan, out TimeSpan actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(TimeSpan.MinValue, actual);
        }
        
        [TestMethod]
        public void MaxTest() {
            string MaximumTimeSpan = "10675199:02:48:05.4775807";
            //string MaximumTimeSpan = "10675199.02:48:05.4775807";

            var expected1 = TimeSpan.Parse(MaximumTimeSpan);
            var expected2 = TimeSpan.MaxValue;

            TimeSpan actual;
            bool success = TimeSpanParser.TryParse(MaximumTimeSpan, out actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(expected1, actual);
        }

        [TestMethod]
        public void NearMaxTest() {
            //string NearMaximumTimeSpan = "111111111:11:11:11.1111111";
            string NearMaximumTimeSpan = "1111111.11:11:11.1111111";

            var expected = TimeSpan.Parse(NearMaximumTimeSpan);
            TimeSpan actual;
            bool success = TimeSpanParser.TryParse(NearMaximumTimeSpan, out actual); ;

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void NotWrittenHereMaxTest() {
            var expected1 = TimeSpan.MaxValue;
            string MaximumTimeSpan1 = "10675199.02:48:05.4775807";
            var expected2 = TimeSpan.Parse(MaximumTimeSpan1);
            string MaximumTimeSpan2 = expected2.ToString();
            var expected3 = TimeSpan.Parse(MaximumTimeSpan2);

            Assert.AreEqual(expected1, expected2);
            Assert.AreEqual(expected2, expected3);
            Assert.AreEqual(expected1, expected3);

            Assert.AreEqual(MaximumTimeSpan1, MaximumTimeSpan2);
        }

        [TestMethod]
        public void NotWrittenHereMinTest() {
            var expected1 = TimeSpan.MinValue;
            string MinimumTimeSpan1 = "-10675199.02:48:05.4775808"; ;
            var expected2 = TimeSpan.Parse(MinimumTimeSpan1);
            string MinimumTimeSpan2 = expected2.ToString();
            var expected3 = TimeSpan.Parse(MinimumTimeSpan2);

            Assert.AreEqual(expected1, expected2);
            Assert.AreEqual(expected2, expected3);
            Assert.AreEqual(expected1, expected3);

            Assert.AreEqual(MinimumTimeSpan1, MinimumTimeSpan2);
        }

    }
}
