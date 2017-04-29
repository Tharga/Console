using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class CommandBase : ICommandBase
    {
        private readonly string _description;
        private readonly string[] _names;
        private IConsole _console;

        public string Name => _names[0];
        public IEnumerable<string> Names => _names;
        public string Description => _description;
        public IConsole Console => _console;
        public abstract IEnumerable<HelpLine> HelpText { get; }

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

        //TODO: Make internal
        public abstract Task<bool> InvokeAsync(string paramList);
        protected abstract ICommand GetHelpCommand(string paramList);

        public virtual bool CanExecute(out string reasonMesage)
        {
            reasonMesage = string.Empty;
            return true;
        }

        //public virtual bool CanExecute()
        //{
        //    string reasonMesage;
        //    return CanExecute(out reasonMesage);
        //}

        protected virtual string GetCanExecuteFailMessage(string reason)
        {
            return $"You cannot execute {Name} command." + (string.IsNullOrEmpty(reason) ? string.Empty : " " + reason);
        }

        //TODO: Make internal
        public virtual async Task<bool> InvokeWithCanExecuteCheckAsync(string paramList)
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

            var inputManager = new InputManager(_console, paramName + (!selection.Any() ? "" : " [Tab]"), passwordEntry);
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

        protected void OutputError(Exception exception)
        {
            _console.OutputError(exception);
        }

        protected void OutputError(string message, params object[] args)
        {
            _console.OutputError(string.Format(message, args));
        }

        protected void OutputWarning(string message, params object[] args)
        {
            _console.OutputWarning(string.Format(message, args));
        }

        protected void OutputInformation(string message, params object[] args)
        {
            _console.OutputInformation(string.Format(message, args));
        }

        protected void OutputTable(IEnumerable<string> title, IEnumerable<string[]> data, ConsoleColor? color = null)
        {
            var table = new List<string[]> { title.ToArray() };
            table.AddRange(data.Select(item => item.ToArray()));
            OutputTable(table.ToArray(), color);
        }

        protected void OutputTable(string[][] data, ConsoleColor? color = null)
        {
            var columnLength = GetColumnSizes(data);

            foreach (var line in data)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < line.Length; i++)
                {
                    sb.AppendFormat("{0}{1}", line[i], new string(' ', columnLength[i] - line[i].Length + 1));
                }

                OutputLine(sb.ToString(), color, OutputLevel.Information);
            }

            var lineCount = data.Length - 1;
            if (lineCount < 0) lineCount = 0;
            OutputLine("{0} lines.", color, OutputLevel.Information, lineCount);
        }

        private static int[] GetColumnSizes(string[][] data)
        {
            if (data.Length == 0)
                return new int[] {};

            var length = new int[data[0].Length];
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

        [Obsolete("Use OutputEvent from the console.")]
        public void OutputEvent(string message, OutputLevel outputLevel = OutputLevel.Default, params object[] args)
        {
            _console.OutputEvent(string.Format(message, args), outputLevel);
        }

        [Obsolete("Use OutputLine from the console.")]
        public void OutputLine(string message, OutputLevel outputLevel, params object[] args)
        {
            _console.Output(string.Format(message, args), _console.GetConsoleColor(outputLevel), outputLevel, true);
        }

        [Obsolete("Use OutputLine from the console.")]
        public void OutputLine(string message, ConsoleColor? color, OutputLevel outputLevel, params object[] args)
        {
            _console.Output(string.Format(message,args), color ?? _console.GetConsoleColor(outputLevel), outputLevel, true);
        }

        [Obsolete("Use Output from the console.")]
        public void Output(string message, OutputLevel outputLevel, bool line, params object[] args)
        {
            _console.Output(string.Format(message, args), _console.GetConsoleColor(outputLevel), outputLevel, line);
        }

        [Obsolete("Use Output from the console.")]
        public void Output(string message, ConsoleColor? color, OutputLevel outputLevel, bool line, params object[] args)
        {
            _console.Output(FormatMessage(message, args), color, outputLevel, line);
        }

        private static string FormatMessage(string message, object[] args)
        {
            return args == null ? message : string.Format(message, args);
        }

        //TODO: Make internal
        [Obsolete("This method is deprecated and will be removed.")]
        public virtual void AttachConsole(IConsole console)
        {
            _console = console;
        }        
    }
}