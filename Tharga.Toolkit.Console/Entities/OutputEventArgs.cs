using System;

namespace Tharga.Toolkit.Console.Entities
{
    public class OutputEventArgs : EventArgs
    {
        public OutputEventArgs(string message, OutputLevel outputLevel)
        {
            Message = message;
            OutputLevel = outputLevel;
        }

        public string Message { get; }
        public OutputLevel OutputLevel { get; }
    }
}