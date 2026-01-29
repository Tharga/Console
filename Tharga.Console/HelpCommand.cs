using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tharga.Console;

internal sealed class HelpCommand : CommandBase
{
    private readonly IReadOnlyDictionary<string, Type> _commandTypes;

    public HelpCommand(IReadOnlyDictionary<string, Type> commandTypes)
        : base("help")
    {
        _commandTypes = commandTypes;
    }

    public override Task ExecuteAsync()
    {
        foreach (var name in _commandTypes.Keys.OrderBy(x => x))
        {
            System.Console.WriteLine(name);
        }

        return Task.CompletedTask;
    }
}
