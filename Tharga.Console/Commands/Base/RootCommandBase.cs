using Tharga.Console.Commands.StartupCommands;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.Base;

public abstract class RootCommandWindowsBase : RootCommandBase
{
    protected RootCommandWindowsBase(IConsole console, ICommandResolver commandResolver = null)
        : base(console, commandResolver)
    {
        RegisterCommand(new StartupCommand(console));
    }
}