namespace Tharga.Toolkit.Console.Commands.Entities
{
    internal class Location
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