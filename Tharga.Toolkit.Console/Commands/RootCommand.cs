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

        //public RootCommand(IConsole console, Action stopAction)
        //    : base(console, new InputManager(console), stopAction)
        //{
        //}
    }
}