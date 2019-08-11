using Superpower;
using Superpower.Parsers;
using System;

namespace SuperTimeSpanParser
{

    // rewrite of TimeSpanParser using Superpower
    // also considered: 

    public static class SuperTimeSpanParser
    {

        enum TimeTokens
        {
            None,
            Integer,
            Float,
            MinusSign,
        }

        static TextParser<int> IntDigits(int count) =>
            Character.Digit
                .Repeat(count)
                .Select(chars => int.Parse(new string(chars)));

        static TextParser<int> TwoDigits { get; } = IntDigits(2);
        static TextParser<int> FourDigits { get; } = IntDigits(4);

        static TextParser<char> Dash { get; } = Character.EqualTo('-');
        static TextParser<char> Colon { get; } = Character.EqualTo(':');

        static TextParser<char> Minus { get; } = Character.EqualTo('-');

        static TextParser<TimeSpan> Time { get; } =
            from hour in TwoDigits
            from _ in Colon
            from minute in TwoDigits
            from second in Colon
                .IgnoreThen(TwoDigits)
                .OptionalOrDefault()
            select new TimeSpan(hour, minute, second);


        static TextParser<string> identifier =
            from first in Character.Letter
            from rest in Character.LetterOrDigit.Or(Character.EqualTo('_')).Many()
            select first + new string(rest);

    }
}
