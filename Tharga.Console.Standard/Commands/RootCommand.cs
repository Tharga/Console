using System;
using Tharga.Console.Commands.Base;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands
{
    public sealed class RootCommand : RootCommandBase
    {
        public RootCommand(IConsole console)
            : base(console)
        {
        }

        public RootCommand(IConsole console, ICommandResolver commandResolver)
            : base(console, commandResolver)
        {
        }

        public RootCommand(IConsole console, Func<Type, ICommand> commandResolver)
	        : base(console, new CommandResolver(commandResolver.Invoke))
        {
        }
	}
}