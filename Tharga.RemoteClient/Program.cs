using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.RemoteClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var console = new ClientConsole(new ConsoleConfiguration { SplashScreen = Constants.SplashScreen });
            var container = InjectionHelper.GetContainer();
            var command = new RootCommand(console, new CommandResolver(type => (ICommand)container.Resolve(type)));

            //TODO: List consoles that are connected
            //- Remote console should register with the server
            //- The client should be able to se a list of consoles

            var commandEngine = new CommandEngine(command);
            commandEngine.Start(args);
        }
    }
}