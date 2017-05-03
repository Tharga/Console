using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Tharga.Toolkit.Console.Consoles.Base
{
    class ConsoleManager : IConsoleManager
    {
        private readonly TextWriter _textWriter;

        public ConsoleManager(TextWriter textWriter, TextReader textReader)
        {
            _textWriter = textWriter;
        }

        public Encoding Encoding => _textWriter.Encoding;

        public void WriteLine(string value)
        {
            _textWriter.WriteLine(value);
        }

        public void Write(string value)
        {
            _textWriter.Write(value);
        }

        public void Dispose()
        {
            _textWriter.Dispose();
        }

        public int CursorLeft
        {
            get
            {
                try
                {
                    return System.Console.CursorLeft;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console cursor left. Using 0 as default. {exception.Message}");
                    return 0;
                }
            }
        }

        public int BufferWidth
        {
            get
            {
                try
                {
                    return System.Console.BufferWidth;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console buffer width. Using 80 as default. {exception.Message}");
                    return 80;
                }
            }
        }

        public int CursorTop
        {
            get
            {
                try
                {
                    return System.Console.CursorTop;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console cursor top. Using 0 as default. {exception.Message}");
                    return 0;
                }
            }
        }

        public int BufferHeight
        {
            get
            {
                try
                {
                    return System.Console.BufferHeight;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console buffer height. Using 80 as default. {exception.Message}");
                    return 300;
                }
            }
        }

        public ConsoleColor ForegroundColor
        {
            get { return System.Console.ForegroundColor; }
            set { System.Console.ForegroundColor = value; }
        }

        public ConsoleColor BackgroundColor
        {
            get { return System.Console.BackgroundColor; }
            set { System.Console.BackgroundColor = value; }
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            try
            {
                System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
                throw;
            }
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public void Clear()
        {
            System.Console.Clear();
        }
    }
}