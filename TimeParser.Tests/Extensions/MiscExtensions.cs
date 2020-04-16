using System;
using System.Collections.Generic;
using System.Linq;

namespace TimeSpanParserUtil.Tests.Extensions {
    public static class MiscExtensions {
        // Ex: collection.TakeLast(5);
        // Use if using .net Standard
        public static IEnumerable<T> TakeLast2<T>(this IEnumerable<T> source, int N) {
            return source.Skip(Math.Max(0, source.Count() - N));
        }
    }
}
