using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class ForegroundColorCommand : ActionCommandBase
    {
        public ForegroundColorCommand()
            : base(new[] { "foreground", "fg" }, "Sets the foreground color.", false)
        {
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
                    yield return new HelpLine($"{color.ToString()}", color);
                }
            }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var color = QueryParam("Color", GetParam(paramList, index++), EnumExtensions.GetValues<ConsoleColor>().ToDictionary(x => x, x => x.ToString()));

            throw new NotImplementedException("Fire event that changes the forground in the console.");
            //System.Console.ForegroundColor = color;
            //System.Console.Clear();

            return true;
        }
    }
}