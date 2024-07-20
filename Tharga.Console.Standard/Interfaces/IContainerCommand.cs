using System;
using System.Collections.Generic;
using Tharga.Console.Commands;
using Tharga.Console.Entities;

namespace Tharga.Console.Interfaces
{
    public interface IContainerCommand : ICommand
    {
        event EventHandler<CommandRegisteredEventArgs> CommandRegisteredEvent;
        IEnumerable<ICommand> SubCommands { get; }
    }
}