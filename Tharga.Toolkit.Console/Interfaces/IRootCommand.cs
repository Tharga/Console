using System;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface ICommandEngine
    {
        string Title { get; }
        string SplashScreen { get; }
        bool ShowAssemblyInfo { get; }
        bool TopMost { get; }
        ConsoleColor BackgroundColor { get; }
        ConsoleColor DefaultForegroundColor { get; }
        Runner[] Runners { get; }
        IConsole Console { get; }

        void Run(string[] args);
        void Stop();
    }
}