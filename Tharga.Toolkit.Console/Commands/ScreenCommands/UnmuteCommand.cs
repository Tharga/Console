using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class UnmuteCommand : ActionCommandBase
    {
        private readonly IConsole _console;

        public UnmuteCommand(IConsole console)
            : base(console, new[] { "unmute" }, "Unmute output.", false)
        {
            _console = console;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var type = QueryParam("Type", GetParam(paramList, index++), EnumExtensions.GetValues<OutputLevel>().ToDictionary(x => x, x => x.ToString()));

            ((SystemConsoleBase)_console).Unmute(type);

            return true;
        }
    }
}