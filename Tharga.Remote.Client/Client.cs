using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Remote.Console;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Remote.Client
{
    internal class Client : IClient
    {
        private readonly ClientConsole _clientConsole;
        private readonly HubConnection _connection;
        private readonly ConcurrentDictionary<string, ConsoleInfo> _store = new();

        public Client(ClientConsole clientConsole)
        {
            _clientConsole = clientConsole;
            var address = "https://localhost:44315/Client";
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

            //TODO: Create reconnect pattern.
            //_connection.Closed += OnClosed;
            //_connection.Reconnecting += OnReconnecting;
            //_connection.Reconnected += OnReconnected;
            _connection.On<ConsoleInfo>(Toolkit.Remote.Console.Constants.OnConsoleConnected, item =>
            {
                _store.AddOrUpdate(item.Key, item, (_, _) => item);
                _clientConsole.OutputInformation($"Console '{item.Name}' connected.");
            });

            _connection.On<ConsoleInfo>(Toolkit.Remote.Console.Constants.OnConsoleDisconnected, item =>
            {
                _store.AddOrUpdate(item.Key, item, (_, _) => item);
                _clientConsole.OutputWarning($"Console '{item.Name}' disconnected.");
            });

            _connection.On<ConsoleInfo[]>(Toolkit.Remote.Console.Constants.OnConsoleList, items =>
            {
                foreach (var item in items)
                {
                    _store.AddOrUpdate(item.Key, item, (_, _) => item);
                }
            });

            _connection.On<LineWrittenInfo>(Toolkit.Remote.Console.Constants.OnLineWritten, data =>
            {
                //TODO: Show what console created the output
                _clientConsole.Output(new WriteEventArgs(data.Value, data.Level));
            });
        }

        public Task ConnectAsync()
        {
            return _connection.StartAsync();
        }

        public Task DisconnectAsync()
        {
            return _connection.StopAsync();
        }

        public Task<ConsoleInfo[]> GetListAsync()
        {
            return Task.FromResult(_store.Values.ToArray());
        }

        public Task SelectAsync(string consoleKey)
        {
            return _connection.SendAsync(Tharga.Toolkit.Remote.Console.Constants.SelectConsole, consoleKey);
        }

        public Task UnselectAsync(string consoleKey)
        {
            return _connection.SendAsync(Tharga.Toolkit.Remote.Console.Constants.UnselectConsole, consoleKey);
        }
    }
}