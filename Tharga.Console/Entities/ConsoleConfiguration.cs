using System;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Entities
{
    public class ConsoleConfiguration : IConsoleConfiguration
    {
        public ConsoleConfiguration()
        {
            Title = null;
            SplashScreen = null;
            ShowAssemblyInfo = true;
            TopMost = false;
            DefaultTextColor = ConsoleColor.Gray;
            BackgroundColor = ConsoleColor.Black;
            RememberStartPosition = true;
        }

        public string Title { get; set; }
        public string SplashScreen { get; set; }
        public bool ShowAssemblyInfo { get; set; }
        public bool TopMost { get; set; }
        public ConsoleColor DefaultTextColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public Position StartPosition { get; set; }
        public bool RememberStartPosition { get; set; }
    }
}