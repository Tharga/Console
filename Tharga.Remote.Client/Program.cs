using Tharga.Remote.Client.ConsoleCommands;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Remote.Client
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var console = new ClientConsole(new ConsoleConfiguration { SplashScreen = Constants.SplashScreen });
            var container = InjectionHelper.Register(console);
            var command = new RootCommand(console, new CommandResolver(type => (ICommand)container.Resolve(type)));

            command.RegisterCommand<RemoteConsoleCommands>();

            var commandEngine = new CommandEngine(command)
            {
                TaskRunners = new[]
                {
                    new TaskRunner(async (_, a) =>
                    {
                        var client = container.Resolve<IClient>();
                        await client.ConnectAsync();
                        a.WaitOne();
                        await client.DisconnectAsync();
                    })
                }
            };
            commandEngine.Start(args);
        }
    }
}