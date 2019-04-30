using System;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Consoles;

namespace SampleFrameworkConsole
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            using (var console = new ClientConsole())
            {
                var command = new RootCommand(console);
                var commandEngine = new CommandEngine(command);
                commandEngine.Start(args);
            }
        }
    }
}