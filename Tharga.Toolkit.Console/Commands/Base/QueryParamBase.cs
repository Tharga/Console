using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class QueryParamBase
    {
        protected abstract IInputManager InputManager { get; }
        protected abstract CancellationToken CancellationToken { get; }

        protected abstract string GetNextParam(IEnumerable<string> param);

        protected string QueryPassword(string paramName, IEnumerable<string> autoParam, string defaultValue = null)
        {
            return QueryPassword(paramName, GetNextParam(autoParam), defaultValue);
        }

        protected string QueryPassword(string paramName, string autoProvideValue = null, string defaultValue = null)
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
                    value = QueryParam(paramName, (string)null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) }, true);
                }
                else
                {
                    value = QueryParam(paramName, (string)null, (List<KeyValuePair<string, string>>)null, true);
                }
            }

            return value;
        }

        public T QueryParam<T>(string paramName, IEnumerable<string> autoParam, string defaultValue = null)
        {
            if (defaultValue == null)
            {
                if (default(T) is Enum)
                {
                    var selection = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(x => x, x => x.ToString());
                    return QueryParam<T>(paramName, GetNextParam(autoParam), selection);
                }
                if (default(T) is bool)
                {
                    var selection = new Dictionary<T, string> { { (T)(object)true, true.ToString() }, { (T)(object)false, false.ToString() } };
                    return QueryParam<T>(paramName, GetNextParam(autoParam), selection);
                }
            }

            return QueryParam<T>(paramName, GetNextParam(autoParam), defaultValue);
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue = null, string defaultValue = null)
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
                        value = QueryParam(paramName, (string)null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) }, false);
                    }
                    else
                    {
                        value = QueryParam(paramName, (string)null, (List<KeyValuePair<string, string>>)null);
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

        protected T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IDictionary<T, string> selectionDelegate)
        {
            return QueryParam(paramName, GetNextParam(autoParam), selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, false);
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, IDictionary<T, string> selectionDelegate)
        {
            return QueryParam(paramName, autoProvideValue, selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, false);
        }

        protected T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IEnumerable<KeyValuePair<T, string>> selectionDelegate, bool passwordEntry = false)
        {
            return QueryParam<T>(paramName, GetNextParam(autoParam), selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, passwordEntry);
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<KeyValuePair<T, string>> selectionDelegate, bool passwordEntry = false)
        {
            return QueryParam(paramName, autoProvideValue, selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, passwordEntry);
        }

        protected internal T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<CommandTreeNode<T>> selection, bool allowEscape, bool passwordEntry)
        {
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