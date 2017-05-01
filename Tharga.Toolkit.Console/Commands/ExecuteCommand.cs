using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    internal class ExecuteCommand : ContainerCommandBase
    {
        public ExecuteCommand(RootCommandBase rootCommand)
            : base(new [] { "exec" }, "Execute command features.", true)
        {
            RegisterCommand(new ExecuteFileCommand(rootCommand));
            RegisterCommand(new ExecuteSleepCommand());
        }
    }
}