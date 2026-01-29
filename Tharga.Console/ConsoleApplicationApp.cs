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
    private readonly string _inputPrompt;

    internal ConsoleApplicationApp(string[] args, CommandNode root, IServiceProvider serviceProvider, string inputPrompt)
    {
        _root = root;
        _serviceProvider = serviceProvider;
        _startupCommands = BuildStartupCommands(args);
        _inputPrompt = inputPrompt ?? string.Empty;
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
            if (_inputPrompt.Length > 0)
                System.Console.Write(_inputPrompt);

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
                if (!current.AliasTargets.TryGetValue(token, out next))
                {
                    matchedAll = false;
                    return current;
                }
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
        ShowHelpTree(_root);
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
        System.Console.WriteLine($"Command: {node.Name}");
        if (!string.IsNullOrWhiteSpace(node.Description))
            System.Console.WriteLine(node.Description);
        System.Console.WriteLine();

        PrintChildren(node.Children.Values.OrderBy(x => x.Name).ToList(), 0);
    }

    private static void ShowHelpTree(CommandNode node)
    {
        PrintTreeChildren(node.Children.Values.OrderBy(x => x.Name).ToList(), string.Empty);
    }

    private static string FormatCommandLine(CommandNode node)
    {
        var text = node.Name;
        if (!string.IsNullOrWhiteSpace(node.Description))
            text += $" - {node.Description}";

        if (node.Aliases.Count > 0)
            text += $" (aliases: {string.Join(", ", node.Aliases.OrderBy(x => x))})";

        return text;
    }

    private static void PrintChildren(IReadOnlyList<CommandNode> children, int indent)
    {
        for (var i = 0; i < children.Count; i++)
        {
            var isLast = i == children.Count - 1;
            var branch = isLast ? "└─ " : "├─ ";
            var prefix = new string(' ', indent * 2);
            System.Console.WriteLine(prefix + branch + FormatCommandLine(children[i]));
        }
    }

    private static void PrintTreeChildren(IReadOnlyList<CommandNode> children, string prefix)
    {
        for (var i = 0; i < children.Count; i++)
        {
            var isLast = i == children.Count - 1;
            var branch = isLast ? "└─ " : "├─ ";
            var linePrefix = prefix + branch;
            System.Console.WriteLine(linePrefix + FormatCommandLine(children[i]));

            var childPrefix = prefix + (isLast ? "   " : "│  ");
            var grandChildren = children[i].Children.Values.OrderBy(x => x.Name).ToList();
            if (grandChildren.Count > 0)
                PrintTreeChildren(grandChildren, childPrefix);
        }
    }
}
