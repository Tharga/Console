namespace Tharga.Toolkit.Console.Entities
{
    public class Location
    {
        public int Left { get; }
        public int Top { get; }

        public Location(int left, int top)
        {
            Left = left;
            Top = top;
        }
    }
}