using System.Collections.Generic;
using System.Threading;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IInputManager
    {
        T ReadLine<T>(string paramName, KeyValuePair<T, string>[] selection, bool allowEscape, CancellationToken cancellationToken, char? passwordChar, int? timeoutMilliseconds);
    }

    //public interface ICommandEngine
    //{
    //    string Title { get; }
    //    string SplashScreen { get; }
    //    bool ShowAssemblyInfo { get; }
    //    bool TopMost { get; }
    //    ConsoleColor BackgroundColor { get; }
    //    ConsoleColor DefaultForegroundColor { get; }
    //    Runner[] Runners { get; }
    //    //IConsole Console { get; }

    //    void Run(string[] args);
    //    void Stop();
    //}
}