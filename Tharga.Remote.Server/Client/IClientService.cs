using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Tharga.Toolkit.Remote.Console;

namespace Tharga.Remote.Server.Client
{
    internal interface IClientService
    {
        Task RegisterAsync(HubCallerContext hubCallerContext);
        Task SendLineWrittenAsync(string consoleKey, LineWrittenInfo lineWrittenInfo);
    }
}