using System;
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

        protected internal override Location WriteLineEx(string value, OutputLevel level)
        {
            OutputEv?.Invoke(this, new OutputEventArgs(value, level));
            return new Location(0, 0);
        }
    }
}