using System;
using System.Collections.Generic;

namespace Tharga.Console;

internal sealed class CommandNode
{
    public CommandNode(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public Type CommandType { get; set; }

    public Dictionary<string, CommandNode> Children { get; } =
        new(StringComparer.OrdinalIgnoreCase);

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
}
