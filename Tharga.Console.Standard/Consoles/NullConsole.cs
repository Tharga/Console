using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Consoles
{
    public class NullConsole : ConsoleBase
    {
        public NullConsole()
            : base(new NullConsoleManager())
        {
        }

        public override void Output(IOutput output)
        {
        }

        protected internal override Location WriteLineEx(string value, OutputLevel level)
        {
            return new Location(0, 0);
        }

        internal override void OnLineWrittenEvent(LineWrittenEventArgs e)
        {
        }

        protected override void OnKeyReadEvent(KeyReadEventArgs e)
        {
        }

        protected override void OnLinesInsertedEvent(int lineCount)
        {
        }

        protected override void OnPushBufferDownEvent(int lineCount)
        {
        }
    }
}