using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class TextWriterInterceptor : TextWriter
    {
        private readonly IConsoleManager _consoleWriter;
        private readonly IConsole _console;

        public TextWriterInterceptor(IConsoleManager consoleWriter, IConsole console)
        {
            _consoleWriter = consoleWriter;
            _console = console;
            System.Console.SetOut(this);
        }

        //protected TextWriter(IFormatProvider formatProvider);

        public override Encoding Encoding => _consoleWriter.Encoding;

        //public override void WriteLine()
        //{
        //    WriteStuff(string.Empty, true);
        //}

        //public override void WriteLine(string value)
        //{
        //    WriteStuff(value, true);
        //}

        //public override void WriteLine(char value)
        //{
        //    WriteStuff(value.ToString(), true);
        //}

        //public override void WriteLine(char[] buffer)
        //{
        //    WriteStuff(new string(buffer), true);
        //}

        //public override void WriteLine(bool value)
        //{
        //    WriteStuff(value.ToString(), true);
        //}

        //public override void WriteLine(char[] buffer, int index, int count)
        //{
        //    WriteStuff(new string(buffer, index, count), true);
        //}

        //public override void WriteLine(int value)
        //{
        //    WriteStuff(value.ToString(), true);
        //}

        //public override void WriteLine(uint value)
        //{
        //    WriteStuff(value.ToString(), true);
        //}

        //public override void WriteLine(long value)
        //{
        //    WriteStuff(value.ToString(), true);
        //}

        //public override void WriteLine(ulong value)
        //{
        //    WriteStuff(value.ToString(), true);
        //}

        //public override void WriteLine(float value)
        //{
        //    WriteStuff(value.ToString(CultureInfo.InvariantCulture), true);
        //}

        //public override void WriteLine(double value)
        //{
        //    WriteStuff(value.ToString(CultureInfo.InvariantCulture), true);
        //}

        //public override void WriteLine(decimal value)
        //{
        //    WriteStuff(value.ToString(CultureInfo.InvariantCulture), true);
        //}

        //public override void WriteLine(object value)
        //{
        //    WriteStuff(value.ToString(), true);
        //}

        //public override void WriteLine(string format, object arg0)
        //{
        //    WriteStuff(string.Format(format, arg0), true);
        //}

        //public override void WriteLine(string format, object arg0, object arg1)
        //{
        //    WriteStuff(string.Format(format, arg0, arg1), true);
        //}

        //public override void WriteLine(string format, object arg0, object arg1, object arg2)
        //{
        //    WriteStuff(string.Format(format, arg0, arg1, arg2), true);
        //}

        //public override void WriteLine(string format, params object[] arg)
        //{
        //    WriteStuff(string.Format(format, arg), true);
        //}

        //public override async Task WriteLineAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task WriteLineAsync(char value)
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task WriteLineAsync(string value)
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task WriteLineAsync(char[] buffer, int index, int count)
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task WriteAsync(char value)
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task WriteAsync(string value)
        //{
        //    throw new NotImplementedException();
        //}

        //public override async Task WriteAsync(char[] buffer, int index, int count)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Write(string value)
        //{
        //    WriteStuff(value, false);
        //}

        ///////////////////////////////////////////////////////////////////////////
        public new static readonly TextWriter Null;
        protected new char[] CoreNewLine;

        //public override IFormatProvider FormatProvider
        //{
        //    get { return _consoleWriter.FormatProvider; }
        //}

        public override string NewLine
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public static TextWriter Synchronized(TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override Task FlushAsync()
        {
            throw new NotImplementedException();
        }

        public override void Write(string value)
        {
            throw new NotImplementedException();
        }

        public override void Write(decimal value)
        {
            throw new NotImplementedException();
        }

        public override void Write(double value)
        {
            throw new NotImplementedException();
        }

        public override void Write(float value)
        {
            throw new NotImplementedException();
        }

        public override void Write(object value)
        {
            throw new NotImplementedException();
        }

        public override void Write(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(uint value)
        {
            throw new NotImplementedException();
        }

        public override void Write(int value)
        {
            throw new NotImplementedException();
        }

        public override void Write(bool value)
        {
            throw new NotImplementedException();
        }

        public override void Write(ulong value)
        {
            throw new NotImplementedException();
        }

        public override void Write(char[] buffer)
        {
            throw new NotImplementedException();
        }

        public override void Write(char value)
        {
            throw new NotImplementedException();
        }

        public override void Write(string format, object arg0)
        {
            throw new NotImplementedException();
        }

        public override void Write(string format, params object[] arg)
        {
            throw new NotImplementedException();
        }

        public override void Write(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(char value)
        {
            throw new NotImplementedException();
        }

        public Task WriteAsync(char[] buffer)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(string value)
        {
            throw new NotImplementedException();
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }
        public override void WriteLine()
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(object value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string value)
        {
            WriteStuff(value, true);
        }

        public override void WriteLine(decimal value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(int value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(float value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(char value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(double value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(bool value)
        {
            throw new NotImplementedException();
        }
        public override void WriteLine(uint value)
        {
            throw new NotImplementedException();
        }
        public override void WriteLine(long value)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(char[] buffer)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(ulong value)
        {
            throw new NotImplementedException();
        }
        public override void WriteLine(string format, object arg0)
        {
            throw new NotImplementedException();
        }
        public override void WriteLine(string format, params object[] arg)
        {
            throw new NotImplementedException();
        }
        public override void WriteLine(string format, object arg0, object arg1)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        public override void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            throw new NotImplementedException();
        }

        public override Task WriteLineAsync()
        {
            throw new NotImplementedException();
        }

        public override Task WriteLineAsync(char value)
        {
            throw new NotImplementedException();
        }

        public override Task WriteLineAsync(string value)
        {
            throw new NotImplementedException();
        }

        public Task WriteLineAsync(char[] buffer)
        {
            throw new NotImplementedException();
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }

        private void WriteStuff(string value, bool lineFeed)
        {
            var outputLevel = OutputLevel.Default;

            //if (_location != null)
            //{
            //    lock (_syncRoot)
            //    {
            //        //Continue to write on a known position
            //        var lns = 0;
            //        while ((lns * -System.Console.BufferWidth) + _location.Left + value.Length >= System.Console.BufferWidth)
            //        {
            //            lns++;
            //            _console.Output(new WriteEventArgs(string.Empty, outputLevel));
            //        }

            //        var pos = new Location(System.Console.CursorLeft, System.Console.CursorTop);

            //        while (_location.Left > System.Console.BufferWidth)
            //        {
            //            _location = new Location(_location.Left - System.Console.BufferWidth, _location.Top + 1);
            //        }

            //        System.Console.CursorTop = _location.Top;
            //        System.Console.CursorLeft = _location.Left;

            //        _consoleWriter.Write(value);

            //        _location = new Location(System.Console.CursorLeft, System.Console.CursorTop);

            //        System.Console.CursorTop = pos.Top;
            //        System.Console.CursorLeft = pos.Left;
            //    }

            //    if (lineFeed)
            //        _location = null;
            //}
            //else if (!lineFeed)
            //{
            //    if (_location == null)
            //    {
            //        //First time a write arrives. assign a new line and remember position
            //        _location = new Location(value.Length, System.Console.CursorTop);
            //        _console.Output(new WriteEventArgs(value, outputLevel));
            //    }
            //}
            //else
            //{
                //There is no previous location, and there is linefeed. This is a normal write line action.
                _console.Output(new WriteEventArgs(value, outputLevel));
            //}
        }
    }
}