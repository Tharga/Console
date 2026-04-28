using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tharga.Console.Consoles;

namespace Tharga.Console;

public sealed class ConsoleApplicationApp
{
    private readonly CommandNode _root;
    private readonly IServiceProvider _serviceProvider;
    private readonly List<string> _startupCommands;
    private readonly string _inputPrompt;
    private readonly IConsoleInput _input;
    private readonly IConsoleOutput _output;

    internal ConsoleApplicationApp(string[] args, CommandNode root, IServiceProvider serviceProvider, string inputPrompt, IConsoleInput input, IConsoleOutput output)
    {
        _root = root;
        _serviceProvider = serviceProvider;
        _startupCommands = BuildStartupCommands(args);
        _inputPrompt = inputPrompt ?? string.Empty;
        _input = input;
        _output = output;
    }

    public void Run()
    {
        ShowAppHeader();

        foreach (var command in _startupCommands)
        {
            if (!ExecuteLine(command))
                return;
        }

        if (_input.CanRead)
        {
            while (true)
            {
                var line = ReadCommandLine(out var eof);
                if (eof)
                    break;

                if (!ExecuteLine(line))
                    break;
            }
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
            _output.WriteLine($"Unknown command: {input}");
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
        ConsoleContext.CurrentOutput = _output;
        try
        {
            command.ExecuteAsync().GetAwaiter().GetResult();
        }
        finally
        {
            ConsoleContext.CurrentOutput = null;
        }
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

    private static bool TryResolveChild(CommandNode current, string token, out CommandNode next)
    {
        if (current.Children.TryGetValue(token, out next))
            return true;

        if (current.AliasTargets.TryGetValue(token, out next))
            return true;

        return false;
    }

    private void ShowHelpRoot()
    {
        var appName = Assembly.GetEntryAssembly()?.GetName().Name ?? "your-app";

        _output.WriteLine("You can pass startup commands as application args.");
        _output.WriteLine("Example:");
        _output.WriteLine($"  {appName} \"command one\" \"command two\" exit");
        _output.WriteLine(string.Empty);
        ShowHelpTree(_root);
    }

    private void ShowAppHeader()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        if (entryAssembly is null)
            return;

        var name = entryAssembly.GetName().Name ?? "Application";
        var version = entryAssembly.GetName().Version?.ToString() ?? "unknown";

        _output.WriteLine($"{name} {version}");
    }

    private void ShowHelp(CommandNode node)
    {
        _output.WriteLine($"Command: {node.Name}");
        if (!string.IsNullOrWhiteSpace(node.Description))
            _output.WriteLine(node.Description);
        _output.WriteLine(string.Empty);

        PrintChildren(OrderChildren(node.Children.Values), 0);
    }

    private void ShowHelpTree(CommandNode node)
    {
        PrintTreeChildren(OrderChildren(node.Children.Values), string.Empty);
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

    private void PrintChildren(IReadOnlyList<CommandNode> children, int indent)
    {
        for (var i = 0; i < children.Count; i++)
        {
            var branch = "+- ";
            var prefix = new string(' ', indent * 2);
            _output.WriteLine(prefix + branch + FormatCommandLine(children[i]));
        }
    }

    private void PrintTreeChildren(IReadOnlyList<CommandNode> children, string prefix)
    {
        for (var i = 0; i < children.Count; i++)
        {
            var isLast = i == children.Count - 1;
            var branch = "+- ";
            var linePrefix = prefix + branch;
            _output.WriteLine(linePrefix + FormatCommandLine(children[i]));

            var childPrefix = prefix + (isLast ? "   " : "|  ");
            var grandChildren = OrderChildren(children[i].Children.Values);
            if (grandChildren.Count > 0)
                PrintTreeChildren(grandChildren, childPrefix);
        }
    }

    private static List<CommandNode> OrderChildren(IEnumerable<CommandNode> nodes)
    {
        return nodes
            .OrderBy(x => x.Order)
            .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private string ReadCommandLine(out bool eof)
    {
        var context = new ConsoleReadContext(_inputPrompt, Complete);
        return _input.ReadLine(context, out eof);
    }

    private CompletionResult Complete(string input)
    {
        var endsWithSpace = input.Length > 0 && char.IsWhiteSpace(input[^1]);
        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

        var context = _root;
        if (parts.Count > 0)
        {
            var limit = endsWithSpace ? parts.Count : parts.Count - 1;
            for (var i = 0; i < limit; i++)
            {
                if (!TryResolveChild(context, parts[i], out var next))
                {
                    context = _root;
                    break;
                }

                context = next;
            }
        }

        var prefix = endsWithSpace || parts.Count == 0 ? string.Empty : parts[^1];
        var baseInput = endsWithSpace ? input : input.Substring(0, input.Length - prefix.Length);

        var names = new List<string>();
        foreach (var node in OrderChildren(context.Children.Values))
        {
            names.Add(node.Name);
        }

        foreach (var alias in context.AliasTargets
                     .OrderBy(x => x.Value.Order)
                     .ThenBy(x => x.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (!names.Contains(alias.Key, StringComparer.OrdinalIgnoreCase))
                names.Add(alias.Key);
        }

        if (!string.IsNullOrWhiteSpace(prefix))
        {
            names = names
                .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return new CompletionResult(baseInput, names);
    }
}
