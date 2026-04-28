using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Helpers
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

        public void Feed(string data)
        {
            foreach (var item in data)
            {
                ConsoleKey val;
                ConsoleKey.TryParse(item.ToString(), true, out val);
                _buffer.Add(new ConsoleKeyInfo(item, val, false, false, false));
            }

            _buffer.Add(new ConsoleKeyInfo((char)13, ConsoleKey.Enter, false, false, false));
        }
    }
}