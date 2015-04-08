using System;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ExitCommand : ActionCommandBase
    {
        private Action _stopAction;

        internal ExitCommand(IConsole console, Action stopAction)
            : base(console, "exit", "Exit from the console.")
        {
            _stopAction = stopAction;
        }

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