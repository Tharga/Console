using System.IO;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class ClientConsole : SystemConsoleBase
    {
        public ClientConsole()
            : base(System.Console.Out)
        {
        }
        
        protected override void WriteLine(string value)
        {
            _consoleWriter.WriteLine(value);
        }
    }
}