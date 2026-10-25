using System.IO;

namespace Tharga.Console.Helpers
{
    internal sealed class TextReaderInterceptor : TextReader
    {
        public TextReaderInterceptor()
        {
            System.Console.SetIn(this);
        }
    }
}