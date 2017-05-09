using System.IO;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Helpers
{
    internal sealed class TextReaderInterceptor : TextReader
    {
        private readonly IConsoleManager _consoleWriter;
        private readonly IConsole _console;

        public TextReaderInterceptor(IConsoleManager consoleWriter, IConsole console)
        {
            _consoleWriter = consoleWriter;
            _console = console;
            System.Console.SetIn(this);
        }

        //public new static TextReader Synchronized(TextReader reader)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void Close()
        //{
        //    throw new NotImplementedException();
        //}

        //public void Dispose()
        //{
        //}

        //public int Peek()
        //{
        //    throw new NotImplementedException();
        //}

        //public int Read()
        //{
        //    throw new NotImplementedException();
        //}

        //public int Read(char[] buffer, int index, int count)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> ReadAsync(char[] buffer, int index, int count)
        //{
        //    throw new NotImplementedException();
        //}

        //public int ReadBlock(char[] buffer, int index, int count)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<int> ReadBlockAsync(char[] buffer, int index, int count)
        //{
        //    throw new NotImplementedException();
        //}

        //public string ReadLine()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<string> ReadLineAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //public string ReadToEnd()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<string> ReadToEndAsync()
        //{
        //    throw new NotImplementedException();
        //}

        //private void Dispose(bool disposing)
        //{
        //    throw new NotImplementedException();
        //}
    }
}