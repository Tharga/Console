using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class KeyInputEngine : IKeyInputEngine
    {
        private readonly BlockingCollection<ConsoleKeyInfo> _buffer = new BlockingCollection<ConsoleKeyInfo>();

        public KeyInputEngine()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var keyPressed = System.Console.ReadKey(true);
                    _buffer.Add(keyPressed);
                }
            });
        }

        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return _buffer.Take(cancellationToken);
        }
    }
}