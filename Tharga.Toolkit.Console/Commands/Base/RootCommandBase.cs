using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Commands.ScreenCommands;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class RootCommandBase : ContainerCommandBase, IRootCommand
    {
        public IConsole Console { get; }

        public event EventHandler<EventArgs> RequestCloseEvent;
        public event EventHandler<ExceptionOccuredEventArgs> ExceptionOccuredEvent;

        protected RootCommandBase(IConsole console)
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
            base.RegisterCommand(command);
        }

        protected virtual void OnExceptionOccuredEvent(ExceptionOccuredEventArgs e)
        {
            var handler = ExceptionOccuredEvent;
            handler?.Invoke(this, e);
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("Root command."); } }

        //public virtual void SetStopAction(Action stopAction)
        //{
        //    var exitCommand = GetCommand("exit");

        //    if (exitCommand is ExitCommand)
        //        ((ExitCommand)GetCommand("exit")).SetStopAction(stopAction);
        //    else
        //        throw new ArgumentOutOfRangeException($"Unknown type for exit command. {exitCommand.GetType()}");
        //}

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
                    Console.OutputError($"Invalid command {entry}.");
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

        ////TODO: Make internal?
        //public void Initiate()
        //{
        //    ((SystemConsoleBase)Console).Initiate(CommandKeys);
        //}
    }
}