using Microsoft.Extensions.DependencyInjection;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

namespace Tharga.Console;

public static class ConsoleServiceCollectionExtensions
{
    public static IServiceCollection AddConsole(this IServiceCollection services)
    {
        services.AddSingleton<IClientConsole, ClientConsole>();
        services.AddTransient<RootCommand>();

        return services;
    }
}
