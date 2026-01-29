using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Console;

namespace Sample.Cli;

internal sealed class BaseCommand : CommandGroupBase
{
    public BaseCommand()
        : base("base")
    {
    }

    public override IEnumerable<ICommand> GetCommands()
    {
        return new ICommand[]
        {
            new MyCommand(),
            new MyOtherCommand()
        };
    }

    public override Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}
