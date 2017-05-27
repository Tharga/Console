using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console
{
    internal static class LocationExtensions
    {
        public static Location Move(this Location item, IConsoleManager consoleManager, string data)
        {
            if (!data.Contains("\n"))
                return item.Move(consoleManager, data.Length);

            var lns = data.Split('\n');
            foreach (var ln in lns)
            {
                var m = item.Move(consoleManager, ln.Length);
                item = new Location(0, m.Top + 1);
            }
            return new Location(item.Left, item.Top - 1);
        }

        public static Location Move(this Location item, IConsoleManager consoleManager, int length)
        {
            var line = 0;
            while (item.Left + length >= consoleManager.BufferWidth)
            {
                line++;
                length = length - consoleManager.BufferWidth;
            }

            return new Location(item.Left + length, item.Top + line);
        }
    }
}