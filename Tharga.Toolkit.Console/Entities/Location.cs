namespace Tharga.Toolkit.Console.Entities
{
    public class Location
    {
        public Location(int left, int top)
        {
            Left = left;
            Top = top;
        }

        public int Left { get; }
        public int Top { get; }
    }
}