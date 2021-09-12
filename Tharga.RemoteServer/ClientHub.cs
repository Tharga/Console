using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tharga.RemoteServer
{
    internal class ClientHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
        }
    }
}