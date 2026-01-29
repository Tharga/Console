using System;

namespace Tharga.Console;

public sealed class ConsoleApplicationBuilder
{
    private readonly CommandNode _root = new(string.Empty);

    internal ConsoleApplicationBuilder(string[] args)
    {
        Args = args;
        AddCommandInstance(new ExitCommand(), _root);
        AddCommandInstance(new HelpCommand(), _root);
    }

    public string[] Args { get; }

    public ConsoleApplicationApp Build()
    {
        return new ConsoleApplicationApp(Args, _root);
    }

    public void AddCommand<T>() where T : ICommand, new()
    {
        AddCommandInstance(new T(), _root);
    }

    private static void AddCommandInstance(ICommand command, CommandNode parent)
    {
        var node = parent.GetOrAddChild(command.Name);
        node.CommandType = command.GetType();

        if (command is ICommandGroup group)
        {
            foreach (var child in group.GetCommands())
            {
                AddCommandInstance(child, node);
            }
        }
    }
}
