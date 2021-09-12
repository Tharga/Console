using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tharga.Remote.Server
{
    internal class ConsoleHub : Hub
    {
        private readonly IConsoleService _consoleService;
        private readonly IClientService _clientService;

        public ConsoleHub(IConsoleService consoleService, IClientService clientService)
        {
            _consoleService = consoleService;
            _clientService = clientService;
        }

        public override async Task OnConnectedAsync()
        {
            await _consoleService.RegisterAsync(Context);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
        }
    }

    internal interface IConsoleService
    {
        Task RegisterAsync(HubCallerContext hubCallerContext);
    }

    internal interface IClientService
    {
        Task RegisterAsync(HubCallerContext hubCallerContext);
    }

    internal class ConsoleService : IConsoleService
    {
        private readonly IHubContext<ClientHub> _consoleHub;

        public ConsoleService(IHubContext<ClientHub> consoleHub)
        {
            _consoleHub = consoleHub;
        }

        public async Task RegisterAsync(HubCallerContext hubCallerContext)
        {
            await _consoleHub.Clients.All.SendCoreAsync(Tharga.Remote.Common.Constants.OnConsoleConnected, new[] { "x" });
        }
    }

    internal class ClientService : IClientService
    {
        public ClientService()
        {
        }

        public Task RegisterAsync(HubCallerContext hubCallerContext)
        {
            return Task.CompletedTask;
        }
    }
}