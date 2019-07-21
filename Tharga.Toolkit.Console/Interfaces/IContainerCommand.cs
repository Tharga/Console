using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IContainerCommand : ICommand
    {
        IEnumerable<ICommand> SubCommands { get; }
        event EventHandler<CommandRegisteredEventArgs> CommandRegisteredEvent;
    }
}