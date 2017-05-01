using System;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface ITextOutput
    {
        string Message { get; }
        OutputLevel OutputLevel { get; }
        bool TrunkateSingleLine { get; }
        bool LineFeed { get; }
        ConsoleColor? TextColor { get; }
        ConsoleColor? TextBackgroundColor { get; }
    }
}