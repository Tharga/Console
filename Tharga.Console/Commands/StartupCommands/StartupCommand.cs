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
        : base("startup", null, false)
    {
        if (console is ConsoleBase)
        {
            RegisterCommand(new RegisterCommand());
            RegisterCommand(new UnregisterCommand());
            RegisterCommand(new ShowCommand());
        }
    }

    // Starts the program, or dotnet with the program when it is a dll, with the path quoted because Windows splits the command line at spaces.
    internal static string GetCommandLine(string location)
    {
        return location.EndsWith(".dll") ? $"dotnet \"{location}\"" : $"\"{location}\"";
    }

    public override IEnumerable<HelpLine> HelpText
    {
        get { yield return new HelpLine("Commands to manage the startup."); }
    }
}