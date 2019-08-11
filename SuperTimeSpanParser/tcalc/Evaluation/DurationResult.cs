using System;

namespace tcalc.Evaluation
{
    public class DurationResult : Result
    {
        public TimeSpan Value { get; }

        public DurationResult(TimeSpan value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString("c");
        }
    }
}