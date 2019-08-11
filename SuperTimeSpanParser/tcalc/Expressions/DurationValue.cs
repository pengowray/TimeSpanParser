using System;

namespace tcalc.Expressions
{
    public class DurationValue : Expression
    {
        public TimeSpan Value { get; }

        public DurationValue(TimeSpan value)
        {
            Value = value;
        }
    }
}

