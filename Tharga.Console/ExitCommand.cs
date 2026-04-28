using System.Threading.Tasks;

namespace Tharga.Console;

internal sealed class ExitCommand : CommandBase
{
    public ExitCommand()
        : base("exit")
    {
    }

    public override Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}
