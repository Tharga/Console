using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Console.Consoles.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;
using Tharga.Console.Interfaces;

namespace Tharga.Console.Consoles
{
    public class EventConsole : ConsoleBase
    {
        public event EventHandler<OutputEventArgs> OutputEv;

        public EventConsole()
            : base(new ConsoleManager(System.Console.Out, System.Console.In))
        {
        }

        public override void Output(IOutput output)
        {
            OutputEv?.Invoke(this,new OutputEventArgs(output.Message, output.OutputLevel));
        }

        public override void Attach(IRootCommand command)
        {
            base.Attach(command);
        }

        public override void Initiate(IEnumerable<string> commandKeys)
        {
            base.Initiate(commandKeys);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override ConsoleKeyInfo ReadKey(CancellationToken cancellationToken)
        {
            return base.ReadKey(cancellationToken);
        }

        protected override void OnLinesInsertedEvent(int lineCount)
        {
            base.OnLinesInsertedEvent(lineCount);
        }

        protected internal override Location WriteLineEx(string value, OutputLevel level)
        {
            OutputEv?.Invoke(this, new OutputEventArgs(value, level));
            return new Location(0, 0);
        }

        protected override void OnPushBufferDownEvent(int lineCount)
        {
            base.OnPushBufferDownEvent(lineCount);
        }

        protected override void OnKeyReadEvent(KeyReadEventArgs e)
        {
            base.OnKeyReadEvent(e);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        internal override void OnLineWrittenEvent(LineWrittenEventArgs e)
        {
            base.OnLineWrittenEvent(e);
        }
    }
}