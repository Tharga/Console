using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Remote.Client
{
    internal class InjectionHelper
    {
        public static WindsorContainer Register(ClientConsole clientConsole)
        {
            var container = new WindsorContainer();

            container.Register(Component.For<IClient>().ImplementedBy<Client>()
                .DependsOn(Dependency.OnValue("clientConsole", clientConsole))
                .LifestyleSingleton());

            container.Register(Classes.FromAssemblyInThisApplication(Assembly.GetAssembly(typeof(Program)))
                .IncludeNonPublicTypes()
                .BasedOn<ICommand>()
                //.Configure(x => System.Diagnostics.Debug.WriteLine($"Registered in IOC: {x.Implementation.Name}"))
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            return container;
        }
    }
}