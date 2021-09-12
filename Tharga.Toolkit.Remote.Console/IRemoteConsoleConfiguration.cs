using System;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Remote.Console
{
    public interface IRemoteConsoleConfiguration : IConsoleConfiguration
    {
        string ConsoleName { get; }
        Uri ServerAddress { get; }
        string[] Tags { get; }
    }
}