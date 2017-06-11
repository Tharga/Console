using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IKeyInputEngine
    {
        void Feed(string data);
        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken);
    }
}