using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    public class HelpCommand : ActionCommandBase
    {
        private readonly List<HelpLine> HelpLines = new List<HelpLine>();

        internal HelpCommand(IConsole console)
            : base(console, "help", "Displays helpt text")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            foreach(var helpLine in HelpLines)
                Output(helpLine.Text, helpLine.CanExecute() ? ConsoleColor.Gray : ConsoleColor.DarkGray, true, null);

            return true;
        }

        internal void AddLine(string text, Func<bool> canExecute = null)
        {
            HelpLines.Add(new HelpLine(text, canExecute ?? (() => true)));
        }
    }
}