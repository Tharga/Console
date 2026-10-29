using System;
using System.Threading;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Helpers
{
    internal class NullKeyInputEngine : IKeyInputEngine
    {
        // There are no keys to read: wait until the read is cancelled, as a console without key presses would.
        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            cancellationToken.WaitHandle.WaitOne();
            throw new OperationCanceledException(cancellationToken);
        }

        public void Feed(string data)
        {
        }
    }
}