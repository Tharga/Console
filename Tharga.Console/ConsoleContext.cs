using System.Threading;
using Tharga.Console.Consoles;

namespace Tharga.Console;

internal static class ConsoleContext
{
    private static readonly AsyncLocal<IConsoleOutput> _current = new();

    public static IConsoleOutput CurrentOutput
    {
        get => _current.Value;
        set => _current.Value = value;
    }
}
