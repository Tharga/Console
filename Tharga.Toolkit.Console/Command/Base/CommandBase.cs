using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class CommandBase : ICommandBase
    {
        private static readonly object SyncRoot = new object();

        private readonly string _description;
        private readonly string[] _names;
        private IConsole _console;
        protected HelpCommand HelpCommand { get; set; }
        public string Name { get { return _names[0]; } }
        public IEnumerable<string> Names { get { return _names; } }
        public string Description { get { return _description; } }
        internal IConsole Console { get { return _console; } }

        internal CommandBase(IConsole console, string name, string description = null)
        {
            _console = console;
            _names = new[] { name.ToLower() };
            _description = description;
        }

        internal CommandBase(IConsole console, IEnumerable<string> names, string description = null)
        {
            _console = console;
            _names = names.Select(x => x.ToLower()).ToArray();
            _description = description;
        }

        public abstract Task<bool> InvokeAsync(string paramList);
        protected abstract CommandBase GetHelpCommand();
        public abstract bool CanExecute();

        protected virtual string GetCanExecuteFailMessage()
        {
            return string.Format("You cannot execute this {0} command.", Name);
        }

        public virtual async Task<bool> InvokeWithCanExecuteCheckAsync(string paramList)
        {
            if (!CanExecute())
            {
                OutputWarning(GetCanExecuteFailMessage());
                return true;
            }

            try
            {
                return await InvokeAsync(paramList);
            }
            catch (CommandEscapeException)
            {
                //TODO: This is used to exit an ongoing command. Is there another way, that does not use exceptions?
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

        internal string QueryRootParam()
        {
            return QueryParam<string>("> ", null, null, false);
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
                    value = QueryParam(paramName, null, () => new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) });
                }
                else
                {
                    value = QueryParam(paramName, null, (Func<List<KeyValuePair<string, string>>>)null);
                }
            }

            var response = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
            return response;
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, Func<List<KeyValuePair<T, string>>> selectionDelegate)
        {
            return QueryParam<T>(paramName, autoProvideValue, selectionDelegate, true);
        }

        private T QueryParam<T>(string paramName, string autoProvideValue, Func<List<KeyValuePair<T, string>>> selectionDelegate, bool allowEscape)
        {
            var selection = new List<KeyValuePair<T, string>>();
            if (selectionDelegate != null)
            {
                selection = selectionDelegate.Invoke();
                var q = GetParamByString(autoProvideValue, selection);
                if (q != null)
                {
                    return q.Value.Key;
                }
            }

            var inputManager = new InputManager(_console, this, paramName);
            var response = inputManager.ReadLine(selection.ToArray(), allowEscape);
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
                if (item.Key.ToString() == autoProvideValue)
                {
                    return item;
                }

                throw new InvalidOperationException("Cannot find provided value in selection.");
            }

            return null;
        }

        public void OutputError(string message, params object[] args)
        {
            Output(message, ConsoleColor.Red, true, args);
        }

        public void OutputWarning(string message, params object[] args)
        {
            Output(message, ConsoleColor.Yellow, true, args);
        }

        public void OutputInformation(string message, params object[] args)
        {
            Output(message, null, true, args);
        }

        public void OutputEvent(string message, params object[] args)
        {
            lock (SyncRoot)
            {
                var cursorLeft = _console.CursorLeft;
                if (cursorLeft != 0)
                {
                    message = string.Format(message, args);
                    var lines = (int)Math.Ceiling(message.Length / (decimal)_console.BufferWidth);
                    _console.MoveBufferArea(0, _console.CursorTop, cursorLeft, 1, 0, _console.CursorTop + lines);
                    _console.SetCursorPosition(0, _console.CursorTop);
                }

                Output(message, ConsoleColor.Yellow, true, args);
                if (cursorLeft != 0)
                {
                    _console.SetCursorPosition(cursorLeft, _console.CursorTop);
                }
            }
        }

        public void Output(string message, ConsoleColor? color, bool line, params object[] args)
        {
            lock (SyncRoot)
            {
                var defaultColor = ConsoleColor.White;
                try
                {
                    if (color != null)
                    {
                        defaultColor = _console.ForegroundColor;
                        _console.ForegroundColor = color.Value;
                    }

                    if (line)
                    {
                        if (args == null)
                        {
                            _console.WriteLine(message);
                        }
                        else
                        {
                            _console.WriteLine(string.Format(message, args));
                        }
                    }
                    else
                    {
                        _console.Write(message, args);
                    }
                }
                finally
                {
                    if (color != null)
                    {
                        _console.ForegroundColor = defaultColor;
                    }
                }
            }
        }

        internal void OutputInformationLine(string message, bool commandMode)
        {
            if (commandMode)
            {
                //By default, do not output information when in command mode
                return;
            }

            Output(message, null, true, null);
        }

        public virtual void CommandRegistered(IConsole console)
        {
            _console = console;
        }
    }
}