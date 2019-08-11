using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using tcalc.Expressions;

using ExpressionTokenParser = Superpower.TokenListParser<tcalc.Parsing.ExpressionToken, tcalc.Expressions.Expression>;

namespace tcalc.Parsing
{
    public static class ExpressionParser
    {
        public static ExpressionTokenParser Number { get; } =
            Token.EqualTo(ExpressionToken.Number)
                .Apply(Numerics.DecimalDouble)
                .Select(d => (Expression)new NumericValue(d));

        public static ExpressionTokenParser Duration { get; } =
            Token.EqualTo(ExpressionToken.Duration)
                .Apply(ExpressionTextParsers.Duration)
                .Select(ts => (Expression)new DurationValue(ts));

        public static TokenListParser<ExpressionToken, Operator> Op(ExpressionToken token, Operator op) => 
            Token.EqualTo(token)
                .Value(op);

        public static TokenListParser<ExpressionToken, Operator> Add { get; } = Op(ExpressionToken.Plus, Operator.Add);
        public static TokenListParser<ExpressionToken, Operator> Subtract { get; } = Op(ExpressionToken.Minus, Operator.Subtract);
        public static TokenListParser<ExpressionToken, Operator> Multiply { get; } = Op(ExpressionToken.Asterisk, Operator.Multiply);
        public static TokenListParser<ExpressionToken, Operator> Divide { get; } = Op(ExpressionToken.Slash, Operator.Divide);

        public static ExpressionTokenParser Literal { get; } = Duration.Or(Number);

        static ExpressionTokenParser Factor { get; } =
            (from lparen in Token.EqualTo(ExpressionToken.LParen)
             from expr in Parse.Ref(() => Expression)
             from rparen in Token.EqualTo(ExpressionToken.RParen)
             select expr)
            .Or(Literal);

        static ExpressionTokenParser Term { get; } = Parse.Chain(Multiply.Or(Divide), Factor, BinaryExpression.Create);
        static ExpressionTokenParser Expression { get; } = Parse.Chain(Add.Or(Subtract), Term, BinaryExpression.Create);

        static ExpressionTokenParser Source { get; } = Expression.AtEnd();

        public static bool TryParse(TokenList<ExpressionToken> tokens, out Expression expr, out string error, out Position errorPosition)
        {
            var result = Source(tokens);
            if (!result.HasValue)
            {
                expr = null;
                error = result.ToString();
                errorPosition = result.ErrorPosition;
                return false;
            }

            expr = result.Value;
            error = null;
            errorPosition = Position.Empty;
            return true;
        }
    }
}
