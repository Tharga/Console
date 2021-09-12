using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tharga.Remote.Server.Client;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Remote.Console;

namespace Tharga.Remote.Server.Console
{
    internal class ConsoleHub : Hub
    {
        private readonly IClientService _clientService;
        private readonly IConsoleService _consoleService;

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
            await _consoleService.UnRegisterAsync(Context, exception);
        }

        [HubMethodName(Toolkit.Remote.Console.Constants.LineWritten)]
        public Task SelectConsole(LineWrittenInfo lineWrittenInfo)
        {
            return _clientService.SendLineWrittenAsync(Context.ConnectionId, lineWrittenInfo);
        }
    }
}