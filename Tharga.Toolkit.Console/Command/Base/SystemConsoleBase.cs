using System;
using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class SystemConsoleBase : IConsole
    {
        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;

        protected virtual void InvokeLinesInsertedEvent(int lineCount)
        {
            var handler = LinesInsertedEvent;
            if (handler != null) handler(this, new LinesInsertedEventArgs(lineCount));
        }

        protected abstract void WriteLine(string value);

        private static readonly object SyncRoot = new object();

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
        public virtual ConsoleKeyInfo ReadKey() { return System.Console.ReadKey(); }
        public virtual ConsoleKeyInfo ReadKey(bool intercept) { return System.Console.ReadKey(intercept); }
        public void NewLine() { System.Console.WriteLine(); }
        public void Write(string value) { System.Console.Write(value); }
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

        public virtual void WriteLine(string value, OutputLevel level)
        {
            lock (SyncRoot)
            {
                var linesToInsert = GetLineCount(value);
                var inputBufferLines = InputManager.CurrentBufferLineCount;
                var intCursorLineOffset = InputManager.CursorLineOffset;
                CursorTop = CursorTop - intCursorLineOffset;
                var cursorLeft = MoveInputBufferDown(linesToInsert, inputBufferLines);

                var preColor = System.Console.ForegroundColor;
                try
                {
                    switch (level)
                    {
                        case OutputLevel.Default:
                            //System.Console.ForegroundColor = ConsoleColor.White;
                            break;
                        case OutputLevel.Information:
                            System.Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case OutputLevel.Warning:
                            System.Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                        case OutputLevel.Error:
                            System.Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        default:
                            System.Console.WriteLine("--> Unknown level {0}.", level);
                            System.Console.ForegroundColor = ConsoleColor.Blue;
                            break;
                    }

                    WriteLine(value);
                }
                finally
                {
                    RestoreCursor(cursorLeft);
                    System.Console.ForegroundColor = preColor;
                    InvokeLinesInsertedEvent(linesToInsert);
                    CursorTop = CursorTop + intCursorLineOffset;
                }
            }
        }

        private int GetLineCount(string value)
        {
            try
            {
                return (int)Math.Ceiling((decimal)value.Length / BufferWidth);
            }
            catch (System.IO.IOException)
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
            catch (System.IO.IOException)
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
            catch (System.IO.IOException)
            {
                return 0;
            }
        }

        public void Write(string value, object[] arg)
        {
            System.Console.Write(value, arg);
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