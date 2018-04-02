using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class EnglishNumberWordsTest {

        //TODO
        [TestMethod]
        public void EnglishDehumanizeWords() {
            Assert.AreEqual(
                new TimeSpan(15, 3, 0, 0, 0),
                TimeSpanParser.Parse("two weeks, one day, three hours"));
        }

        //TODO
        [TestMethod]
        public void EnglishWordExamples() {
            var expected = new TimeSpan(3, 18, 0);
            Assert.AreEqual(expected, TimeSpanParser.Parse("three hours and eighteen minutes"));
        }

    }

}