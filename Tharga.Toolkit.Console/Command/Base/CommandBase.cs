using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class CommandBase : ICommandBase
    {
        private static readonly object _syncRoot = new object();

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
            return string.Format("You cannot execute {0} command.", Name);
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
                    value = QueryParam(paramName, null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) });
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
                OutputInformation("Loading data for " + paramName + "...");
                selection = (await selectionDelegate()).ToList();
            }

            var response = QueryParam(paramName, autoProvideValue, selection, true);
            return response;
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, IDictionary<T, string> selectionDelegate)
        {
            return QueryParam(paramName, autoProvideValue, selectionDelegate, true);
        }

        protected T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<KeyValuePair<T, string>> selectionDelegate)
        {
            return QueryParam(paramName, autoProvideValue, selectionDelegate, true);
        }

        private T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<KeyValuePair<T, string>> selectionDelegate, bool allowEscape)
        {
            var selection = new List<KeyValuePair<T, string>>();
            if (selectionDelegate != null)
            {
                selection = selectionDelegate.ToList();
                var q = GetParamByString(autoProvideValue, selection);
                if (q != null)
                {
                    return q.Value.Key;
                }
            }

            var inputManager = new InputManager(_console, this, paramName + (selectionDelegate == null ? "" : " [Tab]"));
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

        public void OutputError(string message, params object[] args)
        {
            OutputLine(message, OutputLevel.Error, args);
        }

        public void OutputWarning(string message, params object[] args)
        {
            OutputLine(message, OutputLevel.Warning, args);
        }

        public void OutputInformation(string message, params object[] args)
        {
            OutputLine(message, GetConsoleColor(OutputLevel.Information), args);
        }

        public void OutputTable(string[][] data, ConsoleColor? color = null)
        {
            var columnLength = GetColumnSizes(data);

            foreach (var line in data)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < line.Length; i++)
                {
                    sb.AppendFormat("{0}{1}", line[i], new string(' ', columnLength[i] - line[i].Length + 1));
                }

                OutputLine(sb.ToString(), color);
            }

            OutputLine("{0} lines.", color, data.Length - 1);
        }

        private static int[] GetColumnSizes(string[][] data)
        {
            var length = new int[data.Count()];
            foreach (var line in data)
            {
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i].Length > length[i])
                    {
                        length[i] = line[i].Length;
                    }
                }
            }

            return length.ToArray();
        }

        public void OutputEvent(string message, params object[] args)
        {
            Output(message, GetConsoleColor("EventColor", ConsoleColor.Cyan), true, args);
        }

        public void OutputLine(string message, OutputLevel outputLevel, params object[] args)
        {
            Output(message, GetConsoleColor(outputLevel), true, args);
        }

        public void OutputLine(string message, ConsoleColor? color, params object[] args)
        {
            Output(message, color, true, args);
        }

        public void Output(string message, OutputLevel outputLevel, bool line, params object[] args)
        {
            Output(message, GetConsoleColor(outputLevel), line, args);
        }

        public void Output(string message, ConsoleColor? color, bool line, params object[] args)
        {
            if (_console == null) throw new InvalidOperationException("No console assigned. The command was probably not registered, use CommandRegistered to do it manually.");

            lock (_syncRoot)
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
                        _console.Write(string.Format(message, args));
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

        private static ConsoleColor? GetConsoleColor(OutputLevel outputLevel)
        {
            switch (outputLevel)
            {
                case OutputLevel.Information:
                    return GetConsoleColor("Information", ConsoleColor.Green);
                case OutputLevel.Warning:
                    return GetConsoleColor("Warning", ConsoleColor.Yellow);
                case OutputLevel.Error:
                    return GetConsoleColor("Error", ConsoleColor.Red);
                default:
                    return null;
            }
        }

        private static ConsoleColor GetConsoleColor(string name, ConsoleColor defaultColor)
        {
            var colorString = ConfigurationManager.AppSettings[name + "Color"];
            ConsoleColor color;
            if (!Enum.TryParse(colorString, out color))
            {
                color = defaultColor;
            }

            return color;
        }
    }
}