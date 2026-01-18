using System;
using System.Threading.Tasks;

namespace Tharga.Console.Commands;

internal class ExitCommand : ActionCommandBase
{
    public ExitCommand()
        : base("exit", "Exit from the console.")
    {
    }

    public override async Task Invoke(string[] param)
    {
        Output("Exiting...");
        Environment.Exit(0);
    }
}
