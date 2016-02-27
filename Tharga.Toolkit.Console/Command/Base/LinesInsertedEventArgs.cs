using System;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class LinesInsertedEventArgs : EventArgs
    {
        public LinesInsertedEventArgs(int lineCount)
        {
            LineCount = lineCount;
        }

        public int LineCount { get; }
    }
}