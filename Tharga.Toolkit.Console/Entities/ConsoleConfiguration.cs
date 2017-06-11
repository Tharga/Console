using System;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Entities
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
            RememberStartLocation = true;
        }

        public string Title { get; set; }
        public string SplashScreen { get; set; }
        public bool ShowAssemblyInfo { get; set; }
        public bool TopMost { get; set; }
        public ConsoleColor DefaultTextColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
        public Position StartPosition { get; set; }
        public bool RememberStartLocation { get; set; }
    }
}