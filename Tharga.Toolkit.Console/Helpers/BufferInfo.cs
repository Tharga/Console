namespace Tharga.Toolkit.Console.Commands.Helpers
{
    internal class BufferInfo
    {
        public static BufferInfo Instance { get; } = new BufferInfo();

        public int CurrentBufferLineCount { get; set; }
        public int CursorLineOffset { get; set; }

        private BufferInfo()
        {
        }
    }
}