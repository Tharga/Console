using Tharga.Console;

namespace Sample.Cli;

internal sealed class BaseCommand : CommandGroupBase
{
    public BaseCommand()
        : base("xxx")
    {        
        AddCommand<MyCommand>();
        AddCommand<MyOtherCommand>();
    }

    public override Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}
