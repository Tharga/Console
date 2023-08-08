using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Console
{
    internal static class IntExtensions
    {
        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
        {
            if (!source.Any()) return defaultValue;
            var val = source.Max(selector);
            return val;
        }

        public static int Max(this int value, int other)
        {
            return value > other ? value : other;
        }
    }
}