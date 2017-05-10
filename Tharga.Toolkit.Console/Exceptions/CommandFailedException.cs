using System;

namespace Tharga.Toolkit.Console
{
    public sealed class CommandFailedException : SystemException
    {
        public CommandFailedException(string message)
            : base(message)
        {
        }
    }
}