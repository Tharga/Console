using System;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IRootCommand : IContainerCommand
    {
        event EventHandler<EventArgs> RequestCloseEvent;
        IConsole Console { get; }
        string QueryInput();
        void RegisterCommand(ICommand command);
        bool Execute(string entry);
    }
}