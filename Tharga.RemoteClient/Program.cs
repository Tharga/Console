using System.Diagnostics;
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
            var container = InjectionHelper.Register();
            var command = new RootCommand(console, new CommandResolver(type => (ICommand)container.Resolve(type)));
            var commandEngine = new CommandEngine(command)
            {
                TaskRunners = new[]
                {
                    new TaskRunner(async (c, a) =>
                    {
                        var client = container.Resolve<IClient>();
                        await client.ConnectAsync();
                        a.WaitOne();
                        //TODO: This is not triggered on the exit command. Fix this issue.
                        Debug.WriteLine("Closing...");
                    })
                }
            };
            commandEngine.Start(args);
        }
    }
}