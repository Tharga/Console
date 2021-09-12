using System;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Remote.Console
{
    public class RemoteConsoleConfiguration : ConsoleConfiguration, IRemoteConsoleConfiguration
    {
        public string ConsoleName { get; set; }
        public Uri ServerAddress { get; set; }
        public string[] Tags { get; set; }
    }
}