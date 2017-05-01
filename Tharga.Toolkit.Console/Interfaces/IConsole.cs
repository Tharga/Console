using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IConsole : IDisposable
    {
        event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;

        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken);

        void Output(IOutput outputEventArgs);
        void OutputError(Exception exception);
        void OutputTable(IEnumerable<IEnumerable<string>> data);
        void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data);

        void Clear();

        //Technical
        int CursorLeft { get; }
        int CursorTop { get; }
        int BufferWidth { get; }
        void SetCursorPosition(int left, int top);
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);


        //TODO: Revisit
        void Write(string value);
    }
}