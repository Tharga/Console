namespace Tharga.Console.Consoles;

public record VoiceConsoleConfiguration : ConsoleOptions
{
    public bool OnlyActiveWhenInFocus { get; set; }
}