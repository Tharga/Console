using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tharga.Remote.Server.Console
{
    internal interface IConsoleService
    {
        Task RegisterAsync(HubCallerContext hubCallerContext);
        Task UnRegisterAsync(HubCallerContext hubCallerContext, Exception exception);
        Task SelectAsync(string clientKey, string consoleKey);
        Task UnselectAsync(string clientKey, string consoleKey);
        Task UnselectAllAsync(string clientKey);
    }
}