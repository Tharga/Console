using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    internal class HelpCommand : ActionCommandBase
    {
        private readonly IConsole _console;
        private readonly List<HelpLine> _helpLines = new List<HelpLine>();

        internal HelpCommand(IConsole console)
            : base(console, new [] { "help" }, "Displays helpt text.", false)
        {
            _console = console;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            foreach (var helpLine in _helpLines)
            {
                _console.Output(helpLine.Text, OutputLevel.Help, helpLine.CanExecute() ? helpLine.ForeColor : ConsoleColor.DarkGray, null, false, true);
            }

            return true;
        }

        internal void AddLine(string text, Func<bool> canExecute = null, ConsoleColor foreColor = ConsoleColor.Gray)
        {
            _helpLines.Add(new HelpLine(text, canExecute ?? (() => true), foreColor));
        }
    }
}