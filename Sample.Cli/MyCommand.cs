using Tharga.Console;

namespace Sample.Cli;

internal class MyCommand : CommandBase
{
    public MyCommand()
        : base("my")
    {
    }

    public override async Task ExecuteAsync()
    {
        Console.WriteLine("Yeee");
    }
}

internal class MyOtherCommand : CommandBase
{
    public MyOtherCommand()
        : base("other")
    {
    }

    public override async Task ExecuteAsync()
    {
        Console.WriteLine("Yeee other");
    }
}