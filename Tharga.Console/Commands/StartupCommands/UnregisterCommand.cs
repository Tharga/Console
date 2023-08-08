using System;
using System.Reflection;
using Microsoft.Win32;
using Tharga.Console.Commands.Base;
#pragma warning disable CA1416

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
        var name = asm.GetName().Name;
        if (string.IsNullOrEmpty(name)) throw new InvalidOperationException("Cannot find name for assembly");

        var rkApp = Registry.CurrentUser.OpenSubKey(StartupCommand.RegKey, true) ?? throw new NullReferenceException($"Cannot find registry with key '{StartupCommand.RegKey}'.");
        rkApp.DeleteValue(name);
    }
}