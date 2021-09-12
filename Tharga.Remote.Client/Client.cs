using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Tharga.Toolkit.Remote.Console;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Remote.Client
{
    internal class Client : IClient
    {
        private readonly HubConnection _connection;
        private readonly ConcurrentDictionary<string, ConsoleInfo> _store = new();

        public Client()
        {
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

            //_connection.Closed += OnClosed;
            //_connection.Reconnecting += OnReconnecting;
            //_connection.Reconnected += OnReconnected;
            _connection.On<ConsoleInfo>(Tharga.Toolkit.Remote.Console.Constants.OnConsoleConnected, item =>
            {
                _store.AddOrUpdate(item.Key, item, (_, _) => item);
                Console.WriteLine($"Console connected. {item.Name}");
            });

            _connection.On<ConsoleInfo>(Tharga.Toolkit.Remote.Console.Constants.OnConsoleDisconnected, item =>
            {
                _store.AddOrUpdate(item.Key, item, (_, _) => item);
                Console.WriteLine($"Console disconnected. {item.Name}");
            });

            _connection.On<ConsoleInfo[]>(Tharga.Toolkit.Remote.Console.Constants.OnConsoleList, items =>
            {
                foreach (var item in items)
                {
                    _store.AddOrUpdate(item.Key, item, (_, _) => item);
                }
            });

            _connection.On<LineWrittenEventArgs>(Tharga.Toolkit.Remote.Console.Constants.OnLineWritten, data =>
            {
                //TODO: Use nicer output
                Console.WriteLine($"{data.Level}: {data.Value}");
            });
        }

        public Task ConnectAsync()
        {
            return _connection.StartAsync();
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