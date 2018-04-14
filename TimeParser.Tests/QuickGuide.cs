using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class QuickGuide {

        // TimeSpanParser can read English natural-language timespans.

        [TestMethod]
        [DataRow("5 mins", "00:05:00")] // such as "5 mins" (our TimeSpanParser.Parse vs .NET TimeSpan.Parse)
        [DataRow("29 minutes 51 seconds", "00:29:51")] // slightly more complex things
        [DataRow("2h10m58s", "2:10:58")]  // this kinda format
        [DataRow("2h:10m:58s", "2:10:58")]  // and this even odd way of writing things
        [DataRow("2h:58s", "2:00:58")]  // it does as good as it can at parsing things
        [DataRow("2h:58.5s", "2:00:58.5")] 
        [DataRow("1 day 2:10h 58.2s", "1.2:10:58.2")]  // it can take a mix of styles
        [DataRow("1:00", "1:00:00")]  // by default it handles these sorts of numbers as hours
        [DataRow("1:00", "1:00")]  // as does TimeSpan.Parse 
        [DataRow("3.17:25:30.5000000", "3.17:25:30.5000000")] // it also reads whatever TimeSpan.Parse() does... except localization. (TODO)
        public void GuideBasicsTests(string input, string basicInput)  {
            var timeSpanParser = TimeSpanParser.Parse(input);
            var builtInParser = TimeSpan.Parse(basicInput);

            Assert.AreEqual(builtInParser, timeSpanParser);
        }

        [TestMethod]
        public void GuideSettingOptionsTests1() {
            // By default "coloned" numbers give hours
            var oneHour = TimeSpan.FromHours(1);
            Assert.AreEqual(TimeSpanParser.Parse("1:00"), oneHour);

            // If we don't have a colon, then the units are required
            Assert.AreEqual(TimeSpanParser.Parse("1 hour"), oneHour);
            Assert.AreEqual(TimeSpanParser.Parse("1h"), oneHour);

            // Attempting to parse without any units will throw an exception of some kind (TODO: better exception names / messages)
            try {
                TimeSpanParser.Parse("1");
                Assert.Fail("Above should throw an exception before this line is reached.");
            } catch { }


        }

        [TestMethod]
        public void GuideSettingOptionsTests2() {

            // ...Unless you set options.UncolonedDefault = Units.Minutes.
            var options = new TimeSpanParserOptions();
            options.UncolonedDefault = Units.Minutes;
            Assert.AreEqual(TimeSpanParser.Parse("1", options), TimeSpan.FromMinutes(1));

            //UncolonedDefault is what the parseer will interpret a number as by default. e.g. if seconds, then text of "3" becomes 3 seconds
            options.UncolonedDefault = Units.Seconds;
            Assert.AreEqual(TimeSpanParser.Parse("3", options), TimeSpan.FromSeconds(3));

            // if Units.None is chosen (the default for uncoloned numbers) then parsing will fail if the user has not included a unit
            options.UncolonedDefault = Units.None;
            try {
                TimeSpanParser.Parse("1", options);
                Assert.Fail("Above should throw an exception before this line is reached.");
            } catch { }

            // The special exception is zero, which can be unitless.
            options.UncolonedDefault = Units.None;
            options.ColonedDefault = Units.None;
            Assert.AreEqual(TimeSpanParser.Parse("0", options), TimeSpan.Zero);
            Assert.AreEqual(TimeSpanParser.Parse("0:00:00", options), TimeSpan.Zero);

            // Zero is just accepted no matter what
            Assert.AreEqual(TimeSpanParser.Parse("0.00:00:00:00:00:00.00", options), TimeSpan.Zero);

            // ... Unless we choose not to.
            options.AllowUnitlessZero = false;
            try {
                TimeSpanParser.Parse("0", options);
                Assert.Fail("Above should throw an exception before this line is reached.");
            } catch { }

            //You can set the default for coloned timespans, e.g. if you're expecting the user to input minutes rather than hours
            //ColonedDefault is what to interpret a number containing a colon as by default. e.g. if minutes, then "1:00" is parsed as 1 minute.
            options.ColonedDefault = Units.Minutes;
            Assert.AreEqual(TimeSpanParser.Parse("1:00", options), TimeSpan.FromMinutes(1));

            // It will automatically switch to hours again if a number contains too many colons for minutes to make sense
            Assert.AreEqual(TimeSpanParser.Parse("1:00:00", options), TimeSpan.FromHours(1));

            //TODO: split into new test or create new options

            // Unless you disallow that
            options.AutoUnitsIfTooManyColons = false;
            try {
                TimeSpanParser.Parse("1:00:00", options);
                Assert.Fail("Above should throw an exception before this line is reached.");
            } catch { }

            //TODO: split into a new test

            //UncolonedDefault and colonedDefault are only used for the first number found in the text. It would be weird otherwise. 
            options.UncolonedDefault = Units.Minutes;
            Assert.AreEqual(TimeSpanParser.Parse("3 33s", options), TimeSpan.Parse("00:03:33"));

            //Subsequent numbers require their own units
            try {
                TimeSpanParser.Parse("13h 10", options);
                Assert.Fail("Above should throw an exception before this line is reached.");
            } catch { }

        }

        [TestMethod]
        public void GuideSettingOptionsTests3() {
            // Note that days, hour, minutes and seconds must be in big-to-small order (just as English-speakers always write them).
            Assert.AreEqual(TimeSpanParser.Parse("7 days 1 hour 10 minutes 40 seconds"), TimeSpan.Parse("7.1:10:40"));

            // And parsing will stop when the order is broken (here "7 days 1 hour" is ignored)
            Assert.AreEqual(TimeSpanParser.Parse("10 minutes 40 seconds 7 days 1 hour"), TimeSpan.Parse("00:10:40"));

            // ...Unless you turn off "StrictBigToSmall"
            var options = new TimeSpanParserOptions();
            options.StrictBigToSmall = false;
            Assert.AreEqual(TimeSpanParser.Parse("10 minutes 40 seconds 7 days 1 hour", options), TimeSpan.Parse("7.1:10:40"));
        }

    }
}
