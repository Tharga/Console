namespace Tharga.Toolkit.Console.Command.Base
{
    public class LinesInsertedEventArgs
    {
        private readonly int _lineCount;

        public LinesInsertedEventArgs(int lineCount)
        {
            _lineCount = lineCount;
        }

        public int LineCount { get { return _lineCount; } }
    }
}