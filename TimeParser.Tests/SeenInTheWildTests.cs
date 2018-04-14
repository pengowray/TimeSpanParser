using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using TimeSpanParserUtil;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class SeenInTheWildTests {

        /// <summary>
        /// Testing time spans found in tweets or elsewhere in the wild
        /// </summary>
        [TestMethod]
        [DataRow("2h:10m:58s", 0, 2, 10, 58, 0)] // "I just finished walking 11.04 km in 2h:10m:58s with #Endomondo #endorphins"
        [DataRow("3-minute 18-second", 0, 0, 3, 18, 0)] // "Spielberg's DP Russell Metty also shot the 3-minute 18-second one-shot opening crane shot in TOUCH OF EVIL (1958) directed by Orson Welles."
        [DataRow("33 days 18 hours 22 minutes and 25 seconds", 33, 18, 22, 25, 0)] 
        [DataRow("Up for 241 days, 17 hours, 32 minutes, 59 seconds", 241, 17, 32, 59, 0)]
        [DataRow("83days 21h 3m 30s", 83, 21, 3, 30, 0)]
        [DataRow("3m + 30s", 0, 0, 3, 30, 0)] // "Just completed a 5.03 km run - Intervaller ftw! 3m + 30s recover [...] #Runkeeper"
        [DataRow("274 days 1 hours 29 minutes 51 seconds 394 milliseconds left until Minhyun returns!", 274, 1, 29, 51, 394)]
        [DataRow("It took me 32 seconds + 555 milliseconds", 0, 0, 0, 32, 555)]
        [DataRow("1200:00:00", 50, 0, 0, 0, 0)]
        [DataRow("1174:10:07", 48, 22, 10, 7, 0)]
        [DataRow("99d 99h 99m", 99, 99, 99, 0, 0)] // "//Time Remaining: 99d 99h 99m -TIMER ERROR-"
        [DataRow("actually you're wrong its only 0 years, 0 months, 0 days, 1 hours, 17 minutes, and 51 seconds", 0, 1, 17, 51, 0)]
        [DataRow("Only: 0 years 0 months 17 days 1 hours 51 minutes 47 seconds", 17, 1, 51, 47, 0)]
        [DataRow("0 years, 0 months, #10 days, 19 hours", 10, 19, 0, 0, 0)]
        [DataRow("It's been exactly 0 years 0 months 0 days 0 hours 5 minutes 16 seconds 12 milliseconds and 1 nanosecond since we ate", 0, 0, 5, 16, 12)] //note: 1 nanosecond is 100x smaller than TimeSpan's tick resolution so is effectively ignored
        [DataRow("19,999d", 19999, 0, 0, 0, 0)] // "it had been 19,999d since the Satanic Enthronement ceremony"
        [DataRow("9,130.9 days, 219,141 hours, 13,148,477 minutes, 788,908,652 seconds", 36_523, 13, 10, 32, 0)] // ~100 years. (full version in FutureWildTests)
        public void WildTests(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            TimeSpan actual = TimeSpanParser.Parse(parseThis);

            //bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);
            //Assert.IsTrue(success);

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Modified wild-caught time spans, to be a little tougher
        /// </summary>
        [TestMethod]
        [DataRow("10m:58s", 0, 0, 10, 58, 0)]
        [DataRow("10h:58s", 0, 10, 0, 58, 0)]
        [DataRow("3.days,18.seconds", 3, 0, 0, 18, 0)] //shouldn't work in fr-FR though (",18" = .18)
        public void WildesqueTests(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// Support this stuff in the future.
        /// </summary>
        [TestMethod]
        [DataRow("11 AND A HALF MORE HOURS", 0, 11, 30, 0, 0)] // "11 AND A HALF MORE HOURS TIL UR 21ST BDAY."
        //[DataRow("On this day, 25.0 years, 300.0 months, 1,304.4 weeks, 9,130.9 days, 219,141 hours, 13,148,477 minutes, 788,908,652 seconds, myself and 63 other individuals began training at the #1 Fire Academy in this Universe", 0, 0, 0, 0, 0)]
        //[DataRow("half-life of beryllium-13" )] // (larcin) 2.7×10−21 s
        [DataRow("Just 500 trillion nanoseconds!", 0, 0, 0, 500000, 0)] // 5.78703703703703809 days
        public void FutureWildTests(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            TimeSpan actual = TimeSpanParser.Parse(parseThis);

            Assert.AreEqual(expected, actual);
        }


    }
}
