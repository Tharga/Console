using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal class TextWriterInterceptor : TextWriter
    {
        public new static readonly TextWriter Null;
        private readonly IConsole _console;
        private readonly IConsoleManager _consoleWriter;
        protected new char[] CoreNewLine;

        public TextWriterInterceptor(IConsoleManager consoleWriter, IConsole console)
        {
            _consoleWriter = consoleWriter;
            _console = console;
            System.Console.SetOut(this);
        }

        public override Encoding Encoding => _consoleWriter.Encoding;

        public override string NewLine { get; set; }

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

        public override void Close()
        {
        }

        public override void Flush()
        {
        }

        public override Task FlushAsync()
        {
            return Task.Run(() => { Flush(); });
        }

        public override void Write(string value)
        {
            WriteStuff(value, false);
        }

        public override void Write(decimal value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(double value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(float value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(object value)
        {
            WriteStuff(value.ToString(), false);
        }

        public override void Write(long value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(uint value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(int value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(bool value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(ulong value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(char[] buffer)
        {
            WriteStuff(new string(buffer), false);
        }

        public override void Write(char value)
        {
            WriteStuff(value.ToString(CultureInfo.InvariantCulture), false);
        }

        public override void Write(string format, object arg0)
        {
            WriteStuff(string.Format(format, arg0), false);
        }

        public override void Write(string format, params object[] arg)
        {
            WriteStuff(string.Format(format, arg), false);
        }

        public override void Write(string format, object arg0, object arg1)
        {
            WriteStuff(string.Format(format, arg0, arg1), false);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            WriteStuff(new string(buffer, index, count), false);
        }

        public override void Write(string format, object arg0, object arg1, object arg2)
        {
            WriteStuff(string.Format(format, arg0, arg1, arg2), false);
        }

        public override Task WriteAsync(char value)
        {
            return Task.Run(() => { Write(value); });
        }

        public override Task WriteAsync(string value)
        {
            return Task.Run(() => { Write(value); });
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            return Task.Run(() => { Write(buffer, index, count); });
        }

        public override Task WriteLineAsync()
        {
            return Task.Run(() => { WriteLine(); });
        }

        public override Task WriteLineAsync(char value)
        {
            return Task.Run(() => { WriteLine(value); });
        }

        public override Task WriteLineAsync(string value)
        {
            return Task.Run(() => { WriteLine(value); });
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            return Task.Run(() => { WriteLine(buffer, index, count); });
        }

        private void WriteStuff(string value, bool lineFeed)
        {
            _console.Output(new WriteEventArgs(value, lineFeed: lineFeed));
        }
    }
}