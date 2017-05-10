using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class ClearCommand : ActionCommandBase
    {
        internal ClearCommand()
            : base(new[] { "cls", "clear" }, "Clears the display.", false)
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This command cleares all text from the display."); }
        }

        public override void Invoke(params string[] input)
        {
            InvokeAsync(input.ToParamString()).Wait();
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            CommandEngine.RootCommand.Console.Clear();
            return true;
        }
    }
}