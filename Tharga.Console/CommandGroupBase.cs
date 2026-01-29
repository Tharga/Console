using System.Collections.Generic;

namespace Tharga.Console;

public abstract class CommandGroupBase : CommandBase, ICommandGroup
{
    protected CommandGroupBase(string name)
        : base(name)
    {
    }

    public abstract IEnumerable<ICommand> GetCommands();
}
