using Microsoft.Extensions.Options;

namespace Tharga.Console.Consoles;

/// <summary>
/// This is a regular console that can output text to the console as well as execute commands.
/// </summary>
public class ClientConsole : IInputOutputConsole
{
    private readonly ConsoleOptions _consoleConfiguration;

    public ClientConsole(IOptions<ConsoleOptions> consoleConfiguration = default)
    {
        _consoleConfiguration = consoleConfiguration?.Value;
    }
}