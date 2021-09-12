using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Remote.Server.Console
{
    internal class SelectStore : ISelectStore
    {
        private readonly ConcurrentDictionary<string, string[]> _store = new();

        public bool Add(string clientKey, string consoleKey)
        {
            if (_store.TryGetValue(consoleKey, out var clientKeys))
            {
                //clientKeys = clientKeys.Union(new[] { clientKey }).ToArray();
                //_store.TryUpdate(consoleKey, clientKeys, );
                throw new NotImplementedException();
            }
            else
            {
                _store.TryAdd(consoleKey, new[] { clientKey });
                return true;
            }

            return false;
        }

        public bool Remove(string clientKey, string consoleKey)
        {
            if (_store.TryGetValue(consoleKey, out var clientKeys))
            {
                clientKeys = clientKeys.Where(x => x != clientKey).ToArray();
                if (clientKeys.Any())
                {
                    throw new NotImplementedException();
                }
                else
                {
                    _store.TryRemove(consoleKey, out _);
                    return true;
                }
            }

            return false;
        }

        public IEnumerable<string> RemoveClient(string clientKey)
        {
            foreach (var item in _store.Where(x => x.Value.Any(y => y == clientKey)))
            {
                if (Remove(clientKey, item.Key))
                {
                    yield return item.Key;
                }
            }
        }

        public IEnumerable<string> GetSubscribers(string consoleKey)
        {
            if (_store.TryGetValue(consoleKey, out var clients))
            {
                foreach (var client in clients)
                {
                    yield return client;
                }
            }
        }
    }
}