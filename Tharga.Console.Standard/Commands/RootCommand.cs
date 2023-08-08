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
    }
}