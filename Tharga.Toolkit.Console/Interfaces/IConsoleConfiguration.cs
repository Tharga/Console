using System;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IConsoleConfiguration
    {
        string Title { get; }
        string SplashScreen { get; }
        bool ShowAssemblyInfo { get; }
        bool TopMost { get; }
        ConsoleColor DefaultTextColor { get; }
        ConsoleColor BackgroundColor { get; }
    }
}