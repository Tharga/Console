using System;
using System.Collections;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class RootCommandBase : ContainerCommandBase
    {
        public class ExceptionOccuredEventArgs : EventArgs
        {
            private readonly Exception _exception;

            public ExceptionOccuredEventArgs(Exception exception)
            {
                _exception = exception;
            }

            public Exception Exception
            {
                get { return _exception; }
            }
        }

        public event EventHandler<ExceptionOccuredEventArgs> ExceptionOccuredEvent;

        protected virtual void InvokeExceptionOccuredEvent(ExceptionOccuredEventArgs e)
        {
            EventHandler<ExceptionOccuredEventArgs> handler = ExceptionOccuredEvent;
            if (handler != null) handler(this, e);
        }

        protected RootCommandBase(IConsole console, Action stopAction)
            : base(console, "root")
        {
            RegisterCommand(new ExitCommand(Console, stopAction));
            RegisterCommand(new ClearCommand(Console));
            RegisterCommand(new ExecuteCommand(Console, this));
        }

        protected internal virtual void SetStopAction(Action stopAction)
        {
            var exitCommand = GetCommand("exit");

            if (exitCommand is ExitCommand)
                ((ExitCommand)GetCommand("exit")).SetStopAction(stopAction);
            else
                throw new ArgumentOutOfRangeException(string.Format("Unknown type for exit command. {0}", exitCommand.GetType()));
        }

        public sealed override async Task<bool> InvokeAsync(string paramList)
        {
            return await GetHelpCommand().InvokeAsync(paramList);
        }

        public bool ExecuteCommand(string entry)
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
                    OutputError("Invalid command {0}", entry);
            }
            catch (SystemException exception)
            {
                InvokeExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                HandleError(exception);
            }
            catch (AggregateException exception)
            {
                InvokeExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                foreach (var exp in exception.InnerExceptions)
                {
                    HandleError(exp);
                }
            }
            catch (Exception exception)
            {
                InvokeExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                HandleError(exception);
                OutputInformation("Terminating application...");
                throw;
            }

            return success;
        }

        private void HandleError(Exception exception)
        {
            OutputError(exception.Message);
            foreach (DictionaryEntry data in exception.Data)
            {
                OutputError("- {0}: {1}", data.Key, data.Value);
            }
        }

        public void Initiate()
        {
            Console.Initiate(CommandKeys);
        }
    }
}