using System;
using System.Collections.Generic;

namespace Tharga.Console;

public interface ICommandGroup
{
    IEnumerable<Type> GetCommandTypes();
}
