using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console
{
    internal static class LocationExtensions
    {
        public static Location Move(this Location item, int length)
        {
            var line = 0;
            while (item.Left + length >= System.Console.BufferWidth)
            {
                line++;
                length = length - System.Console.BufferWidth;
            }

            return new Location(item.Left + length, item.Top + line);
        }
    }
}