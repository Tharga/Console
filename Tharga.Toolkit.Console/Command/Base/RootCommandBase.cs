using System;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class RootCommandBase : ContainerCommandBase
    {
        public class ExceptionOccuredEventArgs : EventArgs
        {
            public ExceptionOccuredEventArgs(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; }
        }

        public event EventHandler<ExceptionOccuredEventArgs> ExceptionOccuredEvent;

        protected virtual void OnExceptionOccuredEvent(ExceptionOccuredEventArgs e)
        {
            var handler = ExceptionOccuredEvent;
            handler?.Invoke(this, e);
        }

        protected RootCommandBase(IConsole console, Action stopAction)
            : base(console, "root")
        {
            RegisterCommand(new ExitCommand(Console, stopAction));
            RegisterCommand(new ClearCommand(Console));
            RegisterCommand(new ExecuteCommand(Console, this));
        }

        public virtual void SetStopAction(Action stopAction)
        {
            var exitCommand = GetCommand("exit");

            if (exitCommand is ExitCommand)
                ((ExitCommand)GetCommand("exit")).SetStopAction(stopAction);
            else
                throw new ArgumentOutOfRangeException($"Unknown type for exit command. {exitCommand.GetType()}");
        }

        public sealed override async Task<bool> InvokeAsync(string paramList)
        {
            return await GetHelpCommand(paramList).InvokeAsync(paramList);
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
                    var task = command.InvokeWithCanExecuteCheckAsync(subCommand);
                    task.Wait();
                    success = task.Result;
                }
                else
                    OutputError("Invalid command {0}.", entry);
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
                OutputInformation("Terminating application...");
                throw;
            }

            return success;
        }

        internal void Initiate()
        {
            Console.Initiate(CommandKeys);
        }
    }
}