using Tharga.Console.Consoles.Base;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Tests
{
    public class TestConsole : ConsoleBase
    {
        public TestConsole(IConsoleManager consoleManager)
            : base(consoleManager)
        {
        }
    }
}