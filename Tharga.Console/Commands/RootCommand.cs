using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Tharga.Console.Consoles;

namespace Tharga.Console.Commands;

public class RootCommand : CommandGroup, IRootCommand
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsole _console;
    private readonly ConsoleOptions _options;
    // private readonly Dictionary<string, Type> _commandTypes = new();

    internal RootCommand(IServiceProvider serviceProvider, IConsole console, IOptions<ConsoleOptions> options = default)
        : base(null)
    {
        _serviceProvider = serviceProvider;
        _console = console;
        _options = options?.Value ?? new ConsoleOptions();

        RegisterCommand<ExitCommand>();
        RegisterCommand<HelpCommand>();
    }

    public string QueryInput()
    {
        System.Console.Write(_options.Prompt);
        return System.Console.ReadLine();
    }

    public void Execute(string entry)
    {
        if (string.IsNullOrEmpty(entry)) entry = "help";

        var commandType = _command.GetValueOrDefault(entry);
        if (commandType == null) return;

        if (_serviceProvider.GetService(commandType)is ActionCommandBase command)
        {
            command.Invoke(null);
        }
    }

    // private void RegisterCommand<T>(Dictionary<string, ICommand> commands) where T : ActionCommandBase
    // {
    //     var instance = Activator.CreateInstance(typeof(T)) as ICommand;
    //     if (instance == null) throw new InvalidOperationException($"Cannot create instance of {typeof(T).Name}.");
    //     commands.Add(instance.Name, instance);
    // }
}
