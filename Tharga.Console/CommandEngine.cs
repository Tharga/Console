using System.Threading;
using Tharga.Console.Commands;

namespace Tharga.Console;

public class CommandEngine
{
    private readonly IRootCommand _rootCommand;
    private readonly CancellationTokenSource _cancellationTokenSource;

    public CommandEngine(IRootCommand rootCommand, CancellationToken cancellationToken = default)
    {
        _rootCommand = rootCommand;
        _cancellationTokenSource = new CancellationTokenSource(); //TODO: Use cancellationToken if provided.
    }

    public void Start(string[] args)
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var entry = _rootCommand.QueryInput();
            _rootCommand.Execute(entry);
        }
    }
}