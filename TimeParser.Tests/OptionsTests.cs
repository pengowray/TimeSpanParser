using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class OptionsTests
    {
        //TODO: include also numberFormatInfo.NumberGroupSeparator



        [TestMethod]
        [DataRow(Units.Days, "0", "0")]
        [DataRow(Units.Months, "0", "0")]
        [DataRow(Units.Days, "0", "0")]
        [DataRow(Units.Days, "100", "100:0:0:0")]
        [DataRow(Units.Days, "-100.5", "-100:12:0:0")]
        public void UncolonedDefaultTests(Units units, string input, string oldschool) {
            var options = new TimeSpanParserOptions();
            options.UncolonedDefault = units;

            var timeSpanParser = TimeSpanParser.Parse(input, options);
            var builtInParser = TimeSpan.Parse(oldschool);

            Assert.AreEqual(builtInParser, timeSpanParser);
        }

        [TestMethod]
        [DataRow(Units.Years, "1")]
        [DataRow(Units.Months, "1")]
        [DataRow(Units.Error, "0")]
        [DataRow(Units.Error, "1")]
        [DataRow(Units.ErrorAmbiguous, "0")] // pretty sure we even use Units.ErrorAmbiguous any more?
        [DataRow(Units.ErrorAmbiguous, "1")]
        [DataRow(Units.ErrorTooManyUnits, "0")]
        [DataRow(Units.ErrorTooManyUnits, "1")]
        public void UncolonedDefaultFailTests(Units units, string input) {
            var options = new TimeSpanParserOptions();
            options.UncolonedDefault = units;

            Assert.ThrowsException<ArgumentException>(() => TimeSpanParser.Parse(input, options));
        }


    }
}
