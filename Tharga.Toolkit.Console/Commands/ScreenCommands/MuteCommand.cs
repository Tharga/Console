using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Commands.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class MuteCommand : ActionCommandBase
    {
        private readonly IConsole _console;

        public MuteCommand(IConsole console)
            : base(console, new [] { "mute"}, "Mute output.", false)
        {
            _console = console;
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var type = QueryParam("Type", GetParam(paramList, index++), EnumExtensions.GetValues<OutputLevel>().ToDictionary(x => x, x => x.ToString()));

            ((SystemConsoleBase)_console).Mute(type);

            return true;
        }
    }
}