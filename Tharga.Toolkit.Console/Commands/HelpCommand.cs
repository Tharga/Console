using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Commands
{
    internal class HelpCommand : ActionCommandBase
    {
        private readonly List<HelpLine> _helpLines = new List<HelpLine>();

        internal HelpCommand()
            : base(new [] { "help" }, "Displays helpt text.", false)
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            foreach (var helpLine in _helpLines)
            {
                CommandEngine.RootCommand.Console.Output(new WriteTextEventArgs(helpLine.Text, OutputLevel.Help, helpLine.CanExecute() ? helpLine.ForeColor : ConsoleColor.DarkGray, null, false, true));
            }

            return true;
        }

        internal void AddLine(string text, Func<bool> canExecute = null, ConsoleColor foreColor = ConsoleColor.Gray)
        {
            _helpLines.Add(new HelpLine(text, canExecute ?? (() => true), foreColor));
        }
    }
}