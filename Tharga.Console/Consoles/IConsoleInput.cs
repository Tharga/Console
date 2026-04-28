namespace Tharga.Console.Consoles;

public interface IConsoleInput
{
    bool CanRead { get; }
    string ReadLine(ConsoleReadContext context, out bool eof);
}
