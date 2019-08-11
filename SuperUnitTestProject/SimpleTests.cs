using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuperTimeSpanParser.tcalc;
using System;

namespace SuperUnitTestProject {
    [TestClass]
    public class SimpleTests {
        [TestMethod]
        [DataRow("55s", "00:00:55")]
        [DataRow("1h + 2m", "1:02:00")]
        [DataRow("1h+22m+33s", "1:22:33")]
        [DataRow("(12h)", "12:00:00")]
        [DataRow("0h-7h", "-7:00:00")]
        public void TestMethod1(string parseThis, string oldSchool) {
            var expected = TimeSpan.Parse(oldSchool);
            var super = SuperParser.ParseTimeSpan(parseThis);

            Assert.IsTrue(super.HasValue, "ParseTimeSpan returned null");
            Assert.AreEqual(expected, super.Value);
        }
    }
}
