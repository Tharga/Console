using System;
using System.Collections.Generic;

namespace Tharga.Toolkit.Console.Command.Base
{
    public interface IConsole : IDisposable
    {
        event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        event EventHandler<KeyReadEventArgs> KeyReadEvent;

        ConsoleKeyInfo ReadKey(bool intercept);

        void Clear();
        void OutputError(Exception exception);
        void OutputError(string message);
        void OutputWarning(string message);
        void OutputInformation(string message);
        void OutputEvent(string message);
        void OutputHelp(string message);

        void Output(string message, OutputLevel outputLevel, bool trunkateSingleLine = false);
        void Output(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool line);
        void OutputTable(IEnumerable<string> title, IEnumerable<string[]> data, ConsoleColor? consoleColor = null);
        void OutputTable(string[][] data, ConsoleColor? textColor = null);

        void NewLine();
        void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor, ConsoleColor? textBackgroundColor);
        void Write(string value);

        //TODO: Try to hide theese from the interface
        //int CursorLeft { get; set; }
        //int BufferWidth { get; set; }
        //int CursorTop { get; set; }
        //void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop);
        //string ReadLine();
        //ConsoleKeyInfo ReadKey();
        //ConsoleColor ForegroundColor { get; set; }
        //ConsoleColor BackgroundColor { get; set; }
        //void SetCursorPosition(int left, int top);
        //void Initiate(IEnumerable<string> commandKeys);
        //void Output(string message, ConsoleColor? color, OutputLevel outputLevel, bool line);
        //ConsoleColor? GetConsoleColor(OutputLevel outputLevel);
        ////void Mute(string type, bool enabled);
    }
}