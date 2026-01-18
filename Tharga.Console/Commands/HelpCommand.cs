using System;
using System.Collections.Generic;
using System.Linq;
using Tharga.Console.Commands;

namespace Tharga.Console.Commands;

internal class HelpCommand : ActionCommandBase
{
    private readonly Dictionary<string, ICommand> _commands;

    public HelpCommand(Dictionary<string, ICommand> commands)
        : base("help", "Displays help text.")
    {
        _commands = commands;
    }

    public override async Task Invoke(string[] param)
    {
        var rootCommands = _commands.Values
            .Where(cmd => cmd.Name != null && !cmd.Name.Contains(' '))
            .GroupBy(cmd => cmd.Name.Split(' ')[0])
            .ToDictionary(g => g.Key, g => g.ToList());

        foreach (var group in rootCommands.OrderBy(g => g.Key))
        {
            Console.WriteLine($"{group.Key}:");
            foreach (var command in group.Value.OrderBy(c => c.Name))
            {
                Console.WriteLine($"  {command.Name}");
            }
        }
    }
}
