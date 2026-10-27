using System.Collections.Generic;
using Tharga.Console.Commands.Base;
using Tharga.Console.Entities;
using Tharga.Console.Helpers;

namespace Tharga.Console.Commands
{
    internal class CmdCommand : ActionCommandBase
    {
        public CmdCommand()
            : base("cmd", "Command shell commands.", false)
        {
            AddName("command");
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Execute command line commands.");
                yield return new HelpLine("Ex.");
                yield return new HelpLine("cmd dir");
            }
        }

        public override void Invoke(string[] param)
        {
            var data = QueryParam<string>("Input", param.ToParamString());

            OutputDefault(ProcessRunner.Run("cmd.exe", $"/C {data}"));
        }
    }
}