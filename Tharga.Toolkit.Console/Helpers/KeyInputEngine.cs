using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class KeyInputEngine
    {
        public static KeyInputEngine Instance { get; } = new KeyInputEngine();

        private readonly BlockingCollection<ConsoleKeyInfo> _buffer = new BlockingCollection<ConsoleKeyInfo>();

        private KeyInputEngine()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var keyPressed = System.Console.ReadKey(true); //TODO: Access through Console Manager class. Important!!!
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