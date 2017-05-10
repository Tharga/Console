using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.ScreenCommands;
using Tharga.Toolkit.Console.Entities;
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

            WriteEvent += OnOutputEvent;
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

        //TODO: What does this method do? Why was it there?
        //public sealed override async Task<bool> InvokeAsync(string paramList)
        //{
        //    return await ((CommandBase)GetHelpCommand(paramList)).InvokeAsync(paramList);
        //}

        public bool Execute(string entry)
        {
            try
            {
                string subCommand;
                var command = GetSubCommand(entry, out subCommand);
                if (command != null)
                {
                    string reason;
                    if (!CanExecute(out reason))
                    {
                        OutputWarning(GetCanExecuteFailMessage(reason));
                        return false;
                    }

                    var param = subCommand.ToInput().ToArray();

                    var ac = command as ActionAsyncCommandBase;
                    var bc = command as CommandBase;
                    if (ac != null)
                    {
                        Task.Run(() => { ac.InvokeAsyncEx(param); }).Wait();
                    }
                    else if (bc != null)
                    {
                        bc.InvokeEx(param);
                    }
                    else
                    {
                        command.Invoke(param);
                    }

                    return true;
                }
                else
                {
                    Console.Output(new WriteEventArgs($"Invalid command {entry}.", OutputLevel.Error));
                }
            }
            catch (CommandFailedException)
            {
                return false;
            }
            catch (CommandEscapeException)
            {
                return false;
            }
            catch (SystemException exception)
            {
                OnExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                OutputError(exception);
            }
            catch (AggregateException exception)
            {
                if (exception.InnerException is CommandEscapeException)
                    return false;

                OnExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                OutputError(exception);
            }
            catch (Exception exception)
            {
                OnExceptionOccuredEvent(new ExceptionOccuredEventArgs(exception));
                OutputError(exception);
                OutputWarning("Terminating application...");
                throw;
            }

            return false;
        }
    }
}