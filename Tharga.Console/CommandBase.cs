using System;
using System.Threading.Tasks;

namespace Tharga.Console;

public abstract class CommandBase : ICommand
{
    protected CommandBase(string name, string description = "")
    {
        Name = name;
        Description = description ?? string.Empty;
    }

    public string Name { get; }
    public string Description { get; }

    public abstract Task ExecuteAsync();

    protected void Output(string message)
    {
        WriteColored(message, null);
    }

    protected void OutputInfo(string message)
    {
        WriteColored(message, ConsoleColor.Green);
    }

    protected void OutputWarning(string message)
    {
        WriteColored(message, ConsoleColor.Yellow);
    }

    protected void OutputError(string message)
    {
        WriteColored(message, ConsoleColor.Red);
    }

    private static void WriteColored(string message, ConsoleColor? color)
    {
        var previous = System.Console.ForegroundColor;
        if (color.HasValue)
            System.Console.ForegroundColor = color.Value;

        System.Console.WriteLine(message);

        System.Console.ForegroundColor = previous;
    }
}
