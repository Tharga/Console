using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
            var line = ReadCommandLine(out var eof);
            if (eof)
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

        PrintChildren(OrderChildren(node.Children.Values), 0);
    }

    private static void ShowHelpTree(CommandNode node)
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

    private static void PrintChildren(IReadOnlyList<CommandNode> children, int indent)
    {
        for (var i = 0; i < children.Count; i++)
        {
            var branch = "+- ";
            var prefix = new string(' ', indent * 2);
            System.Console.WriteLine(prefix + branch + FormatCommandLine(children[i]));
        }
    }

    private static void PrintTreeChildren(IReadOnlyList<CommandNode> children, string prefix)
    {
        for (var i = 0; i < children.Count; i++)
        {
            var isLast = i == children.Count - 1;
            var branch = "+- ";
            var linePrefix = prefix + branch;
            System.Console.WriteLine(linePrefix + FormatCommandLine(children[i]));

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
        if (System.Console.IsInputRedirected)
        {
            var line = System.Console.ReadLine();
            eof = line is null;
            return line ?? string.Empty;
        }

        if (_inputPrompt.Length > 0)
            System.Console.Write(_inputPrompt);
        eof = false;

        var buffer = new StringBuilder();
        var lastCandidates = new List<string>();
        var lastBase = string.Empty;
        var lastPrefix = string.Empty;
        var candidateIndex = -1;
        var lastRenderLength = 0;

        while (true)
        {
            var key = System.Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter)
            {
                System.Console.WriteLine();
                return buffer.ToString();
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (buffer.Length > 0)
                {
                    buffer.Length -= 1;
                    ResetCompletionState();
                    RedrawLine(buffer.ToString());
                }
                continue;
            }

            if (key.Key == ConsoleKey.Escape)
            {
                buffer.Clear();
                ResetCompletionState();
                RedrawLine(buffer.ToString());
                continue;
            }

            if (key.Key == ConsoleKey.Tab)
            {
                var direction = key.Modifiers.HasFlag(ConsoleModifiers.Shift) ? -1 : 1;
                var input = buffer.ToString();
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

                var canCycle = lastCandidates.Count > 0 &&
                               candidateIndex >= 0 &&
                               string.Equals(input, lastBase + lastCandidates[candidateIndex],
                                   StringComparison.OrdinalIgnoreCase);

                if (!canCycle)
                {
                    var candidates = GetCandidates(context, prefix);
                    if (candidates.Count == 0)
                        continue;

                    lastCandidates = candidates;
                    lastBase = baseInput;
                    lastPrefix = prefix;
                    candidateIndex = direction > 0 ? 0 : candidates.Count - 1;
                }
                else
                {
                    if (direction > 0)
                        candidateIndex = (candidateIndex + 1) % lastCandidates.Count;
                    else
                        candidateIndex = (candidateIndex - 1 + lastCandidates.Count) % lastCandidates.Count;
                }

                var selection = lastCandidates[candidateIndex];
                buffer.Clear();
                buffer.Append(lastBase);
                buffer.Append(selection);
                RedrawLine(buffer.ToString());
                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                buffer.Append(key.KeyChar);
                ResetCompletionState();
                RedrawLine(buffer.ToString());
            }
        }

        void ResetCompletionState()
        {
            lastCandidates.Clear();
            lastBase = string.Empty;
            lastPrefix = string.Empty;
            candidateIndex = -1;
        }

        void RedrawLine(string text)
        {
            var full = (_inputPrompt ?? string.Empty) + text;
            var clearLen = Math.Max(lastRenderLength, full.Length);

            System.Console.Write("\r");
            if (clearLen > 0)
                System.Console.Write(new string(' ', clearLen));
            System.Console.Write("\r");
            System.Console.Write(full);
            lastRenderLength = full.Length;
        }
    }

    private List<string> GetCandidates(CommandNode context, string prefix)
    {
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

        if (string.IsNullOrWhiteSpace(prefix))
            return names;

        return names
            .Where(x => x.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}
