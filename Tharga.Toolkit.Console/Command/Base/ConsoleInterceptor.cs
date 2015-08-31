using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Tharga.Toolkit.Console.Command.Base
{
    internal class ConsoleInterceptor : TextWriter
    {
        private readonly TextWriter _consoleWriter;
        private readonly IConsole _console;
        private readonly object _syncRoot;
        private Location _location;

        public ConsoleInterceptor(TextWriter consoleWriter, IConsole console, object syncRoot)
        {
            _consoleWriter = consoleWriter;
            _console = console;
            _syncRoot = syncRoot;
            System.Console.SetOut(this);
        }

        public override Encoding Encoding
        {
            get
            {
                return _consoleWriter.Encoding;
            }
        }

        public override void WriteLine()
        {
            WriteStuff(string.Empty, true);
        }

        public override void WriteLine(string value)
        {
            WriteStuff(value, true);
        }

        public override void WriteLine(char value)
        {
            WriteStuff(value.ToString(), true);
        }

        public override void WriteLine(char[] buffer)
        {
            WriteStuff(new string(buffer), true);
        }

        public override void WriteLine(bool value)
        {
            WriteStuff(value.ToString(), true);
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            WriteStuff(new string(buffer, index, count), true);
        }

        public override void WriteLine(int value)
        {
            WriteStuff(value.ToString(), true);
        }

        public override void WriteLine(uint value)
        {
            WriteStuff(value.ToString(), true);
        }

        public override void WriteLine(long value)
        {
            WriteStuff(value.ToString(), true);
        }

        public override void WriteLine(ulong value)
        {
            WriteStuff(value.ToString(), true);
        }

        public override void WriteLine(float value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), true);
        }

        public override void WriteLine(double value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), true);
        }

        public override void WriteLine(decimal value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), true);
        }

        public override void WriteLine(object value)
        {
            WriteStuff(value.ToString(), true);
        }

        public override void WriteLine(string format, object arg0)
        {
            WriteStuff(string.Format(format, arg0), true);
        }

        public override void WriteLine(string format, object arg0, object arg1)
        {
            WriteStuff(string.Format(format, arg0, arg1), true);
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            WriteStuff(string.Format(format, arg0, arg1, arg2), true);
        }

        public override void WriteLine(string format, params object[] arg)
        {
            WriteStuff(string.Format(format, arg), true);
        }

        public override async Task WriteLineAsync()
        {
            throw new NotImplementedException();
        }

        public override async Task WriteLineAsync(char value)
        {
            throw new NotImplementedException();
        }

        public override async Task WriteLineAsync(string value)
        {
            throw new NotImplementedException();
        }

        public override async Task WriteLineAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override async Task WriteAsync(char value)
        {
            throw new NotImplementedException();
        }

        public override async Task WriteAsync(string value)
        {
            throw new NotImplementedException();
        }

        public override async Task WriteAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(string value)
        {
            WriteStuff(value, false);
        }

        private void WriteStuff(string value, bool lineFeed)
        {
            var outputLevel = OutputLevel.Default;

            if (_location != null)
            {
                lock (_syncRoot)
                {
                    //Continue to write on a known position
                    var lns = 0;
                    while ((lns * -System.Console.BufferWidth) + _location.Left + value.Length >= System.Console.BufferWidth)
                    {
                        lns++;
                        _console.WriteLine(string.Empty, outputLevel, null);
                    }

                    var pos = new Location(System.Console.CursorLeft, System.Console.CursorTop);

                    while (_location.Left > System.Console.BufferWidth)
                        _location = new Location(_location.Left - System.Console.BufferWidth, _location.Top + 1);

                    System.Console.CursorTop = _location.Top;
                    System.Console.CursorLeft = _location.Left;
    
                    _consoleWriter.Write(value);

                    _location = new Location(System.Console.CursorLeft, System.Console.CursorTop);

                    System.Console.CursorTop = pos.Top;
                    System.Console.CursorLeft = pos.Left;
                }

                if (lineFeed)
                    _location = null;
            }
            else if (!lineFeed)
            {
                if (_location == null)
                {
                    //First time a write arrives. assign a new line and remember position
                    _location = new Location(value.Length, System.Console.CursorTop);
                    _console.WriteLine(value, outputLevel, null);
                }
            }
            else
            {
                //There is no previous location, and there is linefeed. This is a normal write line action.
                _console.WriteLine(value, outputLevel, null);
            }
        }
    }
}