using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class PoshCommand : ActionCommandBase
    {
        private readonly IConsole _console;

        public PoshCommand(IConsole console)
            : base(console, new [] {"posh", "ps"}, "Powershell commands.", true)
        {
            _console = console;
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
            _console.WriteLine(cmd.StandardOutput.ReadToEnd(), OutputLevel.Default, null, null);

            return true;
        }
    }
}