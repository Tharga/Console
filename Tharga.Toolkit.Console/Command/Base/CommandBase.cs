using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class CommandBase
    {
        private readonly static object SyncRoot = new object();

        protected readonly IConsole _console;

        private readonly string _name;
        private readonly string _description;
        protected HelpCommand HelpCommand;

        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        
        internal CommandBase(IConsole console, string name, string description = null)
        {
            _console = console ?? new ClientConsole();
            _name = name.ToLower();
            _description = description;
        }

        public abstract Task<bool> InvokeAsync(string paramList);
        protected abstract CommandBase GetHelpCommand();
        public abstract bool CanExecute();
        protected virtual string GetCanExecuteFaileMessage()
        {
            return string.Format("You cannot execute this {0} command.", Name);
        }

        public virtual async Task<bool> InvokeWithCanExecuteCheckAsync(string paramList)
        {
            if (!CanExecute())
            {
                OutputWarning(GetCanExecuteFaileMessage());
                return true;
            }

            return await InvokeAsync(paramList);
        }

        protected static string GetParam(string paramList, int index)
        {
            if (paramList == null) return null;

            //Group items between delimiters " into one single string.
            var verbs = GetDelimiteredVerbs(ref paramList, '\"');

            var paramArray = paramList.Split(' ');
            if (paramArray.Length <= index)
                return null;

            //Put the grouped verbs back in to the original
            if (verbs.Count > 0)
                for (var i = 0; i < paramArray.Length; i++)
                    if (verbs.ContainsKey(paramArray[i]))
                        paramArray[i] = verbs[paramArray[i]];

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

        #region IO

        public T QueryParam<T>(string paramName, string autoProvideValue = null, string defaultValue = null)
        {
            var value = QueryParam(paramName, autoProvideValue, defaultValue);
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
        }

        private string QueryParam(string paramName, string autoProvideValue, string defaultValue)
        {
            if (!string.IsNullOrEmpty(autoProvideValue))
                return autoProvideValue;

            if (!string.IsNullOrEmpty(defaultValue))
                return QueryParam(paramName, null, () => new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) });

            Output(string.Format("{0}{1}", paramName, paramName.Length > 2 ? ": " : string.Empty), null, false);

            var read = _console.ReadLine();
            System.Diagnostics.Debug.WriteLine("Read: {0}", read);
            return read;
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, Func<List<KeyValuePair<T, string>>> selectionDelegate)
        {
            var selection = selectionDelegate.Invoke();

            var q = GetParamByString(autoProvideValue, selection);
            if (q != null)
                return q.Value.Key;

            Output(string.Format("{0} [TAB]: ", paramName), null, false);

            var left = _console.CursorLeft;
            var tabIndex = -1;
            var input = string.Empty;

            while (true)
            {
                var key = _console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    _console.NewLine();
                    if (tabIndex == -1 || selection == null)
                    {
                        var q2 = GetParamByString(input, selection);
                        if (q2 != null)
                            return q2.Value.Key;
                        //return (T) Convert.ChangeType(input, typeof (T));
                        return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(input);
                    }
                    return selection[tabIndex].Key;
                }
                if (key.KeyChar >= 32 && key.KeyChar <= 126)
                {
                    input += key.KeyChar; //NOTE: When changing the input, automatically exit tab mode (tabIndex = -1;)
                    tabIndex = -1;
                }
                else
                {
                    //Take special action
                    switch (key.Key)
                    {
                        case ConsoleKey.Backspace:
                            if (_console.CursorLeft >= left)
                            {
                                input = input.Substring(0, input.Length - 1);
                                tabIndex = -1;
                                _console.Write(" ");
                                _console.CursorLeft--;
                            }
                            else
                                _console.CursorLeft++;
                            break;
                        case ConsoleKey.Tab:

                            if (!selection.Any())
                            {
                                System.Diagnostics.Debug.WriteLine("There are no selections.");
                                _console.CursorLeft--;
                                break;
                            }

                            if (tabIndex >= selection.Count() - 1)
                                tabIndex = -1;

                            var emptyString = new string(' ', _console.CursorLeft - left);
                            _console.CursorLeft = left;
                            _console.Write(emptyString);

                            _console.CursorLeft = left;
                            _console.Write(selection[++tabIndex].Value);
                            input = selection[tabIndex].Value;
                            break;
                        default:
                            System.Diagnostics.Debug.WriteLine(key);
                            _console.CursorLeft--;
                            break;
                    }
                }
            }
        }

        private static KeyValuePair<T, string>? GetParamByString<T>(string autoProvideValue, List<KeyValuePair<T, string>> selection)
        {
            if (!string.IsNullOrEmpty(autoProvideValue))
            {
                var item = selection.SingleOrDefault(x => string.Compare(x.Value, autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item.Value == autoProvideValue)
                    return item;

                item = selection.SingleOrDefault(x => string.Compare(x.Key.ToString(), autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item.Key.ToString() == autoProvideValue)
                    return item;
            }
            return null;
        }

        private string QueryParam(string paramName, string autoProvideValue) //, Func<List<KeyValuePair<string, string>>> selectionDelegate)
        {
            if (!string.IsNullOrEmpty(autoProvideValue))
            {
                //NOTE: Check if the ecco flag is set, if so, ecco the entry back to the user
                //OutputLine(string.Format("{0}{1}", paramName, autoProvideValue), null, false);

                return autoProvideValue;
            }

            //Output(string.Format("{0} [TAB]: ", paramName), null, false);
            Output(string.Format("{0}: ", paramName), null, false);

            var left = _console.CursorLeft;
            var tabIndex = -1;
            var input = string.Empty;
            List<KeyValuePair<string, string>> selection = null;

            while (true)
            {
                var key = _console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    _console.NewLine();
                    return tabIndex == -1 || selection == null ? input : selection[tabIndex].Key;
                }
                if (key.KeyChar >= 32 && key.KeyChar <= 126)
                {
                    input += key.KeyChar;  //NOTE: When changing the input, automatically exit tab mode (tabIndex = -1;)
                    tabIndex = -1;
                }
                else
                {
                    //Take special action
                    switch (key.Key)
                    {
                        case ConsoleKey.Backspace:
                            if (_console.CursorLeft >= left)
                            {
                                input = input.Substring(0, input.Length - 1);
                                tabIndex = -1;
                                _console.Write(" ");
                                _console.CursorLeft--;
                            }
                            else
                                _console.CursorLeft++;
                            break;
                        //case ConsoleKey.Tab:
                        //    if (selectionDelegate != null)
                        //    {
                        //        if (selection == null)
                        //            selection = selectionDelegate.Invoke();

                        //        if (!selection.Any())
                        //        {
                        //            System.Diagnostics.Debug.WriteLine("There are no selections.");
                        //            _console.CursorLeft--;
                        //            break;
                        //        }

                        //        if (tabIndex >= selection.Count() - 1)
                        //            tabIndex = -1;

                        //        var emptyString = new string(' ', _console.CursorLeft - left);
                        //        _console.CursorLeft = left;
                        //        _console.Write(emptyString);

                        //        _console.CursorLeft = left;
                        //        _console.Write(selection[++tabIndex].Value);
                        //        input = selection[tabIndex].Value;
                        //    }
                        //    else
                        //    {
                        //        System.Diagnostics.Debug.WriteLine("There is no selection. Ignore the tab key.");
                        //        _console.CursorLeft--;
                        //    }
                        //    break;
                        default:
                            System.Diagnostics.Debug.WriteLine(key);
                            _console.CursorLeft--;
                            break;
                    }
                }
            }
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
                        if (args == null)
                            _console.WriteLine(message);
                        else
                            _console.WriteLine(string.Format(message, args));
                    else
                        _console.Write(message, args);
                }
                finally
                {
                    if (color != null)
                        _console.ForegroundColor = defaultColor;
                }
            }
        }


        #endregion

        internal void OutputInformationLine(string message, bool commandMode)
        {
            if (commandMode)
            {
                //By default, do not output information when in command mode
                return;
            }

            Output(message, null, true, null);
        }
    }
}