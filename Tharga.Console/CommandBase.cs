using System;
using System.Threading.Tasks;
using Tharga.Console.Consoles;

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
        var output = ConsoleContext.CurrentOutput;
        if (output is null)
        {
            System.Console.WriteLine(message);
            return;
        }

        output.WriteLine(message, color);
    }

    protected void ClearOutput()
    {
        ConsoleContext.CurrentOutput?.Clear();
    }
}
