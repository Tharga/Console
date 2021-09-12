using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tharga.Remote.Server.Console;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Remote.Server.Client
{
    internal class ClientService : IClientService
    {
        private readonly IHubContext<ClientHub> _clientHub;
        private readonly IConsoleStore _consoleStore;
        private readonly ISelectStore _selectStore;

        public ClientService(IHubContext<ClientHub> clientHub, IConsoleStore consoleStore, ISelectStore selectStore)
        {
            _clientHub = clientHub;
            _consoleStore = consoleStore;
            _selectStore = selectStore;
        }

        public async Task RegisterAsync(HubCallerContext hubCallerContext)
        {
            var consoles = _consoleStore.GetAll();
            await _clientHub.Clients.Client(hubCallerContext.ConnectionId).SendAsync(Toolkit.Remote.Console.Constants.OnConsoleList, consoles);
        }

        public async Task SendLineWrittenAsync(string consoleKey, LineWrittenEventArgs lineWrittenEventArgs)
        {
            //TODO: Send this information to all subscribers
            //If there are no subscribers, tell the client to stop sending stuff.
            foreach (var subscriber in _selectStore.GetSubscribers(consoleKey))
            {
                await _clientHub.Clients.Client(subscriber).SendAsync(Toolkit.Remote.Console.Constants.OnLineWritten, lineWrittenEventArgs);
            }
        }
    }
}