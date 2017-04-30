using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    internal class ExitCommand : ActionCommandBase
    {
        private Action _stopAction;

        internal ExitCommand(IConsole console, Action stopAction)
            : base(console, new [] { "exit" }, "Exit from the console.", false)
        {
            _stopAction = stopAction;
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("This command terminates the application."); } }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            _stopAction();
            return true;
        }

        protected internal void SetStopAction(Action stopAction)
        {
            _stopAction = stopAction;
        }
    }
}