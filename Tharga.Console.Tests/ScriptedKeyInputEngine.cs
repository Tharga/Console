using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Tests
{
    // Returns keys in order and runs each action when its turn comes, on the thread that reads the keys.
    // When the script has run out, a read waits until it is cancelled.
    internal class ScriptedKeyInputEngine : IKeyInputEngine
    {
        private readonly Queue<object> _script = new Queue<object>();

        public ScriptedKeyInputEngine Keys(string text)
        {
            foreach (var character in text)
            {
                _script.Enqueue(character == '\r'
                    ? new ConsoleKeyInfo(character, ConsoleKey.Enter, false, false, false)
                    : new ConsoleKeyInfo(character, (ConsoleKey)Enum.Parse(typeof(ConsoleKey), character.ToString(), true), false, false, false));
            }

            return this;
        }

        public ScriptedKeyInputEngine Then(Action action)
        {
            _script.Enqueue(action);
            return this;
        }

        public void Feed(string data)
        {
            Keys(data);
        }

        public ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            while (_script.Count > 0)
            {
                var next = _script.Dequeue();
                if (next is Action action)
                {
                    action();
                    continue;
                }

                return (ConsoleKeyInfo)next;
            }

            cancellationToken.WaitHandle.WaitOne();
            throw new OperationCanceledException(cancellationToken);
        }
    }
}
