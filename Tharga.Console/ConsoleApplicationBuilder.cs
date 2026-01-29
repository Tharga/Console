using System;
using System.Collections.Generic;

namespace Tharga.Console;

public sealed class ConsoleApplicationBuilder
{
    private readonly Dictionary<string, Type> _commandTypes =
        new(StringComparer.OrdinalIgnoreCase);

    internal ConsoleApplicationBuilder(string[] args)
    {
        Args = args;
        _commandTypes["exit"] = typeof(ExitCommand);
        _commandTypes["help"] = typeof(HelpCommand);
    }

    public string[] Args { get; }

    public ConsoleApplicationApp Build()
    {
        return new ConsoleApplicationApp(Args, _commandTypes);
    }

    public void AddCommand<T>() where T : ICommand, new()
    {
        var instance = new T();
        _commandTypes[instance.Name] = typeof(T);
    }
}
