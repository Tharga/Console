using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ClearCommand : ActionCommandBase
    {
        internal ClearCommand(IConsole console)
            : base(console, new[] { "cls", "clear" }, "Clears the display.")
        {
        }

        //public override IEnumerable<HelpTextLine> HelpText
        //{
        //    get { yield return new HelpTextLine("This command cleares all text on the display."); }
        //}

        public override async Task<bool> InvokeAsync(string paramList)
        {
            Console.Clear();
            return true;
        }
    }
}