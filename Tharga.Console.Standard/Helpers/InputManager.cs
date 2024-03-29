using System;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Console.Entities;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Helpers
{
    internal class InputManager : IInputManager
    {
        private readonly IConsole _console;

        public InputManager(IConsole console)
        {
            _console = console;
        }

        public T ReadLine<T>(string paramName, CommandTreeNode<T> selection, bool allowEscape, CancellationToken cancellationToken, char? passwordChar, int? timeoutMilliseconds)
        {
            Task<T> task = null;
            InputInstance inputInstance = null;
            try
            {
                task = new Task<T>(() =>
                {
                    inputInstance = new InputInstance(_console, paramName, passwordChar, cancellationToken);
                    return inputInstance.ReadLine(selection, allowEscape);
                });

                task.Start();

                if (timeoutMilliseconds != null)
                {
                    var hasEntry = task.Wait(timeoutMilliseconds.Value);
                    if (!hasEntry)
                    {
                        inputInstance.Cancel();
                    }
                }

                return task.Result;
            }
            catch (AggregateException exception)
            {
                throw exception.InnerException ?? exception;
            }
            finally
            {
                task?.Dispose();
                inputInstance?.Dispose();
            }
        }

        public ConsoleKeyInfo ReadKey()
        {
            return System.Console.ReadKey(true);
        }
    }
}