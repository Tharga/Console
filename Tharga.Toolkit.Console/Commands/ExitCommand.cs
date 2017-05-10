using System;
using System.Collections.Generic;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands
{
    internal class ExitCommand : ActionCommandBase
    {
        private readonly Action _stopAction;

        internal ExitCommand(Action stopAction)
            : base(new [] { "exit" }, "Exit from the console.")
        {
            _stopAction = stopAction;
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("This command terminates the application."); } }

        public override void Invoke(params string[] input)
        {
            _stopAction();
        }
    }
}