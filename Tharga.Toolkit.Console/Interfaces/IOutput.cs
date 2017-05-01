using System;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IOutput
    {
        string Message { get; }
        OutputLevel OutputLevel { get; }
        bool TrunkateSingleLine { get; }
        bool LineFeed { get; }
        ConsoleColor? TextColor { get; }
        ConsoleColor? TextBackgroundColor { get; }
    }
}