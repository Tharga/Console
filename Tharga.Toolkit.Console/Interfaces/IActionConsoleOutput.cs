using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Consoles
{
    public interface IActionConsoleOutput
    {
        string Message { get; }
        OutputLevel OutputLevel { get; }
    }
}