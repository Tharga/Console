using System;

namespace Tharga.Console.Entities
{
    public class PushBufferDownEventArgs : EventArgs
    {
        public PushBufferDownEventArgs(int lineCount)
        {
            LineCount = lineCount;
        }

        public int LineCount { get; }
    }
}