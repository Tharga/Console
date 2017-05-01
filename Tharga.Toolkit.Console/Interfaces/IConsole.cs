using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IConsole : IDisposable
    {
        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken);

        void Output(IOutput outputEventArgs);
        void OutputError(Exception exception);

        void OutputTable(IEnumerable<IEnumerable<string>> data);
        void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data);



        //TODO: Revisit
        event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        //event EventHandler<KeyReadEventArgs> KeyReadEvent;

        void Clear();
        //void OutputDefault(string message);
        void OutputInformation(string message);
        //void OutputEvent(string message);
        //void OutputHelp(string message);
        //void OutputWarning(string message);
        void OutputError(string message);

        //void Output(string message, OutputLevel outputLevel, bool trunkateSingleLine = false);
        //void Output(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool line);

        void NewLine();
        void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor, ConsoleColor? textBackgroundColor);
        void Write(string value);

        //Technical
        int CursorLeft { get; }
        int CursorTop { get; }
        int BufferWidth { get; }
        void SetCursorPosition(int left, int top);
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
    }
}