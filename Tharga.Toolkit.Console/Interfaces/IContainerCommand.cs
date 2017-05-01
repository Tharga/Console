using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IContainerCommand : ICommand
    {
        event EventHandler<CommandRegisteredEventArgs> CommandRegisteredEvent;

        IEnumerable<ICommand> SubCommands { get; }
    }
}