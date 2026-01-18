using Microsoft.Extensions.DependencyInjection;
using Tharga.Console;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;

public static class ConsoleServiceCollectionExtensions
{
    public static IServiceCollection AddConsole(this IServiceCollection services)
    {
        services.AddSingleton<IConsole, ClientConsole>();
        services.AddTransient<RootCommand>();
        services.AddTransient<SampleCommands>();

        return services;
    }
}
