using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles
{
    public class RemoteConsole : ClientConsole
    {
        public RemoteConsole(IConsoleConfiguration consoleConfiguration = null)
            : base(consoleConfiguration)
        {
        }
    }
}