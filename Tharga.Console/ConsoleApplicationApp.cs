using System;
using System.Collections.Generic;

namespace Tharga.Console;

public sealed class ConsoleApplicationApp
{
    private readonly IReadOnlyDictionary<string, Type> _commandTypes;

    internal ConsoleApplicationApp(string[] args, IReadOnlyDictionary<string, Type> commandTypes)
    {
        _commandTypes = commandTypes;
    }

    public void Run()
    {
        while (true)
        {
            var line = System.Console.ReadLine();
            if (line is null)
                break;

            var input = line.Trim();
            if (input.Length == 0)
                continue;

            if (_commandTypes.TryGetValue(input, out var commandType))
            {
                var command = commandType == typeof(HelpCommand)
                    ? new HelpCommand(_commandTypes)
                    : (ICommand)Activator.CreateInstance(commandType)!;

                if (command is null)
                    continue;

                command.ExecuteAsync().GetAwaiter().GetResult();
                if (command is ExitCommand)
                    break;
                continue;
            }

            System.Console.WriteLine($"Unknown command: {input}");
        }
    }
}
