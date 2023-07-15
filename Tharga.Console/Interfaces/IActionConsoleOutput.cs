using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IActionConsoleOutput
    {
        string Message { get; }
        OutputLevel OutputLevel { get; }
    }
}