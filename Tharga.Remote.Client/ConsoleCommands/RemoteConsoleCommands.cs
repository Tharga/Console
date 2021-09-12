using Tharga.Toolkit.Console.Commands.Base;

namespace Tharga.Remote.Client.ConsoleCommands
{
    internal class RemoteConsoleCommands : ContainerCommandBase
    {
        public RemoteConsoleCommands()
            : base("Remote")
        {
            RegisterCommand<RemoteConsoleListComman>();
            RegisterCommand<RemoteConsoleSelectComman>();
            RegisterCommand<RemoteConsoleUnselectComman>();
        }
    }
}