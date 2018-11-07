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

        // Compare some of the input formats TimeSpanParser accepts (left), to dotnet's built-in TimeSpan parser (right)

        [TestMethod]
        [DataRow("5 mins", "00:05:00")] // such as "5 mins" (our TimeSpanParser.Parse vs .NET TimeSpan.Parse())
        [DataRow("29 minutes 51 seconds", "00:29:51")] // slightly more complex things
        [DataRow("2h10m58s", "2:10:58")]  // this kinda format
        [DataRow("2h:10m:58s", "2:10:58")]  // and this even odd way of writing things
        [DataRow("2h:58s", "2:00:58")]  // it does as good as it can at parsing strings
        [DataRow("1 day 2:10h 58.2s", "1.2:10:58.2")]  // it can take a mix of styles
        [DataRow("1:00", "1:00:00")]  // by default it treats this as "1 hour"
        [DataRow("1:00", "1:00")]  // as does .NET's TimeSpan.Parse()
        [DataRow("3.17:25:30.5000000", "3.17:25:30.5000000")] // it parses whatever TimeSpan.Parse() does. [Except localization. (TODO)]
        public void GuideBasicsTests(string input, string basicInput)  {

            var timeSpanParser = TimeSpanParser.Parse(input);
            var builtInParser = TimeSpan.Parse(basicInput);

            Assert.AreEqual(builtInParser, timeSpanParser);
        }

        [TestMethod]
        public void GuideSettingOptionsTests1() {
            // Numbers containing a single colon are interpreted as hh:mm
            Assert.AreEqual(
                TimeSpanParser.Parse("1:00"),
                TimeSpan.FromHours(1));

            // If we don't have a colon, then the units are required
            Assert.AreEqual(
                TimeSpanParser.Parse("1 hour"),
                TimeSpan.FromHours(1));

            Assert.AreEqual(
                TimeSpanParser.Parse("1h"),
                TimeSpan.FromHours(1));

            // Attempting to parse a number without any units will throw an exception (TODO: better exception names / messages)
            try {
                TimeSpanParser.Parse("1");
                Assert.Fail("Above will throw an exception before this line is reached.");
            } catch { }

            // ...Unless you set change the units from Units.None to some other Units
            // Here we take the input of "2" to mean "2 minute"
            Assert.AreEqual(
                TimeSpanParser.Parse("2", new TimeSpanParserOptions() { UncolonedDefault = Units.Minutes }), 
                TimeSpan.FromMinutes(2));

            // "UncolonedDefault" is what the parseer will interpret a number as by default. e.g. if seconds, then text of "5" becomes 5 seconds
            Assert.AreEqual(
                TimeSpanParser.Parse("5", new TimeSpanParserOptions() { UncolonedDefault = Units.Seconds }), 
                TimeSpan.FromSeconds(5));

            // The special exception is zero, which can be unitless.
            Assert.AreEqual(
                TimeSpanParser.Parse("0", new TimeSpanParserOptions() { UncolonedDefault = Units.None }),
                TimeSpan.Zero);

            // The same for "coloned" numbers
            Assert.AreEqual(
                TimeSpanParser.Parse("00:00", new TimeSpanParserOptions() { ColonedDefault = Units.None }),
                TimeSpan.Zero);

            // The user can go nuts with zeros and TimeSpanParser will understand it to mean zero time.
            Assert.AreEqual(
                TimeSpanParser.Parse("0.00:00:00:00:00:00.00"), 
                TimeSpan.Zero);

            // ... Unless you choose to be strict about it
            try {
                TimeSpanParser.Parse("0", new TimeSpanParserOptions() { AllowUnitlessZero = false });
                Assert.Fail("Above will throw an exception before this line is reached.");
            } catch { }

            // You can set the default for coloned timespans, e.g. if you're expecting the user to input minutes rather than hours.
            // ColonedDefault is what to interpret a number containing a colon as by default. e.g. if minutes, then "1:00" is parsed as 1 minute.
            // [TODO: isn't this repeated?]
            Assert.AreEqual(
                TimeSpanParser.Parse("5:00", new TimeSpanParserOptions() { ColonedDefault = Units.Minutes } ),
                TimeSpan.FromMinutes(5));

            // It will automatically switch to hours again if a number contains "too many" colons (a second colon), as the string can no longer be interpreted as minutes
            Assert.AreEqual(
                TimeSpanParser.Parse("2:00:00", new TimeSpanParserOptions() { ColonedDefault = Units.Minutes }), 
                TimeSpan.FromHours(2));

            // ...unless you want to be strict about that
            try {
                TimeSpanParser.Parse("2:00:00", new TimeSpanParserOptions() { ColonedDefault = Units.Minutes, AutoUnitsIfTooManyColons = false });
                Assert.Fail("Above should throw an exception before this line is reached.");
            } catch { }

            // UncolonedDefault and ColonedDefault are only used for the first number found in the string. It would be weird otherwise.
            Assert.AreEqual(
                TimeSpanParser.Parse("3 33s", new TimeSpanParserOptions() { UncolonedDefault = Units.Minutes }),
                TimeSpan.Parse("00:03:33"));

            // Subsequent numbers will not use the default
            try {
                TimeSpanParser.Parse("13h 10", new TimeSpanParserOptions() { UncolonedDefault = Units.Minutes });
                Assert.Fail("Above will throw an exception before this line is reached.");
            } catch { }

        }

        [TestMethod]
        public void GuideSettingOptionsTests2() {

            // Note that days, hour, minutes and seconds must be in big-to-small order (just as English-speakers always write them).
            Assert.AreEqual(
                TimeSpanParser.Parse("7 days 1 hour 10 minutes 40 seconds"), 
                TimeSpan.Parse("7.1:10:40"));

            // And parsing will stop when the order is broken (here "7 days 1 hour" is ignored)
            Assert.AreEqual(
                TimeSpanParser.Parse("10 minutes 40 seconds 7 days 1 hour"), 
                TimeSpan.Parse("00:10:40"));

            // ...Unless you turn off "StrictBigToSmall"
            Assert.AreEqual(
                TimeSpanParser.Parse("10 minutes 40 seconds 7 days 1 hour", new TimeSpanParserOptions() { StrictBigToSmall = false }), 
                TimeSpan.Parse("7.1:10:40"));
        }

    }
}
