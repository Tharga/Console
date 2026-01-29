using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Console;

internal sealed class CommandNode
{
    public CommandNode(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public string Description { get; set; } = string.Empty;

    public Type CommandType { get; set; }

    public Dictionary<string, CommandNode> Children { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, CommandNode> AliasTargets { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public List<string> Aliases { get; } = new();

    public bool HasChildren => Children.Count > 0;

    public CommandNode GetOrAddChild(string name)
    {
        if (!Children.TryGetValue(name, out var node))
        {
            node = new CommandNode(name);
            Children.Add(name, node);
        }

        return node;
    }

    public void AddAlias(string alias, CommandNode target)
    {
        if (string.IsNullOrWhiteSpace(alias))
            return;

        AliasTargets[alias] = target;
        if (!target.Aliases.Any(x => string.Equals(x, alias, StringComparison.OrdinalIgnoreCase)))
            target.Aliases.Add(alias);
    }
}
