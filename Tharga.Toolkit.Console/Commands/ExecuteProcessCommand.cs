using System.Collections.Generic;
using System.Diagnostics;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;

namespace Tharga.Toolkit.Console.Commands
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

        public override void Invoke(params string[] param)
        {
            var data = QueryParam<string>("Input", param.ToParamString());
            Process.Start(data);
        }
    }
}