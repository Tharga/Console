using System;
using Tharga.Console.Entities;

namespace Tharga.Console.Interfaces
{
    public interface IConsoleConfiguration
    {
        string Title { get; }
        string SplashScreen { get; }
        bool ShowAssemblyInfo { get; }
        bool TopMost { get; }
        ConsoleColor DefaultTextColor { get; }
        ConsoleColor BackgroundColor { get; }
        Position StartPosition { get; }
        bool RememberStartPosition { get; }
    }
}