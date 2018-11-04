using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class OptionsColonedDefaultTests {
        //TODO: also make a test for numberFormatInfo.NumberGroupSeparator

        //TODO: 

        [TestMethod]
        [DataRow(Units.Days, "0:00", "0")]
        [DataRow(Units.Months, "0:00", "0")]
        [DataRow(Units.Years, "0:00", "0")]
        [DataRow(Units.Days, "0.0:0.0", "0")]
        [DataRow(Units.Months, "0.0:0.0", "0")]
        [DataRow(Units.Years, "0.0:0.0", "0")]
        [DataRow(Units.Days, "100:00", "100:0:0:0")]
        [DataRow(Units.Days, "-100:12", "-100:12:0:0")]
        [DataRow(Units.Hours, "5:00", "5:00:00")]
        [DataRow(Units.Hours, "-5:30", "-5:30:00")]
        [DataRow(Units.Hours, "24:00", "1:00:00:00")]
        [DataRow(Units.Minutes, "10:00", "00:10:00")]
        [DataRow(Units.Minutes, "-10:30", "-0:10:30")]
        //TODO: maybe disallow "ColonedDefault: seconds" ?
        [DataRow(Units.Seconds, "35:00", "0:35:00")] // promoted to minutes even though ends in 0's
        [DataRow(Units.Seconds, "-00:35.5", "-0:00:35.5")] // promoted to minutes
        [DataRow(Units.Seconds, "-00:00:35.5", "-0:00:35.5")] // promoted to hours
        [DataRow(Units.Seconds, "-0:00:00:35.5", "-0:00:35.5")] // promoted to days
        [DataRow(Units.Seconds, "-0.00:00:35.5", "-0:00:35.5")] // promoted to days (. separator)
        [DataRow(Units.Milliseconds, "00:00.642", "0:00:00.642")] // promoted to minutes
        [DataRow(Units.Milliseconds, "-00:642.123", "-0:00:00.642123")] // promoted to minutes
        [DataRow(Units.Nanoseconds, "00:00.0000001", "0:00:00.0000001")]  // promoted to minutes
        // test it doesn't effect uncoloned numbers (todo)
        //[DataRow(Units.Minutes, "10.5", "10:30:00")]
        //[DataRow(Units.Seconds, "10:5", "10:50:00")]
        //[DataRow(Units.Nanoseconds, "10:50", "10:50:00")]
        // "0" should be fine with years or months or without units
        [DataRow(Units.None, "0:00", "0")]
        [DataRow(Units.None, "0:00:00", "0")]
        [DataRow(Units.None, "0:00:00:00", "0")]
        public void ColonedDefaultTests(Units units, string input, string oldschool) {
            var options = new TimeSpanParserOptions();
            options.ColonedDefault = units;

            var timeSpanParser = TimeSpanParser.Parse(input, options);
            var builtInParser = TimeSpan.Parse(oldschool);

            Assert.AreEqual(builtInParser, timeSpanParser);
        }

        [TestMethod]
        [DataRow(Units.Years, "1:00")]
        [DataRow(Units.Years, "-1:00")]
        [DataRow(Units.Months, "1:00")]
        [DataRow(Units.Months, "-1:00")]
        [DataRow(Units.None, "1:00")]
        [DataRow(Units.None, "-1:00")]
        [DataRow(Units.Error, "0:00")]
        [DataRow(Units.Error, "1:00")]
        [DataRow(Units.Error, "-1:00")]
        [DataRow(Units.ErrorAmbiguous, "0:00")] // pretty sure we even use Units.ErrorAmbiguous any more?
        [DataRow(Units.ErrorAmbiguous, "1:00")]
        [DataRow(Units.ErrorTooManyUnits, "0:00")] // might not even need this any more
        [DataRow(Units.ErrorTooManyUnits, "1:00")]
        //TODO: test None doesn't effect coloned
        public void ColonedDefaultFailTests(Units units, string input) {
            var options = new TimeSpanParserOptions();
            options.ColonedDefault = units;

            Assert.ThrowsException<ArgumentException>(() => TimeSpanParser.Parse(input, options));
        }


    }
}
