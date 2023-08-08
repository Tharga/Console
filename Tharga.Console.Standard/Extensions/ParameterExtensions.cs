using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Console
{
    public static class Param
    {
        #region Build

        public static IEnumerable<KeyValuePair<object, string>> Build(Type type)
        {
            if (type.IsEnum)
            {
                foreach (var item in Enum.GetValues(type))
                {
                    yield return new KeyValuePair<object, string>(item, item.ToString());
                }
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                yield return new KeyValuePair<object, string>(true, true.ToString());
                yield return new KeyValuePair<object, string>(false, false.ToString());
            }
        }

        public static IEnumerable<KeyValuePair<T, string>> Build<T>()
        {
            if (default(T) == null)
            {
                var tp = Nullable.GetUnderlyingType(typeof(T));
                if (tp != null)
                {
                    return Build(tp).Select(x => new KeyValuePair<T, string>((T)x.Key, x.Value));
                }
            }

            return Build(typeof(T)).Select(x => new KeyValuePair<T, string>((T)x.Key, x.Value));
        }

        public static IEnumerable<KeyValuePair<T, string>> Build<T>(string defaultParam)
        {
            yield return new KeyValuePair<T, string>(default, defaultParam ?? string.Empty);

            if (default(T) == null)
            {
                var tp = Nullable.GetUnderlyingType(typeof(T));
                if (tp != null)
                {
                    var items = Build(tp);
                    foreach (var item in items)
                    {
                        yield return new KeyValuePair<T, string>((T)item.Key, item.Value);
                    }
                    yield break;
                }
            }

            foreach (var item in Build<T>())
            {
                yield return new KeyValuePair<T, string>(item.Key, item.Value);
            }
        }

        #endregion
        #region AsOption

        public static IEnumerable<KeyValuePair<T, string>> AsOption<T>()
        {
            return Build<T>();
        }

        public static IEnumerable<KeyValuePair<T, string>> AsOption<T>(this T item)
        {
            yield return new KeyValuePair<T, string>(item, item?.ToString() ?? string.Empty);
        }

        public static IEnumerable<KeyValuePair<T, string>> AsOption<T>(this (T, string) item)
        {
            yield return new KeyValuePair<T, string>(item.Item1, item.Item2);
        }

        //public static IEnumerable<KeyValuePair<T, string>> AsOption<T>(this IEnumerable<T> items)
        //{
        //    return items.Select(x => new KeyValuePair<T, string>(x, x.ToString()));
        //}

        //public static IEnumerable<KeyValuePair<T?, string>> AsOption<T>(this T[] items, string nullOption = null)
        //    where T : struct
        //{
        //    if (nullOption != null)
        //    {
        //        yield return new KeyValuePair<T?, string>(null, nullOption);
        //    }

        //    foreach (var item in items)
        //    {
        //        yield return new KeyValuePair<T?, string>(item, item.ToString());
        //    }
        //}

        #endregion
        #region Nullable

        public static IEnumerable<KeyValuePair<T?, string>> AsNullable<T>(this IEnumerable<KeyValuePair<T, string>> item)
            where T : struct
        {
            return item.Select(x => new KeyValuePair<T?, string>(x.Key, x.Value));
        }

        #endregion
        #region Append

        public static IEnumerable<KeyValuePair<T, string>> And<T>(this IEnumerable<KeyValuePair<T, string>> item, T add)
        {
            return item.Union(new[] { new KeyValuePair<T, string>(add, add.ToString()) });
        }

        public static IEnumerable<KeyValuePair<T, string>> And<T>(this IEnumerable<KeyValuePair<T, string>> item, (T, string) add)
        {
            return item.Union(new[] { new KeyValuePair<T, string>(add.Item1, add.Item2) });
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

        //public static IEnumerable<T> And<T>(this T[] item, T add)
        //{
        //    foreach (var itm in item)
        //    {
        //        yield return itm;
        //    }
        //    yield return add;
        //}

        //    public static IEnumerable<T> And<T>(this IEnumerable<T> item, T add)
        //    {
        //        foreach (var itm in item)
        //        {
        //            yield return itm;
        //        }
        //        yield return add;
        //    }

        #endregion
    }
}