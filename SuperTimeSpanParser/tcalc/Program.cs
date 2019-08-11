using System;
using Superpower.Model;
using tcalc.Evaluation;
using tcalc.Parsing;

namespace tcalc
{
    class Program
    {
        const string Prompt = "tcalc> ";

        static void Main()
        {
            Console.Write(Prompt);
            var line = Console.ReadLine();
            while (line != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    try
                    {
                        var tokens = ExpressionTokenizer.TryTokenize(line);
                        if (!tokens.HasValue)
                        {
                            WriteSyntaxError(tokens.ToString(), tokens.ErrorPosition);
                        }
                        else if (!ExpressionParser.TryParse(tokens.Value, out var expr, out var error, out var errorPosition))
                        {
                            WriteSyntaxError(error, errorPosition);
                        }
                        else
                        {
                            var result = ExpressionEvaluator.Evaluate(expr);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine(result);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                    }

                    Console.ResetColor();
                    Console.WriteLine();
                }

                Console.Write(Prompt);
                line = Console.ReadLine();
            }
        }

        static void WriteSyntaxError(string message, Position errorPosition)
        {
            if (errorPosition.HasValue && errorPosition.Line == 1)
                Console.WriteLine(new string(' ', Prompt.Length + errorPosition.Column - 1) + '^');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}
