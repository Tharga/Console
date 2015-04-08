using System;
using System.Globalization;

namespace Tharga.Toolkit.Console.Command.Base
{
    internal class InputManager
    {
        private readonly CommandBase _commandBase;
        private readonly string _paramName;
        private readonly IConsole _console;

        private Location _startLocation;

        public InputManager(CommandBase commandBase, string paramName)
        {
            _commandBase = commandBase;
            _console = _commandBase.Console;
            _console.LinesInsertedEvent += Console_LinesInsertedEvent;
            _paramName = paramName;
        }

        private void Console_LinesInsertedEvent(object sender, LinesInsertedEventArgs e)
        {
            _startLocation = new Location(_startLocation.Left, _startLocation.Top + e.LineCount);
        }

        ////public string ReadLine()
        ////{
        ////    _commandBase.Output(string.Format("{0}{1}", _paramName, _paramName.Length > 2 ? ": " : string.Empty), null, false);
        ////    return _commandBase.Console.ReadLine();
        ////}

        public string ReadLine()
        {
            var inputBuffer = new InputBuffer();

            _console.Write(string.Format("{0}{1}", _paramName, _paramName.Length > 2 ? ": " : string.Empty));
            _startLocation = new Location(_console.CursorLeft, _console.CursorTop);

            while (true)
            {
                try
                {
                    var readKey = _console.ReadKey(true);

                    var currentScreenLocation = new Location(_console.CursorLeft, _console.CursorTop);
                    var currentBufferPosition = ((currentScreenLocation.Top - _startLocation.Top) * _console.BufferWidth) + currentScreenLocation.Left - _startLocation.Left;

                    if ((readKey.KeyChar >= 32 && readKey.KeyChar <= 126) || readKey.Key == ConsoleKey.Oem5)
                    {
                        var input = readKey.KeyChar.ToString(CultureInfo.InvariantCulture);
                        InsertText(currentScreenLocation, input, inputBuffer, currentBufferPosition, _startLocation);
                    }
                    else if (readKey.Modifiers == ConsoleModifiers.Control)
                    {
                        switch (readKey.Key)
                        {
                            case ConsoleKey.V:
                                //TODO: Invoke this on the correct thread.
                                var input = System.Windows.Clipboard.GetText();
                                InsertText(currentScreenLocation, input, inputBuffer, currentBufferPosition, _startLocation);
                                break;

                            case ConsoleKey.LeftArrow:
                                //TODO:
                                System.Diagnostics.Debug.WriteLine("Jump back to previous whitespace");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(string.Format("Ctrl feature not handled for {0} ({1}).", readKey.Key, readKey.KeyChar));
                        }
                    }
                    else
                    {
                        switch (readKey.Key)
                        {
                            case ConsoleKey.Enter:
                                _console.NewLine();
                                return inputBuffer.ToString();

                            case ConsoleKey.LeftArrow:
                                if (currentBufferPosition == 0) continue;
                                MoveCursorLeft();
                                break;

                            case ConsoleKey.RightArrow:
                                if (currentBufferPosition == inputBuffer.Length) continue;
                                MoveCursorRight();
                                break;

                            case ConsoleKey.Home:
                                MoveToStart(_startLocation);
                                break;

                            case ConsoleKey.End:
                                MoveToEnd(_startLocation, inputBuffer);
                                break;

                            case ConsoleKey.DownArrow:
                            case ConsoleKey.UpArrow:
                                //TODO: If in prompt mode, toggle between previous commands
                                break;

                            case ConsoleKey.Delete:
                                if (currentBufferPosition == inputBuffer.Length) continue;
                                MoveBufferLeft(new Location(currentScreenLocation.Left + 1, currentScreenLocation.Top), inputBuffer, _startLocation);
                                inputBuffer.RemoveAt(currentBufferPosition);
                                break;

                            case ConsoleKey.Backspace:
                                if (currentBufferPosition == 0) continue;
                                MoveBufferLeft(currentScreenLocation, inputBuffer, _startLocation);
                                inputBuffer.RemoveAt(currentBufferPosition - 1);
                                MoveCursorLeft();
                                break;

                            case ConsoleKey.Escape:
                                if (inputBuffer.IsEmpty)
                                {
                                    //TODO: If within a command, break the entire command, exiting the command with 'false'. (Perhaps just throw)
                                    continue;
                                }

                                MoveToStart(_startLocation);
                                _console.Write(new string(' ', inputBuffer.Length));
                                MoveToStart(_startLocation);
                                inputBuffer.Clear();
                                break;

                            case ConsoleKey.PageUp:
                            case ConsoleKey.PageDown:
                            case ConsoleKey.LeftWindows:
                            case ConsoleKey.RightWindows:
                            case ConsoleKey.Applications:
                            case ConsoleKey.Insert:
                                //Ignore
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(
                                    "Key " + readKey.Key + " is not handled (" + readKey.KeyChar + ").");
                        }
                    }
                }
                catch (Exception exception)
                {
                    _commandBase.OutputError(exception.Message);
                }
            }
        }

        private void InsertText(Location currentScreenLocation, string input, InputBuffer inputBuffer, int currentBufferPosition, Location startLocation)
        {
            //Check if the text to the right is on more than one line
            var charsToTheRight = inputBuffer.Length - currentBufferPosition;
            var bufferToTheRight = _console.BufferWidth - currentScreenLocation.Left - startLocation.Left + 1;
            if (charsToTheRight > bufferToTheRight)
            {
                var lines = (int)Math.Ceiling((decimal)(inputBuffer.Length - _console.BufferWidth + startLocation.Left + 1) / _console.BufferWidth);
                for (var i = lines; i > 0; i--)
                {
                    _console.MoveBufferArea(0, currentScreenLocation.Top + i - 1 + 1, _console.BufferWidth - input.Length, 1, input.Length, currentScreenLocation.Top + i - 1 + 1);
                    _console.MoveBufferArea(_console.BufferWidth - 1, currentScreenLocation.Top + i - 1, input.Length, 1, 0, currentScreenLocation.Top + i - 1 + 1);
                }
            }

            _console.MoveBufferArea(currentScreenLocation.Left, currentScreenLocation.Top, _console.BufferWidth - currentScreenLocation.Left, 1, currentScreenLocation.Left + input.Length, currentScreenLocation.Top);
            _console.Write(input);
            inputBuffer.Insert(currentBufferPosition, input);
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

        private void MoveToStart(Location startLocation)
        {
            _console.CursorLeft = startLocation.Left;
            _console.CursorTop = startLocation.Top;
        }

        private void MoveToEnd(Location startLocation, InputBuffer inputBuffer)
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