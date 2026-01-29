using Tharga.Console;

namespace Sample.Cli;

internal class MyCommand : CommandBase
{
    private readonly IMyService _myService;

    public MyCommand(IMyService myService)
        : base("my")
    {
        _myService = myService;
    }

    public override async Task ExecuteAsync()
    {
        var some = _myService.GetSomeText();
        Console.WriteLine(some);
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

public interface IMyService
{
    string GetSomeText();
}

internal class MyService : IMyService
{
    public string GetSomeText() => Guid.NewGuid().ToString();
}