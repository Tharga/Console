using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class ScreenCommand : ContainerCommandBase
    {
        public ScreenCommand()
            : base("screen", null, true)
        {
            AddName("scr");
            RegisterCommand(new ClearCommand());
            RegisterCommand(new BackgroundColorCommand());
            RegisterCommand(new ForegroundColorCommand());
            RegisterCommand(new MuteCommand());
            RegisterCommand(new UnmuteCommand());
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("Commands to manage the screen."); } }
    }
}