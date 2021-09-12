using System.Collections.Generic;

namespace Tharga.Remote.Server.Console
{
    internal interface ISelectStore
    {
        bool Add(string clientKey, string consoleKey);
        bool Remove(string clientKey, string consoleKey);
        IEnumerable<string> RemoveClient(string clientKey);
        IEnumerable<string> GetSubscribers(string consoleKey);
    }
}