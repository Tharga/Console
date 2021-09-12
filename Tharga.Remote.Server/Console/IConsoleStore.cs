using Tharga.Toolkit.Remote.Console;

namespace Tharga.Remote.Server.Console
{
    internal interface IConsoleStore
    {
        void Add(ConsoleInfo consoleInfo);
        ConsoleInfo Remove(string consoleKey);
        ConsoleInfo[] GetAll();
    }
}