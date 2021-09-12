using System;
using System.Diagnostics;
using System.Reflection;
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

            //TODO: Provide header data for this specific client.
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

                    if (!string.IsNullOrEmpty(consoleConfiguration?.ConsoleName))
                    {
                        options.Headers.Add("Name", consoleConfiguration?.ConsoleName);
                    }

                    //TODO: Append optional Tags

                    options.Headers.Add("MachineName", Environment.MachineName);

                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    options.Headers.Add("AppName", assemblyName.Name);
                    options.Headers.Add("Version", assemblyName.Version.ToString());

                    if (consoleConfiguration?.Tags != null)
                    {
                        options.Headers.Add("Tags", string.Join(",", consoleConfiguration.Tags));
                    }
                })
                .Build();

            Task.Run(async () =>
            {
                await _connection.StartAsync();
                OutputInformation("Connected.");
            });

            //TODO: Add pattern for reconnect
            _connection.Closed += e =>
            {
                OutputInformation($"Connection closed. ({e?.Message})");
                return Task.CompletedTask;
            };
            _connection.Reconnecting += (e) =>
            {
                OutputInformation($"Reconnecting. ({e?.Message})");
                return Task.CompletedTask;
            };
            _connection.Reconnected += (e) =>
            {
                OutputInformation($"Reconnected. ({e})");
                return Task.CompletedTask;
            };

            _connection.On(Constants.OnSubscribe, () =>
            {
                _subscribing = true;
            });
            _connection.On(Constants.OnUnsubscribe, () =>
            {
                _subscribing = false;
            });
            LineWrittenEvent += async (s, e) =>
            {
                if (_subscribing)
                {
                    var lineWrittenInfo = new LineWrittenInfo
                    {
                        Value = e.Value,
                        Level = e.Level,
                    };
                    await _connection.SendAsync(Constants.LineWritten, lineWrittenInfo);
                }
            };
        }

        public override void Dispose()
        {
            if (_connection.State == HubConnectionState.Connected)
                _connection?.StopAsync().GetAwaiter().GetResult();
            _connection?.DisposeAsync();
            base.Dispose();
        }
    }
}