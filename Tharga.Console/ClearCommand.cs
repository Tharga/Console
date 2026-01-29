using System.Threading.Tasks;

namespace Tharga.Console;

internal sealed class ClearCommand : CommandBase
{
    public ClearCommand()
        : base("clear", "Clears the console")
    {
    }

    public override Task ExecuteAsync()
    {
        System.Console.Clear();
        return Task.CompletedTask;
    }
}
