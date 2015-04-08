namespace Tharga.Toolkit.Console.Command.Base
{
    internal class Location
    {
        private readonly int _left;
        private readonly int _top;

        public Location(int left, int top)
        {
            this._left = left;
            this._top = top;
        }

        public int Left { get { return this._left; } }
        public int Top { get { return this._top; } }
    }
}