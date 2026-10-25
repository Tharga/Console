using System;
using System.Threading;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Helpers
{
    internal class NullKeyInputEngine : IKeyInputEngine
    {
        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
        }

        public void Feed(string data)
        {
        }
    }
}