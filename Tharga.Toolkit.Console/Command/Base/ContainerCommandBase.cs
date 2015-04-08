using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Exceptions;
using Tharga.Toolkit.Console.Helper;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class ContainerCommandBase : CommandBase
    {
        protected readonly List<CommandBase> SubCommands = new List<CommandBase>();

        protected ContainerCommandBase(string name)
            : base(null, name, string.Format("Command that manages {0}.", name))
        {
        }

        internal ContainerCommandBase(IConsole console, string name)
            : base(console, name, string.Format("Command that manages {0}.", name))
        {
        }

        protected virtual IEnumerable<string> CommandKeys
        {
            get
            {
                foreach (var sub in SubCommands)
                {
                    foreach (var name in sub.Names)
                    {
                        if (this is RootCommand) yield return "help";

                        yield return name;

                        var subContainer = sub as ContainerCommandBase;
                        if (subContainer == null) continue;
                        yield return name + " help";
                        var commandKeys = subContainer.CommandKeys;
                        foreach (var key in commandKeys) yield return name + " " + key;
                    }
                }
            }
        }

        public ContainerCommandBase RegisterCommand(CommandBase command)
        {
            if (command.Names.Any(x => GetCommand(x) != null)) throw new CommandAlreadyRegisteredException(command.Name, Name);

            SubCommands.Add(command);
            command.CommandRegistered(Console);
            return this;
        }

        public override void CommandRegistered(IConsole console)
        {
            base.CommandRegistered(console);
            SubCommands.ForEach(x => x.CommandRegistered(Console));
        }

        public void UnregisterCommand(string commandName)
        {
            SubCommands.RemoveAll(x => string.Compare(x.Name, commandName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        public CommandBase GetCommand(string commandName)
        {
            return SubCommands.FirstOrDefault(x => string.Compare(x.Name, commandName, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        protected override CommandBase GetHelpCommand()
        {
            if (HelpCommand == null)
            {
                HelpCommand = new HelpCommand(Console);
                HelpCommand.AddLine(string.Format("Help for command {0}.", Name));
                HelpCommand.AddLine(string.Empty);

                HelpCommand.AddLine(string.Format("Sub Commands for {0}:", Name));
                foreach (var command in SubCommands) HelpCommand.AddLine(string.Format("{0} {1}", command.Name.PadString(10), command.Description), command.CanExecute);
            }

            return HelpCommand;
        }

        public override bool CanExecute()
        {
            return true;
        }

        protected internal CommandBase GetSubCommand(string entry, out string subCommand)
        {
            subCommand = null;

            if (string.Compare("help", entry, StringComparison.CurrentCultureIgnoreCase) == 0) return GetHelpCommand();

            if (string.IsNullOrEmpty(entry)) return this;

            var arr = entry.Split(' ');
            if (arr.Length > 1) subCommand = entry.Substring(entry.IndexOf(' ') + 1);

            var name = arr[0].ToLower();

            //Look for a command registered in current list
            var command = SubCommands.FirstOrDefault(y => y.Names.Any(x => string.Compare(x, name, StringComparison.InvariantCultureIgnoreCase) == 0));
            if (command == null) return null;

            if (!(command is ContainerCommandBase)) return command;

            //If there is a command, take the next parameter and look for a sub-command
            string nextSub;
            var x1 = ((ContainerCommandBase)command).GetSubCommand(subCommand, out nextSub);
            if (x1 == null) return command; //If there is no sub-command, return the command found
            subCommand = nextSub;
            return x1;
        }

        public override async Task<bool> InvokeWithCanExecuteCheckAsync(string paramList)
        {
            return await InvokeAsync(paramList);
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            if (string.IsNullOrEmpty(paramList)) return await GetHelpCommand().InvokeAsync(paramList);

            OutputWarning(string.Format("Unknown sub command {0}, for {1}.", paramList, Name));
            return false;
        }
    }
}