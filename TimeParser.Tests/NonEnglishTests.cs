using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    class NonEnglishTests
    {
        [TestMethod]
        public void NonEnglishDehumanizeTimeSpanSK() {
            // Test roundtripping of Humanizer examples
            Assert.AreEqual(
                TimeSpan.FromMilliseconds(1),
                TimeSpanParser.Parse("1 milisekunda"));

            Assert.AreEqual(
                TimeSpan.FromMilliseconds(2),
                TimeSpanParser.Parse("2 milisekundy"));

            Assert.AreEqual(
                TimeSpan.FromMilliseconds(5),
                TimeSpanParser.Parse("5 milisekúnd"));

        }

        [TestMethod]
        public void NonEnglishDehumanizeTimeSpanDE() {
            // Test roundtripping of Humanizer examples
            Assert.AreEqual(
                TimeSpan.FromDays(1),
                TimeSpanParser.Parse("Ein Tag"));

            Assert.AreEqual(
                TimeSpan.FromDays(2),
                TimeSpanParser.Parse("2 Tage"));
        }

    }
}
