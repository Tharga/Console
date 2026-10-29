using System;
using System.Collections.Generic;
using System.Text;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Tests
{
    internal class FakeConsoleManager : IConsoleManager
    {
        private const int CbufferHeight = 300;
        private readonly int _bufferWidth;
        private int _cursorTop;
        private int _cursorLeft;

        public string[] LineOutput = new string[CbufferHeight];

        public FakeConsoleManager(IKeyInputEngine keyInputEngine = null, int bufferWidth = 80)
        {
            _bufferWidth = bufferWidth;
            KeyInputEngine = keyInputEngine ?? new FakeKeyInputEngine();
        }

        public void Dispose()
        {
        }

        public Encoding Encoding => Encoding.UTF8;

        // Runs once, the next time the cursor column is read. Lets a test write output at that moment, as another thread could.
        public Action OnNextCursorRead { get; set; }

        public int CursorLeft
        {
            get
            {
                var onRead = OnNextCursorRead;
                OnNextCursorRead = null;
                onRead?.Invoke();
                return _cursorLeft;
            }
            private set { _cursorLeft = value; }
        }

        public int CursorTop
        {
            get { return _cursorTop; }
            private set
            {
                if (value == CbufferHeight)
                {
                    // Scrolls, as a real console does: every line moves up one and the new last line is blank.
                    for (var i = 0; i < CbufferHeight - 1; i++)
                        LineOutput[i] = LineOutput[i + 1];
                    LineOutput[CbufferHeight - 1] = null;
                }
                else
                    _cursorTop = value;
            }
        }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public int BufferWidth
        {
            get { return _bufferWidth; }
            set { throw new NotSupportedException(); }
        }

        public int BufferHeight
        {
            get { return CbufferHeight; }
            set { throw new NotSupportedException(); }
        }

        // Every write, with the colours that were set when it was made.
        public List<(string Value, ConsoleColor ForegroundColor, ConsoleColor BackgroundColor)> Writes { get; } = new List<(string, ConsoleColor, ConsoleColor)>();

        public void WriteLine(string value)
        {
            Writes.Add((value, ForegroundColor, BackgroundColor));
            DoWriteLine(value, true);
        }

        public void Write(string value)
        {
            Writes.Add((value, ForegroundColor, BackgroundColor));
            DoWriteLine(value, false);
        }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            if (sourceLeft == 0 && sourceWidth == BufferWidth && targetLeft == 0)
            {
                // Whole lines, one or several: lift them all out first so an overlapping move keeps every line.
                var lines = new string[sourceHeight];
                for (var i = 0; i < sourceHeight; i++)
                {
                    lines[i] = LineOutput[sourceTop + i];
                    LineOutput[sourceTop + i] = null;
                }

                for (var i = 0; i < sourceHeight; i++)
                {
                    LineOutput[targetTop + i] = lines[i];
                }
            }
            else if (sourceTop == targetTop && sourceHeight == 1)
            {
                // Moving the blank part after the text changes nothing.
                if (sourceLeft < (LineOutput[targetTop]?.Length ?? 0))
                {
                    throw new NotImplementedException("Move buffer on same line, is not yet implemented.");
                }
            }
            else
            {
                throw new NotImplementedException("Move partial buffer, is not yet implemented.");
            }
        }

        public void SetCursorPosition(int left, int top)
        {
            if (top >= BufferHeight) throw new InvalidOperationException();
            if (top < 0) throw new InvalidOperationException();
            if (left < 0) throw new InvalidOperationException();
            if (left >= BufferWidth) throw new InvalidOperationException();

            CursorLeft = left;
            CursorTop = top;
        }

        public void Clear()
        {
            LineOutput = new string[CbufferHeight];
            CursorLeft = 0;
            CursorTop = 0;
        }

        public void Intercept(IConsole console)
        {
        }

        public IKeyInputEngine KeyInputEngine { get; }
        public string Title { get; set; }

        private void DoWriteLine(string value, bool lineFeed)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (!lineFeed) return;

                // A line feed moves the cursor to the start of the next line and leaves the text where it is.
                LineOutput[CursorTop] ??= string.Empty;
                CursorTop++;
                CursorLeft = 0;
            }
            else
            {
                var lineCount = (int)Math.Ceiling(value.Length / (decimal)BufferWidth);

                if (value.Length > BufferWidth)
                {
                    for (var i = 0; i < lineCount; i++)
                    {
                        var left = value.Length - i * BufferWidth;
                        if (left == 0)
                            break;
                        if (left > BufferWidth) left = BufferWidth;
                        var input = value.Substring(i * BufferWidth, left);
                        LineOutput[CursorTop] = input;
                        CursorTop++;
                    }
                }
                else
                {
                    if (CursorLeft < (LineOutput[CursorTop]?.Length ?? 0))
                    {
                        LineOutput[CursorTop] = LineOutput[CursorTop].Substring(0, CursorLeft);
                    }

                    LineOutput[CursorTop] += value;

                    if (lineFeed)
                    {
                        CursorTop++;
                        CursorLeft = 0;
                    }
                    else if (LineOutput[CursorTop].Length < BufferWidth)
                    {
                        CursorLeft = LineOutput[CursorTop].Length;
                    }
                    else
                    {
                        // As on a real console, a write that reaches the buffer width continues on the next line.
                        var overflow = LineOutput[CursorTop].Substring(BufferWidth);
                        LineOutput[CursorTop] = LineOutput[CursorTop].Substring(0, BufferWidth);
                        CursorTop++;
                        CursorLeft = 0;
                        DoWriteLine(overflow, false);
                    }
                }
            }
        }
    }
}