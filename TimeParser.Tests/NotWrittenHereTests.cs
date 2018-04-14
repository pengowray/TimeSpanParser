using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;
using System.Globalization;
using System.Numerics;

namespace TimeSpanParserUtil.Tests {

    // Just basic C# stuff. Tests that don't use our TimeSpanParser, because I need to check how stuff works.

    [TestClass]
    public class NotWrittenHereTests
    {

        [TestMethod]
        [TestCategory("External code only")]
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
        [TestCategory("External code only")]
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
        public void WhyIsNkoLikeThisBriefNontest() {

            //just briefly:
            Console.WriteLine((1.23456).ToString("F", new CultureInfo("nqo"))); // 1.235
            Console.WriteLine((1.23456).ToString("F", new CultureInfo("en-US"))); // 1.23 (2 dp in every other culture)

            Console.WriteLine((1.23456).ToString("N", new CultureInfo("nqo"))); // 1.235
            Console.WriteLine((1.23456).ToString("N", new CultureInfo("en-US"))); // 1.23

            Console.WriteLine(new CultureInfo("nqo").NumberFormat.NumberDecimalDigits);
            Console.WriteLine(new CultureInfo("en-US").NumberFormat.NumberDecimalDigits);

            Assert.AreEqual(3, new CultureInfo("nqo").NumberFormat.NumberDecimalDigits);
            Assert.AreEqual(2, new CultureInfo("en-US").NumberFormat.NumberDecimalDigits);
        }

        [TestMethod]
        public void WhyIsNkoLikeThisNontest() {

            //https://stackoverflow.com/questions/49807906/why-does-nko-use-3-decimal-places-for-fixed-point-f-numbers-while-literally
            //https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#the-fixed-point-f-format-specifier

            //more verbose:

            //pick one:
            //float number = 1.2345678;
            //decimal number = new decimal(1.2345678);
            //int number = 1;
            //BigInteger number = new BigInteger(1.2345678);
            double number = 1.2345678;

            Console.WriteLine($"Testing {number.GetType().FullName}");
            Console.WriteLine($"value: {number}");
            foreach (var culture in new string[] { "", "en-US", "fr-FR", "fa-IR", "nqo", "nqo-GN" } ) {
                var cultureinfo = new System.Globalization.CultureInfo(culture);
                string fformat = number.ToString("F", cultureinfo);
                Console.WriteLine($"{fformat} -- {cultureinfo.EnglishName} -- \"{cultureinfo.Name}\"");
            }

            /*
            Testing System.Double
            value: 1.2345678
            1.23 -- Invariant Language (Invariant Country) -- ""
            1.23 -- English (United States) -- "en-US"
            1,23 -- French (France) -- "fr-FR"
            1/23 -- Persian (Iran) -- "fa-IR"
            1.235 -- N'ko -- "nqo"
            1.235 -- N'ko (Guinea) -- "nqo-GN"
            */

            
        }

        [TestMethod]
        public void NormalizePersianNumbers() {

            // see: https://docs.microsoft.com/en-us/dotnet/standard/base-types/parsing-numeric

            string numbers = "۱٫۲۳";
            string normalized = numbers.Normalize(NormalizationForm.FormKC);
            Console.WriteLine(numbers);
            Console.WriteLine(normalized);
            Assert.AreNotEqual("1.23", normalized); // would be equal if it worked
        }

        [TestMethod]
        [DataRow("", "-123456789.123456789")] // -123456789.123456789 (en-US), -123456789,123456789 (fr-FR), -123456789/123456789 (fa-IR)
        [DataRow("N", "-123456789.123456789")] // count: 14. -123,456,789.12 (en-US), -123 456 789,12 (fr-FR), -123.456.789,12 (pt-BR), -12,34,56,789.12 (en-IN hi-IN), -123’456’789.12 (de-CH wal-ET), -123 456 789.12 (xh-ZA), 123,456,789/12- (fa-IR), - 123.456.789,12 (hr-HR), - 123,456,789.12 (km-KH), -123456,789.12 (mn-Mong), 123,456,789.123- (nqo-GN), 123.456.789,12- (prs-AF), 123,456,789.12- (sd-Arab), -123’456’789,12 (wae-CH)
        [DataRow("N8", "-123456789.123456789")] // count: 13 (nqo-GN merges with sd-Arab)
        [DataRow("N", "123456789.123456789")] // count: 10 
        [DataRow("N", "123456789")] // count: 10
        [DataRow("G", "-123456789.123456789")] // -123456789.123456789, -123456789,123456789, -123456789/123456789
        [DataRow("C", "-123456789.123456789")] // count: 253 e.g. (¤123,456,789.12) (invariant)
        [DataRow("E", "-123456789.123456789")] // -1.234568E+008 (en-US), -1,234568E+008 (fr-FR), -1/234568E+008 (fa-IR)
        [DataRow("F", "-123456789.123456789")] // -123456789.12 (en-US), -123456789,12 (fr-FR), -123456789/12 (fa-IR), -123456789.123 (nqo-GN)
        [DataRow("P", "-123456789.123456789")] //  count: 20 e.g. -12’345’678’912.35% (de-CH), -12,345,678,912.35٪ (ar-SO)
        //[DataRow("X", "123456789")] //count: 1 (with long). 75BCD15 (all culutres)
        //[DataRow("R", "-123456789.123456789")] // count 3 (with double): -123456789.12345679, -123456789,12345679, -123456789/12345679 
        public void FindAllDecimalCulturesNontest(string format, string number) {

            // https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings
            // https://en.wikipedia.org/wiki/Decimal_separator#Examples_of_use

            // because writing this was easier than finding this information online

            //var example = long.Parse(number);
            //var example = double.Parse(number);
            var example = decimal.Parse(number);

            Dictionary<string, string> cultureExample = new Dictionary<string, string>();

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
                string formatted = example.ToString(format, culture);
                if (!cultureExample.ContainsKey(formatted)) {
                    cultureExample[formatted] = culture.Name;
                } else {
                    cultureExample[formatted] += " " + culture.Name;
                }
            }
            Console.WriteLine($"Format specifier: \"{format}\" count: {cultureExample.Count():N0}");
            foreach (var r in cultureExample) {
                Console.WriteLine($"{r.Key} ({r.Value})");
            }

            /*
            summary of [DataRow("N", "-123456789.123456789")] 
            count: 14. 
            -123,456,789.12 (en-US) invariant style
            -123 456 789.12 (xh-ZA)
            -123 456 789,12 (fr-FR) SI style
            -123.456.789,12 (pt-BR)
            -12,34,56,789.12 (hi-IN)
            -123456,789.12 (mn-Mong)
            -123’456’789.12 (de-CH)
            -123’456’789,12 (wae-CH)
            - 123,456,789.12 (km-KH)
            - 123.456.789,12 (hr-HR)
            123,456,789.12- (sd-Arab)
            123,456,789.123- (nqo-GN)
            123.456.789,12- (prs-AF)
            123,456,789/12- (fa-IR)
            */
        }


        [TestMethod]
        [DataRow("c", "-1.2:3:4.5")] // common format specifier ("c"). Equal to String.Empty or null
        [DataRow("g", "-1.2:3:4")] // all the same (when no fractions of a second)
        [DataRow("g", "-1.2:3:4.5")] // three formats: -1:2:03:04.5 (en-US), -1:2:03:04,5 (fr-FR), -1:2:03:04/5 (fa-IR)
        [DataRow("G", "-1.2:3:4.5")] // three formats: -1:02:03:04.5000000 (en-US), -1:02:03:04,5000000 (fr-FR), -1:02:03:04/5000000 (fa-IR)
        [DataRow(@"hh\:mm\:ss", "-10675199.02:48:05.4775808")] // all same
        public void FindAllTimespanCulturesNontest(string format, string timespan) {

            // because writing this was easier than finding this information online

            // Format specifier: c
            // -1.02:03:04.5000000 (all cultures)

            // Format specifier: g
            // -1:2:03:04.5 (en-US)
            // -1:2:03:04,5 (fr-FR)
            // -1:2:03:04/5 (fa-IR)

            // Format specifier: G
            // -1:02:03:04.5000000 (en-US)
            // -1:02:03:04,5000000 (fr-FR)
            // -1:02:03:04/5000000 (fa-IR)

            var example = TimeSpan.Parse(timespan);

            // https://msdn.microsoft.com/en-us/library/dd784379(v=vs.110).aspx

            Dictionary<string, string> cultureExample = new Dictionary<string, string>();

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
                string formatted = example.ToString(format, culture);
                if (!cultureExample.ContainsKey(formatted)) {
                    cultureExample[formatted] = culture.Name;
                } else {
                    cultureExample[formatted] += " " + culture.Name;
                }
            }
            Console.WriteLine($"Format specifier: {format}");
            foreach (var r in cultureExample) {
                Console.WriteLine($"{r.Key} ({r.Value})");
            }
        }

        [TestMethod]
        [DataRow("en-US", "fr-FR")] // 106,751.99 (en-US) 106 751,99 (fr-FR)
        [DataRow("en-US", "es-ES")] // 106,751.99 (en-US) 106.751,99 (es-ES)
        [DataRow("fr-FR", "es-ES")] // 106 751,99 (fr-FR) 106.751,99 (es-ES)
        [DataRow("", "fr-FR")] // invariant culture works same as en-US
        public void MutuallyIncompatibleNumbersTest(string culture1, string culture2) {
            // Test how French culture parses TimeSpans really
            var us = new System.Globalization.CultureInfo(culture1);
            var fr = new System.Globalization.CultureInfo(culture2);
            //var ic = CultureInfo.InvariantCulture;

            decimal number = new decimal(106751.99);

            string us_number = number.ToString("N", us); // "106,751.99";
            string fr_number = number.ToString("N", fr); // "106 751,99";

            Console.WriteLine($"{us_number} ({culture1}) {fr_number} ({culture2})");

            var parse_us_us = decimal.Parse(us_number, us);
            var parse_fr_fr = decimal.Parse(fr_number, fr);
            Assert.AreEqual(parse_us_us, parse_fr_fr);

            // can't parse US number with FR
            Assert.ThrowsException<FormatException>(() => decimal.Parse(us_number, fr) );

            // can't parse FR number with US
            Assert.ThrowsException<FormatException>(() => decimal.Parse(fr_number, us) );

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
