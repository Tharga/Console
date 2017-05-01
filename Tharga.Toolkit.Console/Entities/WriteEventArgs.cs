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

        public WriteEventArgs(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool lineFeed)
        {
            Message = message;
            OutputLevel = outputLevel;
            TextColor = textColor;
            TextBackgroundColor = textBackgroundColor;
            TrunkateSingleLine = trunkateSingleLine;
            LineFeed = lineFeed;
        }

        public WriteEventArgs(string message, OutputLevel outputLevel)
            : this(message, outputLevel, null, null, false, true)
        {
        }
    }
}