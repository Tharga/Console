using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Tharga.Toolkit.Console.Consoles;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Remote.Console
{
    public class RemoteConsoleConfiguration : ConsoleConfiguration, IRemoteConsoleConfiguration
    {
        public Uri ServerAddress { get; set; }
    }

    public interface IRemoteConsoleConfiguration : IConsoleConfiguration
    {
        Uri ServerAddress { get; }
    }

    public class RemoteConsole : ClientConsole
    {
        private readonly HubConnection _connection;

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
        }
    }
}