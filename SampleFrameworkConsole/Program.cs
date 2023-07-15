using System;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

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