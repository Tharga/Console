using System;

namespace Tharga.Console;

public sealed class ConsoleApplication
{
    public static ConsoleApplicationBuilder CreateBuilder(string[] args)
    {
        return new ConsoleApplicationBuilder(args ?? Array.Empty<string>());
    }
}
