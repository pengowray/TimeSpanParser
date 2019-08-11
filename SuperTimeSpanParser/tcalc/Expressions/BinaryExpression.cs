using System;

namespace tcalc.Expressions
{
    public class BinaryExpression : Expression
    {
        public Expression Left { get; }
        public Expression Right { get; }
        public Operator Operator { get; }

        public BinaryExpression(Operator @operator, Expression left, Expression right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
            Operator = @operator;
        }

        public static BinaryExpression Create(Operator @operator, Expression left, Expression right)
        {
            return new BinaryExpression(@operator, left, right);
        }
    }
}
