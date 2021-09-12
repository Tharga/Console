using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Entities
{
    public class ActionConsoleOutput : IActionConsoleOutput
    {
        public ActionConsoleOutput(string message, OutputLevel outputLevel)
        {
            Message = message;
            OutputLevel = outputLevel;
        }

        public string Message { get; }
        public OutputLevel OutputLevel { get; }
    }
}