using System;
using Tharga.Console.Commands.Base;

namespace Tharga.Console.Commands.ScreenCommands
{
    internal class MuteCommand : ActionCommandBase
    {
        //private readonly IConsole _console;

        public MuteCommand()
            : base("mute", "Mute output.", false)
        {
        }

        public override void Invoke(string[] param)
        {
            throw new NotImplementedException("Fire event that mutes the console.");

            //var type = QueryParam("Type", param, EnumExtensions.GetValues<OutputLevel>().ToDictionary(x => x, x => x.ToString()));

            //((ConsoleBase)_console).Mute(type);
        }
    }
}