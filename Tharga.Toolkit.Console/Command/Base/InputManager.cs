using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Tharga.Toolkit.Console.Command.Base
{    
    internal class InputManager
    {
        private readonly ICommandBase _commandBase;
        private readonly string _paramName;
        private readonly IConsole _console;
        private static readonly Dictionary<string, List<string>> _commandHistory = new Dictionary<string, List<string>>();
        private int _commandHistoryIndex = -1;
        private Location _startLocation;
        private int _tabIndex = -1;

        //TODO: Theese two properties are the uggliest thing. What can I do to remove them?
        private static int _currentBufferLineCount;
        private static int _cursorLineOffset;
        public static int CurrentBufferLineCount { get { return _currentBufferLineCount == 0 ? 1 : (_currentBufferLineCount + 1); } private set { _currentBufferLineCount = value; } }
        public static int CursorLineOffset { get { return _cursorLineOffset; } set { _cursorLineOffset = value; } }

        public InputManager(IConsole console, ICommandBase commandBase, string paramName)
        {
            _commandBase = commandBase;
            _console = console;
            _console.LinesInsertedEvent += LinesInsertedEvent;
            _paramName = paramName;
            _startLocation = new Location(_console.CursorLeft, _console.CursorTop);
        }

        private void LinesInsertedEvent(object sender, LinesInsertedEventArgs e)
        {
            _startLocation = new Location(_startLocation.Left, _startLocation.Top + e.LineCount);
        }

        //TODO: Test this function        
        public T ReadLine<T>(KeyValuePair<T, string>[] selection, bool allowEscape)
        {
            var inputBuffer = new InputBuffer();
            inputBuffer.InputBufferChangedEvent += InputBufferChangedEvent;

            _console.Write(string.Format("{0}{1}", _paramName, _paramName.Length > 2 ? ": " : string.Empty));
            _startLocation = new Location(_console.CursorLeft, _console.CursorTop);

            while (true)
            {
                try
                {
                    var readKey = _console.ReadKey(true);

                    var currentScreenLocation = new Location(_console.CursorLeft, _console.CursorTop);
                    var currentBufferPosition = ((currentScreenLocation.Top - _startLocation.Top) * _console.BufferWidth) + currentScreenLocation.Left - _startLocation.Left;

                    if (IsOutputKey(readKey))
                    {
                        var input = readKey.KeyChar;
                        InsertText(currentScreenLocation, input, inputBuffer, currentBufferPosition, _startLocation);
                    }
                    else if (readKey.Modifiers == ConsoleModifiers.Control)
                    {
                        switch (readKey.Key)
                        {
                            case ConsoleKey.V:
                                var input = System.Windows.Clipboard.GetText().ToArray();
                                foreach (var chr in input)
                                {
                                    InsertText(currentScreenLocation, chr, inputBuffer, currentBufferPosition, _startLocation);
                                    if (currentScreenLocation.Left == _console.BufferWidth - 1)
                                        currentScreenLocation = new Location(0, currentScreenLocation.Top + 1);
                                    else
                                        currentScreenLocation = new Location(currentScreenLocation.Left + 1, currentScreenLocation.Top);
                                    currentBufferPosition++;
                                }

                                break;

                            case ConsoleKey.LeftArrow:
                                if (currentBufferPosition > 0)
                                {
                                    var leftOfCursor = inputBuffer.ToString().Substring(0, currentBufferPosition).TrimEnd(' ');
                                    var last = leftOfCursor.LastIndexOf(' ');
                                    if (last != -1)
                                        _console.CursorLeft = last + _startLocation.Left + 1;
                                    else
                                        _console.CursorLeft = _startLocation.Left;
                                }

                                break;

                            case ConsoleKey.RightArrow:

                                var l2 = inputBuffer.ToString().IndexOf(' ', currentBufferPosition);
                                if (l2 != -1)
                                {
                                    while (inputBuffer.ToString().Length > l2 + 1 && inputBuffer.ToString()[l2 + 1] == ' ')
                                        l2++;
                                    _console.CursorLeft = l2 + _startLocation.Left + 1;
                                }
                                else
                                    _console.CursorLeft = inputBuffer.ToString().Length + _startLocation.Left;

                                break;

                            default:
                                System.Diagnostics.Debug.WriteLine("No action for ctrl-" + readKey.Key);
                                break;
                        }
                    }
                    else
                    {
                        switch (readKey.Key)
                        {
                            case ConsoleKey.Enter:
                                var response = GetResponse(selection, inputBuffer);
                                RememberCommandHistory(inputBuffer);

                                return response;

                            case ConsoleKey.LeftArrow:
                                if (currentBufferPosition == 0) continue;
                                MoveCursorLeft();
                                break;

                            case ConsoleKey.RightArrow:
                                if (currentBufferPosition == inputBuffer.Length) continue;
                                MoveCursorRight();
                                break;

                            case ConsoleKey.Home:
                                MoveCursorToStart(_startLocation);
                                break;

                            case ConsoleKey.End:
                                MoveCursorToEnd(_startLocation, inputBuffer);
                                break;

                            case ConsoleKey.DownArrow:
                            case ConsoleKey.UpArrow:
                                RecallCommandHistory(readKey, inputBuffer);
                                break;

                            case ConsoleKey.Delete:
                                if (currentBufferPosition == inputBuffer.Length) continue;
                                MoveBufferLeft(new Location(currentScreenLocation.Left + 1, currentScreenLocation.Top), inputBuffer, _startLocation);
                                inputBuffer.RemoveAt(currentBufferPosition);
                                CurrentBufferLineCount = (int)Math.Ceiling((decimal)(inputBuffer.Length - _console.BufferWidth + _startLocation.Left + 1) / _console.BufferWidth);
                                break;

                            case ConsoleKey.Backspace:
                                if (currentBufferPosition == 0) continue;
                                MoveBufferLeft(currentScreenLocation, inputBuffer, _startLocation);
                                inputBuffer.RemoveAt(currentBufferPosition - 1);
                                MoveCursorLeft();
                                CurrentBufferLineCount = (int)Math.Ceiling((decimal)(inputBuffer.Length - _console.BufferWidth + _startLocation.Left + 1) / _console.BufferWidth);
                                break;

                            case ConsoleKey.Escape:
                                if (inputBuffer.IsEmpty && allowEscape)
                                {
                                    _console.NewLine();
                                    throw new CommandEscapeException();
                                }

                                Clear(inputBuffer);
                                break;

                            case ConsoleKey.Tab:
                                if (selection.Any())
                                {
                                    var tabIndex = _tabIndex + 1;
                                    if (tabIndex == selection.Length) tabIndex = 0;
                                    Clear(inputBuffer);
                                    _console.Write(selection[tabIndex].Value);
                                    inputBuffer.Add(selection[tabIndex].Value);
                                    _tabIndex = tabIndex;
                                    CurrentBufferLineCount = (int)Math.Ceiling((decimal)(inputBuffer.Length - _console.BufferWidth + _startLocation.Left + 1) / _console.BufferWidth);
                                }
                                else
                                {
                                    InsertText(currentScreenLocation, (char)9, inputBuffer, currentBufferPosition, _startLocation);
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
                                throw new ArgumentOutOfRangeException(string.Format("Key {0} is not handled ({1}).", readKey.Key, readKey.KeyChar));
                        }
                    }

                    CursorLineOffset = _console.CursorTop - _startLocation.Top;
                }
                catch (CommandEscapeException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    _commandBase.OutputError("{0}", exception.Message);
                    foreach (DictionaryEntry data in exception.Data)
                    {
                        _commandBase.OutputError("- {0}: {1}", data.Key, data.Value);
                    }
                }
            }
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
                case ConsoleKey.Oem1:
                    return false;
                default:
                    return true;
            }
        }

        private void RecallCommandHistory(ConsoleKeyInfo readKey, InputBuffer inputBuffer)
        {
            if (_commandHistory.ContainsKey(_paramName))
            {
                var chi = GetNextCommandHistoryIndex(readKey, _commandHistoryIndex);

                Clear(inputBuffer);
                _commandHistoryIndex = chi;
                _console.Write(_commandHistory[_paramName][_commandHistoryIndex]);
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
            _console.Write(new string(' ', inputBuffer.Length));
            MoveCursorToStart(_startLocation);
            inputBuffer.Clear();
            CurrentBufferLineCount = (int)Math.Ceiling((decimal)(inputBuffer.Length - _console.BufferWidth + _startLocation.Left + 1) / _console.BufferWidth);
        }

        private T GetResponse<T>(KeyValuePair<T, string>[] selection, InputBuffer inputBuffer)
        {
            T response;
            if (selection.Any())
            {
                if (_tabIndex != -1)
                {
                    _console.NewLine();
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
                            _console.NewLine();
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

                        _console.NewLine();
                        response = items.Single().Key;
                    }
                }
            }
            else
            {
                response = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(inputBuffer.ToString());
                _console.NewLine();
            }

            return response;
        }

        private void InputBufferChangedEvent(object sender, InputBufferChangedEventArgs e)
        {
            _tabIndex = -1;
        }

        private void InsertText(Location currentScreenLocation, char input, InputBuffer inputBuffer, int currentBufferPosition, Location startLocation)
        {
            //Check if the text to the right is on more than one line
            var charsToTheRight = inputBuffer.Length - currentBufferPosition;
            var bufferToTheRight = _console.BufferWidth - currentScreenLocation.Left - startLocation.Left + 1;
            if (charsToTheRight > bufferToTheRight)
            {
                var lines = (int)Math.Ceiling((decimal)(inputBuffer.Length - _console.BufferWidth + startLocation.Left + 1) / _console.BufferWidth);
                for (var i = lines; i > 0; i--)
                {
                    _console.MoveBufferArea(0, currentScreenLocation.Top + i - 1 + 1, _console.BufferWidth - 1, 1, 1, currentScreenLocation.Top + i - 1 + 1);
                    _console.MoveBufferArea(_console.BufferWidth - 1, currentScreenLocation.Top + i - 1, 1, 1, 0, currentScreenLocation.Top + i - 1 + 1);
                }
            }

            _console.MoveBufferArea(currentScreenLocation.Left, currentScreenLocation.Top, _console.BufferWidth - currentScreenLocation.Left, 1, currentScreenLocation.Left + 1, currentScreenLocation.Top);
            if (input == 9)
            {
                _console.Write(((char)26).ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                _console.Write(input.ToString());
            }

            inputBuffer.Insert(currentBufferPosition, input.ToString(CultureInfo.InvariantCulture));
            CurrentBufferLineCount = (int)Math.Ceiling((decimal)(inputBuffer.Length - _console.BufferWidth + _startLocation.Left + 1) / _console.BufferWidth);
        }

        private void MoveBufferLeft(Location currentScreenLocation, InputBuffer inputBuffer, Location startLocation)
        {
            _console.MoveBufferArea(currentScreenLocation.Left, currentScreenLocation.Top, _console.BufferWidth - currentScreenLocation.Left, 1, currentScreenLocation.Left - 1, currentScreenLocation.Top);

            var done = _console.BufferWidth - startLocation.Left;
            var line = 1;
            while (inputBuffer.Length >= done)
            {
                _console.MoveBufferArea(0, currentScreenLocation.Top + line, 1, 1, _console.BufferWidth - 1, currentScreenLocation.Top + line - 1);
                _console.MoveBufferArea(1, currentScreenLocation.Top + line, _console.BufferWidth - 1, 1, 0, currentScreenLocation.Top + line);

                done += _console.BufferWidth;
                line++;
            }
        }

        private void MoveCursorToStart(Location startLocation)
        {
            _console.CursorLeft = startLocation.Left;
            _console.CursorTop = startLocation.Top;
        }

        private void MoveCursorToEnd(Location startLocation, InputBuffer inputBuffer)
        {
            var pos = startLocation.Left + inputBuffer.Length;
            var ln = 0;
            while (pos > _console.BufferWidth)
            {
                ln++;
                pos -= _console.BufferWidth;
            }

            _console.CursorLeft = pos;
            _console.CursorTop = startLocation.Top + ln;
        }

        private void MoveCursorRight()
        {
            if (_console.CursorLeft == _console.BufferWidth - 1)
            {
                _console.CursorTop++;
                _console.CursorLeft = 0;
            }
            else
            {
                _console.CursorLeft++;
            }
        }

        private void MoveCursorLeft()
        {
            if (_console.CursorLeft == 0)
            {
                _console.CursorTop--;
                _console.CursorLeft = _console.BufferWidth - 1;
            }
            else
            {
                _console.CursorLeft--;
            }
        }
    }
}