using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Commands;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.RemoteConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var console = new ClientConsole(new ConsoleConfiguration { SplashScreen = Constants.SplashScreen });
            var container = GetContainer();
            var command = new RootCommand(console, new CommandResolver(type => (ICommand)container.Resolve(type)));
            var commandEngine = new CommandEngine(command);
            commandEngine.Start(args);
        }

        private static WindsorContainer GetContainer()
        {
            var container = new WindsorContainer();

            container.Register(Classes.FromAssemblyInThisApplication(Assembly.GetAssembly(typeof(Program)))
                .IncludeNonPublicTypes()
                .BasedOn<ICommand>()
                //.Configure(x => System.Diagnostics.Debug.WriteLine($"Registered in IOC: {x.Implementation.Name}"))
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));
            return container;
        }
    }
}