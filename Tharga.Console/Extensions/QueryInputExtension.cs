using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console
{
    public static class QueryInputExtension
    {
        public static string QueryPassword(this IConsole console, string paramName, IEnumerable<string> autoParam, string defaultValue = null)
        {
            return new QueryInput(console).QueryPassword(paramName, autoParam, defaultValue);
        }

        public static string QueryPassword(this IConsole console, string paramName, string autoProvideValue = null, string defaultValue = null)
        {
            return new QueryInput(console).QueryPassword(paramName, autoProvideValue, defaultValue);
        }

        public static T QueryParam<T>(this IConsole console, string paramName)
        {
            return new QueryInput(console).QueryParam<T>(paramName);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<string> autoParam)
        {
            return new QueryInput(console).QueryParam<T>(paramName, autoParam);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<string> autoParam, IDictionary<T, string> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, autoParam, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<string> autoParam, IEnumerable<KeyValuePair<T, string>> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, autoParam, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<string> autoParam, IEnumerable<(T, string)> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, autoParam, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<string> autoParam, IEnumerable<Tuple<T, string>> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, autoParam, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<string> autoParam, IEnumerable<T> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, autoParam, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IDictionary<T, string> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<KeyValuePair<T, string>> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<(T, string)> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, options);
        }

        public static T QueryParam<T>(this IConsole console, string paramName, IEnumerable<Tuple<T, string>> options)
        {
            return new QueryInput(console).QueryParam<T>(paramName, options);
        }
    }
}