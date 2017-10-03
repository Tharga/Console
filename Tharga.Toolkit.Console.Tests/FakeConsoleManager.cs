using System;
using System.Text;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Tests
{
    internal class FakeConsoleManager : IConsoleManager
    {
        private const int CbufferHeight = 300;
        private const int CbufferWidth = 80;
        private int _cursorTop;

        public string[] LineOutput = new string[CbufferHeight];

        public FakeConsoleManager(IKeyInputEngine keyInputEngine = null)
        {
            KeyInputEngine = keyInputEngine ?? new KeyInputEngine();
        }

        public void Dispose()
        {
        }

        public Encoding Encoding { get; }
        public int CursorLeft { get; private set; }

        public int CursorTop
        {
            get { return _cursorTop; }
            private set
            {
                if (value == CbufferHeight)
                    for (var i = 0; i < CbufferHeight - 1; i++)
                        LineOutput[i] = LineOutput[i + 1];
                else
                    _cursorTop = value;
            }
        }

        public int WindowWidth { get; set; }
        public int WindowHeight { get; set; }

        public int BufferWidth
        {
            get { return CbufferWidth; }
            set { throw new NotSupportedException(); }
        }

        public int BufferHeight
        {
            get { return CbufferHeight; }
            set { throw new NotSupportedException(); }
        }

        public void WriteLine(string value)
        {
            DoWriteLine(value, true);
        }

        public void Write(string value)
        {
            DoWriteLine(value, false);
        }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            if (sourceLeft == 0 && (sourceWidth == BufferWidth && (targetLeft == 0 && sourceHeight == 1)))
            {
                LineOutput[targetTop] = LineOutput[sourceTop];
                LineOutput[sourceTop] = null;
            }
            else if (sourceTop == targetTop && sourceHeight == 1)
            {
                if (LineOutput[targetTop].Length != sourceLeft)
                {
                    throw new NotImplementedException("Move buffer on same line, is not yet implemented.");
                }
            }
            else
            {
                //TODO: Implement fuffer move and remove this feature.
                //if (!_bufferMoveEnabled) return; //NOTE: This feature is just becuase Im lazy and do not want to implement this function
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
                if (CursorLeft == 0)
                {
                    LineOutput[CursorTop] = string.Empty;
                    CursorTop++;
                }
                else
                {
                    CursorTop++;
                }
            }
            else
            {
                var lineCount = (int)Math.Ceiling(value.Length / (decimal)BufferWidth);

                //TODO: If this is longer thant the buffer, separate to several lines
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
                        LineOutput[CursorTop] = LineOutput[CursorTop].Substring(0, CursorLeft - 1);
                    }

                    LineOutput[CursorTop] += value;

                    if (lineFeed)
                    {
                        CursorTop++;
                        CursorLeft = 0;
                    }
                    else
                    {
                        CursorLeft = LineOutput[CursorTop].Length;
                    }
                }
            }
        }

        [Obsolete("This is just a temporary test. It might be removed.")]
        public string ReadLine()
        {
            return null;
        }
    }
}