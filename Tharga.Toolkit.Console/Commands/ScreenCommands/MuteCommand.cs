using System;
using Tharga.Toolkit.Console.Commands.Base;

namespace Tharga.Toolkit.Console.Commands.ScreenCommands
{
    internal class MuteCommand : ActionCommandBase
    {
        //private readonly IConsole _console;

        public MuteCommand()
            : base("mute", "Mute output.")
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