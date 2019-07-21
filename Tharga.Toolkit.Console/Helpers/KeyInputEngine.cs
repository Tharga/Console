using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class KeyInputEngine2 : IKeyInputEngine
    {
        private readonly TextReader _textReader;

        public KeyInputEngine2(TextReader textReader)
        {
            _textReader = textReader;
        }

        public void Feed(string data)
        {
            throw new NotImplementedException();
        }

        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            var buffer = new char[1];
            var c = _textReader.Read(buffer, 0, 1);
            return new ConsoleKeyInfo(buffer[0], ConsoleKey.A, false, false, false);
        }
    }

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
                Enum.TryParse(item.ToString(), true, out val);
                _buffer.Add(new ConsoleKeyInfo(item, val, false, false, false));
            }

            _buffer.Add(new ConsoleKeyInfo((char) 13, ConsoleKey.Enter, false, false, false));
        }
    }
}