using System;
using Pidgin;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace TimeSpanPidgin {
    public class PidginTimeSpanParser {

        public static TimeSpan Trial(string input) {
            /*
            Parser<char, string> parser = String("foo");
            Parser<char, string> sequencedParser = parser1.Then(parser2);
            Parser<char, int> parser = Return(3);
            Parser<char, string> sequencedParser = parser1.Before(parser2);
            Parser<char, string> sequencedParser = Map((foo, bar) => bar + foo, parser1, parser2);
            Parser<char, char> parser = Any.Bind(c => Char(c)); // Bind uses the result of a parser to choose the next parser. 
            Parser<char, string> parser = OneOf(String("foo"), String("bar")); // warning: one fail, all fail
            Parser<char, string> parser = String("food").Or(String("foul")); // warning: one fail, all fail
            Parser<char, string> parser = Try(String("food")).Or(String("foul"));
            */

            Parser<char, string> dayParser = OneOf(String("days"), String("day"), String("d")).ThenReturn("d");
            Parser<char, string> hourParser = OneOf(String("hours"), String("hour"), String("hrs"), String("hr"), String("h")).ThenReturn("h");
            Parser<char, string> minParser = OneOf(String("minutes"), String("mins"), String("min"), String("m")).ThenReturn("m");
            Parser<char, string> secParser = OneOf(String("seconds"), String("secs"), String("sec"), String("s")).ThenReturn("s");
            Parser<char, string> unitsParser = Try(OneOf(dayParser, hourParser, minParser, secParser));
            Parser<char, int> num = Int(10);
            Parser<char, TimeSpan> unitedParser 
                = Map((numVal, unitVal) => unitVal == "m" ? TimeSpan.FromMinutes(numVal) : TimeSpan.FromHours(numVal), num, unitsParser);

            return unitedParser.ParseOrThrow(input);

            //Assert.AreEqual("foo", parser.ParseOrThrow("foo"));
            //Assert.Throws<ParseException>(() => parser.ParseOrThrow("bar")));
        }
    }
}
