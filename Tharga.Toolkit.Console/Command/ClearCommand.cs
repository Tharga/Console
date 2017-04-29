using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ClearCommand : ActionCommandBase
    {
        internal ClearCommand(IConsole console)
            : base(console, new[] { "cls", "clear" }, "Clears the display.", false)
        {
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("This command cleares all text from the display."); } }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            Console.Clear();
            return true;
        }
    }
}