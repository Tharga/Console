using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Tests
{
    public class TestConsole : ConsoleBase
    {
        public TestConsole(IConsoleManager consoleManager)
            : base(consoleManager)
        {
        }
    }
}