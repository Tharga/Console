using System;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Entities
{
    public class WriteTextEventArgs : EventArgs, ITextOutput
    {
        public string Message { get; }
        public OutputLevel OutputLevel { get; }
        public ConsoleColor? TextColor { get; }
        public ConsoleColor? TextBackgroundColor { get; }
        public bool TrunkateSingleLine { get; }
        public bool LineFeed { get; }

        public WriteTextEventArgs(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool lineFeed)
        {
            Message = message;
            OutputLevel = outputLevel;
            TextColor = textColor;
            TextBackgroundColor = textBackgroundColor;
            TrunkateSingleLine = trunkateSingleLine;
            LineFeed = lineFeed;
        }

        public WriteTextEventArgs(string message, OutputLevel outputLevel)
            : this(message, outputLevel, null, null, false, true)
        {
        }
    }
}