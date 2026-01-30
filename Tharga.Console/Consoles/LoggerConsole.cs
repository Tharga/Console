using System;
using Microsoft.Extensions.Logging;

namespace Tharga.Console.Consoles;

public sealed class LoggerConsole : IConsoleOutput
{
    private readonly ILogger _logger;

    public LoggerConsole(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool SupportsColors => false;

    public void Write(string text, ConsoleColor? color = null)
    {
        WriteLine(text, color);
    }

    public void WriteLine(string text, ConsoleColor? color = null)
    {
        _logger.LogInformation("{Message}", text);
    }

    public void Clear()
    {
    }
}
