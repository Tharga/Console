using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ActionConsoleOutput : IActionConsoleOutput
    {
        public string Message { get; }
        public OutputLevel OutputLevel { get; }

        public ActionConsoleOutput(string message, OutputLevel outputLevel)
        {
            Message = message;
            OutputLevel = outputLevel;
        }
    }
}