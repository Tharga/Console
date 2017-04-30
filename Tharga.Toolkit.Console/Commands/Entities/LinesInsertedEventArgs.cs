using System;

namespace Tharga.Toolkit.Console.Commands.Entities
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