using System;
using System.Collections.Generic;
using System.Linq;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class BackgroundColorCommand : ActionCommandBase
    {
        public BackgroundColorCommand()
            : base("background", "Sets the background color.", false)
        {
            AddName("bg");
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Theese are the colors that can be used as background.");
                yield return new HelpLine("(Depending on the background, some colors will change to be visible.)");
                yield return new HelpLine(string.Empty);

                foreach (var color in EnumExtensions.GetValues<ConsoleColor>())
                {
                    yield return new HelpLine($"{color}", color);
                }
            }
        }

        public override void Invoke(params string[] param)
        {
            throw new NotImplementedException("Fire event that changes background on the console.");

            var color = QueryParam("Color", param, EnumExtensions.GetValues<ConsoleColor>().ToDictionary(x => x, x => x.ToString()));

            //System.Console.BackgroundColor = color;
            //System.Console.Clear();

            //return true;
        }
    }
}