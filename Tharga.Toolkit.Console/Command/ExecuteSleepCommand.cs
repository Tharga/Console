using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ExecuteSleepCommand : ActionCommandBase
    {
        internal ExecuteSleepCommand(IConsole console)
            : base(console, "sleep", "Sleep a number of milliseconds")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var millisecondsTimeout = QueryParam<int>("Time", GetParam(paramList, 0));

            System.Threading.Thread.Sleep(millisecondsTimeout);

            return true;
        }
    }
}