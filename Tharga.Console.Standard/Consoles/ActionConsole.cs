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

        /// <summary>
        /// Creates a console that passes every output to an action.
        /// </summary>
        /// <param name="action">Receives every output.</param>
        /// <param name="consoleConfiguration">
        /// Applied as <see cref="ClientConsole"/> applies it: colours, title, splash screen and assembly info.
        /// The splash screen and assembly info are passed to <paramref name="action"/>. When omitted, nothing is applied.
        /// </param>
        public ActionConsole(Action<IActionConsoleOutput> action, IConsoleConfiguration consoleConfiguration = null)
            : this(new ConsoleManager(System.Console.Out), action, consoleConfiguration)
        {
        }

        internal ActionConsole(IConsoleManager consoleManager, Action<IActionConsoleOutput> action, IConsoleConfiguration consoleConfiguration)
            : base(consoleManager)
        {
            _action = action;
            if (consoleConfiguration != null)
                ApplyConfiguration(consoleConfiguration);
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