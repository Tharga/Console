using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IRootCommand : ICommandBase
    {
        void SetStopAction(Action stop);
        IConsole Console { get; }
        //void Initiate();
        bool Execute(string entry);
    }

    public interface ICommandBase : ICommand
    {
        string QueryRootParam();
    }

    public interface ICommand
    {
        string Name { get; }
        IEnumerable<string> Names { get; }
        string Description { get; }
        bool CanExecute(out string reasonMessage);
        IEnumerable<HelpLine> HelpText { get; }
        bool Hidden { get; }
    }
}