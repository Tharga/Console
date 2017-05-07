using System;
using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IOutputConsole : IDisposable
    {
        int BufferWidth { get; }
        int BufferHeight { get; }
        void Output(IOutput outputEventArgs);
        void OutputError(Exception exception);
        void OutputTable(IEnumerable<IEnumerable<string>> data);
        void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data);
    }
}