using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ExecuteProcessCommand : ActionCommandBase
    {
        private readonly IConsole _console;

        public ExecuteProcessCommand(IConsole console)
            : base(console, new [] { "run", "exe", "execute" }, "Execute command.", true)
        {
            _console = console;
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Execute any command.");
                yield return new HelpLine("Ex.");
                yield return new HelpLine("run explorer");
            }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var input = QueryParam<string>("Input", paramList);

            Process.Start(input);
            
            return true;
        }
    }
}