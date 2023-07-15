using Tharga.Console.Commands.Base;

namespace Tharga.Console.Commands
{
    internal class ExecuteCommand : ContainerCommandBase
    {
        public ExecuteCommand(RootCommandBase rootCommand)
            : base("exec", "Execute command features.", true)
        {
            RegisterCommand(new ExecuteFileCommand(rootCommand));
            RegisterCommand(new ExecuteSleepCommand());
        }
    }
}