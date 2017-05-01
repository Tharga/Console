using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands
{
    internal class ExecuteSleepCommand : ActionCommandBase
    {
        internal ExecuteSleepCommand()
            : base(new [] { "sleep" }, "Sleep a number of milliseconds.", false)
        {
        }

        public override IEnumerable<HelpLine> HelpText { get { yield return new HelpLine("Have the application sleep for a period of time. The value is specified in milliseconds."); } }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var millisecondsTimeout = QueryParam<int>("Time", GetParam(paramList, 0));

            System.Threading.Thread.Sleep(millisecondsTimeout);

            return true;
        }
    }
}