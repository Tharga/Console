using System;
using System.Threading;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IKeyInputEngine
    {
        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken);
    }
}