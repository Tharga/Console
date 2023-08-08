using System;
using System.Reflection;
using Microsoft.Win32;
using Tharga.Console.Commands.Base;
#pragma warning disable CA1416

namespace Tharga.Console.Commands.StartupCommands;

internal class ShowCommand : ActionCommandBase
{
    public ShowCommand()
        : base("Show", "Show ir the application is registered for startup.")
    {
    }

    public override void Invoke(string[] param)
    {
        var asm = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Cannot find entry assembly.");

        var rkApp = Registry.CurrentUser.OpenSubKey(StartupCommand.RegKey, true) ?? throw new NullReferenceException($"Cannot find registry with key '{StartupCommand.RegKey}'.");
        var name = asm.GetName().Name;
        var k = rkApp.GetValue(name);
        if (k == null)
        {
            OutputInformation($"No registry for '{name}'.");
        }
        else if ($"{k}".Contains(asm.Location))
        {
            OutputInformation($"Registry contains application path. ({k})");
        }
        else
        {
            OutputWarning($"Registry does not contain application path. ({k})");
        }
    }
}