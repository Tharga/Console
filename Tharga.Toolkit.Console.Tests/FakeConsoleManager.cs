using System;
using System.Collections.Generic;
using System.Text;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Tests
{
    internal class FakeConsoleManager : IConsoleManager
    {
        private const int _bufferHeight = 300;
        private const int _bufferWidth = 80;

        public string[] LineOutput = new string[_bufferHeight];
        private int _cursorTop;

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
                if (value == _bufferHeight)
                {
                    //NOTE: Keep the buffer height, but move the array one step up
                    for (var i = 0; i < _bufferHeight - 1; i++)
                    {
                        LineOutput[i] = LineOutput[i + 1];
                    }
                }
                else
                {
                    _cursorTop = value;
                }
            }
        }

        public int BufferWidth => _bufferWidth;
        public int BufferHeight => _bufferHeight;
        public void WriteLine(string value)
        {
            //RawOutput.Add(value ?? string.Empty);

            if (string.IsNullOrEmpty(value))
            {
                LineOutput[CursorTop] = string.Empty;
                CursorTop++;
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
                        //LineOutput.Add(input);
                        LineOutput[CursorTop] = input;
                        CursorTop++;
                    }
                }
                else
                {
                    LineOutput[CursorTop] = value;
                    CursorTop++;
                }
            }

            //while (LineOutput.Count >= BufferHeight)
            //{
            //    LineOutput.RemoveAt(0);
            //}
        }

        public void Write(string value)
        {
            WriteLine(value);
        }

        public ConsoleColor ForegroundColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
        }

        public void SetCursorPosition(int left, int top)
        {
            if(top >= BufferHeight) throw new InvalidOperationException();
            if(top < 0) throw new InvalidOperationException();
            if(left < 0) throw new InvalidOperationException();
            if(left >= BufferWidth) throw new InvalidOperationException();

            CursorLeft = left;
            CursorTop = top;
        }

        public void Clear()
        {
            LineOutput = new string[_bufferHeight];
            CursorLeft = 0;
            CursorTop = 0;
        }

        public void Intercept(IConsole console)
        {
        }
    }
}