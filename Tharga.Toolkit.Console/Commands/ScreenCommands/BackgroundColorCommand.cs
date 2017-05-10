using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class BackgroundColorCommand : ActionCommandBase
    {
        public BackgroundColorCommand()
            : base(new [] { "background", "bg"}, "Sets the background color.", false)
        {
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
                    yield return new HelpLine($"{color.ToString()}", color);
                }
            }
        }

        public override void Invoke(params string[] input)
        {
            InvokeAsync(input.ToParamString()).Wait();
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var color = QueryParam("Color", GetParam(paramList, index++), EnumExtensions.GetValues<ConsoleColor>().ToDictionary(x => x, x => x.ToString()));

            throw new NotImplementedException("Fire event that changes background on the console.");
            //System.Console.BackgroundColor = color;
            //System.Console.Clear();

            //return true;
        }
    }
}