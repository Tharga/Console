using System;
using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Command.Base
{
    public interface IConsole : IDisposable
    {
        int CursorLeft { get; set; }
        int BufferWidth { get; set; }
        int CursorTop { get; set; }
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
        string ReadLine();
        ConsoleKeyInfo ReadKey();
        ConsoleKeyInfo ReadKey(bool intercept);
        void NewLine();
        void Write(string value);
        void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor = null);
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
        void SetCursorPosition(int left, int top);
        void Clear();
        void Initiate(IEnumerable<string> commandKeys);
        event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        event EventHandler<KeyReadEventArgs> KeyReadEvent;
        void OutputError(Exception exception);
        void OutputError(string message);
        void OutputWarning(string message);
        void OutputInformation(string message);
        void OutputEvent(string message, OutputLevel outputLevel = OutputLevel.Default);

        //TODO: Try to hide theese from the interface
        void Output(string message, ConsoleColor? color, OutputLevel outputLevel, bool line);
        ConsoleColor? GetConsoleColor(OutputLevel outputLevel);
    }
}