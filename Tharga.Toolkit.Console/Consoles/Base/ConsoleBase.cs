using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles.Base
{
    public abstract class ConsoleBase : IConsole
    {
        private readonly IConsoleManager _consoleManager;
        private readonly TextWriterInterceptor _textWriterInterceptor;
        private readonly TextReaderInterceptor _textReaderInterceptor;
        private readonly TextWriterInterceptor _errorInterceptor;
        private readonly List<OutputLevel> _mutedTypes = new List<OutputLevel>();

        protected ConsoleBase(TextWriter originalTextWriter, TextReader originalTextReader)
        {
            _consoleManager = new ConsoleManager(originalTextWriter, originalTextReader);
            if (_consoleManager != null)
            {
                _textWriterInterceptor = new TextWriterInterceptor(_consoleManager, this);
                _textReaderInterceptor = new TextReaderInterceptor(_consoleManager, this);
                _errorInterceptor = new TextWriterInterceptor(_consoleManager, this);
            }

            if (Instance.Console == null)
            {
                Instance.Setup(this);
            }
        }

        public event EventHandler<LinesInsertedEventArgs> LinesInsertedEvent;
        public event EventHandler<KeyReadEventArgs> KeyReadEvent;

        public int CursorLeft => _consoleManager.CursorLeft;
        public int BufferWidth => _consoleManager.BufferWidth;
        public int CursorTop => _consoleManager.CursorTop;

        public virtual ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            var consoleKeyInfo = KeyInputEngine.Instance.ReadKey(cancellationToken);
            OnKeyReadEvent(new KeyReadEventArgs(consoleKeyInfo));
            return consoleKeyInfo;
        }

        public void MoveBufferArea(int sourceLeft, int sourceTop, int sourceWidth, int sourceHeight, int targetLeft, int targetTop)
        {
            _consoleManager.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop);
        }

        public void SetCursorPosition(int left, int top)
        {
            //if (top >= _consoleManager.BufferHeight)
            //{
            //    //top = System.Console.BufferHeight - 1; //TODO: This is not okey. fix the problem!
            //}
            _consoleManager.SetCursorPosition(left, top);
        }

        //TODO: Merge Write and WriteLine to be the same method
        private void Write(string value, OutputLevel level = OutputLevel.Default, ConsoleColor? textColor = null, ConsoleColor? textBackgroundColor = null)
        {
            //TODO: Missing stuff, look at WriteLine, there is more stuff going on there!
            WriteEx(value);
        }

        private void WriteLine(string value, OutputLevel level = OutputLevel.Default, ConsoleColor? textColor = null, ConsoleColor? textBackgroundColor = null)
        {
            //lock (CommandEngine.SyncRoot)
            //{
                if (string.IsNullOrEmpty(value))
                {
                    _consoleManager?.WriteLine(null);
                    return;
                }

                var linesToInsert = GetLineCount(value);
                var inputBufferLines = InputInstance.CurrentBufferLineCount;
                var intCursorLineOffset = MoveCursorUp();
                var cursorLocation = MoveInputBufferDown(linesToInsert, inputBufferLines); //TODO: If this is located at the end of the buffer, then the buffer should be pushed...

            //NOTE: At this point, the buffer is moved down. The cursor is ready to output the 'value' to be written.
            //When done the buffer (ie '> ') should still be visible, and the cursor should be moved in position

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
                    defaultColor = _consoleManager.ForegroundColor;
                    _consoleManager.ForegroundColor = textColor.Value;
                }

                if (textBackgroundColor != null)
                {
                    defaultBack = _consoleManager.BackgroundColor;
                    _consoleManager.BackgroundColor = textBackgroundColor.Value;
                }

                var corr = 0;
                var t1 = CursorTop; //NOTE: The actual cursor location after insert... should normally be one line above the input buffer location, but if the output is exactly the sise of the width, then the buffer would have beend moved down one line too low. In this case the buffer should not moved vertically. This is calculated by the corr value.
                try
                {
                    WriteLineEx(value, level);
                    corr = CursorTop - t1 - linesToInsert;
                }
                finally
                {
                    if (textColor != null)
                    {
                        _consoleManager.ForegroundColor = defaultColor;
                    }
                    if (textBackgroundColor != null)
                    {
                        _consoleManager.BackgroundColor = defaultBack;
                    }

                    RestoreCursor(cursorLocation.Left, intCursorLineOffset - corr);
                    OnLinesInsertedEvent(linesToInsert);
                }
            //}
        }

        public void Clear()
        {
            _consoleManager.Clear();            
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

        internal virtual void OnLineWrittenEvent(LineWrittenEventArgs e)
        {
            LineWrittenEvent?.Invoke(this, e);
        }

        protected virtual void OnKeyReadEvent(KeyReadEventArgs e)
        {
            KeyReadEvent?.Invoke(this, e);
        }

        private void WriteEx(string value)
        {
            //TODO: Duplicated code here and in WriteLineEx
            if (_consoleManager.ForegroundColor == _consoleManager.BackgroundColor)
            {
                _consoleManager.ForegroundColor = _consoleManager.ForegroundColor != ConsoleColor.Black ? ConsoleColor.Black : ConsoleColor.White;
            }

            _consoleManager?.Write(value);
        }

        //TODO: All outputs should go via this method! (Entry to this method is from different methods, should only be one, or)
        //protected internal virtual void WriteLineEx(string value, OutputLevel level)
        protected virtual void WriteLineEx(string value, OutputLevel level)
        {
            if (_consoleManager.ForegroundColor == _consoleManager.BackgroundColor)
            {
                _consoleManager.ForegroundColor = _consoleManager.ForegroundColor != ConsoleColor.Black ? ConsoleColor.Black : ConsoleColor.White;
            }

            var lines = value.Split('\n');
            foreach (var line in lines)
            {
                var r = line.Length % BufferWidth;
                if (r == 0) // && !string.IsNullOrEmpty(line))
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        _consoleManager?.WriteLine(line);
                    }
                    else
                    {
                        _consoleManager?.Write(line);
                    }
                }
                else
                {
                    _consoleManager?.WriteLine(line);
                }
            }
            OnLineWrittenEvent(new LineWrittenEventArgs(value, level));
        }

        private int MoveCursorUp()
        {
            try
            {
                var intCursorLineOffset = InputInstance.CursorLineOffset;
                SetCursorPosition(CursorLeft, CursorTop - intCursorLineOffset);
                return intCursorLineOffset;
            }
            catch (IOException exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
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
            catch (IOException exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
                return 0;
            }
            catch (DivideByZeroException exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
                return 1;
            }
        }

        private void RestoreCursor(int cursorLeft, int intCursorLineOffset)
        {
            try
            {
                SetCursorPosition(cursorLeft, CursorTop + intCursorLineOffset);
            }
            catch (IOException exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
            }
        }

        private Location MoveInputBufferDown(int linesToInsert, int inputBufferLines)
        {
            try
            {
                var cursorLeft = CursorLeft;
                var cursorTop = CursorTop;

                if (inputBufferLines + CursorTop + linesToInsert > _consoleManager.BufferHeight)
                {
                    MoveBufferArea(0, linesToInsert, BufferWidth, CursorTop - linesToInsert, 0, 0);
                    SetCursorPosition(0, cursorTop - linesToInsert);
                    OnLinesInsertedEvent(linesToInsert * -1);
                }
                else
                {
                    MoveBufferArea(0, CursorTop, BufferWidth, inputBufferLines, 0, CursorTop + linesToInsert);
                    SetCursorPosition(0, CursorTop);
                }

                return new Location(cursorLeft, cursorTop);
            }
            catch (IOException exception)
            {
                Trace.TraceError($"{exception.Message} @{exception.StackTrace}");
                return new Location(0, 0);
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

            lock (CommandEngine.SyncRoot)
            {
                if (output.LineFeed)
                {
                    WriteLine(message, output.OutputLevel, output.TextColor, output.TextBackgroundColor);
                }
                else
                {
                    Write(message, output.OutputLevel, output.TextColor, output.TextBackgroundColor);
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
            _textWriterInterceptor?.Dispose();
            _consoleManager?.Dispose();
            _errorInterceptor?.Dispose();
            _textReaderInterceptor?.Dispose();
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