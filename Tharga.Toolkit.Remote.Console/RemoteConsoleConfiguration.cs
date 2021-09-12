using System;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Remote.Console
{
    public class RemoteConsoleConfiguration : ConsoleConfiguration, IRemoteConsoleConfiguration
    {
        public Uri ServerAddress { get; set; }
    }
}