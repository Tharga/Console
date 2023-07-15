namespace Tharga.Console.Entities
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