using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Console
{
    internal static class IntExtensions
    {
        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, int defaultValue)
        {
            // Previously called Any() and then Max(), enumerating the source
            // twice. That is wrong for a lazy or single-pass sequence, so walk
            // it once instead.
            var found = false;
            var max = defaultValue;

            foreach (var item in source)
            {
                var value = selector(item);
                if (!found || value > max)
                {
                    max = value;
                    found = true;
                }
            }

            return found ? max : defaultValue;
        }

        public static int Max(this int value, int other)
        {
            return value > other ? value : other;
        }
    }
}