using System;

namespace Tharga.Console.Entities
{
    public class OutputEventArgs : EventArgs
    {
        public string Message { get; }
        public OutputLevel OutputLevel { get; }

        public OutputEventArgs(string message, OutputLevel outputLevel)
        {
            Message = message;
            OutputLevel = outputLevel;
        }
    }
}