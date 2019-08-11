using System;
using System.Threading;
using Superpower;

// ReSharper disable UnusedMember.Global

namespace tcalc.Parsing
{
    static class ParserExtensions
    {
        static int _instance;

        public static TextParser<T> Log<T>(this TextParser<T> parser, string name)
        {
            var id = Interlocked.Increment(ref _instance);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{id}] Constructing instance of {name}");
            Console.ResetColor();
            return i =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Invoking with input: {i}");
                Console.ResetColor();
                var result = parser(i);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Result: {result}");
                Console.ResetColor();
                return result;
            };
        }

        public static TokenListParser<TKind, T> Log<TKind, T>(this TokenListParser<TKind, T> parser, string name)
        {
            var id = Interlocked.Increment(ref _instance);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{id}] Constructing instance of {name}");
            Console.ResetColor();
            return i =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Invoking with input: {i}");
                Console.ResetColor();
                var result = parser(i);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{id}] Result: {result}");
                Console.ResetColor();
                return result;
            };
        }
    }
}
