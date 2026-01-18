using System;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Console.Commands;
using Tharga.Console.Consoles;
using Tharga.Runtime;

namespace Tharga.Console;

public static class ConsoleServiceCollectionExtensions
{
    public static IServiceCollection AddConsole<TConsole>(this IServiceCollection services, Action<ConsoleOptions> options = default) 
        where TConsole : IConsole
    {
        var o = new ConsoleOptions();
        options?.Invoke(o);

        services.AddSingleton(typeof(IConsole), typeof(TConsole));
        services.AddSingleton<RootCommand>();

        //TODO: Dynamically find all commands
        services.AddTransient<ExitCommand>();
        services.AddTransient<HelpCommand>();

        return services;
    }
}
