using System;
using System.Collections.Concurrent;
using System.Threading;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Tests
{
    internal class FakeKeyInputEngine : IKeyInputEngine
    {
        private readonly BlockingCollection<ConsoleKeyInfo> _buffer = new BlockingCollection<ConsoleKeyInfo>();

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
            throw new NotImplementedException();
        }

        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return _buffer.Take(cancellationToken);
        }
    }
}