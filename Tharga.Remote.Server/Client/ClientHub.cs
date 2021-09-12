using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tharga.Remote.Server.Console;

namespace Tharga.Remote.Server.Client
{
    internal class ClientHub : Hub
    {
        private readonly IClientService _clientService;
        private readonly IConsoleService _consoleService;

        public ClientHub(IClientService clientService, IConsoleService consoleService)
        {
            _clientService = clientService;
            _consoleService = consoleService;
        }

        public override async Task OnConnectedAsync()
        {
            await _clientService.RegisterAsync(Context);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _consoleService.UnselectAllAsync(Context.ConnectionId);
        }

        [HubMethodName(Toolkit.Remote.Console.Constants.SelectConsole)]
        public async Task SelectConsole(string key)
        {
            await _consoleService.SelectAsync(Context.ConnectionId, key);
        }

        [HubMethodName(Toolkit.Remote.Console.Constants.UnselectConsole)]
        public async Task UnselectConsole(string key)
        {
            await _consoleService.UnselectAsync(Context.ConnectionId, key);
        }
    }
}