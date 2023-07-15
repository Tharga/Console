using System;
using Tharga.Console.Entities;

namespace Tharga.Console.Interfaces
{
    public interface IOutput
    {
        string Message { get; }
        OutputLevel OutputLevel { get; }
        bool TrunkateSingleLine { get; }
        bool LineFeed { get; }
        ConsoleColor? TextColor { get; }
        ConsoleColor? TextBackgroundColor { get; }
        string Tag { get; }
    }
}