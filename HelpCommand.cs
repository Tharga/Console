using System;
using System.Collections.Generic;
using System.Linq;
using Tharga.Console.Entities;

namespace Tharga.Console.Commands
{
    internal class HelpCommand : ActionCommandBase
    {
        private readonly ICommandResolver _commandResolver;

        public HelpCommand(ICommandResolver commandResolver)
        {
            _commandResolver = commandResolver;
        }

        public override async Task Invoke(string[] param)
        {
            var commands = _commandResolver.GetRegisteredCommands();
            var rootCommand = BuildCommandTree(commands);

            PrintCommandTree(rootCommand, 0);
        }

        private CommandTreeNode<ICommand> BuildCommandTree(IEnumerable<ICommand> commands)
        {
            var root = new CommandTreeNode<ICommand>();

            foreach (var command in commands)
            {
                var parts = command.GetType().Name.Split('Command');
                if (parts.Length > 1)
                {
                    var current = root;
                    for (int i = 0; i < parts.Length - 1; i++)
                    {
                        var part = parts[i];
                        var node = current.Select(part);
                        if (node == null)
                        {
                            node = new CommandTreeNode<ICommand> { Name = part };
                            current.Add(node);
                        }
                        current = node;
                    }
                    current.Add(new CommandTreeNode<ICommand> { Name = parts[parts.Length - 1], Value = command });
                }
            }

            return root;
        }

        private void PrintCommandTree(CommandTreeNode<ICommand> node, int level)
        {
            if (node == null) return;

            for (int i = 0; i < level; i++)
            {
                Console.Write("  ");
            }
            Console.WriteLine(node.Name);

            foreach (var child in node.Children)
            {
                PrintCommandTree(child.Value, level + 1);
            }
        }
    }
}
