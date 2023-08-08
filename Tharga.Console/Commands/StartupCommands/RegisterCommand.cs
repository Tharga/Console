using System;
using System.Reflection;
using Microsoft.Win32;
using Tharga.Console.Commands.Base;
#pragma warning disable CA1416

namespace Tharga.Console.Commands.StartupCommands;

internal class RegisterCommand : ActionCommandBase
{
    public RegisterCommand()
        : base("Register", "Registers the application to startup with windows.")
    {
    }

    public override void Invoke(string[] param)
    {
        var asm = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Cannot find entry assembly.");
        var path = asm.Location;
        if (path.EndsWith(".dll")) path = $"dotnet {path}";

        var rkApp = Registry.CurrentUser.OpenSubKey(StartupCommand.RegKey, true) ?? throw new NullReferenceException($"Cannot find registry with key '{StartupCommand.RegKey}'.");
        rkApp.SetValue(asm.GetName().Name, path);
    }
}