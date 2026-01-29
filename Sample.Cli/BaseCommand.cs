using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Console;

namespace Sample.Cli;

internal sealed class BaseCommand : CommandGroupBase
{
    public BaseCommand()
        : base("base")
    {        
        AddCommand<MyCommand>();
        AddCommand<MyOtherCommand>();
    }

    public override Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}
