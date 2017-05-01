using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.ScreenCommands;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class RootCommandBase : ContainerCommandBase, IRootCommand
    {
        public IInteractConsole Console { get; }

        public event EventHandler<EventArgs> RequestCloseEvent;
        public event EventHandler<ExceptionOccuredEventArgs> ExceptionOccuredEvent;

        protected RootCommandBase(IInteractConsole console)
            : base(new[] { "root" })
        {
            if (console == null) throw new ArgumentNullException(nameof(console), "No console provided.");

            Console = console;

            RegisterCommand(new ExitCommand(() => { RequestCloseEvent?.Invoke(this, new EventArgs()); }));
            RegisterCommand(new ClearCommand());
            RegisterCommand(new ScreenCommand());
            RegisterCommand(new CmdCommand());
            RegisterCommand(new PoshCommand());
            RegisterCommand(new ExecuteProcessCommand());
            RegisterCommand(new ExecuteCommand(this));
        }

        public new void RegisterCommand(ICommand command)
        {
            command.WriteEvent += OnOutputEvent;

            var containerCommand = command as IContainerCommand;
            if (containerCommand != null)
            {
                containerCommand.CommandRegisteredEvent += (sender, e) =>
                {
                    e.Command.WriteEvent += OnOutputEvent;
                };
            }

            base.RegisterCommand(command);

            if (containerCommand != null)
            {
                foreach (var c in containerCommand.SubCommands)
                {
                    c.WriteEvent += OnOutputEvent;
                }
            }
        }

        private void OnOutputEvent(object sender, WriteEventArgs e)
        {
            Console.Output(e); 
        }

        protected virtual void OnExceptionOccuredEvent(ExceptionOccuredEventArgs e)
        {
            var handler = ExceptionOccuredEvent;
            handler?.Invoke(this, e);
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("Root command."); } }

        public sealed override async Task<bool> InvokeAsync(string paramList)
        {
            return await ((CommandBase)GetHelpCommand(paramList)).InvokeAsync(paramList);
        }

        public bool Execute(string entry)
        {
            var success = false;

            try
            {
                string subCommand;
                var command = GetSubCommand(entry, out subCommand);
                if (command != null)
                {
                    var task = ((CommandBase)command).InvokeWithCanExecuteCheckAsync(subCommand);
                    task.Wait();
                    success = task.Result;
                }
                else
                {
                    Console.Output(new WriteEventArgs($"Invalid command {entry}.", OutputLevel.Error));
                }
            }
            catch (SystemException exception)
            {
                OnExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                OutputError(exception);
            }
            catch (AggregateException exception)
            {
                OnExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                foreach (var exp in exception.InnerExceptions)
                {
                    OutputError(exp);
                }
            }
            catch (Exception exception)
            {
                OnExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                OutputError(exception);
                OutputWarning("Terminating application...");
                throw;
            }

            return success;
        }
    }
}