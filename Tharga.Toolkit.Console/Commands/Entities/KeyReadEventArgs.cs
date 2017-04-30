using System;

namespace Tharga.Toolkit.Console.Commands.Entities
{
    public class KeyReadEventArgs : EventArgs
    {
        public KeyReadEventArgs(ConsoleKeyInfo readKey)
        {
            ReadKey = readKey;
        }

        public ConsoleKeyInfo ReadKey { get; }
    }
}