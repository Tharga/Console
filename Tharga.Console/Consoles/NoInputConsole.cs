namespace Tharga.Console.Consoles;

public sealed class NoInputConsole : IConsoleInput
{
    public bool CanRead => false;

    public string ReadLine(ConsoleReadContext context, out bool eof)
    {
        eof = true;
        return string.Empty;
    }
}
