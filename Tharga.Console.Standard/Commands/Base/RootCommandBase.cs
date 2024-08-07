using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tharga.Console.Commands.ScreenCommands;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.Base
{
    public abstract class RootCommandBase : ContainerCommandBase, IRootCommand
    {
        public IConsole Console { get; }
        internal CommandEngine CommandEngine;
        private ICommandResolver _commandResolver;

        public event EventHandler<EventArgs> RequestCloseEvent;
        public event EventHandler<ExceptionOccuredEventArgs> ExceptionOccuredEvent;

        protected RootCommandBase(IConsole console)
            : base("root")
        {
            Console = console ?? throw new ArgumentNullException(nameof(console), "No console provided.");

            RegisterCommand(new ExitCommand(() => { RequestCloseEvent?.Invoke(this, EventArgs.Empty); }));
            RegisterCommand(new ClearCommand());
            RegisterCommand(new ScreenCommand(console));
            RegisterCommand(new CmdCommand());
            RegisterCommand(new PoshCommand());
            RegisterCommand(new ExecuteProcessCommand());
            RegisterCommand(new ExecuteCommand(this));

            WriteEvent += OnOutputEvent;

            console.Attach(this);
        }

        protected RootCommandBase(IConsole console, ICommandResolver commandResolver)
            : this(console)
        {
            _commandResolver = commandResolver;
        }

        protected virtual ICommandResolver BuildCommandResolver()
        {
	        return _commandResolver;
        }

		internal ICommandResolver GetCommandResolver()
		{
			return _commandResolver ?? (_commandResolver = BuildCommandResolver());
		}

		public new void RegisterCommand<T>()
        {
            SubCommandTypes.Add(new Tuple<Type, Type>(typeof(T), null));
        }

        public new void RegisterCommand<T, TContainer>()
            where TContainer : IContainerCommand
        {
            SubCommandTypes.Add(new Tuple<Type, Type>(typeof(T), typeof(TContainer)));
        }

        public new void RegisterCommand(Type type)
        {
            SubCommandTypes.Add(new Tuple<Type, Type>(type, null));
        }

        public void RegisterCommand(Type type, Type containerType)
        {
            SubCommandTypes.Add(new Tuple<Type, Type>(type, containerType));
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

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("Root command."); }
        }

        public string QueryInput()
        {
            var tabTree = Build(SubCommands, null);
            return QueryParam(Constants.Prompt, null, tabTree, false, false, true);
        }

        private IEnumerable<CommandTreeNode<string>> Build(IEnumerable<ICommand> commands, string lead)
        {
            foreach (var command in commands)
            {
                var cc = command as ContainerCommandBase;
                var ac = command as ActionCommandBase;

                CommandTreeNode<string>[] subTree = null;

                if (cc != null)
                {
                    var l = (lead != null ? (lead + " ") : "") + cc.Name;
                    subTree = Build(cc.SubCommands, l).ToArray();
                }
                else if (ac != null)
                {
                    var l = (lead != null ? (lead + " ") : "") + ac.Name;
                    var sub = ac.GetOptionList().ToArray();
                    subTree = Build(sub, l).ToArray();
                }

                yield return new CommandTreeNode<string>(lead != null ? $"{lead} {command.Name}" : command.Name, command.Name, subTree);
            }
        }

        private IEnumerable<CommandTreeNode<string>> Build(IEnumerable<string>[] commands, string lead)
        {
            foreach (var command in commands[0])
            {
                yield return new CommandTreeNode<string>(lead != null ? $"{lead} {command}" : command, command);
            }
        }

        public bool Execute(string entry)
        {
            try
            {
                var command = GetSubCommand(entry, out var subCommand, out var typeRegistration);
                if (command != null)
                {
                    var bc = command as CommandBase;
                    var ac = command as ActionCommandBase;
                    var cc = command as ContainerCommandBase;

                    if (cc == null)
                    {
                        if (!command.CanExecute(out var reason))
                        {
                            OutputWarning(GetCanExecuteFailMessage(reason));
                            return false;
                        }
                    }

                    var param = subCommand.ToInput().ToArray();

                    if (ac != null)
                    {
                        if (typeRegistration)
                        {
                            try
                            {
                                var instance = (ActionCommandBase)GetCommandResolver()?.Resolve(ac.GetType());
                                if (instance != null)
                                {
                                    instance.WriteEvent += OnOutputEvent;
                                    instance.Attach(RootCommand, null);
                                    ac = instance;
                                }
                            }
                            catch (Exception e)
                            {
                                Trace.TraceWarning(e.Message);
                            }
                        }
                        else
                        {
                        }

                        ac.InvokeEx(param);
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
                    OutputWarning($"Unknown command '{entry}'.");
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

        protected internal void Initiate(CommandEngine commandEngine)
        {
            CommandEngine = commandEngine;
            Attach(this, null);

            var console = commandEngine.RootCommand.Console;
            console?.Attach(commandEngine);
        }
    }
}