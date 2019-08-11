using System.Globalization;

namespace tcalc.Evaluation
{
    public class NumericResult : Result
    {
        public double Value { get; }

        public NumericResult(double value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}