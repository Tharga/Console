using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    //internal class CommandWrapper : ICommand
    //{
    //    private readonly ICommand _command;

    //    public CommandWrapper(ICommand command)
    //    {
    //        _command = command;
    //    }

    //    public string Name => _command.Name;
    //    public IEnumerable<string> Names => _command.Names;
    //    public string Description => _command.Description;
    //    public bool CanExecute(out string reasonMessage)
    //    {
    //        return _command.CanExecute(out reasonMessage);
    //    }

    //    public IEnumerable<HelpLine> HelpText => _command.HelpText;
    //    public bool IsHidden => _command.IsHidden;
    //    public Task<bool> InvokeAsync(string paramList)
    //    {
    //        return _command.InvokeAsync(paramList);
    //    }
    //}

    public abstract class ContainerCommandBase : CommandBase, IContainerCommand
    {
        private readonly List<ICommand> _subCommands = new List<ICommand>();

        public IEnumerable<ICommand> SubCommands => _subCommands;

        public event EventHandler<CommandRegisteredEventArgs> CommandRegisteredEvent;

        internal ContainerCommandBase(string[] names, string description = null, bool hidden = false)
            : base(names, description, hidden)
        {
        }

        protected ContainerCommandBase(string name, string description = null, bool hidden = false)
            : this(new [] { name }, description, hidden)
        {
        }

        public override IEnumerable<HelpLine> HelpText { get { yield break; } }

        protected virtual IEnumerable<string> CommandKeys
        {
            get
            {
                foreach (var sub in _subCommands)
                {
                    foreach (var name in sub.Names)
                    {
                        var c = this as RootCommand;
                        if (c != null)
                        {
                            yield return "help";
                        }

                        yield return name;

                        var subContainer = sub as ContainerCommandBase;
                        if (subContainer == null) continue;
                        yield return name + " help";
                        var commandKeys = subContainer.CommandKeys;
                        foreach (var key in commandKeys)
                        {
                            yield return name + " " + key;
                        }
                    }
                }
            }
        }

        protected void RegisterCommand(ICommand command)
        {
            if (command.Names.Any(x => GetCommand(x) != null)) throw new CommandAlreadyRegisteredException(command.Name, Name);
            _subCommands.Add(command);
            CommandRegisteredEvent?.Invoke(this, new CommandRegisteredEventArgs(command));
        }

        protected void UnregisterCommand(string commandName)
        {
            _subCommands.RemoveAll(x => string.Compare(x.Name, commandName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        protected ICommand GetCommand(string commandName)
        {
            return _subCommands.FirstOrDefault(x => string.Compare(x.Name, commandName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        protected override ICommand GetHelpCommand(string paramList)
        {
            var helpCommand = new HelpCommand();

            var showHidden = true;
            var command = this as ICommand;
            var subCommand = paramList?.Trim();
            if (paramList != " details")
            {
                showHidden = false;
                command = GetSubCommand(paramList, out subCommand);
            }

            if (command == null)
            {
                helpCommand.AddLine($"No command named {paramList}.", foreColor: ConsoleColor.Red);
                return helpCommand;
            }

            if (command.Name == "root")
            {
                var assembly = Assembly.GetEntryAssembly();
                helpCommand.AddLine($"Application {assembly?.GetName().Name ?? "main"} help.", foreColor: ConsoleColor.DarkCyan);
                helpCommand.AddLine($"Version {assembly?.GetName().Version}");
            }
            else
            {
                helpCommand.AddLine($"Help for command {command.Name}.", foreColor: ConsoleColor.DarkCyan);
                helpCommand.AddLine(command.Description);
            }

            string reasonMessage;
            command.CanExecute(out reasonMessage);

            if (subCommand != null && subCommand.EndsWith("details"))
            {
                if (!string.IsNullOrEmpty(reasonMessage))
                {
                    helpCommand.AddLine(string.Empty);
                    helpCommand.AddLine("This command can currently not be executed.", foreColor: ConsoleColor.Yellow);
                    helpCommand.AddLine(reasonMessage, foreColor: ConsoleColor.Yellow);
                }

                if (command.HelpText.Any())
                {
                    foreach (var helpText in command.HelpText)
                    {
                        helpCommand.AddLine(helpText.Text, foreColor: helpText.ForeColor);
                    }
                }

                if (command.Name == "root")
                {
                    helpCommand.AddLine(string.Empty);
                    helpCommand.AddLine("How to use help.", foreColor: ConsoleColor.DarkCyan);
                    helpCommand.AddLine("Use the parameter -? at the end of any command to get more details.");
                    helpCommand.AddLine("It is also possible to type 'help [command]' to get details.");

                    //helpCommand.AddLine(string.Empty);
                    //helpCommand.AddLine("Application parameters.", foreColor: ConsoleColor.DarkCyan);
                    //helpCommand.AddLine("");

                    //helpCommand.AddLine(string.Empty);
                    //helpCommand.AddLine("More details.", foreColor: ConsoleColor.DarkCyan);
                    //helpCommand.AddLine("Visit https://github.com/poxet/tharga-console.");
                }
            }
            else if (command.HelpText.Any())
            {
                if (command.Name == "root")
                {
                    helpCommand.AddLine($"Type \"help\" for more information.", foreColor: ConsoleColor.DarkYellow);
                }
                else
                {
                    helpCommand.AddLine($"Type \"{command.Name} -?\" for more information.", foreColor: ConsoleColor.DarkYellow);
                }
            }

            var containerCommand = command as ContainerCommandBase;
            if (containerCommand != null)
            {
                ShowSubCommandHelp(containerCommand._subCommands, helpCommand, reasonMessage, showHidden);
            }

            if (command.Names.Count() > 1)
            {
                helpCommand.AddLine(string.Empty);
                helpCommand.AddLine("Alternative names:", foreColor: ConsoleColor.DarkCyan);
                foreach (var name in command.Names)
                {
                    helpCommand.AddLine($"{name}");
                }
            }

            return helpCommand;
        }

        private void ShowSubCommandHelp(IEnumerable<ICommand> subCommands, HelpCommand helpCommand, string parentReasonMesage, bool showHidden)
        {
            var anyHidden = false;
            var arr = subCommands as ICommand[] ?? subCommands.ToArray();

            var actionCommands = arr.Where(x => x is ActionCommandBase).ToArray();
            var containerCommands = arr.Where(x => x is ContainerCommandBase).ToArray();

            var padLength = containerCommands.Max(x => x.Name.Length, 0).Max(actionCommands.Max(x => x.Name.Length, 0));

            if (containerCommands.Any(x => !x.IsHidden || showHidden))
            {
                helpCommand.AddLine(string.Empty);
                helpCommand.AddLine($"Sections for {Name}:", foreColor: ConsoleColor.DarkCyan);
                foreach (var command in containerCommands)
                {
                    if (!command.IsHidden || showHidden)
                    {
                        var hidden = command.IsHidden ? "*" : "";
                        if (command.IsHidden) anyHidden = true;
                        helpCommand.AddLine($"{(hidden + command.Name).PadStringAfter(padLength)} {command.Description}", () =>
                        {
                            string reasonMessage;
                            var canExecute = command.CanExecute(out reasonMessage);
                            if (canExecute && !string.IsNullOrEmpty(parentReasonMesage))
                            {
                                return false;
                            }
                            return canExecute;
                        });
                    }
                }
            }

            if (actionCommands.Any(x => !x.IsHidden || showHidden))
            {
                helpCommand.AddLine(string.Empty);
                helpCommand.AddLine($"Commands for {Name}:", foreColor: ConsoleColor.DarkCyan);
                foreach (var command in actionCommands)
                {
                    if (!command.IsHidden || showHidden)
                    {
                        var hidden = command.IsHidden ? "*" : "";
                        if (command.IsHidden) anyHidden = true;
                        helpCommand.AddLine($"{(hidden + command.Name).PadStringAfter(padLength)} {command.Description}", () =>
                        {
                            string reasonMessage;
                            var canExecute = command.CanExecute(out reasonMessage);
                            if (canExecute && !string.IsNullOrEmpty(parentReasonMesage))
                            {
                                return false;
                            }
                            return canExecute;
                        });
                    }
                }
            }

            if (anyHidden)
            {
                helpCommand.AddLine(string.Empty);
                helpCommand.AddLine($"* = Hidden command");
            }
        }

        public override bool CanExecute(out string reasonMessage)
        {
            string dummy;
            if (!_subCommands.Any(x => x.CanExecute(out dummy)))
            {
                reasonMessage = "There are no executable subcommands.";
                return false;
            }

            return base.CanExecute(out reasonMessage);
        }

        protected internal ICommand GetSubCommand(string entry, out string subCommand)
        {
            subCommand = null;

            if (string.IsNullOrEmpty(entry))
            {
                return this;
            }

            var arr = entry.Split(' ');
            if (arr.Length > 1) subCommand = entry.Substring(entry.IndexOf(' ') + 1);
            var name = arr[0].ToLower();

            if (string.Compare("help", name, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                return GetHelpCommand(subCommand + " details");
            }
            else if (entry.EndsWith("-?") || entry.EndsWith("/?") || entry.EndsWith("--help"))
            {
                entry = entry.Replace("-?", string.Empty);
                entry = entry.Replace("/?", string.Empty);
                entry = entry.Replace("--help", string.Empty);
                return GetHelpCommand(entry.Trim() + " details");
            }

            //Look for a command registered in current list
            var command = _subCommands.FirstOrDefault(y => y.Names.Any(x => string.Compare(x, name, StringComparison.InvariantCultureIgnoreCase) == 0));
            if (command == null) return null;

            if (!(command is ContainerCommandBase)) return command;

            //If there is a command, take the next parameter and look for a sub-command
            string nextSub;
            var x1 = ((ContainerCommandBase)command).GetSubCommand(subCommand, out nextSub);
            if (x1 == null) return command; //If there is no sub-command, return the command found
            subCommand = nextSub;
            if (x1 is ActionCommandBase)
            {
                string reasonMessage;
                var a = x1.CanExecute(out reasonMessage);
                var b = command.CanExecute(out reasonMessage);
                if (a && !b)
                {
                    ((ActionCommandBase)x1).SetCanExecute(() => $"{reasonMessage} Inherited by parent.");
                }
            }
            return x1;
        }

        internal override async Task<bool> InvokeWithCanExecuteCheckAsync(string paramList)
        {
            return await InvokeAsync(paramList);
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            string reasonMessage;
            if (!CanExecute(out reasonMessage))
            {
                OutputWarning(GetCanExecuteFailMessage(reasonMessage));
                await ((CommandBase)GetHelpCommand(paramList)).InvokeAsync(paramList);
                return false;
            }

            if (string.IsNullOrEmpty(paramList))
            {
                return await ((CommandBase)GetHelpCommand(paramList)).InvokeAsync(paramList);
            }

            OutputWarning($"Unknown sub command {paramList}, for {Name}.");
            return false;
        }
    }
}