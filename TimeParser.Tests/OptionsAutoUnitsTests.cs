using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class OptionsAutoUnitsTests
    {

        [TestMethod]
        [DataRow("1.1:08:18:10")]
        [DataRow("32:18:10:00:00.01")]
        [DataRow("32.18:10:00:00.01")]
        public void TooManyColonsTests(string parseThis) {
            Console.WriteLine(parseThis);
            Console.WriteLine("number of colons: " + parseThis.Count(ch => ch == ':'));
            Assert.ThrowsException<FormatException>(() => TimeSpan.Parse(parseThis));
        }

        [DataRow("32:18:00:1", "32:18:00:01")] // not too many
        [DataRow("-32:18:00:1", "-32:18:00:01")] // not too many
        [DataRow("32:18:10:00:00:00", "32:18:10:00")] // too many
        public void TooManyColonsButWellCopeTests(string parseThis, string expectThis) {
            Console.WriteLine(parseThis);
            Console.WriteLine("number of colons: " + parseThis.Count(ch => ch == ':'));

            var actual = TimeSpanParser.Parse(parseThis);
            var expected = TimeSpan.Parse(expectThis);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("32:18:10:00:00.01")]
        [DataRow("32:00.01 seconds")]
        [DataRow("10:32:01 minutes")]
        [DataRow("20:10:32:01 hours")]
        [DataRow("10:20:10:32:01 days")]
        //[DataRow("32:18:10:00:00:00")] // ok
        //[DataRow("32:18:00:1")]
        public void TooManyColonsTests2(string parseThis) {
            var options = new TimeSpanParserOptions();
            options.AllowDotSeparatedDayHours = false;
            options.AutoUnitsIfTooManyColons = false;

            Console.WriteLine(parseThis);
            Console.WriteLine("number of colons: " + parseThis.Count(ch => ch == ':'));
            Assert.ThrowsException<FormatException>(() => TimeSpanParser.Parse(parseThis, options));
        }

        [TestMethod]
        [DataRow("1.1:08:18:10")] // ok b/c dot separator not allowed
        [DataRow("1.1:08:18")]
        [DataRow("32:18:00:00.01")]
        [DataRow("32.01 seconds")]
        [DataRow("10:01 minutes")]
        [DataRow("20:10:32 hours")]
        [DataRow("10:20:10:32 days")]
        public void TooManyColonsTests3(string parseThis) {
            var options = new TimeSpanParserOptions();
            options.AllowDotSeparatedDayHours = false;
            options.AutoUnitsIfTooManyColons = false;

            Console.WriteLine(parseThis);
            Console.WriteLine("number of colons: " + parseThis.Count(ch => ch == ':'));
            var actual = TimeSpanParser.Parse(parseThis); // should all pass
        }


        [TestMethod]
        [DataRow("1.1:08:18:10 days", true)]
        [DataRow("1:1:08:18:10 days", true)]
        [DataRow("1.1:08:18:10.222 days", true)]
        [DataRow("1.1:08:18:10 hours", false)]
        [DataRow("1:1:08:18:10 hours", false)]
        [DataRow("1.1:08:18:10.222 hours", false)] // why no exception?
        [DataRow("1:08:18.222", true)] // hour default
        [DataRow("2:34 minutes", true)]
        [DataRow("2:34 seconds", false)]
        [DataRow("-2:34.555 seconds", false)]
        //TODO: test with DefaultColoned too
        public void TooManyColonsNoAutoUnitsTests(string parseThis, bool expectSuccess) {
            var options = new TimeSpanParserOptions();
            options.AllowDotSeparatedDayHours = true;
            options.AutoUnitsIfTooManyColons = false;

            if (expectSuccess) { 
                Console.WriteLine(parseThis);
                Console.WriteLine("number of colons: " + parseThis.Count(ch => ch == ':'));
                var actual = TimeSpanParser.Parse(parseThis); // should all pass
                //TODO: expected value too
            } else {

                Assert.ThrowsException<Exception>(() => TimeSpanParser.Parse(parseThis));
            }
        }

    }
}
