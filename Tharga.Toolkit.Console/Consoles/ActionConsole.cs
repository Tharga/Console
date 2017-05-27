using System;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ActionConsole : ConsoleBase
    {
        private readonly Action<IActionConsoleOutput> _action;

        public ActionConsole(IConsoleManager consoleManager, Action<IActionConsoleOutput> action)
            : base(consoleManager)
        {
            _action = action;
        }

        public override void Output(IOutput output)
        {
            _action(new ActionConsoleOutput(output.Message, output.OutputLevel));
        }
    }
}