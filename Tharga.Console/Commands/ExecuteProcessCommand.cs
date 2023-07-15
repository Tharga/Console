using System.Collections.Generic;
using System.Diagnostics;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;

namespace Tharga.Console.Commands
{
    internal class ExecuteProcessCommand : ActionCommandBase
    {
        public ExecuteProcessCommand()
            : base("run", "Execute command.", true)
        {
            AddName("exe");
            AddName("execute");
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

        public override void Invoke(string[] param)
        {
            var data = QueryParam<string>("Input", param.ToParamString());
            Process.Start(data);
        }
    }
}