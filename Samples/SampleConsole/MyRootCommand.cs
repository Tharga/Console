using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace SampleConsole
{
    internal class MyRootCommand : RootCommandBase
    {
        public MyRootCommand(IConsole console)
            : base(console, null)
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