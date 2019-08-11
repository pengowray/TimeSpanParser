using System;
using Superpower;
using Superpower.Parsers;

namespace tcalc.Parsing
{
    public static class ExpressionTextParsers
    {
        public static TextParser<TimeSpan> Magnitude { get; } =
            Character.EqualTo('d').Value(TimeSpan.FromDays(1))
                .Or(Character.EqualTo('h').Value(TimeSpan.FromHours(1)))
                .Or(Span.EqualTo("ms").Try().Value(TimeSpan.FromMilliseconds(1)))
                .Or(Character.EqualTo('m').Value(TimeSpan.FromMinutes(1)))
                .Or(Character.EqualTo('s').Value(TimeSpan.FromSeconds(1)));

        public static TextParser<TimeSpan> Duration { get; } =
            Numerics.DecimalDouble
                .Then(d => Magnitude.Select(m => TimeSpan.FromSeconds(m.TotalSeconds * d)));
    }
}
