using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class OptionsAllowDotSeparatedDayHoursTests
    {
        [TestMethod]
        [DataRow("1:00:00:00", "1:00:00:00", "1:00:00:00")]
        [DataRow("1.5:00", "1:5:00:00", "1:30:00")] // 1.5:00 normally read as "d.h:mm", but with !AllowDotSeparatedDayHours it's read as "hh:mm" (1.5h, 0m)
        [DataRow("1.5:10", "1:5:10:00", "1:40:00")] // 1d5h10m vs 1.5h10m
        [DataRow("-1.5:10", "-1:5:10:00", "-1:40:00")]
        [DataRow("-1.5:10.5", "-1:5:10:30", "-1:40:30")]
        [DataRow("-1:10.5", "-01:10:30", "-01:10:30")] // [TODO?: should treat as mm:ss because dp in last column implies seconds? (i.e. -00:01:40.5)]
        [DataRow("1.5:00:00", "1:5:00:00", "1:30:00")] // 1.5 hours
        [DataRow("1.5:00:00:00", "1:12:00:00", "1:12:00:00")] // 1.5 days (even with allowing, because too many colons)
        [DataRow("-1.5:00:00:00", "-1:12:00:00", "-1:12:00:00")] // 1.5 days (even with allowing, because too many colons)
        public void WithAndWithoutAllowingDotSeperatorTests(string parseThis, string regularExpected, string withoutAllowing) {
            var options = new TimeSpanParserOptions();
            options.AllowDotSeparatedDayHours = false;

            Console.WriteLine(parseThis);
            Console.WriteLine("number of colons: " + parseThis.Count(ch => ch == ':'));

            var actualRegular = TimeSpanParser.Parse(parseThis);
            var expectRegular = TimeSpan.Parse(regularExpected);

            var actualWithoutAllowing = TimeSpanParser.Parse(parseThis, options);
            var expectWithoutAllowing = TimeSpan.Parse(withoutAllowing);

            Assert.AreEqual(expectRegular, actualRegular);
            Assert.AreEqual(expectWithoutAllowing, actualWithoutAllowing);
        }

        [TestMethod]
        [DataRow("1.5:00", "1:5:00:00", "1:30:00")] 
        [DataRow("1.5:10", "1:5:10:00", "1:40:00")] 
        [DataRow("-1.5:10", "-1:5:10:00", "-1:40:00")]
        [DataRow("-1.5:10.5", "-1:5:10:30", "-1:40:30")]
        [DataRow("-1:10.5", "-01:10:30", "-01:10:30")]
        [DataRow("1.5:00:00", "1:5:00:00", "1:30:00")] 
        [DataRow("1.5:00:00:00", "1:12:00:00", "1:12:00:00")]
        [DataRow("-1.5:00:00:00", "-1:12:00:00", "-1:12:00:00")]
        public void RequireDaysAllowingDotSeperatorTests(string parseThis, string regularExpected, string withoutAllowing) {
            var optionsWithMinutes = new TimeSpanParserOptions();
            optionsWithMinutes.AllowDotSeparatedDayHours = false;
            optionsWithMinutes.AutoUnitsIfTooManyColons = false; // so require days
            optionsWithMinutes.ColonedDefault = Units.Minutes;

            var optionsWithHours = new TimeSpanParserOptions();
            optionsWithHours.AllowDotSeparatedDayHours = false;
            optionsWithHours.AutoUnitsIfTooManyColons = false;
            optionsWithHours.ColonedDefault = Units.Hours; // (default)

            var optionsWithDays = new TimeSpanParserOptions();
            optionsWithDays.AllowDotSeparatedDayHours = false;
            optionsWithDays.AutoUnitsIfTooManyColons = false;
            optionsWithDays.ColonedDefault = Units.Days;

            var optionsWithNone = new TimeSpanParserOptions();
            optionsWithNone.AllowDotSeparatedDayHours = false;
            optionsWithNone.AutoUnitsIfTooManyColons = false;
            optionsWithNone.ColonedDefault = Units.None;

            string withDaysText = $"{parseThis} days";

            var expectRegular = TimeSpan.Parse(regularExpected);
            var expectWithoutAllowing = TimeSpan.Parse(withoutAllowing);

            var actualWithMinutes = TimeSpanParser.Parse(parseThis, optionsWithMinutes);
            var actualWithMinutes2 = TimeSpanParser.Parse(withDaysText, optionsWithMinutes);
            var actualWithHours = TimeSpanParser.Parse(parseThis, optionsWithHours);
            var actualWithHours2 = TimeSpanParser.Parse(withDaysText, optionsWithHours);
            var actualWithDays = TimeSpanParser.Parse(parseThis, optionsWithDays);
            var actualWithDays2 = TimeSpanParser.Parse(withDaysText, optionsWithDays);
            var actualWithNone = TimeSpanParser.Parse(parseThis, optionsWithNone);
            var actualWithNone2 = TimeSpanParser.Parse(withDaysText, optionsWithNone);

            //TODO


            Console.WriteLine("number of colons: " + parseThis.Count(ch => ch == ':'));


            //Assert.AreEqual(expectRegular, actualRegular);
            //Assert.AreEqual(expectWithoutAllowing, actualWithoutAllowing);
        }


    }
}
