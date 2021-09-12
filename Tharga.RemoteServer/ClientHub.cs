using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tharga.RemoteServer
{
    internal class ClientHub : Hub
    {
        private readonly IClientService _clientService;

        public ClientHub(IClientService clientService)
        {
            _clientService = clientService;
        }

        public override async Task OnConnectedAsync()
        {
            await _clientService.RegisterAsync(Context);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
        }
    }
}