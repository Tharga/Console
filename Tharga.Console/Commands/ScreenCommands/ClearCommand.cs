using System.Collections.Generic;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;

namespace Tharga.Console.Commands.ScreenCommands
{
    internal class ClearCommand : ActionCommandBase
    {
        internal ClearCommand()
            : base("cls", "Clears the display.")
        {
            AddName("clear");
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This command cleares all text from the display."); }
        }

        public override void Invoke(string[] param)
        {
            RootCommand.Console.Clear();
        }
    }
}