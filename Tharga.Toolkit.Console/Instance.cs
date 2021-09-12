using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console
{
    public static class Instance
    {
        internal static IConsole Console;
        private static readonly BlockingCollection<object> _queue = new BlockingCollection<object>();

        internal static void Setup(IConsole console, CancellationToken cancellationToken)
        {
            Console = console;
            Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                    try
                    {
                        var item = _queue.Take(cancellationToken);
                        if (item == null) continue;

                        var exception = item as Exception;
                        if (exception != null)
                            Console.OutputError(exception);
                        else if (item is WriteEventArgs) Console.Output((WriteEventArgs)item);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
            }, cancellationToken);
        }

        public static void WriteLine(string value, OutputLevel level = OutputLevel.Default)
        {
            _queue.Add(new WriteEventArgs(value, level));
        }

        public static void WriteLine(Exception exception)
        {
            _queue.Add(exception);
        }
    }
}