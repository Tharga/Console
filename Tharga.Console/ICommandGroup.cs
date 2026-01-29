using System.Collections.Generic;

namespace Tharga.Console;

public interface ICommandGroup
{
    IEnumerable<ICommand> GetCommands();
}
