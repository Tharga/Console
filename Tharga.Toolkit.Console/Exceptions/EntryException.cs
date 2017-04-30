using System;

namespace Tharga.Toolkit.Console
{
    internal sealed class EntryException : SystemException
    {
        public EntryException(string message)
            : base(message)
        {
        }

        public EntryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}