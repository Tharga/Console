using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface ICommand
    {
        string Name { get; }
        IEnumerable<string> Names { get; }
        string Description { get; }
        IEnumerable<HelpLine> HelpText { get; }
        bool IsHidden { get; } //TODO: Change to IsVisible
        event EventHandler<WriteEventArgs> WriteEvent;
        bool CanExecute(out string reasonMessage);
        void Invoke(string[] param);
    }
}