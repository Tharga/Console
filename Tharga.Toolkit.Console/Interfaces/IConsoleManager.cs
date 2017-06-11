using System;
using System.Text;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IConsoleManager : IDisposable
    {
        Encoding Encoding { get; }
        int CursorLeft { get; }
        int CursorTop { get; }
        int BufferWidth { get; set; }
        int BufferHeight { get; set; }
        int WindowWidth { get; set; }
        int WindowHeight { get; set; }
        void WriteLine(string value);
        void Write(string value);
        ConsoleColor ForegroundColor { get; set; }
        ConsoleColor BackgroundColor { get; set; }
        IKeyInputEngine KeyInputEngine { get; }
        string Title { get; set; }
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
        void SetCursorPosition(int left, int top);
        void Clear();
        void Intercept(IConsole console);
    }
}