using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IInputManager
    {
        T ReadLine<T>(string paramName, IEnumerable<CommandTreeNode<T>> selection, bool allowEscape, CancellationToken cancellationToken, char? passwordChar, int? timeoutMilliseconds);
        ConsoleKeyInfo ReadKey();
    }
}