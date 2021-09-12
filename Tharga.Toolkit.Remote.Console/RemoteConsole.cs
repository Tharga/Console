using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Tharga.Toolkit.Console.Consoles;

namespace Tharga.Toolkit.Remote.Console
{
    public class RemoteConsole : ClientConsole
    {
        private readonly HubConnection _connection;
        private bool _subscribing;

        public RemoteConsole(IRemoteConsoleConfiguration consoleConfiguration = null)
            : base(consoleConfiguration)
        {
            var address = consoleConfiguration?.ServerAddress != null
                ? $"{consoleConfiguration.ServerAddress.AbsoluteUri}Console"
                : "https://localhost:44315/Console";

            _connection = new HubConnectionBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
                    logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);
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

            Task.Run(async () =>
            {
                await _connection.StartAsync();
                OutputInformation("Connected to server.");
            });

            //TODO: Add pattern for reconnect
            //_connection.Closed += OnClosed;
            //_connection.Reconnecting += OnReconnecting;
            //_connection.Reconnected += OnReconnected;
            _connection.On(Constants.OnSubscribe, () =>
            {
                //TODO: Start sending output to server.
                //TODO: Send initial buffer (the first x lines, from startup of the service)
                //TODO: Display something to show there is an external subscription opened.
                _subscribing = true;
            });
            _connection.On(Constants.OnUnsubscribe, () =>
            {
                //TODO: Stop subscription
                _subscribing = false;
            });

            this.LineWrittenEvent += async (s, e) =>
            {
                if (_subscribing)
                {
                    await _connection.SendAsync(Constants.LineWritten, e);
                }
            };
        }

        public override void Dispose()
        {
            _connection?.StopAsync().GetAwaiter().GetResult();
            _connection?.DisposeAsync();
            base.Dispose();
        }
    }
}