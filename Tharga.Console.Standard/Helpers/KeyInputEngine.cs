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
        private readonly CancellationTokenSource _readFailed = new CancellationTokenSource();
        private Exception _readFailure;

        public KeyInputEngine()
            : this(() => System.Console.ReadKey(true))
        {
        }

        internal KeyInputEngine(Func<ConsoleKeyInfo> readKey)
        {
            // The keyboard pump runs for the life of the process, or until keys can no longer be read.
            Task.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        var keyPressed = readKey();
                        _buffer.Add(keyPressed);
                    }
                }
                catch (Exception exception)
                {
                    _readFailure = exception;
                    _readFailed.Cancel();
                }
            });
        }

        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            if (_buffer.TryTake(out var key)) return key;

            using (var takeCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _readFailed.Token))
            {
                try
                {
                    return _buffer.Take(takeCancellation.Token);
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    // The failure is signalled after the last key was added, so any key read before it is in the buffer now.
                    if (_buffer.TryTake(out key)) return key;
                    throw new KeyInputException(_readFailure);
                }
            }
        }

        public void Feed(string data)
        {
            foreach (var item in data)
            {
                Enum.TryParse(item.ToString(), true, out ConsoleKey val);
                _buffer.Add(new ConsoleKeyInfo(item, val, false, false, false));
            }

            _buffer.Add(new ConsoleKeyInfo((char)13, ConsoleKey.Enter, false, false, false));
        }
    }
}