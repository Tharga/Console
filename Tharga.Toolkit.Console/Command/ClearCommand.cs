using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ClearCommand : ActionCommandBase
    {
        internal ClearCommand(IConsole console)
            : base(console, new[] { "cls", "clear" }, "Clears the display.", false)
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get { yield return new HelpLine("This command cleares all text from the display."); }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            Console.Clear();
            return true;
        }
    }

    internal class ScreenCommand : ContainerCommandBase
    {
        public ScreenCommand(IConsole console)
            : base(console, new[] { "screen", "scr" }, null, true)
        {
            RegisterCommand(new ClearCommand(console));
            RegisterCommand(new BackgroundColorCommand(console));
            RegisterCommand(new ForegroundColorCommand(console));
        }

        //public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine(""); } }
    }

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

    internal class BackgroundColorCommand : ActionCommandBase
    {
        public BackgroundColorCommand(IConsole console)
            : base(console, new [] { "background", "bg"}, "Sets the background color.", false)
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Theese are the colors that can be used as background.");
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

            System.Console.BackgroundColor = color;
            System.Console.Clear();

            return true;
        }
    }

    internal static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}