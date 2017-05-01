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
            DefaultTextColor = System.Console.ForegroundColor;
            BackgroundColor = System.Console.BackgroundColor;
        }

        public string Title { get; set; }
        public string SplashScreen { get; set; }
        public bool ShowAssemblyInfo { get; set; }
        public bool TopMost { get; set; }
        public ConsoleColor DefaultTextColor { get; set; }
        public ConsoleColor BackgroundColor { get; set; }
    }
}