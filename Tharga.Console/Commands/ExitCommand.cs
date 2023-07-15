using System;
using System.Collections.Generic;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;

namespace Tharga.Console.Commands
{
    internal class ExitCommand : ActionCommandBase
    {
        private readonly Action _stopAction;

        internal ExitCommand(Action stopAction)
            : base("exit", "Exit from the console.")
        {
            _stopAction = stopAction;
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("This command terminates the application."); } }

        public override void Invoke(string[] param)
        {
            _stopAction();
        }
    }
}