using System;

namespace Tharga.Toolkit.Console.Command.Base
{
    public class EntryException : Exception
    {
        public EntryException(string message)
            : base(message)
        {
        }
    }
}