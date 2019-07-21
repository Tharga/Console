using System;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IRootCommand : IContainerCommand
    {
        IConsole Console { get; }
        event EventHandler<EventArgs> RequestCloseEvent;
        string QueryInput();
        void RegisterCommand(ICommand command);
        bool Execute(string entry);
    }
}