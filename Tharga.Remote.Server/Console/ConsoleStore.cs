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

        public void Remove(ConsoleInfo consoleInfo)
        {
            _store.TryRemove(consoleInfo.Key, out _);
        }

        public ConsoleInfo[] GetAll()
        {
            return _store.Values.ToArray();
        }
    }
}