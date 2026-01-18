using System;
using System.Linq;
using System.Threading.Tasks;

namespace Tharga.Console.Commands;

internal class HelpCommand : ActionCommandBase
{
    private RootCommand _rootCommand;

    public HelpCommand(RootCommand rootCommand)
        : base("help", "Displays help text.")
    {
        _rootCommand = rootCommand;
    }

    public override Task Invoke(string[] param)
    {
        var commands = _rootCommand.GetSubCommands();
        foreach (var command in commands.OrderBy(g => g.Key))
        {
            System.Console.WriteLine($"{command.Key}:");
            //TODO: Fetch sub-commandsl
        }

        return Task.CompletedTask;
    }
}
