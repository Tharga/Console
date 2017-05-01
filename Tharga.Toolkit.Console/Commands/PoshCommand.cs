using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    internal class PoshCommand : ActionCommandBase
    {
        public PoshCommand()
            : base(new [] {"posh", "ps"}, "Powershell commands.", true)
        {
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine("Execute powershell commands.");
                yield return new HelpLine("Ex.");
                yield return new HelpLine("posh ls");
            }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var input = QueryParam<string>("Input", paramList);

            var cmd = new Process();
            cmd.StartInfo.FileName = "powershell";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.Arguments = $"{input}";
            cmd.Start();

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            OutputDefault(cmd.StandardOutput.ReadToEnd());

            return true;
        }
    }
}