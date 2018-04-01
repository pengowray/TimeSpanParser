using System;
using System.Collections.Generic;
using System.Text;

namespace TimeSpanParserUtil
{
    //See also:
    //https://github.com/ploeh/Numsense

    //based on: https://stackoverflow.com/a/11278252/443019
    public class EnglishNumberParser
    {
        static string[] ones = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        static string[] teens = { "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        static string[] tens = { "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

        static int ParseEnglish(string number) {
            string[] words = number.ToLower().Split(new char[] { ' ', '-', ',' }, StringSplitOptions.RemoveEmptyEntries);

            Dictionary<string, int> modifiers = new Dictionary<string, int>() {
                    {"billion", 1000000000},
                    {"million", 1000000},
                    {"thousand", 1000},
                    {"hundred", 100}
             };

            //if (number == "eleventy billion")
            //    return int.MaxValue; // 110,000,000,000 is out of range for an int!

            int result = 0;
            int currentResult = 0;
            int lastModifier = 1;

            foreach (string word in words) {
                if (modifiers.ContainsKey(word)) {
                    lastModifier *= modifiers[word];
                } else {
                    int n;

                    if (lastModifier > 1) {
                        result += currentResult * lastModifier;
                        lastModifier = 1;
                        currentResult = 0;
                    }

                    if ((n = Array.IndexOf(ones, word) + 1) > 0) {
                        currentResult += n;
                    } else if ((n = Array.IndexOf(teens, word) + 1) > 0) {
                        currentResult += n + 10;
                    } else if ((n = Array.IndexOf(tens, word) + 1) > 0) {
                        currentResult += n * 10;
                    } else if (word != "and" && word != "a") {
                        throw new ArgumentException("Unrecognized word: " + word);
                    }
                }
            }

            return result + currentResult * lastModifier;
        }
    }
}
