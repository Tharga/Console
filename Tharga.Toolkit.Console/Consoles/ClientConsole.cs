using Tharga.Toolkit.Console.Commands.Base;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ClientConsole : SystemConsoleBase
    {
        public ClientConsole()
            : base(System.Console.Out)
        {
        }
    }
}