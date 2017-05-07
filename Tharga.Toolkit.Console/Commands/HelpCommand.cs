using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands
{
    internal class HelpCommand : ActionCommandBase
    {
        private readonly CommandEngine _commandEngine;
        private readonly List<HelpLine> _helpLines = new List<HelpLine>();

        internal HelpCommand(CommandEngine commandEngine)
            : base(new [] { "help" }, "Displays helpt text.", false)
        {
            _commandEngine = commandEngine;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            foreach (var helpLine in _helpLines)
            {
                _commandEngine.RootCommand.Console.Output(new WriteEventArgs(helpLine.Text, OutputLevel.Help, helpLine.CanExecute() ? helpLine.ForeColor : ConsoleColor.DarkGray, null, false, true));
            }

            return true;
        }

        internal void AddLine(string text, Func<bool> canExecute = null, ConsoleColor foreColor = ConsoleColor.Gray)
        {
            _helpLines.Add(new HelpLine(text, canExecute ?? (() => true), foreColor));
        }
    }
}