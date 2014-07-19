namespace Tharga.Toolkit.Console.Command.Base
{
    public class ClientConsole : SystemConsoleBase
    {
        protected override void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}