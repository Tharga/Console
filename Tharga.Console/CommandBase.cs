using System.Threading.Tasks;

namespace Tharga.Console;

public abstract class CommandBase : ICommand
{
    protected CommandBase(string name, string description = "")
    {
        Name = name;
        Description = description ?? string.Empty;
    }

    public string Name { get; }
    public string Description { get; }

    public abstract Task ExecuteAsync();
}
