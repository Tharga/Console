using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class CommandBase : ICommand
    {
        private readonly Dictionary<string, string> _names;
        private readonly string _description;
        private readonly bool _hidden;

        public string Name => _names.Keys.First();
        public IEnumerable<string> Names => _names.Select(x => x.Key);
        public string Description => _description;
        public bool IsHidden => _hidden;

        public abstract IEnumerable<HelpLine> HelpText { get; }

        public event EventHandler<WriteEventArgs> WriteEvent;

        protected CommandEngine CommandEngine;
        protected int ParamIndex;

        internal CommandBase(string name, string description = null, bool hidden = false)
        {
            _hidden = hidden;
            _names = new Dictionary<string, string> { { name.ToLower(), name } };
            _description = description ?? $"Command that manages {name}.";
        }

        public abstract void Invoke(params string[] param);

        internal void InvokeEx(params string[] param)
        {
            ParamIndex = 0;
            Invoke(param);
        }

        protected void AddName(string name)
        {
            if (!_names.ContainsKey(name.ToLower()))
                _names.Add(name.ToLower(), name);
        }

        //[Obsolete("Start using the function 'Task InvokeAsync(params string[] input)' instead. This feature will be removed.")]
        //public virtual async Task<bool> InvokeAsync(string paramList)
        //{
        //    throw new NotSupportedException("Start implementing 'Task InvokeAsync(params string[] input)' instead of using this legacy function.");
        //}

        protected abstract ICommand GetHelpCommand(string paramList);

        protected internal virtual void Attach(CommandEngine commandEngine)
        {
            CommandEngine = commandEngine;
        }

        public virtual bool CanExecute(out string reasonMesage)
        {
            reasonMesage = string.Empty;
            return true;
        }

        protected virtual string GetCanExecuteFailMessage(string reason)
        {
            return $"You cannot execute {Name} command." + (string.IsNullOrEmpty(reason) ? string.Empty : " " + reason);
        }

        protected string GetNextParam(string[] param)
        {
            return GetParam(param, ParamIndex++);
        }

        protected string GetParam(string[] param, int index)
        {
            if (param == null) return null;
            if (!param.Any()) return null;
            if (param.Length <= index) return null;
            return param[index];
        }

        [Obsolete("Use GetParam with parameter 'string[] input' instead.")]
        protected static string GetParam(string paramList, int index)
        {
            if (paramList == null) return null;

            //Group items between delimiters " into one single string.
            var verbs = ParamExtensions.GetDelimiteredVerbs(ref paramList, '\"');

            var paramArray = paramList.Split(' ');
            if (paramArray.Length <= index) return null;

            //Put the grouped verbs back in to the original
            if (verbs.Count > 0) for (var i = 0; i < paramArray.Length; i++) if (verbs.ContainsKey(paramArray[i])) paramArray[i] = verbs[paramArray[i]];

            return paramArray[index];
        }

        //TODO: This is strictly a root command function
        public string QueryRootParam()
        {
            return QueryParam<string>(Constants.Prompt, null, null, false, false);
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

        protected T QueryParam<T>(string paramName, string[] autoParam = null, string defaultValue = null)
        {
            return QueryParam<T>(paramName, GetNextParam(autoParam), defaultValue);
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

        protected T QueryParam<T>(string paramName, string[] autoParam, IDictionary<T, string> selectionDelegate)
        {
            return QueryParam(paramName, GetNextParam(autoParam), selectionDelegate, true, false);
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

        protected void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data)
        {
            OutputTable(new[] { title }.Union(data));
        }
    }
}