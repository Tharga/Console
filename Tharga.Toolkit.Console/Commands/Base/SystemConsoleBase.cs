using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Commands.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.Base
{
    public abstract class SystemConsoleBase : IConsole
    {
        private static readonly object _syncRoot = new object();
        protected internal readonly TextWriter ConsoleWriter;
        private readonly ConsoleInterceptor _interceptor;
        private readonly List<OutputLevel> _mutedTypes = new List<OutputLevel>();

        protected SystemConsoleBase(TextWriter consoleWriter)
        {
            ConsoleWriter = consoleWriter;
            if (ConsoleWriter != null)
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
            get
            {
                try
                {
                    return System.Console.BufferWidth;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console buffer width. Using 80 as default. {exception.Message}");
                    return 80;
                }
            }
            set { System.Console.BufferWidth = value; }
        }

        public int CursorTop
        {
            get
            {
                try
                {
                    return System.Console.CursorTop;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console cursor top. Using 0 as default. {exception.Message}");
                    return 0;
                }
            }
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

        public void WriteLine(string value, OutputLevel level, ConsoleColor? textColor, ConsoleColor? textBackgroundColor)
        {
            lock (_syncRoot)
            {
                var linesToInsert = GetLineCount(value);
                //Debug.WriteLine("Lines: " + linesToInsert);
                var inputBufferLines = InputManager.CurrentBufferLineCount;
                var intCursorLineOffset = MoveCursorUp();
                var cursorLeft = MoveInputBufferDown(linesToInsert, inputBufferLines);

                var defaultColor = ConsoleColor.White;
                var defaultBack = ConsoleColor.Black;
                if (textColor == null || textBackgroundColor == null)
                {
                    var dk = GetConsoleColor(level);
                    if (textColor == null)
                        textColor = dk.Item1;
                    if (textBackgroundColor == null)
                        textBackgroundColor = dk.Item2;
                }

                if (textColor != null)
                {
                    defaultColor = ForegroundColor;
                    ForegroundColor = textColor.Value;
                }

                if (textBackgroundColor != null)
                {
                    defaultBack = BackgroundColor;
                    BackgroundColor = textBackgroundColor.Value;
                }

                var corr = 0;
                var t1 = CursorTop;
                try
                {
                    WriteLineEx(value, level);
                    corr = CursorTop - t1 - linesToInsert;
                }
                finally
                {
                    if (textColor != null)
                    {
                        ForegroundColor = defaultColor;
                    }
                    if (textBackgroundColor != null)
                    {
                        BackgroundColor = defaultBack;
                    }

                    RestoreCursor(cursorLeft);
                    OnLinesInsertedEvent(linesToInsert);
                    //System.Diagnostics.Debug.WriteLine("Corr: " + corr);
                    MoveCursorDown(intCursorLineOffset- corr);
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

        public void OutputTable(IEnumerable<string> title, IEnumerable<string[]> data, ConsoleColor? consoleColor = null)
        {
            var table = new List<string[]> { title.ToArray() };
            table.AddRange(data.Select(item => item.ToArray()));
            OutputTable(table.ToArray(), consoleColor);
        }

        public void OutputTable(string[][] data, ConsoleColor? textColor = null)
        {
            var columnLength = GetColumnSizes(data);

            foreach (var line in data)
            {
                var sb = new StringBuilder();
                for (var i = 0; i < line.Length; i++)
                {
                    sb.AppendFormat("{0}{1}", line[i], new string(' ', columnLength[i] - line[i].Length + 1));
                }

                Output(sb.ToString(), OutputLevel.Information, textColor, null, false, true);
            }

            var lineCount = data.Length - 1;
            if (lineCount < 0) lineCount = 0;
            Output($"{lineCount} lines.", OutputLevel.Information, textColor, null, false, true);
        }

        private static int[] GetColumnSizes(string[][] data)
        {
            if (data.Length == 0)
                return new int[] {};

            var length = new int[data[0].Length];
            foreach (var line in data)
            {
                for (var i = 0; i < line.Length; i++)
                {
                    if (line[i].Length > length[i])
                    {
                        length[i] = line[i].Length;
                    }
                }
            }

            return length.ToArray();
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
            if (ForegroundColor == BackgroundColor)
            {
                ForegroundColor = ForegroundColor != ConsoleColor.Black ? ConsoleColor.Black : ConsoleColor.White;
            }

            var lines = value.Split('\n');
            foreach (var line in lines)
            {
                var r = line.Length % BufferWidth;
                if (r == 0) // && !string.IsNullOrEmpty(line))
                {
                    if (string.IsNullOrEmpty(line))
                        ConsoleWriter?.WriteLine(line);
                    else
                        ConsoleWriter?.Write(line);
                }
                else
                {
                    ConsoleWriter?.WriteLine(line);
                }
            }
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

                var lns = value.Split('\n');
                var l = 0;
                foreach (var ln in lns)
                {
                    l += (int)Math.Ceiling(((decimal)ln.Length + 1) / BufferWidth);
                    if (ln.Length % BufferWidth == 0 && !string.IsNullOrEmpty(ln))
                        l--;
                }

                return l;
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
            if (_mutedTypes.Contains(OutputLevel.Error)) return;

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
            Output(message, OutputLevel.Error);
        }

        public void OutputDefault(string message)
        {
            Output(message, OutputLevel.Default);
        }

        public void OutputWarning(string message)
        {
            Output(message, OutputLevel.Warning);
        }

        public void OutputInformation(string message)
        {
            Output(message, OutputLevel.Information);
        }

        public void OutputEvent(string message)
        {
            Output(message, OutputLevel.Event);
        }

        public void OutputHelp(string message)
        {
            Output(message, OutputLevel.Help);
        }

        public void Output(string message, OutputLevel outputLevel, bool trunkateSingleLine = false)
        {
            Output(message, outputLevel, null, null, trunkateSingleLine, true);
        }

        public void Output(string message, OutputLevel outputLevel, ConsoleColor? textColor, ConsoleColor? textBackgroundColor, bool trunkateSingleLine, bool line)
        {
            if (_mutedTypes.Contains(outputLevel)) return;

            if (trunkateSingleLine)
            {
                message = message.Truncate();
            }

            lock (_syncRoot)
            {
                if (line)
                {
                    WriteLine(message, outputLevel, textColor, textBackgroundColor);
                }
                else
                {
                    Write(message);
                }
            }
        }

        protected internal Tuple<ConsoleColor?, ConsoleColor?> GetConsoleColor(OutputLevel outputLevel)
        {
            switch (outputLevel)
            {
                case OutputLevel.Default:
                    return new Tuple<ConsoleColor?, ConsoleColor?>(null, null);
                case OutputLevel.Information:
                    return GetConsoleColor("Information", ConsoleColor.Green, null);
                case OutputLevel.Warning:
                    return GetConsoleColor("Warning", ConsoleColor.Yellow, null);
                case OutputLevel.Error:
                    return GetConsoleColor("Error", ConsoleColor.Red, null);
                case OutputLevel.Event:
                    return GetConsoleColor("Event", ConsoleColor.Cyan, null);
                case OutputLevel.Help:
                    return GetConsoleColor("Help", ConsoleColor.Magenta, null);
                case OutputLevel.Title:
                    return GetConsoleColor("Title", ConsoleColor.DarkCyan, null);
                default:
                    return new Tuple<ConsoleColor?, ConsoleColor?>(null, null);
                    //throw new ArgumentOutOfRangeException($"Unknown output level {outputLevel}.");
            }
        }

        private static Tuple<ConsoleColor?, ConsoleColor?> GetConsoleColor(string name, ConsoleColor defaultColor, ConsoleColor? defaultTextBackgroundColor)
        {
            var colorString = ConfigurationManager.AppSettings[name + "Color"];
            if (string.IsNullOrEmpty(colorString)) return new Tuple<ConsoleColor?, ConsoleColor?>(defaultColor, defaultTextBackgroundColor);
            var cols = colorString.Split(';');
            ConsoleColor color;
            if (!Enum.TryParse(cols[0], out color))
            {
                color = defaultColor;
            }

            ConsoleColor? color3 = defaultTextBackgroundColor;
            if (cols.Length > 1)
            {
                ConsoleColor color2;
                if (Enum.TryParse(cols[1], out color2))
                {
                    color3 = color2;
                }
            }

            return new Tuple<ConsoleColor?, ConsoleColor?>(color, color3);
        }

        public void Dispose()
        {
            _interceptor?.Dispose();
            ConsoleWriter?.Dispose();
        }

        public void Mute(OutputLevel type)
        {
            OutputInformation($"{type}s are now muted. Type 'scr unmute {type}' to show messages.");
            _mutedTypes.Add(type);
        }

        public void Unmute(OutputLevel type)
        {
            _mutedTypes.Remove(type);
        }
    }
}