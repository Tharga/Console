using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Tharga.Console;

public sealed class ConsoleApplicationApp
{
    private readonly CommandNode _root;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<string> _startupCommands;

    internal ConsoleApplicationApp(string[] args, CommandNode root, IServiceProvider serviceProvider)
    {
        _root = root;
        _serviceProvider = serviceProvider;
        _startupCommands = BuildStartupCommands(args);
    }

    public void Run()
    {
        ShowAppHeader();

        foreach (var command in _startupCommands)
        {
            if (!ExecuteLine(command))
                return;
        }

        while (true)
        {
            var line = System.Console.ReadLine();
            if (line is null)
                break;

            if (!ExecuteLine(line))
                break;
        }
    }

    private bool ExecuteLine(string line)
    {
        var input = line.Trim();
        if (input.Length == 0)
        {
            ShowHelpRoot();
            return true;
        }

        var tokens = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var node = ResolveNode(tokens, out var matchedAll);

        if (!matchedAll)
        {
            System.Console.WriteLine($"Unknown command: {input}");
            return true;
        }

        if (node.HasChildren)
        {
            ShowHelp(node);
            return true;
        }

        if (node.CommandType == typeof(HelpCommand))
        {
            ShowHelpRoot();
            return true;
        }

        var command = (ICommand)_serviceProvider.GetRequiredService(node.CommandType);
        command.ExecuteAsync().GetAwaiter().GetResult();
        return command is not ExitCommand;
    }

    private static List<string> BuildStartupCommands(string[] args)
    {
        var commands = new List<string>();
        if (args is null || args.Length == 0)
            return commands;

        foreach (var arg in args)
        {
            if (string.IsNullOrWhiteSpace(arg))
                continue;

            if (arg.IndexOfAny(new[] { ';', '\n', '\r' }) >= 0)
            {
                var parts = arg.Split(new[] { ';', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    var trimmed = part.Trim();
                    if (trimmed.Length > 0)
                        commands.Add(trimmed);
                }
            }
            else
            {
                commands.Add(arg.Trim());
            }
        }

        return commands;
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

    private void ShowHelpRoot()
    {
        var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "your-app";

        System.Console.WriteLine("You can pass startup commands as application args.");
        System.Console.WriteLine("Example:");
        System.Console.WriteLine($"  {appName} \"command one\" \"command two\" exit");
        System.Console.WriteLine();
        ShowHelp(_root);
    }

    private static void ShowAppHeader()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is null)
            return;

        var name = entryAssembly.GetName().Name ?? "Application";
        var version = entryAssembly.GetName().Version?.ToString() ?? "unknown";

        System.Console.WriteLine($"{name} {version}");
    }

    private static void ShowHelp(CommandNode node)
    {
        foreach (var name in node.Children.Keys.OrderBy(x => x))
        {
            System.Console.WriteLine(name);
        }
    }
}
