namespace Tharga.Console.Commands;

public abstract class CommandBase : ICommand
{
    protected CommandBase(string name, string description)
    {
        Name = name;
    }

    public string Name { get; }
}