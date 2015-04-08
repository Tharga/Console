using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Toolkit.Console.Command
{
    internal class ExecuteFileCommand : ActionCommandBase
    {
        private readonly RootCommandBase _rootCommand;

        internal ExecuteFileCommand(IConsole console, RootCommandBase rootCommand)
            : base(console, "file", "Execute file")
        {
            _rootCommand = rootCommand;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var filename = QueryParam<string>("Filename", GetParam(paramList, 0));

            if (!System.IO.File.Exists(filename))
            {
                OutputError("File {0} does not exist", filename);
                return false;
            }

            var fileLines = System.IO.File.ReadAllLines(filename);

            OutputInformation("There are {0} commands in file {1}.", fileLines.Length, filename);

            var index = 0;
            foreach (var line in fileLines)
            {
                OutputInformation("Command {0}: {1}", ++index, line);
                if (!line.StartsWith("#"))
                {
                    var success = _rootCommand.ExecuteCommand(line);
                    if (!success)
                    {
                        OutputError("Terminating command chain.");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}