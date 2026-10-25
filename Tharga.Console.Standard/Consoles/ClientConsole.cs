using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Consoles
{
    public class ClientConsole : ConsoleBase
    {
        public ClientConsole(IConsoleConfiguration consoleConfiguration = null)
            : base(new ConsoleManager(System.Console.Out))
        {
            ApplyConfiguration(consoleConfiguration ?? new ConsoleConfiguration());
        }
    }
}