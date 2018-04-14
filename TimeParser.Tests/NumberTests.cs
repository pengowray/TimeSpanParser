using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class NumberTests
    {
        //TODO: more

        [TestMethod]
        //[DataRow("5", "en-US", 5)]
        [DataRow("5,000", 5000)]
        [DataRow("5,000.50", 5000.5)]
        public void GuideBasicsTests(string input, double seconds) {
            var options = new TimeSpanParserOptions();
            options.UncolonedDefault = Units.Seconds;

            var timeSpanParser = TimeSpanParser.Parse(input, options);
            var builtInParser = TimeSpan.FromSeconds(seconds);

            Assert.AreEqual(builtInParser, timeSpanParser);
        }


    }
}
