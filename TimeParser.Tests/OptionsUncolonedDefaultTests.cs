using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class OptionsUncolonedDefaultTests
    {
        //TODO: also make a test for numberFormatInfo.NumberGroupSeparator

        //TODO: 

        [TestMethod]
        [DataRow(Units.Days, "0", "0")]
        [DataRow(Units.Days, "100", "100:0:0:0")]
        [DataRow(Units.Days, "-100.5", "-100:12:0:0")]
        [DataRow(Units.Hours, "5", "5:00:00")]
        [DataRow(Units.Hours, "-5.5", "-5:30:00")]
        [DataRow(Units.Hours, "24", "1:00:00:00")]
        [DataRow(Units.Minutes, "10", "00:10:00")]
        [DataRow(Units.Minutes, "-10.5", "-0:10:30")]
        [DataRow(Units.Seconds, "35", "0:00:35")]
        [DataRow(Units.Seconds, "-35.5", "-0:00:35.5")]
        [DataRow(Units.Milliseconds, "642", "0:00:00.642")]
        [DataRow(Units.Milliseconds, "-642.123", "-0:00:00.642123")]
        [DataRow(Units.Milliseconds, "1", "0:00:00.001")]
        [DataRow(Units.Microseconds, "1", "0:00:00.000001")]
        [DataRow(Units.Microseconds, "-642123", "-0:00:00.642123")]
        [DataRow(Units.Nanoseconds, "100", "0:00:00.0000001")] // 100ns == 1 tick (finest resolution). See MinMaxTests.OverflowExceptionTest() for more.
        [DataRow(Units.Nanoseconds, "-53100", "-0:00:00.00000531")]
        // test it doesn't effect coloned numbers
        [DataRow(Units.Minutes, "10:50", "10:50:00")] 
        [DataRow(Units.Seconds, "10:50", "10:50:00")]
        [DataRow(Units.Nanoseconds, "10:50", "10:50:00")]
        // "0" should be fine with years or months or without units (by default)
        [DataRow(Units.None, "0", "0")]
        [DataRow(Units.Months, "0", "0")]
        [DataRow(Units.Years, "0", "0")]
        public void UncolonedDefaultTests(Units units, string input, string oldschool) {
            var options = new TimeSpanParserOptions();
            options.UncolonedDefault = units;

            var timeSpanParser = TimeSpanParser.Parse(input, options);
            var builtInParser = TimeSpan.Parse(oldschool);

            Assert.AreEqual(builtInParser, timeSpanParser);
        }

        [TestMethod]
        [DataRow(Units.Years, "1")]
        [DataRow(Units.Years, "-1")]
        [DataRow(Units.Months, "1")]
        [DataRow(Units.Months, "-1")]
        [DataRow(Units.None, "1")]
        [DataRow(Units.None, "-1")]
        [DataRow(Units.Error, "0")]
        [DataRow(Units.Error, "1")]
        [DataRow(Units.Error, "-1")]
        [DataRow(Units.ErrorAmbiguous, "0")] // pretty sure we even use Units.ErrorAmbiguous any more?
        [DataRow(Units.ErrorAmbiguous, "1")]
        [DataRow(Units.ErrorTooManyUnits, "0")] // might not even need this any more
        [DataRow(Units.ErrorTooManyUnits, "1")]
        public void UncolonedDefaultFailTests(Units units, string input) {
            var options = new TimeSpanParserOptions();
            options.UncolonedDefault = units;

            Assert.ThrowsException<ArgumentException>(() => TimeSpanParser.Parse(input, options));
        }


    }
}
