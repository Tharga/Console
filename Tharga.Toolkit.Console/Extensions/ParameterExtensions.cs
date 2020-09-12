using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Toolkit.Console
{
    public static class ParameterExtensions
    {
        public static IEnumerable<KeyValuePair<T, string>> AsOption<T>(this T item)
        {
            return new Dictionary<T, string> { { item, item.ToString() } };
        }

        public static IEnumerable<KeyValuePair<T, string>> AsOption<T>()
        {
            if (default(T) is Enum)
            {
                foreach (var item in Enum.GetValues(typeof(T)).Cast<T>())
                {
                    yield return new KeyValuePair<T, string>(item, item.ToString());
                }
            }

            if (default(T) is bool)
            {
                yield return new KeyValuePair<T, string>((T)(object)true, true.ToString());
                yield return new KeyValuePair<T, string>((T)(object)false, false.ToString());
            }
        }

        public static IEnumerable<KeyValuePair<T?, string>> AsOption<T>(string nullOption)
            where T : struct
        {
            yield return new KeyValuePair<T?, string>(null, nullOption);
            foreach (var item in AsOption<T>())
            {
                yield return new KeyValuePair<T?, string>(item.Key, item.Value);
            }
        }

        public static IEnumerable<KeyValuePair<T?, string>> AsNullable<T>(this IEnumerable<KeyValuePair<T, string>> item)
            where T : struct
        {
            return item.Select(x => new KeyValuePair<T?, string>(x.Key, x.Value));
        }

        public static IEnumerable<T> And<T>(this IEnumerable<T> item, T add)
        {
            foreach (var itm in item)
            {
                yield return itm;
            }
            yield return add;
        }

        public static IEnumerable<T> And<T>(this T item, T add)
        {
            yield return item;
            yield return add;
        }

        public static IEnumerable<KeyValuePair<T, string>> And<T>(this IDictionary<T, string> item, T add)
        {
            return item.Union(new []{ new KeyValuePair<T, string>(add, add.ToString()) });
        }

        public static IEnumerable<KeyValuePair<T, string>> And<T>(this IEnumerable<KeyValuePair<T, string>> item, T add)
        {
            return item.Union(new[] { new KeyValuePair<T, string>(add, add.ToString()) });
        }

        public static IEnumerable<KeyValuePair<T, string>> And<T>(this IEnumerable<KeyValuePair<T, string>> item, (T, string) add)
        {
            return item.Union(new[] { new KeyValuePair<T, string>(add.Item1, add.Item2) });
        }

        //public static IEnumerable<KeyValuePair<T, string>> And<T>(this KeyValuePair<T, string>[] item, T add)
        //{
        //    return item.Union(new[] { new KeyValuePair<T, string>(add, add.ToString()) });
        //}
    }
}