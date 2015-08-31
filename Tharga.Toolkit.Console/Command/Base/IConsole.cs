using System;
using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Command.Base
{
    public interface IConsole
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
        //void Write(string message, object[] args);
        //void WriteLine(string value, OutputLevel level);
        void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor);
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
        void SetCursorPosition(int left, int top);
        void Clear();
        void Initiate(IEnumerable<string> commandKeys);
        event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
    }
}