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
        protected int ParamIndex;

        protected RootCommandBase RootCommand;

        internal CommandBase(string name, string description = null, bool hidden = false)
        {
            IsHidden = hidden;
            _names = new Dictionary<string, string> { { name.ToLower(), name } };
            Description = description ?? $"Command that manages {name}.";
        }

        protected override CancellationToken CancellationToken => RootCommand.CommandEngine.CancellationToken;
        protected override IInputManager InputManager => RootCommand.CommandEngine.InputManager;

        public string Name => _names.Keys.First();
        public IEnumerable<string> Names => _names.Select(x => x.Key);
        public string Description { get; }
        public bool IsHidden { get; }

        public abstract IEnumerable<HelpLine> HelpText { get; }

        public event EventHandler<WriteEventArgs> WriteEvent;

        public abstract void Invoke(string[] param);

        public virtual bool CanExecute(out string reasonMesage)
        {
            reasonMesage = string.Empty;
            return true;
        }

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

        protected void Output(string message, OutputLevel level)
        {
            OnWriteEvent(this, new WriteEventArgs(message, level));
        }

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
            OnWriteEvent(this, new WriteEventArgs(message));
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