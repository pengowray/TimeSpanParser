using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class WeirdCharactersTests
    {
        [TestMethod]
        [DataRow("3.17:25:30.5", 3, 17, 25, 30, 500)] // ascii
        [DataRow("3.17︓25︓30.5", 3, 17, 25, 30, 500)] // U+FE13 (presentation form for vertical colon)
        [DataRow("3．17：25：30．5", 3, 17, 25, 30, 500)] // U+FF0E (fullwidth full stop) + U+FF1A (fullwidth colon)
        [DataRow("﻿３．１７：２５：３０．５", 3, 17, 25, 30, 500)] // all fullwidth
        [DataRow("3.17﹕25﹕30.5", 3, 17, 25, 30, 500)] // small colon (U+FE55)
        public void UnicodeFullWidthAndOtherTest(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            TimeSpan actual = TimeSpanParser.Parse(parseThis);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("1e3 s", 0, 0, 0, 1000, 0)] // ascii
        [DataRow("1E3 s", 0, 0, 0, 1000, 0)] // caps
        [DataRow("1ℯ3 s", 0, 0, 0, 1000, 0)] // ℯ exponent character (U+212F)
        public void UnicodeExponentTest(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            TimeSpan actual = TimeSpanParser.Parse(parseThis);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("3_days_18_seconds", 3, 0, 0, 18, 0)] // underscore is treated as a character for regex word boundries (\b) and words (\w)
        public void UnderscoreTest(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            TimeSpan actual = TimeSpanParser.Parse(parseThis);

            //note: main parser converts all underscores to spaces
            //TODO: test underscores in key prefixes

            Assert.AreEqual(expected, actual);
        }

    }
}
