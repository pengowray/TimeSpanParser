using System;
using System.Linq;
using System.Collections.Generic;
using System.Numerics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TimeSpanParserUtil.Tests {

    [TestClass]
    public class NotWrittenHereNumberParserTests {

        //static int MAX_TEXT = 1_000_000_001; // 25 minutes? / crash
        //static int MAX_TEXT = 100_000_001; // 35 sec
        //static int MAX_TEXT = 10_000_001; // 3 sec
        //static int MAX_TEXT = 1_000_001; // 384ms
        static int MAX_TEXT = 100_001; // 59ms

        [TestMethod]
        [DataRow("0.{0}1", '0')]
        [DataRow("0.{0}", '1')]
        [DataRow("0.{0}", '2')]
        [DataRow("0.{0}", '5')]
        [DataRow("1.{0}", '5')]
        [DataRow("0.{0}", '9')]
        [DataRow("{0}1.1", '0')]
        [DataRow("2.2{0}", '0')]
        [DataRow("{0}5.5{0}", '0')]
        public void FloatTest(string template, char repeatedDigit) {
            List<float> previous = new List<float>();
            //Dictionary<string, float> parsed = new Dictionary<string, float>();
            List<Tuple<int, string, float, bool>> list = new List<Tuple<int, string, float, bool>>(); // number of repeats, string, parsed float, same as previous?

            int maxExtra = 10;
            int firstSame = -1;

            int extraLeft = maxExtra;
            int extra = 0;

            int n = 0;
            while (n < MAX_TEXT) {
                //string repeat = new String('0', n);
                //string numberString = $"0.{repeat}1";
                string repeat = new String(repeatedDigit, n);
                string numberString = string.Format(template, repeat);

                //Console.WriteLine(numberString); 

                float number = float.Parse(numberString);
                bool same = false;
                if (previous.Count > 0) {
                    if (previous[previous.Count - 1] == number) {
                        same = true;
                        if (firstSame == -1)
                            firstSame = n;
                        extraLeft--;
                        extra++;
                    }
                }

                previous.Add(number);
                //parsed[numberString] = number; // maybe make both strings
                list.Add(new Tuple<int, string, float, bool>(n, numberString, number, same));

                if (extraLeft < 0) {
                    if (n < 10) {
                        n = 10 ;
                    } else if (n < 100) {
                        n = 100;
                    } else {
                        n = n * 10;
                    }
                } else {
                    n++;
                }
            }

            foreach (var num in list.TakeLast(3 + extra)) {
                string same = num.Item4 ? " (same as above)" : "";
                string p = num.Item2.Length > 200 ? "..." : $"\"{num.Item2}\"";
                Console.WriteLine($"float.Parse({p}); // {num.Item1:N0} × '{repeatedDigit}' ... == {num.Item3}{same}");
            }
        }





        [TestMethod]
        [DataRow("0.{0}1", '0')]
        [DataRow("0.{0}", '1')]
        [DataRow("0.{0}", '2')]
        [DataRow("0.{0}", '5')]
        [DataRow("1.{0}", '5')]
        [DataRow("0.{0}", '9')]
        [DataRow("{0}1.1", '0')]
        [DataRow("2.2{0}", '0')]
        [DataRow("{0}5.5{0}", '0')]
        public void DoubleTest(string template, char repeatedDigit) {
            List<double> previous = new List<double>();
            List<Tuple<int, string, double, bool>> list = new List<Tuple<int, string, double, bool>>(); // number of repeats, string, parsed double, same as previous?

            int maxExtra = 10;
            int firstSame = -1;

            int extraLeft = maxExtra;
            int extra = 0;

            int n = 0;
            while (n < MAX_TEXT) {
                string repeat = new String(repeatedDigit, n);
                string numberString = string.Format(template, repeat);

                double number = double.Parse(numberString);
                bool same = false;
                if (previous.Count > 0) {
                    if (previous[previous.Count - 1] == number) {
                        same = true;
                        if (firstSame == -1)
                            firstSame = n;
                        extraLeft--;
                        extra++;
                    }
                }

                previous.Add(number);
                list.Add(new Tuple<int, string, double, bool>(n, numberString, number, same));

                if (extraLeft < 0) {
                    if (n < 10) {
                        n = 10;
                    } else if (n < 100) {
                        n = 100;
                    } else if (n < 1000) {
                        n = 1000;
                    } else {
                        n = n * 10;
                    }
                } else {
                    n++;
                }
            }

            foreach (var num in list.TakeLast(3 + extra)) {
                string same = num.Item4 ? " (same as above)" : "";
                string p = num.Item2.Length > 200 ? "..." : $"\"{num.Item2}\"";
                Console.WriteLine($"double.Parse({p}); // {num.Item1:N0} × '{repeatedDigit}' ... == {num.Item3}{same}");
            }
        }







        [TestMethod]
        [DataRow("0.{0}1", '0')]
        [DataRow("0.{0}", '1')]
        [DataRow("0.{0}", '2')]
        [DataRow("0.{0}", '5')]
        [DataRow("1.{0}", '5')]
        [DataRow("0.{0}", '6')]
        [DataRow("0.{0}", '8')]
        [DataRow("0.{0}", '9')]
        [DataRow("{0}1.1", '0')]
        [DataRow("2.2{0}", '0')]
        [DataRow("{0}5.5{0}", '0')]
        public void DecimalTest(string template, char repeatedDigit) {
            List<decimal> previous = new List<decimal>();
            List<Tuple<int, string, decimal, bool>> list = new List<Tuple<int, string, decimal, bool>>(); // number of repeats, string, parsed decimal, same as previous?

            int maxExtra = 10;
            int firstSame = -1;

            int extraLeft = maxExtra;
            int extra = 0;

            int n = 0;
            while (n < MAX_TEXT) {
                string repeat = new String(repeatedDigit, n);
                string numberString = string.Format(template, repeat);

                decimal number = decimal.Parse(numberString);
                bool same = false;
                if (previous.Count > 0) {
                    if (previous[previous.Count - 1] == number) {
                        same = true;
                        if (firstSame == -1)
                            firstSame = n;
                        extraLeft--;
                        extra++;
                    }
                }

                previous.Add(number);
                list.Add(new Tuple<int, string, decimal, bool>(n, numberString, number, same));

                if (extraLeft < 0) {
                    if (n < 10) {
                        n = 10;
                    } else if (n < 100) {
                        n = 100;
                    } else {
                        n = n * 10;
                    }
                } else {
                    n++;
                }
            }

            foreach (var num in list.TakeLast(3 + extra)) {
                string same = num.Item4 ? " (same as above)" : "";
                string p = num.Item2.Length > 200 ? "..." : $"\"{num.Item2}\"";
                Console.WriteLine($"decimal.Parse({p}); // {num.Item1:N0} × '{repeatedDigit}' ... == {num.Item3}{same}");
            }
        }

        [TestMethod]
        [DataRow("{0}", '0', 1)]
        [DataRow("{0}1", '0', 0)]
        [DataRow("{0}3", '0', 0)]
        public void IntTest(string template, char repeatedDigit, int startN = 0) {
            List<int> previous = new List<int>();
            List<Tuple<int, string, int, bool>> list = new List<Tuple<int, string, int, bool>>(); // number of repeats, string, parsed int, same as previous?

            int maxExtra = 10;
            int firstSame = -1;

            int extraLeft = maxExtra;
            int extra = 0;

            int n = startN;
            while (n < MAX_TEXT) {
                string repeat = new String(repeatedDigit, n);
                string numberString = string.Format(template, repeat);

                int number = int.Parse(numberString);
                bool same = false;
                if (previous.Count > 0) {
                    if (previous[previous.Count - 1] == number) {
                        same = true;
                        if (firstSame == -1)
                            firstSame = n;
                        extraLeft--;
                        extra++;
                    }
                }

                previous.Add(number);
                list.Add(new Tuple<int, string, int, bool>(n, numberString, number, same));

                if (extraLeft < 0) {
                    if (n < 10) {
                        n = 10;
                    } else if (n < 100) {
                        n = 100;
                    } else {
                        n = n * 10;
                    }
                } else {
                    n++;
                }
            }

            foreach (var num in list.TakeLast(3 + extra)) {
                string same = num.Item4 ? " (same as above)" : "";
                string p = num.Item2.Length > 200 ? "..." : $"\"{num.Item2}\"";
                Console.WriteLine($"int.Parse({p}); // {num.Item1:N0} × '{repeatedDigit}' ... == {num.Item3}{same}");
            }
        }



        [TestMethod]
        [DataRow("{0}", '0', 1)]
        [DataRow("{0}1", '0', 0)]
        [DataRow("{0}3", '0', 0)]
        public void LongTest(string template, char repeatedDigit, int startN = 0) {
            List<long> previous = new List<long>();
            List<Tuple<long, string, long, bool>> list = new List<Tuple<long, string, long, bool>>(); // number of repeats, string, parsed long, same as previous?

            int maxExtra = 10;
            int firstSame = -1;

            int extraLeft = maxExtra;
            int extra = 0;

            int n = startN;
            while (n < MAX_TEXT) {
                string repeat = new String(repeatedDigit, n);
                string numberString = string.Format(template, repeat);

                long number = long.Parse(numberString);
                bool same = false;
                if (previous.Count > 0) {
                    if (previous[previous.Count - 1] == number) {
                        same = true;
                        if (firstSame == -1)
                            firstSame = n;
                        extraLeft--;
                        extra++;
                    }
                }

                previous.Add(number);
                list.Add(new Tuple<long, string, long, bool>(n, numberString, number, same));

                if (extraLeft < 0) {
                    if (n < 10) {
                        n = 10;
                    } else if (n < 100) {
                        n = 100;
                    } else {
                        n = n * 10;
                    }
                } else {
                    n++;
                }
            }

            foreach (var num in list.TakeLast(3 + extra)) {
                string same = num.Item4 ? " (same as above)" : "";
                string p = num.Item2.Length > 200 ? "..." : $"\"{num.Item2}\"";
                Console.WriteLine($"long.Parse({p}); // {num.Item1:N0} × '{repeatedDigit}' ... == {num.Item3}{same}");
            }
        }



        [TestMethod]
        [DataRow("{0}", '0', 1)]
        [DataRow("{0}1", '0', 0)]
        [DataRow("{0}3", '0', 0)]
        public void BigIntegerTest(string template, char repeatedDigit, int startN = 0) {
            List<BigInteger> previous = new List<BigInteger>();
            List<Tuple<BigInteger, string, BigInteger, bool>> list = new List<Tuple<BigInteger, string, BigInteger, bool>>(); // number of repeats, string, parsed BigInteger, same as previous?

            int maxExtra = 10;
            int firstSame = -1;

            int extraLeft = maxExtra;
            int extra = 0;

            int n = startN;
            while (n < MAX_TEXT) {
                string repeat = new String(repeatedDigit, n);
                string numberString = string.Format(template, repeat);

                BigInteger number = BigInteger.Parse(numberString);
                bool same = false;
                if (previous.Count > 0) {
                    if (previous[previous.Count - 1] == number) {
                        same = true;
                        if (firstSame == -1)
                            firstSame = n;
                        extraLeft--;
                        extra++;
                    }
                }

                previous.Add(number);
                list.Add(new Tuple<BigInteger, string, BigInteger, bool>(n, numberString, number, same));

                if (extraLeft < 0) {
                    if (n < 10) {
                        n = 10;
                    } else if (n < 100) {
                        n = 100;
                    } else {
                        n = n * 10;
                    }
                } else {
                    n++;
                }
            }

            foreach (var num in list.TakeLast(3 + extra)) {
                string same = num.Item4 ? " (same as above)" : "";
                string p = num.Item2.Length > 200 ? "..." : $"\"{num.Item2}\"";
                Console.WriteLine($"BigInteger.Parse({p}); // {num.Item1:N0} × '{repeatedDigit}' ... == {num.Item3}{same}");
            }
        }


    }
}
