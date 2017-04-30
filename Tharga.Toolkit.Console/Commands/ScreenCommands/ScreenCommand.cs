using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class ScreenCommand : ContainerCommandBase
    {
        public ScreenCommand(IConsole console)
            : base(console, new[] { "screen", "scr" }, null, true)
        {
            RegisterCommand(new ClearCommand(console));
            RegisterCommand(new BackgroundColorCommand(console));
            RegisterCommand(new ForegroundColorCommand(console));
            RegisterCommand(new MuteCommand(console));
            RegisterCommand(new UnmuteCommand(console));
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("Commands to manage the screen."); } }
    }
}