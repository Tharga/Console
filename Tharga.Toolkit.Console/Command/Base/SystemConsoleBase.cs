using System;
using System.Collections.Generic;
using System.IO;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class SystemConsoleBase : IConsole
    {
        private static readonly object _syncRoot = new object();
        protected readonly TextWriter _consoleWriter;

        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        public event EventHandler<KeyReadEventArgs> KeyReadEvent;
        public event EventHandler<LineWrittenEventArgs> LineWrittenEvent;

        protected SystemConsoleBase(TextWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
            new ConsoleInterceptor(_consoleWriter, this, _syncRoot); //This one intercepts common output.
        }

        protected virtual void OnLinesInsertedEvent(int lineCount)
        {
            var handler = LinesInsertedEvent;
            handler?.Invoke(this, new LinesInsertedEventArgs(lineCount));
        }

        protected virtual void OnLineWrittenEvent(LineWrittenEventArgs e)
        {
            LineWrittenEvent?.Invoke(this, e);
        }

        protected virtual void OnKeyReadEvent(KeyReadEventArgs e)
        {
            KeyReadEvent?.Invoke(this, e);
        }

        public int CursorLeft
        {
            get { return System.Console.CursorLeft; }
            set { System.Console.CursorLeft = value; }
        }

        public int BufferWidth
        {
            get { return System.Console.BufferWidth; }
            set { System.Console.BufferWidth = value; }
        }

        public int CursorTop
        {
            get { return System.Console.CursorTop; }
            set { System.Console.CursorTop = value; }
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

        public virtual string ReadLine() { return System.Console.ReadLine(); }

        public virtual ConsoleKeyInfo ReadKey()
        {
            var consoleKeyInfo = System.Console.ReadKey();
            OnKeyReadEvent(new KeyReadEventArgs(consoleKeyInfo));
            return consoleKeyInfo;
        }

        public virtual ConsoleKeyInfo ReadKey(bool intercept)
        {
            var consoleKeyInfo = System.Console.ReadKey(intercept);
            OnKeyReadEvent(new KeyReadEventArgs(consoleKeyInfo));
            return consoleKeyInfo;
        }

        public void NewLine() { _consoleWriter.WriteLine(); }

        public void Write(string value)
        {
            _consoleWriter.Write(value);
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            try
            {
                System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
            }
            catch (ArgumentOutOfRangeException)
            {
            }            
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor = null)
        {
            lock (_syncRoot)
            {
                var linesToInsert = GetLineCount(value);
                var inputBufferLines = InputManager.CurrentBufferLineCount;
                var intCursorLineOffset = MoveCursorUp();
                var cursorLeft = MoveInputBufferDown(linesToInsert, inputBufferLines);

                var defaultColor = ConsoleColor.White;
                if (consoleColor == null)
                {
                    consoleColor = CommandBase.GetConsoleColor(level);
                }

                if (consoleColor != null)
                {
                    defaultColor = ForegroundColor;
                    ForegroundColor = consoleColor.Value;
                }

                try
                {
                    WriteLineEx(value, level);
                }
                finally
                {
                    if (consoleColor != null)
                    {
                        ForegroundColor = defaultColor;
                    }

                    RestoreCursor(cursorLeft);
                    OnLinesInsertedEvent(linesToInsert);
                    MoveCursorDown(intCursorLineOffset);
                }
            }
        }

        protected virtual void WriteLineEx(string value, OutputLevel level)
        {
            _consoleWriter.WriteLine(value);
            OnLineWrittenEvent(new LineWrittenEventArgs(value, level));
        }

        private void MoveCursorDown(int intCursorLineOffset)
        {
            try
            {
                CursorTop = CursorTop + intCursorLineOffset;
            }
            catch (IOException)
            {
            }
        }

        private int MoveCursorUp()
        {
            try
            {
                var intCursorLineOffset = InputManager.CursorLineOffset;
                CursorTop = CursorTop - intCursorLineOffset;
                return intCursorLineOffset;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        private int GetLineCount(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 1;
                return (int)Math.Ceiling((decimal)value.Length / BufferWidth);
            }
            catch (IOException)
            {
                return 0;
            }
        }

        private void RestoreCursor(int cursorLeft)
        {
            try
            {
                CursorLeft = cursorLeft;
            }
            catch (IOException)
            {
            }
        }

        private int MoveInputBufferDown(int linesToInsert, int inputBufferLines)
        {
            try
            {
                MoveBufferArea(0, CursorTop, BufferWidth, inputBufferLines, 0, CursorTop + linesToInsert);
                var cursorLeft = CursorLeft;
                CursorLeft = 0;
                return cursorLeft;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public void Write(string value, object[] arg)
        {
            _consoleWriter.Write(value, arg);
        }

        public void Clear()
        {
            System.Console.Clear();
        }

        public virtual void Initiate(IEnumerable<string> commandKeys)
        {
        }
    }
}