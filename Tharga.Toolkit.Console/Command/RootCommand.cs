using System;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    public sealed class RootCommand : RootCommandBase
    {
        public RootCommand(IConsole console)
            : this(console, null)
        {
        }

        public RootCommand(IConsole console, Action stopAction)
            : base(console, stopAction)
        {
        }
    }
}