using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Commands.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles.Base
{
    public abstract class ConsoleBase : IConsole
    {
        private static readonly object _syncRoot = new object();
        protected internal readonly TextWriter ConsoleWriter;
        private readonly ConsoleInterceptor _interceptor;
        private readonly List<OutputLevel> _mutedTypes = new List<OutputLevel>();

        protected ConsoleBase(TextWriter consoleWriter)
        {
            ConsoleWriter = consoleWriter;
            if (ConsoleWriter != null)
            {
                _interceptor = new ConsoleInterceptor(ConsoleWriter, this, _syncRoot); //This one intercepts common output.
            }
        }

        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        public event EventHandler<KeyReadEventArgs> KeyReadEvent;

        public int CursorLeft
        {
            get
            {
                try
                {
                    return System.Console.CursorLeft;
                }
                catch (IOException exception)
                {
                    Trace.TraceError($"Cannot get console cursor left. Using 0 as default. {exception.Message}");
                    return 0;
                }
            }
            private set { System.Console.CursorLeft = value; }
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
            private set { System.Console.CursorTop = value; }
        }

        private ConsoleColor ForegroundColor
        {
            get { return System.Console.ForegroundColor; }
            set { System.Console.ForegroundColor = value; }
        }

        private ConsoleColor BackgroundColor
        {
            get { return System.Console.BackgroundColor; }
            set { System.Console.BackgroundColor = value; }
        }

        public virtual ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            var consoleKeyInfo = KeyInputEngine.Instance.ReadKey(cancellationToken);
            OnKeyReadEvent(new KeyReadEventArgs(consoleKeyInfo));
            return consoleKeyInfo;
        }

        //public void Write(string value)
        //{
        //    ConsoleWriter?.Write(value);
        //}

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

        public void WriteLine(string value)
        {
            WriteLine(value, OutputLevel.Default, null, null);
        }

        public void WriteLine(string value, OutputLevel level, ConsoleColor? textColor, ConsoleColor? textBackgroundColor)
        {
            lock (_syncRoot)
            {
                if (string.IsNullOrEmpty(value))
                {
                    ConsoleWriter?.WriteLine();
                    return;
                }

                var linesToInsert = GetLineCount(value);
                var inputBufferLines = InputInstance.CurrentBufferLineCount;
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
                var intCursorLineOffset = InputInstance.CursorLineOffset;
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

        public void OutputError(string message)
        {
            Output(new WriteEventArgs(message, OutputLevel.Error));
        }

        public void OutputDefault(string message)
        {
            Output(new WriteEventArgs(message, OutputLevel.Default));
        }

        public void OutputWarning(string message)
        {
            Output(new WriteEventArgs(message, OutputLevel.Warning));
        }

        public void OutputInformation(string message)
        {
            Output(new WriteEventArgs(message, OutputLevel.Information));
        }

        public void OutputEvent(string message)
        {
            Output(new WriteEventArgs(message, OutputLevel.Event));
        }

        public void OutputHelp(string message)
        {
            Output(new WriteEventArgs(message, OutputLevel.Help));
        }

        public void Output(IOutput output)
        {
            if (_mutedTypes.Contains(output.OutputLevel)) return;

            var message = output.TrunkateSingleLine ? output.Message.Truncate() : output.Message;

            lock (_syncRoot)
            {
                if (output.LineFeed)
                {
                    WriteLine(message, output.OutputLevel, output.TextColor, output.TextBackgroundColor);
                }
                else
                {
                    //Write(message);
                    ConsoleWriter?.Write(message);
                }
            }
        }

        public void OutputError(Exception exception)
        {
            OutputError(exception.ToFormattedString());
        }

        public void OutputTable(IEnumerable<IEnumerable<string>> data)
        {
            OutputInformation(data.ToFormattedString());
        }

        public void OutputTable(IEnumerable<string> title, IEnumerable<IEnumerable<string>> data)
        {
            OutputTable(new[] { title }.Union(data));
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