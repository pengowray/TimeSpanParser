using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace tcalc.Parsing
{
    public static class ExpressionTokenizer
    {
        public static TextParser<TextSpan> Duration { get; } =
            Numerics.Decimal
                .Then(_ => Span.WithAll(char.IsLetter));

 
        /*
        static TextParser<Unit> DurationTest{ get; } =
            from number in Numerics.Decimal
            from optionalSpace in Span.WhiteSpace
                .IgnoreMany()
                .Optional()
            from units in Character(letters)
                //.Value(Unit.Value)
                //.IgnoreMany()
            from close in Character.EqualTo('"')
            select Unit.Value;
        */


        static Tokenizer<ExpressionToken> Tokenizer { get; } = new TokenizerBuilder<ExpressionToken>()
            .Match(Character.EqualTo('+'), ExpressionToken.Plus)
            .Match(Character.EqualTo('-'), ExpressionToken.Minus)
            .Match(Character.EqualTo('*'), ExpressionToken.Asterisk)
            .Match(Character.EqualTo('/'), ExpressionToken.Slash)
            .Match(Duration, ExpressionToken.Duration, requireDelimiters: true)
            .Match(Numerics.Decimal, ExpressionToken.Number, requireDelimiters: true)
            .Match(Character.EqualTo('('), ExpressionToken.LParen)
            .Match(Character.EqualTo(')'), ExpressionToken.RParen)
            .Ignore(Span.WhiteSpace)
            .Build();

        public static Result<TokenList<ExpressionToken>> TryTokenize(string source)
        {
            return Tokenizer.TryTokenize(source);
        }
    }
}
