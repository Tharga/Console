using Tharga.Console.Entities;

namespace Tharga.Console.Interfaces
{
    public interface IActionConsoleOutput
    {
        string Message { get; }
        OutputLevel OutputLevel { get; }
    }
}