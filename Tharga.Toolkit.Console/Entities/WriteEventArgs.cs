using System;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Entities
{
    public class WriteEventArgs : EventArgs, IOutput
    {
        public string Message { get; }
        public OutputLevel OutputLevel { get; }
        public ConsoleColor? TextColor { get; }
        public ConsoleColor? TextBackgroundColor { get; }
        public bool TrunkateSingleLine { get; }
        public bool LineFeed { get; }

        public WriteEventArgs(string message, OutputLevel outputLevel = OutputLevel.Default, ConsoleColor? textColor = null, ConsoleColor? textBackgroundColor = null, bool trunkateSingleLine = false, bool lineFeed = true)
        {
            Message = message;
            OutputLevel = outputLevel;
            TextColor = textColor;
            TextBackgroundColor = textBackgroundColor;
            TrunkateSingleLine = trunkateSingleLine;
            LineFeed = lineFeed;
        }
    }
}