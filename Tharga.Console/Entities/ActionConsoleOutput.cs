using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Entities
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