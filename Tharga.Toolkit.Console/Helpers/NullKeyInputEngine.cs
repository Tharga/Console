using System;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class NullKeyInputEngine : IKeyInputEngine
    {
        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10000);
                }

                return new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false);
            }).Result;
        }

        public void Feed(string data)
        {
        }
    }
}