using System;

namespace Tharga.Toolkit.Console.Command.Base
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