using System;
using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Consoles
{
    public class ActionConsole : ConsoleBase
    {
        private readonly Action<IActionConsoleOutput> _action;

        public ActionConsole(Action<IActionConsoleOutput> action, IConsoleConfiguration consoleConfiguration = null)
            : base(new ConsoleManager(System.Console.Out, System.Console.In))
        {
            _action = action;
        }

        public override void Output(IOutput output)
        {
            if (output == null) throw new ArgumentNullException(nameof(output), "No output parameter provided.");
            _action(new ActionConsoleOutput(output.Message, output.OutputLevel));
        }

        //public override ConsoleKeyInfo ReadKey()
        //{
        //    return base.ReadKey();
        //}

        protected internal override Location WriteLineEx(string value, OutputLevel level)
        {
            _action(new ActionConsoleOutput(value, level));
            return new Location(0, 0);
        }
    }
}