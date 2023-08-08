using System.Runtime.InteropServices;
using Tharga.Console.Commands.StartupCommands;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.Base;

public abstract class RootCommandWindowsBase : RootCommandBase
{
    protected RootCommandWindowsBase(IConsole console, ICommandResolver commandResolver = null)
        : base(console, commandResolver)
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        if (isWindows)
        {
            RegisterCommand(new StartupCommand(console));
        }
    }
}