using System;
using System.Collections.Generic;
using System.Linq;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class BackgroundColorCommand : ActionCommandBase
    {
        private readonly IConsoleManager _consoleManager;

        public BackgroundColorCommand(IConsoleManager consoleManager)
            : base("background", "Sets the background color.")
        {
            _consoleManager = consoleManager;
            AddName("bg");
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Theese are the colors that can be used as background.");
                yield return new HelpLine("(Depending on the background, some colors will change to be visible.)");
                yield return new HelpLine(string.Empty);

                foreach (var color in EnumExtensions.GetValues<ConsoleColor>()) yield return new HelpLine($"{color}", color);
            }
        }

        public override void Invoke(string[] param)
        {
            var color = QueryParam("Color", param, EnumExtensions.GetValues<ConsoleColor>().ToDictionary(x => x, x => x.ToString()));

            _consoleManager.BackgroundColor = color;
            _consoleManager.Clear();
        }
    }
}