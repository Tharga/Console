using System;
using System.Runtime.InteropServices;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console
{
    internal static class LocationExtensions
    {
        public static Location Move(this Location item, string data)
        {
            if (!data.Contains("\n"))
                return item.Move(data.Length);

            var lns = data.Split('\n');
            foreach (var ln in lns)
            {
                var m = item.Move(ln.Length);
                item = new Location(0, m.Top + 1);
            }
            return new Location(item.Left, item.Top - 1);
        }

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