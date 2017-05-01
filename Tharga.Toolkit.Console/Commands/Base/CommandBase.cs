using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Commands.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class CommandBase : ICommand
    {
        private readonly string _description;
        private readonly string[] _names;
        private readonly bool _hidden;

        public string Name => _names[0];
        public IEnumerable<string> Names => _names;
        public string Description => _description;
        public bool IsHidden => _hidden;

        public abstract IEnumerable<HelpLine> HelpText { get; }

        public event EventHandler<WriteEventArgs> WriteEvent;

        internal CommandBase(IEnumerable<string> names, string description, bool hidden)
        {
            _hidden = hidden;
            _names = names.Select(x => x.ToLower()).ToArray();
            _description = description ?? $"Command that manages {_names[0]}.";
        }

        //public abstract Task InvokeAsync(params string[] input); //TODO: Try to make this command happen! and replace the old style Invoke action
        public abstract Task<bool> InvokeAsync(string paramList);
        protected abstract ICommand GetHelpCommand(string paramList);

        public virtual bool CanExecute(out string reasonMesage)
        {
            reasonMesage = string.Empty;
            return true;
        }

        protected virtual string GetCanExecuteFailMessage(string reason)
        {
            return $"You cannot execute {Name} command." + (string.IsNullOrEmpty(reason) ? string.Empty : " " + reason);
        }

        internal virtual async Task<bool> InvokeWithCanExecuteCheckAsync(string paramList)
        {
            string reason;
            if (!CanExecute(out reason))
            {
                OutputWarning(GetCanExecuteFailMessage(reason));
                return true;
            }

            try
            {
                return await InvokeAsync(paramList);
            }
            catch (CommandEscapeException)
            {
                return false;
            }
        }

        protected static string GetParam(string paramList, int index)
        {
            if (paramList == null) return null;

            //Group items between delimiters " into one single string.
            var verbs = GetDelimiteredVerbs(ref paramList, '\"');

            var paramArray = paramList.Split(' ');
            if (paramArray.Length <= index) return null;

            //Put the grouped verbs back in to the original
            if (verbs.Count > 0) for (var i = 0; i < paramArray.Length; i++) if (verbs.ContainsKey(paramArray[i])) paramArray[i] = verbs[paramArray[i]];

            return paramArray[index];
        }

        private static Dictionary<string, string> GetDelimiteredVerbs(ref string paramList, char delimiter)
        {
            var verbs = new Dictionary<string, string>();

            var pos = paramList.IndexOf(delimiter, 0);
            while (pos != -1)
            {
                var endPos = paramList.IndexOf(delimiter, pos + 1);
                var data = paramList.Substring(pos + 1, endPos - pos - 1);
                var key = Guid.NewGuid().ToString();
                verbs.Add(key, data);

                paramList = paramList.Substring(0, pos) + key + paramList.Substring(endPos + 1);

                pos = paramList.IndexOf(delimiter, pos + 1);
            }

            return verbs;
        }

        //TODO: This is strictly a root command function
        public string QueryRootParam()
        {
            return QueryParam<string>("> ", null, null, false, false);
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
                    value = QueryParam(paramName, null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) }, true);
                }
                else
                {
                    value = QueryParam(paramName, null, (List<KeyValuePair<string, string>>)null, true);
                }
            }

            return value;
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue = null, string defaultValue = null)
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
                    value = QueryParam(paramName, null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) }, false);
                }
                else
                {
                    value = QueryParam(paramName, null, (List<KeyValuePair<string, string>>)null);
                }
            }

            var response = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
            return response;
        }

        protected async Task<T> QueryParamAsync<T>(string paramName, string autoProvideValue, Func<Task<IDictionary<T, string>>> selectionDelegate)
        {
            List<KeyValuePair<T, string>> selection = null;
            if (selectionDelegate != null)
            {
                OutputInformation($"Loading data for {paramName}...");
                selection = (await selectionDelegate()).ToList();
            }

            var response = QueryParam(paramName, autoProvideValue, selection);
            return response;
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, IDictionary<T, string> selectionDelegate)
        {
            return QueryParam(paramName, autoProvideValue, selectionDelegate, true, false);
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<KeyValuePair<T, string>> selectionDelegate, bool passwordEntry = false)
        {
            return QueryParam(paramName, autoProvideValue, selectionDelegate, true, passwordEntry);
        }

        private T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<KeyValuePair<T, string>> selectionDelegate, bool allowEscape, bool passwordEntry)
        {
            var selection = new List<KeyValuePair<T, string>>();
            if (selectionDelegate != null)
            {
                selection = selectionDelegate.OrderBy(x => x.Value).ToList();
                var q = GetParamByString(autoProvideValue, selection);
                if (q != null)
                {
                    return q.Value.Key;
                }
            }

            var inputManager = CommandEngine.InputManager;
            var response = inputManager.ReadLine(paramName + (!selection.Any() ? "" : " [Tab]"), selection.ToArray(), allowEscape, CommandEngine.CancellationToken, passwordEntry ? '*' : (char?)null, null);
            return response;
        }

        private static KeyValuePair<T, string>? GetParamByString<T>(string autoProvideValue, List<KeyValuePair<T, string>> selection)
        {
            if (!string.IsNullOrEmpty(autoProvideValue))
            {
                var item = selection.SingleOrDefault(x => string.Compare(x.Value, autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item.Value == autoProvideValue)
                {
                    return item;
                }

                item = selection.SingleOrDefault(x => string.Compare(x.Key.ToString(), autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item.Key != null && item.Key.ToString() == autoProvideValue)
                {
                    return item;
                }

                try
                {
                    var r = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(autoProvideValue);
                    return new KeyValuePair<T, string>(r, autoProvideValue);
                }
                catch (FormatException exception)
                {
                    throw new InvalidOperationException("Cannot find provided value in selection.", exception);
                }
            }

            return null;
        }

        protected void OutputError(Exception exception)
        {
            OutputError(exception.ToFormattedString());
        }

        protected void OutputError(string message)
        {
            WriteEvent?.Invoke(this, new WriteEventArgs(message, OutputLevel.Error));
        }

        protected void OutputWarning(string message)
        {
            WriteEvent?.Invoke(this, new WriteEventArgs(message, OutputLevel.Warning));
        }

        protected void OutputInformation(string message)
        {
            WriteEvent?.Invoke(this, new WriteEventArgs(message, OutputLevel.Information));
        }

        protected void OutputEvent(string message)
        {
            WriteEvent?.Invoke(this, new WriteEventArgs(message, OutputLevel.Event));
        }

        protected void OutputDefault(string message)
        {
            WriteEvent?.Invoke(this, new WriteEventArgs(message, OutputLevel.Default));
        }

        protected void OutputHelp(string message)
        {
            WriteEvent?.Invoke(this, new WriteEventArgs(message, OutputLevel.Help));
        }

        protected void OutputTable(IEnumerable<IEnumerable<string>> data)
        {
            OutputInformation(data.ToFormattedString());
        }

        public void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data)
        {
            OutputTable(new[] { title }.Union(data));
        }
    }
}