namespace Tharga.Console.Helpers
{
    internal class InputLayout
    {
        private int _bufferLineCount;

        public int CurrentBufferLineCount
        {
            get { return _bufferLineCount == 0 ? 1 : (_bufferLineCount + 1); }
            set { _bufferLineCount = value; }
        }

        public int CursorLineOffset { get; set; }
    }
}
