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

        // Testing time spans found in tweets

        [TestMethod]
        [DataRow("2h:10m:58s", 0, 2, 10, 58, 0)] // "I just finished walking 11.04 km in 2h:10m:58s with #Endomondo #endorphins"
        [DataRow("3-minute 18-second", 0, 0, 3, 18, 0)] // "Spielberg's DP Russell Metty also shot the 3-minute 18-second one-shot opening crane shot in TOUCH OF EVIL (1958) directed by Orson Welles."
        [DataRow("33 days 18 hours 22 minutes and 25 seconds", 33, 18, 22, 25, 0)] 
        [DataRow("Up for 241 days, 17 hours, 32 minutes, 59 seconds", 241, 17, 32, 59, 0)]
        [DataRow("83days 21h 3m 30s", 83, 21, 3, 30, 0)]
        [DataRow("3m + 30s", 0, 0, 3, 30, 0)] // "Just completed a 5.03 km run - Intervaller ftw! 3m + 30s recover [...] #Runkeeper"
        [DataRow("274 days 1 hours 29 minutes 51 seconds 394 milliseconds left until Minhyun returns!", 274, 1, 29, 51, 394)]
        [DataRow("It took me 32 seconds + 555 milliseconds", 0, 0, 0, 32, 555)]
        public void WildTests(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }

        // Modified wild-caught time spans, to be a little tougher

        [TestMethod]
        [DataRow("10m:58s", 0, 0, 10, 58, 0)]
        [DataRow("10h:58s", 0, 10, 0, 58, 0)]
        [DataRow("3.days,18.seconds", 3, 0, 0, 18, 0)] //shouldn't work in fr-FR though
        public void WildesqueTests(string parseThis, int days, int hours, int minutes, int seconds, int milliseconds) {
            var expected = new TimeSpan(days, hours, minutes, seconds, milliseconds);
            bool success = TimeSpanParser.TryParse(parseThis, timeSpan: out TimeSpan actual);

            Assert.IsTrue(success);
            Assert.AreEqual(expected, actual);
        }


    }
}
