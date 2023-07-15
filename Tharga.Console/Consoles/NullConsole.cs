using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles
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

        public override void Attach(IRootCommand command)
        {
            base.Attach(command);
        }

        public override void Initiate(IEnumerable<string> commandKeys)
        {
            base.Initiate(commandKeys);
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

        public override ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return base.ReadKey(cancellationToken);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}