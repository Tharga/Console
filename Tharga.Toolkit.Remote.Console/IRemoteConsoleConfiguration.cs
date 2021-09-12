using System;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Remote.Console
{
    public interface IRemoteConsoleConfiguration : IConsoleConfiguration
    {
        Uri ServerAddress { get; }
    }
}