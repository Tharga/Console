using System.Collections.Generic;
using Tharga.Console.Commands.Base;
using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Commands.StartupCommands;

internal class StartupCommand : ContainerCommandBase
{
    public static readonly string RegKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

    public StartupCommand(IConsole console)
        : base("startup", null, true)
    {
        if (console is ConsoleBase)
        {
            RegisterCommand(new RegisterCommand());
            RegisterCommand(new UnregisterCommand());
            RegisterCommand(new ShowCommand());
        }
    }

    public override IEnumerable<HelpLine> HelpText
    {
        get { yield return new HelpLine("Commands to manage the startup."); }
    }
}