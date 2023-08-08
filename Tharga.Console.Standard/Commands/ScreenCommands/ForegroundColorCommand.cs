using System;
using System.Collections.Generic;
using System.Linq;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.ScreenCommands
{
    internal class ForegroundColorCommand : ActionCommandBase
    {
        private readonly IConsoleManager _consoleManager;

        public ForegroundColorCommand(IConsoleManager consoleManager)
            : base("foreground", "Sets the foreground color.")
        {
            _consoleManager = consoleManager;
            AddName("fg");
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Theese are the colors that can be used as foreground.");
                yield return new HelpLine("(Depending on the background, some colors will change to be visible.)");
                yield return new HelpLine(string.Empty);

                foreach (var color in EnumExtensions.GetValues<ConsoleColor>())
                {
                    yield return new HelpLine($"{color}", color);
                }
            }
        }

        public override void Invoke(string[] param)
        {
            var color = QueryParam("Color", param, EnumExtensions.GetValues<ConsoleColor>().ToDictionary(x => x, x => x.ToString()));

            _consoleManager.ForegroundColor = color;
            _consoleManager.Clear();
        }
    }
}