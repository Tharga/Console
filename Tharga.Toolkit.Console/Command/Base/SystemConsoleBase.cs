using System;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class SystemConsoleBase : IConsole
    {
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

        public string ReadLine() { return System.Console.ReadLine(); }
        public ConsoleKeyInfo ReadKey() { return System.Console.ReadKey(); }
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
                    System.Console.ForegroundColor = preColor;
                }
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
    }
}