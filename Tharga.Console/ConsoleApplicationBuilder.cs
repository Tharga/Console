using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Console.Consoles;

namespace Tharga.Console;

public sealed class ConsoleApplicationBuilder
{
    private readonly CommandNode _root = new(string.Empty);
    private readonly Dictionary<Type, CommandNode> _nodesByType = new();
    private int _nextOrder;
    private bool _builtInsAdded;
    private IConsoleInput _input;
    private IConsoleOutput _output;

    internal ConsoleApplicationBuilder(string[] args)
    {
        Args = args;
        Services = new ServiceCollection();
        InputPrompt = "> ";
        var defaultConsole = new DefaultConsole();
        _input = defaultConsole;
        _output = defaultConsole;
    }

    public string[] Args { get; }

    public IServiceCollection Services { get; }

    public string InputPrompt { get; set; }

    public ConsoleApplicationApp Build()
    {
        if (!_builtInsAdded)
        {
            AddConsoleServices();
            using var tempProvider = Services.BuildServiceProvider();
            AddBuiltIns(tempProvider);
            _builtInsAdded = true;
        }

        AddConsoleServices();

        var serviceProvider = Services.BuildServiceProvider();
        return new ConsoleApplicationApp(Args, _root, serviceProvider, InputPrompt, _input, _output);
    }

    public void AddCommand<T>() where T : ICommand
    {
        AddConsoleServices();
        using var provider = Services.BuildServiceProvider();
        AddCommandType(typeof(T), _root, provider);
    }

    public void UseConsole(IConsoleControl console)
    {
        _input = console ?? throw new ArgumentNullException(nameof(console));
        _output = console;
    }

    public void UseInput(IConsoleInput input)
    {
        _input = input ?? throw new ArgumentNullException(nameof(input));
    }

    public void UseOutput(IConsoleOutput output)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
    }

    private void AddCommandType(Type commandType, CommandNode parent, IServiceProvider provider)
    {
        var command = (ICommand)ActivatorUtilities.CreateInstance(provider, commandType);
        var node = parent.GetOrAddChild(command.Name, _nextOrder, out var created);
        if (created)
            _nextOrder++;
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

    private void AddBuiltIns(IServiceProvider provider)
    {
        if (_builtInsAdded)
            return;

        AddCommandType(typeof(ClearCommand), _root, provider);
        AddCommandType(typeof(ExitCommand), _root, provider);
        AddCommandType(typeof(HelpCommand), _root, provider);
        AddAlias("cls", typeof(ClearCommand));
    }

    private void AddConsoleServices()
    {
        Services.AddSingleton(_input);
        Services.AddSingleton(_output);
        Services.AddSingleton<IConsoleOutput>(_output);
        Services.AddSingleton<IConsoleInput>(_input);
    }
}
