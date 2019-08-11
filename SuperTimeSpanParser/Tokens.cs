using Superpower.Display;
using System;
using System.Collections.Generic;
using System.Text;

namespace SuperTimeSpanParser
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