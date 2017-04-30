using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    internal class CmdCommand : ActionCommandBase
    {
        private readonly IConsole _console;

        public CmdCommand(IConsole console)
            : base(console, new [] {"cmd", "command"}, "Command shell commands.", true)
        {
            _console = console;
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

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var input = QueryParam<string>("Input", paramList);

            var cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.Arguments = $"/C {input}";
            cmd.Start();

            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            _console.WriteLine(cmd.StandardOutput.ReadToEnd(), OutputLevel.Default, null, null);

            return true;
        }
    }
}