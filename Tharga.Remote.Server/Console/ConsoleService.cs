using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tharga.Remote.Server.Client;
using Tharga.Toolkit.Remote.Console;

namespace Tharga.Remote.Server.Console
{
    internal class ConsoleService : IConsoleService
    {
        private readonly IHubContext<ClientHub> _clientHub;
        private readonly IHubContext<ConsoleHub> _consoleHub;
        private readonly IConsoleStore _consoleStore;
        private readonly ISelectStore _selectStore;

        public ConsoleService(IHubContext<ClientHub> clientHub, IHubContext<ConsoleHub> consoleHub, IConsoleStore consoleStore, ISelectStore selectStore)
        {
            _clientHub = clientHub;
            _consoleHub = consoleHub;
            _consoleStore = consoleStore;
            _selectStore = selectStore;
        }

        public async Task UnRegisterAsync(HubCallerContext hubCallerContext, Exception exception)
        {
            var consoleInfo = new ConsoleInfo
            {
                Key = hubCallerContext.ConnectionId,
                Name = "?",
                Status = new ConnectionStatus
                {
                    Message = exception?.Message ?? "Disconnected"
                }
            };

            _consoleStore.Remove(consoleInfo);

            await _clientHub.Clients.All.SendCoreAsync(Toolkit.Remote.Console.Constants.OnConsoleDisconnected, new[] { consoleInfo });
        }

        public async Task SelectAsync(string clientKey, string consoleKey)
        {
            if (_selectStore.Add(clientKey, consoleKey)) await _consoleHub.Clients.Client(consoleKey).SendAsync(Toolkit.Remote.Console.Constants.OnSubscribe);
        }

        public async Task UnselectAsync(string clientKey, string consoleKey)
        {
            if (_selectStore.Remove(clientKey, consoleKey)) await _consoleHub.Clients.Client(consoleKey).SendAsync(Toolkit.Remote.Console.Constants.OnUnsubscribe);
        }

        public async Task UnselectAllAsync(string clientKey)
        {
            var consoleKeys = _selectStore.RemoveClient(clientKey);
            foreach (var consoleKey in consoleKeys) await _consoleHub.Clients.Client(consoleKey).SendAsync(Toolkit.Remote.Console.Constants.OnUnsubscribe);
        }

        public async Task RegisterAsync(HubCallerContext hubCallerContext)
        {
            var consoleInfo = new ConsoleInfo
            {
                Key = hubCallerContext.ConnectionId,
                Name = "?",
                Status = new ConnectionStatus()
            };

            _consoleStore.Add(consoleInfo);

            await _clientHub.Clients.All.SendCoreAsync(Toolkit.Remote.Console.Constants.OnConsoleConnected, new[] { consoleInfo });
        }
    }
}