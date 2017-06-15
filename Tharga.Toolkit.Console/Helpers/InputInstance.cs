using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class InputInstance : IDisposable
    {
        private bool _finished;
        private readonly string _paramName;
        private readonly char? _passwordChar;

        private readonly IConsole _console;
        private static readonly Dictionary<string, List<string>> _commandHistory = new Dictionary<string, List<string>>();
        private int _commandHistoryIndex = -1;
        private Location _startLocation;
        private int _tabIndex = -1;

        private static int _currentBufferLineCount;
        private static int _cursorLineOffset;
        private InputBuffer _inputBuffer;
        private readonly CancellationToken _cancellationToken;

        //TODO: Theese values should be read from the instance, not as a static value!
        public static int CurrentBufferLineCount { get { return _currentBufferLineCount == 0 ? 1 : (_currentBufferLineCount + 1); } private set { _currentBufferLineCount = value; } }
        public static int CursorLineOffset { get { return _cursorLineOffset; } set { _cursorLineOffset = value; } }

        public InputInstance(IConsole console, string paramName, char? passwordChar, CancellationToken cancellationToken)
        {
            if (console == null) throw new ArgumentNullException(nameof(console), "No console provided.");

            _console = console;
            _paramName = paramName;
            _passwordChar = passwordChar;
            _cancellationToken = cancellationToken;

            _console.PushBufferDownEvent += PushBufferDownEvent;
            _console.LinesInsertedEvent += LinesInsertedEvent;
            _startLocation = new Location(CursorLeft, CursorTop);
        }

        private int CursorLeft => _console.CursorLeft;
        private int CursorTop => _console.CursorTop;
        private int BufferWidth => _console.BufferWidth;
        private int BufferHeight => _console.BufferHeight;

        private void SetCursorPosition(int left, int top)
        {
            _console.SetCursorPosition(left, top);
        }

        private void PushBufferDownEvent(object sender, PushBufferDownEventArgs e)
        {
            //TODO: Create e.LineCount free extra lines after the current buffer and move the cursor where it was before.

            //Step1: Remember cursor position
            var location = new Location(CursorLeft, CursorTop);

            //Step2: move the cursor to the end of the input buffer,
            //TODO: (Test by having text after the buffer, in one or several lines

            //Step3: Insert lines
            for (var i = 0; i < e.LineCount; i++)
                System.Console.WriteLine(string.Empty);

            //Step4: Move cursor back to position
            SetCursorPosition(location.Left, location.Top - 1);
        }

        private void LinesInsertedEvent(object sender, LinesInsertedEventArgs e)
        {
            lock (_startLocation)
            {
                var top = _startLocation.Top + e.LineCount;
                if (top >= BufferHeight) top = BufferHeight - 1;
                _startLocation = new Location(_startLocation.Left, top);
            }
        }

        public T ReadLine<T>(IEnumerable<CommandTreeNode<T>> selection, bool allowEscape)
        {
            var sel = selection?.ToArray() ?? new CommandTreeNode<T>[] {};

            _inputBuffer = new InputBuffer();
            _inputBuffer.InputBufferChangedEvent += InputBufferChangedEvent;

            _console.Output(new WriteEventArgs($"{_paramName}{(_paramName != Constants.Prompt ? ": " : string.Empty)}", OutputLevel.Default, null, null, false, false));
            lock (_startLocation)
            {
                _startLocation = new Location(CursorLeft, CursorTop);
            }

            while (true)
            {
                try
                {
                    var preEntryBuffWidth = BufferWidth;
                    var preEntryCursorLocationTop = CursorTop;
                    var preStartCursorLocationTop = _startLocation.Top;

                    var readKey = _console.ReadKey(_cancellationToken);
                    lock (CommandEngine.SyncRoot)
                    {
                        var currentScreenLocation = new Location(CursorLeft, CursorTop); //This is where the cursor actually is on screen.

                        //This code handle buffer resize on a running console.
                        if (preEntryBuffWidth != BufferWidth)
                        {
                            if (currentScreenLocation.Top != preEntryCursorLocationTop || _startLocation.Top != preStartCursorLocationTop)
                            {
                                lock (_startLocation)
                                {
                                    var pop1 = currentScreenLocation.Top - preEntryCursorLocationTop;
                                    var pop2 = preStartCursorLocationTop - _startLocation.Top;
                                    var top = _startLocation.Top + pop1 + pop2;
                                    if (top >= BufferHeight) top = BufferHeight - 1;
                                    _startLocation = new Location(_startLocation.Left, top);
                                }
                            }
                        }

                        var currentBufferPosition = ((currentScreenLocation.Top - _startLocation.Top) * BufferWidth) + currentScreenLocation.Left - _startLocation.Left;
                        //Debug.WriteLine($"cbp: {currentBufferPosition} = (({currentScreenLocation.Top} - {_startLocation.Top}) * {_console.BufferWidth}) + {currentScreenLocation.Left} - {_startLocation.Left}");
                        //System.Console.Title = $"cbp: {currentBufferPosition} = (({currentScreenLocation.Top} - {_startLocation.Top}) * {_console.BufferWidth}) + {currentScreenLocation.Left} - {_startLocation.Left}";
                        if (currentBufferPosition < 0)
                        {
                            throw new InvalidOperationException("Buffer insert position cannot be less than zero.");
                        }

                        if (IsOutputKey(readKey))
                        {
                            var input = readKey.KeyChar;
                            InsertText(currentScreenLocation, input, _inputBuffer, currentBufferPosition, _startLocation);
                        }
                        else if (readKey.Modifiers == ConsoleModifiers.Control)
                        {
                            switch (readKey.Key)
                            {
                                case ConsoleKey.V:
                                    var input = System.Windows.Clipboard.GetText().ToArray();
                                    foreach (var chr in input)
                                    {
                                        InsertText(currentScreenLocation, chr, _inputBuffer, currentBufferPosition, _startLocation);
                                        if (currentScreenLocation.Left == BufferWidth - 1)
                                            currentScreenLocation = new Location(0, currentScreenLocation.Top + 1);
                                        else
                                            currentScreenLocation = new Location(currentScreenLocation.Left + 1, currentScreenLocation.Top);
                                        currentBufferPosition++;
                                    }

                                    break;

                                case ConsoleKey.LeftArrow:
                                    if (currentBufferPosition > 0)
                                    {
                                        var leftOfCursor = _inputBuffer.ToString().Substring(0, currentBufferPosition).TrimEnd(' ');
                                        var last = leftOfCursor.LastIndexOf(' ');
                                        if (last != -1)
                                            SetCursorPosition(last + _startLocation.Left + 1, CursorTop);
                                        else
                                            SetCursorPosition(_startLocation.Left, CursorTop);
                                    }

                                    break;

                                case ConsoleKey.RightArrow:
                                    var l2 = _inputBuffer.ToString().IndexOf(' ', currentBufferPosition);
                                    if (l2 != -1)
                                    {
                                        while (_inputBuffer.ToString().Length > l2 + 1 && _inputBuffer.ToString()[l2 + 1] == ' ')
                                            l2++;
                                        SetCursorPosition(l2 + _startLocation.Left + 1, CursorTop);
                                    }
                                    else
                                    {
                                        SetCursorPosition(_inputBuffer.ToString().Length + _startLocation.Left, CursorTop);
                                    }

                                    break;

                                default:
                                    Trace.TraceWarning("No action for ctrl-" + readKey.Key);
                                    break;
                            }
                        }
                        else
                        {
                            switch (readKey.Key)
                            {
                                case ConsoleKey.Enter:
                                    return Enter(sel);

                                case ConsoleKey.LeftArrow:
                                    if (currentBufferPosition == 0) continue;
                                    MoveCursorLeft();
                                    break;

                                case ConsoleKey.RightArrow:
                                    if (currentBufferPosition == _inputBuffer.Length) continue;
                                    MoveCursorRight();
                                    break;

                                case ConsoleKey.Home:
                                    MoveCursorToStart(_startLocation);
                                    break;

                                case ConsoleKey.End:
                                    MoveCursorToEnd(_startLocation, _inputBuffer);
                                    break;

                                case ConsoleKey.DownArrow:
                                case ConsoleKey.UpArrow:
                                    RecallCommandHistory(readKey, _inputBuffer);
                                    break;

                                case ConsoleKey.Delete:
                                    if (currentBufferPosition == _inputBuffer.Length) continue;
                                    MoveBufferLeft(new Location(currentScreenLocation.Left + 1, currentScreenLocation.Top), _inputBuffer, _startLocation);
                                    _inputBuffer.RemoveAt(currentBufferPosition);
                                    CurrentBufferLineCount = (int)Math.Ceiling((decimal)(_inputBuffer.Length - BufferWidth + _startLocation.Left + 1) / BufferWidth);
                                    break;

                                case ConsoleKey.Backspace:
                                    if (currentBufferPosition == 0) continue;
                                    MoveBufferLeft(currentScreenLocation, _inputBuffer, _startLocation);
                                    _inputBuffer.RemoveAt(currentBufferPosition - 1);
                                    MoveCursorLeft();
                                    CurrentBufferLineCount = (int)Math.Ceiling((decimal)(_inputBuffer.Length - BufferWidth + _startLocation.Left + 1) / BufferWidth);
                                    break;

                                case ConsoleKey.Escape:
                                    if (_inputBuffer.IsEmpty && allowEscape)
                                    {
                                        _console.Output(new WriteEventArgs(null));
                                        throw new CommandEscapeException();
                                    }

                                    Clear(_inputBuffer);
                                    break;

                                case ConsoleKey.Tab:
                                    if (sel.Any())
                                    {
                                        //Go to the next item by using the input buffer
                                        if (_tabIndex == -1)
                                        {
                                            var enumerable = sel.Select(x => x.Value).ToList();
                                            var firstHit = enumerable.FirstOrDefault(x => x.StartsWith(_inputBuffer.ToString(), StringComparison.InvariantCultureIgnoreCase));
                                            if (firstHit != null)
                                                _tabIndex = enumerable.IndexOf(firstHit) - 1;
                                        }

                                        var step = 1;
                                        if (readKey.Modifiers == ConsoleModifiers.Shift)
                                            step = -1;

                                        var tabIndex = _tabIndex + step;
                                        if (tabIndex == sel.Length) tabIndex = 0;
                                        if (tabIndex <= -1) tabIndex = sel.Length - 1;
                                        Clear(_inputBuffer);
                                        _console.Output(new WriteEventArgs(sel[tabIndex].Value, OutputLevel.Default, null, null, false, false));
                                        _inputBuffer.Add(sel[tabIndex].Value);
                                        _tabIndex = tabIndex;
                                        CurrentBufferLineCount = (int)Math.Ceiling((decimal)(_inputBuffer.Length - BufferWidth + _startLocation.Left + 1) / BufferWidth);
                                    }

                                    break;

                                case ConsoleKey.PageUp:
                                case ConsoleKey.PageDown:
                                case ConsoleKey.LeftWindows:
                                case ConsoleKey.RightWindows:
                                case ConsoleKey.Applications:
                                case ConsoleKey.Insert:
                                case ConsoleKey.F1:
                                case ConsoleKey.F2:
                                case ConsoleKey.F3:
                                case ConsoleKey.F4:
                                case ConsoleKey.F5:
                                case ConsoleKey.F6:
                                case ConsoleKey.F7:
                                case ConsoleKey.F8:
                                case ConsoleKey.F9:
                                case ConsoleKey.F10:
                                case ConsoleKey.F11:
                                case ConsoleKey.F12:
                                case ConsoleKey.F13:
                                case ConsoleKey.Oem1:
                                    //Ignore
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException($"Key {readKey.Key} is not handled ({readKey.KeyChar}).");
                            }
                        }

                        CursorLineOffset = CursorTop - _startLocation.Top;
                    }
                }
                catch (OperationCanceledException exception)
                {
                    Trace.TraceWarning(exception.Message);
                    //NOTE: Operation was cancelled, causing what ever is in the buffer to be returned, or, should an empty selection be returned?
                    return Enter(sel);
                }
                catch (CommandEscapeException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Trace.TraceError(exception.Message);
                    _console.OutputError(exception);
                }
            }
        }

        private T Enter<T>(CommandTreeNode<T>[] selection)
        {
            var response = GetResponse(selection, _inputBuffer);
            RememberCommandHistory(_inputBuffer);
            return response;
        }

        private bool IsOutputKey(ConsoleKeyInfo readKey)
        {
            if (readKey.Modifiers == ConsoleModifiers.Control)
                return false;

            switch (readKey.Key)
            {
                case ConsoleKey.Enter:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                case ConsoleKey.Home:
                case ConsoleKey.End:
                case ConsoleKey.DownArrow:
                case ConsoleKey.UpArrow:
                case ConsoleKey.Delete:
                case ConsoleKey.Backspace:
                case ConsoleKey.Escape:
                case ConsoleKey.Tab:
                case ConsoleKey.PageUp:
                case ConsoleKey.PageDown:
                case ConsoleKey.LeftWindows:
                case ConsoleKey.RightWindows:
                case ConsoleKey.Applications:
                case ConsoleKey.Insert:
                case ConsoleKey.F1:
                case ConsoleKey.F2:
                case ConsoleKey.F3:
                case ConsoleKey.F4:
                case ConsoleKey.F5:
                case ConsoleKey.F6:
                case ConsoleKey.F7:
                case ConsoleKey.F8:
                case ConsoleKey.F9:
                case ConsoleKey.F10:
                case ConsoleKey.F11:
                case ConsoleKey.F12:
                case ConsoleKey.F13:
                    return false;
                case ConsoleKey.Oem1: //This is the : key on an english keyboard
                    return true;
                default:
                    return true;
            }
        }

        private void RecallCommandHistory(ConsoleKeyInfo readKey, InputBuffer inputBuffer)
        {
            if (_commandHistory.ContainsKey(_paramName) && _passwordChar == null)
            {
                var chi = GetNextCommandHistoryIndex(readKey, _commandHistoryIndex);

                Clear(inputBuffer);
                _commandHistoryIndex = chi;
                _console.Output(new WriteEventArgs(_commandHistory[_paramName][_commandHistoryIndex], OutputLevel.Default, null, null, false, false));
                inputBuffer.Add(_commandHistory[_paramName][_commandHistoryIndex]);
            }
        }

        private int GetNextCommandHistoryIndex(ConsoleKeyInfo readKey, int commandHistoryIndex)
        {
            if (commandHistoryIndex == -1)
            {
                if (readKey.Key == ConsoleKey.UpArrow)
                {
                    commandHistoryIndex = _commandHistory[_paramName].Count - 1;
                }
                else
                {
                    commandHistoryIndex = 0;
                }
            }
            else if (readKey.Key == ConsoleKey.UpArrow)
            {
                commandHistoryIndex++;
                if (commandHistoryIndex == _commandHistory[_paramName].Count)
                {
                    commandHistoryIndex = 0;
                }
            }
            else if (readKey.Key == ConsoleKey.DownArrow)
            {
                commandHistoryIndex--;
                if (commandHistoryIndex < 0)
                {
                    commandHistoryIndex = _commandHistory[_paramName].Count - 1;
                }
            }

            return commandHistoryIndex;
        }

        private void RememberCommandHistory(InputBuffer inputBuffer)
        {
            if (inputBuffer.Length == 0)
            {
                return;
            }

            if (!_commandHistory.ContainsKey(_paramName))
            {
                _commandHistory.Add(_paramName, new List<string>());
            }

            var inputString = inputBuffer.ToString();

            _commandHistory[_paramName].RemoveAll(x => string.Compare(inputString, x, StringComparison.InvariantCulture) == 0);
            _commandHistory[_paramName].Add(inputString);
        }

        private void Clear(InputBuffer inputBuffer)
        {
            _commandHistoryIndex = -1;
            MoveCursorToStart(_startLocation);
            _console.Output(new WriteEventArgs(new string(' ', inputBuffer.Length), OutputLevel.Default, null, null, false, false));
            MoveCursorToStart(_startLocation);
            inputBuffer.Clear();
            CurrentBufferLineCount = (int)Math.Ceiling((decimal)(inputBuffer.Length - BufferWidth + _startLocation.Left + 1) / BufferWidth);
        }

        private T GetResponse<T>(CommandTreeNode<T>[] selection, InputBuffer inputBuffer)
        {
            if (_finished) throw new InvalidOperationException("Cannot get response more than once from a single input manager.");

            T response;
            if (selection.Any())
            {
                if (_tabIndex != -1)
                {
                    _console.Output(new WriteEventArgs(null, OutputLevel.Default));
                    response = selection[_tabIndex].Key;
                }
                else
                {
                    var items = selection.Where(x => x.Value == inputBuffer.ToString()).ToArray();
                    if (!items.Any())
                    {
                        try
                        {
                            response = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(inputBuffer.ToString());
                            _console.Output(new WriteEventArgs(null, OutputLevel.Default));
                        }
                        catch (FormatException exception)
                        {
                            throw new EntryException("No item match the entry.", exception);
                        }
                    }
                    else
                    {
                        if (items.Count() > 1)
                        {
                            throw new EntryException("There are several matches to the entry.");
                        }

                        _console.Output(new WriteEventArgs(null, OutputLevel.Default));
                        response = items.Single().Key;
                    }
                }
            }
            else
            {
                response = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(inputBuffer.ToString());
                _console.Output(new WriteEventArgs(null, OutputLevel.Default));
            }

            _finished = true;
            return response;
        }

        private void InputBufferChangedEvent(object sender, InputBufferChangedEventArgs e)
        {
            _tabIndex = -1;
        }

        private void InsertText(Location currentScreenLocation, char input, InputBuffer inputBuffer, int currentBufferPosition, Location startLocation)
        {
            if (BufferWidth <= 1) throw new ArgumentException("BufferWidth needs to be larger than 1.");

            //Check if the text to the right is on more than one line
            var charsToTheRight = inputBuffer.Length - currentBufferPosition;
            var bufferToTheRight = BufferWidth - currentScreenLocation.Left - startLocation.Left + 1;
            if (charsToTheRight > bufferToTheRight)
            {
                var lines = (int)Math.Ceiling((decimal)(inputBuffer.Length - BufferWidth + startLocation.Left + 1) / BufferWidth);
                for (var i = lines; i > 0; i--)
                {
                    _console.MoveBufferArea(0, currentScreenLocation.Top + i - 1 + 1, BufferWidth - 1, 1, 1, currentScreenLocation.Top + i - 1 + 1);
                    _console.MoveBufferArea(BufferWidth - 1, currentScreenLocation.Top + i - 1, 1, 1, 0, currentScreenLocation.Top + i - 1 + 1);
                }
            }

            _console.MoveBufferArea(currentScreenLocation.Left, currentScreenLocation.Top, BufferWidth - currentScreenLocation.Left, 1, currentScreenLocation.Left + 1, currentScreenLocation.Top);
            if (input == 9)
            {
                _console.Output(new WriteEventArgs(((char)26).ToString(CultureInfo.InvariantCulture), OutputLevel.Default, null, null, false, false));
            }
            else
            {
                _console.Output(new WriteEventArgs(_passwordChar?.ToString() ?? input.ToString(), OutputLevel.Default, null, null, false, false));
            }

            inputBuffer.Insert(currentBufferPosition, input.ToString(CultureInfo.InvariantCulture));
            CurrentBufferLineCount = (int)Math.Ceiling((decimal)(inputBuffer.Length - BufferWidth + _startLocation.Left + 1) / BufferWidth);
        }

        private void MoveBufferLeft(Location currentScreenLocation, InputBuffer inputBuffer, Location startLocation)
        {
            _console.MoveBufferArea(currentScreenLocation.Left, currentScreenLocation.Top, BufferWidth - currentScreenLocation.Left, 1, currentScreenLocation.Left - 1, currentScreenLocation.Top);

            var done = BufferWidth - startLocation.Left;
            var line = 1;
            while (inputBuffer.Length >= done)
            {
                _console.MoveBufferArea(0, currentScreenLocation.Top + line, 1, 1, BufferWidth - 1, currentScreenLocation.Top + line - 1);
                _console.MoveBufferArea(1, currentScreenLocation.Top + line, BufferWidth - 1, 1, 0, currentScreenLocation.Top + line);

                done += BufferWidth;
                line++;
            }
        }

        private void MoveCursorToStart(Location startLocation)
        {
            SetCursorPosition(startLocation.Left, startLocation.Top);
        }

        private void MoveCursorToEnd(Location startLocation, InputBuffer inputBuffer)
        {
            var pos = startLocation.Left + inputBuffer.Length;
            var ln = 0;
            while (pos > BufferWidth)
            {
                ln++;
                pos -= BufferWidth;
            }

            SetCursorPosition(pos, startLocation.Top + ln);
        }

        private void MoveCursorRight()
        {
            if (CursorLeft == BufferWidth - 1)
            {
                SetCursorPosition(0, CursorTop + 1);
            }
            else
            {
                SetCursorPosition(CursorLeft + 1, CursorTop);
            }
        }

        private void MoveCursorLeft()
        {
            if (CursorLeft == 0)
            {
                SetCursorPosition(BufferWidth - 1, CursorTop - 1);
            }
            else
            {
                SetCursorPosition(CursorLeft - 1, CursorTop);
            }
        }

        public void Cancel()
        {
            throw new NotImplementedException();
            //_cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            _console?.Dispose();
            //_inputBuffer?.Dispose();
        }
    }
}