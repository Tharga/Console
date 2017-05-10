using System.Collections.Generic;
using System.Diagnostics;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Helpers;

namespace Tharga.Toolkit.Console.Commands
{
    internal class CmdCommand : ActionCommandBase
    {
        public CmdCommand()
            : base("cmd", "Command shell commands.", true)
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

        public override void Invoke(params string[] param)
        {
            var data = QueryParam<string>("Input", param.ToParamString());

            var cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.Arguments = $"/C {data}";
            cmd.Start();

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            OutputDefault(cmd.StandardOutput.ReadToEnd());
        }
    }
}