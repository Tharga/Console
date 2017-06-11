using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class InputManager : IInputManager
    {
        private readonly IConsole _console;

        public InputManager(IConsole console)
        {
            _console = console;
        }

        public T ReadLine<T>(string paramName, KeyValuePair<T, string>[] selection, bool allowEscape, CancellationToken cancellationToken, char? passwordChar, int? timeoutMilliseconds, IEnumerable<CommandTreeNode> tabTree)
        {
            Task<T> task = null;
            InputInstance inputInstance = null;
            try
            {
                inputInstance = new InputInstance(_console, paramName, passwordChar, cancellationToken, tabTree);
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