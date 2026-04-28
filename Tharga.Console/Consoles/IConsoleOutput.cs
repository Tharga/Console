using System;

namespace Tharga.Console.Consoles;

public interface IConsoleOutput
{
    bool SupportsColors { get; }
    void Write(string text, ConsoleColor? color = null);
    void WriteLine(string text, ConsoleColor? color = null);
    void Clear();
}
