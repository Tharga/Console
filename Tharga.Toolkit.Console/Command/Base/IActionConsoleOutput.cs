namespace Tharga.Toolkit.Console.Command.Base
{
    public interface IActionConsoleOutput
    {
        string Value { get; }
        OutputLevel Level { get; }
    }
}