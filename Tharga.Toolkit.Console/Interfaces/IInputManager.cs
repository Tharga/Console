using System.Collections.Generic;
using System.Threading;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IInputManager
    {
        T ReadLine<T>(string paramName, KeyValuePair<T, string>[] selection, bool allowEscape, CancellationToken cancellationToken, char? passwordChar, int? timeoutMilliseconds);
    }
}