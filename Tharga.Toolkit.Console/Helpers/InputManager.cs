using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Helpers
{
    internal class InputManager : IInputManager
    {
        private readonly IConsole _console;

        public InputManager(IConsole console)
        {
            _console = console;
        }

        public T ReadLine<T>(string paramName, KeyValuePair<T, string>[] selection, bool allowEscape, CancellationToken cancellationToken, char? passwordChar, int? timeoutMilliseconds)
        {
            Task<T> task = null;
            InputInstance inputInstance = null;
            try
            {
                inputInstance = new InputInstance(_console, paramName, passwordChar, cancellationToken);
                task = new Task<T>(() => inputInstance.ReadLine(selection, allowEscape));
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
            catch (Exception e)
            {
                System.Console.WriteLine(e);
                throw;
            }
            finally
            {
                task?.Dispose();
                inputInstance?.Dispose();
            }
        }
    }
}