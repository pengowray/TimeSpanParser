using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimeSpanParserUtil.Tests {
    /// <summary>
    /// Test the odd behavior of System.TimeSpan.Parse().
    /// 
    /// Called "Not Written Here" because this is only testing dotnet's System library, and does not run any other code from this project.
    /// Called "Underflow weirdness" with full awareness that they're technically still considered Overflows by technical smart people.
    /// 
    /// If this test fails, bugs in TimeSpan.Parse() have been patched.
    /// 
    /// I've submitted a patch to fix these issues: https://github.com/dotnet/corefx/pull/33581
    /// 
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
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0000005").Ticks, 5);   // Passes correctly
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000005").Ticks, 5);   // Passes but shouldn't. Expected: 0, 1 or OverflowException
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000050").Ticks, 50);  // Passes but shouldn't. Expected: 5 or OverflowException
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000055").Ticks, 55);  // Passes but shouldn't. Expected: 5, 6 or OverflowException
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0000055").Ticks, 55);  // Passes correctly
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00000550").Ticks, 550); // Passes but shouldn't

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

            // let's try larger numbers
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0123450"),
                            TimeSpan.Parse("0:00:00.00123450"));
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0123450").Ticks, 123450);
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00123450").Ticks, 123450);

            Assert.AreEqual(TimeSpan.Parse("0:00:00.0123456"),
                            TimeSpan.Parse("0:00:00.00123456"));
            Assert.AreEqual(TimeSpan.Parse("0:00:00.0123456").Ticks, 123456);
            Assert.AreEqual(TimeSpan.Parse("0:00:00.00123456").Ticks, 123456);

        }

        /// <summary>
        /// Double check the maths in internal static long Pow10(int pow) 
        /// in https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Globalization/TimeSpanParse.cs
        /// (It's fine.)
        /// </summary>
        /// <param name="pow"></param>
        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(6)]
        [DataRow(7)]
        [DataRow(8)]
        [DataRow(9)]
        public void Pow10Test(int pow) {

            long maths = (long)Math.Pow(10, pow);
            Assert.AreEqual(maths, Pow10(pow));
            Assert.AreEqual(Pow10(pow) * 10, Pow10(pow + 1));
        }

        internal static long Pow10(int pow) {
            switch (pow) {
                case 0: return 1;
                case 1: return 10;
                case 2: return 100;
                case 3: return 1000;
                case 4: return 10000;
                case 5: return 100000;
                case 6: return 1000000;
                case 7: return 10000000;
                default: return (long)Math.Pow(10, pow);
            }
        }


        [TestMethod]
        [DataRow("1", false)]
        [DataRow("0", false)]
        [DataRow("01", false)]
        [DataRow("09", false)]
        [DataRow("000001", false)]
        [DataRow("000000", false)]
        [DataRow("999999", false)]
        [DataRow("9999999", false)]
        [DataRow("0000000", false)]
        [DataRow("0000001", false)]
        [DataRow("1000000", false)]
        [DataRow("1000001", false)]
        [DataRow("0999999", false)]
        [DataRow("0099999", false)]
        [DataRow("9999990", false)]
        [DataRow("9999900", false)]
        [DataRow("00000000", true)] // all zeroes, theoretically could be false
        [DataRow("00000001", true)]
        [DataRow("10000000", true)]
        [DataRow("10000001", true)]
        [DataRow("00000098", true)]
        [DataRow("00000055", true)]
        [DataRow("00000099", true)]
        [DataRow("00000550", true)]
        [DataRow("00099999", true)]
        [DataRow("00999999", true)]
        [DataRow("09999999", true)]
        [DataRow("00099000", true)]
        [DataRow("99999999", true)]
        [DataRow("10000000", true)]
        [DataRow("01000000", true)]
        [DataRow("00100000", true)]
        [DataRow("00010000", true)]
        [DataRow("00001000", true)]
        [DataRow("00000100", true)]
        [DataRow("00000010", true)]
        [DataRow("00000001", true)]
        [DataRow("000000000", true)]
        [DataRow("000000001", true)]
        [DataRow("100000000", true)]
        [DataRow("999999999", true)]
        [DataRow("000000000", true)]
        [DataRow("099999999", true)]
        [DataRow("000000000000", true)] // theoretically could be false
        public void InvalidFractionTest(string numberText, bool expectedInvalid) {
            int number = int.Parse(numberText);
            int leadingZeroes = numberText.Length - numberText.TrimStart('0').Length;

            bool failedByOriginalToo = expectedInvalid != IsInvalidFractionOriginal(number, leadingZeroes);

            var result = IsInvalidFractionPengo(number, leadingZeroes);
            Assert.AreEqual(expectedInvalid, result, failedByOriginalToo ? "Also failed by original." : "But not failed by original");
        }

        /// <summary>
        /// IsInvalidFractio() from TimeSpanParse.cs (dotnet/corefx) has errors.
        /// https://github.com/dotnet/corefx/blob/master/src/Common/src/CoreLib/System/Globalization/TimeSpanParse.cs
        /// </summary>
        /// <param name="numberText">A number found after the decimal place. The fractional unit of seconds.</param>
        /// <param name="isReallyInvalid">For your reference only (ignored by test)</param>
        /// <param name="originalThinksIsInvalid">What does the original IsInvalidFractio() code return for this? We'll test if it really does.</param>
        [TestMethod]
        // 1 digit is not invalid
        [DataRow("1", false, false)]
        [DataRow("0", false, false)]
        // 7 digits is not invalid
        [DataRow("0099999", false, false)]
        [DataRow("9999990", false, false)]
        [DataRow("9999900", false, false)]
        // 8 digits all should be invalid
        [DataRow("00000001", true, false)] // invalid but original thinks is not
        [DataRow("10000000", true, true)]
        [DataRow("10000001", true, true)]
        [DataRow("00000098", true, false)] // invalid but original thinks is not
        [DataRow("00000055", true, false)] // invalid but original thinks is not
        [DataRow("00000098", true, false)] // invalid but original thinks is not
        [DataRow("00000099", true, true)]  // original catches this one correctly
        [DataRow("00000000", true, true)]  // all zeroes, so theoretically could be valid (i.e. false), but it's invalid and that's fair enough
        // 9+ digits is right out
        [DataRow("000000000", true, true)]
        [DataRow("000000001", true, true)]
        [DataRow("100000000", true, true)]
        [DataRow("999999999", true, true)]
        [DataRow("000000000000", true, true)] // theoretically could be valid (i.e. false)
        public void IsInvalidFractionOriginalHasErrorsTest(string numberText, bool isReallyInvalid, bool originalThinksIsInvalid) {
            int number = int.Parse(numberText);
            int leadingZeroes = numberText.Length - numberText.TrimStart('0').Length;

            var result = IsInvalidFractionOriginal(number, leadingZeroes);

            Assert.AreEqual(result, originalThinksIsInvalid);
        }

        // used by private static bool TryTimeToTicks()
        public static bool IsInvalidFractionOriginal(int _num, int _zeroes) {
            //Debug.Assert(_num > -1);
            const int MaxFraction = 9999999;
            const int MaxFractionDigits = 7;

            if (_num > MaxFraction || _zeroes > MaxFractionDigits)
                return true;

            if (_num == 0 || _zeroes == 0) // --- zeroes is here to avoid divide by zero but fails
                return false;

            // num > 0 && zeroes > 0 && num <= maxValue && zeroes <= maxPrecision
            return _num >= MaxFraction / Pow10(_zeroes - 1);
        }

        // modified to not misbehave
        public static bool IsInvalidFractionPengo(int _num, int _zeroes) {
            //Debug.Assert(_num > -1);
            const int MaxFraction = 9999999;
            const int MaxFractionDigits = 7;

            if (_num > MaxFraction || _zeroes > MaxFractionDigits)
                return true;

            if (_num == 0 || _zeroes == 0)
                return false;

            Console.WriteLine($"num: {_num}, digits: {_num.ToString().Length}, zeroes:{_zeroes}");

            Console.WriteLine("Working of old algorithm:");
            Console.WriteLine($"_num = {_num} >= {MaxFraction} / Pow10({_zeroes} - 1)");
            Console.WriteLine($"_num = {_num} >= {MaxFraction} / Pow10({_zeroes - 1})");
            Console.WriteLine($"_num = {_num} >= {MaxFraction} / {Pow10(_zeroes - 1)}");
            Console.WriteLine($"_num = {_num} >= {MaxFraction / Pow10(_zeroes - 1)}");
            Console.WriteLine();
            Console.WriteLine("Working of new algorithm:");
            Console.WriteLine($"_num = {_num} >= {MaxFraction} / Pow10({_zeroes})");
            Console.WriteLine($"_num = {_num} >= {MaxFraction} / {Pow10(_zeroes)}");
            Console.WriteLine($"_num = {_num} >= {MaxFraction / Pow10(_zeroes)}");

            return _num > MaxFraction / Pow10(_zeroes);
        }


        [TestMethod]
        public void Bug32907Test() {
            //TryParse should never throw an exception, but it does
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => DateTime.TryParse("9999-12-31T23:59:59.99999999Z", out var dateTime)); ;
        }

        [TestMethod]
        public void Bug32907TruncationTest() {
            //TODO: when above is fixed, check if the final 9 is truncated

            //var dateTime8 = DateTime.Parse("9999-12-31T23:59:59.99999999Z");
            //var dateTime7 = DateTime.Parse("9999-12-31T23:59:59.9999999Z");

            //Assert.AreEqual(dateTime8, dateTime7, "Final 9 is not truncated");
        }

    }
}