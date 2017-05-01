using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface ICommand
    {
        event EventHandler<WriteTextEventArgs> WriteTextEvent;

        string Name { get; }
        IEnumerable<string> Names { get; }
        string Description { get; }
        bool CanExecute(out string reasonMessage);
        IEnumerable<HelpLine> HelpText { get; }
        bool IsHidden { get; } //TODO: Change to IsVisible

        Task<bool> InvokeAsync(string paramList);
    }
}