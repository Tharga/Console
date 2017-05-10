using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;

namespace Tharga.Toolkit.Console.Commands
{
    internal class ExecuteProcessCommand : ActionCommandBase
    {
        public ExecuteProcessCommand()
            : base(new [] { "run", "exe", "execute" }, "Execute command.", true)
        {
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

        public override void Invoke(params string[] input)
        {
            InvokeAsync(input.ToParamString()).Wait();
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var input = QueryParam<string>("Input", paramList);

            Process.Start(input);

            return true;
        }
    }
}