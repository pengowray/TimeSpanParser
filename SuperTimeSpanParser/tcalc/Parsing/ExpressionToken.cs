using Superpower.Display;

namespace tcalc.Parsing
{
    public enum ExpressionToken
    {
        Number,

        Duration,

        [Token(Example = "+")]
        Plus,

        [Token(Example = "-")]
        Minus,

        [Token(Example = "*")]
        Asterisk,

        [Token(Example = "/")]
        Slash,

        [Token(Example = "(")]
        LParen,

        [Token(Example = ")")]
        RParen
    }
}