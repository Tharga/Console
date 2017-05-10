using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class ClearCommand : ActionCommandBase
    {
        internal ClearCommand()
            : base("cls", "Clears the display.", false)
        {
            AddName("clear");
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This command cleares all text from the display."); }
        }

        public override void Invoke(params string[] param)
        {
            CommandEngine.RootCommand.Console.Clear();
        }
    }
}