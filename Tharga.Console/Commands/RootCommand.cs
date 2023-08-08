using Tharga.Console.Commands.Base;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands;

public sealed class RootCommandWindows : RootCommandWindowsBase
{
    public RootCommandWindows(IConsole console, ICommandResolver commandResolver = null)
        : base(console, commandResolver)
    {
    }
}