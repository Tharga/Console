using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class CommandBase
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

            return await InvokeAsync(paramList);
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

        internal T QueryParam<T>(string paramName, string autoProvideValue = null, string defaultValue = null)
        {
            var value = QueryParam(paramName, autoProvideValue, defaultValue);
            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
        }

        //TODO: If this works, move the code to the function above.
        private string QueryParam(string paramName, string autoProvideValue, string defaultValue)
        {
            if (!string.IsNullOrEmpty(autoProvideValue)) return autoProvideValue;

            if (!string.IsNullOrEmpty(defaultValue)) return QueryParam(paramName, null, () => new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) });
            return QueryParam(paramName, null, (Func<List<KeyValuePair<string, string>>>)null);

            ////TODO: Remember the location of the input here.
            //Output(string.Format("{0}{1}", paramName, paramName.Length > 2 ? ": " : string.Empty), null, false);
            //RememberInputPosition();

            //var read = _console.ReadLine();
            //Debug.WriteLine(string.Format("Read: {0}", read));
            //return read;
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, Func<List<KeyValuePair<T, string>>> selectionDelegate)
        {
            if (selectionDelegate != null) throw new NotSupportedException("Enter data with [TAB] alternatives is not yet supported.");

            var inputManager = new InputManager(this, paramName);
            var response = inputManager.ReadLine();
            var result = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(response);
            return result;

            ////Show prompt
            //var prompt = string.Format("{0}{1}", paramName, paramName.Length > 2 ? ": " : string.Empty);
            //if (selectionDelegate != null)
            //    prompt = string.Format("{0} [TAB]: ", paramName);
            //Output(prompt, null, false);

            ////Execute function with alternatives
            //List<KeyValuePair<T, string>> selection = new List<KeyValuePair<T, string>>();
            //if (selectionDelegate != null)
            //{
            //    selection = selectionDelegate.Invoke();

            //    var q = GetParamByString(autoProvideValue, selection);
            //    if (q != null) return q.Value.Key;
            //}

            //var startLocation = new Location(_console.CursorLeft, _console.CursorTop);

            //var tabIndex = -1;
            //InputBuffer = string.Empty;

            //while (true)
            //{
            //    var lastLocation = new Location(_console.CursorLeft, _console.CursorTop);

            //    var key = _console.ReadKey();

            //    if (key.Key == ConsoleKey.Enter)
            //    {
            //        _console.NewLine();
            //        if (selectionDelegate == null)
            //        {
            //            var result = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(InputBuffer);
            //            return result;
            //        }

            //        if (tabIndex == -1 || selection == null)
            //        {
            //            var q2 = GetParamByString(InputBuffer, selection);
            //            if (q2 != null)
            //                return q2.Value.Key;
            //            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(InputBuffer);
            //        }

            //        return selection[tabIndex].Key;
            //    }
            //    if (key.KeyChar >= 32 && key.KeyChar <= 126)
            //    {
            //        InputBuffer += key.KeyChar; //NOTE: When changing the input, automatically exit tab mode (tabIndex = -1;)
            //        tabIndex = -1;
            //    }
            //    else
            //    {
            //        //Take special action
            //        switch (key.Key)
            //        {
            //            case ConsoleKey.Backspace:

            //                if (_console.CursorLeft >= startLocation.Left)
            //                {
            //                    var charToBeRemoved = InputBuffer.Substring(InputBuffer.Length - 1).ToCharArray()[0];

            //                    InputBuffer = InputBuffer.Substring(0, InputBuffer.Length - 1);

            //                    //If this is a special char, the console might react differently.
            //                    switch (charToBeRemoved)
            //                    {
            //                        case (char)9: //Tab

            //                            //_console.Write(" ");
            //                            _console.CursorLeft = _console.CursorLeft - TabSize + 1;
            //                            break;

            //                        default:
            //                            _console.Write(" ");
            //                            _console.CursorLeft--;

            //                            break;
            //                    }

            //                    tabIndex = -1;
            //                }
            //                else
            //                    _console.CursorLeft++;
            //                break;

            //            case ConsoleKey.Tab:

            //                if (!selection.Any())
            //                {
            //                    _console.CursorLeft = lastLocation.Left + TabSize;
            //                    InputBuffer += key.KeyChar;
            //                    break;
            //                }

            //                if (tabIndex >= selection.Count() - 1)
            //                    tabIndex = -1;

            //                var emptyString = new string(' ', _console.CursorLeft - startLocation.Left);
            //                _console.CursorLeft = startLocation.Left;
            //                _console.Write(emptyString);

            //                _console.CursorLeft = startLocation.Left;
            //                _console.Write(selection[++tabIndex].Value);
            //                InputBuffer = selection[tabIndex].Value;
            //                break;

            //            case ConsoleKey.Escape:

            //                Debug.WriteLine("Escape resets the input");
            //                //TODO: Remove the entire input and move the cursor back to input position
            //                //TODO: Reset the tab index to start over
            //                break;

            //            default:

            //                Debug.WriteLine("No support for key: " + key);
            //                _console.CursorLeft--;
            //                break;
            //        }
            //    }
            //}
        }

        private static KeyValuePair<T, string>? GetParamByString<T>(string autoProvideValue, List<KeyValuePair<T, string>> selection)
        {
            if (!string.IsNullOrEmpty(autoProvideValue))
            {
                var item = selection.SingleOrDefault(x => string.Compare(x.Value, autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item.Value == autoProvideValue) return item;

                item = selection.SingleOrDefault(x => string.Compare(x.Key.ToString(), autoProvideValue, StringComparison.InvariantCultureIgnoreCase) == 0);
                if (item.Key.ToString() == autoProvideValue) return item;
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