using System.Threading.Tasks;

namespace Tharga.Console;

internal sealed class HelpCommand : CommandBase
{
    public HelpCommand()
        : base("help")
    {
    }

    public override Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}
