using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IContainerCommand : ICommand
    {        
    }

    public interface IActionCommand : ICommand
    {
    }

    public interface IRootCommand : ICommand
    {
        event EventHandler<EventArgs> RequestCloseEvent;

        //void SetStopAction(Action stop);
        IConsole Console { get; }
        //void Initiate();
        bool Execute(string entry);
        string QueryRootParam();
    }

    public interface ICommand
    {
        string Name { get; }
        IEnumerable<string> Names { get; }
        string Description { get; }
        bool CanExecute(out string reasonMessage);
        IEnumerable<HelpLine> HelpText { get; }
        bool IsHidden { get; } //TODO: Change to IsVisible

        Task<bool> InvokeAsync(string paramList);
    }
}