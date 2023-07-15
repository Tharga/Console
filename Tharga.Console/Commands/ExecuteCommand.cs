using Tharga.Toolkit.Console.Commands.Base;

namespace Tharga.Toolkit.Console.Commands
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