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
        ClearOutput();
        return Task.CompletedTask;
    }
}
