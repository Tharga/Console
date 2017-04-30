using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    internal class ExecuteCommand : ContainerCommandBase
    {
        public ExecuteCommand(IConsole console, RootCommandBase rootCommand)
            : base(console, new [] { "exec" }, "Execute command features.", true)
        {
            RegisterCommand(new ExecuteFileCommand(console, rootCommand));
            RegisterCommand(new ExecuteSleepCommand(console));
        }
    }
}