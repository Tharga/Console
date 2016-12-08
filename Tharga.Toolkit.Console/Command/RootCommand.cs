using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    public sealed class RootCommand : RootCommandBase
    {
        public RootCommand(IConsole console)
            : this(console, null)
        {
        }

        public RootCommand(IConsole console, Action stopAction)
            : base(console, stopAction)
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("This is a sample program for the Tharga console project.", ConsoleColor.DarkMagenta);
                yield return new HelpLine("Visit the github page https://github.com/poxet/tharga-console for more information.", ConsoleColor.DarkMagenta);
            }
        }
    }
}