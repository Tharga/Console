using System;

namespace Tharga.Console
{
    public sealed class CommandFailedException : SystemException
    {
        public CommandFailedException(string message)
            : base(message)
        {
        }
    }
}