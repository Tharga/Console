using System;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Entities
{
    public class WriteEventArgs : EventArgs, IOutput
    {
        public WriteEventArgs(string message, OutputLevel outputLevel = OutputLevel.Default, ConsoleColor? textColor = null, ConsoleColor? textBackgroundColor = null, bool trunkateSingleLine = false, bool lineFeed = true, string tag = null)
        {
            Message = message;
            OutputLevel = outputLevel;
            TextColor = textColor;
            TextBackgroundColor = textBackgroundColor;
            TrunkateSingleLine = trunkateSingleLine;
            LineFeed = lineFeed;
            Tag = tag;
        }

        public string Message { get; }
        public OutputLevel OutputLevel { get; }
        public ConsoleColor? TextColor { get; }
        public ConsoleColor? TextBackgroundColor { get; }
        public bool TrunkateSingleLine { get; }
        public bool LineFeed { get; }
        public string Tag { get; }
    }
}