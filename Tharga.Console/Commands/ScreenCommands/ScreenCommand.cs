using System.Collections.Generic;
using Tharga.Console.Commands.Base;
using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.ScreenCommands
{
    internal class ScreenCommand : ContainerCommandBase
    {
        public ScreenCommand(IConsole console)
            : base("screen", null, true)
        {
            AddName("scr");
            RegisterCommand(new ClearCommand());

            var consoleBase = console as ConsoleBase;
            if (consoleBase != null)
            {
                RegisterCommand(new ResetScreenCommand(consoleBase));
                RegisterCommand(new InfoScreenCommand(consoleBase));
                RegisterCommand(new SaveScreenCommand(consoleBase));
                RegisterCommand(new BackgroundColorCommand(consoleBase.ConsoleManager));
                RegisterCommand(new ForegroundColorCommand(consoleBase.ConsoleManager));
                //RegisterCommand(new MuteCommand());
                //RegisterCommand(new UnmuteCommand());
            }
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("Commands to manage the screen."); } }
    }
}