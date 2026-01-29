using System;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Console;

public sealed class ConsoleApplicationBuilder
{
    private readonly CommandNode _root = new(string.Empty);

    internal ConsoleApplicationBuilder(string[] args)
    {
        Args = args;
        Services = new ServiceCollection();
        using var provider = Services.BuildServiceProvider();
        AddCommandType(typeof(ExitCommand), _root, provider);
        AddCommandType(typeof(HelpCommand), _root, provider);
        AddCommandType(typeof(ClearCommand), _root, provider);
        AddAlias("cls", typeof(ClearCommand), "Clears the console");
    }

    public string[] Args { get; }

    public IServiceCollection Services { get; }

    public ConsoleApplicationApp Build()
    {
        var serviceProvider = Services.BuildServiceProvider();
        return new ConsoleApplicationApp(Args, _root, serviceProvider);
    }

    public void AddCommand<T>() where T : ICommand
    {
        using var provider = Services.BuildServiceProvider();
        AddCommandType(typeof(T), _root, provider);
    }

    private void AddCommandType(Type commandType, CommandNode parent, IServiceProvider provider)
    {
        var command = (ICommand)ActivatorUtilities.CreateInstance(provider, commandType);
        var node = parent.GetOrAddChild(command.Name);
        node.CommandType = commandType;
        node.Description = command.Description;
        Services.AddTransient(commandType);

        if (command is not ICommandGroup group)
            return;

        foreach (var childType in group.GetCommandTypes())
        {
            AddCommandType(childType, node, provider);
        }
    }

    private void AddAlias(string alias, Type commandType, string description)
    {
        var node = _root.GetOrAddChild(alias);
        node.CommandType = commandType;
        node.Description = description ?? string.Empty;
        Services.AddTransient(commandType);
    }
}
