namespace Tharga.Console.Commands;

public interface IRootCommand : ICommand
{
    string QueryInput();
    void Execute(string entry);
}