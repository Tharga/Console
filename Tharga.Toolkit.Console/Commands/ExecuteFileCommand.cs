using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands
{
    internal class ExecuteFileCommand : ActionCommandBase
    {
        private readonly RootCommandBase _rootCommand;

        internal ExecuteFileCommand(RootCommandBase rootCommand)
            : base(new []{ "file" }, "Execute script file.", false)
        {
            _rootCommand = rootCommand;
        }

        public override IEnumerable<HelpLine> HelpText
        {
            get
            {
                yield return new HelpLine(string.Empty);
                yield return new HelpLine("Usage:");
                yield return new HelpLine("exec file [FileName]");
                yield return new HelpLine(string.Empty);
                yield return new HelpLine("Each line read from the file is executed as a command, one by one.");
                yield return new HelpLine("A line marked with # will be ignored.");
            }
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var filename = QueryParam<string>("Filename", GetParam(paramList, 0));

            if (!System.IO.File.Exists(filename))
            {
                OutputError($"File {filename} does not exist.");
                return false;
            }

            var fileLines = System.IO.File.ReadAllLines(filename);

            OutputInformation($"There are {fileLines.Length} commands in file {filename}.");

            var index = 0;
            foreach (var line in fileLines)
            {
                OutputInformation($"Command {++index}: {line}");
                if (!line.StartsWith("#"))
                {
                    var success = _rootCommand.Execute(line);
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