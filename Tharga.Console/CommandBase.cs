using System.Threading.Tasks;

namespace Tharga.Console;

public abstract class CommandBase : ICommand
{
    protected CommandBase(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public abstract Task ExecuteAsync();
}
