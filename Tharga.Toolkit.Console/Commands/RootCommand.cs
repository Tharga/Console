using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    public sealed class RootCommand : RootCommandBase
    {
        public RootCommand(IConsole console)
            : base(console)
        {
        }
    }
}