using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.Base
{
    public abstract class QueryParamBase
    {
        protected abstract IInputManager InputManager { get; }
        protected abstract CancellationToken CancellationToken { get; }

        protected abstract string GetNextParam(IEnumerable<string> param);

        public string QueryPassword(string paramName, IEnumerable<string> autoParam, string defaultValue = null)
        {
            return QueryPassword(paramName, GetNextParam(autoParam), defaultValue);
        }

        public string QueryPassword(string paramName, string autoProvideValue = null, string defaultValue = null)
        {
            return QueryParam<string>(paramName, autoProvideValue, defaultValue);
        }

        public T QueryParam<T>(string paramName, bool sortParameters = true)
        {
            var selection = GenerateSelection<T>();
            return QueryParam(paramName, null, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<string> autoParam, bool sortParameters = true)
        {
            var selection = GenerateSelection<T>();
            var autoProvideValue = GetNextParam(autoParam);
            return QueryParam(paramName, autoProvideValue, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IDictionary<T, string> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Key, x.Value));
            var autoProvideValue = GetNextParam(autoParam);
            return QueryParam(paramName, autoProvideValue, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IEnumerable<KeyValuePair<T,string>> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Key, x.Value));
            var autoProvideValue = GetNextParam(autoParam);
            return QueryParam(paramName, autoProvideValue, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IEnumerable<(T, string)> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Item1, x.Item2));
            var autoProvideValue = GetNextParam(autoParam);
            return QueryParam(paramName, autoProvideValue, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IEnumerable<Tuple<T, string>> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Item1, x.Item2));
            var autoProvideValue = GetNextParam(autoParam);
            return QueryParam(paramName, autoProvideValue, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IEnumerable<T> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x, x.ToString()));
            var autoProvideValue = GetNextParam(autoParam);
            return QueryParam(paramName, autoProvideValue, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IDictionary<T, string> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Key, x.Value));
            return QueryParam(paramName, null, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<KeyValuePair<T, string>> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Key, x.Value));
            return QueryParam(paramName, null, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<(T, string)> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Item1, x.Item2));
            return QueryParam(paramName, null, selection, true, false, sortParameters);
        }

        public T QueryParam<T>(string paramName, IEnumerable<Tuple<T, string>> options, bool sortParameters = true)
        {
            var selection = options?.Select(x => new CommandTreeNode<T>(x.Item1, x.Item2));
            return QueryParam(paramName, null, selection, true, false, sortParameters);
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue = null, string defaultValue = null, bool sortParameters = true)
        {
            return QueryParam<T>(paramName, autoProvideValue, defaultValue, false, sortParameters);
        }

        private T QueryParam<T>(string paramName, string autoProvideValue, string defaultValue, bool passwordEntry, bool sortParameters)
        {
            try
            {
                string value;

                if (!string.IsNullOrEmpty(autoProvideValue))
                {
                    value = autoProvideValue;
                }
                else
                {
                    if (!string.IsNullOrEmpty(defaultValue))
                    {
                        var p1 = new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) };
                        var p2 = p1.Select(x => new CommandTreeNode<string>(x.Key, x.Value));
                        value = QueryParam(paramName, null, p2, true, passwordEntry, sortParameters);
                    }
                    else
                    {
                        value = QueryParam(paramName, null, (IEnumerable<CommandTreeNode<string>>)null, true, passwordEntry, sortParameters);
                    }
                }

                var response = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
                return response;
            }
            catch (Exception exception)
            {
                if (exception.InnerException?.GetType() == typeof(FormatException))
                {
                    throw exception.InnerException;
                }

                throw;
            }
        }

        internal T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<CommandTreeNode<T>> selection, bool allowEscape, bool passwordEntry, bool sortParameters)
        {
            if (sortParameters) selection = selection?.OrderBy(x => x.Value);
            var sel = new CommandTreeNode<T>(selection?.ToArray() ?? new CommandTreeNode<T>[] { });

            var q = GetParamByString(autoProvideValue, sel);
            if (q != null)
            {
                return q.Key;
            }

            var prompt = paramName + ((!sel.Subs.Any() || paramName == Constants.Prompt) ? string.Empty : " [Tab]");
            var response = InputManager.ReadLine(prompt, sel, allowEscape, CancellationToken, passwordEntry ? '*' : (char?)null, null);
            return response;
        }

        private static IEnumerable<CommandTreeNode<T>> GenerateSelection<T>()
        {
            var selection = Param.AsOption<T>().ToArray();
            return selection.Select(x => new CommandTreeNode<T>(x.Key, x.Value));
        }

        private static CommandTreeNode<T> GetParamByString<T>(string autoProvideValue, CommandTreeNode<T> selection)
        {
            if (!string.IsNullOrEmpty(autoProvideValue))
            {
                var item = selection.Subs.SingleOrDefault(x => string.Compare(x.Value, autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (string.Equals(item?.Value, autoProvideValue, StringComparison.CurrentCultureIgnoreCase))
                {
                    return item;
                }

                item = selection.Subs.SingleOrDefault(x => string.Compare(x.Key.ToString(), autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item != null)
                {
                    if (item.Key != null && item.Key.ToString() == autoProvideValue)
                    {
                        return item;
                    }
                }

                try
                {
                    var r = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(autoProvideValue);
                    return new CommandTreeNode<T>(r, autoProvideValue);
                }
                catch (FormatException exception)
                {
                    throw new InvalidOperationException("Cannot find provided value in selection.", exception);
                }
            }

            return null;
        }
    }
}