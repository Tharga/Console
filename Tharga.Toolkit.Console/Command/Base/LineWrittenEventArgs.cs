using System;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class LineWrittenEventArgs : EventArgs
    {
        public LineWrittenEventArgs(string value, OutputLevel level)
        {
            Value = value;
            Level = level;
        }

        public string Value { get; }
        public OutputLevel Level { get; }
    }
}