using Tharga.Toolkit.Remote.Console;

namespace Tharga.Remote.Server.Console
{
    internal interface IConsoleStore
    {
        void Add(ConsoleInfo consoleInfo);
        void Remove(ConsoleInfo consoleInfo);
        ConsoleInfo[] GetAll();
    }
}