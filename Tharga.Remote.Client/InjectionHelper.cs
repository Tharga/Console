using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Tharga.Remote.Client
{
    internal class InjectionHelper
    {
        public static WindsorContainer Register()
        {
            var container = new WindsorContainer();

            container.Register(Component.For<IClient>().ImplementedBy<Client>().LifestyleSingleton());

            container.Register(Classes.FromAssemblyInThisApplication(Assembly.GetAssembly(typeof(Program)))
                .IncludeNonPublicTypes()
                .BasedOn<Toolkit.Console.Interfaces.ICommand>()
                //.Configure(x => System.Diagnostics.Debug.WriteLine($"Registered in IOC: {x.Implementation.Name}"))
                .Configure(x => x.LifeStyle.Is(LifestyleType.Transient)));

            return container;
        }
    }
}