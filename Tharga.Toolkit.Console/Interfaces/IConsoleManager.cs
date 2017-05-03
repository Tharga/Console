using System;
using System.Text;

namespace Tharga.Toolkit.Console.Consoles.Base
{
    interface IConsoleManager : IDisposable
    {
        Encoding Encoding { get; }
        int CursorLeft { get; }
        int CursorTop { get; }
        int BufferWidth { get; }
        void WriteLine(string value);
        void Write(string value);
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
        int BufferHeight { get; }
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
        void SetCursorPosition(int left, int top);
        void Clear();
    }
}