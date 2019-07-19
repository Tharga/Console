using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class OutputConsoleManager : IConsoleManager
    {
        private readonly TextWriter _textWriter;
        private TextWriterInterceptor _textWriterInterceptor;
        private TextWriterInterceptor _errorInterceptor;
        private int _bufferWidth;
        private IKeyInputEngine _keyInputEngine;

        public OutputConsoleManager(TextWriter textWriter)
        {
            _textWriter = textWriter;
        }

        public Encoding Encoding => _textWriter.Encoding;

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

        public int BufferWidth
        {
            get
            {
                var defaultWidth = 80;
                try
                {
                    if (System.Console.BufferWidth == 0)
                    {
                        Trace.TraceError($"Console buffer width is 0, using {defaultWidth} instead.");
                        return defaultWidth;
                    }

                    return System.Console.BufferWidth;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console buffer width. Using {defaultWidth} as default. {exception.Message}");
                    return defaultWidth;
                }
            }
            set { System.Console.BufferWidth = value; }
        }

        public int BufferHeight
        {
            get
            {
                var defaultHeight = 300;
                try
                {
                    if (System.Console.BufferHeight == 0)
                    {
                        Trace.TraceError($"Console buffer height is 0, using {defaultHeight} instead.");
                        return defaultHeight;
                    }

                    return System.Console.BufferHeight;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console buffer height. Using {defaultHeight} as default. {exception.Message}");
                    return defaultHeight;
                }
            }
            set { System.Console.BufferHeight = value; }
        }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public IKeyInputEngine KeyInputEngine => _keyInputEngine ?? (_keyInputEngine = new NullKeyInputEngine());
        public string Title { get; set; }

        public void Dispose()
        {
            _textWriter.Dispose();
        }

        public void WriteLine(string value)
        {
            _textWriter.WriteLine(value);
        }

        public void Write(string value)
        {
            _textWriter.Write(value);
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            //TODO: This method should throw 'NotSupportedException', it should never needed to be called when using the output console manager.
            //throw new NotSupportedException();
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public void Clear()
        {
        }

        public void Intercept(IConsole console)
        {
            _textWriterInterceptor = new TextWriterInterceptor(this, console);
            //_textReaderInterceptor = new TextReaderInterceptor(this, console);
            _errorInterceptor = new TextWriterInterceptor(this, console);
        }
    }
}