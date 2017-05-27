using System;
using System.Collections.Generic;
using System.Threading;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Consoles
{
    public class ActionConsole : ConsoleBase
    {
        private readonly Action<IActionConsoleOutput> _action;

        public ActionConsole(Action<IActionConsoleOutput> action, IConsoleConfiguration consoleConfiguration = null)
            : base(new ConsoleManager(System.Console.Out, System.Console.In))
        {
            _action = action;
        }

        public override void Output(IOutput output)
        {
            _action(new ActionConsoleOutput(output.Message, output.OutputLevel));
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

        public override ConsoleKeyInfo ReadKey()
        {
            return base.ReadKey();
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
            _action(new ActionConsoleOutput(value, level));
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