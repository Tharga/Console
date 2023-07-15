using System;

namespace Tharga.Toolkit.Console.Entities
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