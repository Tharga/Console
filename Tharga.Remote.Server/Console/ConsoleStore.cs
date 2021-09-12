using System.Collections.Concurrent;
using System.Linq;
using Tharga.Toolkit.Remote.Console;

namespace Tharga.Remote.Server.Console
{
    internal class ConsoleStore : IConsoleStore
    {
        private readonly ConcurrentDictionary<string, ConsoleInfo> _store = new();

        public void Add(ConsoleInfo consoleInfo)
        {
            _store.TryAdd(consoleInfo.Key, consoleInfo);
        }

        public ConsoleInfo Remove(string consoleKey)
        {
            _store.TryRemove(consoleKey, out var consoleInfo);
            return consoleInfo;
        }

        public ConsoleInfo[] GetAll()
        {
            return _store.Values.ToArray();
        }
    }
}