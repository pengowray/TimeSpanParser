using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;
using System.Globalization;

namespace TimeSpanParserUtil.Tests {

    // Just basic C# stuff. Tests that don't use our TimeSpanParser, because I need to check how stuff works.

    [TestClass]
    public class NotWrittenHereTests
    {

        [TestMethod]
        public void NotWrittenHereMaxTest() {
            // Check that TimeSpan.MaxValue can really roundtrip with .NET's TimeSpan.Parse() 
            // and really is what they say it is in the docs because I was having trouble, OK?

            var expected1 = TimeSpan.MaxValue;
            string MaximumTimeSpan1 = "10675199.02:48:05.4775807";
            var expected2 = TimeSpan.Parse(MaximumTimeSpan1);
            string MaximumTimeSpan2 = expected2.ToString();
            var expected3 = TimeSpan.Parse(MaximumTimeSpan2);

            Assert.AreEqual(expected1, expected2);
            Assert.AreEqual(expected2, expected3);
            Assert.AreEqual(expected1, expected3);

            Assert.AreEqual(MaximumTimeSpan1, MaximumTimeSpan2);
        }

        [TestMethod]
        public void NotWrittenHereMinTest() {
            var expected1 = TimeSpan.MinValue;
            string MinimumTimeSpan1 = "-10675199.02:48:05.4775808"; ;
            var expected2 = TimeSpan.Parse(MinimumTimeSpan1);
            string MinimumTimeSpan2 = expected2.ToString();
            var expected3 = TimeSpan.Parse(MinimumTimeSpan2);

            Assert.AreEqual(expected1, expected2);
            Assert.AreEqual(expected2, expected3);
            Assert.AreEqual(expected1, expected3);

            Assert.AreEqual(MinimumTimeSpan1, MinimumTimeSpan2);
        }

        [TestMethod]
        public void BasicFrenchNumbersTest() {
            // Test how French culture parses TimeSpans really
            var fr = new System.Globalization.CultureInfo("fr-FR");
            var us = new System.Globalization.CultureInfo("en-US");
            var ic = CultureInfo.InvariantCulture;

            string us_number = "106751.99";
            string fr_number = "106751,99";
            decimal number = new decimal(106751.99);

            var parse_us_us = decimal.Parse(us_number, us);
            var parse_fr_fr = decimal.Parse(fr_number, fr);
            Assert.AreEqual(parse_us_us, parse_fr_fr);

            // can't parse US number with FR
            try {
                var parse_us_fr = decimal.Parse(us_number, fr);
                Assert.Fail("above should have failed");
            } catch { }

            // can't parse FR number with US
            try {
                var parse_fr_us = decimal.Parse(fr_number, us);
                Assert.Fail("above should have failed");
            } catch { }

            // what about styles?
            
        }


        [TestMethod]
        public void BasicFrenchTimeSpanTest() {

            // See System.Globalization.TimeSpanParse
            // https://referencesource.microsoft.com/#mscorlib/system/globalization/timespanparse.cs,8

            // System.Globalization.TimeSpanFormat 
            // https://referencesource.microsoft.com/#mscorlib/system/globalization/timespanformat.cs.html

            // TimeSpan #region ParseAndFormat (just calls above stuff)
            // https://referencesource.microsoft.com/#mscorlib/system/timespan.cs


            // Test how French culture parses TimeSpans really
            var fr = new System.Globalization.CultureInfo("fr-FR");
            var us = new System.Globalization.CultureInfo("en-US");
            var ic = CultureInfo.InvariantCulture;

            string timespan_us = "10675199.02:48:05.4775807";
            string timespan_fr = "10675199:2:48:05,4775807"; // {0:g} fr-FR
            string semifrench = "10675199.02:48:05,4775807";

            var parse_us_us = TimeSpan.Parse(timespan_us, us);
            var parse_fr_fr = TimeSpan.Parse(timespan_fr, fr);

            // Can parse US-formatted with fr-FR
            var parse_us_fr = TimeSpan.Parse(timespan_us, fr);

            // Cannot parse FR-formatted with US
            Assert.IsFalse(TimeSpan.TryParse(timespan_fr, us, out TimeSpan parse_fr_us));

            // Cannot parse semi-French string containing a "." (US-style) for the day.month separator but a "," (FR-style) for decimal separator
            // fr sensibly uses a ':' not a '.' for day:month separator
            Assert.IsFalse(TimeSpan.TryParse(semifrench, fr, out TimeSpan bad_fr_fr));
            // Can't be parsed by the US or IC parser either
            Assert.IsFalse(TimeSpan.TryParse(semifrench, us, out TimeSpan bad_fr_us));
            Assert.IsFalse(TimeSpan.TryParse(semifrench, ic, out TimeSpan bad_fr_ic));

            // Can convert to French formatting
            // And it matches the pre-defined French string
            var timespan_to_fr = parse_us_us.ToString("g", fr);
            Assert.AreEqual(timespan_fr, timespan_to_fr);

            // Can read it back
            var parse_fr2_fr = TimeSpan.Parse(timespan_to_fr, fr);
            Assert.AreEqual(parse_us_us, parse_fr2_fr);
        }
    }
}
