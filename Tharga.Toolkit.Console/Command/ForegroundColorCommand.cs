using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ForegroundColorCommand : ActionCommandBase
    {
        public ForegroundColorCommand(IConsole console)
            : base(console, new[] { "foreground", "fg" }, "Sets the foreground color.", false)
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Theese are the colors that can be used as foreground.");
                yield return new HelpLine("(Depending on the background, some colors will change to be visible.)");
                yield return new HelpLine(string.Empty);

                foreach (var color in EnumUtil.GetValues<ConsoleColor>())
                {
                    yield return new HelpLine($"{color.ToString()}", color);
                }
            }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var color = QueryParam("Color", GetParam(paramList, index++), EnumUtil.GetValues<ConsoleColor>().ToDictionary(x => x, x => x.ToString()));

            System.Console.ForegroundColor = color;
            System.Console.Clear();

            return true;
        }
    }
}