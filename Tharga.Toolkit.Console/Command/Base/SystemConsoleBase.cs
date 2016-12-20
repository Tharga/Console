using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace Tharga.Toolkit.Console.Command.Base
{
    public abstract class SystemConsoleBase : IConsole
    {
        private static readonly object _syncRoot = new object();
        protected internal readonly TextWriter ConsoleWriter;
        private readonly ConsoleInterceptor _interceptor;

        protected SystemConsoleBase(TextWriter consoleWriter)
        {
            ConsoleWriter = consoleWriter;
            if(ConsoleWriter != null)
                _interceptor = new ConsoleInterceptor(ConsoleWriter, this, _syncRoot); //This one intercepts common output.
        }

        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        public event EventHandler<KeyReadEventArgs> KeyReadEvent;

        public int CursorLeft
        {
            get { return System.Console.CursorLeft; }
            set { System.Console.CursorLeft = value; }
        }

        public int BufferWidth
        {
            get { return System.Console.BufferWidth; }
            set { System.Console.BufferWidth = value; }
        }

        public int CursorTop
        {
            get { return System.Console.CursorTop; }
            set { System.Console.CursorTop = value; }
        }

        public ConsoleColor ForegroundColor
        {
            get { return System.Console.ForegroundColor; }
            set { System.Console.ForegroundColor = value; }
        }

        public ConsoleColor BackgroundColor
        {
            get { return System.Console.BackgroundColor; }
            set { System.Console.BackgroundColor = value; }
        }

        public virtual string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public virtual ConsoleKeyInfo ReadKey()
        {
            var consoleKeyInfo = System.Console.ReadKey();
            OnKeyReadEvent(new KeyReadEventArgs(consoleKeyInfo));
            return consoleKeyInfo;
        }

        public virtual ConsoleKeyInfo ReadKey(bool intercept)
        {
            var consoleKeyInfo = System.Console.ReadKey(intercept);
            OnKeyReadEvent(new KeyReadEventArgs(consoleKeyInfo));
            return consoleKeyInfo;
        }

        public void NewLine()
        {
            ConsoleWriter?.WriteLine();
        }

        public void Write(string value)
        {
            ConsoleWriter?.Write(value);
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            try
            {
                System.Console.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }

        public void WriteLine(string value, OutputLevel level, ConsoleColor? consoleColor = null)
        {
            lock (_syncRoot)
            {
                var linesToInsert = GetLineCount(value);
                var inputBufferLines = InputManager.CurrentBufferLineCount;
                var intCursorLineOffset = MoveCursorUp();
                var cursorLeft = MoveInputBufferDown(linesToInsert, inputBufferLines);

                var defaultColor = ConsoleColor.White;
                if (consoleColor == null)
                    consoleColor = GetConsoleColor(level);

                if (consoleColor != null)
                {
                    defaultColor = ForegroundColor;
                    ForegroundColor = consoleColor.Value;
                }

                try
                {
                    WriteLineEx(value, level);
                }
                finally
                {
                    if (consoleColor != null)
                        ForegroundColor = defaultColor;

                    RestoreCursor(cursorLeft);
                    OnLinesInsertedEvent(linesToInsert);
                    MoveCursorDown(intCursorLineOffset);
                }
            }
        }

        public void Clear()
        {
            System.Console.Clear();
        }

        public virtual void Initiate(IEnumerable<string> commandKeys)
        {
        }

        public event EventHandler<LineWrittenEventArgs> LineWrittenEvent;

        protected virtual void OnLinesInsertedEvent(int lineCount)
        {
            var handler = LinesInsertedEvent;
            handler?.Invoke(this, new LinesInsertedEventArgs(lineCount));
        }

        protected virtual void OnLineWrittenEvent(LineWrittenEventArgs e)
        {
            LineWrittenEvent?.Invoke(this, e);
        }

        protected virtual void OnKeyReadEvent(KeyReadEventArgs e)
        {
            KeyReadEvent?.Invoke(this, e);
        }

        protected internal virtual void WriteLineEx(string value, OutputLevel level)
        {
            ConsoleWriter?.WriteLine(value);
            OnLineWrittenEvent(new LineWrittenEventArgs(value, level));
        }

        private void MoveCursorDown(int intCursorLineOffset)
        {
            try
            {
                CursorTop = CursorTop + intCursorLineOffset;
            }
            catch (IOException)
            {
            }
        }

        private int MoveCursorUp()
        {
            try
            {
                var intCursorLineOffset = InputManager.CursorLineOffset;
                CursorTop = CursorTop - intCursorLineOffset;
                return intCursorLineOffset;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        private int GetLineCount(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 1;
                return (int)Math.Ceiling((decimal)value.Length / BufferWidth);
            }
            catch (IOException)
            {
                return 0;
            }
            catch (DivideByZeroException)
            {
                return 1;
            }
        }

        private void RestoreCursor(int cursorLeft)
        {
            try
            {
                CursorLeft = cursorLeft;
            }
            catch (IOException)
            {
            }
        }

        private int MoveInputBufferDown(int linesToInsert, int inputBufferLines)
        {
            try
            {
                MoveBufferArea(0, CursorTop, BufferWidth, inputBufferLines, 0, CursorTop + linesToInsert);
                var cursorLeft = CursorLeft;
                CursorLeft = 0;
                return cursorLeft;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public void Write(string value, object[] arg)
        {
            ConsoleWriter.Write(value, arg);
        }

        public void OutputError(Exception exception)
        {
            OutputError(exception, 0);
        }

        private void OutputError(Exception exception, int indentationLevel)
        {
            var indentation = new string(' ', indentationLevel * 2);
            OutputError($"{indentation}{exception.Message}");
            foreach (DictionaryEntry data in exception.Data)
            {
                OutputError($"{indentation}{data.Key}: {data.Value}");
            }

            if (exception.InnerException != null)
            {
                OutputError(exception.InnerException, ++indentationLevel);
            }
        }

        public void OutputError(string message)
        {
            OutputLine(message, OutputLevel.Error);
        }

        public void OutputWarning(string message)
        {
            OutputLine(message, OutputLevel.Warning);
        }

        public void OutputInformation(string message)
        {
            OutputLine(message, OutputLevel.Information);
        }

        public void OutputEvent(string message, OutputLevel outputLevel = OutputLevel.Default)
        {
            Output(message, outputLevel == OutputLevel.Default ? GetConsoleColor("EventColor", ConsoleColor.Cyan) : GetConsoleColor(outputLevel), outputLevel, true);
        }

        private void OutputLine(string message, OutputLevel outputLevel)
        {
            Output(message, GetConsoleColor(outputLevel), outputLevel, true);
        }

        public void Output(string message, ConsoleColor? color, OutputLevel outputLevel, bool line)
        {
            //if (_console == null) throw new InvalidOperationException("No console assigned. The command was probably not registered, use AttachConsole to do it manually.");

            lock (_syncRoot)
            {
                if (line)
                {
                    //if (args == null || !args.Any())
                    //{
                        WriteLine(message, outputLevel, color);
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        _console.WriteLine(string.Format(message, args), outputLevel, color);
                    //    }
                    //    catch (FormatException exception)
                    //    {
                    //        var exp = new FormatException(exception.Message + " Perhaps the parameters provided does not match the message.", exception);
                    //        exp.Data.Add("Message", message);
                    //        exp.Data.Add("Parameters", args.Count());
                    //        throw exp;
                    //    }
                    //}
                }
                else
                {
                    Write(message);
                }
            }
        }

        public ConsoleColor? GetConsoleColor(OutputLevel outputLevel)
        {
            switch (outputLevel)
            {
                case OutputLevel.Information:
                    return GetConsoleColor("Information", ConsoleColor.Green);
                case OutputLevel.Warning:
                    return GetConsoleColor("Warning", ConsoleColor.Yellow);
                case OutputLevel.Error:
                    return GetConsoleColor("Error", ConsoleColor.Red);
                default:
                    return null;
            }
        }

        private static ConsoleColor GetConsoleColor(string name, ConsoleColor defaultColor)
        {
            var colorString = ConfigurationManager.AppSettings[name + "Color"];
            ConsoleColor color;
            if (!Enum.TryParse(colorString, out color))
            {
                color = defaultColor;
            }

            return color;
        }

        public void Dispose()
        {
            _interceptor?.Dispose();
            ConsoleWriter?.Dispose();
        }
    }
}