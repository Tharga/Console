using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class ConsoleManager2 : IConsoleManager
    {
        private readonly TextWriter _textWriter;
        private readonly TextReader _textReader;
        private TextWriterInterceptor _textWriterInterceptor;
        private TextReaderInterceptor _textReaderInterceptor;
        private TextWriterInterceptor _errorInterceptor;
        private IKeyInputEngine _keyInputEngine;

        public ConsoleManager2(TextWriter textWriter, TextReader textReader)
        {
            _textWriter = textWriter;
            _textReader = textReader;
        }

        public void Dispose()
        {
            _textReader.Dispose();
            _textWriter.Dispose();
        }

        public Encoding Encoding => Encoding.UTF8;

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
        public void WriteLine(string value)
        {
            _textWriter.WriteLine(value);
        }

        public void Write(string value)
        {
            _textWriter.Write(value);
        }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public IKeyInputEngine KeyInputEngine => _keyInputEngine ?? (_keyInputEngine = new KeyInputEngine2(_textReader));
        public string Title { get; set; }
        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            throw new NotImplementedException();
        }

        public void SetCursorPosition(int left, int top)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            try
            {
                System.Console.Clear();
            }
            catch (IOException exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
            }
        }

        public void Intercept(IConsole console)
        {
            _textWriterInterceptor = new TextWriterInterceptor(this, console);
            _textReaderInterceptor = new TextReaderInterceptor(this, console);
            _errorInterceptor = new TextWriterInterceptor(this, console);
        }
    }
}