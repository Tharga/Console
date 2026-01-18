using Tharga.Console.Commands;

namespace Sample.Cli;

internal class SampleCommands : CommandGroup
{
    public SampleCommands()
        : base("Sample")
    {
        RegisterCommand<OutputCommand>();
        RegisterCommand<InputCommand>();
    }
}