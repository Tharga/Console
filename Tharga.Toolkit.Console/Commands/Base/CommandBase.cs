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

        public string Name => _names.Keys.First();
        public IEnumerable<string> Names => _names.Select(x => x.Key);
        public string Description { get; }
        public bool IsHidden { get; }

        public abstract IEnumerable<HelpLine> HelpText { get; }

        public event EventHandler<WriteEventArgs> WriteEvent;

        protected CommandEngine CommandEngine;
        protected int ParamIndex;

        internal CommandBase(string name, string description = null, bool hidden = false)
        {
            IsHidden = hidden;
            _names = new Dictionary<string, string> { { name.ToLower(), name } };
            Description = description ?? $"Command that manages {name}.";
        }

        public abstract void Invoke(string[] param);

        internal void InvokeEx(string[] param)
        {
            ParamIndex = 0;
            Invoke(param);
        }

        protected void AddName(string name)
        {
            if (!_names.ContainsKey(name.ToLower()))
                _names.Add(name.ToLower(), name);
        }

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

        protected string GetNextParam(IEnumerable<string> param)
        {
            return GetParam(param, ParamIndex);
        }

        protected string GetParam(IEnumerable<string> param, int index)
        {
            if (param == null) return null;
            var enumerable = param as string[] ?? param.ToArray();
            if (!enumerable.Any()) return null;
            if (enumerable.Length <= index) return null;
            ParamIndex++;
            return enumerable[index];
        }

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

        protected T QueryParam<T>(string paramName, IEnumerable<string> autoParam, string defaultValue = null)
        {
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

        internal protected T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<CommandTreeNode<T>> selection, bool allowEscape, bool passwordEntry)
        {
            if (CommandEngine == null) throw new InvalidOperationException("The command engine has not been assigned yet.");

            var sel = new CommandTreeNode<T>(selection?.OrderBy(x => x.Value).ToArray() ?? new CommandTreeNode<T>[] { });
            var q = GetParamByString(autoProvideValue, sel);
            if (q != null)
            {
                return q.Key;
            }

            var inputManager = CommandEngine.InputManager;
            var prompt = paramName + ((!sel.Subs.Any() || paramName == Constants.Prompt) ? string.Empty : " [Tab]");
            var response = inputManager.ReadLine(prompt, sel, allowEscape, CommandEngine.CancellationToken, passwordEntry ? '*' : (char?)null, null);
            return response;
        }

        private static CommandTreeNode<T> GetParamByString<T>(string autoProvideValue, CommandTreeNode<T> selection)
        {
            if (!string.IsNullOrEmpty(autoProvideValue))
            {
                var item = selection.Subs.SingleOrDefault(x => string.Compare(x.Value, autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item?.Value == autoProvideValue)
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

        protected void OutputError(Exception exception, bool includeStackTrace = false)
        {
            OutputError(exception.ToFormattedString(includeStackTrace));
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