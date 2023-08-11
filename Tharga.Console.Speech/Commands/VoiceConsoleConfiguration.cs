using Tharga.Console.Entities;

namespace Tharga.Console.Commands;

public class VoiceConsoleConfiguration : ConsoleConfiguration
{
    public bool OnlyActiveWhenInFocus { get; set; }
    public bool UseOutputSound { get; set; } = true;
}