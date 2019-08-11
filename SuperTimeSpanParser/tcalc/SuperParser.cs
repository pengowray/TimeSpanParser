using Superpower.Model;
using System;
using System.Collections.Generic;
using System.Text;
using tcalc.Evaluation;
using tcalc.Expressions;
using tcalc.Parsing;

namespace SuperTimeSpanParser.tcalc {
    public class SuperParser {

        // via Program.Main() + ExpressionEvaluator.Evaluate()
        public static TimeSpan? ParseTimeSpan(string line) {
            //Console.WriteLine("hello");
            var tokens = ExpressionTokenizer.TryTokenize(line);
            if (!tokens.HasValue) {
                Console.WriteLine($"no, pos:{tokens.ErrorPosition}, {tokens.ToString()}");
                return null;
            }
            
            var success = (!ExpressionParser.TryParse(tokens.Value, out var expr, out var error, out var errorPosition)) ;

            if (!success) {
                var result = ExpressionEvaluator.Evaluate(expr);
                if (result is DurationResult)
                    return ((DurationResult)result).Value;

                //TODO: possible NumericResult
                Console.WriteLine("no success: " + result);
                return null;
            }

            Expression expression = expr;

            if (expression == null) throw new ArgumentNullException(nameof(expression));
            
            switch (expression) {
                case DurationValue duration:
                    Console.WriteLine("From expression");
                    //return new DurationResult(duration.Value);
                    return duration.Value;
                case NumericValue numeric:
                    //return new NumericResult(numeric.Value);
                    Console.WriteLine("numeric");
                    return null;
                case BinaryExpression binary:
                    //return DispatchOperator(Evaluate(binary.Left), Evaluate(binary.Right), binary.Operator);
                    Console.WriteLine("binary");
                    return null;
                default:
                    //throw new ArgumentException($"Unsupported expression {expression}.");
                    Console.WriteLine("default / Unsupported expression ");
                    return null;
            }
        }

        /*
        public static TimeSpan? ParseTimeSpan(string line) {
            try {
                var tokens = ExpressionTokenizer.TryTokenize(line);
                if (!tokens.HasValue) {
                    WriteSyntaxError(line, tokens.ToString(), tokens.ErrorPosition);
                    return null;
                } else if (!ExpressionParser.TryParse(tokens.Value, out var expr, out var error, out var errorPosition)) {
                    WriteSyntaxError(line, error, errorPosition);
                } else {
                    var result = ExpressionEvaluator.Evaluate(expr);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(result);
                    return result;
                }
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
            }

        }
        */

        static void WriteSyntaxError(string line, string message, Position errorPosition) {
            string Prompt = "";
            Console.WriteLine(line);
            if (errorPosition.HasValue && errorPosition.Line == 1)
                Console.WriteLine(new string(' ', Prompt.Length + errorPosition.Column - 1) + '^');
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

    }
}
