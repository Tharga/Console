using System;

namespace Tharga.Toolkit.Console.Entities
{
    public class ExceptionOccuredEventArgs : EventArgs
    {
        public ExceptionOccuredEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }
    }
}