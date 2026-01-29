using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Console;

public sealed class ConsoleApplicationApp
{
    private readonly CommandNode _root;
    private readonly IServiceProvider _serviceProvider;

    internal ConsoleApplicationApp(string[] args, CommandNode root, IServiceProvider serviceProvider)
    {
        _root = root;
        _serviceProvider = serviceProvider;
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
            {
                ShowHelp(_root);
                continue;
            }

            var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var node = ResolveNode(tokens, out var matchedAll);

            if (!matchedAll)
            {
                System.Console.WriteLine($"Unknown command: {input}");
                continue;
            }

            if (node.HasChildren)
            {
                ShowHelp(node);
                continue;
            }

            if (node.CommandType == typeof(HelpCommand))
            {
                ShowHelp(_root);
                continue;
            }

            var command = (ICommand)_serviceProvider.GetRequiredService(node.CommandType);
            command.ExecuteAsync().GetAwaiter().GetResult();
            if (command is ExitCommand)
                break;
        }
    }

    private CommandNode ResolveNode(string[] tokens, out bool matchedAll)
    {
        var current = _root;
        foreach (var token in tokens)
        {
            if (!current.Children.TryGetValue(token, out var next))
            {
                matchedAll = false;
                return current;
            }

            current = next;
        }

        matchedAll = true;
        return current;
    }

    private static void ShowHelp(CommandNode node)
    {
        foreach (var name in node.Children.Keys.OrderBy(x => x))
        {
            System.Console.WriteLine(name);
        }
    }
}
