using System;

namespace Tharga.Console
{
    // Keys can no longer be read from the console, for example because standard input is redirected.
    internal sealed class KeyInputException : InvalidOperationException
    {
        public KeyInputException(Exception innerException)
            : base($"Cannot read keys from the console: {innerException?.Message} When standard input is redirected, pass the commands as program arguments instead.", innerException)
        {
        }
    }
}
