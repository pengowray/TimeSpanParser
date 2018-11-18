using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimeSpanParserUtil.Tests {
    /// <summary>
    /// Test the odd behavior of System.TimeSpan.Parse().
    /// 
    /// Called "Not Written Here" because this is only testing dotnet's System library, and does not run any other code from this project.
    /// Called "Underflow weirdness" with full awareness that they're technically still considered Overflows by technical smart people.
    /// 
    /// If this test fails, Microsoft has fixed bugs in TimeSpan.Parse().
    /// </summary>
    [TestClass]
    public class NotWrittenHereUnderflowWeirdnessTests {

        [TestMethod]
        public void IfThisTestFailsThenDotNetBugsHaveBeenFixed() { // aka TimeSpanWeirdnessDemo

            Console.WriteLine("If this test fails, Microsoft has fixed bugs in TimeSpan.Parse().");

            // This is 1 tick (100 nanoseconds). It has 7 fractional digits.
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0000001").Ticks, 1);  // Passes correctly. 

            // This ought to be 0.1 ticks, but it's also 1 tick? Looks like it's just rounding up...
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000001").Ticks, 1); // Passes but shouldn't.

            // ...But then why does 0.2 ticks round up to 2 ticks?
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000002").Ticks, 2); // Passes but shouldn't.

            // When we reach 9 fractional digits an OverflowException is thrown (correctly).
            Assert.ThrowsException<OverflowException>(() =>
                            TimeSpan.Parse("0:00:00.000000001"));

            // Let's try some more combinations
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0000005" ).Ticks, 5);   // Passes correctly
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000005").Ticks, 5);   // Passes but shouldn't. Expected: 0, 1 or OverflowException
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000050").Ticks, 50);  // Passes but shouldn't. Expected: 5 or OverflowException
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000055").Ticks, 55);  // Passes but shouldn't. Expected: 5, 6 or OverflowException
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0000055").Ticks,  55);  // Passes correctly
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000550").Ticks, 550); // Passes but shouldn't

            // larger numbers
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0123450"),
                            TimeSpan.Parse("0:00:00.00123450"));

            Assert.AreEqual(TimeSpan.Parse("0:00:00.0123456"),
                            TimeSpan.Parse("0:00:00.00123456"));

            // Just reiterating, 5,500 ns == 55,000 ns
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000055"), 
                            TimeSpan.Parse("0:00:00.0000055"));

            // Still passes and still shouldn't
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000098").Ticks, 98);

            // Uniquely, but correctly (perhaps), causes an overflow
            Assert.ThrowsException<OverflowException>(() => 
                            TimeSpan.Parse("0:00:00.00000099"));

            // Passes correctly (7 fractional zeroes)
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0000000"), TimeSpan.Zero);

            // Needlessly overflows (8 fractional zeroes)
            Assert.ThrowsException<OverflowException>(() => 
                            TimeSpan.Parse("0:00:00.00000000")); 

        }
    }
}
