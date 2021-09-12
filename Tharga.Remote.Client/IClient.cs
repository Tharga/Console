using System.Threading.Tasks;
using Tharga.Toolkit.Remote.Console;

namespace Tharga.Remote.Client
{
    public interface IClient
    {
        public Task ConnectAsync();
        public Task DisconnectAsync();
        public Task<ConsoleInfo[]> GetListAsync();
        Task SelectAsync(string consoleKey);
        Task UnselectAsync(string consoleKey);
    }
}