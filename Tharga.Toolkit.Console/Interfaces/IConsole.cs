using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Toolkit.Console.Commands.Entities;

namespace Tharga.Toolkit.Console.Interfaces
{
    public interface IConsole : IDisposable
    {
        void Output(ITextOutput outputEventArgs);

        void OutputError(Exception exception);

        void OutputTable(IEnumerable<IEnumerable<string>> data);
        void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data);

        //TODO: Revisit
        event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        event EventHandler<KeyReadEventArgs> KeyReadEvent;

        ConsoleKeyInfo ReadKey(CancellationToken cancellationToken); //bool intercept);

        void Clear();
        void OutputDefault(string message);
        void OutputInformation(string message);
        void OutputEvent(string message);
        void OutputHelp(string message);
        void OutputWarning(string message);
        void OutputError(string message);

        //void Output(string message, OutputLevel outputLevel, bool trunkateSingleLine = false);
        //void Output(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool line);

        void NewLine();
        void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor, ConsoleColor? textBackgroundColor);
        void Write(string value);

        //Technical
        int CursorLeft { get; set; }
        int CursorTop { get; set; }
        int BufferWidth { get; }
        void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
    }
}