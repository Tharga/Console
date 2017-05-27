using System;
using System.Text;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class NullConsoleManager : IConsoleManager
    {
        private readonly NullKeyInputEngine _keyInputEngine;

        public NullConsoleManager()
        {
            _keyInputEngine = new NullKeyInputEngine();
        }

        public void Dispose()
        {
        }

        public Encoding Encoding => Encoding.UTF8;
        public int CursorLeft => 0;
        public int CursorTop => 0;
        public int BufferWidth => 80;
        public int BufferHeight => 300;

        public void WriteLine(string value)
        {
        }

        public void Write(string value)
        {
        }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public IKeyInputEngine KeyInputEngine => _keyInputEngine;
        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
        }

        public void SetCursorPosition(int left, int top)
        {
        }

        public void Clear()
        {
        }

        public void Intercept(IConsole console)
        {
        }
    }
}