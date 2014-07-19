using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    class ExecuteCommand : ContainerCommandBase
    {
        public ExecuteCommand(IConsole console, RootCommandBase rootCommand)
            : base(console, "exec")
        {
            RegisterCommand(new ExecuteFileCommand(console, rootCommand));
            RegisterCommand(new ExecuteSleepCommand(console));
        }
    }
}