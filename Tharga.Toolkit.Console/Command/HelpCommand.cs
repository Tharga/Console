using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    public class HelpCommand : ActionCommandBase
    {
        private readonly List<HelpLine> _helpLines = new List<HelpLine>();

        internal HelpCommand(IConsole console)
            : base(console, "help", "Displays helpt text")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            foreach (var helpLine in _helpLines)
            {
                Output(helpLine.Text, helpLine.CanExecute() ? ConsoleColor.Gray : ConsoleColor.DarkGray, true, null);
            }

            return true;
        }

        internal void AddLine(string text, Func<bool> canExecute = null)
        {
            _helpLines.Add(new HelpLine(text, canExecute ?? (() => true)));
        }
    }
}