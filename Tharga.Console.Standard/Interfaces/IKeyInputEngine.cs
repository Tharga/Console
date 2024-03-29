using System;
using System.Threading;

namespace Tharga.Console.Interfaces
{
    public interface IKeyInputEngine
    {
        void Feed(string data);
        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken);
    }
}