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
    }
}