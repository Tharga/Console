using System;

namespace Tharga.Toolkit.Console.Entities
{
    public class LineWrittenEventArgs : EventArgs
    {
        internal LineWrittenEventArgs(string value, OutputLevel level)
        {
            Value = value;
            Level = level;
        }

        public string Value { get; }
        public OutputLevel Level { get; }
    }
}