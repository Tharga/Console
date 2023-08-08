using System;
using System.Reflection;
using Microsoft.Win32;
using Tharga.Console.Commands.Base;

namespace Tharga.Console.Commands.StartupCommands;

internal class UnregisterCommand : ActionCommandBase
{
    public UnregisterCommand()
        : base("Unregister", "Unregisters the application, not to startup with windows.")
    {
    }

    public override void Invoke(string[] param)
    {
        var asm = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Cannot find entry assembly.");

        var rkApp = Registry.CurrentUser.OpenSubKey(StartupCommand.RegKey, true);
        rkApp.DeleteValue(asm.GetName().Name);
    }
}