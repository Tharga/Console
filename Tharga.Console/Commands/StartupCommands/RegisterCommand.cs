using System.Reflection;
using Microsoft.Win32;
using Tharga.Console.Commands.Base;

namespace Tharga.Console.Commands.StartupCommands;

internal class RegisterCommand : ActionCommandBase
{
    public RegisterCommand()
        : base("Register", "Registers the application to startup with windows.")
    {
    }

    public override void Invoke(string[] param)
    {
        var asm = Assembly.GetEntryAssembly();
        var path = asm.Location;
        if (path.EndsWith(".dll")) path = $"dotnet {path}";

        var rkApp = Registry.CurrentUser.OpenSubKey(StartupCommand.RegKey, true);
        rkApp.SetValue(asm.GetName().Name, path);
    }
}