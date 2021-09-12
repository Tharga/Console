using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Tharga.Remote.Client
{
    internal class Client : IClient
    {
        private readonly HubConnection _connection;

        public Client()
        {
            var address = "https://localhost:44315/Client";
            _connection = new HubConnectionBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddFilter("Microsoft.AspNetCore.SignalR", Microsoft.Extensions.Logging.LogLevel.Debug);
                    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", Microsoft.Extensions.Logging.LogLevel.Debug);
                })
                .WithUrl(address, options =>
                {
                    if (Debugger.IsAttached)
                    {
                        options.Transports = HttpTransportType.LongPolling;
                        options.CloseTimeout = TimeSpan.FromMinutes(10);
                    }

                    //TODO: Use for integration-testing
                    //if (handler != null)
                    //{
                    //    //NOTE: This is needed for integration-unit testing
                    //    options.Transports = HttpTransportType.LongPolling;
                    //    options.HttpMessageHandlerFactory = x => handler;
                    //}
                })
                .Build();

            //_connection.Closed += OnClosed;
            //_connection.Reconnecting += OnReconnecting;
            //_connection.Reconnected += OnReconnected;
            _connection.On<string>(Remote.Common.Constants.OnConsoleConnected, x =>
            {
                Console.WriteLine($"Console connected. {x}");
            });
        }

        public Task ConnectAsync()
        {
            return _connection.StartAsync();
        }
    }
}