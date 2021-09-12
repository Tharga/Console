using System;
using System.Threading;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IConsole : IOutputConsole
    {
        int CursorLeft { get; }
        int CursorTop { get; }
        event EventHandler<PushBufferDownEventArgs> PushBufferDownEvent;
        event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken);
        void Clear();
        void SetCursorPosition(int left, int top);
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
        void Attach(IRootCommand rootCommand);
        void Close();
    }
}