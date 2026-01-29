using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Console;

public sealed class ConsoleApplicationBuilder
{
    private readonly CommandNode _root = new(string.Empty);
    private readonly Dictionary<Type, CommandNode> _nodesByType = new();

    internal ConsoleApplicationBuilder(string[] args)
    {
        Args = args;
        Services = new ServiceCollection();
        InputPrompt = "> ";
        using var provider = Services.BuildServiceProvider();
        AddCommandType(typeof(ExitCommand), _root, provider);
        AddCommandType(typeof(HelpCommand), _root, provider);
        AddCommandType(typeof(ClearCommand), _root, provider);
        AddAlias("cls", typeof(ClearCommand));
    }

    public string[] Args { get; }

    public IServiceCollection Services { get; }

    public string InputPrompt { get; set; }

    public ConsoleApplicationApp Build()
    {
        var serviceProvider = Services.BuildServiceProvider();
        return new ConsoleApplicationApp(Args, _root, serviceProvider, InputPrompt);
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
        _nodesByType[commandType] = node;

        if (command is not ICommandGroup group)
            return;

        foreach (var childType in group.GetCommandTypes())
        {
            AddCommandType(childType, node, provider);
        }
    }

    private void AddAlias(string alias, Type commandType)
    {
        if (!_nodesByType.TryGetValue(commandType, out var node))
            return;

        _root.AddAlias(alias, node);
    }
}
