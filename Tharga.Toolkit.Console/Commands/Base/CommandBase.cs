using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class CommandBase : QueryParamBase, ICommand
    {
        private readonly Dictionary<string, string> _names;

        public string Name => _names.Keys.First();
        public IEnumerable<string> Names => _names.Select(x => x.Key);
        public string Description { get; }
        public bool IsHidden { get; }

        public abstract IEnumerable<HelpLine> HelpText { get; }

        public event EventHandler<WriteEventArgs> WriteEvent;

        protected RootCommandBase RootCommand;
        protected int ParamIndex;
        //private QueryInput _queryParamManager;

        internal CommandBase(string name, string description = null, bool hidden = false)
        {
            IsHidden = hidden;
            _names = new Dictionary<string, string> { { name.ToLower(), name } };
            Description = description ?? $"Command that manages {name}.";
        }

        protected override CancellationToken CancellationToken => RootCommand.CommandEngine.CancellationToken;
        protected override IInputManager InputManager => RootCommand.CommandEngine.InputManager;

        public abstract void Invoke(string[] param);

        internal virtual void InvokeEx(string[] param)
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

        protected internal virtual void Attach(RootCommandBase rootCommand, List<Tuple<Type, Type>> subCommandTypes)
        {
            if (rootCommand == null) throw new ArgumentNullException(nameof(rootCommand), "No rootCommand provided");
            if (RootCommand != null) throw new InvalidOperationException("The command is already attached.");

            RootCommand = rootCommand;
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

        protected override string GetNextParam(IEnumerable<string> param)
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

        protected ConsoleKeyInfo QueryKey()
        {
            return RootCommand.Console.ReadKey(RootCommand.CommandEngine.CancellationToken);
        }

        //TODO: Theese are to be exposed in several places, can we use some synergy effect?
        //protected string QueryPassword(string paramName, IEnumerable<string> autoParam, string defaultValue = null)
        //{
        //    return QueryPassword(paramName, GetNextParam(autoParam), defaultValue);
        //}

        //protected string QueryPassword(string paramName, string autoProvideValue = null, string defaultValue = null)
        //{
        //    string value;

        //    if (!string.IsNullOrEmpty(autoProvideValue))
        //    {
        //        value = autoProvideValue;
        //    }
        //    else
        //    {
        //        if (!string.IsNullOrEmpty(defaultValue))
        //        {
        //            value = QueryParam(paramName, (string)null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) }, true);
        //        }
        //        else
        //        {
        //            value = QueryParam(paramName, (string)null, (List<KeyValuePair<string, string>>)null, true);
        //        }
        //    }

        //    return value;
        //}

        //protected T QueryParam<T>(string paramName, IEnumerable<string> autoParam, string defaultValue = null)
        //{
        //    if (defaultValue == null)
        //    {
        //        if (default(T) is Enum)
        //        {
        //            var selection = Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(x => x, x => x.ToString());
        //            return QueryParam<T>(paramName, GetNextParam(autoParam), selection);
        //        }
        //        if (default(T) is bool)
        //        {
        //            var selection = new Dictionary<T, string> { { (T)(object)true, true.ToString() }, { (T)(object)false, false.ToString() } };
        //            return QueryParam<T>(paramName, GetNextParam(autoParam), selection);
        //        }
        //    }

        //    return QueryParam<T>(paramName, GetNextParam(autoParam), defaultValue);
        //}

        //protected T QueryParam<T>(string paramName, string autoProvideValue = null, string defaultValue = null)
        //{
        //    try
        //    {
        //        string value;

        //        if (!string.IsNullOrEmpty(autoProvideValue))
        //        {
        //            value = autoProvideValue;
        //        }
        //        else
        //        {
        //            if (!string.IsNullOrEmpty(defaultValue))
        //            {
        //                value = QueryParam(paramName, (string)null, new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(defaultValue, defaultValue) }, false);
        //            }
        //            else
        //            {
        //                value = QueryParam(paramName, (string)null, (List<KeyValuePair<string, string>>)null);
        //            }
        //        }

        //        var response = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
        //        return response;
        //    }
        //    catch (Exception exception)
        //    {
        //        if (exception.InnerException?.GetType() == typeof(FormatException))
        //        {
        //            throw exception.InnerException;
        //        }
        //        throw;
        //    }
        //}

        //protected T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IDictionary<T, string> selectionDelegate)
        //{
        //    return QueryParam(paramName, GetNextParam(autoParam), selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, false);
        //}

        //protected T QueryParam<T>(string paramName, string autoProvideValue, IDictionary<T, string> selectionDelegate)
        //{
        //    return QueryParam(paramName, autoProvideValue, selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, false);
        //}

        //protected T QueryParam<T>(string paramName, IEnumerable<string> autoParam, IEnumerable<KeyValuePair<T, string>> selectionDelegate, bool passwordEntry = false)
        //{
        //    return QueryParam<T>(paramName, GetNextParam(autoParam), selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, passwordEntry);
        //}

        //protected T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<KeyValuePair<T, string>> selectionDelegate, bool passwordEntry = false)
        //{
        //    //return QueryParamManager.QueryParam<T>(paramName, autoProvideValue, selectionDelegate, passwordEntry);
        //    return QueryParam(paramName, autoProvideValue, selectionDelegate?.Select(x => new CommandTreeNode<T>(x.Key, x.Value)), true, passwordEntry);
        //}

        //internal protected T QueryParam<T>(string paramName, string autoProvideValue, IEnumerable<CommandTreeNode<T>> selection, bool allowEscape, bool passwordEntry)
        //{
        //    return QueryParamManager.QueryParam(paramName, autoProvideValue, selection, allowEscape, passwordEntry);
        //}

        protected void OutputError(Exception exception, bool includeStackTrace = false, string prefix = null)
        {
            OutputError(exception.ToFormattedString(includeStackTrace, prefix));
        }

        protected void OutputError(string message)
        {
            OnWriteEvent(this, new WriteEventArgs(message, OutputLevel.Error));
        }

        protected void OutputWarning(string message)
        {
            OnWriteEvent(this, new WriteEventArgs(message, OutputLevel.Warning));
        }

        protected void OutputInformation(string message)
        {
            OnWriteEvent(this, new WriteEventArgs(message, OutputLevel.Information));
        }

        protected void OutputEvent(string message)
        {
            OnWriteEvent(this, new WriteEventArgs(message, OutputLevel.Event));
        }

        protected void OutputDefault(string message)
        {
            OnWriteEvent(this, new WriteEventArgs(message, OutputLevel.Default));
        }

        protected void OutputHelp(string message)
        {
            OnWriteEvent(this, new WriteEventArgs(message, OutputLevel.Help));
        }

        internal void OnWriteEvent(object sender, WriteEventArgs e)
        {
            if (WriteEvent == null) throw new InvalidOperationException($"Command {Name} has no WriteEvent listener attached.");
            WriteEvent.Invoke(sender, e);
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