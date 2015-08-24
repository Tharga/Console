namespace Tharga.Toolkit.Console.Command.Base
{
    internal class Location
    {
        private readonly int _left;
        private readonly int _top;

        public Location(int left, int top)
        {
            _left = left;
            _top = top;
        }

        public int Left { get { return _left; } }
        public int Top { get { return _top; } }
    }
}