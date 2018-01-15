using System;
using System.Collections.Concurrent;
using System.Threading;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Tests
{
    internal class FakeKeyInputEngine : IKeyInputEngine
    {
        private readonly BlockingCollection<ConsoleKeyInfo> _buffer = new BlockingCollection<ConsoleKeyInfo>();

        public FakeKeyInputEngine()
        {            
        }

        public FakeKeyInputEngine(ConsoleKey[] data)
        {
            foreach (var item in data)
            {
                _buffer.Add(new ConsoleKeyInfo((char)item, item, false, false, false));
            }
        }

        public FakeKeyInputEngine(ConsoleKeyInfo[] data)
        {
            foreach (var item in data)
            {
                _buffer.Add(item);
            }
        }

        public void Feed(string data)
        {
            foreach (char item in data)
            {
                ConsoleKey key;
                switch (item)
                {
                    case '\r':
                        key = ConsoleKey.Enter;
                        break;
                    case '\n':
                        continue;
                    default:
                        if (!Enum.TryParse(item.ToString(), true, out key))
                            throw new InvalidOperationException($"Cannot parse {item} to key.");
                        break;
                }
                var k = new ConsoleKeyInfo(item, key, false, false, false);
                _buffer.Add(k);
            }
        }

        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return _buffer.Take(cancellationToken);
        }
    }
}