using System;

namespace Tharga.Console.Entities
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