using System;
using tcalc.Expressions;

namespace tcalc.Evaluation
{
    public static class ExpressionEvaluator
    {
        public static Result Evaluate(Expression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            switch (expression)
            {
                case DurationValue duration:
                    return new DurationResult(duration.Value);
                case NumericValue numeric:
                    return new NumericResult(numeric.Value);
                case BinaryExpression binary:
                    return DispatchOperator(Evaluate(binary.Left), Evaluate(binary.Right), binary.Operator);
                default:
                    throw new ArgumentException($"Unsupported expression {expression}.");
            }
        }

        static Result DispatchOperator(Result left, Result right, Operator @operator)
        {
            if (left == null) throw new ArgumentNullException(nameof(left));
            if (right == null) throw new ArgumentNullException(nameof(right));

            switch (@operator)
            {
                case Operator.Add:
                    return DispatchAdd(left, right);
                case Operator.Subtract:
                    return DispatchSubtract(left, right);
                case Operator.Multiply:
                    return DispatchMultiply(left, right);
                case Operator.Divide:
                    return DispatchDivide(left, right);
                default:
                    throw new ArgumentException($"Unsupported operator {@operator}.");
            }
        }

        static Result DispatchAdd(Result left, Result right)
        {
            if (left is DurationResult dl && right is DurationResult dr)
                return new DurationResult(dl.Value + dr.Value);

            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value + rn.Value);

            throw new EvaluationException($"Values {left} and {right} cannot be added.");
        }

        static Result DispatchSubtract(Result left, Result right)
        {
            if (left is DurationResult dl && right is DurationResult dr)
                return new DurationResult(dl.Value - dr.Value);

            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value - rn.Value);

            throw new EvaluationException($"Value {right} cannot be subtracted from {left}.");
        }

        static Result DispatchMultiply(Result left, Result right)
        {
            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value * rn.Value);

            if (left is DurationResult dl && right is NumericResult nr)
                return new DurationResult(TimeSpan.FromSeconds(dl.Value.TotalSeconds * nr.Value));

            if (left is NumericResult nl && right is DurationResult dr)
                return new DurationResult(TimeSpan.FromSeconds(nl.Value * dr.Value.TotalSeconds));

            throw new EvaluationException($"Values {left} and {right} cannot be multiplied.");
        }

        static Result DispatchDivide(Result left, Result right)
        {
            if (left is NumericResult ln && right is NumericResult rn)
                return new NumericResult(ln.Value / rn.Value);

            if (left is DurationResult dl && right is NumericResult nr)
                return new DurationResult(TimeSpan.FromSeconds(dl.Value.TotalSeconds / nr.Value));

            if (left is DurationResult dl2 && right is DurationResult dr)
                return new NumericResult(dl2.Value.TotalSeconds / dr.Value.TotalSeconds);

            throw new EvaluationException($"Value {left} cannot be divided by {right}.");
        }
    }
}
