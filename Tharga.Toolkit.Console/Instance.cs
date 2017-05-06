using System;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console
{
    public static class Instance
    {
        internal static IConsole Console;

        internal static void Setup(IConsole console)
        {
            Console = console;
        }

        public static void WriteLine(string value, OutputLevel level = OutputLevel.Default)
        {
            if (Console == null) throw new InvalidOperationException("No Tharga.Toolkit.Console has been set up.");
            Console.Output(new WriteEventArgs(value, level));
        }
    }
}