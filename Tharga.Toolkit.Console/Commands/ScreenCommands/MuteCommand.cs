using System;
using System.Linq;
using System.Threading.Tasks;
using Tharga.Toolkit.Console.Commands.Base;
using Tharga.Toolkit.Console.Consoles.Base;
using Tharga.Toolkit.Console.Entities;
using Tharga.Toolkit.Console.Interfaces;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class MuteCommand : ActionCommandBase
    {
        private readonly IConsole _console;

        public MuteCommand()
            : base(new [] { "mute"}, "Mute output.", false)
        {
        }

        public override void Invoke(params string[] param)
        {
            var type = QueryParam("Type", GetNextParam(param), EnumExtensions.GetValues<OutputLevel>().ToDictionary(x => x, x => x.ToString()));

            throw new NotImplementedException("Fire event that mutes the console.");
            ((ConsoleBase)_console).Mute(type);
        }
    }
}