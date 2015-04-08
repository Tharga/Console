using System;

namespace Tharga.Toolkit.Console.Command.Base
{
    internal class InputManager
    {
        private readonly CommandBase _commandBase;

        private readonly string _paramName;

        private readonly IConsole _console;

        public InputManager(CommandBase commandBase, string paramName)
        {
            this._commandBase = commandBase;
            this._console = this._commandBase.Console;
            this._paramName = paramName;
        }
        
        //public string ReadLine()
        //{
        //    _commandBase.Output(string.Format("{0}{1}", _paramName, _paramName.Length > 2 ? ": " : string.Empty), null, false);
        //    return _commandBase.Console.ReadLine();
        //}

        public string ReadLine()
        {
            var inputBuffer = new InputBuffer();

            _console.Write(string.Format("{0}{1}", this._paramName, this._paramName.Length > 2 ? ": " : string.Empty));
            var startLocation = new Location(_console.CursorLeft, _console.CursorTop);

            while (true)
            {
                var currentScreenLocation = new Location(_console.CursorLeft, _console.CursorTop);
                var currentBufferPosition = (currentScreenLocation.Top - startLocation.Top) * _console.BufferWidth + currentScreenLocation.Left - startLocation.Left;

                var readKey = this._console.ReadKey(true);

                if (readKey.KeyChar >= 32 && readKey.KeyChar <= 126)
                {
                    var charInput = readKey.KeyChar;
                    if (_console.CursorLeft < inputBuffer.Length + startLocation.Left)
                    {
                        //Push the end of the line forward.
                        _console.MoveBufferArea(
                            currentScreenLocation.Left,
                            currentScreenLocation.Top,
                            _console.BufferWidth - currentScreenLocation.Left,
                            1,
                            currentScreenLocation.Left + 1,
                            currentScreenLocation.Top);
                    }
                    _console.Write(charInput.ToString());
                    inputBuffer.Insert(currentBufferPosition, charInput);
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
                            MoveToStart(startLocation);
                            break;

                        case ConsoleKey.End:
                            MoveToEnd(startLocation, inputBuffer);
                            break;

                        case ConsoleKey.PageUp:
                        case ConsoleKey.PageDown:
                            //Ignore
                            break;

                        case ConsoleKey.DownArrow:
                        case ConsoleKey.UpArrow:
                            //TODO: If in prompt mode, toggle between previous commands
                            break;

                        case ConsoleKey.Delete:
                            if (currentBufferPosition == inputBuffer.Length) continue;
                            MoveBufferLeft(new Location(currentScreenLocation.Left+1, currentScreenLocation.Top), inputBuffer, startLocation);
                            inputBuffer.RemoveAt(currentBufferPosition);
                            break;

                        case ConsoleKey.Backspace:
                            if (currentBufferPosition == 0) continue;
                            MoveBufferLeft(currentScreenLocation, inputBuffer, startLocation);
                            inputBuffer.RemoveAt(currentBufferPosition - 1);
                            MoveCursorLeft();
                            break;

                        case ConsoleKey.Escape:
                            if (inputBuffer.IsEmpty)
                            {
                                //TODO: If within a command, break the entire command, exiting the command with 'false'. (Perhaps just throw)
                                continue;
                            }
                            MoveToStart(startLocation);
                            _console.Write(new string(' ', inputBuffer.Length));
                            MoveToStart(startLocation);
                            inputBuffer.Clear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException("Key " + readKey.Key + " is not handled.");
                    }
                }
            }
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
            while (pos > this._console.BufferWidth)
            {
                ln++;
                pos -= this._console.BufferWidth;
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