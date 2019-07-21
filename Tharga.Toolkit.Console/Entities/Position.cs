namespace Tharga.Toolkit.Console.Entities
{
    public class Position
    {
        public Position(int left, int top, int? width = null, int? height = null, int? bufferWidth = null, int? bufferHeight = null)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            BufferWidth = bufferWidth;
            BufferHeight = bufferHeight;
        }

        public int Left { get; }
        public int Top { get; }
        public int? Width { get; }
        public int? Height { get; }
        public int? BufferWidth { get; }
        public int? BufferHeight { get; }
    }
}